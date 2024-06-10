using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
        YellowWorm,RedWorm, Yoma, FootMan, DualMan, ShieldMan, BowMan, Magician, Skulslr, Parat,Cannon, Revenger,
        BoomSpider,HeavyAr
    }

    [System.Serializable]
    public class SpawnInfo
    {
        public EnemyId id;
        public int start;
        public int end;
        public float respawn;
        public bool IsBoss = false;
        public bool IsLast = false;
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
        // Set Spawn Area
        SpawnArea = new Vector3[100];
        
        int i = 0;
        for (int x = -25; x <= 25; x+=2) SpawnArea[i++] = new Vector3(x, 15,0);
        for (int x = 15; x >= -15; x-=2) { SpawnArea[i++] = new Vector3(-25, x,0); SpawnArea[i++] = new Vector3(25, x,0); }
        for (int x = -25; x <= 25; x+=2) SpawnArea[i++] = new Vector3(x, -15,0);
        SpawnAreaSize = i;
        EnemyScripts = new Enemy[PoolSize.Max(), EnemyTypes.Length];
        Pool = new GameObject[PoolSize.Max(), EnemyTypes.Length];
        MakeNewPref(0, StageInits[0]);
        GameManager.instance.StartLoading();
    }
    [SerializeField] int InitRange;

    [SerializeField]
    List<int> StageInits;

    public void MakeNewPref(int st, int ed)
    {
        if (st == ed && st != 0) return;
        for (int i = st; i <= ed; i++)  for (int y = 0; y < PoolSize[i]; y++)
            {
                Pool[y, i] = Instantiate(EnemyTypes[i], transform);
                EnemyScripts[y, i] = Pool[y, i].GetComponent<Enemy>();
                Pool[y, i].SetActive(false);
            }
    }

    public void StartStage()
    {
        BossSet.SetActive(false);
        if (CurStage >= Stages.Count) GameManager.instance.UM.GameClear();
        else
        {
            if(CurStage != 0)MakeNewPref(StageInits[CurStage]+1, StageInits[CurStage+1]);
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
                    Vector3 cnt = SpawnArea[UnityEngine.Random.Range(0, SpawnAreaSize - 1)] + GameManager.instance.player.Self.position; cnt.z = 1;
                    Pool[y, (int)Info.id].transform.position = cnt;
                    Pool[y, (int)Info.id].SetActive(true);
                    break;
                }
            }
            if (Info.IsBoss) break;
            yield return SpawnGap;
        }
        if (Info.IsLast)
        {
            for(int i = 0; i < PoolSize[(int)Info.id]; i++)
            {
                Destroy(Pool[i, (int)Info.id]); Pool[i, (int)Info.id] = null;
            }
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
                    Vector3 cnt = SpawnArea[Random.Range(0, SpawnAreaSize - 1)] + GameManager.instance.player.Self.position; cnt.z = 1;
                    Pool[i, ind].transform.position = cnt;
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

    public Vector3 ReBatchCall()
    {
        return SpawnArea[Random.Range(0,SpawnAreaSize)];
    }
}
