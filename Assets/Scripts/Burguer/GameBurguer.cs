using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;

public class GameBurguer : MonoBehaviourPunCallbacks
{
    public GameObject UpBread;
    public GameObject DownBread;
    public List<GameObject> BurguerOptions;
    public List<Transform> DemoDownPositionList;
    public Transform PlayerDownPosition;
    public float dropDuration = 1.0f; // Duración de la caída de la hamburguesa
    public float dropHeight = 2.0f; // Altura desde la cual cae la hamburguesa
    public float ingredientDelay = 2f; // Retraso entre la instanciación de cada ingrediente

    private GameObject DemoDownBread;
    private GameObject PlayerDownBread;
    private GameObject DemoUpBread;
    private GameObject PlayerUpBread;
    private int sortingPlayerOrder;
    private bool canLocalPlayerPlay = false;
    private int demoSortingOrder = 2;
    private int demoTurnCount = 0;

    public void DoStart()
    {
        if (photonView.IsMine)
        {
            // Instancia el pan de abajo en la posición DemoDown
            DemoDownBread = Instantiate(DownBread, DemoDownPositionList[demoTurnCount].position, Quaternion.identity);
            DemoDownBread.GetComponent<SpriteRenderer>().sortingOrder = 1; // Pan de abajo order 1
            DemoDownBread.SetActive(false);
            
            // Instancia el pan de arriba en la posición DemoDown más la altura de caída
            DemoUpBread = Instantiate(UpBread, DemoDownPositionList[DemoDownPositionList.Count - 1].position + Vector3.up * dropHeight,
                Quaternion.identity);
            DemoUpBread.GetComponent<SpriteRenderer>().sortingOrder = BurguerOptions.Count + 3; // Pan de arriba order maximo
            DemoUpBread.SetActive(false);

            // Instancia el pan de abajo en la posición PlayerDown
            PlayerDownBread = Instantiate(DownBread, PlayerDownPosition.position, Quaternion.identity);
            PlayerDownBread.GetComponent<SpriteRenderer>().sortingOrder = 1; // Orden en layer 1
            PlayerDownBread.SetActive(false);

            // Instancia el pan de arriba en la posición PlayerDown más la altura de caída
            PlayerUpBread = Instantiate(UpBread, PlayerDownPosition.position + Vector3.up * dropHeight,
                Quaternion.identity);
            PlayerUpBread.GetComponent<SpriteRenderer>().sortingOrder =
                BurguerOptions.Count + 3; // Orden en layer (máximo)
            PlayerUpBread.SetActive(false);
        }

        //Comienza la demo
        DemoDownBread.SetActive(true);

        //Solo lo hace el player master
        if (PhotonNetwork.CurrentRoom.Players.ContainsKey(1) && PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            DemoTurn();
        }
    }

    private void DemoTurn()
    {
        int option = Random.Range(0, BurguerOptions.Count);

        photonView.RPC(nameof(SyncBurguerElement), RpcTarget.All, option, demoSortingOrder, demoTurnCount);

        demoSortingOrder++;

        demoTurnCount++;

        if (demoTurnCount == DemoDownPositionList.Count)
        {
            photonView.RPC(nameof(SetDemoUpBread), RpcTarget.Others, true);
        }
    }

    private void PlayerTurn()
    {
        PlayerDownBread.SetActive(true);
        sortingPlayerOrder = 2;
        canLocalPlayerPlay = true;
    }

    public void CheckPlayerTurn(int element)
    {
        if (!canLocalPlayerPlay) return;

        // Ajusta la posición de caída para que cada ingrediente caiga uno encima del otro
        //Vector3 dropPosition = previousPlayerIngredient.transform.position + Vector3.up * dropHeight;

        //GameObject ingredient = Instantiate(BurguerOptions[element], dropPosition, Quaternion.identity);
       // ingredient.GetComponent<SpriteRenderer>().sortingOrder = sortingPlayerOrder; // Orden en layer incremental
        sortingPlayerOrder++;
        // playerImages.Add(ingredient);

        if (sortingPlayerOrder == BurguerOptions.Count + 3)
        {
            canLocalPlayerPlay = false;
            StartCoroutine(nameof(DelayedPlayerTurn));
        }
    }

    private IEnumerator DelayedPlayerTurn()
    {
        yield return new WaitForSeconds(ingredientDelay);

        PlayerUpBread.SetActive(true);
    }

    [PunRPC]
    void SetDemoDownBread(bool active)
    {
        DemoDownBread.SetActive(active);
    }

    [PunRPC]
    void SetDemoUpBread(bool active)
    {
        DemoUpBread.SetActive(active);
    }
    

    [PunRPC]
    private void SyncBurguerElement(int element, int sortingPlayerOrder, int demoTurnCount)
    {
        GameObject ingredient = Instantiate(BurguerOptions[element], DemoDownPositionList[demoTurnCount].position + Vector3.up * dropHeight,
            Quaternion.identity);
        
        ingredient.GetComponent<SpriteRenderer>().sortingOrder = sortingPlayerOrder;
        
        ingredient.transform.DOMove(DemoDownPositionList[demoTurnCount].position, dropDuration)
            .SetEase(Ease.OutBounce).Play().OnComplete(()=>
            {
                if (PhotonNetwork.CurrentRoom.Players.ContainsKey(1) && PhotonNetwork.LocalPlayer.ActorNumber == 1)
                {
                    if (demoTurnCount < DemoDownPositionList.Count())
                    { 
                        DemoTurn();
                    }
                }
            });

    }


}