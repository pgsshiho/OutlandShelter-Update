using UnityEngine;

public class PoisonStatus : MonoBehaviour
{
    private bool isPoisoned = false;
    private float poisonTimer = 0f;
    private IDamageable playerInterface;
    private Personal_resource playerRes; // 체력 확인용

    void Start()
    {
        playerInterface = GetComponent<IDamageable>();
        playerRes = GetComponent<Personal_resource>();
    }

    public void ApplyPoison(bool isStrong)
    {
        isPoisoned = true;
        poisonTimer = 10f;
        Notion.Log("중독 상태에 빠졌습니다!");
    }

    void Update()
    {
        if (!isPoisoned) return;

        // 1초마다 최대 체력의 1% 감소
        poisonTimer -= Time.deltaTime;
        playerInterface.Damage(Personal_resource.MaxHP * 0.01f * Time.deltaTime);

        // 조건: 체력 최대치 회복 시 해제
        if (Personal_resource.NowHP >= Personal_resource.MaxHP)
        {
            isPoisoned = false;
            Notion.Log("중독이 정화되었습니다.");
        }

        if (poisonTimer <= 0) isPoisoned = false;
    }
}