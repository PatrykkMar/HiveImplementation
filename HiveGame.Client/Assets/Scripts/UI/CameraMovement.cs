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
    public float maxAngle = 80f;
    public float minAngle = 15f;
    public float startingRotation = 30f;

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

        rotationY = startingRotation;

        UpdateCameraPosition();
    }

    void Update()
    {
        HandleMouseControls();
        HandleTouchControls();
        UpdateCameraPosition();
    }

    private void HandleMouseControls()
    {
        // PC
        if (Input.GetMouseButton(1))
        {
            rotationX += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            rotationY -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, minAngle, maxAngle);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * scrollSensitivity;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 move = (transform.right * moveDirection.x + transform.forward * moveDirection.z) * wasdMoveSpeed * Time.deltaTime;
        move.y = 0;
        target.position += move;
    }

    private Vector2 lastTouchPos1;
    private Vector2 lastTouchPos2;
    private bool wasPinching = false;

    private void HandleTouchControls()
    {
        // Phone
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Moved)
            {
                rotationX += t.deltaPosition.x * sensitivity * 0.01f;
                rotationY -= t.deltaPosition.y * sensitivity * 0.01f;
                rotationY = Mathf.Clamp(rotationY, minAngle, maxAngle);
            }

            wasPinching = false;
        }
        else if (Input.touchCount == 2)
        {
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);

            Vector2 pos1 = t1.position;
            Vector2 pos2 = t2.position;

            if (!wasPinching)
            {
                lastTouchPos1 = pos1;
                lastTouchPos2 = pos2;
                wasPinching = true;
            }
            else
            {
                float prevDist = (lastTouchPos1 - lastTouchPos2).magnitude;
                float currDist = (pos1 - pos2).magnitude;
                float diff = currDist - prevDist;

                distance -= diff * 0.01f * scrollSensitivity;
                distance = Mathf.Clamp(distance, minDistance, maxDistance);

                Vector2 avgPrev = (lastTouchPos1 + lastTouchPos2) * 0.5f;
                Vector2 avgCurr = (pos1 + pos2) * 0.5f;

                Vector2 delta = avgCurr - avgPrev;

                Vector3 move =
                    (transform.right * delta.x +
                    transform.forward * delta.y) *
                    (wasdMoveSpeed * 0.005f);

                move.y = 0;
                target.position -= move;
            }

            lastTouchPos1 = pos1;
            lastTouchPos2 = pos2;
        }
    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);
        transform.position = target.position - (rotation * Vector3.forward * distance);
        transform.LookAt(target);
    }
}