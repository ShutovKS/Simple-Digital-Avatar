using System.Threading.Tasks;
using UnityEngine;

public class DigitalAvatar : MonoBehaviour
{
    [SerializeField] private MicrophoneButton microphoneButton;
    [SerializeField] private AudioSource audioSource;

    private IAudioRecorder _audioRecorder;
    private ISpeechRecognizer _speechRecognizer;
    private IConversationGeneration _conversationGeneration;
    private ISpeechSynthesis _speechSynthesis;

    private string _generatedText;

    private void Start()
    {
        var serviceLocator = ServiceLocator.Instance;
        _audioRecorder = serviceLocator.Get<IAudioRecorder>();
        _speechRecognizer = serviceLocator.Get<ISpeechRecognizer>();
        _conversationGeneration = serviceLocator.Get<IConversationGeneration>();
        _speechSynthesis = serviceLocator.Get<ISpeechSynthesis>();

        _audioRecorder.OnAudioDataReceived += OnAudioDataReceived;
        _conversationGeneration.OnMessageReceived += OnMessageReceived;
        _conversationGeneration.OnMessageReceivedCompleted += OnMessageReceivedCompleted;
    }

    private async void OnAudioDataReceived(float[] audioData)
    {
        microphoneButton.SetIsInteractable(false);

        var text = await RecognizeSpeechAsync(audioData);

        Debug.Log($"Распознанный текст: {text}");

        await _conversationGeneration.StartGeneration(text);
    }

    private async Task<string> RecognizeSpeechAsync(float[] audioData)
    {
        if (audioData == null) return string.Empty;

        return await _speechRecognizer.GetTextAsync(audioData);
    }

    private void OnMessageReceived(string message)
    {
        _generatedText = message;
    }

    private async void OnMessageReceivedCompleted()
    {
        Debug.Log($"Получено сообщение: {_generatedText}");

        microphoneButton.SetIsInteractable(true);

        var audioClip = await _speechSynthesis.TextToSpeech(_generatedText);
        audioSource.PlayOneShot(audioClip);
    }
}