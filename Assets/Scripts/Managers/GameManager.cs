using Newtonsoft.Json;
using UnityEngine;
using System.Linq;
using System.IO;
using System;
using System.Collections.Generic;




public class GameManager : MonoBehaviour
{
    public static WaitForSeconds OneSec = new WaitForSeconds(1);
    public static GameManager instance;
    public static int StringToInt(string Var)
    {
        int outValue = 0;
        for (int i = 0; i < Var.Length; i++) outValue = outValue * 10 + (Var[i] - '0');
        return outValue;
    }


    [HideInInspector] public Player player;
    [HideInInspector] public BulletManager BM;
    [HideInInspector] public ItemManager IM;
    [HideInInspector] public EnemySpawner ES;
    [HideInInspector] public DamageManager DM;
    [HideInInspector] public UIManager UM;
    [HideInInspector] public BuffManager BFM;
    [HideInInspector] public GameObject Git;
    // ID

    public string[] Player_ID =
    {
        "Amiya",
        "Aurora",
        "Cutter",
        "Platinum",
        "Wafarin",
        "Rosmontis",
        "Kazemaru"
    };




    [NonSerialized] public string PlayerName = "Amiya";
    public attribute PlayerStatus;

    private void Awake()
    {
        instance = this;
        LoadAssets();
    }

    
    public Player[] Players;
    public int[] CurPlayerID;
    public int PlayerInd;


    [SerializeField]
    public List<ItemSub> Items;

    [SerializeField]
    public List<ItemSub> WeaponSub;


    private async void LoadAssets()
    {
        Transform Managers = GameObject.Find("Managers").transform;
        IM = Managers.GetChild(1).GetComponent<ItemManager>();
        BM = Managers.GetChild(2).GetComponent<BulletManager>();
        ES = Managers.GetChild(3).GetComponent<EnemySpawner>();
        UM = Managers.GetChild(4).GetComponent<UIManager>();
        DM = Managers.GetChild(5).GetComponentInChildren<DamageManager>();
        Git = Managers.GetChild(5).GetChild(0).gameObject;
        BFM = Managers.GetChild(6).GetComponentInChildren<BuffManager>();
       

        // Get Items
        string DirPath = Directory.GetCurrentDirectory();
        var BatchName = CurPlayerID.Select(index => Player_ID[index]).ToArray();
        int LL = BatchName.Length;
        // Get Operators
        Players = new Player[LL];
        GameObject[] Prefs = new GameObject[LL];
        Sprite[] Heads = new Sprite[LL];
        Sprite[] Weapons = new Sprite[LL];
        await AddressablesLoader.InitAssets(BatchName, "Operator_Scriptable", Players, typeof(Player));
        for (int i = 0; i < LL; i++) Players[i].Id = i;
        Players[PlayerInd].IsPlayer = true;
        player = Players[PlayerInd];


        await AddressablesLoader.InitAssets(BatchName, "Operator_Pref", Prefs, Managers.GetChild(5));
        await AddressablesLoader.InitAssets(BatchName, "Operator_Head", Heads, typeof(Sprite));
        //await AddressablesLoader.InitAssets(BatchName, "Operator_Weapon", Weapons, typeof(Sprite));

        IM.Init(); BM.Init(); ES.Init(1); DM.Init(); BFM.Init();

        

        UM.Init(LL, CurPlayerID.Select(index => WeaponSub[index]).ToList(),Players,Prefs,Heads,PlayerInd);
        
    }

    // 오퍼레이터 무기
    public void RequestOfWeapon(Func<int> func,int id)
    {
        if (UM.WeaponLevelUps == null)
        {
            UM.WeaponLevelUps = new Func<int>[CurPlayerID.Length];
        }
        UM.WeaponLevelUps[id] = func;
    }

    List<float> TimeSet = new List<float>();
    public void SetTime(float var,bool IsRemove)
    {
        if (IsRemove)
        {
            TimeSet.Remove(var);
            if (TimeSet.Count == 0) Time.timeScale = 1;
            else Time.timeScale = TimeSet[0];
        }
        else
        {
            if (TimeSet.Count == 0) Time.timeScale = var;
            else if (var < TimeSet[0]) Time.timeScale = var;
            TimeSet.Add(var);
            TimeSet.Sort();
        }
    }

    // --------------------------------------------------------------------------------------------

    

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