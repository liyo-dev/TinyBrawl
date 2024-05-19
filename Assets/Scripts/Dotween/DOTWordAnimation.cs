using TMPro;
using UnityEngine;
using DG.Tweening;

public class DOTWordAnimation : MonoBehaviour
{
    public float jumpHeight = 1f; // Altura del salto
    public float jumpDuration = 0.5f; // Duración de la animación

    private TextMeshProUGUI textComponent;

    private void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        AnimateWord();
    }

    private void AnimateWord()
    {
        foreach (var character in textComponent.text)
        {
            var text = character.ToString();
            var letterTransform = textComponent.transform.Find(text);
            if (letterTransform != null)
            {
                var originalPosition = letterTransform.localPosition;
                letterTransform.DOLocalJump(originalPosition, jumpHeight, 1, jumpDuration);
            }
        }
    }
}