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

        ItemManager.Instance.SpawnRandomItem(leftPos);
        ItemManager.Instance.SpawnRandomItem(center);
        ItemManager.Instance.SpawnRandomItem(rightPos);

        Debug.Log("Shop items spawned!");
    }
}