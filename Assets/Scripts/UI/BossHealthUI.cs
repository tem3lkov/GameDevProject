using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour {
    [Header("UI References")]
    public GameObject healthBarContainer;
    public Image healthFillImage;

    private void OnEnable() {
        Boss.OnBossHealthUpdatedUI += UpdateHealthBar;
        Boss.OnBossFightActiveUI += ToggleHealthBar;
    }

    private void OnDisable() {
        Boss.OnBossHealthUpdatedUI -= UpdateHealthBar;
        Boss.OnBossFightActiveUI -= ToggleHealthBar;
    }

    private void Start() {
        healthBarContainer.SetActive(false);
    }

    private void ToggleHealthBar(bool isActive) {
        healthBarContainer.SetActive(isActive);
    }

    private void UpdateHealthBar(float fillPercentage) {
        healthFillImage.fillAmount = fillPercentage;
    }
}