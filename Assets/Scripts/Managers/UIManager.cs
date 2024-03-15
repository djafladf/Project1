using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;

public class UIManager : MonoBehaviour
{
    [NonSerialized] public int KillCount = 0;
    [NonSerialized] public int CurLevel = 1;
    [NonSerialized] public float CurCost = 0;
    [NonSerialized] public float CurTime = 0;
    [NonSerialized] public float BatchAreaSize = 600;

    [SerializeField] TMP_Text Name;
    [SerializeField] TMP_Text LV;
    [SerializeField] TMP_Text Kill;
    [SerializeField] TMP_Text Hp;
    [SerializeField] Image Face;
    [SerializeField] Image ExpBar;
    [SerializeField] TMP_Text Timer;
    [SerializeField] TMP_Text Cost;

    [SerializeField] TMP_Text[] Goods;

    [SerializeField] BatchCnt BatchObj;

    [SerializeField] GameObject BatchTool;

    [SerializeField] Transform ToolField;

    [SerializeField] GameObject GetAreaPref;
    Transform GetArea;
    int[] GoodsCount = new int[3];

    private void FixedUpdate()
    {
        CurTime += Time.fixedDeltaTime;
        if (CurCost < 99) CurCost += Time.fixedDeltaTime * GameManager.instance.PlayerStatus.cost;
        Cost.text = $"{(int)CurCost}";
        Timer.text = string.Format("{0:00}:{1:00}", Mathf.FloorToInt(CurTime * 0.017f), Mathf.FloorToInt(CurTime % 60));
    }

    float ExpSub = 0.05f;
    public void ExpUp(int value)
    {
        if (ItemInfo.Items.Count == 0) return;
        float cnt = ExpBar.fillAmount + ExpSub * value * GameManager.instance.PlayerStatus.exp;

        while (cnt >= 1)
        {
            LevelUpEvent();
            cnt -= 1;
        }

        ExpBar.fillAmount = cnt;
    }

    public void HpChange()
    {
        Hp.text = $"{GameManager.instance.player.CurHP}";
    }

    int CurKill = 0;
    public void KillCountUp(int value)
    {
        CurKill += value;
        Kill.text = $"{CurKill}";
    }

    public void GoodsUp(int type, int value)
    {
        GoodsCount[type] += value;
        Goods[type].text = $"{GoodsCount[type]}";
    }

    // Level Up
    [SerializeField]
    List<Sprite> RelicSprites;

    [SerializeField]
    GameObject LevelUP;

    [SerializeField]
    LevelUpSelection[] Selections;

    [NonSerialized]
    public Func<int>[] WeaponLevelUps;

    void LevelUpEvent()
    {
        int[] array = new int[ItemInfo.Items.Count]; for (int i = 0; i < array.Length; i++) array[i] = i;
        array = array.OrderBy(x => Guid.NewGuid()).ToArray();

        int RealSelect = ItemInfo.Items.Count; if (RealSelect > GameManager.instance.PlayerStatus.selection) RealSelect = GameManager.instance.PlayerStatus.selection;

        int[] pickedElements = array.Take(RealSelect).ToArray();
        foreach (var k in Selections) k.gameObject.SetActive(false);
        for (int i = 0; i < RealSelect; i++)
        {
            int Ind = pickedElements[i]; ItemSub cnt = ItemInfo.Items[Ind];
            Selections[i].gameObject.SetActive(true);
            if(cnt.IsWeapon)Selections[i].Init(RelicSprites[cnt.id], cnt.name, cnt.description[cnt.lv - 1], cnt.extra, Ind,cnt.lv);
            else Selections[i].Init(RelicSprites[cnt.id], cnt.name, cnt.description[cnt.lv - 1], cnt.extra, Ind, -1);
        }

        LV.text = $"LV.{++CurLevel}";
        ExpSub = ExpSub * 0.9f;


        LevelUP.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void ApplySelection(int ind)
    {
        ItemSub cnt = ItemInfo.Items[ind];
        if (!cnt.IsWeapon)
        {
            attribute cntatt = cnt.attributes;
            ItemInfo.Selected.Add(cnt); ItemInfo.Items.RemoveAt(ind);
            GameManager.instance.PlayerStatus.attack *= cntatt.attack; 
            GameManager.instance.PlayerStatus.cost *= cntatt.cost; 
            GameManager.instance.PlayerStatus.defense *= cntatt.defense; 
            GameManager.instance.PlayerStatus.pickup *= cntatt.pickup;
            GameManager.instance.PlayerStatus.selection += cntatt.selection;
            GameManager.instance.PlayerStatus.exp *= cntatt.exp; GameManager.instance.PlayerStatus.sp += cntatt.sp;
            if (cntatt.pickup != 1) GetArea.localScale *= GameManager.instance.PlayerStatus.pickup;
            if (ItemInfo.Items.Count == 0) GameManager.instance.PlayerStatus.exp = 0;
        }
        else
        {
            int j = WeaponLevelUps[cnt.operatorid]();
            if (j == 7) ItemInfo.Items.RemoveAt(ind);
            else cnt.lv++;
        }
    }

    // About Batch  -------------------------------------------------------------------------------

    public OperatorBatchTool CurRequest;
    public Image BatchImage;
    [SerializeField] RectTransform BatchRect;

    [SerializeField] CinemachineVirtualCamera VC;

    ItemInfos ItemInfo;

    public void Init(int Count, ItemInfos ItemInfo, ItemInfos WeaponSub, Sprite[] Weapons, 
        Player[] Players, GameObject[] Prefs, Sprite[] Heads,int PlayerInd)
    {
        this.ItemInfo = ItemInfo;


        Vector3 StartPos = new Vector3(300 - Count * 150, 0, 0); Vector3 Cnt = new Vector3(250, 0, 0);
        int batchl = 0;
        for (int i = 0; i < Count; i++)
        {
            // Add Weapon To Item
            ItemSub j = WeaponSub.Items[i];
            j.id = ItemInfo.Items.Count;
            j.IsWeapon = true;
            j.operatorid = i;
            ItemInfo.Items.Add(j);
            RelicSprites.Add(Weapons[i]);
            // Init BatchTool;
            if (i != PlayerInd)
            {
                int ind = i;
                GameObject Tool = Instantiate(BatchTool, ToolField);
                var k = Tool.GetComponent<OperatorBatchTool>();
                Players[i].MyBatch = k;
                k.Init(Prefs[i], Heads[i]);
                Tool.GetComponent<RectTransform>().anchoredPosition = StartPos + Cnt * batchl++;
                Tool.SetActive(true);
            }
            else
            {
                GetArea = Instantiate(GetAreaPref, Prefs[i].transform).transform.GetChild(0);
            }
        }

        HpChange();

        Prefs[PlayerInd].SetActive(true);
        VC.Follow = Prefs[PlayerInd].transform;
        Prefs[PlayerInd].SetActive(true);
        Prefs[PlayerInd].transform.position = Vector2.zero;
    }


    public bool BatchRequest(Sprite image, OperatorBatchTool req)
    {
        if (CurRequest != null) return false;
        CurRequest = req;
        BatchImage.sprite = image;

        BatchRect.localScale = image.bounds.size * 0.15f;
        BatchObj.gameObject.SetActive(true);
        return true;
    }

    public void EndBatch(int Cost)
    {
        CurRequest = null;
        CurCost -= Cost;
    }
}
