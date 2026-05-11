using UnityEngine;
using UnityEngine.UI;

public class MinimapCellUI : MonoBehaviour {
    public Image backgroundImage;
    public Image iconImage;

    public void SetupEmpty() {
        backgroundImage.color = new Color(0, 0, 0, 0);
        iconImage.color = new Color(0, 0, 0, 0);
    }
    public void SetVisuals(Sprite bgSprite, Sprite iconSprite, Color bgColor) {
        backgroundImage.sprite = bgSprite;
        backgroundImage.color = bgColor;

        if (iconSprite != null) {
            iconImage.sprite = iconSprite;
            iconImage.color = Color.white;
        } else {
            iconImage.color = new Color(0, 0, 0, 0);
        }
    }
}