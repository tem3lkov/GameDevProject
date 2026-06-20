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

    private void OnEnable() => roomLogic.OnPlayerFirstEnteredRoom += SetupShop;
    private void OnDisable() => roomLogic.OnPlayerFirstEnteredRoom -= SetupShop;

    private void SetupShop()
    {
        if (hasSpawned) return;
        hasSpawned = true;

        Vector2 center = transform.position;
        Vector2 leftPos = center + new Vector2(-2f, 0f);
        Vector2 rightPos = center + new Vector2(2f, 0f);

        ItemManager.Instance.SpawnShopItem(leftPos, priceTextPrefab);
        ItemManager.Instance.SpawnShopItem(center, priceTextPrefab);
        ItemManager.Instance.SpawnShopItem(rightPos, priceTextPrefab);

        Debug.Log("Shop items and dynamic prices spawned!");
    }
}