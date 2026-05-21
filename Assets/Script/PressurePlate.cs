using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [Header("Settings")]
    public float requiredMass = 1f; // How heavy does the object need to be?
    
    [Header("Events")]
    public UnityEvent onPressed;
    public UnityEvent onReleased;

    private int objectsOnPlate = 0;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
        if (rb != null && rb.mass >= requiredMass)
        {
            objectsOnPlate++;
            if (objectsOnPlate == 1) // Only trigger once when the first valid object steps on
            {
                sr.color = Color.cyan; // Visual feedback
                DynamicCamera.Instance?.TriggerShake(0.1f, 0.1f); // Satisfying "clunk"
                onPressed?.Invoke();
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
        if (rb != null && rb.mass >= requiredMass)
        {
            objectsOnPlate--;
            if (objectsOnPlate <= 0) // Only release if EVERYTHING gets off the plate
            {
                objectsOnPlate = 0;
                sr.color = Color.white;
                onReleased?.Invoke();
            }
        }
    }
}