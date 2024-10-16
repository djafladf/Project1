using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MinimumButton : Buttons
{
    [SerializeField] float MaxY;
    [SerializeField] Sprite MaxB;
    [SerializeField] float MinY;
    [SerializeField] Sprite SmallB;
    [SerializeField] List<GameObject> Subs;
    [SerializeField] RectTransform Moved;
    Image image;

    bool MinimumType;
    float Gap;
    bool IsOn = false;

    protected override void Awake()
    {
        base.Awake();
        Gap = (MaxY - MinY) * 0.05f;
        image = GetComponent<Image>();
        
    }
    protected override void Click(PointerEventData Data)
    {
        StartCoroutine(Mover());
    }

    WaitForSeconds wfs = new WaitForSeconds(0.01f);
    IEnumerator Mover()
    {
        image.sprite = IsOn ? SmallB : MaxB;
        Vector2 GapVec = new Vector2(0, Gap);
        for(int i = 0; i < 20; i++)
        {
            var cnt = Moved.anchoredPosition;
            if (IsOn) cnt -= GapVec;
            else cnt += GapVec;
            Moved.anchoredPosition = cnt;
            yield return wfs;
        }
        
        IsOn = IsOn == false; foreach (var k in Subs) k.SetActive(IsOn);
    }
}
