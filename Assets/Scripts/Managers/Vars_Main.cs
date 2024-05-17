using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OperatorInfos
{
    public string name;
    public string name_e;
    public string Inst_Weapon;
    public string Inst_Prop;
    public int id;
    public bool IsLD;
    public Player player;
    public Sprite Head;
    public Sprite Standing;
    public Sprite Standing2;
}

[System.Serializable]
public class GameStatus
{
    public int[] Objects;
    public int[] Stat;
    public int[] Enem;
    public List<int> LastBatch;
    public List<bool> Unlock_Char;
    public List<bool> Unlock_Item;

    public GameStatus()
    {
        Objects = new int[3] {0,0,0};
        Stat = new int[10] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        Enem = new int[6] { 0, 0, 0, 0, 0 ,0}; 
        LastBatch = new List<int>() {};
    }
}
