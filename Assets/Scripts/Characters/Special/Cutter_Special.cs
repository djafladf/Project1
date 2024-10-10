using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Cutter_Special : MonoBehaviour
{
    [SerializeField] Player Cutter;

    BulletInfo BI;
    private void Start()
    {
        BI = new BulletInfo(0, false, 0, ignoreDefense: 0.2f, dealFrom: Cutter.name[0] - '0');
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            BI.Damage = Mathf.FloorToInt((1 + GameManager.instance.PlayerStatus.attack + Cutter.AttackRatio + Cutter.ReinforceAmount[0]) * 15);
            GameManager.instance.BM.MakeMeele(BI, 0.3f, collision.transform.position, Vector3.zero, 0, false);
        }
    }
}
