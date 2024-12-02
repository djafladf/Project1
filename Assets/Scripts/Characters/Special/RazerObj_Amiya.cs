using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RazerObj_Amiya : MonoBehaviour
{
    [SerializeField] Transform Bull;
    [SerializeField] BoxCollider2D Coll;


    Vector3 StartSize = new Vector3(10, 15);
    Vector3 Gap = new Vector3(3, 0);
    int i = 0;

    private void OnEnable()
    {
        StartCoroutine(RazerAct());
    }

    IEnumerator RazerAct()
    {
        Bull.localScale = StartSize;
        Coll.enabled = true;
        i = 0;
        while (true)
        {
            yield return GameManager.DotOneSec;
            if (i++ < 10) Bull.localScale += Gap;
            Coll.enabled = Coll.enabled == false;
        }
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }
}
