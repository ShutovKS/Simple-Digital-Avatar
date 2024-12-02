using LLMUnity;
using UnityEngine;
using Whisper;

public class DependencyInstaller : MonoBehaviour
{
    [SerializeField] private WhisperManager whisperManager;
    [SerializeField] private LLMCharacter llmCharacter;
    
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
    }
}