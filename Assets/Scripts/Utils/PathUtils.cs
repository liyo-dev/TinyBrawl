using System.IO;
using UnityEngine;

public static class PathUtils
{
    private const string PlayerDataFolderName = "PlayerSavedData";
    private static string uniqueFolderName;
    /// <summary>
    /// Inicializa el nombre único de la carpeta usando un identificador único.
    /// </summary>
    static PathUtils()
    {
        // Usa el Process ID como identificador único
        uniqueFolderName = $"{PlayerDataFolderName}_{System.Diagnostics.Process.GetCurrentProcess().Id}";
    }


    /// <summary>
    /// Devuelve la ruta completa de la carpeta PlayerSavedData dentro de la ubicación persistente.
    /// Si no existe, se crea automáticamente.
    /// </summary>
    /// <returns>Ruta completa de la carpeta PlayerSavedData.</returns>
    public static string GetPlayerDataFolderPath()
    {
        // Crear la ruta para la carpeta "PlayerSavedData" dentro de "persistentDataPath"
        string playerDataFolder = Path.Combine(Application.persistentDataPath, uniqueFolderName);

        // Verificar si la carpeta no existe y crearla
        if (!Directory.Exists(playerDataFolder))
        {
            Directory.CreateDirectory(playerDataFolder);
            Debug.Log($"Carpeta creada en: {playerDataFolder}");
        }

        return playerDataFolder;
    }

    /// <summary>
    /// Devuelve la ruta completa de un archivo dentro de la carpeta PlayerSavedData.
    /// </summary>
    /// <param name="fileName">Nombre del archivo, incluyendo la extensión.</param>
    /// <returns>Ruta completa del archivo.</returns>
    public static string GetPlayerDataFilePath(string fileName)
    {
        string folderPath = GetPlayerDataFolderPath();
        return Path.Combine(folderPath, fileName);
    }
}
