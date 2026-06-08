using UnityEngine;
using TMPro;

[RequireComponent(typeof(Room))]
public class ShopLogic : MonoBehaviour
{
    private Room roomLogic;
    private bool hasSpawned = false;

    [Header("Shop Setup")]
    public GameObject priceTextPrefab;

    // Notice we completely removed the manual left/center/right prices!

    private void Awake() => roomLogic = GetComponent<Room>();

    private void OnEnable() => roomLogic.OnPlayerEnteredRoom += SetupShop;
    private void OnDisable() => roomLogic.OnPlayerEnteredRoom -= SetupShop;

    private void SetupShop()
    {
        if (hasSpawned) return;
        hasSpawned = true;

        Vector2 center = transform.position;
        Vector2 leftPos = center + new Vector2(-2f, 0f);
        Vector2 rightPos = center + new Vector2(2f, 0f);

        SpawnShopItem(leftPos);
        SpawnShopItem(center);
        SpawnShopItem(rightPos);

        Debug.Log("Shop items and dynamic prices spawned!");
    }

    private void SpawnShopItem(Vector2 position)
    {
        Item spawnedItem = ItemManager.Instance.SpawnRandomItem(position, true);

        if (priceTextPrefab != null && spawnedItem != null)
        {
            GameObject textObj = Instantiate(priceTextPrefab, position, Quaternion.identity);

            textObj.transform.SetParent(spawnedItem.transform);

            textObj.transform.localPosition = new Vector3(0f, -0.7f, 0f);

            if (textObj.TryGetComponent(out TextMeshPro textMesh))
            {
                int cost = spawnedItem.GetInventoryItemData().itemPrice;
                textMesh.text = cost.ToString() + "¢";
            }
        }
    }
}