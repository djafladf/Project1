using System.Collections;
using System.Collections.Generic;
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
            Items[First].SetActive(true);
            Items[First].transform.position = pos.position;
            if (Ran < 45)
            {
                ItemsSprite[First].sprite = EXPs[0];
                ItemsScript[First].Init(0,1);
            }
            else if (Ran < 70)
            {
                ItemsSprite[First].sprite = Money;
                ItemsScript[First].Init(1, 10);
            }
            else if (Ran < 72)
            {
                ItemsSprite[First].sprite = Stone;
                ItemsScript[First].Init(2, 1);
            }
            else if (Ran < 74)
            {
                ItemsSprite[First].sprite = RefinedStone;
                ItemsScript[First].Init(3, 10);
            }

            CreatedTiming.Add(First);
        }
        else
            for(int i = 0; i < MaxItem; i++)
            {
                if (!Items[i].activeSelf)
                {
                    Ran = Random.Range(0, 101);
                    Items[i].SetActive(true);
                    Items[i].transform.position = pos.position;
                    if (Ran < 45)
                    {
                        ItemsSprite[i].sprite = EXPs[0];
                        ItemsScript[i].Init(0, 1);
                    }
                    else if (Ran < 70)
                    {
                        ItemsSprite[i].sprite = Money;
                        ItemsScript[i].Init(1, 10);
                    }
                    else if (Ran < 72)
                    {
                        ItemsSprite[i].sprite = Stone;
                        ItemsScript[i].Init(2, 1);
                    }
                    else if (Ran < 74)
                    {
                        ItemsSprite[i].sprite = RefinedStone;
                        ItemsScript[i].Init(3, 10);
                    }

                    CreatedTiming.Add(i);
                    return;
                }
            }
    }

    public void RemoveItem(int ind)
    {
        int l = CreatedTiming.IndexOf(ind);
        if(l != -1) CreatedTiming.RemoveAt(CreatedTiming.IndexOf(ind));
    }
}
