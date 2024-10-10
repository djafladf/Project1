using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;


// InGameManager

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

    [SerializeField] BatchCnt BatchObj;

    [SerializeField] GameObject BatchTool;

    [SerializeField] Transform ToolField;

    [SerializeField] GameObject GetAreaPref;
    [SerializeField] GameObject PauseObj;

    [SerializeField] GameObject MobileSet;
    [SerializeField] RectTransform Stick;

    [SerializeField] Transform DamageField;

    public Transform BossShaft;
    Transform GetArea;
    int[] GoodsCount = new int[3];

    private void Awake()
    {
        GameManager.instance.UM = this;
        CurCost = GameManager.instance.gameStatus.Stat[6] * 5;
        GameManager.instance.StartLoading();

#if UNITY_STANDALONE
        MobileSet.SetActive(false);
#endif
    }

    // PC 조작
#if UNITY_STANDALONE

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.FloatM.gameObject.SetActive(false);
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
#endif
    // Android 조작
#if UNITY_ANDROID || UNITY_IOS
    private void Update()
    {
        if (Time.timeScale != 1) return;
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed && GitAble)
        {
            Vector3 TouchPos = Touchscreen.current.primaryTouch.position.ReadValue();
            if (Vector3.Magnitude(TouchPos - Stick.position) < Stick.rect.width) return;
            TouchPos = Camera.main.ScreenToWorldPoint(TouchPos); TouchPos.z = 1;
            GameManager.instance.Git.transform.position = TouchPos;
            GameManager.instance.Git.SetActive(true);
            foreach (var k in GameManager.instance.Players) k.IsFollow = true;
            GitAble = false;
        }

        if (Touchscreen.current != null && !Touchscreen.current.primaryTouch.press.isPressed && !GitAble) GitAble = true;
    }
