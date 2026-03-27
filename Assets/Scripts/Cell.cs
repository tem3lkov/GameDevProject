using UnityEngine;

public class Cell : MonoBehaviour
{
    public int Index { get; set; }
    public int Value { get; set; }

    [SerializeField] private SpriteRenderer spriteRenderer;

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

}
