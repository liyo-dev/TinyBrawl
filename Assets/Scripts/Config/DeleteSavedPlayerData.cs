using System.IO;
using UnityEngine;

public class DeleteSavedPlayerData: MonoBehaviour
{
    private string playerDataFilePath;

    private void Awake()
    {
        if (string.IsNullOrEmpty(playerDataFilePath))
        {
            playerDataFilePath = Path.Combine(Application.persistentDataPath, "PlayerLoginData.json");
        }

    }


    /// <summary>
    /// Borra el archivo de datos del jugador si existe.
    /// </summary>
    public void DeletePlayerDataFile()
    {
        if (File.Exists(playerDataFilePath))
        {
            try
            {
                File.Delete(playerDataFilePath);
                Debug.Log($"Archivo eliminado exitosamente: {playerDataFilePath}");
            }
            catch (IOException ex)
            {
                Debug.LogError($"Error al intentar eliminar el archivo: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"No se encontró ningún archivo en la ruta: {playerDataFilePath}");
        }
    }
}
