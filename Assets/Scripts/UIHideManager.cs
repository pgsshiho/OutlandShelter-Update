using UnityEngine;

public class UIHideManager : MonoBehaviour
{
    [SerializeField] private GameObject ui;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            ui.SetActive(!ui.activeSelf);
        }
    }
}
