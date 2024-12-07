using LLMUnity;
using Unity.Sentis;
using UnityEngine;
using Whisper;

public class DependencyInstaller : MonoBehaviour
{
    [SerializeField] private WhisperManager whisperManager;
    [SerializeField] private LLMCharacter llmCharacter;
    [SerializeField] private ModelAsset model;
    [SerializeField] private string language;
    [SerializeField] private int sampleRate;

    private void Awake()
    {
        var serviceLocator = ServiceLocator.Instance;

        serviceLocator.Register<WhisperManager>(whisperManager);
        serviceLocator.Register<LLMCharacter>(llmCharacter);

        var audioRecorder = new AudioRecorder();
        serviceLocator.Register<IAudioRecorder>(audioRecorder);

        var speechRecognizer = new SpeechRecognizer();
        serviceLocator.Register<ISpeechRecognizer>(speechRecognizer);

        var conversationGeneration = new ConversationGeneration();
        serviceLocator.Register<IConversationGeneration>(conversationGeneration);

        var speechSynthesis = new SpeechSynthesisWithPiper(model, language, sampleRate);
        serviceLocator.Register<ISpeechSynthesis>(speechSynthesis);
    }
}