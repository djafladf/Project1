using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aurora_Special : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Sprite Sp;

    WaitForSeconds ZeroDotFive = new WaitForSeconds(0.5f);
    BulletInfo BI;
    IEnumerator EAE()
    {
        while (true)
        {
            yield return ZeroDotFive;
            BI.Damage = (int)(player.InitDefense * (1 + player.DefenseRatio + GameManager.instance.PlayerStatus.defense + player.ReinforceAmount[1]) * 0.5f);
            GameManager.instance.BM.MakeMeele(BI,0.3f, transform.position, Vector3.zero, 0, false, Sp);
            yield return ZeroDotFive;
        }
    }

    private void Start()
    {
        BI = new BulletInfo(0, false, 0, debuffs: new DeBuff(ice: 2.5f), dealFrom: player.Id);
    }
    private void OnEnable()
    {
        transform.localPosition = Vector3.zero;
        StartCoroutine(EAE());
    }

}
