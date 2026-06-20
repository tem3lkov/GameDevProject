using UnityEngine;

[RequireComponent(typeof(Room))]
public class ItemRoomLogic : MonoBehaviour
{
    private Room roomLogic;
    private bool hasSpawned = false;
    private void Awake() => roomLogic = GetComponent<Room>();

    private void OnEnable() => roomLogic.OnPlayerFirstEnteredRoom += SpawnPedestalItem;
    private void OnDisable() => roomLogic.OnPlayerFirstEnteredRoom -= SpawnPedestalItem;

    private void SpawnPedestalItem()
    {
        if (hasSpawned) return;
        hasSpawned = true;

        Vector2 spawnPosition = transform.position;

        bool forPurchase = false;

        ItemManager.Instance.SpawnRandomNonResourceItem(spawnPosition, forPurchase);

        Debug.Log("Treasure Room item spawned!");
    }
}