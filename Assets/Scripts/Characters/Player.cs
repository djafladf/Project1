using System;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewPlayer", menuName = "ScriptableObjects/Player")]
public class Player : ScriptableObject
{
    [NonSerialized] public bool IsPlayer = false;

    [NonSerialized] public Transform Self;
    [NonSerialized] public Vector2 Dir;


    [SerializeField] public float speed;
    [SerializeField] public int InitHP;
    [SerializeField] public int InitSP;
    [SerializeField] public int InitDefense;
    [NonSerialized] public int MaxHP;
    [NonSerialized] public int CurHP;
    [NonSerialized] public int MaxSp;
    [NonSerialized] public int CurSP;


    [NonSerialized] public GameObject Weapon;

    [NonSerialized] public Vector3 WeaponPos;
    [NonSerialized] public Vector3 FlipWeaponPos;
    [NonSerialized] public SpriteRenderer WeaponSprite;

    [NonSerialized] public Rigidbody2D rigid;
    [NonSerialized] public SpriteRenderer sprite;
    [NonSerialized] public Animator anim;

    [NonSerialized] public int WeaponLevel;
    [NonSerialized] public int Id;

    [NonSerialized] public float AttackSpeed = 1f;
    [NonSerialized] public float MaxAttackSpeed = 1f;

    [NonSerialized] public float DefenseRatio = 0f;
    [NonSerialized] public float HPRatio = 0f;
    [NonSerialized] public float AttackRatio = 0f;
    [NonSerialized] public float SpeedRatio = 0f;

    [NonSerialized] public bool IsFollow = false;
    [NonSerialized] public bool ChangeOccur = false;
    [NonSerialized] public OperatorBatchTool MyBatch;
}

