using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Basket : MonoBehaviour
{
    private List<GameObject> potionsInBasket = new List<GameObject>();

    public void ClearPotionsAtEndOfDay(GoblinNPC goblin)
    {
        ScanForPotionsInside();

        foreach (GameObject potionObject in new List<GameObject>(potionsInBasket))
        {
            if (potionObject == null) continue;

            PotionObject potionData = potionObject.GetComponent<PotionObject>();
            if (potionData != null && potionData.potionData != null)
            {
                Potion potion = potionData.potionData;
                Debug.Log("Checking potion: " + potion.PotionName);

                bool accepted = goblin.AcceptPotion(potion);
                if (accepted)
                {
                    Destroy(potionObject);

                    if (GameManager.Instance.GetCurrentDay() == "Sunday")
                    {
                        SpawnRandomEscapeIngredient();
                    }
                }
            }
        }
    }

    public void ScanForPotionsInside()
    {
        Collider basketCollider = GetComponent<Collider>();
        if (basketCollider == null)
        {
            Debug.LogWarning("Basket has no collider.");
            return;
        }

        Vector3 center = basketCollider.bounds.center;
        Vector3 halfExtents = basketCollider.bounds.extents;
        Collider[] hits = Physics.OverlapBox(center, halfExtents, Quaternion.identity);

        int potionLayer = LayerMask.NameToLayer("Potion");


        foreach (Collider hit in hits)
        {
            if (hit.gameObject.layer == potionLayer)
            {
                TryAddPotion(hit.gameObject);
            }
        }

        Debug.Log("Scanned basket. Potions count: " + potionsInBasket.Count);
    }

    private void TryAddPotion(GameObject obj)
    {
        PotionObject potion = obj.GetComponent<PotionObject>();
        if (potion != null && !potionsInBasket.Contains(obj))
        {
            potionsInBasket.Add(obj);
            Debug.Log("Basket count (added): " + potionsInBasket.Count);
        }
    }

    private void SpawnRandomEscapeIngredient()
    {
        string[] ingredientNames = { "TreeDoll", "UnicornHorn" };
        string randomName = ingredientNames[Random.Range(0, ingredientNames.Length)];

        GameObject ingredientPrefab = Resources.Load<GameObject>($"EscIngredients/{randomName}");

        if (ingredientPrefab == null)
        {
            Debug.LogWarning($"Ingredient prefab not found: {randomName}");
            return;
        }

        Instantiate(ingredientPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
    }


    public List<GameObject> GetPotions()
    {
        ScanForPotionsInside();
        return potionsInBasket;
    }
}
