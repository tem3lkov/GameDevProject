using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Transform child;

    private void Awake() {
        child = transform.GetChild(0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameObject.GetComponent<Button>().interactable)
            child.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        child.gameObject.SetActive(false);
    }
}