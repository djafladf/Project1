using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System.IO;
using System;
using System.Collections.Generic;
using Cinemachine;




public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    public BulletManager BM;
    public ItemManager IM;
    public EnemySpawner ES;
    public DamageManager DM;
    public UIManager UM;
    


    [NonSerialized] public string PlayerName = "Amiya";
    public static WaitForSeconds OneSec = new WaitForSeconds(1);
    [SerializeField] string[] BatchName;
    public attribute PlayerStatus;

    private void Awake()
    {
        LoadAssets();
        instance = this;
    }

    
    Player[] Players;
    public int PlayerInd;


    private async void LoadAssets()
    {
        Transform Managers = GameObject.Find("Managers").transform;
        IM = Managers.GetChild(1).GetComponent<ItemManager>();
        BM = Managers.GetChild(2).GetComponent<BulletManager>();
        ES = Managers.GetChild(3).GetComponent<EnemySpawner>();
        UM = Managers.GetChild(4).GetComponent<UIManager>();
        DM = Managers.GetChild(5).GetComponentInChildren<DamageManager>();
        

        // Get Items
        string DirPath = Directory.GetCurrentDirectory();
        ItemInfos ItemInfo = JsonConvert.DeserializeObject<ItemInfos>(File.ReadAllText(DirPath + "\\Assets\\JSON\\ItemInfos.Json"));
        ItemInfo.Selected = new List<ItemSub>();
        int LL = BatchName.Length;
        // Get Operators
        ItemInfos WeaponSub = JsonConvert.DeserializeObject<ItemInfos>(File.ReadAllText(DirPath + "\\Assets\\JSON\\WeaponInfos.Json"));
        Players = new Player[LL];
        GameObject[] Prefs = new GameObject[LL];
        Sprite[] Heads = new Sprite[LL];
        Sprite[] Weapons = new Sprite[LL];
        await AddressablesLoader.InitAssets(BatchName, "Operator_Scriptable", Players, typeof(Player));
        for (int i = 0; i < LL; i++) Players[i].Id = i;
        Players[PlayerInd].IsPlayer = true;
        player = Players[PlayerInd];


        await AddressablesLoader.InitAssets(BatchName, "Operator_Pref", Prefs, transform.parent);
        await AddressablesLoader.InitAssets(BatchName, "Operator_Head", Heads, typeof(Sprite));
        await AddressablesLoader.InitAssets(BatchName, "Operator_Weapon", Weapons, typeof(Sprite));

        IM.Init(); BM.Init(); ES.Init(); DM.Init();

        

        UM.Init(LL,ItemInfo,WeaponSub,Weapons,Players,Prefs,Heads,PlayerInd);
        
    }

    // 오퍼레이터 무기
    public void RequestOfWeapon(Func<int> func,int id)
    {
        if (UM.WeaponLevelUps == null)
        {
            UM.WeaponLevelUps = new Func<int>[BatchName.Length];
        }
        UM.WeaponLevelUps[id] = func;
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