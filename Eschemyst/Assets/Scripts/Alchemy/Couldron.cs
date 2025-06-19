using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Couldron : MonoBehaviour
{
    #region - Declarations
    [SerializeField] private float pourSpeed = 50f;
    [SerializeField] public List<int> AddedIngredientsID = new List<int>();
    [SerializeField] private GameObject liquidSurface;
    [SerializeField] private Renderer liquidRenderer;
    [SerializeField] private GameManager GameManager;
    private Color currentLiquidColor = Color.clear;
    public AudioClip waterboiling;
    public AudioClip waterpouring;
    public AudioClip[] waterdorp;

    private bool isPouring = false;
    private float pourProgress = 0f;
    private int ingredientCount = 0;

    [Header("UI Elements")]
    [SerializeField] private Image pourBarFill;
    #endregion

    #region - Events
    private void Update()
    {
        if (AddedIngredientsID.Count == 0)
        {
            pourBarFill.fillAmount = 0;
            return;
        }

        if (!IsLookingAtCauldron())
        {
            if (isPouring)
            {
                isPouring = false;
                pourProgress = 0f;
                pourBarFill.fillAmount = 0;
                Debug.Log("Stopped pouring – not looking at the cauldron.");
            }
            return;
        }

        if (Input.GetKey(KeyCode.E))
        {
            isPouring = true;
            pourProgress += pourSpeed * Time.deltaTime;
            pourBarFill.fillAmount = Mathf.Clamp01(pourProgress / 100f);
            Debug.Log($"Pouring... {pourProgress}%");

            if (pourProgress >= 100f)
            {
                BrewPotion();
            }
        }

        else if (isPouring)
        {
            isPouring = false;
            pourProgress = 0f;
            pourBarFill.fillAmount = 0;
            Debug.Log("Pouring canceled.");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ingredient"))
        {
            IngredientObject ingredient = other.GetComponent<IngredientObject>();

            if (ingredient != null)
            {
                if (ingredient.ingredientData.ingredientName == "Water")
                {
                    liquidSurface.SetActive(true);
                    currentLiquidColor = ingredient.ingredientData.color;
                    ingredientCount = 1;
                    AddedIngredientsID.Add(ingredient.ingredientData.id);
                    SoundFXManager.instance.PlaySoundFXClip(waterboiling, transform, 1f);
                }
                else
                {
                    if (!liquidSurface.activeSelf)
                    {
                        Debug.Log("Cannot add ingredients without water!");
                        return;
                    }

                    ingredientCount++;
                    AddedIngredientsID.Add(ingredient.ingredientData.id);
                    currentLiquidColor = MixColors(currentLiquidColor, ingredient.ingredientData.color, ingredientCount);
                    Debug.Log($"Added ingredient: {ingredient.ingredientData.ingredientName}");
                    SoundFXManager.instance.PlaySoundFXClips(waterdorp, transform, 1f);
                }

                UpdateLiquidColor();
                Destroy(other.gameObject);
            }
        }
    }
    #endregion

    #region  - Methods
    private void BrewPotion()
    {
        Potion matchedPotion = FindMatchingPotion();

        if (matchedPotion != null)
        {
            Debug.Log($"Brewed {matchedPotion.PotionName}!");
            SpawnPotion(matchedPotion);
            SoundFXManager.instance.PlaySoundFXClip(waterpouring, transform, 1f);
        }
        else
        {
            Debug.Log("Brew failed! No matching potion found.");
        }

        liquidSurface.SetActive(false);
        currentLiquidColor = Color.clear;
        ingredientCount = 0;
        UpdateLiquidColor();

        AddedIngredientsID.Clear();
        pourProgress = 0f;
        pourBarFill.fillAmount = 0;
    }


    private Potion FindMatchingPotion()
    {
        Potion[] allPotions = Resources.LoadAll<Potion>("Potions");

        foreach (Potion potion in allPotions)
        {
            if (potion.ingredientIDs.OrderBy(x => x).SequenceEqual(AddedIngredientsID.OrderBy(x => x)))
            {
                return potion;
            }
        }

        return null;
    }

    private void SpawnPotion(Potion potion)
    {
        GameObject potionPrefab = Resources.Load<GameObject>($"PrefabPotions/{potion.name}");

        if (potionPrefab != null)
        {
            Instantiate(potionPrefab, transform.position + Vector3.up * 5f + Vector3.left * 2, Quaternion.identity);

            if (potion.name == "Phantom Draught")
                GameManager.youWin = true;
        }
        else
        {
            Debug.LogError($"Potion prefab not found for: {potion.name}");
        }
    }

    private void UpdateLiquidColor()
    {
        if (liquidRenderer != null)
        {
            liquidRenderer.material.color = currentLiquidColor;
        }
    }

    private Color MixColors(Color baseColor, Color newColor, int count)
    {
        return new Color(
            Mathf.Lerp(baseColor.r, newColor.r, 1f / count),
            Mathf.Lerp(baseColor.g, newColor.g, 1f / count),
            Mathf.Lerp(baseColor.b, newColor.b, 1f / count)
        );
    }

    private bool IsLookingAtCauldron()
    {
        Camera playerCamera = Camera.main; 
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5f)) 
        {
            return hit.collider.gameObject == this.gameObject;
        }

        return false;
    }

    #endregion
}

