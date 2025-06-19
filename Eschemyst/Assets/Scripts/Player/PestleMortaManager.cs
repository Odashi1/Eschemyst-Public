using UnityEngine;

public class ThistleMortaManager : MonoBehaviour
{
    #region - Declarations
    public Transform mortarCameraPosition;
    public Transform pestleStone;
    public Transform mortarTop;

    public float rotationSpeed = 50f;
    public float maxLookAngle = 30f;

    private Transform originalCameraParent;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private bool isStirring = false;
    private Camera playerCamera;
    private Controller playerController;
    private Mortar mortar;
    #endregion

    #region - Events
    void Start()
    {
        playerCamera = Camera.main;
        playerController = GetComponent<Controller>();
        mortar = FindObjectOfType<Mortar>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            float distance = Vector3.Distance(transform.position, mortarTop.position);
            if (distance <= 3)
            {
                if (isStirring)
                    ExitStirringMode();
                else if (IsLookingAtMortar() && mortar.HasIngredientReady())
                    EnterStirringMode();
            }
        }

        if (isStirring)
            RotateStick();
    }
    #endregion

    #region - Methods
    bool IsLookingAtMortar()
    {
        Vector3 directionToMortar = (mortarTop.position - playerCamera.transform.position).normalized;
        float angle = Vector3.Angle(playerCamera.transform.forward, directionToMortar);
        return angle < maxLookAngle;
    }

    void EnterStirringMode()
    {
        isStirring = true;
        playerController.SetMovement(false);

        originalCameraParent = playerCamera.transform.parent;
        originalCameraPosition = playerCamera.transform.position;
        originalCameraRotation = playerCamera.transform.rotation;

        playerCamera.transform.SetPositionAndRotation(mortarCameraPosition.position, mortarCameraPosition.rotation);
        playerCamera.transform.parent = mortarCameraPosition;
    }

    void ExitStirringMode()
    {
        isStirring = false;
        playerController.SetMovement(true);

        playerCamera.transform.SetParent(originalCameraParent);
        playerCamera.transform.SetPositionAndRotation(originalCameraPosition, originalCameraRotation);
    }

    void RotateStick()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        pestleStone.RotateAround(mortarTop.position, Vector3.up, mouseX);
    }
    #endregion
}
