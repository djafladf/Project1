using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics.Platform;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ItemManager : MonoBehaviour
{
    [SerializeField] Sprite[] EXPs;
    [SerializeField] Sprite Money;
    [SerializeField] Sprite Stone;
    [SerializeField] Sprite RefinedStone;

    [SerializeField] GameObject ItemPref;

    [SerializeField] int MaxItem = 250;
    GameObject[] Items;
    SpriteRenderer[] ItemsSprite;
    Item[] ItemsScript;
    List<int> CreatedTiming;

    private void Awake()
    {
        CreatedTiming = new List<int>();
        Items = new GameObject[MaxItem];
        ItemsSprite = new SpriteRenderer[MaxItem];
        ItemsScript = new Item[MaxItem];

        for(int i =  0; i < MaxItem; i++)
        {
            Items[i] = Instantiate(ItemPref, transform);
            ItemsSprite[i] = Items[i].GetComponent<SpriteRenderer>();
            ItemsScript[i] = Items[i].GetComponent<Item>();
            ItemsScript[i].poolInd = i;
            Items[i].SetActive(false);
        }
    }

    public void MakeItem(Transform pos)
    {
        int Ran;
        if (CreatedTiming.Count == MaxItem)
        {
            // If Over MaxItem Num. FIFO
            int First = CreatedTiming[0]; CreatedTiming.RemoveAt(0);
            Ran = Random.Range(0, 101);
            if (Ran < 72)
            {
                CreatedTiming.Add(First);
                Items[First].SetActive(true);
                Items[First].transform.position = pos.position;
            }
            if (Ran < 45)
            {
                int CurExp = Mathf.FloorToInt(GameManager.instance.CurTime*0.002f);
                ItemsSprite[First].sprite = EXPs[CurExp];
                ItemsScript[First].Init(0, (int)Mathf.Pow(2 ,CurExp));
            }
            else if (Ran < 70)
            {
                ItemsSprite[First].sprite = Money;
                ItemsScript[First].Init(1, 10);
            }
            else if (Ran < 71)
            {
                ItemsSprite[First].sprite = Stone;
                ItemsScript[First].Init(2, 1);
            }
            else if (Ran < 72)
            {
                ItemsSprite[First].sprite = RefinedStone;
                ItemsScript[First].Init(3, 10);
            }
        }
        else
            for(int i = 0; i < MaxItem; i++)
            {
                if (!Items[i].activeSelf)
                {
                    Ran = Random.Range(0, 101);
                    if (Ran < 72)
                    {
                        CreatedTiming.Add(i);
                        Items[i].SetActive(true);
                        Items[i].transform.position = pos.position;
                    }
                    if (Ran < 45)
                    {
                        int CurExp = Mathf.FloorToInt(GameManager.instance.CurTime * 0.002f);
                        ItemsSprite[i].sprite = EXPs[CurExp];
                        ItemsScript[i].Init(0, (int)Mathf.Pow(2, CurExp+1));
                    }
                    else if (Ran < 70)
                    {
                        ItemsSprite[i].sprite = Money;
                        ItemsScript[i].Init(1, 10);
                    }
                    else if (Ran < 71)
                    {
                        ItemsSprite[i].sprite = Stone;
                        ItemsScript[i].Init(2, 1);
                    }
                    else if (Ran < 72)
                    {
                        ItemsSprite[i].sprite = RefinedStone;
                        ItemsScript[i].Init(3, 10);
                    }

                    break;
                }
            }
    }

    public void RemoveItem(int ind)
    {
        int l = CreatedTiming.IndexOf(ind);
        if(l != -1) CreatedTiming.RemoveAt(CreatedTiming.IndexOf(ind));
    }
}
