using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    public Vector3[] SpawnArea;
    public int SpawnAreaSize;

    private void Awake()
    {
        SpawnArea = new Vector3[136];
        instance = this;
        int i = 0;
        for (int x = -21; x <= 21; x++) SpawnArea[i++] = new Vector2(x,12);
        for (int x = 12; x >= -12; x--) { SpawnArea[i++] = new Vector2(-22, x); SpawnArea[i++] = new Vector2(22, x); }
        for (int x = -21; x <= 21; x++) SpawnArea[i++] = new Vector2(x, -12);
        SpawnAreaSize = i;
    }
}
