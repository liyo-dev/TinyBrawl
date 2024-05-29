using System.Net.Mime;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DOTDialogAnimator : MonoBehaviour
{
    [SerializeField]
    private GameObject dialogImageObj;
    private float fadeInTime = 1f;
    private Image dialogImage;

    private void Start()
    {
        dialogImage = dialogImageObj.GetComponent<Image>();
    }

    public void ShowDialogBox()
    {
        dialogImageObj.SetActive(true);
        dialogImage.color = new Color(dialogImage.color.r, dialogImage.color.g, dialogImage.color.b, 0f);
        dialogImage.DOFade(1f, fadeInTime);
    }

    public void HideDialogBox()
    {
        dialogImage.DOFade(0f, 0.05f).OnComplete(() => {
            dialogImageObj.SetActive(false);
        });
    }
}
