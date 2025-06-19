using UnityEngine;

[CreateAssetMenu(fileName = "NewIngredient", menuName = "Alchemy/Ingredient")]
public class Ingredient : ScriptableObject
{
    [Tooltip("Ingredient Name")]
    public string ingredientName;

    [Tooltip("Effect Description")]
    [TextArea]
    public string description;

    [Tooltip("Ingredient ID")]
    public int id;

    [Tooltip("Ingredient Color")]
    public Color color;
}
