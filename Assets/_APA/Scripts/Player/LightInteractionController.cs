using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class LightInteractionController : MonoBehaviour
{
    [Header("Light Interaction Settings")]
    [SerializeField] private Transform[] lightCheckPoints;
    [SerializeField] private LayerMask lightAreaLayer;
    [SerializeField] private LayerMask lightBlockerLayer;
    [SerializeField] private float lightPushForce = 50f;
    [SerializeField] private float stuckInLightDuration = 0.5f;
    [SerializeField] private float stuckVelocityThreshold = 0.1f;
    [SerializeField] private float maxLightDistance = 3.0f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isInUnblockedLight = false;
    private Vector2 lightPushDirection = Vector2.zero;
    private float stuckInLightTimer = 0f;
    private bool isCurrentlyStuckPlayingAnimation = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & lightAreaLayer.value) != 0)
        {
            CheckLightExposure(other);
        }
    }

    private void FixedUpdate()
    {
        bool applyLightForceThisFrame = isInUnblockedLight;
        isInUnblockedLight = false;

        bool isBeingPushedAndStuck = false;
        if (applyLightForceThisFrame)
        {
            rb.AddForce(lightPushDirection * lightPushForce, ForceMode2D.Force);
            if (rb.linearVelocity.magnitude < stuckVelocityThreshold)
            {
                stuckInLightTimer += Time.fixedDeltaTime;
                if (stuckInLightTimer >= stuckInLightDuration)
                {
                    isBeingPushedAndStuck = true;
                }
            }
            else
            {
                stuckInLightTimer = 0f;
            }
        }
        else
        {
            stuckInLightTimer = 0f;
        }

        if (isBeingPushedAndStuck && animator != null && !isCurrentlyStuckPlayingAnimation)
        {
            animator.SetTrigger("StuckInLightTrigger");
            isCurrentlyStuckPlayingAnimation = true;
            stuckInLightTimer = 0f;
        }
    }

    private void CheckLightExposure(Collider2D lightAreaTrigger)
    {
        LightSourceIdentifier lightSource = lightAreaTrigger.GetComponent<LightSourceIdentifier>();
        if (lightSource == null || lightSource.SourceTransform == null)
            return;

        Vector2 lightOrigin = lightSource.SourceTransform.position;
        bool foundUnblockedLight = false;
        Vector2 cumulativePushDirection = Vector2.zero;

        if (lightCheckPoints == null || lightCheckPoints.Length == 0)
        {
            Debug.LogWarning($"{gameObject.name} has no Light Check Points assigned!");
            return;
        }

        foreach (Transform checkPoint in lightCheckPoints)
        {
            if (checkPoint == null) continue;

            Vector2 playerPoint = checkPoint.position;
            float distanceToLight = Vector2.Distance(playerPoint, lightOrigin);

            if (distanceToLight <= maxLightDistance)
            {
                RaycastHit2D hit = Physics2D.Linecast(playerPoint, lightOrigin, lightBlockerLayer);

#if UNITY_EDITOR
                Debug.DrawLine(playerPoint, lightOrigin, hit.collider == null ? Color.red : Color.green, 0f);
#endif

                if (hit.collider == null)
                {
                    foundUnblockedLight = true;
                    cumulativePushDirection += (playerPoint - lightOrigin).normalized;
                }
            }
        }

        if (foundUnblockedLight)
        {
            isInUnblockedLight = true;
            lightPushDirection = cumulativePushDirection.magnitude > 0.01f
                ? cumulativePushDirection.normalized
                : Vector2.right * -Mathf.Sign(lightOrigin.x - rb.position.x);
        }
    }

    public void ResetStuckStateFromAnimation()
    {
        isCurrentlyStuckPlayingAnimation = false;
    }
}