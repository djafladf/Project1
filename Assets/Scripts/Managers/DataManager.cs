using Newtonsoft.Json;
using UnityEngine;

using System.Collections.Generic;

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


    private void Start()
    {
        GameManager.instance.Data = this;
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
