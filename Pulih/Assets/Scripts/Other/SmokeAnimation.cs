using UnityEngine;
using System.Collections;

public class SmokeAnimation : MonoBehaviour
{
    public ParticleSystem smokeEffect;

    [Range(0, 255)]
    public int alpha = 100;

    public float delayStart = 1f;
    public float duration = 0.2f;

    Material smokeMaterial;
    Coroutine smokeRoutine;

    void Start()
    {
        if (smokeEffect != null)
        {
            ParticleSystemRenderer renderer =
                smokeEffect.GetComponent<ParticleSystemRenderer>();

            if (renderer != null)
            {
                smokeMaterial = renderer.material;
            }
        }

        SetAlpha(0);
    }

    public void PlaySmoke()
    {
        if (smokeRoutine != null)
        {
            StopCoroutine(smokeRoutine);
        }

        smokeRoutine = StartCoroutine(SmokeRoutine());
    }

    IEnumerator SmokeRoutine()
    {
        SetAlpha(alpha);

        yield return new WaitForSeconds(delayStart);

        float timer = 0f;
        float startAlpha = alpha;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            float currentAlpha =
                Mathf.Lerp(startAlpha, 0, t);

            SetAlpha(currentAlpha);

            yield return null;
        }

        SetAlpha(0);
    }

    void SetAlpha(float value)
    {
        if (smokeMaterial == null)
            return;

        Color color = smokeMaterial.color;

        color.a = value / 255f;

        smokeMaterial.color = color;
    }
}