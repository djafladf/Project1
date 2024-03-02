using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amiya : PlayerSetting
{
    [SerializeField] Sprite Bullet1;
    [SerializeField] Sprite Bullet2;
    [SerializeField] GameObject Weapon2;
    public int WeaponLevel = 1;
    RangeAttack MyAttack;
    protected override void Awake()
    {
        base.Awake();
        MyAttack = GetComponent<RangeAttack>();
        player.WeaponSprite = player.Weapon.GetComponent<SpriteRenderer>();
        player.CurHP = player.MaxHP = 150;
        player.CurSP = player.MaxSP = 30;
    }

    private void Start()
    {
        StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        
        while(true){
            for (int i = 0; i <= WeaponLevel/3 + WeaponLevel / 7 * 3; i++)
            {
                Vector3 RandomSub = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f));
                MyAttack.Fire
                    (1, (int)GameManager.instance.PlayerStatus.attack * 2,
                    WeaponLevel/3,
                    transform.position + RandomSub,10,
                    WeaponLevel < 7 ? Bullet1 : Bullet2, false,false);
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    Vector3 RotateSub = new Vector3(30f, 60f, 90f);

    protected override void WeaponAnim()
    {
        Weapon2.transform.Rotate(RotateSub * Time.fixedDeltaTime);
    }
}
        
