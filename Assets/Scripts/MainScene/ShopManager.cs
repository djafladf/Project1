using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] TMP_Text[] Objects;

    // About Up
    [SerializeField] Button[] UpBTs;
    List<TMP_Text> UpLevels = new List<TMP_Text>();
    List<TMP_Text> UpCosts = new List<TMP_Text>();

    int[] Costs =
    {
        0,150,300,600,1200,3200,6400,12800,25600,51200,0
    };
    
    List<GameObject> SoldOut = new List<GameObject>();

    private void Start()
    {
        GameManager.instance.Shop = this;
        for (int i = 0; i < 3; i++) Objects[i].text = $"{GameManager.instance.gameStatus.Objects[i]}";
        foreach(var k in UpBTs)
        {
            UpLevels.Add(k.transform.GetChild(4).GetComponent<TMP_Text>());
            UpCosts.Add(k.transform.GetChild(3).GetComponent<TMP_Text>());
            SoldOut.Add(k.transform.GetChild(5).gameObject);
        }
        for(int i = 0; i < GameManager.instance.gameStatus.Stat.Length; i++)
        {
            int cnt = GameManager.instance.gameStatus.Stat[i];
            UpLevels[i].text = cnt == 10 ? "<color=red>MAX</color>" : $"{cnt} / <color=red>10</color>";
            UpCosts[i].text = $"{Costs[cnt]}";
            SoldOut[i].SetActive(cnt == 10);
            UpBTs[i].enabled = !(cnt == 10);
        }
        gameObject.SetActive(false);
    }

    public void LevelUp(int ind)
    {
        int cnt = GameManager.instance.gameStatus.Stat[ind];
        if (GameManager.instance.gameStatus.Objects[0]>= Costs[cnt])
        {
            GameManager.instance.gameStatus.Objects[0] -= Costs[cnt++];
            Objects[0].text = $"{GameManager.instance.gameStatus.Objects[0]}";
            GameManager.instance.gameStatus.Stat[ind]++;
            UpLevels[ind].text = cnt == 10 ? "<color=red>MAX</color>" : $"{cnt} / <color=red>10</color>";
            UpCosts[ind].text = $"{Costs[cnt]}";
            SoldOut[ind].SetActive(cnt == 10);
            UpBTs[ind].enabled = !(cnt == 10);
        }
        else
        {
        }
    }
}
