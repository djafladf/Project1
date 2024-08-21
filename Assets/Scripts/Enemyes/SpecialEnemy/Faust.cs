using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;
using UnityEngine.UIElements;

public class Faust : Enemy
{

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ChangeDirCor = StartCoroutine(ChangeDir());
        StartCoroutine(BatchCool());
        transform.position = GameManager.instance.ES.ReBatchCall();
        Dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        if (Dir.x > 0 && !spriteRenderer.flipX) spriteRenderer.flipX = true;
        else if (Dir.x < 0 && spriteRenderer.flipX) spriteRenderer.flipX = false;
        MakeWalls();
    }


    Vector2 Dir;
    protected override void FixedUpdate()
    {
        if (!IsLive || OnIce) return;
        if (MoveAble && !anim.GetBool("IsAttack"))
        {
            if (BatchAble) { anim.SetTrigger("Batch"); anim.SetBool("IsAttack", true); BatchAble = false; }
            else if (!OnHit) rigid.MovePosition(rigid.position + Dir * speed * Time.fixedDeltaTime * (1 + GameManager.instance.EnemyStatus.speed - DeBuffVar[0]));
        }
    }

    int AttackCount = 0;
    new Transform Target;
    Vector3 DirSub;

    WaitForSeconds changedir = new WaitForSeconds(5);
    Coroutine ChangeDirCor = null;
    IEnumerator ChangeDir()
    {
        yield return changedir;

        Dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        RaycastHit2D IsWallExit = Physics2D.Raycast(transform.position, Dir , 10, LayerMask.GetMask("Wall"));
        while (IsWallExit.collider != null)
        {
            Dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            IsWallExit = Physics2D.Raycast(transform.position, Dir, 7, LayerMask.GetMask("Wall"));
        }

        anim.SetBool("IsAttack",true);
        if (AttackCount++ == 2) anim.SetTrigger("Special");

        Target = GameManager.instance.UM.ReturnOrderPriority().transform;
        DirSub = (Target.position - transform.position).normalized;

        if (DirSub.x > 0 && !spriteRenderer.flipX) spriteRenderer.flipX = true;
        else if (DirSub.x < 0 && spriteRenderer.flipX) spriteRenderer.flipX = false;
        ChangeDirCor = null;
    }

    bool BatchAble = false;
    WaitForSeconds batchcool = new WaitForSeconds(5);
    IEnumerator BatchCool()
    {
        yield return batchcool;
        BatchAble = true;
    }

    public void BatchBow()
    {
        if (Bows.Count != 0)
        {
            int Ind = Random.Range(0, Bows.Count);
            Bows[Ind].SetActive(true); Bows.RemoveAt(Ind);
            StartCoroutine(BatchCool());
        }
    }

    protected override void AttackMethod()
    {
        if (GameManager.instance.player.Self.gameObject.activeSelf)
        {
            Target = GameManager.instance.UM.ReturnOrderPriority().transform;
            DirSub = (Target.position - transform.position).normalized;

            if (DirSub.x > 0 && !spriteRenderer.flipX) spriteRenderer.flipX = true;
            else if (DirSub.x < 0 && spriteRenderer.flipX) spriteRenderer.flipX = false;

            if (AttackCount == 3) { GameManager.instance.BM.MakeBullet(new BulletInfo(Mathf.FloorToInt(Damage * 3 * (1 + GameManager.instance.EnemyStatus.attack - DeBuffVar[1])), false, 0, ignoreDefense: 0.2f), 100, transform.position, DirSub, 50, true, Bull); AttackCount = 0; }
            else GameManager.instance.BM.MakeBullet(new BulletInfo(Mathf.FloorToInt(Damage * (1 + GameManager.instance.EnemyStatus.attack - DeBuffVar[1])), false, 0, ignoreDefense: 0.2f), 0, transform.position, DirSub, 50, true, Bull);
        }
    }

    bool IsHide = true;

    protected override void AttackEnd()
    {
        anim.SetBool("IsAttack", false);
        if (Dir.x > 0 && !spriteRenderer.flipX) spriteRenderer.flipX = true;
        else if (Dir.x < 0 && spriteRenderer.flipX) spriteRenderer.flipX = false;
        if (GameManager.instance.player.Self.gameObject.activeSelf && ChangeDirCor == null) ChangeDirCor = StartCoroutine(ChangeDir());
    }

    [SerializeField] GameObject Wall;
    List<GameObject> Bows = new List<GameObject>();
    void MakeWalls()
    {
        for (float x = -30; x <= 30; x += 2) Instantiate(Wall).transform.position = new Vector3(x, 20, 0);
        for (float x = 18.5f; x >= -19; x -= 1.5f) 
        {
            var cnt = Instantiate(Wall);
            cnt.transform.position = new Vector3(-30, x, 0);
            var tmp = cnt.transform.GetChild(0).gameObject; tmp.GetComponent<SpriteRenderer>().flipX = true; tmp.GetComponent<PaustBow>().Dir = Vector3.right;
            Bows.Add(tmp);
            cnt = Instantiate(Wall);
            cnt.transform.position = new Vector3(30, x, 0);
            tmp = cnt.transform.GetChild(0).gameObject; tmp.GetComponent<PaustBow>().Dir = Vector3.left;
            Bows.Add(tmp);
        }
        for (float x = -30; x <= 30; x += 2) Instantiate(Wall).transform.position = new Vector3(x, -20.5f, 0);
    }

    [SerializeField] GameObject HideEffect;
    public void PhazeT()
    {
        GameManager.instance.UM.BossHP.fillAmount = 1;
        IsHide = false; gameObject.layer = LayerMask.GetMask("Enem");
        HideEffect.SetActive(false);
        Damage = (int)(Damage * 1.5f);
    }

    protected override void Dead()
    {
        gameObject.SetActive(false);
        for (int i = 0; i < 10; i++)
        {
            GameManager.instance.IM.MakeItem(transform.position +
                new Vector3(Random.Range(-10, 10) * 0.3f, Random.Range(-10, 10) * 0.3f, 0), true);
        }
        GameManager.instance.UM.ShowDialog(new List<string>() { "¤·¤µ¤·" },
                () => { GameManager.instance.BossEnd(); }
                );

    }


    protected override void HPChange()
    {
        GameManager.instance.UM.BossHP.fillAmount = HP / (float)MaxHP;
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(!IsHide) base.OnTriggerEnter2D(collision);
    }
}
