using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField] List<Scrollbar> Scrolls;
    [SerializeField] List<OnOffButton> OnOffSettings;

    [SerializeField] GameObject Border;

    [SerializeField] List<OnOffButton> Toggles;     // Contents

    [SerializeField] List<GameObject> Contents;
    [SerializeField] List<TMP_Text> KeyBinds;
    [SerializeField] List<OnOffButton> KeyBindsImages;

    [SerializeField] int DetailStartInd;
    List<List<List<OnOffButton>>> CustomDetails = new List<List<List<OnOffButton>>>();


    int CurOpen = 0;

    private void Awake()
    {
        if (GameManager.instance.SettingM == null) GameManager.instance.SettingM = this;
        else Destroy(gameObject);
        foreach (var j in KeyBindsImages) KeyBinds.Add(j.transform.GetChild(0).GetComponent<TMP_Text>());

        Transform cnt = Contents[2].transform.GetChild(0).GetChild(0);

        for (int x = 0; x < 3; x++)
        {
            CustomDetails.Add(new List<List<OnOffButton>>());
            for (int i = DetailStartInd; i < DetailStartInd + 3; i++)
            {
                Transform tmp = cnt.GetChild(i);
                CustomDetails[x].Add(new List<OnOffButton>{ tmp.GetChild(1).GetComponent<OnOffButton>(), tmp.GetChild(2).GetComponent<OnOffButton>() });
            }
            DetailStartInd += 4;
        }
    }
    private void Start()
    {
        Toggles[0].ExternalOn(); Toggles[0].DisAbleEvent(); Contents[1].SetActive(false); Contents[2].SetActive(false);
        gameObject.SetActive(false);
    }

    public void Init()
    {
        // SoundSetting
        for(int i = 0; i < Scrolls.Count; i++)
        {
            if (i < 3) Scrolls[i].value = GameManager.instance.gameStatus.Sounds[i];
            else if (i == 3) Scrolls[i].value = GameManager.instance.gameStatus.AttackAlpha;
        }
        // GameSetting
        if (GameManager.instance.gameStatus.IsShowDamage) OnOffSettings[0].ExternalOff(); else OnOffSettings[0].ExternalOn();
        for(int i = 0; i < 3; i++)for(int x = 0; x <3; x++) for(int y = 0; y <2; y++)
                {
                    if (GameManager.instance.gameStatus.UnitKeySetting[i][x][y]) CustomDetails[i][x][y].ExternalOn();
                    else CustomDetails[i][x][y].ExternalOff();
                }
#if UNITY_STANDALONE
        CurBindKeyCodes.Clear();
        // KeySetting
        for (int i = 0; i < 8; i++)
        {
            if (i < 4) KeyBinds[i].text = GameManager.instance.gameStatus.MoveKey[i].Split('/')[1];
            else if (i == 4) KeyBinds[i].text = GameManager.instance.gameStatus.PauseKey.Split('/')[1];
            else KeyBinds[i].text = GameManager.instance.gameStatus.UnitKey[i - 5].Split('/')[1];
            CurBindKeyCodes.Add(KeyBinds[i].text);
        }
#endif
#if UNITY_ANDROID || UNITY_IOS
        Destroy(Toggles[2].gameObject); Destroy(Contents[2].gameObject);
#endif
        
        //GameManager.instance.ApplyKeyOption();
    }

    public void ResetSetting()
    {
        GameManager.instance.gameStatus.ResetVars();
        Init();
    }

    public void SaveSetting()
    {
        // SoundSetting
        for (int i = 0; i < Scrolls.Count; i++)
        {
            if (i < 3)  GameManager.instance.gameStatus.Sounds[i] = Scrolls[i].value;
            else if (i == 3) GameManager.instance.gameStatus.AttackAlpha = Scrolls[i].value;
        }
        // GameSetting

        GameManager.instance.gameStatus.IsShowDamage = !OnOffSettings[0].Onuse;
        for (int i = 0; i < 3; i++) for (int x = 0; x < 3; x++) for (int y = 0; y < 2; y++)
                {
                    GameManager.instance.gameStatus.UnitKeySetting[i][x][y] = CustomDetails[i][x][y].Onuse;
                }

#if UNITY_STANDALONE
        // KeySetting
        for (int i = 0; i < 8; i++)
        {
            if (i < 4) GameManager.instance.gameStatus.MoveKey[i] = "<Keyboard>/" + CurBindKeyCodes[i];
            else if (i == 4) GameManager.instance.gameStatus.PauseKey = "<Keyboard>/" + CurBindKeyCodes[i];
            else GameManager.instance.gameStatus.UnitKey[i - 5] = "<Keyboard>/" + CurBindKeyCodes[i];
        }
#endif
    }



    public void Toggle(int ind)
    {
        Toggles[CurOpen].ExternalOff(); Toggles[CurOpen].AbleEvent(); Contents[CurOpen].SetActive(false);
        CurOpen = ind;
        Toggles[CurOpen].ExternalOn(); Contents[CurOpen].SetActive(true);
    }


    public List<string> CurBindKeyCodes = new List<string>();
    int CurKeyChange; bool OnGetKey = false;

    public void ChangeKeyBind(int ind)
    {
        CurKeyChange = ind; OnGetKey = true; Border.SetActive(true);
    }

    bool IsFirst = true;
    private void OnEnable()
    {
        if (IsFirst) return;
        Init();
    }

    private void OnDisable()
    {
        if (IsFirst) { IsFirst = false; return; }
        SaveSetting();
    }

    private void Update()
    {
        if (OnGetKey)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame) foreach (var key in Keyboard.current.allKeys)
                    if (key.isPressed)
                    {
                        var GetKey = key.displayName.ToLower(); 
                        if (GetKey == "esc") GetKey = "escape";
                        if (GetKey.StartsWith("num")) GetKey = $"numpad{GetKey[4..]}";
                        if (!CurBindKeyCodes.Contains(GetKey))
                        {
                            KeyBinds[CurKeyChange].text = GetKey;
                            CurBindKeyCodes[CurKeyChange] = GetKey;
                        }
                        KeyBindsImages[CurKeyChange].ExternalOff();
                        Border.SetActive(false);
                        OnGetKey = false;
                    }
        }
    }
}


