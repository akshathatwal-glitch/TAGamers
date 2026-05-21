using UnityEngine;
using System.Collections;

public class PlasmaBeam : MonoBehaviour
{
    public float beamRange = 10f;
    public KeyCode fireKey = KeyCode.E;
    
    private LineRenderer lr;
    private PlayerController pc;

    void Awake()
    {
        pc = GetComponent<PlayerController>();
        
        // Setup LineRenderer visually
        lr = gameObject.AddComponent<LineRenderer>();
        lr.startWidth = 0.2f;
        lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.blue;
        lr.endColor = Color.cyan;
        lr.enabled = false;
        lr.sortingOrder = 10;
    }

    void Update()
    {
        if (Input.GetKeyDown(fireKey) && pc.playerName == "Player 1")
        {
            FireBeam();
        }
    }

    void FireBeam()
    {
        StartCoroutine(ShowLaser());
        DynamicCamera.Instance?.TriggerShake(0.15f, 0.3f);

        // Figure out which way the player is moving/facing
        Vector2 facingDir = transform.up; // Default up
        if (Input.GetKey(pc.moveUp)) facingDir = Vector2.up;
        if (Input.GetKey(pc.moveDown)) facingDir = Vector2.down;
        if (Input.GetKey(pc.moveLeft)) facingDir = Vector2.left;
        if (Input.GetKey(pc.moveRight)) facingDir = Vector2.right;

        // Shoot a raycast to see what we hit
        RaycastHit2D hit = Physics2D.Raycast(transform.position, facingDir, beamRange);

        lr.SetPosition(0, transform.position); // Laser starts at player

        if (hit.collider != null)
        {
            lr.SetPosition(1, hit.point); // Laser stops at the wall/object
            
            // Did we hit a button?
            Interactable terminal = hit.collider.GetComponent<Interactable>();
            if (terminal != null)
            {
                terminal.Activate(pc);
            }
        }
        else
        {
            lr.SetPosition(1, (Vector2)transform.position + (facingDir * beamRange)); // Laser shoots into the void
        }
    }

    IEnumerator ShowLaser()
    {
        lr.enabled = true;
        yield return new WaitForSeconds(0.1f); // Flash the laser for a split second
        lr.enabled = false;
    }
}