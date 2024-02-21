using System;
using UnityEngine.InputSystem;
using UnityEngine;

public class Player : MonoBehaviour
{
    [NonSerialized] public Vector2 Dir;
    [SerializeField] float speed;

    Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 nextVec = Dir * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    void OnMove(InputValue value)
    {
        Dir = value.Get<Vector2>();
    }

}
