using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // 0 : 경치, 1 : 돈, 2 : 원석, 3 : 돌
    int type;
    int amount;
    public int poolInd;
    bool IsMoving;

    Rigidbody2D rigid;

    public void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(int type, int amount)
    {
        this.type = type; this.amount = amount; IsMoving = false; IsMoving = false;
    }

    public void FixedUpdate()
    {
        if (IsMoving)
        {
            Vector2 Dir = (GameManager.instance.player.Self.position - transform.position).normalized;
            rigid.MovePosition(rigid.position + Dir * 20 * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GainArea") && !IsMoving) IsMoving = true;
        else if (collision.CompareTag("Player"))
        {
            IsMoving = false;
            gameObject.SetActive(false);
            GameManager.instance.IM.RemoveItem(poolInd);
            switch (type)
            {
                case 0: GameManager.instance.ExpUp(amount); break;
                case 1: break;
                case 2: break;
                case 3: break;
            }
        }
    }
}
