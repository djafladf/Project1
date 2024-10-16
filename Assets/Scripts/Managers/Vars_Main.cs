using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DoubleArray<T>
{
    List<List<T>> array;
}
[System.Serializable]
public class OperatorInfos
{
    public string name;
    public string name_e;
    public string Inst_Weapon;
    public string Inst_Prop;
    public int id;
    public int CurAwaken;
    public string[] AwakenInfo;
    public bool IsLD;
    public bool IsPriorityAttack;
    public Player player;
    public Sprite Head;
    public Sprite Standing;
    public Sprite Standing2;
}

[System.Serializable]
public class GameStatus
{
    // KeySetting
    public string[] MoveKey;
    public string PauseKey;
    public string[] UnitKey;
    public bool[][][] UnitKeySetting;

    // GameSetting
    public float[] Sounds;      // Game, Unit, Effect
    public bool IsShowDamage;
    public float AttackAlpha;

    // GameVar
    public int LastVersion;
    public int[] Objects;
    public int[] Stat;
    public int[] Enem;
    public int[] Exceed;
    public List<int> LastBatch;
    public List<bool> Unlock_Char;
    public List<bool> Unlock_Item;

    public GameStatus()
    {
        MoveKey = new string[4] { "<Keyboard>/w", "<Keyboard>/s", "<Keyboard>/a", "<Keyboard>/d" };
        PauseKey = "<Keyboard>/escape";
        UnitKey = new string[3] { "<Keyboard>/1","<Keyboard>/2","<Keyboard>/3" };
        UnitKeySetting = new bool[3][][]
        {
            new[] { new bool[2], new bool[2], new bool[2] },
            new[] { new bool[2], new bool[2], new bool[2] },
            new[] { new bool[2], new bool[2], new bool[2] }
        };
        Sounds = new float[3] { 1, 1, 1 };
        IsShowDamage = true; AttackAlpha = 1;

        Objects = new int[3] {0,0,0};
        Stat = new int[10] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        Enem = new int[6] { 0, 0, 0, 0, 0 ,0};
        Exceed = new int[17];
        LastBatch = new List<int>() {};
    }

    public void ResetVars(bool IsResetGameVar = false)
    {
        MoveKey = new string[4] { "<Keyboard>/w", "<Keyboard>/s", "<Keyboard>/a", "<Keyboard>/d" };
        PauseKey = "<Keyboard>/escape";
        UnitKey = new string[3] { "<Keyboard>/1", "<Keyboard>/2", "<Keyboard>/3" };
        UnitKeySetting = new bool[3][][]
        {
            new[] { new bool[2], new bool[2], new bool[2] },
            new[] { new bool[2], new bool[2], new bool[2] },
            new[] { new bool[2], new bool[2], new bool[2] }
        };
        Sounds = new float[3] { 1, 1, 1 };
        IsShowDamage = true; AttackAlpha = 1;

        if (IsResetGameVar)
        {
            Objects = new int[3] { 0, 0, 0 };
            Stat = new int[10] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            Enem = new int[6] { 0, 0, 0, 0, 0, 0 };
            LastBatch = new List<int>() { 0 };
        }
    }
}
