using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
            if (MustMake) Ran = Random.Range(0, 63);
            else Ran = Random.Range(0, 101);
            if (Ran < 62)
            {
                CreatedTiming.Add(First);
                Items[First].SetActive(true);
                Items[First].transform.position = pos;
            }
            if (Ran < 55)
            {
                int CurExp = Mathf.FloorToInt(GameManager.instance.UM.CurMinute * 0.125f); if (CurExp > 3) CurExp = 3;
                ItemsSprite[First].sprite = EXPs[CurExp];
                ItemsScript[First].Init(0, (int)Mathf.Pow(2 ,CurExp));
            }
            else if (Ran < 60)
            {
                ItemsSprite[First].sprite = Money;
                ItemsScript[First].Init(1, 5);
            }
            else if (Ran < 61)
            {
                ItemsSprite[First].sprite = Stone;
                ItemsScript[First].Init(2, 1);
            }
            else if (Ran < 62)
            {
                ItemsSprite[First].sprite = RefinedStone;
                ItemsScript[First].Init(3, 5);
            }
        }
        else
            for(int i = 0; i < MaxItem; i++)
            {
                if (!Items[i].activeSelf)
                {
                    if (MustMake) Ran = Random.Range(0, 63);
                    else Ran = Random.Range(0, 101);
                    if (Ran < 62)
                    {
                        CreatedTiming.Add(i);
                        Items[i].SetActive(true);
                        Items[i].transform.position = pos;
                    }
                    if (Ran < 55)
                    {
                        int CurExp = Mathf.FloorToInt(GameManager.instance.UM.CurMinute * 0.125f); if (CurExp > 3) CurExp = 3;
                        ItemsSprite[i].sprite = EXPs[CurExp];
                        ItemsScript[i].Init(0, (int)Mathf.Pow(2, CurExp));
                    }
                    else if (Ran < 60)
                    {
                        ItemsSprite[i].sprite = Money;
                        ItemsScript[i].Init(1, 5);
                    }
                    else if (Ran < 61)
                    {
                        ItemsSprite[i].sprite = Stone;
                        ItemsScript[i].Init(2, 1);
                    }
                    else if (Ran < 62)
                    {
                        ItemsSprite[i].sprite = RefinedStone;
                        ItemsScript[i].Init(3, 5);
                    }

                    break;
                }
            }
    }

    public void RemoveItem(int ind)
    {
        if(CreatedTiming.Contains(ind)) CreatedTiming.RemoveAt(CreatedTiming.IndexOf(ind));
    }

    
}
