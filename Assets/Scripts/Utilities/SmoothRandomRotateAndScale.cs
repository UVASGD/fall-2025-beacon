using UnityEngine;
using System.Collections;

public class SmoothRandomRotateAndScale : MonoBehaviour
{
    [Header("Animation Settings")]
    public float growDuration = 1f;
    public float rotateInterval = 0.33f;
    public float rotateLerpSpeed = 5f;
    public float shrinkDuration = 0.5f;

    private Vector3 fullScale;
    private Quaternion targetRotation;
    private bool isShrinking = false;

    void Awake()
    {
        fullScale = transform.localScale;
        transform.localScale = Vector3.zero;
        targetRotation = transform.rotation;

        StartCoroutine(GrowAndRotateSimultaneously());
    }

    private IEnumerator GrowAndRotateSimultaneously()
    {
        float growTime = 0f;
        float rotateTimer = 0f;

        while (!isShrinking)
        {
            // --- Grow until full scale ---
            if (growTime < growDuration)
            {
                growTime += Time.deltaTime;
                float t = Mathf.Clamp01(growTime / growDuration);
                transform.localScale = Vector3.Lerp(Vector3.zero, fullScale, t);
            }
            else
            {
                transform.localScale = fullScale;
            }

            // --- Pick new rotation target every interval ---
            rotateTimer += Time.deltaTime;
            if (rotateTimer >= rotateInterval)
            {
                rotateTimer = 0f;
                float randomAngle = Random.Range(0f, 360f);
                targetRotation = Quaternion.Euler(0f, randomAngle, 0f);
            }

            // --- Smoothly rotate toward target ---
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateLerpSpeed);

            yield return null;
        }
    }

    /// <summary>
    /// Smoothly shrinks the GameObject and destroys it after shrinkDuration seconds.
    /// </summary>
    public void ShrinkAndDestroy()
    {
        if (!gameObject.activeInHierarchy || isShrinking)
            return;

        isShrinking = true;
        StopAllCoroutines();
        StartCoroutine(ShrinkCoroutine());
    }

    private IEnumerator ShrinkCoroutine()
    {
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / shrinkDuration);
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }

        Destroy(gameObject);
    }
}
