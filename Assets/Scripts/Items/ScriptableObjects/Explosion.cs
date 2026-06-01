using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
    public float explosionRadius;
    public float explosionDamage;
    public float explosionDelay;
    [SerializeField] private Transform aoeIndicator;

    public void TriggerExplode()
    {
        StartCoroutine(FadeOut(3f));
        StartCoroutine(ExplodeAfterDelay());
    }
    private IEnumerator FadeOut(float duration)
    {
        SpriteRenderer sr = aoeIndicator.GetComponent<SpriteRenderer>();
        Color start = sr.color;
        float scale = explosionRadius * 1.3f;
        aoeIndicator.localScale = new Vector3(scale, scale, 1f);

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(start.a, 0f, t / duration);

            sr.color = new Color(start.r, start.g, start.b, alpha);

            yield return null;
        }
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    private void Explode()
    {            
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in hits)
        {
            var dmg = hit.GetComponent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(explosionDamage);
            }
            
            var destr = hit.GetComponent<IDestructible>();
            if (destr != null)
            {
                destr.UponDestruction();
            }
        }
        Destroy(gameObject);
    }

}
