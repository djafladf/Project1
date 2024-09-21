using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aurora_Special : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Sprite Sp;

    WaitForSeconds ZeroDotFive = new WaitForSeconds(0.5f);
    IEnumerator EAE()
    {
        while (true)
        {
            yield return ZeroDotFive;
            GameManager.instance.BM.MakeBullet(
                new BulletInfo((int)(player.InitDefense * (1 + player.DefenseRatio + GameManager.instance.PlayerStatus.defense + player.ReinforceAmount[1]) * 0.5f), false, 0, debuffs: new DeBuff(ice: 1)),
                0, transform.position, Vector3.zero, 0, false, Sp);
            yield return ZeroDotFive;
        }
    }

    private void OnEnable()
    {
        transform.localPosition = Vector3.zero;
        StartCoroutine(EAE());
    }

}
