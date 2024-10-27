using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float sensitivity = 100f;
    public float scrollSensitivity = 10f;
    public float distance = 10f;
    public float minDistance = 2f;
    public float maxDistance = 20f;
    public float wasdMoveSpeed = 5f;
    public float verticalAngleLimit = 45f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        if (target == null)
        {
            GameObject targetObject = new GameObject("Camera Target");
            targetObject.transform.position = Vector3.zero;
            target = targetObject.transform;
        }

        Vector3 angles = transform.eulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;

        transform.position = target.position - transform.forward * distance;
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            rotationX += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            rotationY -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, 0f, verticalAngleLimit);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * scrollSensitivity;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 move = (transform.right * moveDirection.x + transform.forward * moveDirection.z) * wasdMoveSpeed * Time.deltaTime;
        move.y = 0;
        target.position += move;

        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);
        transform.position = target.position - (rotation * Vector3.forward * distance);
        transform.LookAt(target);
    }
}