using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange_Enemy : MonoBehaviour
{
    Enemy enemy;

    List<int> Targets = new List<int>();

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int ind = collision.name[0] - '0';
            if (!Targets.Contains(ind))
            {
                Targets.Add(ind);
                enemy.BeginAttack = true;
                if (enemy.Target == null || GameManager.instance.UM.IsPriorityAttack[ind]) enemy.Target = collision.transform;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Player_Hide"))
        {
            int ind = collision.name[0] - '0';
            if (Targets.Contains(ind))
            {
                Targets.Remove(ind);
                if (Targets.Count == 0) { enemy.BeginAttack = false; enemy.Target = null; }
                else enemy.Target = GameManager.instance.Prefs[ind].transform;
            }
        }
    }

    private void OnEnable()
    {
        Targets.Clear();
    }
}