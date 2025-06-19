using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI reputationDisplayer;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] Controller ControllerManager;
    public AudioClip rooster;

    [SerializeField] public GameObject youLost;
    public bool youWin = false;
    [SerializeField] public GameObject youEscaped;

    private string[] daysOfWeek = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
    private int currentDayIndex = 0;
    private int reputation = 100;

    public List<Potion> pendingPotions = new List<Potion>();
    public delegate void DayChanged();
    public static event DayChanged OnDayChanged;
    [SerializeField] Basket _basket;
    [SerializeField] GoblinNPC _goblin;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateDayUI();
        UpdateReputationUI();
    }

    public void AdvanceDay()
    {
        ApplyPenalty();


        if (youWin)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);
            youEscaped.active = true;
            ControllerManager.SetMovement(false);

            return;
        }
        else if (reputation <= 0)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);
            youLost.active = true;
            ControllerManager.SetMovement(false);

            return;
        }


        if (_basket != null && _goblin != null)
        {
            _basket.ClearPotionsAtEndOfDay(_goblin);
        }

        if (Instance == null) return;
        StartCoroutine(FadeTransition());
    }

    private void ApplyPenalty()
    {
        GoblinNPC goblin = FindAnyObjectByType<GoblinNPC>();
        if (goblin == null || _basket == null) return;

        List<Potion> requestedPotions = goblin.GetRequestedPotions();
        Dictionary<string, int> requiredPotionCounts = new Dictionary<string, int>();
        foreach (Potion potion in requestedPotions)
        {
            if (!requiredPotionCounts.ContainsKey(potion.PotionName))
                requiredPotionCounts[potion.PotionName] = 0;
            requiredPotionCounts[potion.PotionName]++;
        }

        foreach (GameObject potionObject in _basket.GetPotions())
        {
            if (potionObject == null) continue;

            PotionObject potionData = potionObject.GetComponent<PotionObject>();
            if (potionData != null && potionData.potionData != null)
            {
                string potionName = potionData.potionData.PotionName;

                if (requiredPotionCounts.ContainsKey(potionName) && requiredPotionCounts[potionName] > 0)
                {
                    requiredPotionCounts[potionName]--;
                }
            }
        }

        foreach (var pair in requiredPotionCounts)
        {
            int missingCount = pair.Value;
            if (missingCount > 0)
            {
                reputation -= 5 * missingCount;
            }
        }
        UpdateReputationUI();
    }

    private IEnumerator FadeTransition()
    {
        yield return StartCoroutine(Fade(1));

        currentDayIndex = (currentDayIndex + 1) % daysOfWeek.Length;
        UpdateDayUI();

        OnDayChanged?.Invoke();

        yield return StartCoroutine(Fade(0));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeImage.color.a;
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, newAlpha);
            yield return null;
        }

        SoundFXManager.instance.PlaySoundFXClip(rooster, transform, 1f);
    }

    private void UpdateDayUI()
    {
        if (dayText != null)
        {
            dayText.text = $"Day: {daysOfWeek[currentDayIndex]}";
        }
    }

    public string GetCurrentDay()
    {
        return daysOfWeek[currentDayIndex];
    }

    public void AddReputation(int amount)
    {
        reputation += amount;
        UpdateReputationUI();
    }

    private void UpdateReputationUI()
    {
        reputationDisplayer.text = "Reputation: " + reputation;
    }
}
