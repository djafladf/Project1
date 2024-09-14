using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseArea : MonoBehaviour
{
    GameObject[] Barriers = { null, null, null, null,null,null };
    [SerializeField] GameObject Barrier;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") || collision.CompareTag("Player_Hide"))
        {
            int num = collision.name[0] - '0';
            GameManager.instance.Players[num].Unbeat = true;
            if (Barriers[num] == null) Barriers[num] = Instantiate(Barrier, GameManager.instance.Players[num].Self);
            else Barriers[num].SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Player_Hide"))
        {
            int num = collision.name[0] - '0';
            GameManager.instance.Players[num].Unbeat = false;
            Barriers[num].SetActive(false);
        }
    }
}
