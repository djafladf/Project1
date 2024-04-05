using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BatchRemoveBT : Buttons
{
    [SerializeField] Transform Sel;

    protected override void OnPointer(PointerEventData data)
    {
        Sel.gameObject.SetActive(true);
        Sel.transform.position = transform.position;
    }
    protected override void OutPointer(PointerEventData data)
    {
        Sel.gameObject.SetActive(false);
    }
}
