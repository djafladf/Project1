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
    int rarity;
    bool IsWeapon;

    [SerializeField] List<Color> RarityColor;

    public void Init(Sprite Im, string nm,int Rarity, string desc, string ext,int ind,int Level,bool SetRating = false)
    {
        Image.sprite = Im;
        Name.text = nm; Name.color = RarityColor[Rarity];
        Description.text = desc;
        Extra.text = ext;
        Rating.gameObject.SetActive(SetRating); IsWeapon = SetRating;
        Rating.text = $"LV.{Level+1}";
        this.ind = ind; rarity = Rarity;

        NonSelect_Extra = Extra.color; NonSelect_Inst = Description.color;
        Select_Extra = Extra.color + new Color(0.25f, 0.25f, 0.25f, 0); Select_Inst = Description.color + new Color(0.25f, 0.25f, 0.25f, 0);
    }


    Color NonSelect_Inst;
    Color NonSelect_Extra;
    Color Select_Inst;
    Color Select_Extra;


    protected override void Click(PointerEventData Data)
    {
        MyIm.sprite = NonClicked;

        Description.color = NonSelect_Inst;
        Extra.color = NonSelect_Extra;
        GameManager.instance.UM.ApplySelection(ind,IsWeapon,rarity);
    }

    protected override void OnPointer(PointerEventData data)
    {
        MyIm.sprite = Clicked;
        Description.color = Select_Inst;
        Extra.color = Select_Extra;
    }

    protected override void OutPointer(PointerEventData data)
    {
        MyIm.sprite = NonClicked;
        Description.color = NonSelect_Inst;
        Extra.color = NonSelect_Extra;
    }


}
