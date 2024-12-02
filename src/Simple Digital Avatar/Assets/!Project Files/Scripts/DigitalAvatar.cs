using System.Threading.Tasks;
using UnityEngine;

public class DigitalAvatar : MonoBehaviour
{
    [SerializeField] private TextDisplay textDisplay;

    private IAudioRecorder _audioRecorder;
    private ISpeechRecognizer _speechRecognizer;
    private IConversationGeneration _conversationGeneration;

    private void Start()
    {
        var serviceLocator = ServiceLocator.Instance;
        _audioRecorder = serviceLocator.Get<IAudioRecorder>();
        _speechRecognizer = serviceLocator.Get<ISpeechRecognizer>();
        _conversationGeneration = serviceLocator.Get<IConversationGeneration>();

        _audioRecorder.OnAudioDataReceived += OnAudioDataReceived;
        _conversationGeneration.OnMessageReceived += OnMessageReceived;
    }

    private async void OnAudioDataReceived(float[] audioData)
    {
        var text = await RecognizeSpeechAsync(audioData);

        await _conversationGeneration.StartGeneration(text);
    }

    private async Task<string> RecognizeSpeechAsync(float[] audioData)
    {
        if (audioData == null) return string.Empty;

        return await _speechRecognizer.GetTextAsync(audioData);
    }

    private void OnMessageReceived(string message)
    {
        textDisplay.SetText(message);
    }
}