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


    [SerializeField] TMP_Text Resolution;


    private void Start()
    {
        GameManager.instance.Data = this;
        // 지원되는 해상도 목록 가져오기
    }

    public void StartBT()
    {
        GameManager.instance.StartGame();
    }

    public void GoUrl(string Url)
    {
        Application.OpenURL(Url);
    }
}
