using UnityEngine;

[RequireComponent(typeof(Room))]
public class SecretRoomLogic : MonoBehaviour
{
    private Room roomLogic;

    private void Awake() => roomLogic = GetComponent<Room>();

    private void OnEnable() => roomLogic.OnPlayerFirstEnteredRoom += SetupSecretRoom;
    private void OnDisable() => roomLogic.OnPlayerFirstEnteredRoom -= SetupSecretRoom;
    
    private void SetupSecretRoom()
    {
        Vector2 center = transform.position;
        Vector2 leftPos = center + new Vector2(-2f, 0f);
        Vector2 rightPos = center + new Vector2(2f, 0f);
        Vector2 downPos = center + new Vector2(0f, -2f);
        Vector2 upPos = center + new Vector2(0f, 2f);

        ItemManager.Instance.SpawnRandomResource(leftPos, false);
        ItemManager.Instance.SpawnRandomResource(rightPos, false);
        ItemManager.Instance.SpawnRandomResource(downPos, false);
        ItemManager.Instance.SpawnRandomResource(upPos, false);

        Debug.Log("Secret room items spawned!");
    }
    
}
