using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BatchInfoBT : Buttons
{
    int ind; bool IsUnlocked;
    bool IsLeader;
    BatchSet BS;

    public void Init(int ind,bool IsUnlocked, bool IsLeader, BatchSet bs)
    {
        this.ind = ind; this.IsUnlocked = IsUnlocked; BS = bs; this.IsLeader = IsLeader;

    }


    protected override void OnPointer(PointerEventData data)
    {
        BS.ChangePan(ind, transform);
    }
    protected override void OutPointer(PointerEventData data)
    {

    }
    protected override void Click(PointerEventData Data)
    {
        BS.SelectOper(ind, IsLeader);
    }
}
