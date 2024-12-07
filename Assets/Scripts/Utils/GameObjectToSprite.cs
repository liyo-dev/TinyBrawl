using UnityEngine;

public class GameObjectToSprite : MonoBehaviour
{
    public Camera renderCamera; // Cámara que usaremos para capturar el objeto
    public GameObject targetObject; // El GameObject que queremos capturar
    public int textureWidth = 256; // Ancho de la textura
    public int textureHeight = 256; // Altura de la textura
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (targetObject == null)
        {
            Debug.LogError("No se asignó un GameObject objetivo.");
            return;
        }

        if (renderCamera == null)
        {
            Debug.LogError("No se asignó una cámara.");
            return;
        }

        // Configurar la cámara para apuntar al objeto
        PositionCamera();

        // Generar y asignar el sprite
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = CaptureGameObjectToSprite();
    }

    private void PositionCamera()
    {
        // Obtener el MeshRenderer del hijo "Body"
        MeshRenderer meshRenderer = targetObject.GetComponentInChildren<MeshRenderer>();

        if (meshRenderer == null)
        {
            Debug.LogError("No se encontró un MeshRenderer en el objeto objetivo o sus hijos.");
            return;
        }

        // Calcular los límites del MeshRenderer
        Bounds targetBounds = meshRenderer.bounds;

        // Colocar la cámara frente al objeto
        Vector3 cameraPosition = targetBounds.center - renderCamera.transform.forward * (targetBounds.extents.magnitude + 1f);
        renderCamera.transform.position = cameraPosition;

        // Asegurarse de que la cámara apunte al centro del objeto
        renderCamera.transform.LookAt(targetBounds.center);

        // Ajustar el tamaño de la cámara ortográfica si es ortográfica
        if (renderCamera.orthographic)
        {
            renderCamera.orthographicSize = Mathf.Max(targetBounds.extents.x, targetBounds.extents.y);
        }
    }

    public Sprite CaptureGameObjectToSprite()
    {
        // Configurar la cámara para capturar al GameObject
        RenderTexture renderTexture = new RenderTexture(textureWidth, textureHeight, 24);
        renderCamera.targetTexture = renderTexture;

        // Renderizar la cámara
        renderCamera.Render();

        // Crear una textura a partir del RenderTexture
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
        texture.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
        texture.Apply();

        // Limpiar
        renderCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // Crear un sprite a partir de la textura
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
