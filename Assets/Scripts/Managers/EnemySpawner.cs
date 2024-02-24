using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] EnemyTypes;
    [SerializeField] int[] PoolSize;
    
    GameObject[,] Pool;
    
    Enemy[,] EnemyScripts;


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

    IEnumerator SpawnTest()
    {
        for(int i = 0; i < 50; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.5f);
        }
    }


    public void SpawnEnemy()
    {
        Vector3 SpawnPos = GameManager.instance.SpawnArea[Random.Range(0, GameManager.instance.SpawnAreaSize - 1)] + GameManager.instance.player.Self.position;

        for (int i = 0; i < 50; i++)
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
