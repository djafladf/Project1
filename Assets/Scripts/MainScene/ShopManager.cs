using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] TMP_Text[] Objects;
    [SerializeField] GameObject Inf;
    [SerializeField] TMP_Text InfText;

    // About Up
    [SerializeField] Button[] UpBTs;
    List<TMP_Text> UpLevels = new List<TMP_Text>();
    List<TMP_Text> UpCosts = new List<TMP_Text>();

    // About Enem
    [SerializeField] List<TMP_Text> EnemLevels;
    [SerializeField] List<Button> EnemUps;
    [SerializeField] List<Button> EnemDown;
    [SerializeField] List<Sprite> GradeSprites;
    [SerializeField] Image Grade;
    [SerializeField] TMP_Text GradeText;
    [SerializeField] TMP_Text ResultText;
    List<GameObject> TestGrades = new List<GameObject>();

    // About Gacha
    [SerializeField] GachaSimul Gacha;

    int[] Costs =
    {
        0,150,300,600,1200,3200,6400,12800,25600,51200,100000,0
    };
    
    List<GameObject> SoldOut = new List<GameObject>();

    private void Start()
    {
        GameManager.instance.Shop = this;
        transform.parent.GetComponent<CanvasScaler>().matchWidthOrHeight = GameManager.instance.RatType;


        for (int i = 0; i < 3; i++) Objects[i].text = $"{GameManager.instance.gameStatus.Objects[i]}";
        foreach(var k in UpBTs)
        {
            UpLevels.Add(k.transform.GetChild(4).GetComponent<TMP_Text>());
            UpCosts.Add(k.transform.GetChild(3).GetComponent<TMP_Text>());
            SoldOut.Add(k.transform.GetChild(5).gameObject);
        }
        for (int i = 0; i < GameManager.instance.gameStatus.Stat.Length; i++)
        {
            int cnt = GameManager.instance.gameStatus.Stat[i];
            UpLevels[i].text = cnt == 11 ? "<color=red>MAX</color>" : $"{cnt-1} / <color=red>10</color>";
            UpCosts[i].text = $"{Costs[cnt]}";
            SoldOut[i].SetActive(cnt == 11);
            UpBTs[i].enabled = !(cnt == 11);
        }

        CurTestLevel = 0;
        for(int i = 0; i < GameManager.instance.gameStatus.Enem.Length; i++)
        {
            int cnt = GameManager.instance.gameStatus.Enem[i];
            EnemLevels[i].text = cnt == 5 ? "<color=red>MAX</color>" : $"{cnt} / <color=red>5</color>";
            EnemUps[i].interactable = cnt != 5;
            EnemDown[i].interactable = cnt != 0;
            CurTestLevel += cnt;
        }

        for (int i = 1; i <= 30; i++) TestGrades.Add(GradeText.transform.parent.GetChild(i).gameObject);
        for (int i = 0; i < CurTestLevel; i++) TestGrades[i].SetActive(true);
        GradeSet(0);
        gameObject.SetActive(false);
    }

    string[] UpStr =
        {
            "공격력", "방어력","체력","이동속도","습득범위","공격속도","초기 코스트","코스트 회복속도","경험치 획득량","리롤 횟수"
        };
    int[] UpCount =
    {
            5,2,4,5,10,2,5,5,5,1
    };

    public void CloseMessage()
    {
        GameManager.instance.FloatM.gameObject.SetActive(false);
    }

    public void FloatMessage(string message)
    {

        GameManager.instance.FloatM.Init(message);
    }

    public void InfChange_Upgrade(int i)
    {
        if (i == 6 || i == 9)
            GameManager.instance.FloatM.Init($"레벨 당 <color=blue><u>{UpStr[i]} {UpCount[i]}</color></u> 증가\n 현재 증가량 : <color=red><u>{UpCount[i] * (GameManager.instance.gameStatus.Stat[i] - 1)}</u></color>");
        else
            GameManager.instance.FloatM.Init($"레벨 당 <color=blue><u>{UpStr[i]} {UpCount[i]}%</color></u> 증가\n 현재 증가량 : <color=red><u>{UpCount[i] * (GameManager.instance.gameStatus.Stat[i] - 1)}%</u></color>");
    }

    string[] DownStr =
        {
            "적 공격력", "적 방어력","적 체력","적 이동속도","적 스폰양","보스 스탯(체력/방어력)"
        };
    int[] DownCount =
    {
            10,5,10,10,10,5
    };
    int CurTestLevel;

    public void InfChange_Enem(int i)
    {
        string j = $"레벨 당 <color=blue><u>{DownStr[i]} {DownCount[i]}%</color></u> 증가\n 현재 증가량 : <color=red><u>{DownCount[i] * (GameManager.instance.gameStatus.Enem[i])}%</u></color>";
        if (i == 5) j += "\n* 다른 적 스탯과 곱연산으로 적용";
        GameManager.instance.FloatM.Init(j);
    }

    

    public void LevelUpEnem(int ind)
    {
        GameManager.instance.gameStatus.Enem[Mathf.Abs(ind) - 1] +=  ind >= 0 ? 1 : -1;
        GradeSet(ind >= 0 ? 1 : -1);
        ind = Mathf.Abs(ind) - 1;
        if (GameManager.instance.gameStatus.Enem[ind] == 5) EnemUps[ind].interactable = false;
        else if (GameManager.instance.gameStatus.Enem[ind] == 0) EnemDown[ind].interactable = false;
        else if (GameManager.instance.gameStatus.Enem[ind] == 4) EnemUps[ind].interactable = true;
        else if (GameManager.instance.gameStatus.Enem[ind] == 1) EnemDown[ind].interactable = true;

        EnemLevels[ind].text = GameManager.instance.gameStatus.Enem[ind] == 5 ? "<color=red>MAX</color>" : $"{GameManager.instance.gameStatus.Enem[ind]} / <color=red>5</color>";
        InfChange_Enem(ind);
    }

    void GradeSet(int Change)
    {
        if (Change == 1) TestGrades[CurTestLevel].SetActive(true);
        if (Change == -1) TestGrades[CurTestLevel-1].SetActive(false);

        CurTestLevel += Change;
        if (CurTestLevel == 0) Grade.sprite = GradeSprites[0];
        else Grade.sprite = GradeSprites[(CurTestLevel-1) / 5 + 1];
        GradeText.text = $"{CurTestLevel}";
        ResultText.text = $"보상 획득 : <color=red>{CurTestLevel * 10}%</color>";
    }

    public void LevelUp(int ind)
    {
        int cnt = GameManager.instance.gameStatus.Stat[ind];
        if (GameManager.instance.gameStatus.Objects[0]>= Costs[cnt])
        {
            GameManager.instance.gameStatus.Objects[0] -= Costs[cnt++];
            Objects[0].text = $"{GameManager.instance.gameStatus.Objects[0]}";
            GameManager.instance.gameStatus.Stat[ind]++;
            UpLevels[ind].text = cnt == 11 ? "<color=red>MAX</color>" : $"{cnt-1} / <color=red>10</color>";
            UpCosts[ind].text = $"{Costs[cnt]}";
            SoldOut[ind].SetActive(cnt == 11);
            UpBTs[ind].enabled = !(cnt == 11);
            InfChange_Upgrade(ind);
        }
        else
        {
            GameManager.instance.FloatM.Init("보유한 용문패가 부족합니다.");
        }
    }


    [SerializeField] List<Button> TabsButton;
    [SerializeField] List<Image> Tabs;
    [SerializeField] List<TMP_Text> Tabs_Text;
    [SerializeField] List<GameObject> Tabs_Sub;
    int CurOpenTab = 0;


    public void ChangeTab(int ind)
    {
        TabsButton[CurOpenTab].interactable = true;
        Tabs[CurOpenTab].color = Color.gray;
        Tabs_Text[CurOpenTab].color = Color.white;
        Tabs_Sub[CurOpenTab].SetActive(false);

        CurOpenTab = ind;
        TabsButton[CurOpenTab].interactable = false;
        Tabs[CurOpenTab].color = Color.white;
        Tabs_Text[CurOpenTab].color = Color.black;
        Tabs_Sub[CurOpenTab].SetActive(true);
    }

    public void GachaAble()
    {
        if (Gacha.GachaNum * 100 <= GameManager.instance.gameStatus.Objects[2])
        {
            Gacha.StartGacha();
            GameManager.instance.gameStatus.Objects[2] -= Gacha.GachaNum * 100;
            Objects[2].text = $"{GameManager.instance.gameStatus.Objects[2]}";
        }
        else
        {
            GameManager.instance.FloatM.Init("보유한 합성옥이 부족합니다.");
        }
    }

}
