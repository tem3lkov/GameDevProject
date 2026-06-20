using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RunData
{
    public int runSeed;
    public int currentLevel;

    public int maxHealth;
    public int redHealth;
    public int blueHealth;

    public int bombs;
    public int keys;
    public int coins;

    public string activeItemID;
    public List<string> passiveItemIDs = new();
}