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

public class BulletInfo
{
    public int Damage;
    public bool IsEffect;
    public float KnockBack;

    public DeBuff DeBuffs;

    public Buff Buffs;

    public BulletInfo(int damage, bool isEffect, float knockBack, DeBuff debuffs = null, Buff buffs = null)
    {
        Damage = damage;
        IsEffect = isEffect;
        KnockBack = knockBack;
        DeBuffs = debuffs;
        Buffs = buffs;
    }

    public void Set(int damage, bool isEffect, float knockBack, DeBuff debuffs = null, Buff buffs = null)
    {
        Damage = damage;
        IsEffect = isEffect;
        KnockBack = knockBack;
        DeBuffs = debuffs;
        Buffs = buffs;
    }
}

public class Buff
{
    public float Last;
    public float Speed;
    public float Attack;
    public float Defense;
    public float Fragility;
    public int Heal;

    public Buff(float last = 0, float speed = 1, float attack = 1, float defense = 1,float fragility = 0, int heal = 0)
    {
        Last = last;
        Speed = speed;
        Attack = attack;
        Defense = defense;
        Fragility = fragility;
        Heal = heal;
    }
}

public class DeBuff
{
    public float Last;
    public float Speed;
    public float Attack;
    public float Defense;
    public float Ice;
    public float Fragility;

    public DeBuff(float last = 0, float speed = 1, float attack = 1, float defense = 1, float ice = 0, float fragility = 0)
    {
        Last = last;
        Speed = speed;
        Attack = attack;
        Defense = defense;
        Ice = ice;
        Fragility = fragility;
    }
}

