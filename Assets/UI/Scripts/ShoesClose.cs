using UnityEngine;

public class ShoesClose : MonoBehaviour
{
    public GameObject shoePanel;

    private void Awake()
    {
        shoePanel.SetActive(false);
        WorkingManager.OnShoesCrafted += CloseShoe;
    }

    private void CloseShoe(int x) => shoePanel.SetActive(true);

    private void OnDestroy()
    {
        WorkingManager.OnShoesCrafted -= CloseShoe;
    }
}
