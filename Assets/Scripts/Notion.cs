using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Notion : MonoBehaviour
{
    private static Notion instance;
    [SerializeField] private Image notionPanel;
    [SerializeField] private TextMeshProUGUI notionText;
    [SerializeField] private Image warningPanel;
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private Image toolTipPanel;
    [SerializeField] private TextMeshProUGUI toolTipText;
    [SerializeField] private float notionDuration = 1f;
    [SerializeField] private float warningDuration = 1f;
    [SerializeField] private float fadeDuration = 0.5f;
    private Sequence notionSequence;
    private Sequence warningSequence;
    private Vector2 canvasSize;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        canvasSize = GetComponent<CanvasScaler>().referenceResolution;
        
        Application.targetFrameRate = 240;
        DontDestroyOnLoad(gameObject);
        instance = this;

        void SequenceInit(ref Sequence sequence, Image panel, TextMeshProUGUI tmp, float interval)
        {
            sequence = DOTween.Sequence();
            sequence.Append(panel.transform.DOScale(1.2f, 0.25f).From(Vector3.one * 0.5f))
                .Append(panel.transform.DOScale(1f, 0.25f))
                .AppendInterval(interval)
                .Append(panel.DOFade(0, fadeDuration).From(panel.color.a))
                .Join(tmp.DOFade(0, fadeDuration).From(tmp.color.a).OnComplete(() => panel.gameObject.SetActive(false)))
                .SetAutoKill(false)
                .Pause();
        }

        SequenceInit(ref notionSequence, notionPanel, notionText, notionDuration);
        SequenceInit(ref warningSequence, warningPanel, warningText, warningDuration);
    }

    public static void Log(string text)
    {
        instance.notionText.text = text;
        instance.notionPanel.gameObject.SetActive(true);

        instance.notionSequence.Restart();
    }

    public static void Warning(string text)
    {
        instance.warningText.text = text;
        instance.warningPanel.gameObject.SetActive(true);

        instance.warningSequence.Restart();
    }

    public static void ToolTip(string text, bool value)
    {
        if (!value)
        {
            instance.toolTipPanel.gameObject.SetActive(false);
            return;
        }

        instance.toolTipPanel.gameObject.SetActive(true);
        Vector2 pos = Input.mousePosition;
        float centerY = Screen.height / 2f;
        float panelOffset = centerY / instance.canvasSize.y;

        instance.toolTipText.text = text;
        instance.toolTipPanel.transform.position = pos + new Vector2(0, instance.toolTipPanel.rectTransform.sizeDelta.y * panelOffset)
            * (pos.y > centerY ? -1 : 1);
    }
}
