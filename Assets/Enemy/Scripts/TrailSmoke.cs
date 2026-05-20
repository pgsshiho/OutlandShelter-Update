using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class TrailSmoke : MonoBehaviour
{
    [Header("Smoke Settings")]
    public float duration = 2f;          // 연기가 필드에 머무는 시간
    public float tickInterval = 0.4f;    // 데미지 주기
    public int damagePerTick = 2;        // 잔상 데미지 수치

    private float lifetimeTimer = 0f;
    private float tickTimer = 0f;
    private Vector3 targetScale;         // 에디터에서 설정한 기본 스케일을 저장할 변수

    private SpriteRenderer spriteRenderer;
    private List<PlayerMove> targetPlayers = new List<PlayerMove>();

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetScale = transform.localScale;
    }

    private void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(targetScale, 0.3f).SetEase(Ease.OutCubic);

        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 0f;
            spriteRenderer.color = c;
            spriteRenderer.DOFade(0.4f, 0.2f);
        }
    }

    private void Update()
    {
        lifetimeTimer += Time.deltaTime;

        if (lifetimeTimer >= duration)
        {
            FadeAndDestroy();
            return;
        }

        tickTimer += Time.deltaTime;
        if (tickTimer >= tickInterval)
        {
            ApplyTickEffect();
            tickTimer = 0f;
        }
    }

    private void ApplyTickEffect()
    {
        for (int i = targetPlayers.Count - 1; i >= 0; i--)
        {
            PlayerMove player = targetPlayers[i];

            if (player == null)
            {
                targetPlayers.RemoveAt(i);
                continue;
            }

            if (player.TryGetComponent(out IDamageable d))
            {
                d.Damage(damagePerTick);
            }

            if (player.TryGetComponent(out PoisonStatus ps))
            {
                ps.ApplyPoison(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 🌟 [레이어 검사 강화] 닿은 오브젝트의 레이어가 "Player"인 경우에만 리스트 등록
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (collision.TryGetComponent(out PlayerMove pm))
            {
                if (!targetPlayers.Contains(pm))
                {
                    targetPlayers.Add(pm);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (collision.TryGetComponent(out PlayerMove pm))
            {
                if (targetPlayers.Contains(pm))
                {
                    targetPlayers.Remove(pm);
                }
            }
        }
    }

    private void FadeAndDestroy()
    {
        if (TryGetComponent(out Collider2D col)) col.enabled = false;

        targetPlayers.Clear();

        spriteRenderer.DOFade(0f, 0.4f);
        transform.DOScale(Vector3.zero, 0.4f).OnComplete(() => {
            Destroy(gameObject);
        });
    }
}