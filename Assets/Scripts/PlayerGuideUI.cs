using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerGuideUI : MonoBehaviour
{
    public float range;
    public bool isOpened = false;
    public static Dictionary<Guide, Guide> guides = new();
    [HideInInspector] public List<Guide> selectGuide = new();

    private void Awake()
    {
        guides.Clear();
    }

    private void Update()
    {
        if (!isOpened)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

            if (hits.Length > 0)
            {
                List<Guide> filter = new();

                foreach (Collider2D hit in hits)
                {
                    if (hit.TryGetComponent(out Guide guide))
                    {
                        filter.Add(guide);
                    }
                }

                for (int i = 0; i < filter.Count; i++)
                {
                    Guide[] temp = filter[i].GetComponents<Guide>();

                    for (int j = 0; j < temp.Length; j++)
                    {
                        if (!filter.Contains(temp[j]))
                        {
                            filter.Add(temp[j]);
                            guides[temp[j]] = temp[j];
                        }
                    }
                }

                foreach (var guide in guides.Values)
                {
                    if (!filter.Contains(guide))
                    {
                        guide.GuideOnOff(null, false);
                    }
                }

                if (filter.Count != 0)
                {
                    float min = Vector2.Distance(filter[0].transform.position, transform.position);
                    int index = 0;
                    if (filter.Count > 1)
                    {
                        for (int i = 1; i < filter.Count; i++)
                        {
                            float temp = Vector2.Distance(filter[i].transform.position, transform.position);
                            if (temp < min)
                            {
                                min = temp;
                                index = i;
                            }
                        }
                    }
                    selectGuide.Clear();
                    for (int i = 0; i < filter.Count; i++)
                    {
                        guides[filter[i]] = filter[i];

                        if (filter[i].enabled == true)
                        {
                            if (filter[i].transform != filter[index].transform)
                            {
                                filter[i].GuideOnOff(null, false);
                            }
                            else
                            {
                                filter[i].GuideOnOff(this, true);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var guide in guides.Values)
                {
                    guide.GuideOnOff(null, false);
                }
            }
        }
        else
        {
            foreach (var guide in guides.Values)
            {
                guide.GuideOnOff(selectGuide.Contains(guide) && guide.enabled ? this : null, false);
            }
        }
    }
}
