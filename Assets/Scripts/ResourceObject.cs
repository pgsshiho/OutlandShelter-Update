using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class ResourceObject : MonoBehaviour
{
    [SerializeField] protected float HP = 100;
    [SerializeField] protected float defense = 0;
    protected float hp;
    public IObjectPool<GameObject> pool;
    [SerializeField] private Image hpBar;
    [SerializeField] Vector2 offset = new(0, 3f);
    protected PlayerBag bag;
    private Coroutine coroutine;

    private void Awake()
    {
        bag = FindAnyObjectByType<PlayerBag>();
    }

    private void OnEnable()
    {
        hp = HP;

        Transform temp = hpBar.transform.parent;
        while (temp.parent != null)
        {
            temp = temp.parent;
        }

        temp.gameObject.SetActive(true);

        hpBar.transform.parent.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (hpBar.transform.parent.gameObject.activeSelf) hpBar.transform.parent.position = Camera.main.WorldToScreenPoint(transform.position + (Vector3)offset);
    }

    public virtual int Break(float amount)
    {
        Backpack.ResourceKind kind = this is ResourceWood ? Backpack.ResourceKind.Wooden : this is ResourceSteel ? Backpack.ResourceKind.Steel : Backpack.ResourceKind.Metal;

        if (bag.capacity - bag.resources[(int)kind] > 0)
        {
            hp -= Mathf.Clamp(amount - defense, 0, bag.capacity - bag.resources[(int)kind]);

            hpBar.fillAmount = hp / HP;

            hpBar.transform.parent.gameObject.SetActive(true);

            if (coroutine != null) StopCoroutine(coroutine);
            
            coroutine = StartCoroutine(WaitAction.wait(2f, () =>
            {
                hpBar.transform.parent.gameObject.SetActive(false);

                coroutine = null;
            }));

            if (hp > 0)
            {
                return (int)Mathf.Clamp(amount - defense, 0, bag.capacity - bag.resources[(int)kind]);
            }
            else
            {
                StartCoroutine(WaitAction.waitOneFrame(() =>
                {
                    CanvasDelete();
                    pool.Release(gameObject);
                }));
                return (int)Mathf.Clamp(amount - defense, 0, bag.capacity - bag.resources[(int)kind]);
            }
        }
        else
        {
            Notion.Warning("¿Œ∫•≈‰∏Æ∞° ≤À √°Ω¿¥œ¥Ÿ!!!");
            return 0;
        }
    }

    private void CanvasDelete()
    {
        Transform temp = hpBar.transform.parent;
        while (temp.parent != null)
        {
            temp = temp.parent;
        }

        temp.gameObject.SetActive(false);
    }
}
