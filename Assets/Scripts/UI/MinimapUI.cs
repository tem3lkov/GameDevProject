using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MinimapUI : MonoBehaviour {
    [Header("UI Setup")]
    public GameObject minimapCellPrefab;
    public Transform gridContainer;

    [Header("Scrolling Setup")]
    public float cellSize = 25f;
    public float spacing = 2f;
    private RectTransform gridRect;

    [Header("Sprites")]
    public Sprite defaultRoomSprite;
    public Sprite bossIcon;
    public Sprite itemIcon;
    public Sprite shopIcon;
    public Sprite secretIcon;

    [Header("Colors")]
    public Color currentColor = Color.white; 
    public Color visitedColor = new Color(0.6f, 0.6f, 0.6f, 1f); 
    public Color discoveredColor = new Color(0.3f, 0.3f, 0.3f, 1f); 

    private MinimapCellUI[] mapCells = new MinimapCellUI[100];
    private List<int> visitedRoomIndices = new List<int>();

    private void Awake() {
        gridRect = gridContainer.GetComponent<RectTransform>();
    }

    private void OnEnable() {
        RoomManager.OnMapGenerated += InitializeMap;
        Room.OnRoomEnteredGlobal += UpdateMap;
    }

    private void OnDisable() {
        RoomManager.OnMapGenerated -= InitializeMap;
        Room.OnRoomEnteredGlobal -= UpdateMap;
    }

    private void InitializeMap(Vector2 startPos) {
        foreach (Transform child in gridContainer) Destroy(child.gameObject);
        visitedRoomIndices.Clear();

        for (int i = 0; i < 100; i++) {
            GameObject cellObj = Instantiate(minimapCellPrefab, gridContainer);
            MinimapCellUI cellUI = cellObj.GetComponent<MinimapCellUI>();
            mapCells[i] = cellUI;
            cellUI.SetupEmpty();
        }
    }

    private void UpdateMap(Room currentRoom) {
        if (!visitedRoomIndices.Contains(currentRoom.GetRoomIndex())) {
            visitedRoomIndices.Add(currentRoom.GetRoomIndex());
        }

        var spawnedCells = MapGenerator.Instance.getSpawnedCells;

        for (int i = 0; i < 100; i++) {
            Cell cellData = spawnedCells.FirstOrDefault(c => c.Index == i);

            if (cellData == null) {
                mapCells[i].SetupEmpty();
                continue;
            }

            bool isCurrent = (i == currentRoom.GetRoomIndex());
            bool isVisited = visitedRoomIndices.Contains(i);
            bool isAdjacent = IsAdjacentToVisited(i);

            if (isCurrent) {
                mapCells[i].SetVisuals(defaultRoomSprite, GetIconForRoom(cellData.roomType), currentColor);
            } else if (isVisited) {
                mapCells[i].SetVisuals(defaultRoomSprite, GetIconForRoom(cellData.roomType), visitedColor);
            } else if (isAdjacent && cellData.roomType != RoomType.Secret) {
                mapCells[i].SetVisuals(defaultRoomSprite, GetIconForRoom(cellData.roomType), discoveredColor);
            } else {
                mapCells[i].SetupEmpty();
            }
        }

        CenterMapOnRoom(currentRoom.GetRoomIndex());
    }

    private void CenterMapOnRoom(int roomIndex) {
        if (gridRect == null) return;

        int x = roomIndex % 10;
        int y = roomIndex / 10;

        float step = cellSize + spacing;

        float targetX = (x * step) + (cellSize / 2f);
        float targetY = (y * step) + (cellSize / 2f);

        gridRect.anchoredPosition = new Vector2(-targetX, -targetY);
    }

    private bool IsAdjacentToVisited(int index) {
        int[] adjacentOffsets = { 10, -10, -1, 1 };
        foreach (int offset in adjacentOffsets) {
            int neighbor = index + offset;
            if (neighbor >= 0 && neighbor < 100 && visitedRoomIndices.Contains(neighbor)) {
                return true;
            }
        }
        return false;
    }

    private Sprite GetIconForRoom(RoomType type) {
        switch (type) {
            case RoomType.Boss: return bossIcon;
            case RoomType.Item: return itemIcon;
            case RoomType.Shop: return shopIcon;
            case RoomType.Secret: return secretIcon;
            default: return null;
        }
    }
}