using UnityEngine;

public class PlaneDragAndDrop : MonoBehaviour
{
    private Vector3 mouseOffset;
    private Plane currentPlane;
    private Camera mainCamera;
    private Rigidbody rb;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        SetInitialPlane(); // Set the default plane (X-Z plane)
    }

    void Update()
    {
        // Update the plane depending on whether Q is held or not
        if (Input.GetKey(KeyCode.Q))
        {
            if (currentPlane.normal != Vector3.forward) // If it's not already X-Y plane
            {
                // Switch to X-Y plane where Z is fixed
                currentPlane = new Plane(Vector3.forward, transform.position);
                RecalculateMouseOffset();
            }
        }
        else
        {
            if (currentPlane.normal != Vector3.up) // If it's not already X-Z plane
            {
                // Switch to X-Z plane where Y is fixed
                currentPlane = new Plane(Vector3.up, transform.position);
                RecalculateMouseOffset();
            }
        }
    }

    private void OnMouseDown()
    {
        // Disable gravity while dragging
        if (rb != null)
        {
            rb.useGravity = false;
        }

        // Create a ray from the camera to the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // Calculate the mouse offset based on the current plane
        if (currentPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            // Store the difference between the hit point and the object's world position
            mouseOffset = transform.position - hitPoint;
        }
    }

    private void OnMouseDrag()
    {
        // Create a ray from the camera to the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // Calculate the point of intersection between the ray and the current plane
        if (currentPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            // Update the object position to the new hit point, adding the offset to maintain the initial click position
            transform.position = hitPoint + mouseOffset;
        }
    }

    private void OnMouseUp()
    {
        // Enable gravity when the object is released
        if (rb != null)
        {
            rb.useGravity = true;
        }
    }

    private void SetInitialPlane()
    {
        // Default to X-Z plane where the normal is Vector3.up (Y-axis), and the object's Y is fixed
        currentPlane = new Plane(Vector3.up, transform.position);
    }

    // Recalculate the mouse offset when the plane changes
    private void RecalculateMouseOffset()
    {
        // Create a ray from the camera to the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // Calculate the mouse offset based on the new plane
        if (currentPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            // Recalculate the offset when plane switches
            mouseOffset = transform.position - hitPoint;
        }
    }
}
