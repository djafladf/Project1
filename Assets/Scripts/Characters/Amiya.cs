using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amiya : PlayerSetting
{
    RangeAttack MyAttack;
    bool WeaponA = true;
    protected override void Awake()
    {
        base.Awake();
        MyAttack = GetComponent<RangeAttack>();
        player.WeaponSprite = player.Weapon.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        while(true){
            MyAttack.Fire(5);
            yield return new WaitForSeconds(0.5f);
        }
    }

    protected override void WeaponAnim()
    {
        if (WeaponA) { player.WeaponSprite.color -= new Color(0,0,0,Time.fixedDeltaTime * 0.6f); }
        else {player.WeaponSprite.color += new Color(0, 0, 0, Time.fixedDeltaTime * 0.6f); }
        if (player.WeaponSprite.color.a <= 0.2 || player.WeaponSprite.color.a >= 1) WeaponA = WeaponA == false;
    }
}
        
