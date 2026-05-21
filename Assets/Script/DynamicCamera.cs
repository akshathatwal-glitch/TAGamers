using UnityEngine;
using System.Collections;

public class DynamicCamera : MonoBehaviour
{
    public static DynamicCamera Instance;

    [Header("Targets")]
    public Transform player1;
    public Transform player2;

    [Header("Camera Zoom Settings")]
    public float minZoom = 5f;
    public float maxZoom = 10f;
    public float zoomLimiter = 10f;
    
    [Header("Smoothing")]
    public float smoothTime = 0.5f;
    private Vector3 velocity;
    private Camera cam;

    [Header("Screen Shake")]
    public float shakeDuration = 0f;
    public float shakeMagnitude = 0.7f;
    private Vector3 originalPos;

    void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (player1 == null || player2 == null) return;

        MoveCamera();
        ZoomCamera();
        
        if (shakeDuration > 0)
        {
            transform.localPosition = transform.position + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime;
        }
    }

    void MoveCamera()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + new Vector3(0, 0, -10f); // Keep Z offset
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    void ZoomCamera()
    {
        float distance = Vector3.Distance(player1.position, player2.position);
        float newZoom = Mathf.Lerp(maxZoom, minZoom, distance / zoomLimiter);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
    }

    Vector3 GetCenterPoint()
    {
        Bounds bounds = new Bounds(player1.position, Vector3.zero);
        bounds.Encapsulate(player2.position);
        return bounds.center;
    }

    // Call this from other scripts! e.g., DynamicCamera.Instance.TriggerShake(0.2f, 0.5f);
    public void TriggerShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}