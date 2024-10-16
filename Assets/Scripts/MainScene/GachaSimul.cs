using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GachaSimul : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] List<Sprite> Images;
    [SerializeField] Transform Lights;
    [SerializeField] ParticleSystem Particle;
    [SerializeField] GameObject ResWindow;
    [SerializeField] GameObject Gacha0;
    [SerializeField] GameObject Gacha1;
    [SerializeField] GameObject Bag;


    List<Image> LightList = new List<Image>();
    [SerializeField] Scrollbar scroll;
    [SerializeField] Image image;

    List<RectTransform> ResPan = new List<RectTransform>();
    List<List<Image>> Results = new List<List<Image>>();

    private void Awake()
    {
        for (int i = 0; i < Lights.childCount; i++) LightList.Add(Lights.GetChild(i).GetComponent<Image>());

        ResPan.Add(ResWindow.transform.GetChild(0).GetComponent<RectTransform>());

        for (int i = 0; i < 10; i++)
        {
            Transform cnt;
            if (i != 0)
            {
                cnt = Instantiate(ResPan[0].gameObject, ResWindow.transform).transform;
                ResPan.Add(cnt.GetComponent<RectTransform>());
            }
            else cnt = ResPan[0];
            Results.Add(new List<Image> { ResPan[i].GetChild(0).GetComponent<Image>(), ResPan[i].GetChild(2).GetComponent<Image>() });
        }
    }

    [HideInInspector]public int GachaNum = 1;
    bool IsEndGacha = false;

    private void MakeGacha()
    {
        scroll.enabled = false;
        ResWindow.SetActive(true);
        int LRSpace = 1280 - GachaNum * 125 - (GachaNum - 1) * 5;
        IsEndGacha = false;
        for (int i = 0; i < GachaNum; i++)
        {
            int cnt = Random.Range(0, GameManager.instance.Data.Infos.Count);
            GameManager.instance.gameStatus.Exceed[cnt]++;
            OperatorInfos tmp = GameManager.instance.Data.Infos[cnt];
            Results[i][0].sprite = tmp.Standing2; Results[i][1].sprite = tmp.Head;
            ResPan[i].anchoredPosition = new Vector3(LRSpace + i * 260, (i % 2 == 0 ? 1000 : -1000));
            ResPan[i].gameObject.SetActive(true);
        }
        for (int i = GachaNum; i < 10; i++) ResPan[i].gameObject.SetActive(false);
        StartCoroutine(PanMove());
    }

    WaitForSeconds WFS = new WaitForSeconds(0.05f);
    IEnumerator PanMove()
    {
        for (int i = 1; i <= 40; i++)
        {
            float CurSpeed = 238 / i;
            for (int x = 0; x < GachaNum; x++)
            {
                var tmp = ResPan[x].anchoredPosition;
                if (x % 2 == 0) tmp.y -= CurSpeed;
                else tmp.y += CurSpeed;
                ResPan[x].anchoredPosition = tmp;
            }
            yield return WFS;
        }

        for (int x = 0; x < GachaNum; x++)
        {
            var tmp = ResPan[x].anchoredPosition; tmp.y = 0; ResPan[x].anchoredPosition = tmp;
        }
        IsEndGacha = true;
    }

    public void EndGacha()
    {
        if (!IsEndGacha)
        {
            StopAllCoroutines();
            for (int x = 0; x < GachaNum; x++)
            {
                var tmp = ResPan[x].anchoredPosition; tmp.y = 0; ResPan[x].anchoredPosition = tmp;
            }
            IsEndGacha = true;
        }
        else
        {
            ResWindow.SetActive(false); Gacha1.SetActive(false); Gacha0.SetActive(true);
        }
    }

    private void OnEnable()
    {
        if (GachaNum == 10) Increase.interactable = false;
        if (GachaNum == 1) Decrease.interactable = false;
        Count.text = $"{GachaNum}회 뽑기";
        SumCount.text = $"x {GachaNum * 100}";
    }

    public void OnValueChange()
    {

        int CurType = Mathf.FloorToInt((1 - scroll.value) * 5);
        int lightype = Mathf.FloorToInt((1 - scroll.value) * 3);

        for (int i = 0; i < lightype; i++) LightList[i].gameObject.SetActive(true);
        for (int i = lightype; i < LightList.Count; i++) LightList[i].gameObject.SetActive(false);
        image.sprite = Images[CurType];
        if (scroll.value == 0) MakeGacha();
    }
    [SerializeField] TMP_Text Count;
    [SerializeField] TMP_Text SumCount;
    [SerializeField] Button Increase;
    [SerializeField] Button Decrease;

    public void StartGacha()
    {
        Gacha0.SetActive(false); Gacha1.SetActive(true); Bag.SetActive(true);
        scroll.value = 1; scroll.enabled = true;
    }

    public void AddGacha()
    {
        GachaNum++;
        if (GachaNum == 10) Increase.interactable = false;
        if (GachaNum == 2) Decrease.interactable = true;
        Count.text = $"{GachaNum}회 뽑기";
        SumCount.text = $"x {GachaNum * 100}";
    }

    public void DelGacha()
    {
        GachaNum--;
        if (GachaNum == 9) Increase.interactable = true;
        if (GachaNum == 1) Decrease.interactable = false;
        Count.text = $"{GachaNum}회 뽑기";
        SumCount.text = $"x {GachaNum * 100}";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Particle.Play();
    }

    // 드래그가 끝났을 때 (마우스 버튼을 놓았을 때)
    public void OnPointerUp(PointerEventData eventData)
    {
        Particle.Stop();
    }
}