#endif

    public void RemoveGit(BaseEventData data)
    {
        PointerEventData Data = data as PointerEventData;
        if (Data.button == PointerEventData.InputButton.Left)
        {
            foreach (var k in GameManager.instance.Players) k.IsFollow = false;
            GameManager.instance.Git.SetActive(false);
        }
    }

    public void StickStatChange(bool IsActive)
    {
        if (IsActive) Stick.localScale = Vector3.one;
        else Stick.localScale = Vector3.zero;
    }

    public void Pause()
    {
        if (PauseObj.activeSelf)
        {
            GameManager.instance.SetTime(0, true);
            PauseObj.SetActive(false);
            StickStatChange(true);
        }
        else
        {
            StickStatChange(false);
            PauseObj.SetActive(true);
            GameManager.instance.SetTime(0, false);
        }
    }

    public int CurMinute = 0;

    [HideInInspector] public Transform BossTransform;
    private void FixedUpdate()
    {
        if (!GameManager.instance.IsBoss)
        {
            CurTime += Time.fixedDeltaTime;
            if (CurTime >= 60) { CurMinute++; CurTime = 0; }
            Timer.text = string.Format("{0:00}:{1:00}", CurMinute, Mathf.FloorToInt(CurTime));
        }

        if (BossShaft.gameObject.activeSelf)
        {
            Vector3 Dir = (BossTransform.position - GameManager.instance.player.Self.transform.position).normalized;
            BossShaft.rotation = Quaternion.FromToRotation(Vector3.up, Dir);
            BossShaft.position = new Vector3(Screen.width / 2, Screen.height / 2) + new Vector3(Dir.x * Screen.width * 0.5f, Dir.y * Screen.height * 0.5f);

        }
        if (CurCost < 99) CurCost += Time.fixedDeltaTime * GameManager.instance.PlayerStatus.cost * 0.75f;
        Cost.text = $"{(int)CurCost}";
    }

    float ExpSub = 0.08f;
    float CurExpVar = 0;
    public void ExpUp(int value,bool Test = false)
    {
        CurExpVar += ExpSub * value * GameManager.instance.PlayerStatus.exp;
        if (CurExpVar >= 1 || Test) {
            CurExpVar--;
            LV.text = $"LV.{++CurLevel}";
            ExpSub *= 0.92f;
            LevelUpEvent();
        }
        ExpBar.fillAmount =  Mathf.Min(CurExpVar,1);
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
        ReRollCountText.text = $"남은 횟수 : <color=red>{ReRollCount}</color>";
        LevelUpEvent();
    }

    int[] RarityPickVar = { 90, 99 };
    public void LevelUpEvent()
    {
        StickStatChange(false);
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
                    if (RarityPick < RarityPickVar[0] && pickedElements_Normal.Count != 0)
                    {
                        Ind = pickedElements_Normal[0]; pickedElements_Normal.RemoveAt(0);
                        cnt = NormalItem[Ind % NormalItem.Count];
                    }
                    else if (RarityPick < RarityPickVar[1] && pickedElements_Rare.Count != 0)
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

    [SerializeField] List<Sprite> RelicSubSprites;
    bool[] Dragons = { false, false, false, false, false };
    [HideInInspector] public int DragonCount = 0;
    public void ApplySelection(int ind,bool IsWeapon,int rarity)
    {
        StickStatChange(true);
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
                        case 1: GameManager.instance.PlayerStatus.attack += 0.03f * DragonCount; break;
                        case 2: GameManager.instance.PlayerStatus.defense += 0.03f * DragonCount; break;
                        case 3: GameManager.instance.PlayerStatus.exp += 0.05f * DragonCount; break;
                        case 4: GameManager.instance.PlayerStatus.attackspeed += 0.03f * DragonCount; break;
                        case 5: GameManager.instance.PlayerStatus.hp += 0.05f * DragonCount; break;
                    }
                }
                DragonCount++;
                if (Dragons[0])
                {
                    GameManager.instance.PlayerStatus.attack += 0.03f;
                    AttackStat.text = $"{Mathf.FloorToInt(GameManager.instance.PlayerStatus.attack * 100)}%";
                }

                if (Dragons[1])
                {
                    GameManager.instance.PlayerStatus.defense += 0.03f;
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
                        sub.HPRatio += 0.3f; sub.AttackRatio += 0.3f; sub.ChangeOccur = true; cnt.description[0] += $"<size=75%>({sub.name})</size>";
                        break;
                    case 4:
                        RarityPickVar[0] = 86; RarityPickVar[1] = 98;
                        break;
                }
            }

            if(cntatt.dragons != 0) Instantiate(RelicObj, RelicList).GetComponent<RelicSub>().Init(RelicSubSprites[cnt.rarity], cnt.sprite, cnt.name, cnt.description[0], cnt.extra, 1, ((cntatt.dragons == 3 || cntatt.dragons == 5) ? 5 : 3));
            else Instantiate(RelicObj, RelicList).GetComponent<RelicSub>().Init(RelicSubSprites[cnt.rarity],cnt.sprite, cnt.name, cnt.description[0], cnt.extra);

        }
        else
        {
            ItemSub cnt = WeaponSelection[ind];
            WeaponLevelUps[cnt.operatorid]();
            cnt.lv++; if (cnt.lv == 7) WeaponSelection.RemoveAt(ind);
        }

        if (CurExpVar>=1) ExpUp(0);
        else
        {
            LevelUP.SetActive(false);
            GameManager.instance.SetTime(0, true);
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

    [HideInInspector] public List<int> BatchOrder;
    [HideInInspector] public List<bool> IsPriorityAttack = new List<bool>();
    [SerializeField] RectTransform BatchRect;
    GameObject BatchArea;

    public GameObject ReturnOrderPriority()
    {
        if (BatchOrder.Count == 0) return GameManager.instance.player.Self.gameObject;
        int ind = BatchOrder[BatchOrder.Count-1];
        foreach (var k in BatchOrder) if (IsPriorityAttack[k]) ind = k;
        return GameManager.instance.Prefs[ind];
    }

    public bool BatchRequest(Sprite image, OperatorBatchTool req,int CharInd)
    {
        if (CurRequest != null) return false;

        if (IsPriorityAttack[CharInd]) BatchOrder.Insert(0, CharInd);
        else BatchOrder.Add(CharInd);

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
        GameManager.instance.ES.StopCor();
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
    List<TMP_Text> DamageTexts = new List<TMP_Text>();
    List<TMP_Text> HealTexts = new List<TMP_Text>();
    List<TMP_Text> DefenseTexts = new List<TMP_Text>();
    List<List<int>> DamageInt = new List<List<int>>();

    public void DamageUp(int type,int num, int Count)
    {
        DamageInt[type][num] += Count;
    }

    public void SetDamage()
    {
        for(int i = 0; i < DamageTexts.Count; i++)
        {
            DamageTexts[i].text = DamageInt[0][i] > 1000 ? $"{(int)(DamageInt[0][i]*0.001)}k" : $"{DamageInt[0][i]}";
            HealTexts[i].text = DamageInt[1][i] > 1000 ? $"{(int)(DamageInt[1][i] * 0.001)}k" : $"{DamageInt[1][i]}";
            DefenseTexts[i].text = DamageInt[2][i] > 1000 ? $"{(int)(DamageInt[2][i] * 0.001)}k" : $"{DamageInt[2][i]}";
        }
    }

    public void Init(int Count, List<ItemSub> Weapons, Player[] Players, GameObject[] Prefs, OperatorInfos[] Opers, int PlayerInd)
    {
        transform.GetComponent<CanvasScaler>().matchWidthOrHeight = GameManager.instance.RatType;

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

        Transform NameSet = DamageField.GetChild(1);
        Transform DamageText = DamageField.GetChild(2);
        Transform HealText = DamageField.GetChild(3);
        Transform DefenseText = DamageField.GetChild(4);

        DamageInt.Add(new List<int>()); DamageInt.Add(new List<int>()); DamageInt.Add(new List<int>());

        for (int i = 0; i < Count; i++)
        {
            // Add Weapon To Item
            ItemSub j = Weapons[i];
            j.operatorid = i;
            WeaponSelection.Add(j);
            IsPriorityAttack.Add(Opers[i].IsPriorityAttack);

            var tmp = NameSet.GetChild(i); tmp.gameObject.SetActive(true);
            tmp.GetComponent<TMP_Text>().text = Players[i].name_k;
            DamageInt[0].Add(0); DamageInt[1].Add(0); DamageInt[2].Add(0);
            var ttmp = DamageText.GetChild(i).GetComponent<TMP_Text>(); DamageTexts.Add(ttmp); ttmp.text = "0"; ttmp.gameObject.SetActive(true);
            var tttmp = HealText.GetChild(i).GetComponent<TMP_Text>(); HealTexts.Add(tttmp); tttmp.text = "0"; tttmp.gameObject.SetActive(true);
            var ttttmp = DefenseText.GetChild(i).GetComponent<TMP_Text>(); DefenseTexts.Add(ttttmp); ttttmp.text = "0"; ttttmp.gameObject.SetActive(true);
            Prefs[i].name = $"{i}";
            // Init BatchTool;
            if (i != PlayerInd)
            {
                int ind = i;
                GameObject Tool = Instantiate(BatchTool, ToolField);
                var k = Tool.transform.GetChild(0).GetComponent<OperatorBatchTool>();
                Players[i].MyBatch = k;
                k.Init(Prefs[i], Opers[i].Head, Players[i].Cost, Players[i].ReBatchTime,i);
                Tool.GetComponent<RectTransform>().anchoredPosition = StartPos - Cnt * batchl++;
                Tool.SetActive(true);
            }
            else
            {
                BatchOrder.Add(i);
                GetArea = Instantiate(GetAreaPref, Prefs[i].transform).transform.GetChild(0);
                GetArea.localScale *= GameManager.instance.PlayerStatus.pickup;
                BatchArea = GetArea.transform.parent.GetChild(3).gameObject;
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
        ReRollCountText.text = $"남은 횟수 : <color=red>{ReRollCount}</color>";

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
