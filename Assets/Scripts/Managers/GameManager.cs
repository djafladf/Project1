using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System.IO;
using System;
using TMPro;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using Cinemachine;

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

    [NonSerialized] public float CurTime = 0;
    

    string DirPath;

    public attribute PlayerStatus;


    private void Awake()
    {
        //Read External Data
        


        // Read Asset
        LoadAssets();
        
        // Set Spawn Area
        
        instance = this;
        GoodsCount = new int[3];

        

        // Set Player
        Name.text = PlayerName;
        Face.sprite = Resources.Load<Sprite>($"{PlayerName}\\Head");

        // Test
        ExpSub = 0.05f;
    }

    [SerializeField] Transform ToolField;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] GameObject GetAreaPref;
    Transform GetArea;
    Player[] Players;
    public int PlayerInd;
    private async void LoadAssets()
    {
        // Get Items
        DirPath = Directory.GetCurrentDirectory();
        ItemInfo = JsonConvert.DeserializeObject<ItemInfos>(File.ReadAllText(DirPath + "\\Assets\\JSON\\ItemInfos.Json"));
        ItemInfo.Selected = new List<Item>();

        int LL = BatchName.Length;

        // Get Operators
        ItemInfos WeaponSub = JsonConvert.DeserializeObject<ItemInfos>(File.ReadAllText(DirPath + "\\Assets\\JSON\\WeaponInfos.Json"));

        Players = new Player[LL];
        await AddressablesLoader.InitAssets(BatchName, "Operator_Scriptable", Players, typeof(Player));
        for (int i = 0; i < LL; i++) Players[i].Id = i;
        Players[PlayerInd].IsPlayer = true;
        player = Players[PlayerInd];
        HpChange();

        GameObject[] Prefs = new GameObject[LL];
        Sprite[] Heads = new Sprite[LL];
        Sprite[] Weapons = new Sprite[LL];
        await AddressablesLoader.InitAssets(BatchName, "Operator_Pref", Prefs, BatchField);
        await AddressablesLoader.InitAssets(BatchName, "Operator_Head", Heads, typeof(Sprite));
        await AddressablesLoader.InitAssets(BatchName, "Operator_Weapon", Weapons, typeof(Sprite));

        Prefs[PlayerInd].SetActive(true);
        virtualCamera.Follow = Prefs[PlayerInd].transform;

        // Init 
        List<GameObject> ETC = new List<GameObject>();
        await AddressablesLoader.InitAssets("ETC", ETC, null);
        IM = ETC[0].transform.GetChild(0).GetComponent<ItemManager>();
        BM = ETC[0].transform.GetChild(1).GetComponent<BulletManager>();
        ES = ETC[0].transform.GetChild(2).GetComponent<EnemySpawner>();
        DM = ETC[0].transform.GetChild(3).GetComponentInChildren<DamageManager>();

        Vector3 StartPos = new Vector3(150 - Prefs.Length * 150, 0, 0); Vector3 Cnt = new Vector3(250, 0, 0);
        int batchl = 0;
        for(int i = 0; i< LL; i++)
        {
            // Add Weapon To Item
            Item j = WeaponSub.Items[i];
            j.id = ItemInfo.Items.Count;
            j.IsWeapon = true;
            j.operatorid = i;
            ItemInfo.Items.Add(j);
            ItemSprites.Add(Weapons[i]);

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

        Prefs[PlayerInd].SetActive(true);
        Prefs[PlayerInd].transform.position = Vector2.zero;

    }

    private void FixedUpdate()
    {
        CurTime += Time.fixedDeltaTime;
        if(CurCost<99)CurCost += Time.fixedDeltaTime * PlayerStatus.cost;
        Cost.text = $"{(int)CurCost}";
        Timer.text = string.Format("{0:00}:{1:00}",Mathf.FloorToInt(CurTime*0.017f),Mathf.FloorToInt(CurTime%60));
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
        public int operatorid;
        public int lv;
        public string name;
        public List<string> description;
        public string extra;
        public bool IsWeapon;
        public attribute attributes;
        public Item() { lv = 1; IsWeapon = false; }
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
    }

    ItemInfos ItemInfo;

    [Header("- 유물 -")]

    [SerializeField]
    LevelUpSelection[] Selections;
    [SerializeField]
    List<Sprite> ItemSprites;

    // 오퍼레이터 무기
    [NonSerialized]
    Func<int>[] WeaponLevelUps;

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
            Selections[i].Init(ItemSprites[cnt.id], cnt.name, cnt.description[cnt.lv-1],cnt.extra,Ind); 
        }
        
        LV.text = $"LV.{++CurLevel}";
        ExpSub = ExpSub * 0.9f;


        LevelUP.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void ApplySelection(int ind)
    {
        Item cnt = ItemInfo.Items[ind];
        if (!cnt.IsWeapon)
        {
            attribute cntatt = cnt.attributes;
            ItemInfo.Selected.Add(cnt); ItemInfo.Items.RemoveAt(ind);
            PlayerStatus.attack *= cntatt.attack; PlayerStatus.cost *= cntatt.cost; PlayerStatus.defense *= cntatt.defense; PlayerStatus.pickup *= cntatt.pickup;
            PlayerStatus.selection += cntatt.selection; 
            PlayerStatus.exp *= cntatt.exp; PlayerStatus.sp += cntatt.sp;
            if (cntatt.pickup != 1) GetArea.localScale *= PlayerStatus.pickup;
            if (ItemInfo.Items.Count == 0) PlayerStatus.exp = 0;
        }
        else
        {
            int j = WeaponLevelUps[cnt.operatorid]();
            if (j == 7) ItemInfo.Items.RemoveAt(ind);
            else cnt.lv++;
        }
    }

    public void RequestOfWeapon(Func<int> func,int id)
    {
        if (WeaponLevelUps == null)
        {
            WeaponLevelUps = new Func<int>[BatchName.Length];
        }

        WeaponLevelUps[id] = func;
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

        BatchRect.localScale = image.bounds.size * 0.15f;
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
        if (ItemInfo.Items.Count == 0) return;
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
    
    /*private async void GetExternalAsset<T>(string _label, List<T> _createdObjs, Transform _parent) where T : Object
    {
        await AddressablesLoader.InitAssets(_label, _createdObjs, _parent);
    }

    private async void GetExternalAsset<T>(string[] name, string _label, List<T> _createdObjs, Transform _parent) where T : Object
    {
        await AddressablesLoader.InitAssets(name, _label, _createdObjs, _parent);
    }*/

    /*private async void GetExternalAsset<T>(string[] name, string _label, bool IsIm, List<T> _createdObjs) where T : Object
    {
        await AddressablesLoader.InitAssets(name, _label,IsIm, _createdObjs);
    }
*/
}