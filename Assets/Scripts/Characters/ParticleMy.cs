using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ParticleMy : MonoBehaviour
{
    [SerializeField] List<Sprite> Sprites;
    [SerializeField] Transform TargetPos;
    [SerializeField] int MaxIm;
    [SerializeField] float LastTime;
    [SerializeField] float MakeGap;
    [SerializeField] int MakeTime;
    [SerializeField] Gradient ColorGrad;
    [SerializeField] AnimationCurve Width;
    [SerializeField] bool MakeOnStart = false;
    
    public List<Quaternion> RotationsByTime = new List<Quaternion>();
    public List<Color> Colors = new List<Color>();
    public List<float> SizeByTime = new List<float>();
    public List<float> StartSize = new List<float>();
    public List<Quaternion> StartRotations = new List<Quaternion>();


    List<GameObject> Images;
    List<SpriteRenderer> Renderers;
    WaitForSeconds MakerGap;
    private void Awake()
    {
        MakerGap = new WaitForSeconds(MakeGap);
        Images = new List<GameObject>() { transform.GetChild(0).gameObject };
        Renderers = new List<SpriteRenderer> { Images[0].GetComponent<SpriteRenderer>() };
        int j = Renderers[0].sortingOrder;
        StartSize.Add(1); StartRotations.Add(new Quaternion(0, 0, 0, 0));
        for (int i = 1; i < MaxIm; i++)
        {
            Images.Add(Instantiate(Images[0], transform));
            Renderers.Add(Images[i].GetComponent<SpriteRenderer>());
            StartSize.Add(1); StartRotations.Add(new Quaternion(0, 0, 0, 0));
        }

        for (int i = 0; i <= LastTime * 10; i++)
        {
            Colors.Add(ColorGrad.Evaluate(1 / LastTime * 0.1f * i));
            SizeByTime.Add(Width.Evaluate(1 / LastTime * 0.1f * i));
        }
        if (MakeOnStart) StartMaking();
    }

    WaitForSeconds WFS = new WaitForSeconds(0.1f);
    int LastIm = 0;
    IEnumerator MakeIm()
    {
        int CurIm = LastIm; LastIm = (LastIm + 1) % (MaxIm);
        Images[CurIm].SetActive(true); Images[CurIm].transform.position = TargetPos.position; Renderers[CurIm].sprite = Sprites[CurIm % Sprites.Count];
        Renderers[CurIm].color = Colors[0];
        Renderers[CurIm].transform.rotation = StartRotations[CurIm];
        Renderers[CurIm].transform.localScale = TargetPos.localScale * StartSize[CurIm] * SizeByTime[0];
        for (int i = 0; i <= LastTime * 10; i++)
        {
            Renderers[CurIm].color = Colors[i];
            Renderers[CurIm].transform.localScale = TargetPos.localScale * StartSize[CurIm] * SizeByTime[i];
            yield return WFS;
            
        }
        Images[CurIm].SetActive(false);
    }

    IEnumerator Maker()
    {
        if(MakeTime > 0)
        for (int i = 0; i < MakeTime; i++)
        {
            StartCoroutine(MakeIm());
            yield return MakerGap;
        }
        else
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
