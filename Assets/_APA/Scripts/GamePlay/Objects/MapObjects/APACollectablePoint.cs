namespace _APA.Scripts.Managers
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(Collider2D))]
    public class CollectablePoint : MonoBehaviour
    {
        public enum CollectableType { Light, Dark }

        [Header("Collection Settings")]
        [Tooltip("Which type of player can collect this point?")]
        [SerializeField] private CollectableType requiredPlayerType;

        [Header("Event Manager Signal")]
        [Tooltip("The unique ID of the object this point activates via EventManager upon collection.")]
        [SerializeField] private string targetObjectID;

        [Header("Optional Feedback")]
        [Tooltip("Invoked when the point is successfully collected (before destruction).")]
        public UnityEvent OnCollectedFeedback;

        private bool isCollected = false;

        void Awake()
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null && !col.isTrigger)
            {
                Debug.LogWarning($"CollectablePoint on '{gameObject.name}' requires Collider2D to be trigger.", this);
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isCollected) return;

            string otherLayerName = LayerMask.LayerToName(other.gameObject.layer);

            bool isLightMatch = requiredPlayerType == CollectableType.Light && otherLayerName == "LightPlayer";
            bool isDarkMatch = requiredPlayerType == CollectableType.Dark && otherLayerName == "DarkPlayer";

            if (isLightMatch || isDarkMatch)
            {
                Collect();
            }
            else
            {
                Debug.Log($"CollectablePoint: '{gameObject.name}' ignored collision with '{other.gameObject.name}' (Layer: {otherLayerName}) – required: {requiredPlayerType}.");
            }
        }

        private void Collect()
        {
            isCollected = true;

            if (!string.IsNullOrEmpty(targetObjectID))
            {
                EventManager.TriggerObjectActivate(targetObjectID, this.gameObject);
            }

            OnCollectedFeedback?.Invoke();
            Destroy(gameObject);
        }
    }
}
