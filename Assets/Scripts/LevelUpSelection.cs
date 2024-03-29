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
    [SerializeField] TMP_Text Rating;
    [SerializeField] Image Image;

    [SerializeField] GameObject SelectionButtons;
    int ind;

    public void Init(Sprite Im, string nm, string desc, string ext,int ind,int Level,bool SetRating = false)
    {
        Image.sprite = Im;
        Name.text = nm;
        Description.text = desc;
        Extra.text = ext;
        Rating.gameObject.SetActive(SetRating);
        Rating.text = $"LV.{Level+1}";
        this.ind = ind;
    }

    Color Sub = new Color(0.25f, 0.25f, 0.25f,0);

    protected override void Click(PointerEventData Data)
    {
        MyIm.sprite = NonClicked;
        
        Description.color -= Sub;
        Extra.color -= Sub;
        GameManager.instance.SetTime(0, true);
        GameManager.instance.UM.ApplySelection(ind);
        transform.parent.parent.gameObject.SetActive(false);
    }

    protected override void OnPointer(PointerEventData data)
    {
        MyIm.sprite = Clicked;
        Description.color += Sub;
        Extra.color += Sub;
    }

    protected override void OutPointer(PointerEventData data)
    {
        MyIm.sprite = NonClicked;
        Description.color -= Sub;
        Extra.color -= Sub;
    }


}
