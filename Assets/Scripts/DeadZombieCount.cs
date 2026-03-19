using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeadZombieCount : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<TextMeshProUGUI>().text = $"처리한 좀비의 수 : {BasicZombie.deathCount}";
    }
}
