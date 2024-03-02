using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System.IO;
using System;
using TMPro;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.Runtime.CompilerServices;

public class AttackType
{
    public bool IsMeele;
    public float AttackSpeed;
    public float FirstDelay;
    public float LastDelay;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    public BulletManager BM;
    public ItemManager IM;
    public EnemySpawner ES;
    public DamageManager DM;
    public Vector3[] SpawnArea;
    public int SpawnAreaSize;

    [NonSerialized] public string PlayerName = "Amiya";
    [NonSerialized] public int KillCount = 0;
    [NonSerialized] public int CurLevel = 1;
    [NonSerialized] public float CurCost = 0;
    [NonSerialized] public float BatchAreaSize = 500;

    public static WaitForSeconds OneSec = new WaitForSeconds(1);


    [SerializeField] GameObject LevelUP;

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
    [SerializeField] string[] BatchName;

    int[] GoodsCount;

    float CurTime = 0;
    

    string DirPath;

    public attribute PlayerStatus;


    private void Awake()
    {
        //Read External Data
        DirPath = Directory.GetCurrentDirectory();
        ItemInfo = JsonConvert.DeserializeObject<ItemInfos>(File.ReadAllText(DirPath + "\\Assets\\JSON\\ItemInfos.Json"));
        ItemInfo.Selected = new List<Item>();


        // Read Asset
        MakeBatch();
        
        // Set Spawn Area
        SpawnArea = new Vector3[88];
        instance = this;
        GoodsCount = new int[3];
        int i = 0;
        for (int x = -10; x <= 10; x++) SpawnArea[i++] = new Vector2(x,6);
        for (int x = 11; x >= -11; x--) { SpawnArea[i++] = new Vector2(-11, x); SpawnArea[i++] = new Vector2(11, x); }
        for (int x = -10; x <= 10; x++) SpawnArea[i++] = new Vector2(x, -6);
        SpawnAreaSize = i;

        // Set Player
        Name.text = PlayerName;
        Face.sprite = Resources.Load<Sprite>($"{PlayerName}\\Head");

        // Test
        ExpSub = 0.05f;
    }

    [SerializeField] Transform ToolField;
    private async void MakeBatch()
    {
        List<GameObject> Prefs = new List<GameObject>();
        List<Sprite> Heads = new List<Sprite>();
        List<Player> Players = new List<Player>();
        await AddressablesLoader.InitAssets(BatchName, "Operator_Pref", Prefs, BatchField);
        await AddressablesLoader.InitAssets(BatchName, "Operator_Head", true, Heads);
        await AddressablesLoader.InitAssets(BatchName, "Operator_Scriptable", false, Players);
        Vector3 StartPos = new Vector3(150 - Prefs.Count * 150, 0, 0); Vector3 Cnt = new Vector3(250, 0, 0);
        for(int i = 0; i< Prefs.Count; i++)
        {
            GameObject Tool = Instantiate(BatchTool,ToolField);
            var k = Tool.GetComponent<OperatorBatchTool>();
            Players[i].MyBatch = k;
            k.Init(Prefs[i], Heads[i]);
            Tool.GetComponent<RectTransform>().anchoredPosition = StartPos + Cnt * i;
            Tool.SetActive(true);
        }
    }


    private void Start()
    {
        HpChange();
    }

    private void FixedUpdate()
    {
        CurTime += Time.fixedDeltaTime;
        if(CurCost<99)CurCost += Time.fixedDeltaTime * PlayerStatus.cost;
        Cost.text = $"{(int)CurCost}";
        Timer.text = string.Format("{0:00}:{1:00}",Mathf.FloorToInt(CurTime/60f),Mathf.FloorToInt(CurTime%60f));
    }

    // About Items  -----------------------------------------------------------------------------------------

    public class ItemInfos
    {
        public List<Item> Items;
        public List<Item> Selected;
    }

    [Serializable]
    public class Item
    {
        public int id;
        public string name;
        public string description;
        public string extra;
        public attribute attributes;
    }

    [Serializable]
    public class attribute
    {
        public float attack;
        public float defense;
        public float cost;
        public float sp;
        public float pickup;
        public int selection;
        public float exp;
        
        public attribute()
        {
            attack = 1; defense = 1; cost = 1; pickup = 1; exp = 1;
        }

