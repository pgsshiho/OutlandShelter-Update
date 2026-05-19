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

    private SpriteRenderer spriteRenderer;

    // 장판 안에 있는 플레이어들과 각 플레이어의 원래 속도를 저장하는 딕셔너리
    private Dictionary<PlayerMove, float> affectedPlayers = new Dictionary<PlayerMove, float>();

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // [연출] 생성될 때 크기가 커지면서 자연스럽게 페이드인
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one * 3f, 0.5f).SetEase(Ease.OutQuad); // 장판 크기에 맞게 조절

        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 0f;
            spriteRenderer.color = c;
            spriteRenderer.DOFade(0.6f, 0.5f); // 반투명하게 설정
        }
    }

    private void Update()
    {
        lifetimeTimer += Time.deltaTime;

        // 1. 지속 시간이 다 되면 소멸 연출 후 파괴
        if (lifetimeTimer >= duration)
        {
            DestroyCloud();
            return;
        }

        // 2. 주기적인 도트 데미지 처리
        tickTimer += Time.deltaTime;
        if (tickTimer >= tickInterval)
        {
            ApplyTickDamage();
            tickTimer = 0f;
        }
    }

    private void ApplyTickDamage()
    {
        // 딕셔너리에 있는 모든 플레이어에게 데미지 및 독 상태이상 부여
        foreach (var player in affectedPlayers.Keys)
        {
            if (player != null)
            {
                if (player.TryGetComponent(out IDamageable d))
                {
                    d.Damage(damagePerTick);
                }

                if (player.TryGetComponent(out PoisonStatus ps))
                {
                    ps.ApplyPoison(false); // Haze의 독 적용
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerMove pm))
        {
            // 장판에 들어오면 플레이어의 속도 배율을 감소시킴 (50% 슬로우라면 0.5f)
            pm.CurrentSpeedMultiplier = (1f - slowAmount);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerMove pm))
        {
            // 장판에서 나가면 원래 속도 배율(100%)로 복구
            pm.CurrentSpeedMultiplier = 1f;
        }
    }

    private void DestroyCloud()
    {
        // 장판 파괴 시 안에 있던 플레이어 속도 복구 예외처리
        // (간단하게 OverlapCircle로 현재 닿아있는 플레이어를 찾아 원복하거나, 
        // 기존에 관리하던 affectedPlayers 리스트가 있다면 pm.CurrentSpeedMultiplier = 1f를 대입해주면 됩니다.)

        spriteRenderer.DOFade(0f, 0.5f);
        transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => {
            Destroy(gameObject);
        });
    }

    private void RemoveSlowEffect(PlayerMove pm)
    {
        if (affectedPlayers.ContainsKey(pm))
        {
            // 원래 속도로 복구
            if (pm != null)
            {
                pm.moveSpeed = affectedPlayers[pm];
            }
            affectedPlayers.Remove(pm);
        }
    }

    // 오브젝트가 예기치 못하게 파괴될 때를 대비한 예외 처리
    private void OnDestroy()
    {
        foreach (var kvp in affectedPlayers)
        {
            if (kvp.Key != null) kvp.Key.moveSpeed = kvp.Value;
        }
    }
}