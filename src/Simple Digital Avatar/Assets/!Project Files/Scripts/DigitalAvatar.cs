using System.Threading.Tasks;
using UnityEngine;

public class DigitalAvatar : MonoBehaviour
{
    [SerializeField] private TextDisplay textDisplay;

    private IAudioRecorder _audioRecorder;
    private ISpeechRecognizer _speechRecognizer;

    private void Start()
    {
        var serviceLocator = ServiceLocator.Instance;
        _audioRecorder = serviceLocator.Get<IAudioRecorder>();
        _speechRecognizer = serviceLocator.Get<ISpeechRecognizer>();

        _audioRecorder.OnAudioDataReceived += OnAudioDataReceived;
    }

    private async void OnAudioDataReceived(float[] audioData)
    {
        var text = await RecognizeSpeechAsync(audioData);
        textDisplay.SetText(text);
    }

    private async Task<string> RecognizeSpeechAsync(float[] audioData)
    {
        if (audioData == null) return string.Empty;

        return await _speechRecognizer.GetTextAsync(audioData);
    }
}