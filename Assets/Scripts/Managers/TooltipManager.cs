using UnityEngine;
using TMPro;

public class TooltipManager : SingletonMonoBehaviour<TooltipManager>
{
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    protected override void Awake()
    {
        base.Awake();
        tooltipPanel.SetActive(false);
    }

    public void Show(ItemScriptable item)
    {
        Debug.Log("Show item tooltip");
        nameText.text = item.itemName;
        descriptionText.text = item.description;

        tooltipPanel.SetActive(true);
    }

    public void Hide()
    {
        tooltipPanel.SetActive(false);
    }
}