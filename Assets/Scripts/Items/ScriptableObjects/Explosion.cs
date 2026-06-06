using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Explosion : MonoBehaviour
{
    public float explosionRadius;
    public float explosionDamage;
    public float explosionDelay;
    [SerializeField] private Transform aoeIndicator;

    [Header("Tilemap Drops")]
    [Tooltip("Drag your Blue Stone Tile asset from the Project window here")]
    public TileBase blueStoneTile;
    [Tooltip("Drag your Blue Health Item Prefab here")]
    public GameObject blueHealthPrefab;

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
                        // 1. IDENTIFY THE TILE: Get the specific tile we are about to destroy
                        TileBase hitTile = map.GetTile(checkPos);

                        // 2. CHECK IT: Is it the Blue Stone?
                        if (hitTile == blueStoneTile)
                        {
                            // Spawn the blue health exactly where the rock was!
                            if (blueHealthPrefab != null)
                            {
                                Instantiate(blueHealthPrefab, tileWorldPos, Quaternion.identity);
                            }
                        }

                        // 3. Delete the rock!
                        map.SetTile(checkPos, null);

                        UpdatePathfindingGrid(tileWorldPos);
                    }
                }
            }
        }
    }

    private void UpdatePathfindingGrid(Vector3 clearedPosition)
    {
        AStarGrid roomGrid = GetComponentInParent<AStarGrid>();
        if (roomGrid != null) roomGrid.InitializeGrid();
    }

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

            if (hit.TryGetComponent(out TilemapCollider2D tilemapCollider))
            {
                Tilemap rockMap = tilemapCollider.GetComponent<Tilemap>();
                DestroyTilesInRadius(rockMap);
            }
        }
        Destroy(gameObject);
    }

}
