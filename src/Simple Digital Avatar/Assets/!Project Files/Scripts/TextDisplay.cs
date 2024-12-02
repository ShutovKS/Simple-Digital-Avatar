using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TextDisplay : MonoBehaviour
{
    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    public void SetText(string text) => _text.text = text;
}