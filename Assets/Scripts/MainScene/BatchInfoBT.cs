using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BatchInfoBT : Buttons
{
    int ind; bool IsUnlocked;
    BatchSet BS;

    public void Init(int ind,bool IsUnlocked, BatchSet bs)
    {
        this.ind = ind; this.IsUnlocked = IsUnlocked; BS = bs;

    }


    protected override void OnPointer(PointerEventData data)
    {
        BS.ChangePan(ind,transform);
    }
    protected override void OutPointer(PointerEventData data)
    {

    }
    protected override void Click(PointerEventData Data)
    {
        BS.SelectOper(ind);
    }
}
