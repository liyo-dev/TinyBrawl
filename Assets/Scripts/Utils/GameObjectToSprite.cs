using UnityEngine;

public class GameObjectToSprite : MonoBehaviour
{
    public Camera renderCamera; // C�mara que usaremos para capturar el objeto
    public GameObject targetObject; // El GameObject que queremos capturar
    public int textureWidth = 256; // Ancho de la textura
    public int textureHeight = 256; // Altura de la textura
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (targetObject == null)
        {
            Debug.LogError("No se asign� un GameObject objetivo.");
            return;
        }

        if (renderCamera == null)
        {
            Debug.LogError("No se asign� una c�mara.");
            return;
        }

        // Configurar la c�mara para apuntar al objeto
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
            Debug.LogError("No se encontr� un MeshRenderer en el objeto objetivo o sus hijos.");
            return;
        }

        // Calcular los l�mites del MeshRenderer
        Bounds targetBounds = meshRenderer.bounds;

        // Colocar la c�mara frente al objeto
        Vector3 cameraPosition = targetBounds.center - renderCamera.transform.forward * (targetBounds.extents.magnitude + 1f);
        renderCamera.transform.position = cameraPosition;

        // Asegurarse de que la c�mara apunte al centro del objeto
        renderCamera.transform.LookAt(targetBounds.center);

        // Ajustar el tama�o de la c�mara ortogr�fica si es ortogr�fica
        if (renderCamera.orthographic)
        {
            renderCamera.orthographicSize = Mathf.Max(targetBounds.extents.x, targetBounds.extents.y);
        }
    }

    public Sprite CaptureGameObjectToSprite()
    {
        // Configurar la c�mara para capturar al GameObject
        RenderTexture renderTexture = new RenderTexture(textureWidth, textureHeight, 24);
        renderCamera.targetTexture = renderTexture;

        // Renderizar la c�mara
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
