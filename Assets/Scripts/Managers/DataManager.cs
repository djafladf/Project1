using Newtonsoft.Json;
using UnityEngine;

using System.Collections.Generic;
using TMPro;

public class DataManager : MonoBehaviour
{
    [SerializeField]
    public string[] Player_ID;

    [SerializeField]
    public List<ItemSub> Items;

    [SerializeField]
    public List<ItemSub> BossItems;

    [SerializeField]
    public List<ItemSub> WeaponSub;

    [SerializeField]
    public List<OperatorInfos> Infos;


    [SerializeField] TMP_Text Title;
    Material Mat;

    private void Start()
    {
        GameManager.instance.Data = this;
        Mat = Title.fontMaterial;
        Mat.SetColor("_GlowColor", Color.red);
        Title.fontSize = 200;
    }

    Color TitleSub = new Color(1, 0, 0);

    int ind = 1;
    int df = 1;

    private void Update()
    {
        TitleSub[ind] += Time.deltaTime * df;
        if(df == 1 && TitleSub[ind] >= 1)
        {
            TitleSub[ind] = 1;
            ind--; if (ind < 0) ind = 2;
            df = -1;
        }
        else if(df == -1 && TitleSub[ind] <= 0)
        {
            TitleSub[ind] = 0;
            ind--; if (ind < 0) ind = 2;
            df = 1;
        }


        Mat.SetColor("_GlowColor", TitleSub);
    }

    public void StartBT()
    {
        GameManager.instance.StartGame();
    }

    public void GoUrl(string Url)
    {
        Application.OpenURL(Url);
    }

    public void ShowSetting()
    {
        GameManager.instance.SettingM.gameObject.SetActive(true);
    }
}
