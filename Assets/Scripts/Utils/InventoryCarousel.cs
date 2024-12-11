using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class InventoryCarousel : MonoBehaviour
{
    [Header("Inventory Items")]
    public GameObject[] inventoryItems; // Lista de prefabs de �tems del inventario
    private int currentIndex = 0; // �ndice del �tem actual
    private GameObject activeItem; // �tem actualmente instanciado

    [Header("UI Elements")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemInfoText;
    public Button nextButton;
    public Button previousButton;
    public Button equipRightHandButton;
    public Button equipLeftHandButton;

    [Header("Player Data")]
    [SerializeField] private PlayerDataSO playerDataSO;

    void Start()
    {
        if (inventoryItems.Length == 0)
        {
            Debug.LogError("No se han asignado �tems en el carrusel.");
            return;
        }

        // Configurar botones
        nextButton.onClick.AddListener(ShowNextItem);
        previousButton.onClick.AddListener(ShowPreviousItem);
        equipRightHandButton.onClick.AddListener(() => EquipItemToHand("EquipRight"));
        equipLeftHandButton.onClick.AddListener(() => EquipItemToHand("EquipLeft"));

        // Mostrar el primer �tem
        ShowItem(currentIndex);
    }

    private void ShowItem(int index)
    {
        // Destruir el �tem actual si existe
        if (activeItem != null)
        {
            Destroy(activeItem);
        }

        // Instanciar el �tem actual
        activeItem = Instantiate(inventoryItems[index], transform);
        Rigidbody rb = activeItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Evitar que la gravedad o las fuerzas afecten al �tem
        }
        activeItem.transform.localPosition = Vector3.zero; // Centrar en el parent
        activeItem.transform.localRotation = Quaternion.Euler(0, 180, 0);
        activeItem.transform.localScale = new Vector3(4, 4, 4);

        // Actualizar la informaci�n del �tem
        itemNameText.text = inventoryItems[index].name;
        itemInfoText.text = $"Informaci�n de {inventoryItems[index].name}"; // Ajusta seg�n tus datos
    }

    private void ShowNextItem()
    {
        currentIndex = (currentIndex + 1) % inventoryItems.Length;
        ShowItem(currentIndex);
    }

    private void ShowPreviousItem()
    {
        currentIndex = (currentIndex - 1 + inventoryItems.Length) % inventoryItems.Length;
        ShowItem(currentIndex);
    }

    public void EquipItemToHand(string hand)
    {
        // Guardar en el SO
        if (hand == "EquipLeft")
        {
            playerDataSO.leftHandItemId = currentIndex;
            Debug.Log($"Arma equipada en la mano izquierda: {inventoryItems[currentIndex].name}");
        }
        else if (hand == "EquipRight")
        {
            playerDataSO.rightHandItemId = currentIndex;
            Debug.Log($"Arma equipada en la mano derecha: {inventoryItems[currentIndex].name}");
        }

        // Sincronizar con PlayFab
        SaveEquippedItemsToPlayFab();
    }

    private void SaveEquippedItemsToPlayFab()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new System.Collections.Generic.Dictionary<string, string>
            {
                { "LeftHandItemId", playerDataSO.leftHandItemId.ToString() },
                { "RightHandItemId", playerDataSO.rightHandItemId.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, OnItemsSavedToPlayFab, OnPlayFabError);
    }

    private void OnItemsSavedToPlayFab(UpdateUserDataResult result)
    {
        Debug.Log("�tems equipados guardados correctamente en PlayFab.");
    }

    private void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError($"Error al guardar �tems en PlayFab: {error.GenerateErrorReport()}");
    }
}
