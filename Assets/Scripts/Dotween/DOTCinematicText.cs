using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Cinematic
{
    public class DOTCinematicText : MonoBehaviour
    {
        private TextMeshProUGUI textMesh;
        public Action OnTextAnimationComplete;

        private void Awake()
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }

        public void AnimateText(string originalText, float fadeInDuration = 0.5f)
        {
            if (textMesh == null || originalText == string.Empty)
            {
                return;
            }

            textMesh.text = "";

            try
            {
                textMesh.text = originalText;
                Color originalColor = textMesh.color;
                textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
                textMesh.DOFade(1f, fadeInDuration).OnComplete(() =>
                {
                    OnTextAnimationComplete?.Invoke();
                }).Play();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
        
        private void OnDestroy()
        {
            textMesh.text = "";
        }
    }
}