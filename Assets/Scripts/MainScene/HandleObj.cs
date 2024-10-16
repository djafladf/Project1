using UnityEngine;
using UnityEngine.EventSystems;

public class HandleObj : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] ParticleSystem PS;

    // 마우스가 Handle 위에 올라갔을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        PS.Play();
    }

    // 마우스가 Handle에서 벗어났을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        PS.Stop();
    }
}
