using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Settings")]
    public string requiredPlayer = "Player 1"; // leave blank = any player
    public bool   toggleable     = false;       // true = press again to deactivate

    [Header("Events")]
    public UnityEvent onActivate;
    public UnityEvent onDeactivate;

    private bool isActive = false;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Activate(PlayerController caller)
    {
        // Gate by player name if required
        if (!string.IsNullOrEmpty(requiredPlayer) &&
            caller.playerName != requiredPlayer)
        {
            Debug.Log($"{caller.playerName} cannot activate this — needs {requiredPlayer}.");
            return;
        }

        if (toggleable && isActive)
        {
            isActive = false;
            SetVisual(false);
            onDeactivate?.Invoke();
        }
        else
        {
            isActive = true;
            SetVisual(true);
            AchievementManager.Instance?.LogInteraction();
            onActivate?.Invoke();
        }
    }

    void SetVisual(bool active)
    {
        if (sr != null)
            sr.color = active ? Color.green : Color.white;
    }
}