using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System.IO;
using System;
using TMPro;



public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    public BulletManager BM;
    public ItemManager IM;
    public Vector3[] SpawnArea;
    public int SpawnAreaSize;

    public string PlayerName;
    public int KillCount = 0;
    public int HP = 100;
    public int MaxHP = 100;
    public int CurLevel = 1;

    [SerializeField] GameObject LevelUP;

    [SerializeField] TMP_Text Name;
    [SerializeField] TMP_Text LV;
    [SerializeField] TMP_Text Kill;
    [SerializeField] TMP_Text Hp;
    [SerializeField] Image Face;
    [SerializeField] Image ExpBar;
    [SerializeField] TMP_Text Timer;

    [SerializeField] TMP_Text[] Goods;
    int[] GoodsCount;

    float CurTime = 0;

    string DirPath;


    private void Awake()
    {
        //Read External Data
        DirPath = Directory.GetCurrentDirectory();
        ItemInfo = JsonConvert.DeserializeObject<ItemInfos>(File.ReadAllText(DirPath + "\\Assets\\JSON\\ItemInfos.Json"));

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

    private void FixedUpdate()
    {
        CurTime += Time.fixedDeltaTime;
        Timer.text = string.Format("{0:00}:{1:00}",Mathf.FloorToInt(CurTime/60f),Mathf.FloorToInt(CurTime%60f));
    }


    public class ItemInfos
    {
        public Item[] Items;
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
        public int attack;
        public int defense;
        public int cost;
        public int sp;
        public int pickup;
        public int speical;
    }

    ItemInfos ItemInfo;

    [SerializeField] 
    int SelectCount = 3;
    [SerializeField]
    LevelUpSelection[] Selections;
    [SerializeField]
    Sprite[] ItemSprites;
    void LevelUpEvent()
    {
        int[] array = new int[ItemInfo.Items.Length]; for (int i = 0; i < array.Length; i++) array[i] = i;
        array = array.OrderBy(x => Guid.NewGuid()).ToArray();
        int[] pickedElements = array.Take(SelectCount).ToArray();
        for (int i = 0; i < SelectCount; i++) 
        {
            int Ind = pickedElements[i]; Item cnt = ItemInfo.Items[Ind]; 
            Selections[i].Init(ItemSprites[Ind],cnt.name,cnt.description,cnt.extra,Ind); 
        }
        
        LV.text = $"LV.{++CurLevel}";
        ExpSub = ExpSub * 0.9f;


        LevelUP.gameObject.SetActive(true);
        Time.timeScale = 0;
    }


    float ExpSub;
    public void ExpUp(int value)
    {
        float cnt = ExpBar.fillAmount + ExpSub * value;

        while(cnt >= 1)
        {
            LevelUpEvent();
            cnt -= 1;
        }
        
        ExpBar.fillAmount = cnt;
    }

    public void HpChange(int value)
    {
        if (value > 0 && HP < MaxHP) HP++;
        else if (value < 0 && HP > 0) HP--;
        Hp.text = $"{HP}";
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
}
