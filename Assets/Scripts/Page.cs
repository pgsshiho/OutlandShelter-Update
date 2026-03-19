using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour
{
    [SerializeField] private GameObject[] pages;

    public void PageChange(GameObject go)
    {
        foreach (GameObject page in pages)
        {
            page.SetActive(page == go);
        }
    }
}
