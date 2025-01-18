using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 mouseOffset; // Offset between the mouse position and the object's center
    private Rigidbody rb;


    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component

    }

    void OnMouseDown()
    {
        // Create a ray from the camera to the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane xzPlane = new Plane(Vector3.up, transform.position);

        if (xzPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            // Store the difference between the hit point and the object's world position
            mouseOffset = transform.position - hitPoint;
            ApplyHoldRestrictions();
        }
    }

    void OnMouseDrag()
    {
        // Create a ray from the camera to the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane xzPlane = new Plane(Vector3.up, transform.position);

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
    }

    private void RemoveHoldRestrictions()
    {
        rb.useGravity = true;
        rb.mass = 1;
        rb.constraints = RigidbodyConstraints.None;
    }
}
