using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FloatMessage : MonoBehaviour
{
    TMP_Text MainText;
    RectTransform ParentRect;
    RectTransform InfRect;
    private void Awake()
    {
        ParentRect = transform.parent.GetComponent<RectTransform>();
        InfRect = GetComponent<RectTransform>();
        MainText = transform.GetChild(0).GetComponent<TMP_Text>();
    }
    private void Start()
    {
        if (GameManager.instance.FloatM == null)
        {
            GameManager.instance.FloatM = this;
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
            Vector3 MousePos = Touchscreen.current.primaryTouch.position.ReadValue();
#endif
#if UNITY_STANDALONE
        Vector3 MousePos = Input.mousePosition;
#endif
        if (ParentRect.sizeDelta.x - MousePos.x < InfRect.sizeDelta.x) MousePos.x = ParentRect.sizeDelta.x - InfRect.sizeDelta.x;
        transform.position = MousePos;
    }
    public void Init(string Message,float font = 50)
    {
        MainText.text = Message; MainText.fontSize = font;
        gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(InfRect);
    }
}
