using UnityEngine;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float moveSpeed = 10f;

    private Camera playerCamera;
    private Rigidbody heldObject;

    private void Start()
    {
        playerCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
                TryInteract();
            else
                DropObject();
        }
    }

    private void FixedUpdate()
    {
        if (heldObject != null)
            MoveHeldObject();
    }

    private void TryInteract()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {

            if (hit.collider.CompareTag("Jar"))
            {
                Jar jarScript = hit.collider.GetComponent<Jar>();
                if (jarScript != null)
                {
                    jarScript.SpawnIngredient(playerCamera.transform);
                }
            }
            else if (hit.collider.CompareTag("Ingredient") || hit.collider.CompareTag("Potion"))
            {
                Rigidbody objectRb = hit.collider.GetComponent<Rigidbody>();
                if (objectRb != null)
                {
                    PickUpObject(objectRb);
                }
            }
            else if (hit.collider.CompareTag("Bed"))
            {
                Bed bedScript = hit.collider.GetComponent<Bed>();
                if (bedScript != null)
                {
                    bedScript.Interact();
                }
            }
        }
    }

    private void MoveHeldObject()
    {
        Vector3 targetPosition = holdPoint.position;
        heldObject.MovePosition(Vector3.Lerp(heldObject.position, targetPosition, Time.fixedDeltaTime * moveSpeed));
    }

    private void PickUpObject(Rigidbody objectRb)
    {
        heldObject = objectRb;
        heldObject.useGravity = false;
        heldObject.linearDamping = 10;
        heldObject.isKinematic = true;
    }

    public void DropObject()
    {
        StartCoroutine(DelayedDrop());
    }

    private IEnumerator DelayedDrop()
    {
        yield return new WaitForEndOfFrame();

        heldObject.useGravity = true;
        heldObject.linearDamping = 0;
        heldObject.isKinematic = false;
        heldObject = null;
    }

}
