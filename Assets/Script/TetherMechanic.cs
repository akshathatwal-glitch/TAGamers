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
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        // FIX 1: positionCount must be set — without this, SetPosition() silently does nothing
        lineRenderer.positionCount = 2;

        lineRenderer.startWidth = 0.15f;
        lineRenderer.endWidth   = 0.15f;

        // FIX 2: Sprites/Default can be null in URP at runtime — find a shader that actually exists
        Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Hidden/InternalErrorShader"); // last resort
        lineRenderer.material = new Material(shader);

        // FIX 3: Foreground sorting layer so it renders in front of the dark background
        lineRenderer.sortingLayerName = "Foreground";
        lineRenderer.sortingOrder     = 20;

        lineRenderer.startColor  = Color.cyan;
        lineRenderer.endColor    = Color.cyan;
        lineRenderer.useWorldSpace = true; // positions are world-space coordinates
    }

    void Update()
    {
        if (player1 == null || player2 == null) return;

        lineRenderer.SetPosition(0, player1.position);
        lineRenderer.SetPosition(1, player2.position);

        float distance = Vector2.Distance(player1.position, player2.position);

        if (distance > maxDistance)
        {
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor   = Color.red;
        }
        else
        {
            lineRenderer.startColor = Color.cyan;
            lineRenderer.endColor   = Color.cyan;
        }
    }
}