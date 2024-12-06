using Photon.Pun;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class GameFishing : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject optionLeft;
    [SerializeField] private GameObject optionCenter;
    [SerializeField] private GameObject optionRight;
    [SerializeField] private TextMeshProUGUI round_txt;

    private MyGameManager _myGameManager;
    private int activeOption = -2;
    private bool canLocalPlayerPlay = false;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Invoke(nameof(DoStart), 4f);
        }
    }

    public void DoStart()
    {
        activeOption = Random.Range(-1, 2);

        photonView.RPC(nameof(ActivateOption), RpcTarget.All, activeOption);
    }

    [PunRPC]
    public void ActivateOption(int option)
    {
        ResetUI();

        //Guardo la opcion activa en todos los jugadores
        activeOption = option;

        GameObject targetOption = null;

        switch (option)
        {
            case -1:
                targetOption = optionLeft;
                break;
            case 0:
                targetOption = optionCenter;
                break;
            case 1:
                targetOption = optionRight;
                break;
        }

        if (targetOption != null)
        {
            canLocalPlayerPlay = true;

            targetOption.transform.DOShakePosition(2f, new Vector3(0.2f, 0, 0), 10, 90, false, true).Play();
        }
    }

    public void OptionSelectedByPlayer(int option)
    {
        if (!canLocalPlayerPlay) return;

        if (option != activeOption)
        {
            ShowWrongFeedback();

            return;
        }

        photonView.RPC(nameof(AwardPoint), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    public void AwardPoint(int playerId)
    {
        canLocalPlayerPlay = false;

        round_txt.text = $"Player {playerId} scored!";

        if (PhotonNetwork.LocalPlayer.ActorNumber == playerId)
        {
            ShowCorrectFeedback();
        }

        ResetRound();
    }

    private void ResetRound()
    {
        activeOption = -2;

        if (PhotonNetwork.IsMasterClient)
        {
            Invoke(nameof(DoStart), 2f);
        }
    }

    private void ResetUI()
    {
        round_txt.text = "";
    }

    void ShowCorrectFeedback()
    {
        if (_myGameManager == null)
        {
            _myGameManager = FindObjectOfType<MyGameManager>();
        }

        _myGameManager.IncreaseLocalScore(1);
    }

    void ShowWrongFeedback()
    {
        if (_myGameManager == null)
        {
            _myGameManager = FindObjectOfType<MyGameManager>();
        }

        _myGameManager.DecreaseLocalScore(1);
    }
}
