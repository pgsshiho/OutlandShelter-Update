using UnityEngine;

public class ShoesClose : MonoBehaviour
{
    public GameObject shoePanel;
    private void Start()
    {
        shoePanel.SetActive(false);
    }
    public void CloseShoePanel()
    {
        shoePanel.SetActive(true);
    }
}
