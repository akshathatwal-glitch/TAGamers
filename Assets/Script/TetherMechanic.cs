using UnityEngine;

public class TetherMechanic : MonoBehaviour
{
    [Header("References")]
    public Transform player1;
    public Transform player2;

    [Header("Tether Settings")]
    public float maxDistance = 7f;
    
    private LineRenderer lineRenderer;

    void Start()
    {
        // Setup a quick visual line between players
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    void Update()
    {
        if (player1 == null || player2 == null) return;

        // Update visual line positions
        lineRenderer.SetPosition(0, player1.position);
        lineRenderer.SetPosition(1, player2.position);

        // Check distance
        float distance = Vector2.Distance(player1.position, player2.position);

        if (distance > maxDistance)
        {
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
            // The circuit is broken! 
            // Optional: You can reduce player speeds here if you pass their references.
        }
        else
        {
            lineRenderer.startColor = Color.cyan;
            lineRenderer.endColor = Color.cyan;
        }
    }
}