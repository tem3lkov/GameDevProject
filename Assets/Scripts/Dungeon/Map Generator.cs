using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class MapGenerator : SingletonMonoBehaviour<MapGenerator>
{
    private int[] floorCells;
    public int[] getFloorCells => floorCells;

    private int floorCellsCount;
    private int minRoomCount;
    private int maxRoomCount;

    private List<int> endRooms;
    private List<Cell> spawnedCells;
    private Queue<int> cellQueue;
    public List<Cell> getSpawnedCells => spawnedCells;

    private int bossRoomIndex;
    private int itemRoomIndex;
    private int shopRoomIndex;
    private int secretRoomIndex;

    private float cellSize;

    [SerializeField] private Cell cellPrefab;

    [Header("Sprite References")]
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private Sprite shopSprite;
    [SerializeField] private Sprite secretSprite;
    [SerializeField] private Sprite bossSprite;

    private System.Random mapRng;

    private void Start()
    {
        cellSize = 0.16f;
        spawnedCells = new List<Cell>();

        int level = GameManager.Instance.currentLevel;

        minRoomCount = 3 + (level * 3);
        maxRoomCount = minRoomCount + 2;

        int floorSeed = GameManager.Instance.GetCurrentSeed() + level;
        mapRng = new System.Random(floorSeed);

        SetupFloor();
    }

    private void SetupFloor()
    {
        for (int i = 0; i < spawnedCells.Count; i++)
        {
            Destroy(spawnedCells[i].gameObject);
        }

        spawnedCells.Clear();
        floorCells = new int[100];

        floorCellsCount = default;

        cellQueue = new Queue<int>();
        endRooms = new List<int>();

        VisitCell(45);

        GenerateFloor();
    }

    private void GenerateFloor()
    {
        while (cellQueue.Count > 0)
        {
            int index = cellQueue.Dequeue();
            int x = index % 10;
            int branchesCreated = 0;

            if (x > 1 && VisitCell(index - 1)) branchesCreated++;
            if (x < 9 && VisitCell(index + 1)) branchesCreated++;
            if (index > 20 && VisitCell(index - 10)) branchesCreated++;
            if (index < 70 && VisitCell(index + 10)) branchesCreated++;

            if (branchesCreated == 0)
            {
                endRooms.Add(index);
            }
        }

        if (floorCellsCount < minRoomCount)
        {
            SetupFloor();
            return;
        }
        SetupSpecialRooms();
    }

    private void SetupSpecialRooms()
    {
        bossRoomIndex = endRooms.Count > 0 ? endRooms[endRooms.Count - 1] : -1;

        if (bossRoomIndex != -1)
        {
            endRooms.RemoveAt(endRooms.Count - 1);
        }

        itemRoomIndex = GetRandomEndRoom();
        shopRoomIndex = GetRandomEndRoom();
        secretRoomIndex = GetRandomSecretRoom();

        if (itemRoomIndex == -1 || shopRoomIndex == -1 || secretRoomIndex == -1)
        {
            SetupFloor();
            return;
        }

        SpawnRoom(secretRoomIndex);
        UpdateSpecialRoomSprites();
        floorCells[secretRoomIndex] = 1;
        RoomManager.Instance.SetupRooms(spawnedCells);
    }

    private void UpdateSpecialRoomSprites()
    {
        foreach (Cell cell in spawnedCells)
        {
            if (cell.Index == 45)
            {
                cell.SetRoomType(RoomType.Enterance);
            }
            if (cell.Index == bossRoomIndex)
            {
                cell.SetSprite(bossSprite);
                cell.SetRoomType(RoomType.Boss);
            } else if (cell.Index == itemRoomIndex)
            {
                cell.SetSprite(itemSprite);
                cell.SetRoomType(RoomType.Item);
            } else if (cell.Index == shopRoomIndex)
            {
                cell.SetSprite(shopSprite);
                cell.SetRoomType(RoomType.Shop);
            } else if (cell.Index == secretRoomIndex)
            {
                cell.SetSprite(secretSprite);
                cell.SetRoomType(RoomType.Secret);
            }
        }
    }

    private int GetNeighbourCount(int index)
    {
        return floorCells[index - 10] + floorCells[index + 10] + floorCells[index - 1] + floorCells[index + 1];
    }

    private void SpawnRoom(int index)
    {
        int x = index % 10;
        int y = index / 10;

        Vector2 pos = new Vector2(x * cellSize, y * cellSize);

        Cell newCell = Instantiate(cellPrefab, pos, Quaternion.identity);
        newCell.Index = index;
        newCell.SetRoomType(RoomType.Normal);
        newCell.cellList.Add(index);

        spawnedCells.Add(newCell);
    }

    private int GetRandomEndRoom()
    {
        if (endRooms.Count == 0) return -1;

        int randomRoom = mapRng.Next(0, endRooms.Count);
        int roomIndex = endRooms[randomRoom];

        endRooms.RemoveAt(randomRoom);
        return roomIndex;
    }

    private int GetRandomSecretRoom()
    {
        int maxAttempts = 600;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int x = mapRng.Next(1, 10);
            int y = mapRng.Next(2, 10);

            int index = (y * 10) + x;

            if (!IsCellEligibleForSecretRoom(index))
            {
                continue;
            }

            int neighbours = GetNeighbourCount(index);

            int requiredNeighbours = 3;

            if (attempt > 400) requiredNeighbours = 1;
            else if (attempt > 200) requiredNeighbours = 2;

            if (neighbours >= requiredNeighbours)
            {
                return index;
            }
        }

        return -1;
    }

    private bool IsCellEligibleForSecretRoom(int index)
    {
        if (floorCells[index] != 0)
            return false;

        if (index - 10 < 0 || index + 10 >= floorCells.Length)
            return false;

        bool isNextToBoss = (index - 1 == bossRoomIndex) ||
                            (index + 1 == bossRoomIndex) ||
                            (index - 10 == bossRoomIndex) ||
                            (index + 10 == bossRoomIndex);

        return !isNextToBoss;
    }

    private bool VisitCell(int index)
    {
        if (floorCells[index] != 0 || GetNeighbourCount(index) > 1 || floorCellsCount > maxRoomCount || mapRng.NextDouble() < 0.5)
            return false;

        cellQueue.Enqueue(index);
        floorCells[index] = 1;
        floorCellsCount++;

        SpawnRoom(index);

        return true;
    }
}