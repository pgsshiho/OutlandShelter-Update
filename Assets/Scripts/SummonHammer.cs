using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonHammer : SummonObject
{
    private Animator anim;
    private Collider2D col;
    [SerializeField] private float breakDamage;
    [SerializeField] private float recoveryAmount;
    public Personal_resource pr;
    private PlayerBag bag;

    Vector3 initScale;

    protected override void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        initScale = transform.localScale;
        bag = FindAnyObjectByType<PlayerBag>();
    }

    protected override void OnEnable()
    {
        anim.SetTrigger("Attack");

        col.enabled = true;

        transform.localScale = initScale * TechTreeUnlock.hammerRange;

        StartCoroutine(WaitAction.wait(0.02f, () => col.enabled = false));
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        
        if (other.TryGetComponent(out ResourceObject resource))
        {
            int addResource = resource.Break(breakDamage);

            if (resource is ResourceWood)
            {
                pr.Wooden += addResource;
            }
            else if (resource is ResourceSteel)
            {
                pr.Steel += addResource;
            }
        }

        if (other.TryGetComponent(out ICenter center))
        {
            center.Recovery(recoveryAmount * TechTreeUnlock.fixing);
        }
        else if (other.TryGetComponent(out IFacility facility))
        {
            facility.Damage(-recoveryAmount * TechTreeUnlock.fixing);
        }
        else if (other.TryGetComponent(out ITurret turret))
        {
            turret.Damage(-recoveryAmount * TechTreeUnlock.fixing);
        }
    }
}
