using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPotion", menuName = "Alchemy/Potion")]
public class Potion : ScriptableObject
{
    [Tooltip("Potion Name")]
    public string PotionName;

    [Tooltip("Effect Description")]
    [TextArea]
    public string description;

    [Tooltip("Needed Ingredient IDs")]
    public List<int> ingredientIDs;
}
