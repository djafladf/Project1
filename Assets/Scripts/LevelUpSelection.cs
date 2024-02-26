using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelUpSelection : Buttons
{
    [SerializeField] Image MyIm;
    [SerializeField] Sprite Clicked;
    [SerializeField] Sprite NonClicked;
    [SerializeField] TMP_Text Name;
    [SerializeField] TMP_Text Description;
    [SerializeField] TMP_Text Extra;
    [SerializeField] Image Image;
    int ind;

    public void Init(Sprite Im, string nm, string desc, string ext,int ind)
    {
        Image.sprite = Im;
        Name.text = nm;
        Description.text = desc;
        Extra.text = ext;
        this.ind = ind;
    }

    protected override void Click(PointerEventData Data)
    {
        MyIm.sprite = NonClicked;
        Time.timeScale = 1;
        transform.parent.parent.gameObject.SetActive(false);
    }

    protected override void OnPointer(PointerEventData data)
    {
        MyIm.sprite = Clicked;
    }

    protected override void OutPointer(PointerEventData data)
    {
        MyIm.sprite = NonClicked;
    }


}
