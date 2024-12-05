using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CountdownTimerLocal : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public GameObject TresGameObject;
    public GameObject DosGameObject;
    public GameObject UnoGameObject;
    public Image TimeBarImage;
    public GameObject ExitMenuBtn;
    private float totalTime = GameConfig.TOTAL_TIME;
    private float timeLeft;
    private bool countdownStarted = false;
    private MyGameManagerLocal _myGameManager;

    void Start()
    {
        timeLeft = totalTime;
        // Iniciar el conteo regresivo mostrando el número 3
        TresGameObject.SetActive(true);

        Invoke("CountdownTwo", 1f);
    }

    void Update()
    {
        if (countdownStarted)
        {
            timeLeft -= Time.deltaTime;

            if (timeLeft < 0)
            {
                timeLeft = 0;
            }

            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);

        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // Actualizar el tamaño de la barra de tiempo
        float fillAmount = timeLeft / totalTime;
        TimeBarImage.fillAmount = fillAmount;
    }

    void CountdownTwo()
    {
        // Desactivar el número 3 y mostrar el número 2
        TresGameObject.SetActive(false);
        DosGameObject.SetActive(true);
        Invoke("CountdownOne", 1f);
    }

    void CountdownOne()
    {
        // Desactivar el número 2 y mostrar el número 1
        DosGameObject.SetActive(false);
        UnoGameObject.SetActive(true);
        Invoke("StartCountdown", 1f);
    }

    void StartCountdown()
    {
        // Ocultar el número 1 y comenzar el conteo regresivo real
        UnoGameObject.SetActive(false);

        countdownStarted = true;

        Invoke("TimeOut", totalTime);
    }

    void LoadScene()
    {
        SceneManager.LoadScene(SceneNames.Title.ToString());
    }

    void TimeOut()
    {
        _myGameManager.DoStop();


        //Cambiar este boton por el de ver el ranking?
        ExitMenuBtn.SetActive(true);
    }
}
