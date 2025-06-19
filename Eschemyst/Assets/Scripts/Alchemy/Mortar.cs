using UnityEngine;
using System.Collections;

public class Mortar : MonoBehaviour
{
    #region - Declarations
    [SerializeField] PlayerInteraction HoldingItem;

    public Transform snapPoint;
    public float shrinkSpeed = 0.5f;
    public float grindTime = 2f;
    public float spawnHeight = 0.2f;

    private GameObject currentIngredientObject;
    private int crushedIngredientID;
    private bool hasIngredient = false;
    private bool isGrinding = false;
    #endregion

    #region - Events
    private void Update()
    {
        if (hasIngredient && Input.GetKeyDown(KeyCode.R) && !isGrinding)
        {
            StartCoroutine(GrindIngredient());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasIngredient) return;

        if (other.CompareTag("Ingredient"))
        {
            IngredientObject ingredientObject = other.GetComponent<IngredientObject>();
            if (ingredientObject != null)
            {
                int ingredientID = ingredientObject.ingredientData.id;
                crushedIngredientID = ingredientID * 100;

                Ingredient matchingCrushedIngredient = FindMatchingCrushedIngredient(crushedIngredientID);

                if (matchingCrushedIngredient != null)
                {
                    HoldingItem.DropObject();
                    Debug.Log($"Crushing {ingredientObject.ingredientData.ingredientName} into {matchingCrushedIngredient.ingredientName}");
                    SnapIngredient(ingredientObject);
                }
                else
                {
                    Debug.Log("This action has no use");
                }
            }
        }
    }

    #endregion

    #region - Methods
    private Ingredient FindMatchingCrushedIngredient(int crushedID)
    {
        Ingredient[] allIngredients = Resources.LoadAll<Ingredient>("Ingredients");

        foreach (Ingredient ingredient in allIngredients)
        {
            if (ingredient.id == crushedID)
            {
                return ingredient;
            }
        }

        return null;
    }

    private void SnapIngredient(IngredientObject ingredientObject)
    {
        currentIngredientObject = ingredientObject.gameObject;
        hasIngredient = true;

        StartCoroutine(SnapToCenter(currentIngredientObject));
    }

    private IEnumerator SnapToCenter(GameObject ingredient)
    {
        float duration = 0.5f;
        Vector3 startPos = ingredient.transform.position;
        Vector3 targetPos = snapPoint.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            ingredient.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ingredient.transform.position = targetPos;
        ingredient.transform.SetParent(snapPoint);
    }

    private IEnumerator GrindIngredient()
    {
        isGrinding = true;
        float elapsedTime = 0;

        while (elapsedTime < grindTime)
        {
            if (currentIngredientObject != null)
            {
                currentIngredientObject.transform.localScale *= (1 - shrinkSpeed * Time.deltaTime);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (currentIngredientObject != null)
        {
            Destroy(currentIngredientObject);
        }

        SpawnCrushedIngredient();
        isGrinding = false;
        hasIngredient = false;
    }

    private void SpawnCrushedIngredient()
    {
        Ingredient matchingCrushedIngredient = FindMatchingCrushedIngredient(crushedIngredientID);

        if (matchingCrushedIngredient != null)
        {
            GameObject crushedIngredientPrefab = Resources.Load<GameObject>($"PrefabIngredients/{matchingCrushedIngredient.name}");

            if (crushedIngredientPrefab != null)
            {
                Instantiate(crushedIngredientPrefab, snapPoint.position + Vector3.up * spawnHeight, Quaternion.identity);
                Debug.Log($"Spawned crushed ingredient: {matchingCrushedIngredient.name}");
            }
            else
            {
                Debug.LogWarning($"Crushed ingredient prefab missing for: {matchingCrushedIngredient.name}");
            }
        }
        else
        {
            Debug.LogWarning($"No matching crushed ingredient found for ID: {crushedIngredientID}");
        }
    }


    public bool HasIngredientReady()
    {
        return hasIngredient && !isGrinding;
    }

    #endregion
}
