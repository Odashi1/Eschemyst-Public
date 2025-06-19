using UnityEngine;

public class Bed : MonoBehaviour
{

    public void Interact()
    {
        Debug.Log("Going to sleep...");
        GameManager.Instance.AdvanceDay();
    }
}
