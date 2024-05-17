using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astesia_Special : MonoBehaviour
{
    [SerializeField] Player Astesia;
    SpriteRenderer Sprite;
    Coroutine color = null;
    Color Sub = new Color(0.75f, 0.75f, 1);

    private void Awake()
    {
        Sprite = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (color == null) color = StartCoroutine(ColorChange());
            GameManager.instance.BM.MakeBullet(new BulletInfo(Mathf.FloorToInt((1 + GameManager.instance.PlayerStatus.attack) * 15), false, 3), 0, collision.transform.position, Vector3.zero, 0, false);
        }
    }

    IEnumerator ColorChange()
    {
        Sprite.color = Sub;
        yield return GameManager.OneSec;
        Sprite.color = Color.white;
        color = null;
    }
}
