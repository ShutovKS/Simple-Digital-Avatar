using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MicrophoneButton : MonoBehaviour
{
    public event Action OnMicrophoneToggle;

    private Button _button;

    public void SetIsInteractable(bool isInteractable)
    {
        _button.interactable = isInteractable;
    }

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(ToggleMicrophone);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(ToggleMicrophone);
    }

    private void ToggleMicrophone() =>
        OnMicrophoneToggle?.Invoke();
}