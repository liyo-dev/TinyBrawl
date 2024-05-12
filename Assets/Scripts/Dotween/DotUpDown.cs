using UnityEngine;
using DG.Tweening;

public class DotUpDown : MonoBehaviour
{
    public Transform telon; 
    public float alturaLevantado = 5f; 
    public float animationDuration; 


    public void Up()
    {
        telon.DOMoveY(alturaLevantado, animationDuration).Play();
    }


    public void Down()
    {
        telon.DOMoveY(0f, animationDuration).Play();
    }
}