using UnityEngine;

public class PotionObject : MonoBehaviour
{
    [Tooltip("Assign the potion data here")]
    public Potion potionData;

    private void Start()
    {
        if (potionData != null)
        {
            gameObject.name = potionData.PotionName;
        }
    }
}