        public override string ToString()
        {
            return $"{attack},{defense},{cost},{sp},{pickup},{selection},{exp}";
        }
    }

    ItemInfos ItemInfo;

    [Header("- À¯¹° -")]

    [SerializeField]
    LevelUpSelection[] Selections;
    [SerializeField]
    Sprite[] ItemSprites;

    void LevelUpEvent()
    {
        int[] array = new int[ItemInfo.Items.Count]; for (int i = 0; i < array.Length; i++) array[i] = i;
        array = array.OrderBy(x => Guid.NewGuid()).ToArray();
        
        int RealSelect = ItemInfo.Items.Count; if (RealSelect > PlayerStatus.selection) RealSelect = PlayerStatus.selection;

        int[] pickedElements = array.Take(RealSelect).ToArray();
        foreach (var k in Selections) k.gameObject.SetActive(false);
        for (int i = 0; i < RealSelect; i++) 
        {
            int Ind = pickedElements[i]; Item cnt = ItemInfo.Items[Ind];
            Selections[i].gameObject.SetActive(true);
            Selections[i].Init(ItemSprites[cnt.id],cnt.name,cnt.description,cnt.extra,Ind); 
        }
        
        LV.text = $"LV.{++CurLevel}";
        ExpSub = ExpSub * 0.9f;


        LevelUP.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    [SerializeField] Transform GetArea;
    public void ApplySelection(int ind)
    {
        Item cnt = ItemInfo.Items[ind];
        attribute cntatt = cnt.attributes;
        ItemInfo.Selected.Add(cnt); ItemInfo.Items.RemoveAt(ind);
        PlayerStatus.attack *= cntatt.attack; PlayerStatus.cost *= cntatt.cost; PlayerStatus.defense *= cntatt.defense; PlayerStatus.pickup *= cntatt.pickup;
        PlayerStatus.selection += cntatt.selection; PlayerStatus.exp *= cntatt.exp; PlayerStatus.sp += cntatt.sp;
        if (cntatt.pickup != 1) GetArea.localScale *= PlayerStatus.pickup;
        if (ItemInfo.Items.Count == 0) PlayerStatus.exp = 0;
    }

    // --------------------------------------------------------------------------------------------

    // About Batch  -------------------------------------------------------------------------------

    public OperatorBatchTool CurRequest;
    [SerializeField] Transform BatchField;
    public Image BatchImage;
    [SerializeField] RectTransform BatchRect;
    public bool BatchRequest(Sprite image, OperatorBatchTool req)
    {
        if (CurRequest != null) return false;
        CurRequest = req;
        BatchImage.sprite = image;
        Vector2 Cnt = image.bounds.size.normalized;
        if (Cnt.x > Cnt.y) Cnt = Cnt/Cnt.x * 0.8f;
        else Cnt /= Cnt.y * 0.8f;

        BatchRect.localScale = Cnt;
        BatchObj.gameObject.SetActive(true);
        return true;
    }

    public void EndBatch(int Cost)
    {
        CurRequest = null;
        CurCost -= Cost;
    }

    //  --------------------------------------------------------------------------------------------

    // About Status --------------------------------------------------------------------------------

    float ExpSub;
    public void ExpUp(int value)
    {
        float cnt = ExpBar.fillAmount + ExpSub * value * PlayerStatus.exp;

        while(cnt >= 1)
        {
            LevelUpEvent();
            cnt -= 1;
        }
        
        ExpBar.fillAmount = cnt;
    }

    public void HpChange()
    {
        Hp.text = $"{player.CurHP}";
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

    // ETC Func -------------------------------
    
    private async void GetExternalAsset<T>(string _label, List<T> _createdObjs, Transform _parent) where T : Object
    {
        await AddressablesLoader.InitAssets(_label, _createdObjs, _parent);
    }

    private async void GetExternalAsset<T>(string[] name, string _label, List<T> _createdObjs, Transform _parent) where T : Object
    {
        await AddressablesLoader.InitAssets(name, _label, _createdObjs, _parent);
    }

    private async void GetExternalAsset<T>(string[] name, string _label, bool IsIm, List<T> _createdObjs) where T : Object
    {
        await AddressablesLoader.InitAssets(name, _label,IsIm, _createdObjs);
    }

}