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

    private void Awake()
    {
        EnemyScripts = new Enemy[100, EnemyTypes.Length];
        Pool = new GameObject[100, EnemyTypes.Length];
        for(int i = 0; i < EnemyTypes.Length; i++)
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
        Vector3 SpawnPos = GameManager.instance.SpawnArea[Random.Range(0, GameManager.instance.SpawnAreaSize - 1)] + GameManager.instance.player.Self.position;

        for (int i = 0; i < PoolSize[0]; i++)
        {
            if (!Pool[i, 0].activeSelf)
            {
                Pool[i, 0].SetActive(true);
                Pool[i, 0].transform.position = SpawnPos;
                break;
            }
        }
    }
}
