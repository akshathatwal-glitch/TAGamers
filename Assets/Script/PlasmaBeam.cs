using UnityEngine;
using System.Collections;

public class PlasmaBeam : MonoBehaviour
{
    public float beamRange = 10f;
    public KeyCode fireKey = KeyCode.E;

    private LineRenderer lr;
    private PlayerController pc;

    // Muzzle flash — a small bright circle that pops at the beam origin
    private SpriteRenderer muzzleFlash;

    void Awake()
    {
        pc = GetComponent<PlayerController>();

        // ── Main beam line ────────────────────────────────────
        lr = gameObject.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.startWidth    = 0.18f;
        lr.endWidth      = 0.04f;

        Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Hidden/InternalErrorShader");

        lr.material           = new Material(shader);
        lr.material.color     = Color.cyan;
        lr.startColor         = Color.white;
        lr.endColor           = Color.cyan;
        lr.sortingLayerName   = "Foreground";
        lr.sortingOrder       = 20;
        lr.useWorldSpace      = true;
        lr.enabled            = false;

        // ── Muzzle flash sprite ───────────────────────────────
        GameObject mfGO = new GameObject("MuzzleFlash");
        mfGO.transform.SetParent(transform, false);

        muzzleFlash                     = mfGO.AddComponent<SpriteRenderer>();
        muzzleFlash.sprite              = CreateCircleSprite();
        muzzleFlash.color               = new Color(0.4f, 0.9f, 1f, 0.9f); // bright cyan
        muzzleFlash.sortingLayerName    = "Foreground";
        muzzleFlash.sortingOrder        = 21;
        muzzleFlash.transform.localScale = Vector3.one * 0.55f;
        muzzleFlash.enabled             = false;

        Debug.Log($"[PlasmaBeam] Ready on '{gameObject.name}'. Shader='{shader?.name}'");
    }

    void Update()
    {
        if (Input.GetKeyDown(fireKey) && pc.playerName == "Player 1")
        {
            Debug.Log("[PlasmaBeam] Fire key pressed. Firing beam.");
            FireBeam();
        }
    }

    void FireBeam()
    {
        DynamicCamera.Instance?.TriggerShake(0.15f, 0.3f);

        // Direction from held movement keys
        Vector2 facingDir = Vector2.right;
        if (Input.GetKey(pc.moveUp))    facingDir = Vector2.up;
        if (Input.GetKey(pc.moveDown))  facingDir = Vector2.down;
        if (Input.GetKey(pc.moveLeft))  facingDir = Vector2.left;
        if (Input.GetKey(pc.moveRight)) facingDir = Vector2.right;

        // FIX: push the ray origin outside the player's own collider
        // so it doesn't immediately hit itself
        Vector2 origin = (Vector2)transform.position + facingDir * 0.8f;

        Debug.Log($"[PlasmaBeam] Direction={facingDir}  Origin={origin}");

        // Ignore the player's own GameObject layer when raycasting
        int layerMask = ~(1 << gameObject.layer);
        RaycastHit2D hit = Physics2D.Raycast(origin, facingDir, beamRange, layerMask);

        // Beam goes from the player centre outward (looks better visually)
        lr.SetPosition(0, (Vector2)transform.position + facingDir * 0.5f);

        if (hit.collider != null)
        {
            lr.SetPosition(1, hit.point);
            Debug.Log($"[PlasmaBeam] Hit '{hit.collider.gameObject.name}' at {hit.point}");

            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                Debug.Log($"[PlasmaBeam] Activating '{hit.collider.gameObject.name}'");
                interactable.Activate(pc);
            }
        }
        else
        {
            lr.SetPosition(1, origin + facingDir * beamRange);
            Debug.Log($"[PlasmaBeam] Hit nothing. Beam extends to max range.");
        }

        // Position muzzle flash at beam start
        muzzleFlash.transform.position = (Vector2)transform.position + facingDir * 0.5f;

        StartCoroutine(ShowBeam());
    }

    IEnumerator ShowBeam()
    {
        // Frame 0 — full brightness, wide beam
        lr.enabled            = true;
        muzzleFlash.enabled   = true;
        lr.startWidth         = 0.18f;
        lr.endWidth           = 0.04f;
        lr.startColor         = Color.white;
        lr.endColor           = new Color(0f, 1f, 1f, 1f);

        yield return new WaitForSeconds(0.04f);

        // Frame 1 — fade to thinner, dimmer
        lr.startWidth  = 0.10f;
        lr.endWidth    = 0.02f;
        lr.startColor  = new Color(0.5f, 0.8f, 1f, 0.7f);
        lr.endColor    = new Color(0f, 1f, 1f, 0.4f);
        muzzleFlash.color = new Color(0.4f, 0.9f, 1f, 0.45f);

        yield return new WaitForSeconds(0.04f);

        // Frame 2 — nearly gone
        lr.startColor  = new Color(0.3f, 0.6f, 1f, 0.3f);
        lr.endColor    = new Color(0f, 1f, 1f, 0.15f);
        muzzleFlash.color = new Color(0.4f, 0.9f, 1f, 0.1f);

        yield return new WaitForSeconds(0.03f);

        lr.enabled          = false;
        muzzleFlash.enabled = false;

        // Reset colours for next shot
        lr.startColor = Color.white;
        lr.endColor   = new Color(0f, 1f, 1f, 1f);
        muzzleFlash.color = new Color(0.4f, 0.9f, 1f, 0.9f);
    }

    // Builds a simple filled-circle sprite at runtime — no texture asset needed
    Sprite CreateCircleSprite()
    {
        int    size    = 32;
        float  radius  = size * 0.5f;
        Texture2D tex  = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dx   = x - radius + 0.5f;
            float dy   = y - radius + 0.5f;
            float dist = Mathf.Sqrt(dx * dx + dy * dy);
            // soft edge
            float alpha = Mathf.Clamp01(1f - (dist - (radius - 2f)) / 2f);
            pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f), size);
    }
}