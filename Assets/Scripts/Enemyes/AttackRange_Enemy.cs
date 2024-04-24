using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange_Enemy : MonoBehaviour
{
    Enemy enemy;

    List<GameObject> Targets = new List<GameObject>();

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!Targets.Contains(collision.gameObject))
            {
                Targets.Add(collision.gameObject);
                enemy.BeginAttack = true;
                if (enemy.Target == null) enemy.Target = collision.transform;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Player_Hide"))
        {
            if (Targets.Contains(collision.gameObject))
            {
                Targets.Remove(collision.gameObject);
                if (Targets.Count == 0) { enemy.BeginAttack = false; enemy.Target = null; }
                else enemy.Target = Targets[0].transform;
            }
        }
    }

    private void OnEnable()
    {
        Targets.Clear();
    }
}