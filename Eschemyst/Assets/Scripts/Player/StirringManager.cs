using System;
using System.Linq;
using UnityEngine;

public class StirringManager : MonoBehaviour
{
    public Transform cauldronCameraPosition;
    public Transform stirringStick;
    public Transform cauldronCenter;
    public Transform cauldronBottom;

    public float rotationSpeed = 50f;
    public float maxLookAngle = 30f;

    private Transform originalCameraParent;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private bool isStirring = false;
    private Camera playerCamera;
    private Controller playerController;
    [SerializeField] private Couldron couldron;

    void Start()
    {
        playerCamera = Camera.main;
        playerController = GetComponent<Controller>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            float distance = Vector3.Distance(transform.position, cauldronCenter.position);
            if (distance <= 3)
            {
                if (isStirring)
                    ExitStirringMode();
                else if (IsLookingAtCauldron() && couldron.AddedIngredientsID.Any())
                    EnterStirringMode();
            }
        }

        if (isStirring)
            RotateStick();
    }

    bool IsLookingAtCauldron()
    {
        Vector3 directionToCauldron = (cauldronCenter.position - playerCamera.transform.position).normalized;
        float angle = Vector3.Angle(playerCamera.transform.forward, directionToCauldron);
        return angle < maxLookAngle;
    }

    void EnterStirringMode()
    {
        isStirring = true;
        playerController.SetMovement(false);

        originalCameraParent = playerCamera.transform.parent;
        originalCameraPosition = playerCamera.transform.position;
        originalCameraRotation = playerCamera.transform.rotation;

        playerCamera.transform.SetPositionAndRotation(cauldronCameraPosition.position, cauldronCameraPosition.rotation);
        playerCamera.transform.parent = cauldronCameraPosition;
    }

    void ExitStirringMode()
    {
        isStirring = false;
        playerController.SetMovement(true);

        if (couldron.AddedIngredientsID.Last() > 0)
            couldron.AddedIngredientsID.Add(-1);


        playerCamera.transform.SetParent(originalCameraParent);
        playerCamera.transform.SetPositionAndRotation(originalCameraPosition, originalCameraRotation);
    }

    void RotateStick()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

        stirringStick.RotateAround(cauldronCenter.position, Vector3.up, mouseX);

        Vector3 newPosition = stirringStick.position;
        newPosition.y = cauldronCenter.position.y;
        stirringStick.position = newPosition;
    }
}
