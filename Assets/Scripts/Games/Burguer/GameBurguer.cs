using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;
using Random = UnityEngine.Random;

public class GameBurguer : MonoBehaviourPunCallbacks
{
    public GameObject GoodFeedback;
    public GameObject WrongFeedback;
    public GameObject UpBread;
    public GameObject DownBread;
    public List<GameObject> BurguerOptions;
    public List<Transform> DemoPositionList;
    public List<Transform> PlayerPositionList;
    public List<Transform> RemotePlayerPositionList;
    public List<Transform> OptionsPositionList;
    public float dropDuration = 1.0f; // Duración de la caída de la hamburguesa
    public float dropHeight = 2.0f; // Altura desde la cual cae la hamburguesa
    public float ingredientDelay = .2f; // Retraso entre la instanciación de cada ingrediente


    private List<GameObject> DemoIngredientsList = new List<GameObject>();
    private List<GameObject> PlayerIngredientsList = new List<GameObject>();
    private List<GameObject> RemoteIngredientsList = new List<GameObject>();

    private GameObject DemoDownBread;
    private GameObject PlayerDownBread;
    private GameObject DemoUpBread;
    private GameObject PlayerUpBread;
    private GameObject RemoteUpBread;
    private GameObject RemoteDownBread;
    private bool canLocalPlayerPlay = false;
    private int demoSortingOrder = 2;
    private int playerSortingOrder = 2;
    private int remotePlayerSortingOrder = 2;
    private int demoTurnCount = 0;
    private int playerTurnCount = 0;
    private int remotePlayerTurnCount = 0;
    private bool playerTurnFail = false;
    private bool remotePlayerTurnFail = false;

    private GameObject options;
    private MyGameManager _myGameManager;

