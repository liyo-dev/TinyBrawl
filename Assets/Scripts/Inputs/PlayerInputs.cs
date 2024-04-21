using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs
{
    // Eventos de acci�n
    public event Action<int> OnActionNorthStarted;
    public event Action<int> OnActionNorthCanceled;

    // Componentes
    private Controls _controls;
    private int _deviceID; // ID del dispositivo asociado a este jugador

    public int DeviceID => _deviceID; // Propiedad para obtener el ID del dispositivo

    public PlayerInputs(int deviceID)
    {
        _deviceID = deviceID;
        InitControlsPlayer();
    }

    private void InitControlsPlayer()
    {
        _controls = new Controls();
        _controls.Player.Enable();
        _controls.Player.North.started += ctx => OnNorthStarted(ctx);
        _controls.Player.North.canceled += ctx => OnNorthCanceled(ctx);
    }

    private void OnNorthStarted(InputAction.CallbackContext context)
    {
        // Obtener el ID del dispositivo que inici� la acci�n
        int deviceID = context.control.device.deviceId;

        // Verificar si el ID del dispositivo coincide con el ID del dispositivo asociado a este jugador
        if (deviceID == _deviceID)
        {
            // Invocar el evento con el ID del dispositivo que activ� la acci�n
            OnActionNorthStarted?.Invoke(deviceID);
        }
    }

    private void OnNorthCanceled(InputAction.CallbackContext context)
    {
        // Obtener el ID del dispositivo que cancel� la acci�n
        int deviceID = context.control.device.deviceId;

        // Verificar si el ID del dispositivo coincide con el ID del dispositivo asociado a este jugador
        if (deviceID == _deviceID)
        {
            // Invocar el evento con el ID del dispositivo que activ� la acci�n
            OnActionNorthCanceled?.Invoke(deviceID);
        }
    }
}
