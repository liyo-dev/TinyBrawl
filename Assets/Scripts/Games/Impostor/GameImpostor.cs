using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameImpostor : MonoBehaviourPunCallbacks
{
    public List<GameObject> images; 
    public Transform[] spawnPositions;
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
    private bool localPlayerCanPlay = false;
    private bool remotePlayerCanPlay = false;
    private bool imagesCalculated = false;
    private bool stopGame = false;
    private bool canClick = false;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Invoke(nameof(DoStart), 4f); // Start the game after 2 seconds
        }
    }

    public void DoStart()
    {
        photonView.RPC(nameof(SyncTurn), RpcTarget.All);
    }

    public void DoStop()
    {
        photonView.RPC(nameof(GameOver), RpcTarget.Others);
    }

    private IEnumerator NextTurn()
    {
        Telon.Down();

        yield return new WaitForSeconds(Telon.animationDuration + 0.1f);

        photonView.RPC(nameof(DestroyImages), RpcTarget.Others);

        // Esperar a que las imágenes sean destruidas antes de calcular e instanciar nuevas imágenes
        yield return new WaitUntil(() => Image1 == null && Image2 == null && Image3 == null);

        if (PhotonNetwork.IsMasterClient)
        {
            CalculateImagesIfNeeded();
        }

        canClick = true;
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
        int correctIndexImage = Random.Range(0, images.Count);
        int wrongIndexImage = Random.Range(0, images.Count);

        while (correctIndexImage == wrongIndexImage)
        {
            wrongIndexImage = Random.Range(0, images.Count);
        }

        // Posiciones
        positionOk = Random.Range(0, spawnPositions.Length);

        if (positionOk == 0)
        {
            positionKO1 = 1;
            positionKO2 = 2;
        }
        else if (positionOk == 1)
        {
            positionKO1 = 0;
            positionKO2 = 2;
        }
        else if (positionOk == 2)
        {
            positionKO1 = 0;
            positionKO2 = 1;
        }

        photonView.RPC(nameof(SyncImagesAndPosition), RpcTarget.Others, correctIndexImage, wrongIndexImage, positionOk, positionKO1, positionKO2);
        photonView.RPC(nameof(SyncImages), RpcTarget.All, correctIndexImage, wrongIndexImage, positionOk, positionKO1, positionKO2);
        photonView.RPC(nameof(SyncTelonUp), RpcTarget.All);
    }

    public void OnClickPosition(int position)
    {

        if (!canClick) return;
        if (stopGame) return;
        if (!localPlayerCanPlay) return;

        localPlayerCanPlay = false;

        Vector2 position_Instantiate = new Vector2(spawnPositions[position].position.x,
            spawnPositions[position].position.y - 1);

        if (position == positionOk)
        {
            photonView.RPC(nameof(SyncTurn), RpcTarget.All);
            var goodFB = Instantiate(GoodFeedback, position_Instantiate, Quaternion.identity);
            Destroy(goodFB, .5f);
            ShowCorrectFeedback();
        }
        else
        {
            // pongo remotePlayerCanPlay a false para resetear cuando seleccione el ultimo jugador
            photonView.RPC(nameof(SyncRemotePlayerCanPlay), RpcTarget.Others);
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

    private IEnumerator DestroyImagesAndWait()
    {
        if (Image1 != null)
        {
            Destroy(Image1);
            yield return new WaitUntil(() => Image1 == null);
        }
        if (Image2 != null)
        {
            Destroy(Image2);
            yield return new WaitUntil(() => Image2 == null);
        }
        if (Image3 != null)
        {
            Destroy(Image3);
            yield return new WaitUntil(() => Image3 == null);
        }
    }


    [PunRPC]
    private void SyncImagesAndPosition(int correctIndexImage, int wrongIndexImage, int positionOk, int positionKO1, int positionKO2)
    {
        this.positionOk = positionOk;
        this.positionKO1 = positionKO1;
        this.positionKO2 = positionKO2;
    }


    [PunRPC]
    private void SyncImages(int correctIndexImage, int wrongIndexImage, int positionOk, int positionKO1, int positionKO2)
    {
        Image1 = Instantiate(images[correctIndexImage], spawnPositions[positionOk].position, Quaternion.identity);
        Image2 = Instantiate(images[wrongIndexImage], spawnPositions[positionKO1].position, Quaternion.identity);
        Image3 = Instantiate(images[wrongIndexImage], spawnPositions[positionKO2].position, Quaternion.identity);
    }


    [PunRPC]
    private void DestroyImages()
    {
        StartCoroutine(DestroyImagesAndWait());
    }

    [PunRPC]
    private void SyncTurn()
    {
        if (stopGame) return;

        canClick = false;
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

    [PunRPC]
    private void SyncTelonUp()
    {
        Telon.Up();
        localPlayerCanPlay = true;
    }
}
