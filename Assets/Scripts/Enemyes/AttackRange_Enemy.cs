using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange_Enemy : MonoBehaviour
{
    Enemy enemy;

    int InCount = 0;


    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (InCount++ == 0) { enemy.BeginAttack = true; enemy.Target = collision.transform; }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InCount--;
            if(InCount == 0) enemy.BeginAttack = false;
        }
    }

    private void OnEnable()
    {
        InCount = 0;
    }
}
