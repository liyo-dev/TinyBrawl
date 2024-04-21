using UnityEngine;

public class MyGameManager : MonoBehaviour
{
    [SerializeField] private MoveSprite _moveSprite;
    [SerializeField] private GameObject _cast;
    
    public void MiniGame1()
    {
        _cast.SetActive(true);
        _moveSprite.DoStart();
    } 
}
