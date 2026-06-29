using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackOpen : UIOpen
{
    public override void Update()
    {
        base.Update();
        if (Tutorial.instance != null && Tutorial.instance.isTutorial && ui.activeSelf && Input.GetKeyUp(keyCode) && Tutorial.instance.nowpage == 4)
        {
            Tutorial.instance.nextpage();
        }
    }
}
