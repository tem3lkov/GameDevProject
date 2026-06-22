using UnityEngine;
using TMPro;

[RequireComponent(typeof(Room))]
public class ShopLogic : MonoBehaviour
{
    private Room roomLogic;

    [Header("Shop Setup")]
    public GameObject priceTextPrefab;

    private void Awake() => roomLogic = GetComponent<Room>();

    private void OnEnable() => roomLogic.OnPlayerFirstEnteredRoom += SetupShop;
    private void OnDisable() => roomLogic.OnPlayerFirstEnteredRoom -= SetupShop;

    private void SetupShop()
    {
        Vector2 center = transform.position;
        Vector2 leftPos = center + new Vector2(-2f, 0f);
        Vector2 rightPos = center + new Vector2(2f, 0f);

        ItemManager.Instance.SpawnShopItem(leftPos, priceTextPrefab);
        ItemManager.Instance.SpawnShopItem(center, priceTextPrefab);
        ItemManager.Instance.SpawnShopItem(rightPos, priceTextPrefab);

        Debug.Log("Shop items and dynamic prices spawned!");
    }
}