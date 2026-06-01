using UnityEngine;

[RequireComponent(typeof(Room))]
public class ItemRoomLogic : MonoBehaviour
{
    private Room roomLogic;
    private bool hasSpawned = false;

    private void Awake() => roomLogic = GetComponent<Room>();

    private void OnEnable() => roomLogic.OnPlayerEnteredRoom += SpawnPedestalItem;
    private void OnDisable() => roomLogic.OnPlayerEnteredRoom -= SpawnPedestalItem;

    private void SpawnPedestalItem()
    {
        if (hasSpawned) return;
        hasSpawned = true;

        Vector2 spawnPosition = transform.position;

        ItemManager.Instance.SpawnRandomNonResourceItem(spawnPosition);

        Debug.Log("Treasure Room item spawned!");
    }
}