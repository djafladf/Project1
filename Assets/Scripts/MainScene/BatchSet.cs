using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BatchSet : MonoBehaviour
{
    [SerializeField] List<Image> BatchIms;
    [SerializeField] Sprite NormalBatch;


    [SerializeField] Transform BatchScroll;
    [SerializeField] GameObject BatchPref;
    [SerializeField] GameObject LDBatchPref;
    [SerializeField] GameObject SelectPan;
    [SerializeField] GameObject Extra;    
    
    [SerializeField] TMP_Text Name;
    [SerializeField] TMP_Text HP;
    [SerializeField] TMP_Text Defense;
    [SerializeField] TMP_Text Cost;
    [SerializeField] TMP_Text ReBatch;
    [SerializeField] Image Image;
    [SerializeField] Image Weapon;
    [SerializeField] TMP_Text Inf_Main;
    [SerializeField] TMP_Text Inf_Prop;

    private void Start()
    {
        int j = 0;
        int LDCount = 0;
        bool FS = true;
        foreach(var k in GameManager.instance.Data.Infos)
        {
            GameObject cnt;
            if (k.IsLD)
            {
                if (LDCount == 0) cnt = LDBatchPref;
                else { cnt = Instantiate(LDBatchPref, BatchScroll); cnt.transform.SetSiblingIndex(LDCount); }
                LDCount++;
            }
            else
            {
                if (FS) { cnt = BatchPref;  FS = false; }
                else cnt = Instantiate(BatchPref, BatchScroll);
            }
            cnt.GetComponent<Image>().sprite = k.Head;
            cnt.GetComponent<BatchInfoBT>().Init(j++,false,k.IsLD,this);
            cnt.transform.GetChild(1).GetComponent<TMP_Text>().text = k.name;
        }
        j = 0;
        foreach (var k in GameManager.instance.CurPlayerID)
        {
            if (k == -1) continue;
            BatchIms[j++].sprite = GameManager.instance.Data.Infos[k].Standing2;
        }
        ChangePan(0, BatchPref.transform);
    }
    
    public void ChangePan(int ind, Transform TR)
    {
        SelectPan.transform.SetParent(TR); SelectPan.transform.localPosition = Vector3.zero; SelectPan.SetActive(true);
        OperatorInfos cnt = GameManager.instance.Data.Infos[ind];
        Name.text = cnt.name; HP.text = $"{cnt.player.InitHP}"; Defense.text = $"{cnt.player.InitDefense}";
        Cost.text = $"{cnt.player.Cost}"; ReBatch.text = $"{cnt.player.ReBatchTime}s";
        Image.sprite = cnt.Standing; Weapon.sprite = GameManager.instance.Data.WeaponSub[ind].sprite;
        Inf_Main.text = cnt.Inst_Weapon; Inf_Prop.text = cnt.Inst_Prop;
    }

    public void SelectOper(int ind,bool IsLeader)
    {
        if (IsLeader) 
        {
            GameManager.instance.CurPlayerID[0] = ind;
            BatchIms[0].sprite = GameManager.instance.Data.Infos[ind].Standing2;
        }

        else if (!GameManager.instance.CurPlayerID.Contains(ind) && GameManager.instance.CurPlayerID.Count < 4) 
        { 
            GameManager.instance.CurPlayerID.Add(ind);
            BatchIms[GameManager.instance.CurPlayerID.Count - 1].sprite = GameManager.instance.Data.Infos[ind].Standing2;
        }
    }
    public void RemoveOper(int ind)
    {
        if(ind < GameManager.instance.CurPlayerID.Count && ind !=0)
        {
            GameManager.instance.CurPlayerID.RemoveAt(ind);
            for(int i = 1; i < GameManager.instance.CurPlayerID.Count;i++) BatchIms[i].sprite = GameManager.instance.Data.Infos[GameManager.instance.CurPlayerID[i]].Standing2;
            for (int i = GameManager.instance.CurPlayerID.Count; i < 4; i++) BatchIms[i].sprite = NormalBatch;
        }
    }
}
