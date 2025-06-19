using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoblinNPC : MonoBehaviour
{
    private List<Potion> requestedPotions = new List<Potion>();
    [SerializeField] private TextMeshProUGUI TaskList;
    [SerializeField] private GameObject VillegerGoblin;

    public void GenerateDailyPotions()
    {
        requestedPotions.Clear();
        Potion[] allPotions = Resources.LoadAll<Potion>("Potions");

        if (GameManager.Instance.GetCurrentDay() == "Sunday")
        {
            requestedPotions.Clear();

            Potion randomPotion;
            do
            {
                randomPotion = allPotions[Random.Range(0, allPotions.Length)];
            }
            while (randomPotion.PotionName == "Phantom Draught");

            requestedPotions.Add(randomPotion);

            TaskList.text = "Hey pssstt.. could you do me a favour? I need a " +
                string.Join(", ", requestedPotions.ConvertAll(p => p.PotionName)) +
                ". I will give you something cool, just leave it in the chest before you go to sleep, the guard doesn't need to know.";

            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                r.enabled = false;
            }

            VillegerGoblin.transform.position = new Vector3(-5f, 0.07f, -5f);

            return;
        }

        if (allPotions.Length == 0)
        {
            Debug.LogError("No potions found in Resources/Potions!");
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            Potion randomPotion;

            do
            {
                randomPotion = allPotions[Random.Range(0, allPotions.Length)];
            }
            while (randomPotion.PotionName == "Phantom Draught");

            requestedPotions.Add(randomPotion);
        }

        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = true;
        }

        VillegerGoblin.transform.position = new Vector3(25f, -1f, 0f);

        TaskList.text = "Today's requests: " + string.Join(", ", requestedPotions.ConvertAll(p => p.PotionName));
    }

    public bool AcceptPotion(Potion potion)
    {
        if (requestedPotions.Contains(potion))
        {
            requestedPotions.Remove(potion);
            Debug.Log("Accepted: " + potion.PotionName);
            GameManager.Instance.AddReputation(5);
            return true;
        }

        return false;

    }

    public int GetRemainingRequests()
    {
        return requestedPotions.Count;
    }

    private void OnEnable()
    {
        GameManager.OnDayChanged += GenerateDailyPotions;
    }

    private void OnDisable()
    {
        GameManager.OnDayChanged -= GenerateDailyPotions;
    }

    public List<Potion> GetRequestedPotions()
    {
        return new List<Potion>(requestedPotions);
    }


}
