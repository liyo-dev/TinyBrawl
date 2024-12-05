using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameImpostorLocal : MonoBehaviour
{
    public List<GameObject> images; // Lista de imágenes para el minijuego
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
    private MyGameManagerLocal _myGameManager;
    private float turnTimer; // Temporizador para el turno actual
    private float timeBetweenTurns = 6f;
    private bool localPlayerCanPlay;
    private bool imagesCalculated = false;
    private bool stopGame = true;
    private bool canClick = false;
    private int telonLowerCount = 0;

    private void Start()
    {
        Invoke(nameof(DoStart), 4f); // Start the game after 2 seconds
    }

    private void Update()
    {
        if (!stopGame)
        {
            // Reducir el temporizador del turno actual
            turnTimer -= Time.deltaTime;

            // Si el temporizador del turno actual llega a cero, iniciar un nuevo turno
            if (turnTimer <= 0)
            {
                // Iniciar un nuevo temporizador para el próximo turno
                turnTimer = timeBetweenTurns;

                // Cambiar de turno
                SyncTurn();
            }
        }
    }


    public void DoStart()
    {
        SyncTurn();
        stopGame = false;
    }

    public void DoStop()
    {
        GameOver();
    }

    private IEnumerator NextTurn()
    {
        canClick = false;

        Telon.Down();

        yield return new WaitForSeconds(Telon.animationDuration + 0.1f);

        DestroyImages();

        // Esperar a que las imágenes sean destruidas antes de calcular e instanciar nuevas imágenes
        yield return new WaitUntil(() => Image1 == null && Image2 == null && Image3 == null);

        CalculateImagesIfNeeded();

        Telon.Up();

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
        correctIndexImage = Random.Range(0, images.Count);
        wrongIndexImage = Random.Range(0, images.Count);

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

        Debug.Log("Indice correcto imagen: " + correctIndexImage);
        Debug.Log("Indice erroneo imagen: " + wrongIndexImage);
        Debug.Log("Posicion OK: " + positionOk);
        Debug.Log("Posicion KO1: " + positionKO1);
        Debug.Log("Posicion KO2: " + positionKO2);

        SyncImages(correctIndexImage, wrongIndexImage, positionOk, positionKO1, positionKO2);
    }

    public void OnClickPosition(int position)
    {
        if (!canClick) return;
        if (stopGame) return;
        if (!localPlayerCanPlay) return;

        Vector2 position_Instantiate = new Vector2(spawnPositions[position].position.x,
            spawnPositions[position].position.y - 1);

        if (position == positionOk)
        {
            var goodFB = Instantiate(GoodFeedback, position_Instantiate, Quaternion.identity);
            Destroy(goodFB, .5f);
            ShowCorrectFeedback();
            localPlayerCanPlay = false;
            turnTimer = timeBetweenTurns;
            SyncTurn();
        }
        else
        {
            localPlayerCanPlay = false;
            var worngFB = Instantiate(WrongFeedback, position_Instantiate, Quaternion.identity);
            Destroy(worngFB, .5f);

            ShowWrongFeedback();
            turnTimer = timeBetweenTurns;
            if (!localPlayerCanPlay)  SyncTurn();
        }
    }

    void ShowCorrectFeedback()
    {
        if (_myGameManager == null)
        {
            _myGameManager = FindObjectOfType<MyGameManagerLocal>();
        }

        _myGameManager.IncreaseLocalScore(1);
    }

    void ShowWrongFeedback()
    {
        if (_myGameManager == null)
        {
            _myGameManager = FindObjectOfType<MyGameManagerLocal>();
        }

        _myGameManager.DecreaseLocalScore(1);
    }

    private void DestroyImages()
    {
        if (Image1 != null)
        {
            Destroy(Image1);
        }
        if (Image2 != null)
        {
            Destroy(Image2);
        }
        if (Image3 != null)
        {
            Destroy(Image3);
        }
    }

    private void SyncImages(int correctIndexImage, int wrongIndexImage, int positionOk, int positionKO1, int positionKO2)
    {
        Image1 = Instantiate(images[correctIndexImage], spawnPositions[positionOk].position, Quaternion.identity);
        Image2 = Instantiate(images[wrongIndexImage], spawnPositions[positionKO1].position, Quaternion.identity);
        Image3 = Instantiate(images[wrongIndexImage], spawnPositions[positionKO2].position, Quaternion.identity);
    }

    private void SyncTurn()
    {
        if (stopGame) return;

        localPlayerCanPlay = true;
        imagesCalculated = false;

        // Incrementar el contador de bajadas de telón
        telonLowerCount++;

        // Cada dos bajadas de telón, reducir el tiempo entre turnos
        if (telonLowerCount % 2 == 0)
        {
          //  timeBetweenTurns = Mathf.Max(1f, timeBetweenTurns - 0.5f);
        }

        timeBetweenTurns = Mathf.Max(2f, timeBetweenTurns - 0.5f);
        
        StopAllCoroutines();
        StartCoroutine(NextTurn());
    }

    private void GameOver()
    {
        StopAllCoroutines();
        stopGame = true;
        Telon.Down();
    }
}
