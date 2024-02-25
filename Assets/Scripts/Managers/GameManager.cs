using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    public BulletManager BM;
    public ItemManager IM;
    public Vector3[] SpawnArea;
    public int SpawnAreaSize;

    public string PlayerName;
    public int KillCount = 0;
    public int HP = 100;
    public int MaxHP = 100;
    public int CurLevel = 1;

    [SerializeField] TMP_Text Name;
    [SerializeField] TMP_Text Kill;
    [SerializeField] TMP_Text Hp;
    [SerializeField] Image Face;
    [SerializeField] Image ExpBar;
    [SerializeField] TMP_Text Timer;

    float CurTime = 0;

    private void Awake()
    {
        // Set Spawn Area
        SpawnArea = new Vector3[88];
        instance = this;
        int i = 0;
        for (int x = -10; x <= 10; x++) SpawnArea[i++] = new Vector2(x,6);
        for (int x = 11; x >= -11; x--) { SpawnArea[i++] = new Vector2(-11, x); SpawnArea[i++] = new Vector2(11, x); }
        for (int x = -10; x <= 10; x++) SpawnArea[i++] = new Vector2(x, -6);
        SpawnAreaSize = i;

        // Set Player
        Name.text = PlayerName;
        Face.sprite = Resources.Load<Sprite>($"{PlayerName}\\Head");

        // Test
        ExpSub = 0.05f;
    }

    private void FixedUpdate()
    {
        CurTime += Time.fixedDeltaTime;
        Timer.text = string.Format("{0:00}:{1:00}",Mathf.FloorToInt(CurTime/60f),Mathf.FloorToInt(CurTime%60f));
    }


    float ExpSub;
    public void ExpUp(int value)
    {
        float cnt = ExpBar.fillAmount + ExpSub * value;

        while(cnt >= 1)
        {
            CurLevel++;
            ExpSub = ExpSub * 0.9f;
            cnt -= 1;
        }
        ExpBar.fillAmount = cnt;
    }

    public void HpChange(int value)
    {
        if (value > 0 && HP < MaxHP) HP++;
        else if (value < 0 && HP > 0) HP--;
        Hp.text = $"{HP}";
    }

    int CurKill = 0;
    public void KillCountUp(int value)
    {
        CurKill += value;
        Kill.text = $"{CurKill}";
    }
}
