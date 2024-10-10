using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnOffButton : Buttons
{
    [SerializeField] Sprite OnSprite;
    [SerializeField] Sprite OffSprite;
    [SerializeField] TMP_Text Text;
    [SerializeField] Color OnColor;
    [SerializeField] Color OffColor;
    Image Render;

    [Serializable] class Event : UnityEvent<BaseEventData> { }

    [SerializeField] Event ClickEvent;

    [HideInInspector] public bool Onuse = false;

    protected override void Awake()
    {
        base.Awake();
        Render = GetComponent<Image>();
    }

    protected override void Click(PointerEventData Data)
    {
        Onuse = Onuse == false;
        ClickEvent.Invoke(Data);
        Data.Use();
    }

    protected override void OnPointer(PointerEventData data)
    {
        if (OnSprite == null) Render.color = Onuse ? OffColor : OnColor;
        else Render.sprite = Onuse ? OffSprite : OnSprite;
        if (Text != null) Text.color = Onuse ? OffColor : OnColor;
        data.Use();
    }

    protected override void OutPointer(PointerEventData data)
    {
        if(OnSprite == null) Render.color = Onuse ? OnColor : OffColor;
        else Render.sprite = Onuse ? OnSprite : OffSprite;
        if (Text != null) Text.color = Onuse ? OnColor : OffColor;
        data.Use();
    }

    public void ExternalOn()
    {
        Onuse = true;
        if (OnSprite == null) Render.color = OnColor;
        else Render.sprite = OnSprite;
        if (Text != null) Text.color = OnColor;
    }

    public void ExternalOff()
    {
        Onuse = false;
        if (OffSprite == null) Render.color = OffColor;
        else Render.sprite = OffSprite;
        if (Text != null) Text.color = OffColor;
    }

    public void DisAbleEvent()
    {
        ET.enabled = false;
    }

    public void AbleEvent()
    {
        ET.enabled = true;
    }

}
