using System;
using System.Collections.Generic;

public class AttackType
{
    public bool IsMeele;
    public float AttackSpeed;
    public float FirstDelay;
    public float LastDelay;
}

public class ItemInfos
{
    public List<ItemSub> Items;
    public List<ItemSub> Selected;
}

[Serializable]
public class ItemSub
{
    public int id;
    public int operatorid;
    public int lv;
    public string name;
    public List<string> description;
    public string extra;
    public bool IsWeapon;
    public attribute attributes;
    public ItemSub() { lv = 1; IsWeapon = false; }
}

[Serializable]
public class attribute
{
    public float attack;
    public float defense;
    public float cost;
    public float sp;
    public float pickup;
    public int selection;
    public float exp;

    public attribute()
    {
        attack = 1; defense = 1; cost = 1; pickup = 1; exp = 1;
    }
}

