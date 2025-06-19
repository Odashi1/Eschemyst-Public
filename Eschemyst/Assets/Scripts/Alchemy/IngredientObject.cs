using UnityEngine;

public class IngredientObject : MonoBehaviour
{
    [Tooltip("Assign the ingredient data here")]
    public Ingredient ingredientData;

    private void Start()
    {
        if (ingredientData != null)
        {
            gameObject.name = ingredientData.ingredientName;
        }
    }
}
