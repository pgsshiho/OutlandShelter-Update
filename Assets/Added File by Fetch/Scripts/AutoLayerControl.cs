using UnityEngine;

public class AutoLayerControl : MonoBehaviour
{
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / 1000f);
    }
}
