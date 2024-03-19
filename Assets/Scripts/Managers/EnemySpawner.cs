using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] EnemyTypes;
    [SerializeField] int[] PoolSize;
    
    GameObject[,] Pool;
    
    Enemy[,] EnemyScripts;

    [HideInInspector] public int CurActive = 0;

    Vector3[] SpawnArea;
    int SpawnAreaSize;

    [SerializeField]
    public class Enems
    {
        public List<SpawnInfo> spawninfo;   
    }
    [SerializeField]
    public class SpawnInfo
    {
        public int id;
        public int start;
        public int end;
        public float respawn;
    }


    public void Init(int Stage)
    {
        Enems EnemSpawn = JsonConvert.DeserializeObject<Enems>(File.ReadAllText($"{Directory.GetCurrentDirectory()}\\Assets\\JSON\\Stage\\{Stage}.Json"));


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

        for(i = 0; i < EnemyTypes.Length; i++)
        {
            StartCoroutine(Spawn(EnemSpawn.spawninfo[i]));
        }
    }


    IEnumerator Spawn(SpawnInfo Info)
    {
        yield return new WaitForSeconds(Info.start);
        int SpawnTimes = Mathf.FloorToInt((Info.end - Info.start) / Info.respawn);
        WaitForSeconds SpawnGap = new WaitForSeconds(Info.respawn);
        for (int i = 0; i <= SpawnTimes; i++)
        {
            for (int y = 0; y < PoolSize[Info.id]; y++)
            {
                if (!Pool[y, Info.id].activeSelf)
                {
                    Pool[y, Info.id].transform.position = SpawnArea[Random.Range(0, SpawnAreaSize - 1)] + GameManager.instance.player.Self.position;
                    Pool[y, Info.id].SetActive(true);
                    break;
                }
            }
            yield return SpawnGap;
        }
    }


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
    }

    int BatchCall = 0;
    public Vector3 ReBatchCall()
    {
        if (BatchCall >= SpawnArea.Length) BatchCall = 0;
        return SpawnArea[BatchCall++];
    }
}
