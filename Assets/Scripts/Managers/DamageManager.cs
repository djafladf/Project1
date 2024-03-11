using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    [SerializeField] GameObject DamagePref;
    GameObject[] DamageObjects;
    DamageObject[] DamageScripts;
    public int MaxDamage = 100;

    private void Awake()
    {
        DamageObjects = new GameObject[MaxDamage];
        DamageScripts = new DamageObject[MaxDamage];
        for (int i = 0; i < MaxDamage; i++)
        {
            DamageObjects[i] = Instantiate(DamagePref, transform);
            DamageScripts[i] = DamageObjects[i].GetComponent<DamageObject>();
        }
    }

    public void MakeDamage(int Damage, Transform position)
    {
        if (Damage <= 0) return;
        for(int i = 0; i < MaxDamage; i++)
        {
            if (!DamageObjects[i].activeSelf)
            {
                DamageObjects[i].SetActive(true);
                DamageScripts[i].Init(Damage, position);
                break;
            }
        }
    }
}
