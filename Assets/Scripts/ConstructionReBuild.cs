using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionReBuild : Guide
{
    [SerializeField] private Buildings.Resource needResource;
    [SerializeField] private int needMetal;

    [SerializeField] private TextMeshProUGUI wooden;
    [SerializeField] private TextMeshProUGUI steel;
    [SerializeField] private TextMeshProUGUI metal;
    [SerializeField] private TextMeshProUGUI watt;

    private Personal_resource pr;
    [SerializeField] private Sprite recovery;
    private SpriteRenderer spriteRenderer;
    [HideInInspector] public Image hpBar;

    public enum FacilityKind
    {
        Turret,
        Facility
    }

    [SerializeField] private FacilityKind kind;

    private void Awake()
    {
        pr = FindAnyObjectByType<Personal_resource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Disable()
    {
        if (kind == FacilityKind.Facility)
        {
            CanvasDelete();
            Destroy(gameObject);
        }
        else
        {
            Transform temp = hpBar.transform.parent;
            while (temp.parent != null)
            {
                temp = temp.parent;
            }

            temp.gameObject.SetActive(false);

            ObjectPoolManager.instance[Kind.Turret].Pool.Release(gameObject);
        }
    }

    private void CanvasDelete()
    {
        Transform temp = hpBar.transform.parent;
        while (temp.parent != null)
        {
            temp = temp.parent;
        }

        Destroy(temp.gameObject);
    }

    public override void Enable()
    {
        if (pr.Wooden >= Mathf.FloorToInt(needResource.wooden * TechTreeUnlock.destroyedConstructionFixedSpendResource)
            && pr.Steel >= Mathf.FloorToInt(needResource.steel * TechTreeUnlock.destroyedConstructionFixedSpendResource)
            && pr.Metal >= Mathf.FloorToInt(needMetal * TechTreeUnlock.destroyedConstructionFixedSpendResource) && global::Resource.public_watt >= needResource.watt)
        {
            pr.Wooden -= Mathf.FloorToInt(needResource.wooden * TechTreeUnlock.destroyedConstructionFixedSpendResource);
            pr.Steel -= Mathf.FloorToInt(needResource.steel * TechTreeUnlock.destroyedConstructionFixedSpendResource);
            pr.Metal -= Mathf.FloorToInt(needMetal * TechTreeUnlock.destroyedConstructionFixedSpendResource);
            Resource.public_watt -= needResource.watt;

            spriteRenderer.sprite = recovery;

            if (TryGetComponent(out Animator anim)) anim.enabled = true;
            if (TryGetComponent(out Collider2D col)) col.isTrigger = false;

            foreach (MonoBehaviour temp in GetComponents<MonoBehaviour>())
            {
                if (temp != this) temp.enabled = true;
            }
            gameObject.layer = 0;

            enabled = false;
        }
        else
        {
            Notion.Warning("자원이 부족합니다");
        }
    }

    protected override void Update()
    {
    }

    private void OnEnable()
    {
        wooden.text = "" + needResource.wooden * TechTreeUnlock.destroyedConstructionFixedSpendResource;
        steel.text = "" + needResource.steel * TechTreeUnlock.destroyedConstructionFixedSpendResource;
        metal.text = "" + needMetal * TechTreeUnlock.destroyedConstructionFixedSpendResource;
        watt.text = "" + needResource.watt * TechTreeUnlock.destroyedConstructionFixedSpendResource;

        Transform temp = hpBar.transform.parent;
        while (temp.parent != null)
        {
            temp = temp.parent;
        }

        temp.gameObject.SetActive(true);
    }
}
