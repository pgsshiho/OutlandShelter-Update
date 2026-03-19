using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
{
    [SerializeField, TextArea] private string toolTipText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Notion.ToolTip(toolTipText, true);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        Notion.ToolTip(toolTipText, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Notion.ToolTip(toolTipText, false);
    }

    private void OnDisable()
    {
        Notion.ToolTip(toolTipText, false);
    }
}
