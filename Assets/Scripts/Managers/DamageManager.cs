using UnityEngine;
using UnityEngine.UIElements;

public class DamageManager : MonoBehaviour
{
    [SerializeField] GameObject DamagePref;
    GameObject[] DamageObjects;
    DamageObject[] DamageScripts;
    public int MaxDamage = 100;

    public void Init()
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

    public void MakeHealCount(int Amount, Transform Position)
    {
        for (int i = 0; i < MaxDamage; i++)
        {
            if (!DamageObjects[i].activeSelf)
            {
                DamageObjects[i].SetActive(true);
                DamageScripts[i].Init_Heal(Amount, Position);
                break;
            }
        }
    }
}
