using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 mouseOffset; // Offset between the mouse position and the object's center
    private Plane xzPlane; // XZ plane with same Y value as the cursor hit point
    private Rigidbody rb;
    private int blockLayer;
    private bool isColliding = false;
    private bool isHeld = false;

    [SerializeField] private float forceStrength = 10f;
    [SerializeField] private float stopThreshold = 5f;
    [SerializeField] private float lerpSpeed = 10f;
    

    public delegate void BlockHoldHandler();
    public static BlockHoldHandler OnBlockHold;

    public delegate void BlockReleaseHandler();
    public static BlockReleaseHandler OnBlockRelease;


    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        blockLayer = LayerMask.NameToLayer("Block");
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
            MoveBlockTowardsCursor(hitPoint);
        }
    }

    private void OnMouseUp()
    {
        RemoveHoldRestrictions();
    }

    private void MoveBlockTowardsCursor(Vector3 hitPoint)
    {
        // Use two different methods to move block towards the cursor
        if (isColliding)
        {
            // Use force when block is colliding - simulates force interactions
            ApplyForce(hitPoint);
        }
        else
        {
            // Use lerp when block is free - prevents block from overshooting the cursor
            LerpBlockToCursor(hitPoint);
        }
    }

    private void ApplyForce(Vector3 hitPoint)
    {
        // Force scales with distance between cursor and object
        Vector3 direction = (hitPoint + mouseOffset) - transform.position;
        float distanceToTarget = direction.magnitude;

        if (distanceToTarget > stopThreshold)
        {
            rb.AddForce(direction * forceStrength, ForceMode.Force);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void LerpBlockToCursor(Vector3 hitPoint)
    {
        transform.position = Vector3.Lerp(transform.position, (hitPoint + mouseOffset), Time.deltaTime * lerpSpeed);
    }

    private void ApplyHoldRestrictions()
    {
        rb.useGravity = false;
        rb.mass = 1;
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        isHeld = true;
        OnBlockHold?.Invoke();
    }

    private void RemoveHoldRestrictions()
    {
        rb.useGravity = true;
        rb.mass = 1;
        rb.constraints = RigidbodyConstraints.None;
        isHeld = false;
        OnBlockRelease?.Invoke();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Block"))
        {
            isColliding = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Block"))
        {
            isColliding = false;

            // Reset block velocity once it has been removed (prevents overshooting cursor)
            if (isHeld)
            {
                rb.velocity = Vector3.zero;
            }
        }
    }
}
