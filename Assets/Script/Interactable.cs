using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Settings")]
    public string requiredPlayer = "Player 1"; // leave blank = any player
    public bool   toggleable     = false;

    [Header("Events")]
    public UnityEvent onActivate;
    public UnityEvent onDeactivate;

    private bool isActive = false;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // ── Called by PlasmaBeam raycast hit ──────────────────────
    public void Activate(PlayerController caller)
    {
        if (!string.IsNullOrEmpty(requiredPlayer) && caller.playerName != requiredPlayer)
        {
            Debug.Log($"[Interactable] {caller.playerName} tried to activate '{gameObject.name}' but it requires '{requiredPlayer}'. Blocked.");
            return;
        }

        if (toggleable && isActive)
        {
            isActive = false;
            SetVisual(false);
            Debug.Log($"[Interactable] '{gameObject.name}' DEACTIVATED by {caller.playerName}.");
            onDeactivate?.Invoke();
        }
        else
        {
            isActive = true;
            SetVisual(true);
            Debug.Log($"[Interactable] '{gameObject.name}' ACTIVATED by {caller.playerName}.");
            AchievementManager.Instance?.LogInteraction();
            onActivate?.Invoke();
        }
    }

    // ── FIX: Walk-on trigger detection ────────────────────────
    // Smol uses a trigger BoxCollider2D — this is what actually fires when a player steps on it
    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log($"[Interactable] OnTriggerEnter2D on '{gameObject.name}' — collider: {col.gameObject.name}");

        PlayerController pc = col.GetComponent<PlayerController>();
        if (pc == null)
        {
            Debug.Log($"[Interactable] '{col.gameObject.name}' has no PlayerController, ignoring.");
            return;
        }

        Activate(pc);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Debug.Log($"[Interactable] OnTriggerExit2D on '{gameObject.name}' — collider: {col.gameObject.name}");

        PlayerController pc = col.GetComponent<PlayerController>();
        if (pc == null) return;

        // If not toggleable, deactivate when player leaves
        if (!toggleable && isActive)
        {
            isActive = false;
            SetVisual(false);
            Debug.Log($"[Interactable] '{gameObject.name}' DEACTIVATED — {pc.playerName} stepped off.");
            onDeactivate?.Invoke();
        }
    }

    void SetVisual(bool active)
    {
        if (sr != null)
            sr.color = active ? Color.green : Color.white;
    }
}