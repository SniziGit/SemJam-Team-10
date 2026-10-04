
using UnityEngine.Events;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ColliderTriggerEvent2D : MonoBehaviour // this script uses unity events to perform any public function on the player during collision events
{
    private Collider2D trigger;
    private Rigidbody rb;
    public string respondToTag = "Player";
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    public UnityEvent<GameObject> onTriggerObject;
    public bool oneTime = true;
    private bool hasTriggered = false;
    public float eventCooldown = 0.5f; // minimum time between events
    private float lastEventTime = -9999f;

    private void OnValidate() // initialise the sphere collider if it is not there
    {
        trigger = GetComponent<Collider2D>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<CircleCollider2D>();
            trigger.isTrigger = true;
            CircleCollider2D circle = trigger as CircleCollider2D;
            circle.radius = 5f;
        }
    }
    void Awake()
    {
        if (string.IsNullOrWhiteSpace(respondToTag)) respondToTag = "Player";
        trigger = GetComponent<Collider2D>();
        if (trigger.GetType() != typeof(MeshCollider)) trigger.isTrigger = true;
        rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true; // Make sure the Rigidbody is kinematic to avoid physics interactions
        //rb.useGravity = false; // Disable gravity if not needed
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled) return;//allow enabling and disabling of this script to affect event firing
        if (oneTime && hasTriggered) return;
        if (Time.time - lastEventTime < eventCooldown || !gameObject.activeSelf) return; // still in cooldown
        if (other.CompareTag(respondToTag)) 
        {
            TriggerEnterEvent();

            onTriggerObject?.Invoke(other.gameObject);
            //Debug.LogError($"Triggered by {other.gameObject.name} on {gameObject.name}");
        }
    }
    [ContextMenu("Invoke Trigger Enter")]
    public void TriggerEnterEvent()
    {
       
        onTriggerEnter?.Invoke();
        lastEventTime = Time.time;
        hasTriggered = true;
    }
   
    [ContextMenu("Invoke Trigger Exit")]
    public void TriggerExitEvent()
    {
        onTriggerExit?.Invoke();
    }
   
    void OnTriggerExit2D(Collider2D other)
    {
        if (!enabled) return;//allow enabling and disabling of this script to affect event firing
        if (onTriggerExit != null && other.CompareTag(respondToTag))
        {
            TriggerExitEvent();
        }
    }
}
