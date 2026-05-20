using UnityEngine;

public class MovableBlock : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode  pushKey      = KeyCode.RightShift;
    public float    pushForce    = 8f;
    public float    friction     = 3f;   // drag while not being pushed

    private Rigidbody2D rb;
    private bool beingPushed = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.drag = friction;
    }

    void OnCollisionStay2D(Collision2D col)
    {
        // Only react to Player 2 (check by tag or script)
        PlayerController pc = col.gameObject.GetComponent<PlayerController>();
        if (pc == null || pc.playerName != "Player 2") return;

        if (Input.GetKey(pushKey))
        {
            beingPushed = true;
            // Push direction = from player toward block
            Vector2 dir = (transform.position - col.transform.position).normalized;
            rb.AddForce(dir * pushForce, ForceMode2D.Force);
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        beingPushed = false;
    }

    void Update()
    {
        // Increase drag when not pushed so block doesn't slide forever
        rb.drag = beingPushed ? 0.5f : friction;
    }
}