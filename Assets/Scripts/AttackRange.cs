using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    Enemy enemy;
    PlayerSetting player;
    bool IsPlayer;


    private void Awake()
    {
        if (CompareTag("Enemy_Array"))
        {
            IsPlayer = false;
            enemy = GetComponentInParent<Enemy>();
        }
        else if(CompareTag("TeamArray"))
        {
            IsPlayer = true;
            player = GetComponentInParent<PlayerSetting>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsPlayer && collision.CompareTag("Player"))
        {
            enemy.BeginAttack = true; enemy.Target = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsPlayer && collision.CompareTag("Player")) enemy.BeginAttack = false;
    }
}
