using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summoner_Enemy : Enemy
{
    [SerializeField] GameObject BullPref;
    [SerializeField] Transform SummonPos;
    [SerializeField] bool HasSummonPoint;
    GameObject PrefStore = null;
    protected override void AttackMethod()
    {
        if (PrefStore == null) PrefStore = Instantiate(BullPref,GameManager.instance.BM.transform);

        if(HasSummonPoint) PrefStore.transform.position = SummonPos.position;
        else PrefStore.transform.position = AttackPos;
        PrefStore.SetActive(true);
    }
}
