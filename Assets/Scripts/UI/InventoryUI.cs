using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class InventoryUI : MonoBehaviour
{
    [Header("Resource Text (TextMeshPro)")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI bombText;
    public TextMeshProUGUI keyText;

    [Header("Active Item UI")]
    public Image activeItemImage;

    public GameObject cooldownBarContainer;
    public Image cooldownBarFill;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            UpdateUIVisibility(GameManager.Instance.currentState);
        }

        if (cooldownBarContainer != null) cooldownBarContainer.SetActive(false);
    }

    private void OnEnable()
    {
        PlayerInventory.OnResourcesUpdated += UpdateResources;
        PlayerInventory.OnActiveItemChanged += UpdateActiveItem;
        PlayerInventory.OnCooldownUpdated += UpdateCooldownBar;
        GameManager.OnGameStateChanged += UpdateUIVisibility;
    }

    private void OnDisable()
    {
        PlayerInventory.OnResourcesUpdated -= UpdateResources;
        PlayerInventory.OnActiveItemChanged -= UpdateActiveItem;
        PlayerInventory.OnCooldownUpdated -= UpdateCooldownBar;
        GameManager.OnGameStateChanged -= UpdateUIVisibility;
    }

    private void UpdateUIVisibility(GameState state)
    {
         canvasGroup.alpha = 1f;
    }

    private void UpdateResources(int coins, int keys, int bombs)
    {
        if (coinText != null) coinText.text = coins.ToString("D2");
        if (keyText != null) keyText.text = keys.ToString("D2");
        if (bombText != null) bombText.text = bombs.ToString("D2");
    }

    private void UpdateActiveItem(Sprite itemIcon, bool hasCooldown)
    {
        if (activeItemImage == null) return;

        if (itemIcon != null)
        {
            activeItemImage.sprite = itemIcon;
            activeItemImage.color = Color.white;

            if (cooldownBarContainer != null) cooldownBarContainer.SetActive(hasCooldown);
        } else
        {
            activeItemImage.sprite = null;
            activeItemImage.color = new Color(1, 1, 1, 0);

            if (cooldownBarContainer != null) cooldownBarContainer.SetActive(false);
        }
    }

    private void UpdateCooldownBar(float fillPercentage)
    {
        if (cooldownBarFill != null)
        {
            cooldownBarFill.fillAmount = fillPercentage;
        }
    }
}