using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerSetting : MonoBehaviour
{
    [SerializeField] protected Player player;
    [SerializeField] protected GameObject Weapon;
    protected virtual void Awake()
    {
        player.Weapon = Weapon;
        player.Self = transform;
        player.rigid = GetComponent<Rigidbody2D>();
        player.anim = GetComponent<Animator>();
        player.sprite = GetComponent<SpriteRenderer>();
        player.WeaponPos = player.Weapon.transform.position;
        player.FlipWeaponPos = new Vector3(-player.WeaponPos.x, player.WeaponPos.y);
    }

    protected virtual void FixedUpdate()
    {
        Vector2 nextVec = player.Dir * player.speed * Time.fixedDeltaTime;
        if (nextVec.Equals(Vector2.zero)) player.anim.SetBool("IsWalk", false);

        if (player.Dir.x > 0 && !player.sprite.flipX)
        {
            player.sprite.flipX = true;
            player.Weapon.transform.position = player.FlipWeaponPos + transform.position;
        }
        else if (player.Dir.x < 0 && player.sprite.flipX)
        {
            player.sprite.flipX = false;
            player.Weapon.transform.position = player.WeaponPos + transform.position;
        }
        player.rigid.MovePosition(player.rigid.position + nextVec);
        WeaponAnim();
    }

    protected virtual void OnMove(InputValue value)
    {
        player.Dir = value.Get<Vector2>();
        player.anim.SetBool("IsWalk", true);
    }
    protected virtual void WeaponAnim()
    {

    }
}
