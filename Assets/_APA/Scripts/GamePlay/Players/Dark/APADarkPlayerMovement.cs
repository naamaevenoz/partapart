using System.Collections;
using APA.Core;
using UnityEngine;

public class DarkPlayerMovement : PlayerMovement
{
    [Header("Settings - push")]
    [SerializeField] private float lightPushForce = 50f;
    [SerializeField] private float pushDuration = 0.5f;

    public void StartFixedUpdateCoroutine(bool isEnable)
    {
        if (isEnable)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(FixedUpdateCoroutine());
        }
        else
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
    }
    public IEnumerator LightPushDirectionCoroutine(Vector2 lightPushDirection)
    {
        APADebug.Log($"LightPushDirectionCoroutine");
        StartFixedUpdateCoroutine(false);
        float elpased = 0;
        while (elpased < pushDuration)
        {
            rb.AddForce(lightPushDirection * lightPushForce, ForceMode2D.Force);

            elpased += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        StartFixedUpdateCoroutine(true);
    }
}
