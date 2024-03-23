using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackType
{
    public bool IsMeele;
    public float AttackSpeed;
    public float FirstDelay;
    public float LastDelay;
}


[System.Serializable]
public class ItemSub
{
    public string name;
    public int operatorid;
    public int lv;
    public List<string> description;
    [Multiline(2)]
    public string extra;
    public bool IsWeapon;
    public Sprite sprite;
    public attribute attributes;
}

[System.Serializable]
public class attribute
{
    public float attack;
    public float attackspeed;
    public float defense;
    public float speed;
    public float cost;
    public float pickup;
    public int selection;
    public float exp;
    public float heal;    
    public int dragons;
    public int special;
    public float hp;
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

