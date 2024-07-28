using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    [SerializeField] GameObject PauseObj;
    Transform GetArea;
    int[] GoodsCount = new int[3];

    private void Awake()
    {
        GameManager.instance.UM = this;
        CurCost = GameManager.instance.gameStatus.Stat[6] * 5;
        GameManager.instance.StartLoading();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseObj.activeSelf)
            {
                GameManager.instance.SetTime(0, true);
                PauseObj.SetActive(false);
            }
            else
            {
                PauseObj.SetActive(true);
                GameManager.instance.SetTime(0, false);
                
            }
        }

        if (Time.timeScale == 0) return;
        if (Input.GetMouseButtonDown(1))
        {
            GameManager.instance.Git.SetActive(true);
            Vector3 cnt = Camera.main.ScreenToWorldPoint(Input.mousePosition); cnt.z = 1;
            GameManager.instance.Git.transform.position = cnt;
            foreach (var k in GameManager.instance.Players) k.IsFollow = true;
        }
    }

    public void RemoveGit(BaseEventData data)
    {
        PointerEventData Data = data as PointerEventData;
        if (Data.button == PointerEventData.InputButton.Left)
        {
            foreach (var k in GameManager.instance.Players) k.IsFollow = false;
            GameManager.instance.Git.SetActive(false);
        }
    }


    public int CurMinute = 0;
    private void FixedUpdate()
    {
        if (!GameManager.instance.IsBoss)
        {
            CurTime += Time.fixedDeltaTime;
            if (CurTime >= 60) { CurMinute++; CurTime = 0; }
            Timer.text = string.Format("{0:00}:{1:00}", CurMinute, Mathf.FloorToInt(CurTime));
        }
        if (CurCost < 99) CurCost += Time.fixedDeltaTime * GameManager.instance.PlayerStatus.cost;
        Cost.text = $"{(int)CurCost}";
    }

    float ExpSub = 0.125f;
    public void ExpUp(int value)
    {
        float cnt = ExpBar.fillAmount + ExpSub * value * GameManager.instance.PlayerStatus.exp;
        if (cnt < 0) cnt = 0;
        if (cnt > 1) { 
            LevelUpEvent(); cnt -= 1; LV.text = $"LV.{++CurLevel}";
            ExpSub *= 0.92f;
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
        GoodsCount[type] += (int)(value * GameManager.instance.PlayerStatus.GoodsEarn);
        Goods[type].text = $"{GoodsCount[type]}";
    }

    // Level Up

    [SerializeField]
    GameObject LevelUP;

    [SerializeField]
    LevelUpSelection[] Selections;

    //[NonSerialized]
    public Func<int>[] WeaponLevelUps;


    List<ItemSub> NonSelected;
    List<ItemSub> WeaponSelection;


    List<ItemSub> NormalItem;
    List<ItemSub> RareItem;
    List<ItemSub> LegendItem;

    List<ItemSub> Selected;

    [SerializeField] List<ItemSub> StatItem;


    public void ReRoll()
    {
        ReRollCount--;
        ReRollBT.interactable = ReRollCount != 0;
        ReRollCountText.text = $"³²Àº È½¼ö : <color=red>{ReRollCount}</color>";
        LevelUpEvent();
    }
    public void LevelUpEvent()
    {
        if (NormalItem.Count + RareItem.Count + LegendItem.Count < GameManager.instance.PlayerStatus.selection) NormalItem.AddRange(StatItem);
        foreach (var k in Selections) k.gameObject.SetActive(false);

        int[] array = new int[NormalItem.Count]; for (int i = 0; i < array.Length; i++) array[i] = i;
        array = array.OrderBy(x => Guid.NewGuid()).ToArray();
        List<int> pickedElements_Normal = array.Take(GameManager.instance.PlayerStatus.selection).ToList();

        array = new int[RareItem.Count]; for (int i = 0; i < array.Length; i++) array[i] = i;
        array = array.OrderBy(x => Guid.NewGuid()).ToArray();
        List<int> pickedElements_Rare = array.Take(GameManager.instance.PlayerStatus.selection).ToList();

        array = new int[LegendItem.Count]; for (int i = 0; i < array.Length; i++) array[i] = i;
        array = array.OrderBy(x => Guid.NewGuid()).ToArray();
        List<int> pickedElements_Legend = array.Take(GameManager.instance.PlayerStatus.selection).ToList();

        int[] _array = new int[WeaponSelection.Count]; for (int i = 0; i < _array.Length; i++) _array[i] = i;
        _array = _array.OrderBy(x => Guid.NewGuid()).ToArray();
        List<int> _pickedElements = _array.Take(WeaponSelection.Count).ToList();

        for (int i = 0; i < GameManager.instance.PlayerStatus.selection;i++)
        {
            Selections[i].gameObject.SetActive(true);
            int l = UnityEngine.Random.Range(0,3);
            if(l == 0 && _pickedElements.Count > 0)
            {
                int Ind = _pickedElements[0]; _pickedElements.RemoveAt(0);
                ItemSub cnt = WeaponSelection[Ind]; 
                Selections[i].Init(cnt.sprite, cnt.name, 0,cnt.description[cnt.lv - 1], cnt.extra, Ind, cnt.lv,true);
            }
            else
            {
                int Ind = 0;

                ItemSub cnt = null;
                while (cnt == null)
                {
                    int RarityPick = UnityEngine.Random.Range(0, 100);
                    if (RarityPick < 85 && pickedElements_Normal.Count != 0)
                    {
                        Ind = pickedElements_Normal[0]; pickedElements_Normal.RemoveAt(0);
                        cnt = NormalItem[Ind % NormalItem.Count];
                    }
                    else if (RarityPick < 95 && pickedElements_Rare.Count != 0)
                    {
                        Ind = pickedElements_Rare[0]; pickedElements_Rare.RemoveAt(0);
                        cnt = RareItem[Ind % RareItem.Count];
                    }
                    else if(pickedElements_Legend.Count != 0)
                    {
                        Ind = pickedElements_Legend[0]; pickedElements_Legend.RemoveAt(0);
                        cnt = LegendItem[Ind % LegendItem.Count];
                    }
                }
                Selections[i].Init(cnt.sprite, cnt.name, cnt.rarity,cnt.description[cnt.lv - 1], cnt.extra, Ind, cnt.lv);
            }
        }

        if (!LevelUP.gameObject.activeSelf)
        {
            LevelUP.gameObject.SetActive(true);
            GameManager.instance.SetTime(0, false);
        }
    }

    [SerializeField] TMP_Text AttackStat;
    [SerializeField] TMP_Text HealthStat;
    [SerializeField] TMP_Text DefenseStat;
    [SerializeField] TMP_Text SpeedStat;
    [SerializeField] TMP_Text HasteStat;
    [SerializeField] TMP_Text GainStat;
    [SerializeField] TMP_Text CostStat;
    [SerializeField] TMP_Text ExpStat;

    [SerializeField] Transform RelicList;
    [SerializeField] GameObject RelicObj;
    [SerializeField] Button ReRollBT;
    TMP_Text ReRollCountText;

    int ReRollCount;


    bool[] Dragons = { false, false, false, false, false };
    int DragonCount = 0;
    public void ApplySelection(int ind,bool IsWeapon,int rarity)
    {
        if (!IsWeapon)
        {
            ItemSub cnt;
            switch (rarity)
            {
                case 0: 
                    cnt = NormalItem[ind];
                    if (cnt.attributes.special != -1) { Selected.Add(cnt); NormalItem.RemoveAt(ind); }
                    break;
                case 1: 
                    cnt = RareItem[ind];
                    if (cnt.attributes.special != -1) { Selected.Add(cnt); RareItem.RemoveAt(ind); }
                    break;
                default: 
                    cnt = LegendItem[ind];
                    if (cnt.attributes.special != -1) { Selected.Add(cnt); LegendItem.RemoveAt(ind); }
                    break;
            }
            GameObject cntRelic = Instantiate(RelicObj,RelicList);
            cntRelic.transform.GetChild(0).GetComponent<Image>().sprite = cnt.sprite;

            attribute cntatt = cnt.attributes;
            attribute enem = cnt.attribute_Enem;


            if(cntatt.GoodsEarn != 0)GameManager.instance.PlayerStatus.GoodsEarn += cntatt.GoodsEarn;
            if(cntatt.heal != 0) GameManager.instance.PlayerStatus.heal += cntatt.heal;
            if(cntatt.power != 0) GameManager.instance.PlayerStatus.power += cntatt.power;
            if (enem.attack != 0) GameManager.instance.EnemyStatus.attack += enem.attack;
            if (enem.defense != 0) GameManager.instance.EnemyStatus.defense += enem.defense;
            if (enem.speed != 0) GameManager.instance.EnemyStatus.speed += enem.speed;
            if (enem.hp != 0) GameManager.instance.EnemyStatus.hp += enem.hp;

            //if (cntatt.special != -1) { Selected.Add(cnt); NonSelected.RemoveAt(ind); }
            
            if (cntatt.attack != 0)
            {
                GameManager.instance.PlayerStatus.attack += cntatt.attack;
                AttackStat.text = $"{Mathf.FloorToInt(GameManager.instance.PlayerStatus.attack * 100)}%";
            }

            if (cntatt.cost != 0)
            {
                GameManager.instance.PlayerStatus.cost += cntatt.cost;
                CostStat.text = $"{Mathf.FloorToInt((GameManager.instance.PlayerStatus.cost - 1) * 100)}%";
            }

            if (cntatt.defense != 0)
            {
                GameManager.instance.PlayerStatus.defense += cntatt.defense;
                DefenseStat.text = $"{Mathf.FloorToInt(GameManager.instance.PlayerStatus.defense * 100)}%";
            }

            if(cntatt.speed != 0)
            {
                GameManager.instance.PlayerStatus.speed += cntatt.speed;
                SpeedStat.text = $"{Mathf.FloorToInt(GameManager.instance.PlayerStatus.speed * 100)}%";
            }

            if (cntatt.pickup != 0) 
            { 
                GameManager.instance.PlayerStatus.pickup += cntatt.pickup; 
                GetArea.localScale = new Vector3(1.5f,1.5f,1) * GameManager.instance.PlayerStatus.pickup;
                GainStat.text = $"{Mathf.FloorToInt((GameManager.instance.PlayerStatus.pickup-1) * 100)}%";
            }
            if (cntatt.exp != 0)
            {
                GameManager.instance.PlayerStatus.exp += cntatt.exp;
                ExpStat.text = $"{Mathf.FloorToInt((GameManager.instance.PlayerStatus.exp-1) * 100)}%";
            }

            if (cntatt.selection != 0) GameManager.instance.PlayerStatus.selection += cntatt.selection;
            if(cntatt.attackspeed != 0)
            {
                GameManager.instance.PlayerStatus.attackspeed += cntatt.attackspeed;
                HasteStat.text = $"{Mathf.FloorToInt(GameManager.instance.PlayerStatus.attackspeed * 100)}%";
                foreach (var k in GameManager.instance.Players) k.ChangeOccur = true;
            }
            if(cntatt.hp != 0)
            {
                GameManager.instance.PlayerStatus.hp += cntatt.hp;
                foreach (var k in GameManager.instance.Players) k.ChangeOccur = true;
                HealthStat.text = $"{Mathf.FloorToInt(cntatt.hp * 100)}%";
            }

            if (cntatt.dragons != 0)
            {
                Dragons[cntatt.dragons - 1] = true;
                if(DragonCount >= 1)
                {
                    switch (cntatt.dragons)
                    {
                        case 1: GameManager.instance.PlayerStatus.attack += 0.05f * DragonCount++; break;
                        case 2: GameManager.instance.PlayerStatus.defense += 0.05f * DragonCount++; break;
                        case 3: GameManager.instance.PlayerStatus.exp += 0.05f * DragonCount++; break;
                        case 4: GameManager.instance.PlayerStatus.attackspeed += 0.03f * DragonCount++; break;
                        case 5: GameManager.instance.PlayerStatus.hp += 0.05f * DragonCount++; break;
                    }

                }
                if (Dragons[0])
                {
                    GameManager.instance.PlayerStatus.attack += 0.05f;
                    AttackStat.text = $"{Mathf.FloorToInt(GameManager.instance.PlayerStatus.attack * 100)}%";
                }

                if (Dragons[1])
                {
                    GameManager.instance.PlayerStatus.defense += 0.05f;
                    DefenseStat.text = $"{Mathf.FloorToInt(GameManager.instance.PlayerStatus.defense * 100)}%";
                }

                if (Dragons[2])
                {
                    GameManager.instance.PlayerStatus.exp += 0.05f;
                    ExpStat.text = $"{Mathf.FloorToInt(GameManager.instance.PlayerStatus.exp * 100)}%";
                }

                if (Dragons[3])
                {
                    GameManager.instance.PlayerStatus.attackspeed += 0.03f;
                    foreach (var k in GameManager.instance.Players) k.ChangeOccur = true;
                    HasteStat.text = $"{Mathf.FloorToInt(GameManager.instance.PlayerStatus.attackspeed * 100)}%";
                }
                if (Dragons[4])
                {
                    GameManager.instance.PlayerStatus.hp += 0.05f;
                    foreach (var k in GameManager.instance.Players) k.ChangeOccur = true;
                    HealthStat.text = $"{Mathf.FloorToInt(GameManager.instance.PlayerStatus.hp * 100)}%";
                }
            }
            if (cntatt.special != 0)
            {
                switch (cntatt.special)
                {
                    case 1:
                        StartCoroutine(SpecialAct(60, () => { ExpUp(-100000); }));
                        break;
                    case 2:
                        StartCoroutine(SpecialAct(60, () => { 
                            GameManager.instance.PlayerStatus.attack += UnityEngine.Random.Range(-10, 10) * 0.01f;
                            AttackStat.text = $"{Mathf.FloorToInt(GameManager.instance.PlayerStatus.attack * 100)}%";
                        }));
                        break;
                    case 3:
                        Player sub = GameManager.instance.Players[UnityEngine.Random.Range(0,GameManager.instance.Players.Length)];
                        sub.HPRatio += 0.3f; sub.AttackRatio += 0.3f; sub.ChangeOccur = true;
                        break;
                }
            }
        }
        else
        {
            ItemSub cnt = WeaponSelection[ind];
            WeaponLevelUps[cnt.operatorid]();
            cnt.lv++; if (cnt.lv == 7) WeaponSelection.RemoveAt(ind);
        }
    }

    IEnumerator SpecialAct(int Count, Action act)
    {
        while (true)
        {
            act();
            yield return new WaitForSeconds(Count);
        }
    }

    // About Batch  -------------------------------------------------------------------------------

    public OperatorBatchTool CurRequest;
    public Image BatchImage;
    [SerializeField] RectTransform BatchRect;
    GameObject BatchArea;

    public bool BatchRequest(Sprite image, OperatorBatchTool req)
    {
        if (CurRequest != null) return false;
        CurRequest = req;
        BatchImage.sprite = image;

        BatchRect.localScale = image.bounds.size * 0.15f;
        BatchObj.gameObject.SetActive(true);
        BatchArea.SetActive(true);
        return true;
    }

    public void EndBatch(int Cost)
    {
        CurRequest = null;
        CurCost -= Cost;
        BatchArea.SetActive(false);
    }


    // Boss
    public Image BossHP;
    public TMP_Text BossName;


    // Result
    [SerializeField] Image Ending;
    [SerializeField] Image EndingSprite;
    [SerializeField] Sprite FailEnding;

    public void GameClear()
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { GameManager.instance.EndGame(); });
        Ending.GetComponent<EventTrigger>().triggers.Add(entry);
        Ending.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        Ending.gameObject.SetActive(true); 
        for (int i = 0; i < 3; i++) GameManager.instance.gameStatus.Objects[i] += GoodsCount[i];
    }
    public void GameFail()
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { GameManager.instance.EndGame(); });
        ExpSub = 0;
        Ending.GetComponent<EventTrigger>().triggers.Add(entry);
        Ending.color = new Color(1, 0, 0, 0.4f);
        EndingSprite.sprite = FailEnding;
        Ending.gameObject.SetActive(true);
        for (int i = 0; i < 3; i++) GameManager.instance.gameStatus.Objects[i] += GoodsCount[i];
    }

    //ETC

    public void Init(int Count, List<ItemSub> Weapons, Player[] Players, GameObject[] Prefs, OperatorInfos[] Opers, int PlayerInd)
    {
        NormalItem = new List<ItemSub>(); RareItem = new List<ItemSub>(); LegendItem = new List<ItemSub>();

        foreach (var k in GameManager.instance.Data.Items)
        {
            if (k.Islocked) continue;
            switch (k.rarity)
            {
                case 0: NormalItem.Add(k); break;
                case 1: RareItem.Add(k); break;
                default: LegendItem.Add(k); break;
            }
        }


        Selected = new List<ItemSub>(); WeaponSelection = new List<ItemSub>();
        //NonSelected.AddRange(GameManager.instance.Data.Items);
        Vector3 StartPos = new Vector3(100, 840, 0); Vector3 Cnt = new Vector3(0, 280, 0);
        int batchl = 0;

        for (int i = 0; i < Count; i++)
        {
            // Add Weapon To Item
            ItemSub j = Weapons[i];
            j.operatorid = i;
            WeaponSelection.Add(j);
            // Init BatchTool;
            if (i != PlayerInd)
            {
                int ind = i;
                GameObject Tool = Instantiate(BatchTool, ToolField);
                var k = Tool.GetComponent<OperatorBatchTool>();
                Players[i].MyBatch = k;
                k.Init(Prefs[i], Opers[i].Head, Players[i].Cost, Players[i].ReBatchTime);
                Tool.GetComponent<RectTransform>().anchoredPosition = StartPos - Cnt * batchl++;
                Tool.SetActive(true);
            }
            else
            {
                GetArea = Instantiate(GetAreaPref, Prefs[i].transform).transform.GetChild(0);
                GetArea.localScale *= GameManager.instance.PlayerStatus.pickup;
                BatchArea = GetArea.transform.parent.GetChild(2).gameObject;
            }
        }

        HpChange();
        GameManager.instance.VC.Follow = Prefs[PlayerInd].transform;
        Prefs[PlayerInd].transform.position = Vector2.zero;

        AttackStat.text = $"{Mathf.FloorToInt(GameManager.instance.PlayerStatus.attack * 100)}%";
        CostStat.text = $"{Mathf.FloorToInt((GameManager.instance.PlayerStatus.cost - 1) * 100)}%";
        DefenseStat.text = $"{Mathf.FloorToInt(GameManager.instance.PlayerStatus.defense * 100)}%";
        SpeedStat.text = $"{Mathf.FloorToInt(GameManager.instance.PlayerStatus.speed * 100)}%";
        GainStat.text = $"{Mathf.FloorToInt((GameManager.instance.PlayerStatus.pickup - 1) * 100)}%";
        ExpStat.text = $"{Mathf.FloorToInt((GameManager.instance.PlayerStatus.exp - 1) * 100)}%";
        HasteStat.text = $"{Mathf.FloorToInt(GameManager.instance.PlayerStatus.attackspeed * 100)}%";
        HealthStat.text = $"{Mathf.FloorToInt(GameManager.instance.PlayerStatus.hp * 100)}%";
        ReRollCount = GameManager.instance.gameStatus.Stat[9] - 1;
        ReRollCountText = ReRollBT.transform.GetChild(1).GetComponent<TMP_Text>();
        ReRollBT.interactable = ReRollCount != 0;
        ReRollCountText.text = $"³²Àº È½¼ö : <color=red>{ReRollCount}</color>";

        GameManager.instance.StartLoading();
    }


    [SerializeField] TMP_Text Dialog;

    public void ShowDialog(List<string> text, Action After = null)
    {
        StartCoroutine(MakeDialog(text,After));
    }

    IEnumerator MakeDialog(List<string> text,Action After)
    {
        WaitForSeconds wfs = new WaitForSeconds(0.1f);
        foreach(var k in text)
        {
            foreach (var j in k)
            {
                if(j != ' ') Dialog.text += j;
                yield return wfs;
            }
            yield return GameManager.OneSec;
            for (int i = Dialog.text.Length - 1; i >= 0; i--)
            {
                Dialog.text = Dialog.text.Remove(i);
                yield return wfs;
            }
            yield return GameManager.OneSec;
        }
        if (After != null) After();
    }
}
