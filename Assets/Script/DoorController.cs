using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 openOffset = new Vector3(0, 3f, 0); // How far the door moves up
    public float speed = 5f;

    private Vector3 closedPosition;
    private Vector3 targetPosition;

    void Start()
    {
        closedPosition = transform.position;
        targetPosition = closedPosition;
    }

    void Update()
    {
        // Smoothly slide to the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    public void OpenDoor()
    {
        targetPosition = closedPosition + openOffset;
    }

    public void CloseDoor()
    {
        targetPosition = closedPosition;
    }
}