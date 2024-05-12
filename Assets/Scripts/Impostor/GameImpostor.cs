using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameImpostor : MonoBehaviourPunCallbacks
{
    public List<GameObject> images; // Lista de imágenes para el minijuego
    public float telonDuration; // Duración de cada turno
    public Transform[] spawnPositions; // Posiciones donde instanciar las imágenes
    public DotUpDown Telon;
    public GameObject GoodFeedback;
    public GameObject WrongFeedback;
    
    
    private int correctIndexImage;
    private int wrongIndexImage;
    private int positionOk;
    private int positionKO1;
    private int positionKO2;
    private GameObject Image1;
    private GameObject Image2;
    private GameObject Image3;
    private MyGameManager _myGameManager;
    private float turnDuration = 4f; // Duración de cada turno
    private bool localPlayerCanPlay;
    private bool remotePlayerCanPlay;
    private bool imagesCalculated = false;
    private bool stopGame;

    public void DoStart()
    {
        photonView.RPC("SyncTurn", RpcTarget.All);
    }

    public void DoStop()
    {
        photonView.RPC("GameOver", RpcTarget.Others);
    }
    
    private IEnumerator NextTurn()
    {
        Telon.Down();
        
        yield return new WaitForSeconds(Telon.animationDuration + 0.1f);
        
        photonView.RPC("DestroyImages", RpcTarget.All);

        if (PhotonNetwork.CurrentRoom.Players.ContainsKey(1) && PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            CalculateImagesIfNeeded();
        }

        Telon.Up();
    }
    
    void CalculateImagesIfNeeded()
    {
        if (!imagesCalculated)
        {
            CalculateImages();
            imagesCalculated = true;
        }
    }

    
    void CalculateImages()
    {
        // Imágenes
        correctIndexImage = Random.Range(0, images.Count);
        wrongIndexImage = Random.Range(0, images.Count);

        while (correctIndexImage == wrongIndexImage)
        {
            wrongIndexImage = Random.Range(0, images.Count);
        }

        // Posiciones
        positionOk = Random.Range(0, spawnPositions.Length);
        positionKO1 = (positionOk + 1) % spawnPositions.Length;
        positionKO2 = (positionOk + 2) % spawnPositions.Length;

        photonView.RPC("SyncImagesAndPosition", RpcTarget.Others, correctIndexImage, wrongIndexImage, positionOk, positionKO1, positionKO2);
    }


    void CalculateTiming()
    {
        if (turnDuration >= 1.5)
        {
            turnDuration -= 0.25f;
        }

        if (telonDuration >= 0.25f)
        {
            if (turnDuration <= 3f)
            {
                telonDuration -= 0.05f;
            }
        }
    }
    
    public void OnClickPosition(int position)
    {
        if (!localPlayerCanPlay) return; 
        
        Vector2 position_Instantiate = new Vector2(spawnPositions[position].position.x,
            spawnPositions[position].position.y - 1);
        
        if (position == positionOk)
        {
            var goodFB = Instantiate(GoodFeedback, position_Instantiate, Quaternion.identity);
            Destroy(goodFB, .5f);
            ShowCorrectFeedback();
            localPlayerCanPlay = false; 
            photonView.RPC("SyncTurn", RpcTarget.All);
        }
        else
        {
            localPlayerCanPlay = false;
            photonView.RPC("SyncRemotePlayerCanPlay", RpcTarget.Others);
            var worngFB = Instantiate(WrongFeedback, position_Instantiate, Quaternion.identity);
            Destroy(worngFB, .5f);

            ShowWrongFeedback();
            
            if (!localPlayerCanPlay && !remotePlayerCanPlay) DoStart();
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
    
    [PunRPC]
    private void SyncImagesAndPosition(int correctIndexImage, int wrongIndexImage, int positionOk, int positionKO1, int positionKO2)
    {
        this.positionOk = positionOk;
        this.positionKO1 = positionKO1;
        this.positionKO2 = positionKO2;
        photonView.RPC("SyncImages", RpcTarget.Others, correctIndexImage, wrongIndexImage, positionOk, positionKO1, positionKO2);
    }
    
    
    [PunRPC]
    private void SyncImages(int correctIndexImage, int wrongIndexImage, int positionOk, int positionKO1, int positionKO2)
    {
        Image1 = PhotonNetwork.Instantiate(images[correctIndexImage].name, spawnPositions[positionOk].position, Quaternion.identity);
        Image2 = PhotonNetwork.Instantiate(images[wrongIndexImage].name, spawnPositions[positionKO1].position, Quaternion.identity);
        Image3 = PhotonNetwork.Instantiate(images[wrongIndexImage].name, spawnPositions[positionKO2].position, Quaternion.identity);
    }

    [PunRPC]
    private void DestroyImages()
    {
        if (Image1 == null) return;
        PhotonNetwork.Destroy(Image1);
        PhotonNetwork.Destroy(Image2);
        PhotonNetwork.Destroy(Image3);
    }

    [PunRPC]
    private void SyncTurn()
    {
        if (stopGame) return;
        
        localPlayerCanPlay = true;
        remotePlayerCanPlay = true;
        imagesCalculated = false;
        StopAllCoroutines();
        StartCoroutine(NextTurn());
    }
    
    [PunRPC]
    private void GameOver()
    {
        StopAllCoroutines();
        stopGame = true;
        Telon.Down();
    }
    
    [PunRPC]
    private void SyncRemotePlayerCanPlay()
    {
        remotePlayerCanPlay = false;
    }
}
