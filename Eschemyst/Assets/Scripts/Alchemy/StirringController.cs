using UnityEngine;

public class StirringController : MonoBehaviour
{
    public Transform cauldronCenter;
    public float rotationSpeed = 50f;
    private bool isStirring = false;

    void Update()
    {
        if (!isStirring) return;

        float horizontal = Input.GetAxis("Mouse X");
        float vertical = Input.GetAxis("Mouse Y");

        Vector3 rotationDirection = new Vector3(vertical, 0, -horizontal);
        transform.RotateAround(cauldronCenter.position, Vector3.up, rotationSpeed * Time.deltaTime * rotationDirection.magnitude);
    }

    public void SetStirringState(bool state)
    {
        isStirring = state;
    }
}
