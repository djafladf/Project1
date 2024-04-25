using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImMaker : MonoBehaviour
{
    SpriteRenderer TargetSprite;
    Transform TargetPos;
    [SerializeField] int MaxIm;
    [SerializeField] float LastTime;
    [SerializeField] float MakeGap;
    [SerializeField] Color StartColor;
    [SerializeField] Color EndColor;

    Color ColorGap;

    List<GameObject> Images;
    List<SpriteRenderer> Renderers;
    WaitForSeconds MakerGap;
    private void Awake()
    {
        TargetSprite = transform.parent.GetComponent<SpriteRenderer>();
        TargetPos = transform.parent;
        MakerGap = new WaitForSeconds(MakeGap);
        Images = new List<GameObject>() { transform.GetChild(0).gameObject };
        Renderers = new List<SpriteRenderer> { Images[0].GetComponent<SpriteRenderer>() };
        for(int i = 1; i < MaxIm; i++)
        {
            Images.Add(Instantiate(Images[0], transform));
            Renderers.Add(Images[i].GetComponent<SpriteRenderer>());
        }
        foreach (var k in Renderers) k.sortingOrder = TargetSprite.sortingOrder - 1;
        ColorGap = (EndColor - StartColor) / (LastTime * 10);
    }

    WaitForSeconds WFS = new WaitForSeconds(0.1f);
    int LastIm = 0;
    IEnumerator MakeIm()
    {
        int CurIm = LastIm; LastIm = (LastIm + 1) % (MaxIm);
        Images[CurIm].SetActive(true); Images[CurIm].transform.position = TargetPos.position; Renderers[CurIm].sprite = TargetSprite.sprite; Renderers[CurIm].flipX = TargetSprite.flipX;
        Renderers[CurIm].color = StartColor;
        for(int i = 0; i < LastTime * 10; i++)
        {
            yield return WFS;
            Renderers[CurIm].color += ColorGap;
        }
        Images[CurIm].SetActive(false);
    }

    IEnumerator Maker()
    {
        yield return WFS;
        while (true)
        {
            StartCoroutine(MakeIm());
            yield return MakerGap;
        }
    }

    Coroutine Making = null;

    public void StopMaking()
    {
        if(Making != null) StopCoroutine(Making); Making = null;
    }

    public void StartMaking()
    {
        Making = StartCoroutine(Maker());
    }

    private void OnEnable()
    {
        foreach (var k in Images) k.SetActive(false);
        LastIm = 0;
    }
}
