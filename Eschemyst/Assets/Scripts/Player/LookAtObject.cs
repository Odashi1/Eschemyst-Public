using UnityEngine;
using TMPro;
using System.Collections;

public class LookAtObjectReyCast : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 5f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private TextMeshProUGUI worldText;
    [SerializeField] private Transform worldCanvas;
    [SerializeField] private float fadeDuration = 0.5f;

    private Transform currentTarget;
    private Coroutine fadeCoroutine;

    private void Update()
    {
        DetectInteractable();

        if (currentTarget != null)
        {
            worldCanvas.position = Vector3.Lerp(worldCanvas.position, currentTarget.position + Vector3.up * 0.3f, Time.deltaTime * 10);
            worldCanvas.LookAt(Camera.main.transform);
            worldCanvas.Rotate(0, 180, 0);
        }
    }

    private void DetectInteractable()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, interactableLayer))
        {
            IngredientObject ingredientObject = hit.collider.GetComponent<IngredientObject>();
            if (ingredientObject != null && ingredientObject.ingredientData != null)
            {
                ShowInfo(ingredientObject.ingredientData.ingredientName, ingredientObject.ingredientData.description, hit.transform);
                return;
            }

            PotionObject potionObject = hit.collider.GetComponent<PotionObject>();
            if (potionObject != null && potionObject.potionData != null)
            {
                ShowInfo(potionObject.potionData.PotionName, potionObject.potionData.description, hit.transform);
                return;
            }
        }

        HideInfo();
    }

    private void ShowInfo(string name, string description, Transform target)
    {
        worldText.text = $"{name}\n{description}";
        currentTarget = target;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeText(1));
    }

    private void HideInfo()
    {
        currentTarget = null;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeText(0));
    }

    private IEnumerator FadeText(float targetAlpha)
    {
        float startAlpha = worldText.color.a;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            Color newColor = worldText.color;
            newColor.a = alpha;
            worldText.color = newColor;
            yield return null;
        }

        Color finalColor = worldText.color;
        finalColor.a = targetAlpha;
        worldText.color = finalColor;
    }
}