    private void Start()
    {
        options = GameObject.FindWithTag("BurguerOptions");
        options.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(StartGameWithDelay(4f));
        }
    }

    private IEnumerator StartGameWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        photonView.RPC(nameof(DoStart), RpcTarget.All);
    }


    [PunRPC]
    public void DoStart()
    {
        options.SetActive(true);

        // Instancia el pan de abajo en la posición DemoDown
        DemoDownBread = Instantiate(DownBread, DemoPositionList[demoTurnCount].position, Quaternion.identity);
        DemoDownBread.GetComponent<SpriteRenderer>().sortingOrder = 1; // Pan de abajo order 1
        DemoDownBread.SetActive(false);

        // Instancia el pan de arriba en la posición DemoDown más la altura de caída
        DemoUpBread = Instantiate(UpBread,
            DemoPositionList[^1].position + Vector3.up * dropHeight,
            Quaternion.identity);
        DemoUpBread.GetComponent<SpriteRenderer>().sortingOrder =
            BurguerOptions.Count + 3; // Pan de arriba order maximo
        DemoUpBread.SetActive(false);

        // Instancia el pan de abajo en la posición PlayerDown
        PlayerDownBread = Instantiate(DownBread, PlayerPositionList[playerTurnCount].position, Quaternion.identity);
        PlayerDownBread.GetComponent<SpriteRenderer>().sortingOrder = 1; // Orden en layer 1
        PlayerDownBread.SetActive(false);

        // Instancia el pan de arriba en la posición PlayerDown más la altura de caída
        PlayerUpBread = Instantiate(UpBread, PlayerPositionList[^1].position + Vector3.up * dropHeight,
            Quaternion.identity);
        PlayerUpBread.GetComponent<SpriteRenderer>().sortingOrder =
            BurguerOptions.Count + 3;
        PlayerUpBread.SetActive(false);

        // Pan de abajo del player remote
        RemoteDownBread = Instantiate(DownBread, RemotePlayerPositionList[remotePlayerTurnCount].position, Quaternion.identity);
        RemoteDownBread.GetComponent<SpriteRenderer>().sortingOrder = 1;
        RemoteDownBread.SetActive(false);

        // Pan de arriba del player remote
        RemoteUpBread = Instantiate(UpBread, RemotePlayerPositionList[^1].position, Quaternion.identity);
        RemoteUpBread.GetComponent<SpriteRenderer>().sortingOrder =
            BurguerOptions.Count + 3;
        RemoteUpBread.SetActive(false);

        //Comienza la demo
        if (PhotonNetwork.IsMasterClient)
        {
            DemoTurn();
        }
    }

    public void DoStop()
    {
        photonView.RPC("GameOver", RpcTarget.Others);
    }

    private void DemoTurn()
    {
        photonView.RPC(nameof(SetDemoDownBread), RpcTarget.All, true);

        int option = Random.Range(0, BurguerOptions.Count);

        photonView.RPC(nameof(SyncBurguerElement), RpcTarget.All, option, demoSortingOrder, demoTurnCount);

        demoSortingOrder++;

        demoTurnCount++;

        if (demoTurnCount == DemoPositionList.Count - 2)
        {
            photonView.RPC(nameof(SetDemoUpBread), RpcTarget.All, true);
        }
    }

    private void PlayerTurn()
    {
        photonView.RPC(nameof(SetPlayerDownBread), RpcTarget.All, true);

        photonView.RPC(nameof(SyncRemotePlayerTurn), RpcTarget.Others, remotePlayerTurnCount, 0);

        canLocalPlayerPlay = true;
    }

    public void CheckPlayerTurn(int element)
    {
        if (!canLocalPlayerPlay) return;

        playerTurnCount++;
        remotePlayerTurnCount++;

        GameObject ingredient = Instantiate(BurguerOptions[element], PlayerPositionList[playerTurnCount].position, Quaternion.identity);
        ingredient.GetComponent<SpriteRenderer>().sortingOrder = playerSortingOrder;
        PlayerIngredientsList.Add(ingredient);

        //Sincronizo el turno en el equipo remoto
        photonView.RPC(nameof(SyncRemotePlayerTurn), RpcTarget.Others, remotePlayerTurnCount, element);

        playerSortingOrder++;

        if (playerTurnCount == PlayerPositionList.Count - 2)
        {
            canLocalPlayerPlay = false;
            StartCoroutine(nameof(SetPlayerUpBread));
        }
    }

    private IEnumerator SetPlayerUpBread()
    {
        canLocalPlayerPlay = false;

        PlayerUpBread.SetActive(true);

        PlayerUpBread.transform.DOMove(PlayerPositionList[^1].position, dropDuration)
            .SetEase(Ease.OutBounce).Play();

        yield return new WaitForSeconds(0.5f);

        if (CompareBurgers())
        {
            var goodFB = Instantiate(GoodFeedback, PlayerPositionList[0].position, Quaternion.identity);
            Destroy(goodFB, .5f);
            ShowCorrectFeedback();
            photonView.RPC(nameof(SyncReset), RpcTarget.All);
        }
        else
        {
            var worngFB = Instantiate(WrongFeedback, PlayerPositionList[0].position, Quaternion.identity);
            Destroy(worngFB, .5f);
            ShowWrongFeedback();
            photonView.RPC(nameof(SyncTurnFail), RpcTarget.Others);
            playerTurnFail = true;
            CheckPlayerFail();
        }
    }

    private bool CompareBurgers()
    {
        if (DemoIngredientsList.Count != PlayerIngredientsList.Count)
        {
            return false;
        }

        // Comparar los ingredientes uno por uno
        for (int i = 0; i < DemoIngredientsList.Count; i++)
        {
            // Si los ingredientes en la misma posición no son iguales, las hamburguesas no son iguales
            if (DemoIngredientsList[i].name != PlayerIngredientsList[i].name)
            {
                return false;
            }
        }

        // Si todas las comparaciones fueron iguales, las hamburguesas son iguales
        return true;
    }

    void CheckPlayerFail()
    {
        if (playerTurnFail && remotePlayerTurnFail)
        {
            photonView.RPC(nameof(SyncReset), RpcTarget.All);
        }
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

    void ResetTurn()
    {
        canLocalPlayerPlay = false;

        // Destruir las instancias de ingredientes de la demo
        foreach (var ingredient in DemoIngredientsList)
        {
            Destroy(ingredient);
        }
        DemoIngredientsList.Clear();
        DemoDownBread.SetActive(false);
        DemoUpBread.SetActive(false);
        demoSortingOrder = 2;
        demoTurnCount = 0;

        // Destruir las instancias de ingredientes del jugador local
        foreach (var ingredient in PlayerIngredientsList)
        {
            Destroy(ingredient);
        }
        PlayerIngredientsList.Clear();
        PlayerDownBread.SetActive(false);
        PlayerUpBread.SetActive(false);
        playerSortingOrder = 2;
        playerTurnCount = 0;

        // Destruir las instancias de ingredientes del jugador remoto
        foreach (var ingredient in RemoteIngredientsList)
        {
            Destroy(ingredient);
        }
        RemoteIngredientsList.Clear();
        RemoteDownBread.SetActive(false);
        RemoteUpBread.SetActive(false);
        remotePlayerSortingOrder = 2;
        remotePlayerTurnCount = 0;

        playerTurnFail = false;
        remotePlayerTurnFail = false;

        if (PhotonNetwork.IsMasterClient)
        {
            DemoTurn();
        }
    }

    [PunRPC]
    void SyncReset()
    {
        ResetTurn();
    }


    [PunRPC]
    void SyncTurnFail()
    {
        remotePlayerTurnFail = true;
    }

    [PunRPC]
    void SetDemoUpBread(bool active)
    {
        DemoUpBread.SetActive(active);
        DemoUpBread.transform.DOMove(DemoPositionList[^1].position, dropDuration)
            .SetEase(Ease.OutBounce).Play();
        PlayerTurn();
    }

    [PunRPC]
    void SetPlayerDownBread(bool active)
    {
        PlayerDownBread.SetActive(active);
    }

    [PunRPC]
    void SetDemoDownBread(bool active)
    {
        DemoDownBread.SetActive(active);
    }

    [PunRPC]
    private void SyncBurguerElement(int element, int sortingPlayerOrder, int demoTurnCount)
    {
        GameObject ingredient = Instantiate(BurguerOptions[element],
            DemoPositionList[demoTurnCount].position + Vector3.up * dropHeight,
            Quaternion.identity);

        ingredient.GetComponent<SpriteRenderer>().sortingOrder = sortingPlayerOrder;

        DemoIngredientsList.Add(ingredient);

        ingredient.transform.DOMove(DemoPositionList[demoTurnCount].position, dropDuration)
            .SetEase(Ease.OutBounce).Play().OnComplete(() =>
            {
                if (PhotonNetwork.CurrentRoom.Players.ContainsKey(1) && PhotonNetwork.LocalPlayer.ActorNumber == 1)
                {
                    if (demoTurnCount < DemoPositionList.Count() - 3)
                    {
                        DemoTurn();
                    }
                }
            });
    }

    [PunRPC]
    private void SyncRemotePlayerTurn(int remotePlayerTurnCount, int elementIndex)
    {
        if (remotePlayerTurnCount == 0)
        {
            RemoteDownBread.SetActive(true);
        }
        else if (remotePlayerTurnCount == RemotePlayerPositionList.Count - 2)
        {
            GameObject ingredient = Instantiate(BurguerOptions[elementIndex], RemotePlayerPositionList[remotePlayerTurnCount].position, Quaternion.identity);
            ingredient.GetComponent<SpriteRenderer>().sortingOrder = remotePlayerSortingOrder;
            RemoteIngredientsList.Add(ingredient);
            RemoteUpBread.SetActive(true);
        }
        else
        {
            GameObject ingredient = Instantiate(BurguerOptions[elementIndex], RemotePlayerPositionList[remotePlayerTurnCount].position, Quaternion.identity);
            ingredient.GetComponent<SpriteRenderer>().sortingOrder = remotePlayerSortingOrder;
            RemoteIngredientsList.Add(ingredient);
            remotePlayerSortingOrder++;
        }
    }

    [PunRPC]
    private void GameOver()
    {
        StopAllCoroutines();
        options.SetActive(false);
        canLocalPlayerPlay = false;
    }
}