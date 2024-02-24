using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D rigid;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector3 Dir)
    {
        rigid.velocity = Dir;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) gameObject.SetActive(false);

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Area")) gameObject.SetActive(false);
    }
}
