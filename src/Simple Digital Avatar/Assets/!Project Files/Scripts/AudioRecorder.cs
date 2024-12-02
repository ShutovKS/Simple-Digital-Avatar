using System;
using UnityEngine;
using Object = UnityEngine.Object;

public interface IAudioRecorder
{
    event Action<float[]> OnAudioDataReceived;

    void StartRecording();
    void StopRecording();
    float[] ReadAudioData();

    void SetMicrophoneIndex(int index);
}

public class AudioRecorder : IAudioRecorder
{
    public event Action<float[]> OnAudioDataReceived;

    private int _microphoneIndex;
    private AudioClip _microphoneClip;
    private int _position1, _position2;

    public void SetMicrophoneIndex(int index)
    {
        _microphoneIndex = index;
    }

    public void StartRecording()
    {
        if (_microphoneClip != null)
        {
            Object.Destroy(_microphoneClip);
        }

        _microphoneClip = Microphone.Start(Microphone.devices[_microphoneIndex], true, 3500, 16000);
        _position1 = 0;
    }

    public void StopRecording()
    {
        if (_microphoneClip == null) return;

        OnAudioDataReceived?.Invoke(GetAudioData());

        Microphone.End(Microphone.devices[_microphoneIndex]);

        _microphoneClip = null;

        _position1 = 0;
        _position2 = 0;
    }

    public float[] ReadAudioData()
    {
        if (_microphoneClip == null || !Microphone.IsRecording(Microphone.devices[_microphoneIndex])) return null;

        _position2 = Microphone.GetPosition(Microphone.devices[_microphoneIndex]);

        if (_position2 <= _position1) return null;

        var audioData = new float[_position2 - _position1];
        _microphoneClip.GetData(audioData, _position1);

        _position1 = _position2;

        return audioData;
    }

    private float[] GetAudioData()
    {
        _position2 = Microphone.GetPosition(Microphone.devices[_microphoneIndex]);

        if (_position2 <= 0) return null;

        var audioData = new float[_position2];
        _microphoneClip.GetData(audioData, 0);
        return audioData;
    }
}