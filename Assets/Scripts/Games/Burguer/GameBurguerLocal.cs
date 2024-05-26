using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameBurguerLocal : MonoBehaviour
{
public GameObject UpBread;
    public GameObject DownBread;
    public List<GameObject> BurguerOptions;
    public Transform DemoDownPosition;
    public Transform PlayerDownPosition;
    public float dropDuration = 1.0f; // Duración de la caída de la hamburguesa
    public float dropHeight = 2.0f; // Altura desde la cual cae la hamburguesa
    public float ingredientDelay = 0.5f; // Retraso entre la instanciación de cada ingrediente

    private GameObject DemoDownBread;
    private GameObject PlayerDownBread;
    private GameObject DemoUpBread;
    private GameObject PlayerUpBread;
    private GameObject previousPlayerIngredient;
    private int sortingPlayerOrder;
    private bool canLocalPlayerPlay = false;

    public void DoStart()
    {
        // Instancia el pan de abajo en la posición DemoDown
        DemoDownBread = Instantiate(DownBread, DemoDownPosition.position, Quaternion.identity);
        DemoDownBread.GetComponent<SpriteRenderer>().sortingOrder = 1; // Orden en layer 1
        DemoDownBread.SetActive(false);

        // Instancia el pan de arriba en la posición DemoDown más la altura de caída
        DemoUpBread = Instantiate(UpBread, DemoDownPosition.position + Vector3.up * dropHeight, Quaternion.identity);
        DemoUpBread.GetComponent<SpriteRenderer>().sortingOrder = BurguerOptions.Count + 3; // Orden en layer (máximo)
        DemoUpBread.SetActive(false);
        
        // Instancia el pan de abajo en la posición PlayerDown
        PlayerDownBread = Instantiate(DownBread, PlayerDownPosition.position, Quaternion.identity);
        PlayerDownBread.GetComponent<SpriteRenderer>().sortingOrder = 1; // Orden en layer 1
        PlayerDownBread.SetActive(false);

        // Instancia el pan de arriba en la posición PlayerDown más la altura de caída
        PlayerUpBread = Instantiate(UpBread, PlayerDownPosition.position + Vector3.up * dropHeight, Quaternion.identity);
        PlayerUpBread.GetComponent<SpriteRenderer>().sortingOrder = BurguerOptions.Count + 3; // Orden en layer (máximo)
        PlayerUpBread.SetActive(false);
        
        DemoTurn();
    }

    private IEnumerator DelayedDemoTurn()
    {
        DemoDownBread.SetActive(true);

        int sortingOrder = 2;
        GameObject previousIngredient = DemoDownBread;
    
        for (int i = 0; i < BurguerOptions.Count + 1; i++)
        {
            GameObject option = BurguerOptions[Random.Range(0, BurguerOptions.Count)];
            GameObject burger = Instantiate(option, DemoDownPosition.position + Vector3.up * dropHeight, Quaternion.identity);
            burger.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder; // Orden en layer incremental
            sortingOrder++;
            burger.transform.DOMove(previousIngredient.transform.position, dropDuration).SetEase(Ease.OutBounce); // Animación de caída
            yield return new WaitForSeconds(ingredientDelay);
            previousIngredient = burger;
        }

        DemoUpBread.SetActive(true);
        
        yield return new WaitForSeconds(ingredientDelay);
    }


    private void DemoTurn()
    {
        StartCoroutine(DelayedDemoTurn());
    }

    private void PlayerTurn()
    {
        PlayerDownBread.SetActive(true);
        previousPlayerIngredient = PlayerDownBread;
        sortingPlayerOrder = 2;
        canLocalPlayerPlay = true;
    }
    
    public void CheckPlayerTurn(int element)
    {
        if (!canLocalPlayerPlay) return;

        // Ajusta la posición de caída para que cada ingrediente caiga uno encima del otro
        Vector3 dropPosition = previousPlayerIngredient.transform.position + Vector3.up * dropHeight;

        GameObject ingredient = Instantiate(BurguerOptions[element], dropPosition, Quaternion.identity);
        ingredient.GetComponent<SpriteRenderer>().sortingOrder = sortingPlayerOrder; // Orden en layer incremental
        sortingPlayerOrder++;
        ingredient.transform.DOMove(previousPlayerIngredient.transform.position, dropDuration).SetEase(Ease.OutBounce);
       // playerImages.Add(ingredient);
        previousPlayerIngredient = ingredient;

        if (sortingPlayerOrder == BurguerOptions.Count + 3)
        {
            canLocalPlayerPlay = false;
            StartCoroutine(nameof(DelayedPlayerTurn));
        }
    }
    
    private IEnumerator DelayedPlayerTurn()
    {
        yield return new WaitForSeconds(ingredientDelay);
        
        PlayerUpBread.SetActive(true);
    }
}
