using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using Cinemachine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static WaitForSeconds OneSec = new WaitForSeconds(1);
    public static WaitForSeconds DotOneSec = new WaitForSeconds(0.1f);
    public static int StringToInt(string Var)
    {
        int outValue = 0;
        for (int i = 0; i < Var.Length; i++) outValue = outValue * 10 + (Var[i] - '0');
        return outValue;
    }

    public static List<Transform> GetNearest(float scanRange, int count, Vector3 Position, LayerMask targetLayer)
    {
        RaycastHit2D[] targets = Physics2D.CircleCastAll(Position, scanRange, Vector2.zero, 0, targetLayer);
        Dictionary<Transform, float> Set = new Dictionary<Transform, float>();
        foreach (RaycastHit2D target in targets)
        {
            float curDiff = Vector3.Distance(Position, target.transform.position);
            Set.Add(target.transform,curDiff);
            if (Set.Count > count)
            {
                var cnt = Set.OrderBy(x => x.Value).ToList();cnt.RemoveAt(count);
                Set = cnt.ToDictionary(x => x.Key, x => x.Value);
            }
        }
        return Set.Keys.ToList();
    }

    // On Loby
    [HideInInspector] public DataManager Data;
    [HideInInspector] public ShopManager Shop;

    // On Game
    [HideInInspector] public Player player;
    [HideInInspector] public BulletManager BM;
    [HideInInspector] public ItemManager IM;
    [HideInInspector] public EnemySpawner ES;
    [HideInInspector] public DamageManager DM;
    [HideInInspector] public UIManager UM;
    [HideInInspector] public BuffManager BFM;
    [HideInInspector] public GameObject Git;

    public Camera MainCam;
    public CinemachineVirtualCamera VC;

    // ID

    public string[] Player_ID;


    [NonSerialized] public string PlayerName = "Amiya";
    public attribute PlayerStatus;
    public EnemStat EnemyStatus;
    attribute InitPlayerStatus;

    public GameStatus gameStatus;


    private void OnApplicationQuit()
    {
        gameStatus.LastBatch = CurPlayerID;
        string json = JsonConvert.SerializeObject(gameStatus);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "Status.json"), json);
    }
    
    void Awake()
    {
        InitPlayerStatus = PlayerStatus;
        if (instance == null) { 
            instance = this; 
            DontDestroyOnLoad(gameObject);
            string path = Path.Combine(Application.persistentDataPath, "Status.json");
            if (File.Exists(path))
            {
                gameStatus = JsonConvert.DeserializeObject<GameStatus>(File.ReadAllText(path));
            }
            else
            {
                gameStatus = new GameStatus(); gameStatus.LastBatch.Add(0);
            }
            CurPlayerID = gameStatus.LastBatch;
        }
        else if (instance != this) Destroy(gameObject);
        //else Destroy(gameObject);
        
        //LoadAssets();
    }


    [HideInInspector] public Player[] Players;
    [HideInInspector] public GameObject[] Prefs;
    public List<int> CurPlayerID;
    public int PlayerInd = 0;

    public void AddSummonInfo(GameObject obj,Player pl,bool IsPriority = false)
    {
        obj.name = $"{Prefs.Length}";

        var cnt = Prefs.ToList(); cnt.Add(obj);
        Prefs = cnt.ToArray();
        UM.IsPriorityAttack.Add(IsPriority);

        var tmp = Players.ToList(); tmp.Add(player);
        Players = tmp.ToArray();
    }


    /*[SerializeField]
    public List<ItemSub> Items;

    [SerializeField]
    public List<ItemSub> WeaponSub;*/

    public List<Sprite> LoadingSprites;
    [SerializeField] Image LoadedImage;
    public void StartGame()
    {
        if (CurPlayerID[0] == -1) return;
        FirstLoading = 6; LastLoading = 7;
        TimeSet.Clear(); Time.timeScale = 1;
        StartCoroutine(LoadingAct("MainGame", LoadingSprites[1],false));
    }

    public void EndGame()
    {
        PlayerStatus = InitPlayerStatus;
        TimeSet.Clear(); Time.timeScale = 1;
        StartCoroutine(LoadingAct("MainLoby", LoadingSprites[0],true));
    }

    Color CntAlpha = new Color(0, 0, 0, 0.1f);
    IEnumerator LoadingAct(string SceneName,Sprite Loadings, bool AutoEnd)
    {
        LoadedImage.sprite = Loadings; LoadedImage.gameObject.SetActive(true);
        for(int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.05f);
            LoadedImage.color += CntAlpha;
        }
        SceneManager.LoadSceneAsync(SceneName);
        if (AutoEnd) StartCoroutine(LoadingEndAct());
    }

    IEnumerator LoadingEndAct()
    { 
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.05f);
            LoadedImage.color -= CntAlpha;
        }
        LoadedImage.gameObject.SetActive(false);
    }

    int FirstLoading = 6;
    int LastLoading = 7;
    public void StartLoading()
    {
        if (--FirstLoading <= 0)
        {
            LastLoading--;
            if(LastLoading == 6) LoadAsset_Game();
            else if (LastLoading == 0)
            {
                StartCoroutine(LoadingEndAct());
                PlayerObj.SetActive(true);
                ES.StartStage();
            }
        }
    }
    GameObject PlayerObj;
    
    private async void LoadAsset_Game()
    {
        // SetManager
        Git = DM.transform.parent.GetChild(0).gameObject;

        var BatchName = CurPlayerID.Select(index => Player_ID[index]).ToArray();
        int LL = BatchName.Length;

        // Get Operators
        Players = new Player[LL];
        Prefs = new GameObject[LL];
        
        await AddressablesLoader.InitAssets(BatchName, "Operator_Scriptable", Players, typeof(Player));
        for (int i = 0; i < LL; i++) { Players[i].Id = i; }
        Players[PlayerInd].IsPlayer = true;
        player = Players[PlayerInd];
        await AddressablesLoader.InitAssets(BatchName, "Operator_Pref", Prefs, DM.transform.parent);
        PlayerObj = Prefs[PlayerInd]; PlayerObj.SetActive(false);

        // Init

        PlayerStatus.attack = gameStatus.Stat[0] * 0.05f - 0.05f;
        PlayerStatus.defense = gameStatus.Stat[1] * 0.02f - 0.02f;
        PlayerStatus.hp = gameStatus.Stat[2] * 0.04f - 0.04f;
        PlayerStatus.speed = gameStatus.Stat[3] * 0.05f - 0.05f;
        PlayerStatus.pickup = gameStatus.Stat[4] * 0.1f + 0.9f;
        PlayerStatus.attackspeed = gameStatus.Stat[5] * 0.02f - 0.02f;
        PlayerStatus.cost = gameStatus.Stat[7] * 0.05f + 0.95f;
        PlayerStatus.exp = gameStatus.Stat[8] * 0.05f + 0.95f;
        PlayerStatus.power = 0;
        PlayerStatus.heal = 0;
        PlayerStatus.selection = 3;

        EnemyStatus.attack = gameStatus.Enem[0] * 0.1f;
        EnemyStatus.defense = gameStatus.Enem[1] * 0.05f;
        EnemyStatus.hp = gameStatus.Enem[2] * 0.1f;
        EnemyStatus.speed = gameStatus.Enem[3] * 0.1f;
        EnemyStatus.spawn = gameStatus.Enem[4] * 0.1f;
        EnemyStatus.boss = gameStatus.Enem[5] * 0.05f;

        PlayerStatus.GoodsEarn = gameStatus.Enem.Sum() * 0.1f + 1;


        IM.Init(); BM.Init(); ES.Init(1); DM.Init(); BFM.Init();
        UM.Init(LL, CurPlayerID.Select(index => Data.WeaponSub[index]).ToList(), Players, Prefs, CurPlayerID.Select(index => Data.Infos[index]).ToArray(), PlayerInd);
    }

    // 오퍼레이터 무기
    public void RequestOfWeapon(Func<int> func, int id)
    {
        if (UM.WeaponLevelUps == null)
        {
            UM.WeaponLevelUps = new Func<int>[CurPlayerID.Count];
        }
        UM.WeaponLevelUps[id] = func;
    }

    List<float> TimeSet = new List<float>();
    public void SetTime(float var, bool IsRemove)
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

    public bool IsBoss = false;
    public void BossStage()
    {
        ES.StopCor();
        IsBoss = true;
    }

    public void BossEnd()
    {
        ES.StartStage();
        IsBoss = false;
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