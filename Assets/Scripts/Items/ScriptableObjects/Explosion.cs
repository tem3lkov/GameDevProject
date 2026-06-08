using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Explosion : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius;
    public float explosionDamage;
    public float explosionDelay;
    [SerializeField] private Transform aoeIndicator;

    [Header("Tilemap Drops")]
    [Tooltip("Drag your Blue Stone Tile asset from the Project window here")]
    public TileBase blueStoneTile;
    [Tooltip("Drag your Blue Health Item Prefab here")]
    public GameObject blueHealthPrefab;

    [Tooltip("Set this to the layer your destructible rocks are on! (e.g., Obstacle)")]
    public LayerMask destructibleTileLayer;

    public void TriggerExplode()
    {
        StartCoroutine(FadeOut(explosionDelay));
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
            // Damage enemies/player
            if (hit.TryGetComponent(out IDamageable dmg))
            {
                dmg.TakeDamage(explosionDamage);
            }

            // Destroy basic destructible objects (like pots)
            if (hit.TryGetComponent(out IDestructible destr))
            {
                destr.UponDestruction();
            }

            if (hit.TryGetComponent(out TilemapCollider2D tilemapCollider))
            {
                if ((destructibleTileLayer.value & (1 << hit.gameObject.layer)) > 0)
                {
                    Tilemap hitRockMap = tilemapCollider.GetComponent<Tilemap>();
                    DestroyTilesInRadius(hitRockMap);
                }
            }
        }

        StartCoroutine(CleanupAndBroadcast());
    }

    private void DestroyTilesInRadius(Tilemap map)
    {
        Vector3Int centerCell = map.WorldToCell(transform.position);
        float currentCellSize = map.layoutGrid.cellSize.x;
        int cellRadius = Mathf.CeilToInt(explosionRadius / currentCellSize);

        for (int x = -cellRadius; x <= cellRadius; x++)
        {
            for (int y = -cellRadius; y <= cellRadius; y++)
            {
                Vector3Int checkPos = new Vector3Int(centerCell.x + x, centerCell.y + y, 0);

                if (map.HasTile(checkPos))
                {
                    Vector3 tileWorldPos = map.GetCellCenterWorld(checkPos);
                    if (Vector2.Distance(tileWorldPos, transform.position) <= explosionRadius)
                    {
                        TileBase hitTile = map.GetTile(checkPos);

                        if (hitTile == blueStoneTile && blueHealthPrefab != null)
                        {
                            Instantiate(blueHealthPrefab, tileWorldPos, Quaternion.identity);
                        }

                        map.SetTile(checkPos, null);
                    }
                }
            }
        }
    }

    private IEnumerator CleanupAndBroadcast()
    {
        if (TryGetComponent(out SpriteRenderer sr)) sr.enabled = false;
        if (TryGetComponent(out Collider2D col)) col.enabled = false;
        if (aoeIndicator != null) aoeIndicator.gameObject.SetActive(false);

        yield return new WaitForFixedUpdate();

        GridEventManager.TriggerEnvironmentChanged(transform.position, explosionRadius);

        Destroy(gameObject);
    }
}