using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    Enemy enemy;

    private void Awake()
    {
        if (CompareTag("Enemy_Array")) enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CompareTag("Enemy_Array") && collision.CompareTag("Player")) enemy.BeginAttack = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (CompareTag("Enemy_Array") && collision.CompareTag("Player")) enemy.BeginAttack = false;
    }
}
