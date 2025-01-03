using UnityEngine;
using UnityEngine.UIElements;

public class DamageManager : MonoBehaviour
{
    [SerializeField] GameObject DamagePref;
    GameObject[] DamageObjects;
    DamageObject[] DamageScripts;
    public int MaxDamage = 100;

    private void Awake()
    {
        GameManager.instance.DM = this;
        GameManager.instance.StartLoading();
    }

    public void Init()
    {
        DamageObjects = new GameObject[MaxDamage];
        DamageScripts = new DamageObject[MaxDamage];
        for (int i = 0; i < MaxDamage; i++)
        {
            DamageObjects[i] = Instantiate(DamagePref, transform);
            DamageScripts[i] = DamageObjects[i].GetComponent<DamageObject>();
        }

        GameManager.instance.StartLoading();
    }

    public void MakeDamage(int Damage, Transform position)
    {
        if (!GameManager.instance.gameStatus.IsShowDamage) return;
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
        if (!GameManager.instance.gameStatus.IsShowDamage) return;
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
