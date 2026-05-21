using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 openOffset = new Vector3(0, 3f, 0);
    public float speed = 5f;

    private Vector3 closedPosition;
    private Vector3 targetPosition;

    void Start()
    {
        closedPosition = transform.position;
        targetPosition = closedPosition;
        Debug.Log($"[DoorController] '{gameObject.name}' ready. ClosedPos={closedPosition}, OpenPos={closedPosition + openOffset}");
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    public void OpenDoor()
    {
        targetPosition = closedPosition + openOffset;
        Debug.Log($"[DoorController] OpenDoor() called on '{gameObject.name}'. Moving to {targetPosition}");
    }

    public void CloseDoor()
    {
        targetPosition = closedPosition;
        Debug.Log($"[DoorController] CloseDoor() called on '{gameObject.name}'. Moving to {targetPosition}");
    }
}