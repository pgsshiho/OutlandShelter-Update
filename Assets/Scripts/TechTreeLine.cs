using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechTreeLine : MonoBehaviour
{
    private TechTreeUnlock[] techTrees;

    private int treeLevel = 0;

    private void Start()
    {
        techTrees = new TechTreeUnlock[transform.childCount];

        for (int i = 0; i < techTrees.Length; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out techTrees[i]))
            {
                if (i != treeLevel)
                {
                    techTrees[i].blocker.gameObject.SetActive(true);
                    techTrees[i].blocker.raycastTarget = true;
                    techTrees[i].myText.text = "Àá±è";
                    techTrees[i].enabled = false;
                }
            }
        }
    }

    private void Update()
    {
        for (int i = treeLevel; i < techTrees.Length; i++)
        {
            if (i != treeLevel)
            {
                if (techTrees[i].enabled)
                {
                    techTrees[i].blocker.gameObject.SetActive(true);
                    techTrees[i].blocker.raycastTarget = true;
                    techTrees[i].myText.text = "Àá±è";
                    techTrees[i].enabled = false;
                }
            }
            else
            {
                if (!techTrees[i].enabled)
                {
                    techTrees[i].blocker.raycastTarget = false;
                    techTrees[i].blocker.gameObject.SetActive(false);
                    techTrees[i].myText.text = techTrees[i].originalText;
                    techTrees[i].enabled = true;
                }
            }
        }

        if (techTrees[treeLevel].isUnlocked) treeLevel++;
    }
}
