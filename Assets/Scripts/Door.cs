using UnityEngine;

public class Door : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    private Color originalColor = Color.white;
    public void SetDoorSprite(Sprite door)
    {
        spriteRenderer.sprite = door;
    }
    public void LockDoor() {
        spriteRenderer.color = Color.red;
    }

    public void UnlockDoor() {
        spriteRenderer.color = originalColor;
    }
}
