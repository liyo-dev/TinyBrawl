using Cinemachine;
using UnityEngine;

public class CameraSwipeRotation : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;

    [Header("Configuración de rotación")]
    public float rotationSpeed = 2.0f; // Velocidad de rotación

    [Header("Configuración inicial de la cámara")]
    private float cameraRotation = 0f; // Rotación inicial en el eje X

    private Vector2 startTouchPosition; // Posición inicial del toque
    private Vector2 endTouchPosition;   // Posición final del toque
    private Transform cameraTransform;

    private bool isRightSideTouch = false; // Verifica si el toque es en el lado derecho

    void Start()
    {
        virtualCamera = GameObject.FindAnyObjectByType<CinemachineVirtualCamera>();

        if (!virtualCamera) return;

        cameraTransform = virtualCamera.transform;
        cameraRotation = virtualCamera.transform.rotation.x;
        ConfigureInitialCameraPosition();
    }

    void Update()
    {
        if (!virtualCamera) return;
        HandleSwipeRotation();
    }

    private void ConfigureInitialCameraPosition()
    {
        cameraTransform.rotation = Quaternion.Euler(cameraRotation, 0f, 0f);
    }

    private void HandleSwipeRotation()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Guardar la posición inicial del toque
                    startTouchPosition = touch.position;

                    // Comprobar si el toque está en el lado derecho de la pantalla
                    isRightSideTouch = startTouchPosition.x > Screen.width / 2;
                    break;

                case TouchPhase.Moved:
                    if (isRightSideTouch) // Solo realizar la rotación si el toque es en el lado derecho
                    {
                        // Actualizar la posición final del toque
                        endTouchPosition = touch.position;

                        // Calcular la diferencia en Y
                        float deltaY = endTouchPosition.y - startTouchPosition.y;

                        // Ajustar la rotación inicial de la cámara
                        cameraRotation += deltaY * rotationSpeed * Time.deltaTime;
                        cameraTransform.rotation = Quaternion.Euler(cameraRotation, 0f, 0f);

                        // Actualizar la posición inicial para suavizar el movimiento
                        startTouchPosition = endTouchPosition;
                    }
                    break;

                case TouchPhase.Ended:
                    // Reiniciar la bandera al finalizar el toque
                    isRightSideTouch = false;
                    break;
            }
        }
    }
}
