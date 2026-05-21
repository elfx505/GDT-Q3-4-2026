using UnityEngine;

public class SimpleFlyCamera : MonoBehaviour
{
    public float mainSpeed = 100f;
    public float shiftAdd = 250f;
    public float maxShift = 1000f;
    public float camSens = 3f;

    private float totalRun = 1.0f;
    private bool lockHeight = false;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rotationX = transform.eulerAngles.x;
        rotationY = transform.eulerAngles.y;
    }

    void Update()
    {
        // Toggle height lock
        if (Input.GetKeyDown(KeyCode.L))
            lockHeight = !lockHeight;

        // Mouse look using delta (FIXED)
        rotationX -= Input.GetAxis("Mouse Y") * camSens;
        rotationY += Input.GetAxis("Mouse X") * camSens;

        // Clamp vertical rotation
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);

        // Movement
        Vector3 p = GetBaseInput();

        // Vertical movement
        if (Input.GetKey(KeyCode.Space))
            p += Vector3.up;
        if (Input.GetKey(KeyCode.LeftControl))
            p += Vector3.down;

        // Speed boost
        if (Input.GetKey(KeyCode.LeftShift))
        {
            totalRun += Time.deltaTime;
            p *= totalRun * shiftAdd;

            p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
            p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
            p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
        }
        else
        {
            totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
            p *= mainSpeed;
        }

        p *= Time.deltaTime;

        // Apply movement
        if (lockHeight)
        {
            float y = transform.position.y;
            transform.Translate(p, Space.Self);
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
        else
        {
            transform.Translate(p, Space.Self);
        }
    }

    private Vector3 GetBaseInput()
    {
        Vector3 velocity = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            velocity += Vector3.forward;
        if (Input.GetKey(KeyCode.S))
            velocity += Vector3.back;
        if (Input.GetKey(KeyCode.A))
            velocity += Vector3.left;
        if (Input.GetKey(KeyCode.D))
            velocity += Vector3.right;

        return velocity;
    }
}