using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceReturn : Guide, IFacility, IEnemyAttackable
{
    protected ResourceSpawner area;

    public Buildings.Resource returnResources;
    protected Personal_resource resource;
    [SerializeField] protected float maxHp = 100;
    protected float nowHp = 100;
    [SerializeField] protected ConstructionReBuild reBuild;
    [SerializeField] protected Sprite broken;
    protected SpriteRenderer spriteRenderer;
    [HideInInspector] public BoxCollider2D boxCollider;
    [SerializeField] protected Image hpSlider;
    protected LayerMask wall;
    protected PlayerBag bag;
    private Coroutine coroutine;

    protected float MaxHP
    {
        get
        {
            return maxHp * TechTreeUnlock.facilityHP;
        }
    }
    protected float NowHP
    {
        get
        {
            return nowHp * TechTreeUnlock.facilityHP;
        }
        set
        {
            nowHp = value * TechTreeUnlock.facilityHP;
        }
    }

    [SerializeField] protected Vector2 offset = new(0, 2);

    protected virtual void Awake()
    {
        resource = Personal_resource.instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        wall = LayerMask.GetMask("Wall");
        area = FindAnyObjectByType<ResourceSpawner>();
        bag = FindAnyObjectByType<PlayerBag>();
    }

    protected virtual void OnEnable()
    {
        nowHp = maxHp;
    }

    public virtual void TearDown()
    {
        global::Resource.public_watt += returnResources.watt;
        resource.Wooden = Mathf.Clamp(resource.Wooden + returnResources.wooden, 0, bag.capacity);
        resource.Steel = Mathf.Clamp(resource.Steel + returnResources.steel, 0, bag.capacity);

        PlayerGuideUI.guides.Remove(this);

        Destroy(gameObject);
    }

    public virtual void Damage(float damage)
    {
        if (enabled == true)
        {
            NowHP = Mathf.Clamp(NowHP - damage, 0, MaxHP);

            hpSlider.fillAmount = nowHp / maxHp;

            hpSlider.transform.parent.gameObject.SetActive(true);

            if (coroutine != null) StopCoroutine(coroutine);

            coroutine = StartCoroutine(WaitAction.wait(2f, () =>
            {
                hpSlider.transform.parent.gameObject.SetActive(false);

                coroutine = null;
            }));

            if (NowHP <= 0)
            {
                Break();
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        if (hpSlider.transform.parent.gameObject.activeSelf) hpSlider.transform.parent.position = Camera.main.WorldToScreenPoint(transform.position + (Vector3)offset);
    }

    protected virtual void FixedUpdate()
    {
        if (TechTreeUnlock.openAutoFix) Damage(-Time.fixedDeltaTime * (maxHp / 100f));
    }

    public virtual void Break()
    {
        if (enabled == true)
        {
            global::Resource.public_watt += returnResources.watt;

            spriteRenderer.sprite = broken;

            if (TryGetComponent(out Animator anim)) anim.enabled = false;
            if (TryGetComponent(out Collider2D col)) col.isTrigger = true;

            foreach (MonoBehaviour temp in GetComponents<MonoBehaviour>())
            {
                if (temp != this)
                {
                    temp.enabled = false;
                }
            }

            gameObject.layer = wall;

            reBuild.hpBar = hpSlider;
            reBuild.enabled = true;
            enabled = false;
        }
    }

    public override void Enable()
    {
    }

    public override void Disable()
    {
    }

    protected virtual void OnDestroy()
    {
        area.constraints.Remove
            (new Range((Vector2)transform.position + boxCollider.offset - (Vector2)transform.localScale * boxCollider.size / 2f,
                    (Vector2)transform.position + boxCollider.offset + (Vector2)transform.localScale * boxCollider.size / 2f));

        Transform temp = hpSlider.transform.parent;
        while (temp.parent != null)
        {
            temp = temp.parent;
        }

        Destroy(temp.gameObject);
    }
}
