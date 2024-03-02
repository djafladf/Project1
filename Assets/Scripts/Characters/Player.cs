using System;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewPlayer", menuName = "ScriptableObjects/Player")]
public class Player : ScriptableObject
{
    [NonSerialized] public Transform Self;
    [NonSerialized] public Vector2 Dir;
    [SerializeField] public float speed;
    [SerializeField] public GameObject Weapon;

    [NonSerialized] public Vector3 WeaponPos;
    [NonSerialized] public Vector3 FlipWeaponPos;
    [NonSerialized] public SpriteRenderer WeaponSprite;

    [NonSerialized] public Rigidbody2D rigid;
    [NonSerialized] public SpriteRenderer sprite;
    [NonSerialized] public Animator anim;

    [NonSerialized] public int MaxHP;
    [NonSerialized] public int CurHP;
    [NonSerialized] public int MaxSP;
    [NonSerialized] public int CurSP;
    [NonSerialized] public int Defense;

    [NonSerialized] public OperatorBatchTool MyBatch;
}

