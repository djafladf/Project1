using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class OperatorBatchTool : Buttons
{
    [NonSerialized] public int Cost = 5;
    [NonSerialized] public int ReBatchTime = 10;
    GameObject BatchObject;

    int LeftReBatchTime = 0;
    int CharInd = 0;
    bool CanBatch = true;
    int BatchCount = 0;

    [SerializeField] TMP_Text cost;
    [SerializeField] Image ReBatchIm;
    [SerializeField] TMP_Text ReBatchText;
    [SerializeField] Image pan;
    [SerializeField] Image face;
    [SerializeField] Image state;

    [SerializeField] OnOffButton FollowSet;
    [SerializeField] OnOffButton MoveSet;
 
    public Image HPBar;
    Sprite ObjectImage;

    Color CCnt = new Color(0.5f, 0.5f, 0.5f, 1);
    Color Black = new Color(0.1f, 0.1f, 0.1f, 0.5f);
    Color Red = new Color(1, 0, 0, 0.5f);


    public void Init(GameObject BatchObj, Sprite Head, int Cost, int BatchTime,int CharInd)
    {
        this.Cost = Cost; ReBatchTime = BatchTime; this.CharInd = CharInd;
        cost.text = $"{Cost}";
        BatchObject = BatchObj;
        ObjectImage = BatchObject.GetComponent<SpriteRenderer>().sprite;
        face.sprite = Head; CanBatch = true;
    }

    public void AllowFollow(int ind = -1)
    {
        if (ind >= 0)
        {
            if (!GameManager.instance.gameStatus.UnitKeySetting[ind][CharInd-1][1]) { GameManager.instance.Players[CharInd].AllowFollow = true; FollowSet.ExternalOff(); }
            else { GameManager.instance.Players[CharInd].AllowFollow = false; FollowSet.ExternalOn(); }
        }
        else GameManager.instance.Players[CharInd].AllowFollow = GameManager.instance.Players[CharInd].AllowFollow == false;
    }

    public void AllowMove(int ind = -1)
    {
        if (ind >= 0)
        {
            if (!GameManager.instance.gameStatus.UnitKeySetting[ind][CharInd-1][0]) { GameManager.instance.Players[CharInd].AllowMove = true; MoveSet.ExternalOff(); }
            else { GameManager.instance.Players[CharInd].AllowMove = false; MoveSet.ExternalOn(); }
        }
        else GameManager.instance.Players[CharInd].AllowMove = GameManager.instance.Players[CharInd].AllowMove == false;
    }

    private void FixedUpdate()
    {
        if (BatchObject.activeSelf) return;

        if(GameManager.instance.UM.CurCost >= Cost && !CanBatch)
        {
            CanBatch = true;
            face.color = Color.white;
            pan.color = Color.white;
        }
        else if (GameManager.instance.UM.CurCost < Cost && CanBatch)
        {
            CanBatch = false;
            face.color = CCnt;
            pan.color = CCnt;
        }
    }

    /*private void OnPointerMove(InputEventPtr eventPtr, InputDevice device)
    {
        print(Pointer.current);
    }*/
    
    protected override void Click(PointerEventData Data)
    {
        if (BatchObject.activeSelf)
        {
            GameManager.instance.UM.BatchOrder.Remove(CharInd);
            BatchObject.SetActive(false);
            state.color = Black;
            StartCoroutine(ReBatchAble());
        }
        else if(CanBatch)
        {
            if (GameManager.instance.UM.BatchRequest(ObjectImage, this,CharInd))
            {
                CanBatch = false;
                face.color = Color.white;
                pan.color = Color.white;
                GameManager.instance.SetTime(0.1f, false);
            }
        }
    }

    protected override void OnPointer(PointerEventData data)
    {
        if (BatchObject.activeSelf)
        {
            state.color = Red;
        }
        else if(CanBatch)
        {
            face.color = CCnt;
            pan.color = CCnt;
        }
        
    }

    protected override void OutPointer(PointerEventData data)
    {
        if (BatchObject.activeSelf)
        {
            state.color = Black;
        }
        else if(CanBatch)
        {
            face.color = Color.white;
            pan.color = Color.white;
        }
    }

    public void EndBatch()
    {
        BatchObject.SetActive(true);
#if UNITY_STANDALONE
        Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); newPos.z = 1;
#endif
#if UNITY_ANDROID || UNITY_IOS
        Vector3 newPos = Camera.main.ScreenToWorldPoint(Touchscreen.current.primaryTouch.position.ReadValue()); newPos.z = 1;
#endif
        if (BatchCount < 2) BatchCount++;
        Cost = Mathf.FloorToInt(Cost * Mathf.Pow(1.2f,BatchCount)); cost.text = $"{Cost}";
        BatchObject.transform.position = newPos;
        state.gameObject.SetActive(true);
        ReBatchIm.gameObject.SetActive(false);
        ReBatchText.gameObject.SetActive(false);
        GameManager.instance.UM.EndBatch(Cost);
    }

    public void ReBatch()
    {
        StartCoroutine(ReBatchAble());
    }

    IEnumerator ReBatchAble()
    {
        state.gameObject.SetActive(true); ReBatchIm.gameObject.SetActive(true); ReBatchText.gameObject.SetActive(true); 
        LeftReBatchTime = ReBatchTime;
        ReBatchIm.fillAmount = 1; float a = 1 / (float)ReBatchTime; ReBatchText.text = $"{ReBatchTime}"; state.color = Red;
        ET.enabled = false;
        while (LeftReBatchTime > 0)
        {
            yield return GameManager.OneSec;
            ReBatchIm.fillAmount -= a;
            LeftReBatchTime--;
            ReBatchText.text = $"{LeftReBatchTime}";
        }
        state.color = Black; state.gameObject.SetActive(false); ReBatchIm.gameObject.SetActive(false); ReBatchText.gameObject.SetActive(false);
        ET.enabled = true;
    }
}
