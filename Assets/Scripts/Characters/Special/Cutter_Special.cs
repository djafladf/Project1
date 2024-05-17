using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Cutter_Special : MonoBehaviour
{
    [SerializeField] Player Cutter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
            GameManager.instance.BM.MakeBullet(new BulletInfo(Mathf.FloorToInt((1 + GameManager.instance.PlayerStatus.attack + Cutter.AttackRatio) * 15), false, 0), 0, collision.transform.position, Vector3.zero, 0, false);
    }
}
