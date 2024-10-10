using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlideVar : MonoBehaviour
{
    [SerializeField] Scrollbar slide;
    [SerializeField] TMP_Text VarText;

    private void Awake()
    {
        slide = GetComponent<Scrollbar>();
        VarText.text = $"{Mathf.FloorToInt(slide.value*100)}";
    }

    public void ValueChange()
    {
        VarText.text = $"{Mathf.FloorToInt(slide.value*100)}";
    }
}
