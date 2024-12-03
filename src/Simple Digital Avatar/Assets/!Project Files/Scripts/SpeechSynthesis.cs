using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Piper;
using Piper.Native;
using Unity.Sentis;
using UnityEngine;

public interface ISpeechSynthesis
{
    Task<AudioClip> TextToSpeech(string text);
}

public class SpeechSynthesisWithPiper : ISpeechSynthesis, IDisposable
{
    private const BackendType BACKEND = BackendType.CPU;
    private readonly string _voice;
    private readonly int _sampleRate;

    private readonly Worker _worker;

    public SpeechSynthesisWithPiper(ModelAsset model, string language = "en-us", int sampleRate = 22050)
    {
        _voice = language;
        _sampleRate = sampleRate;

        var espeakPath = Path.Combine(Application.streamingAssetsPath, "espeak-ng-data");
        PiperWrapper.InitPiper(espeakPath);

        var runtimeModel = ModelLoader.Load(model);

        _worker = new Worker(runtimeModel, BACKEND);
    }

    public async Task<AudioClip> TextToSpeech(string text)
    {
        var phonemes = PiperWrapper.ProcessText(text, _voice);

        var inputLengthsShape = new TensorShape(1);
        var scalesShape = new TensorShape(3);
        using var scalesTensor = new Tensor<float>(scalesShape, new[] { 0.667f, 1f, 0.8f });

        var audioBuffer = new List<float>();
        foreach (var sentence in phonemes.Sentences)
        {
            var inputPhonemes = sentence.PhonemesIds;
            var inputShape = new TensorShape(1, inputPhonemes.Length);
            using var inputTensor = new Tensor<int>(inputShape, inputPhonemes);
            using var inputLengthsTensor = new Tensor<int>(inputLengthsShape, new[] { inputPhonemes.Length });

            _worker.SetInput("input", inputTensor);
            _worker.SetInput("input_lengths", inputLengthsTensor);
            _worker.SetInput("scales", scalesTensor);

            _worker.Schedule();

            using var outputTensor = _worker.PeekOutput() as Tensor<float>;
            await outputTensor.ReadbackAndCloneAsync();

            var output = outputTensor.AsReadOnlySpan().ToArray();
            audioBuffer.AddRange(output);
        }

        return CreateAudioClip(audioBuffer);
    }

    private AudioClip CreateAudioClip(List<float> audioBuffer)
    {
        var audioClip = AudioClip.Create("piper_tts", audioBuffer.Count, 1, _sampleRate, false);
        audioClip.SetData(audioBuffer.ToArray(), 0);
        return audioClip;
    }

    public void Dispose()
    {
        PiperWrapper.FreePiper();
        _worker?.Dispose();
    }
}

namespace Piper
{
    public struct PiperProcessedSentence
    {
        public readonly int[] PhonemesIds;

        public unsafe PiperProcessedSentence(PiperProcessedSentenceNative native)
        {
            var len = (uint)native.length;
            PhonemesIds = new int[len];
            for (var i = 0; i < len; i++)
            {
                PhonemesIds[i] = (int)native.phonemesIds[i];
            }
        }
    };


    public class PiperProcessedText
    {
        public readonly PiperProcessedSentence[] Sentences;

        public unsafe PiperProcessedText(PiperProcessedTextNative native)
        {
            var len = (uint)native.sentencesCount;
            Sentences = new PiperProcessedSentence[len];
            for (var i = 0; i < len; i++)
            {
                var nativeSentence = native.sentences[i];

                Sentences[i] = new PiperProcessedSentence(nativeSentence);
            }
        }
    }

    public static class PiperWrapper
    {
        public static bool InitPiper(string datapath)
        {
            if (!Directory.Exists(datapath))
            {
                Debug.LogError($"Provided espeak data path \"{datapath}\" doesn't exist!");
                return false;
            }

            var code = PiperNative.init_piper(datapath);
            if (code < 0)
            {
                Debug.LogError($"Failed to init Piper with code: {code}");
                return false;
            }

            return true;
        }

        public static PiperProcessedText ProcessText(string text, string voice)
        {
            var code = PiperNative.process_text(text, voice);
            if (code < 0)
            {
                Debug.LogError($"Failed to get phonemes with code: {code}");
                return null;
            }

            var nativePhonemes = PiperNative.get_processed_text();
            return new PiperProcessedText(nativePhonemes);
        }

        public static void FreePiper()
        {
            PiperNative.free_piper();
        }
    }
}

namespace Piper.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct PiperProcessedSentenceNative
    {
        public long* phonemesIds;
        public UIntPtr length;
    }


    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct PiperProcessedTextNative
    {
        public PiperProcessedSentenceNative* sentences;
        public UIntPtr sentencesCount;
    }

    public static unsafe class PiperNative
    {
        private const string LibraryName = "piper_phonemize";

        [DllImport(LibraryName)]
        public static extern int init_piper(string dataPath);

        [DllImport(LibraryName)]
        public static extern int process_text(string text, string voice);

        [DllImport(LibraryName)]
        public static extern PiperProcessedTextNative get_processed_text();

        [DllImport(LibraryName)]
        public static extern void free_piper();
    }
}