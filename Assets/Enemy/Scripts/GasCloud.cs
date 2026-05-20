using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class GasCloud : MonoBehaviour
{
    [Header("Gas Settings")]
    public float duration = 5f;          // 가스 장판 유지 시간
    public float tickInterval = 0.5f;    // 데미지 주기 (0.5초당 1번)
    public int damagePerTick = 5;        // 틱당 데미지
    public float slowAmount = 0.5f;      // 이동 속도 감소율 (50% 감소)

    private float lifetimeTimer = 0f;
    private float tickTimer = 0f;
    private Vector3 targetScale;         // 에디터에서 설정한 기본 스케일을 저장할 변수

    private SpriteRenderer spriteRenderer;

    // 장판 내부의 플레이어 관리 리스트
    private List<PlayerMove> affectedPlayers = new List<PlayerMove>();

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetScale = transform.localScale;
    }

    private void Start()
    {
        // [연출] 원래 세팅된 크기까지만 자연스럽게 커짐
        transform.localScale = Vector3.zero;
        transform.DOScale(targetScale, 0.5f).SetEase(Ease.OutQuad);

        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 0f;
            spriteRenderer.color = c;
            spriteRenderer.DOFade(0.6f, 0.5f);
        }
    }

    private void Update()
    {
        lifetimeTimer += Time.deltaTime;

        if (lifetimeTimer >= duration)
        {
            DestroyCloud();
            return;
        }

        tickTimer += Time.deltaTime;
        if (tickTimer >= tickInterval)
        {
            ApplyTickDamage();
            tickTimer = 0f;
        }
    }

    private void ApplyTickDamage()
    {
        for (int i = affectedPlayers.Count - 1; i >= 0; i--)
        {
            PlayerMove player = affectedPlayers[i];
            if (player == null)
            {
                affectedPlayers.RemoveAt(i);
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
        // 🌟 [레이어 검사 강화] 닿은 오브젝트의 레이어가 "Player"인 경우에만 작동
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (collision.TryGetComponent(out PlayerMove pm))
            {
                if (!affectedPlayers.Contains(pm))
                {
                    affectedPlayers.Add(pm);
                    pm.CurrentSpeedMultiplier = (1f - slowAmount);
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
                if (affectedPlayers.Contains(pm))
                {
                    pm.CurrentSpeedMultiplier = 1f;
                    affectedPlayers.Remove(pm);
                }
            }
        }
    }

    private void DestroyCloud()
    {
        if (TryGetComponent(out Collider2D col)) col.enabled = false;

        ReleaseAllPlayers();

        spriteRenderer.DOFade(0f, 0.5f);
        transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => {
            Destroy(gameObject);
        });
    }

    private void ReleaseAllPlayers()
    {
        foreach (var player in affectedPlayers)
        {
            if (player != null)
            {
                player.CurrentSpeedMultiplier = 1f;
            }
        }
        affectedPlayers.Clear();
    }

    private void OnDestroy()
    {
        ReleaseAllPlayers();
    }
}