using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 mouseOffset; // Offset between the mouse position and the object's center
    private Plane xzPlane; // XZ plane with same Y value as the cursor hit point
    private Rigidbody rb;


    public delegate void BlockHoldHandler();
    public static BlockHoldHandler OnBlockHold;

    public delegate void BlockReleaseHandler();
    public static BlockReleaseHandler OnBlockRelease;


    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component

    }

    void OnMouseDown()
    {
        // Create ray from camera to mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 hitPoint = hitInfo.point;

            // Create XZ plane with same Y value as the hit point to simplify offset calculations 
            xzPlane = new Plane(Vector3.up, hitPoint);
            // Get offset between hit point and the object's position
            mouseOffset = transform.position - hitPoint;
            ApplyHoldRestrictions();
        }
        
    }

    void OnMouseDrag()
    {
        // Create a ray from the camera to the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (xzPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            transform.position = hitPoint + mouseOffset;
        }
    }

    private void OnMouseUp()
    {
        RemoveHoldRestrictions();
    }


    private void ApplyHoldRestrictions()
    {
        rb.useGravity = false;
        rb.mass = 10000;
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        OnBlockHold?.Invoke();
    }

    private void RemoveHoldRestrictions()
    {
        rb.useGravity = true;
        rb.mass = 1;
        rb.constraints = RigidbodyConstraints.None;
        OnBlockRelease?.Invoke();
    }
}
