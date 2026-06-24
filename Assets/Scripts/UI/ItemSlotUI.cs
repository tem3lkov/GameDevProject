using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (PlayerInventory.Instance.GetActiveItem() == null) return;

        ItemScriptable data = (ItemScriptable)PlayerInventory.Instance.GetActiveItem();
        TooltipManager.Instance.Show(data);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.Hide();
    }
}