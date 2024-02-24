using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class PlayerArea: MonoBehaviour
{
    Vector2 ColliderSize;

    Vector2 RU = new Vector2(1, 1);

    [SerializeField] int Test;

    private void Start()
    {
        if (TryGetComponent(out CompositeCollider2D CD)) ColliderSize = CD.bounds.size;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Area"))
        {
            Vector3 PlayerPos = GameManager.instance.player.Self.transform.position;
            Vector3 MyPos = transform.position;

            Vector3 Dir = GameManager.instance.player.Dir;
            float diffX = MyPos.x - PlayerPos.x;
            float diffY = MyPos.y - PlayerPos.y;

            Vector2 DirSub = Vector2.zero;

            if (diffX >= ColliderSize.x) DirSub.x = -ColliderSize.x;
            else if (diffX <= -ColliderSize.x) DirSub.x = ColliderSize.x;

            if (diffY >= ColliderSize.y) DirSub.y = -ColliderSize.y;
            else if (diffY <= -ColliderSize.y) DirSub.y = ColliderSize.y;
            
            if(!DirSub.Equals(Vector2.zero)) transform.Translate(DirSub * Test);
        }
    }
}
