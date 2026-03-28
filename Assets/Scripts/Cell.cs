using UnityEngine;
using System.Collections.Generic;
public enum RoomType
{
    Normal,
    Item,
    Shop,
    Secret,
    Boss
}

public class Cell : MonoBehaviour
{
    public int Index { get; set; }
    public int Value { get; set; }



    [SerializeField] private SpriteRenderer spriteRenderer;
    public RoomType roomType;
    public List<int> cellList = new List<int>();

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void SetRoomType(RoomType type)
    {
        roomType = type;
    }

}
