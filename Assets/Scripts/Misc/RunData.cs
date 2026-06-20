using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RunData
{
    public int maxHealth;
    public int redHealth;
    public int blueHealth;

    public int bombs;
    public int keys;
    public int coins;

    public string activeItemID;
    public List<string> passiveItemIDs = new();

    //room matrix or something of the like
    //public int currentRoom;

}
