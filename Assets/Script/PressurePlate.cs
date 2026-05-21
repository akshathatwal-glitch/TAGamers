using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [Header("Settings")]
    public float requiredMass = 1f;

    [Header("Events")]
    public UnityEvent onPressed;
    public UnityEvent onReleased;

    private int objectsOnPlate = 0;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        Debug.Log($"[PressurePlate] '{gameObject.name}' initialized. RequiredMass={requiredMass}. Collider isTrigger should be TRUE.");

        // Safety check — warn if collider isn't a trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
            Debug.LogWarning($"[PressurePlate] '{gameObject.name}' collider is NOT set to isTrigger! Objects will never enter it. Fix in Inspector.");
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log($"[PressurePlate] OnTriggerEnter2D — '{col.gameObject.name}' entered plate '{gameObject.name}'.");

        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.Log($"[PressurePlate] '{col.gameObject.name}' has no Rigidbody2D, ignoring.");
            return;
        }

        Debug.Log($"[PressurePlate] '{col.gameObject.name}' mass={rb.mass}, requiredMass={requiredMass}.");

        if (rb.mass >= requiredMass)
        {
            objectsOnPlate++;
            Debug.Log($"[PressurePlate] Valid object! objectsOnPlate={objectsOnPlate}. Firing onPressed.");
            if (objectsOnPlate == 1)
            {
                if (sr != null) sr.color = Color.cyan;
                DynamicCamera.Instance?.TriggerShake(0.1f, 0.1f);
                onPressed?.Invoke();
                Debug.Log($"[PressurePlate] onPressed invoked on '{gameObject.name}'.");
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Debug.Log($"[PressurePlate] OnTriggerExit2D — '{col.gameObject.name}' left plate '{gameObject.name}'.");

        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        if (rb.mass >= requiredMass)
        {
            objectsOnPlate--;
            Debug.Log($"[PressurePlate] objectsOnPlate={objectsOnPlate}.");
            if (objectsOnPlate <= 0)
            {
                objectsOnPlate = 0;
                if (sr != null) sr.color = Color.white;
                onReleased?.Invoke();
                Debug.Log($"[PressurePlate] onReleased invoked on '{gameObject.name}'.");
            }
        }
    }
}