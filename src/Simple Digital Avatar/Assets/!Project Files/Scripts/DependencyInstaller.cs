using UnityEngine;
using Whisper;

public class DependencyInstaller : MonoBehaviour
{
    [SerializeField] private WhisperManager whisperManager;
    
    private void Awake()
    {
        var serviceLocator = ServiceLocator.Instance;

        serviceLocator.Register<WhisperManager>(whisperManager);
        
        var audioRecorder = new AudioRecorder();
        serviceLocator.Register<IAudioRecorder>(audioRecorder);
        
        var speechRecognizer = new SpeechRecognizer();
        serviceLocator.Register<ISpeechRecognizer>(speechRecognizer);
    }
}