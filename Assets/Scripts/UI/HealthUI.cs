using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthUI : MonoBehaviour {
    [Header("UI Sprites")]
    public Sprite fullRedSprite;
    public Sprite halfRedSprite;
    public Sprite emptyContainerSprite;
    public Sprite fullBlueSprite;
    public Sprite halfBlueSprite;

    [Header("Setup")]
    public GameObject heartUIPrefab;
    private List<Image> heartImages = new List<Image>();

    private void OnEnable() {
        PlayerHealth.OnHealthChanged += RedrawHearts;
    }

    private void OnDisable() {
        PlayerHealth.OnHealthChanged -= RedrawHearts;
    }

    private void RedrawHearts(int currentRedHalves, int maxRedHalves, int currentBlueHalves) {
        int maxRedContainers = maxRedHalves / 2;
        int totalBlueContainers = Mathf.CeilToInt(currentBlueHalves / 2f); // Round up for half blues
        int totalSlotsNeeded = maxRedContainers + totalBlueContainers;

        while (heartImages.Count < totalSlotsNeeded) {
            GameObject newHeart = Instantiate(heartUIPrefab, transform);
            heartImages.Add(newHeart.GetComponent<Image>());
        }

        for (int i = 0; i < heartImages.Count; i++) {
            heartImages[i].gameObject.SetActive(i < totalSlotsNeeded);
        }

        for (int i = 0; i < totalSlotsNeeded; i++) {
            if (i < maxRedContainers) {
                int redHalvesForThisSlot = currentRedHalves - (i * 2);

                if (redHalvesForThisSlot >= 2) heartImages[i].sprite = fullRedSprite;
                else if (redHalvesForThisSlot == 1) heartImages[i].sprite = halfRedSprite;
                else heartImages[i].sprite = emptyContainerSprite;
            } else {
                int blueIndex = i - maxRedContainers;
                int blueHalvesForThisSlot = currentBlueHalves - (blueIndex * 2);

                if (blueHalvesForThisSlot >= 2) heartImages[i].sprite = fullBlueSprite;
                else heartImages[i].sprite = halfBlueSprite;
            }
        }
    }
}