using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Controller : MonoBehaviour
{
    #region - Declarations
    CharacterController characterController;

    #region - Audio
    public AudioClip[] Footstep;
    private Vector3 previousPosition;
    private float accumulatedDistance = 0f;
    public float footstepDistance = 1.0f;
    #endregion

    #region - Movement
    public float walkSpeed = 2f;
    public float runSpeed = 3f;
    public float jumpPower = 6f;
    public float gravity = 20f;
    #endregion

    #region - States
    public bool isCrouching = false;
    public float crouchHeight = 1f;
    public float standHeight = 1.5f;

    public bool isMoving = false;
    bool isRunning = false;
    public bool canMove = true;
    private bool canJump = true;
    #endregion

    #region - Camera
    public Camera playerCamera;

    public float lookSpeed = 2f;
    public float lookXLimit = 70f;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    #endregion

    #region - References
    [SerializeField] GameObject BookController;
    [SerializeField] Book recpBook;
    #endregion
    #endregion

    #region - Methods
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        SetMovement(true);
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleCrouch();
        HandleBook();
    }

    void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;

        isMoving = (curSpeedX != 0 || curSpeedY != 0);

        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded && canJump)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        PlayFootstepSound();
    }

    void PlayFootstepSound()
    {
        try
        {
            float distanceMoved = Vector3.Distance(previousPosition, transform.position);
            accumulatedDistance += distanceMoved;

            if (accumulatedDistance >= footstepDistance && isMoving && characterController.isGrounded)
            {
                SoundFXManager.instance.PlaySoundFXClips(Footstep, transform, 1f);
                accumulatedDistance = 0f;
            }

            previousPosition = transform.position;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    void HandleRotation()
    {
        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            bool isObstacleAbove = Physics.Raycast(
                transform.position,
                Vector3.up,
                characterController.height / 2 + 1f
            );

            if (isCrouching && isObstacleAbove)
            {
                Debug.Log("Cannot stand up! Obstacle above.");
                return;
            }

            isCrouching = !isCrouching;

            if (isCrouching)
            {
                characterController.height = crouchHeight;
                canJump = false;
            }
            else
            {
                characterController.height = standHeight;
                canJump = true;
            }
        }
    }

    public void SetMovement(bool state)
    {
        canMove = state;
        canJump = state;

        if (state)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void HandleBook()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool state = BookController.activeSelf;
            BookController.SetActive(!state);
            canMove = state;
            Cursor.visible = !state;
            if (!state)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;

        }
    }
    #endregion
}