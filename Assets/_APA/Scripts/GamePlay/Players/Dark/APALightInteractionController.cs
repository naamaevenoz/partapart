using System.Collections;
using APA.Core;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class LightInteractionController : MonoBehaviour
{
    [Header("Light Interaction Settings")]
    [SerializeField] private DarkPlayerMovement darkPlayerMovement;

    [Header("References")]
    [SerializeField] private Transform[] lightCheckPoints;
    [SerializeField] private LayerMask lightBlockerLayer;
    [Header("Settings")]
    [SerializeField] private float maxLightDistance = 3.0f;

    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        StartCoroutine(FixedUpdateCoroutine());
    }
    protected IEnumerator FixedUpdateCoroutine()
    {
        var waitForFixedUpdate = new WaitForFixedUpdate();
        while (true)
        {
            foreach (Transform checkPoint in lightCheckPoints)
            {
                if (checkPoint == null) continue;

                float distanceToLight = Vector2.Distance(transform.position, checkPoint.position);
                if (distanceToLight <= maxLightDistance)
                {
                    RaycastHit2D hit = Physics2D.Linecast(transform.position, checkPoint.position, lightBlockerLayer);

#if UNITY_EDITOR
                    Debug.DrawLine(transform.position, checkPoint.position, hit.collider != null && hit.collider.tag == "Light" ? Color.red : Color.green, 0f);
#endif
                    if (hit.collider != null && hit.collider.tag == "Light")
                    {
                        var pushDirection = Vector2.right * -Mathf.Sign(checkPoint.position.x - rb.position.x);
                        yield return darkPlayerMovement.LightPushDirectionCoroutine(pushDirection);

                        break;
                    }
                }
            }

            yield return waitForFixedUpdate;
        }
    }
}