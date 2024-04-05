using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using Cinemachine;
using System.Collections;
using UnityEngine.UI;

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

    // On Loby
    [HideInInspector] public DataManager Data;

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
    attribute InitPlayerStatus;

    private void Awake()
    {
        InitPlayerStatus = PlayerStatus;
        if (instance == null) { instance = this;  this.name = "Babo"; DontDestroyOnLoad(gameObject); }
        else if (instance != this) Destroy(gameObject);
        //else Destroy(gameObject);
        
        //LoadAssets();
    }


    [HideInInspector] public Player[] Players;
    public List<int> CurPlayerID;
    public int PlayerInd = 0;


    /*[SerializeField]
    public List<ItemSub> Items;

    [SerializeField]
    public List<ItemSub> WeaponSub;*/

    public List<Sprite> LoadingSprites;
    [SerializeField] Image LoadedImage;
    public void StartGame()
    {
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
        GameObject[] Prefs = new GameObject[LL];
        await AddressablesLoader.InitAssets(BatchName, "Operator_Scriptable", Players, typeof(Player));
        for (int i = 0; i < LL; i++) Players[i].Id = i;
        Players[PlayerInd].IsPlayer = true;
        player = Players[PlayerInd];
        await AddressablesLoader.InitAssets(BatchName, "Operator_Pref", Prefs, DM.transform.parent);
        PlayerObj = Prefs[PlayerInd]; PlayerObj.SetActive(false);

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