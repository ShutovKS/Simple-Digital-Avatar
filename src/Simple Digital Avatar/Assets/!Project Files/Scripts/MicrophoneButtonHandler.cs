using UnityEngine;

[RequireComponent(typeof(MicrophoneButton))]
public class MicrophoneButtonHandler : MonoBehaviour
{
    private void Awake()
    {
        var microphoneButton = GetComponent<MicrophoneButton>();
        microphoneButton.OnMicrophoneToggle += OnMicrophoneToggle;
    }

    private void OnMicrophoneToggle()
    {
        var serviceLocator = ServiceLocator.Instance;
        var audioRecorder = serviceLocator.Get<IAudioRecorder>();

        if (Microphone.IsRecording(Microphone.devices[0]))
        {
            audioRecorder.StopRecording();
        }
        else
        {
            audioRecorder.StartRecording();
        }
    }
}