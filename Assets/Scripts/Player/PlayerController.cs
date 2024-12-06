using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    
    [SerializeField] private int _playerNumber;
    private int _device; // ID del dispositivo asociado a este jugador
    private PlayerInputs _playerInputs;
    private bool cast;
    private int score;
    public int Score => score;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Obtener la lista de gamepads disponibles
        Gamepad[] gamepads = Gamepad.all.ToArray();
        if (gamepads.Length >= 1)
            _device = _playerNumber == 1 ? gamepads[0].deviceId : gamepads[1].deviceId;

        // Crear una instancia de PlayerInputs y pasarle el ID del dispositivo asignado manualmente
        _playerInputs = new PlayerInputs(_device);

        // Suscribirse a los eventos OnActionNorthStarted y OnActionNorthCanceled para detectar cu�ndo se inicia y cancela la acci�n
        _playerInputs.OnActionNorthStarted += OnNorthStarted;
        _playerInputs.OnActionNorthCanceled += OnNorthCanceled;
    }

    
    //North Input
    private void OnNorthStarted(int deviceID)
    {
        // Verificar si el ID del dispositivo coincide con el ID del dispositivo asociado a este jugador
        if (_device == deviceID)
        {
            if (cast)
            {
                score++;
                //_getScore.SetScore(score);
            }
            else
            {
                score = Mathf.Max(0, score - 1);
                //_getScore.SetScore(score);
            }
        }
    }

    private void OnNorthCanceled(int deviceID)
    {
        // Verificar si el ID del dispositivo coincide con el ID del dispositivo asociado a este jugador
        if (_device == deviceID)
        {
        }
    }

    
    // Triggers
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cast"))
        {
            cast = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Cast"))
        {
            cast = false;
        }
    }
}
