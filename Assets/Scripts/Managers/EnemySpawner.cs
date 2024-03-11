using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] EnemyTypes;
    [SerializeField] int[] PoolSize;
    
    GameObject[,] Pool;
    
    Enemy[,] EnemyScripts;

    [SerializeField] double MaxAble = 20;
    [HideInInspector] public int CurActive = 0;

    Vector3[] SpawnArea;
    int SpawnAreaSize;

    private void Awake()
    {
        // Set Spawn Area
        SpawnArea = new Vector3[68];
        SpawnAreaSize = SpawnArea.Length;
        int i = 0;
        for (int x = -20; x <= 20; x+=2) SpawnArea[i++] = new Vector3(x, 12,0);
        for (int x = 12; x >= -12; x-=2) { SpawnArea[i++] = new Vector3(-20, x,0); SpawnArea[i++] = new Vector3(20, x,0); }
        for (int x = -20; x <= 20; x+=2) SpawnArea[i++] = new Vector3(x, -12,0);

        EnemyScripts = new Enemy[100, EnemyTypes.Length];
        Pool = new GameObject[100, EnemyTypes.Length];
        for(i = 0; i < EnemyTypes.Length; i++)
        {
            for(int y = 0; y < PoolSize[i]; y++)
            {
                Pool[y, i] = Instantiate(EnemyTypes[i], transform);
                EnemyScripts[y, i] = Pool[y, i].GetComponent<Enemy>();
                Pool[y, i].SetActive(false);
            }
        }

        
    }

    private void Start()
    {
        StartCoroutine(SpawnTest());
    }

    float j = 0;
    WaitForSeconds SpawnGap = new WaitForSeconds(0.1f);
    IEnumerator SpawnTest()
    {
        while (true)
        {
            if (CurActive < MaxAble) { SpawnEnemy(); CurActive++; }
            yield return SpawnGap;
            j += 0.1f; if (j >= 10) { MaxAble *= 1.05; j = 0; }
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
