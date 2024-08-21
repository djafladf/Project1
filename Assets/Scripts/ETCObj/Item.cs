using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // 0 : 경치, 1 : 돈, 2 : 원석, 3 : 돌, 4 : 자석
    int type;
    int amount;
    public int poolInd;
    public bool IsMoving;

    Rigidbody2D rigid;

    public void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(int type, int amount)
    {
        this.type = type; this.amount = amount; IsMoving = false; speed = 10;
    }

    float speed; 

    public void FixedUpdate()
    {
        if (IsMoving)
        {
            Vector2 Dir = (GameManager.instance.player.Self.position - transform.position).normalized;
            rigid.MovePosition(rigid.position + Dir * speed * Time.fixedDeltaTime);
            speed *= 1.05f; if (speed > 30) speed = 30;
        }
        else if (GameManager.instance.IM.MagTime != 0) IsMoving = true;
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
                case 0: GameManager.instance.UM.ExpUp(amount); break;
                case 4: GameManager.instance.IM.MagStart(); break;
                default: GameManager.instance.UM.GoodsUp(type-1,amount); break;
            }
        }
    }
}
