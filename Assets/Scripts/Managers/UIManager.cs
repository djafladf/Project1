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


    private void FixedUpdate()
    {
        if (!GameManager.instance.IsBoss)
        {
            CurTime += Time.fixedDeltaTime;
            Timer.text = string.Format("{0:00}:{1:00}", Mathf.FloorToInt(CurTime * 0.017f), Mathf.FloorToInt(CurTime % 60));
        }
        if (CurCost < 99) CurCost += Time.fixedDeltaTime * GameManager.instance.PlayerStatus.cost;
        Cost.text = $"{(int)CurCost}";
    }

    float ExpSub = 0.05f;
    public void ExpUp(int value)
    {
        if (NonSelected.Count == 0) return;
        float cnt = ExpBar.fillAmount + ExpSub * value * GameManager.instance.PlayerStatus.exp;
        if (cnt < 0) cnt = 0;
        if (cnt > 1) { LevelUpEvent(); cnt -= 1; }
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
    GameObject LevelUP;

    [SerializeField]
    LevelUpSelection[] Selections;

    [NonSerialized]
    public Func<int>[] WeaponLevelUps;


    List<ItemSub> NonSelected;
    List<ItemSub> Selected;

    [SerializeField] List<ItemSub> StatItem;

    void LevelUpEvent()
    {
        if (NonSelected.Count < GameManager.instance.PlayerStatus.selection) NonSelected.AddRange(StatItem);
        int[] array = new int[NonSelected.Count]; for (int i = 0; i < array.Length; i++) array[i] = i;
        array = array.OrderBy(x => Guid.NewGuid()).ToArray();
        int[] pickedElements = array.Take(GameManager.instance.PlayerStatus.selection).ToArray();
        foreach (var k in Selections) k.gameObject.SetActive(false);
        for (int i = 0; i < GameManager.instance.PlayerStatus.selection; i++)
        {
            int Ind = pickedElements[i];
            ItemSub cnt = NonSelected[Ind];
            Selections[i].gameObject.SetActive(true);
            Selections[i].Init(cnt.sprite, cnt.name, cnt.description[cnt.lv - 1], cnt.extra, Ind, cnt.lv,cnt.IsWeapon);
        }

        LV.text = $"LV.{++CurLevel}";
        ExpSub = ExpSub * 0.9f;


        LevelUP.gameObject.SetActive(true);
        GameManager.instance.SetTime(0, false);
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


    bool[] Dragons = { false, false, false, false, false };
    int DragonCount = 0;
    public void ApplySelection(int ind)
    {
        ItemSub cnt = NonSelected[ind];
        if (!cnt.IsWeapon)
        {
            GameObject cntRelic = Instantiate(RelicObj,RelicList);
            cntRelic.transform.GetChild(0).GetComponent<Image>().sprite = cnt.sprite;

            attribute cntatt = cnt.attributes;
            if (cntatt.special != -1) { Selected.Add(cnt); NonSelected.RemoveAt(ind); }
            
            if (cntatt.attack != 0)
            {
                GameManager.instance.PlayerStatus.attack += cntatt.attack;
                AttackStat.text = $"{Mathf.FloorToInt(GameManager.instance.PlayerStatus.attack * 100)}%";
            }

            if (cntatt.cost != 0)
            {
                GameManager.instance.PlayerStatus.cost += cntatt.cost;
                CostStat.text = $"{Mathf.FloorToInt((cntatt.cost-1) * 100)}%";
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

            if (cntatt.power != 0) GameManager.instance.PlayerStatus.power += cntatt.power;

            if (cntatt.pickup != 0) 
            { 
                GameManager.instance.PlayerStatus.pickup += cntatt.pickup; 
                GetArea.localScale *= GameManager.instance.PlayerStatus.pickup;
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
                HealthStat.text = $"{Mathf.FloorToInt(cntatt.exp * 100)}%";
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
                }
            }
        }
        else
        {

            if (cnt.lv == 7) NonSelected.RemoveAt(ind);
            else
            {
                WeaponLevelUps[cnt.operatorid]();
                cnt.lv++;
            }
        }

        if (NonSelected.Count == 0) GameManager.instance.PlayerStatus.exp = 0;
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

    


    public void Init(int Count, List<ItemSub> Weapons, Player[] Players, GameObject[] Prefs, OperatorInfos[] Opers, int PlayerInd)
    {
        NonSelected = new List<ItemSub>(); Selected = new List<ItemSub>();
        NonSelected.AddRange(GameManager.instance.Data.Items);
        Vector3 StartPos = new Vector3(100, 840, 0); Vector3 Cnt = new Vector3(0, 280, 0);
        int batchl = 0;

        for (int i = 0; i < Count; i++)
        {
            // Add Weapon To Item
            ItemSub j = Weapons[i];
            j.IsWeapon = true;
            j.operatorid = i;
            NonSelected.Add(j);
            // Init BatchTool;
            if (i != PlayerInd)
            {
                int ind = i;
                GameObject Tool = Instantiate(BatchTool, ToolField);
                var k = Tool.GetComponent<OperatorBatchTool>();
                Players[i].MyBatch = k;
                k.Init(Prefs[i], Opers[i].Head);
                Tool.GetComponent<RectTransform>().anchoredPosition = StartPos - Cnt * batchl++;
                Tool.SetActive(true);
            }
            else
            {
                GetArea = Instantiate(GetAreaPref, Prefs[i].transform).transform.GetChild(0);
            }
        }

        HpChange();
        GameManager.instance.VC.Follow = Prefs[PlayerInd].transform;
        Prefs[PlayerInd].transform.position = Vector2.zero;
        GameManager.instance.StartLoading();
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
        Ending.gameObject.SetActive(true);
    }
    public void GameFail()
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { GameManager.instance.EndGame(); });
        Ending.GetComponent<EventTrigger>().triggers.Add(entry);
        Ending.color = new Color(1, 0, 0, 0.4f);
        EndingSprite.sprite = FailEnding;
        Ending.gameObject.SetActive(true);
    }
}
