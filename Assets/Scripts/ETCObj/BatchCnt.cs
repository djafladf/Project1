using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BatchCnt : MonoBehaviour
{
#if UNITY_STANDALONE
    void Update()
    {
        transform.position = Input.mousePosition;
        Vector3 cnt = Input.mousePosition - Camera.main.WorldToScreenPoint(GameManager.instance.player.Self.position);
        bool BatchAble = cnt.magnitude < GameManager.instance.UM.BatchAreaSize;

        if (!BatchAble) GameManager.instance.UM.BatchImage.color = Color.red;
        else GameManager.instance.UM.BatchImage.color = Color.white;

        if (Input.GetMouseButtonUp(0) && BatchAble)
        {
            GameManager.instance.SetTime(0.1f, true);
            GameManager.instance.UM.CurRequest.EndBatch();
            gameObject.SetActive(false);
        }
    }
#endif
#if UNITY_ANDROID || UNITY_IOS
    private void Update()
    {
        if (Touchscreen.current == null) return;
        transform.position = Touchscreen.current.primaryTouch.position.ReadValue();
        Vector3 cnt = transform.position - Camera.main.WorldToScreenPoint(GameManager.instance.player.Self.position);

        if (!(cnt.magnitude < GameManager.instance.UM.BatchAreaSize)) GameManager.instance.UM.BatchImage.color = Color.red;
        else
        {
            GameManager.instance.UM.BatchImage.color = Color.white;
            if (!Touchscreen.current.press.isPressed)
            {
                GameManager.instance.SetTime(0.1f, true);
                GameManager.instance.UM.CurRequest.EndBatch();
                gameObject.SetActive(false);
            }
        }
    }
#endif
}
