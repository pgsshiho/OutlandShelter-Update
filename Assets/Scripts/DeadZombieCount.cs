using UnityEngine;
using TMPro;

public class DeadZombieCount : MonoBehaviour
{
    private TextMeshProUGUI countText;

    private void Awake()
    {
        countText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        // "DeadZombieCountFormat" 키를 찾아서 {0} 자리에 deathCount를 넣습니다.
        // 확장 메서드 .Localize()를 사용합니다.
        countText.text = "DeadZombieCountFormat".Localize("En", BasicZombie.deathCount);
    }
}