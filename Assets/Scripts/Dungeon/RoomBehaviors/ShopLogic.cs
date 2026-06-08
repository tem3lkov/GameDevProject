using UnityEngine;

[RequireComponent(typeof(Room))]
public class ShopLogic : MonoBehaviour
{
    private Room roomLogic;
    private bool hasSpawned = false;

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

        bool forPurchase = true;
        ItemManager.Instance.SpawnRandomItem(leftPos, forPurchase);
        ItemManager.Instance.SpawnRandomItem(center, forPurchase);
        ItemManager.Instance.SpawnRandomItem(rightPos, forPurchase);

        Debug.Log("Shop items spawned!");
    }
}