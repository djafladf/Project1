using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatchCnt : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
        Vector3 cnt = Input.mousePosition - Camera.main.WorldToScreenPoint(GameManager.instance.player.Self.position);
        bool BatchAble = cnt.magnitude < GameManager.instance.UM.BatchAreaSize;

        if (!BatchAble) GameManager.instance.UM.BatchImage.color = Color.red;
        else GameManager.instance.UM.BatchImage.color = Color.white;

        if (Input.GetMouseButtonUp(0) && BatchAble)
        {
            Time.timeScale = 1;
            GameManager.instance.UM.CurRequest.EndBatch();
            gameObject.SetActive(false);
        }
    }
}
