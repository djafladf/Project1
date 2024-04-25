using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OperatorBatchTool : Buttons
{
    [NonSerialized] public int Cost = 5;
    [NonSerialized] public int ReBatchTime = 10;
    GameObject BatchObject;

    int LeftReBatchTime = 0;
    bool CanBatch = true;

    [SerializeField] TMP_Text cost;
    [SerializeField] Image ReBatchIm;
    [SerializeField] TMP_Text ReBatchText;
    [SerializeField] Image pan;
    [SerializeField] Image face;
    [SerializeField] Image state;
    public Image HPBar;
    Sprite ObjectImage;

    Color CCnt = new Color(0.5f, 0.5f, 0.5f, 1);
    Color Black = new Color(0.1f, 0.1f, 0.1f, 0.5f);
    Color Red = new Color(1, 0, 0, 0.5f);


    public void Init(GameObject BatchObj, Sprite Head, int Cost, int BatchTime)
    {
        this.Cost = Cost; ReBatchTime = BatchTime;
        cost.text = $"{Cost}";
        BatchObject = BatchObj;
        ObjectImage = BatchObject.GetComponent<SpriteRenderer>().sprite;
        face.sprite = Head; CanBatch = true;
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

    
    protected override void Click(PointerEventData Data)
    {
        if (BatchObject.activeSelf)
        {
            BatchObject.SetActive(false);
            StartCoroutine(ReBatchAble());
        }
        else if(CanBatch)
        {
            if (GameManager.instance.UM.BatchRequest(ObjectImage, this))
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
        Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); newPos.z = 1;
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
        state.gameObject.SetActive(true); ReBatchIm.gameObject.SetActive(true); ReBatchText.gameObject.SetActive(true); state.color = Red;
        LeftReBatchTime = ReBatchTime;
        ReBatchIm.fillAmount = 1; float a = 1 / (float)ReBatchTime; ReBatchText.text = $"{ReBatchTime}";
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
