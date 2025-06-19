using UnityEngine;

public class Jar : MonoBehaviour
{
    [SerializeField] private IngredientObject ingredientPrefab;
    [SerializeField] private float spawnDistance = 1f;

    public void SpawnIngredient(Transform playerCamera)
    {
        if (ingredientPrefab != null && playerCamera != null)
        {
            Vector3 spawnPosition = playerCamera.position + playerCamera.forward * spawnDistance;
            IngredientObject newIngredient = Instantiate(ingredientPrefab, spawnPosition, Quaternion.identity);
            newIngredient.gameObject.layer = LayerMask.NameToLayer("Ingredient");
            newIngredient.gameObject.tag = "Ingredient";

            Rigidbody rb = newIngredient.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }
        }
    }
}
