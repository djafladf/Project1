using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] EnemyTypes;
    [SerializeField] GameObject BossSet;
    [SerializeField] int[] PoolSize;
    
    GameObject[,] Pool;
    
    Enemy[,] EnemyScripts;

    [HideInInspector] public int CurActive = 0;

    Vector3[] SpawnArea;
    int SpawnAreaSize;

    [System.Serializable]
    public class StageInfo
    {
        public List<SpawnInfo> spawninfo;   
    }

    public enum EnemyId
    {
        RedWorm, Yoma, FootMan, DualMan, ShieldMan, BowMan, Magician, Skulslr
    }

    [System.Serializable]
    public class SpawnInfo
    {
        public EnemyId id;
        public int start;
        public int end;
        public float respawn;
        public bool IsBoss = false;
    }

    [SerializeField] List<StageInfo> Stages;
    int CurStage = 0;

    private void Awake()
    {
        GameManager.instance.ES = this;
        GameManager.instance.StartLoading();
    }

    public void Init(int Stage)
    {
        //Enems EnemSpawn = JsonConvert.DeserializeObject<Enems>(File.ReadAllText($"{Directory.GetCurrentDirectory()}\\Assets\\JSON\\Stage\\{Stage}.Json"));


        // Set Spawn Area
        SpawnArea = new Vector3[68];
        SpawnAreaSize = SpawnArea.Length;
        int i = 0;
        for (int x = -20; x <= 20; x+=2) SpawnArea[i++] = new Vector3(x, 12,0);
        for (int x = 12; x >= -12; x-=2) { SpawnArea[i++] = new Vector3(-20, x,0); SpawnArea[i++] = new Vector3(20, x,0); }
        for (int x = -20; x <= 20; x+=2) SpawnArea[i++] = new Vector3(x, -12,0);

        EnemyScripts = new Enemy[50, EnemyTypes.Length];
        Pool = new GameObject[50, EnemyTypes.Length];
        for(i = 0; i < EnemyTypes.Length; i++)
        {
            for(int y = 0; y < PoolSize[i]; y++)
            {
                Pool[y, i] = Instantiate(EnemyTypes[i], transform);
                EnemyScripts[y, i] = Pool[y, i].GetComponent<Enemy>();
                Pool[y, i].SetActive(false);
            }
        }
        GameManager.instance.StartLoading();
        //StartStage();
        /*for(i = 0; i < EnemyTypes.Length; i++)
        {
            StartCoroutine(Spawn(EnemSpawn.spawninfo[i]));
        }*/
    }

    public void StartStage()
    {
        BossSet.SetActive(false);
        if (CurStage >= Stages.Count) GameManager.instance.UM.GameClear();
        else
        {
            foreach (SpawnInfo cnt in Stages[CurStage].spawninfo) StartCoroutine(Spawn(cnt));
            CurStage++;
        }
    }

    IEnumerator Spawn(SpawnInfo Info)
    {
        yield return new WaitForSeconds(Info.start);
        int SpawnTimes = Mathf.FloorToInt((Info.end - Info.start) / Info.respawn);
        WaitForSeconds SpawnGap = new WaitForSeconds(Info.respawn);
        if (Info.IsBoss) { GameManager.instance.BossStage(); BossSet.SetActive(true); }
        for (int i = 0; i <= SpawnTimes; i++)
        {
            for (int y = 0; y < PoolSize[(int)Info.id]; y++)
            {
                if (!Pool[y, (int)Info.id].activeSelf)
                {
                    Pool[y, (int)Info.id].transform.position = SpawnArea[Random.Range(0, SpawnAreaSize - 1)] + GameManager.instance.player.Self.position;
                    Pool[y, (int)Info.id].SetActive(true);
                    break;
                }
            }
            if (Info.IsBoss) break;
            yield return SpawnGap;
        }
    }

    public void ExternalSpawnCall(int ind,int Times, float Gap)
    {
        StartCoroutine(ExtraSpawn(ind,Times,Gap));
    }

    IEnumerator ExtraSpawn(int ind,int Times,float Gap)
    {
        WaitForSeconds SpawnGap = new WaitForSeconds(Gap);
        while(Times-- != 0)
        {
            for(int i = 0; i < PoolSize[ind]; i++)
            {
                if (!Pool[i, ind].activeSelf)
                {
                    Pool[i, ind].transform.position = SpawnArea[Random.Range(0, SpawnAreaSize - 1)] + GameManager.instance.player.Self.position;
                    Pool[i, ind].SetActive(true);
                    break;
                }
            }
            yield return SpawnGap;
        }
    }

    public void StopCor()
    {
        StopAllCoroutines();
    }

/*
    public void SpawnEnemy()
    {
        Vector3 SpawnPos = SpawnArea[Random.Range(0, SpawnAreaSize- 1)] + GameManager.instance.player.Self.position;

        for (int i = 0; i < PoolSize[0]; i++)
        {
            if (!Pool[i, 0].activeSelf)
            {
                Pool[i, 0].transform.position = SpawnPos;
                Pool[i, 0].SetActive(true);
                break;
            }
        }
    }*/

    int BatchCall = 0;
    public Vector3 ReBatchCall()
    {
        if (BatchCall >= SpawnArea.Length) BatchCall = 0;
        return SpawnArea[BatchCall++];
    }
}
