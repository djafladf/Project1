using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] Sprite[] EXPs;
    [SerializeField] Sprite Money;
    [SerializeField] Sprite Stone;
    [SerializeField] Sprite RefinedStone;
    [SerializeField] Sprite BlackHole;

    [SerializeField] GameObject ItemPref;

    [SerializeField] int MaxItem = 250;
    GameObject[] Items;
    SpriteRenderer[] ItemsSprite;
    Item[] ItemsScript;
    List<int> CreatedTiming;

    private void Awake()
    {
        GameManager.instance.IM = this;
        GameManager.instance.StartLoading();
    }

    public void Init()
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
        GameManager.instance.StartLoading();
    }


    public void MakeItem(Vector3 pos, bool MustMake = false)
    {
        int Ran;
        if (CreatedTiming.Count == MaxItem)
        {
            // If Over MaxItem Num. FIFO
            int First = CreatedTiming[0]; CreatedTiming.RemoveAt(0);
            if (MustMake) Ran = Random.Range(0, 149);
            else Ran = Random.Range(0, 200);
            if (Ran < 169)
            {
                CreatedTiming.Add(First);
                Items[First].SetActive(true);
                Items[First].transform.position = pos;
            }
            if (Ran < 140)
            {
                int CurExp = Mathf.FloorToInt(GameManager.instance.UM.CurMinute * 0.125f); if (CurExp > 3) CurExp = 3;
                ItemsSprite[First].sprite = EXPs[CurExp];
                ItemsScript[First].Init(0, (int)Mathf.Pow(2 ,CurExp));
            }
            else if (Ran < 160)
            {
                ItemsSprite[First].sprite = Money;
                ItemsScript[First].Init(1, 5);
            }
            else if (Ran < 164)
            {
                ItemsSprite[First].sprite = Stone;
                ItemsScript[First].Init(2, 1);
            }
            else if (Ran < 168)
            {
                ItemsSprite[First].sprite = RefinedStone;
                ItemsScript[First].Init(3, 5);
            }
            else if (Ran < 169)
            {
                ItemsSprite[First].sprite = BlackHole;
                ItemsScript[First].Init(4, 0);
            }
        }
        else
            for(int i = 0; i < MaxItem; i++)
            {
                if (!Items[i].activeSelf)
                {
                    if (MustMake) Ran = Random.Range(0, 149);
                    else Ran = Random.Range(0, 200);
                    if (Ran < 169)
                    {
                        CreatedTiming.Add(i);
                        Items[i].SetActive(true);
                        Items[i].transform.position = pos;
                    }

                    if (Ran < 140)
                    {
                        int CurExp = Mathf.FloorToInt(GameManager.instance.UM.CurMinute * 0.125f); if (CurExp > 3) CurExp = 3;
                        ItemsSprite[i].sprite = EXPs[CurExp];
                        ItemsScript[i].Init(0, (int)Mathf.Pow(2, CurExp));
                    }
                    else if (Ran < 160)
                    {
                        ItemsSprite[i].sprite = Money;
                        ItemsScript[i].Init(1, 5);
                    }
                    else if (Ran < 164)
                    {
                        ItemsSprite[i].sprite = Stone;
                        ItemsScript[i].Init(2, 1);
                    }
                    else if (Ran < 168)
                    {
                        ItemsSprite[i].sprite = RefinedStone;
                        ItemsScript[i].Init(3, 5);
                    }
                    else if (Ran < 169)
                    {
                        ItemsSprite[i].sprite = BlackHole;
                        ItemsScript[i].Init(4, 0);
                    }

                    break;
                }
            }
    }

    public void RemoveItem(int ind)
    {
        if(CreatedTiming.Contains(ind)) CreatedTiming.RemoveAt(CreatedTiming.IndexOf(ind));
    }

    public int MagTime = 0;
    Coroutine Mag = null;

    public void MagStart()
    {
        MagTime = 5;
        if (Mag == null) Mag = StartCoroutine(MagCount());
    }
    
    IEnumerator MagCount()
    {
        while(MagTime > 0)
        {
            MagTime--;
            yield return GameManager.OneSec;
        }
        Mag = null;
    }
}
