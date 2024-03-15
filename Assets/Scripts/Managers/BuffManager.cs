using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    [SerializeField] Sprite Chill;
    [SerializeField] Sprite Freeze;
    [SerializeField] Sprite Slow;
    [SerializeField] Sprite DefenseDown;
    [SerializeField] GameObject DebuffObject;

    GameObject[] DeBuffs;
    SpriteRenderer[] Sprites;

    public void Init()
    {
        DeBuffs = new GameObject[200];
        Sprites = new SpriteRenderer[200];
        for(int i = 0; i < 200; i++)
        {
            DeBuffs[i] = Instantiate(DebuffObject, transform);
            Sprites[i] = DeBuffs[i].GetComponent<SpriteRenderer>();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">
    /// 0 : 서리, 1 : 빙결, 2 : 감속, 3 : 방어구 파괴
    /// </param>
    /// <returns></returns>
    public GameObject RequestForDebuff(int type)
    {
        for (int i = 0; i < 200; i++)
        {
            if (!DeBuffs[i].activeSelf)
            {
                switch (type)
                {
                    case 0: Sprites[i].sprite = Chill; break;
                    case 1:
                        Sprites[i].sprite = Freeze; break;
                    case 2:
                        Sprites[i].sprite = Slow; break;
                    case 3:
                        Sprites[i].sprite = DefenseDown; break;
                }

                return DeBuffs[i];
            }
        }
        return null;
    }

}
