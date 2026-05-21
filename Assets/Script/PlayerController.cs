using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Identity")]
    public string playerName = "Player 1";

    [Header("Key Bindings")]
    public KeyCode moveUp     = KeyCode.W;
    public KeyCode moveDown   = KeyCode.S;
    public KeyCode moveLeft   = KeyCode.A;
    public KeyCode moveRight  = KeyCode.D;
    public KeyCode dashKey    = KeyCode.Space;
    public KeyCode actionKey  = KeyCode.E;

    [Header("Movement Settings")]
    public float moveSpeed  = 5f;
    public float dashSpeed  = 15f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;

    // ── Private state ──────────────────────────────────────────
    private Rigidbody2D rb;
    private Vector2     moveInput;
    private bool        isDashing;
    private float       dashTimer;
    private float       cooldownTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        GatherInput();
        HandleDashInput();
        HandleActionInput();

        // Count down cooldown
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
                isDashing = false;
        }
        else
        {
            rb.velocity = moveInput * moveSpeed;
        }
    }

    // ── Input gathering ────────────────────────────────────────
    void GatherInput()
    {
        float x = 0f, y = 0f;
        if (Input.GetKey(moveRight)) x += 1f;
        if (Input.GetKey(moveLeft))  x -= 1f;
        if (Input.GetKey(moveUp))    y += 1f;
        if (Input.GetKey(moveDown))  y -= 1f;
        moveInput = new Vector2(x, y).normalized;
    }

    void HandleDashInput()
    {
        if (!isDashing && cooldownTimer <= 0f && Input.GetKeyDown(dashKey))
        {
            // Dash in move direction, or face direction if stationary
            Vector2 dir = (moveInput.sqrMagnitude > 0.01f) ? moveInput : transform.up;
            rb.velocity = dir * dashSpeed;
            isDashing      = true;
            AchievementManager.Instance?.LogDash(playerName);
            dashTimer      = dashDuration;
            cooldownTimer  = dashCooldown;
        }
        DynamicCamera.Instance?.TriggerShake(0.1f, 0.2f);
    }

    void HandleActionInput()
    {
        if (Input.GetKeyDown(actionKey))
        {
            // Check for nearby interactables with a small overlap circle
            Collider2D hit = Physics2D.OverlapCircle(transform.position, 1.2f,
                LayerMask.GetMask("Interactable"));

            if (hit != null)
            {
                Interactable interactable = hit.GetComponent<Interactable>();
                if (interactable != null)
                    interactable.Activate(this);
            }
        }
    }

    // Draw interaction range in the Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1.2f);
    }
}