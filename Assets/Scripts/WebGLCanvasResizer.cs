using UnityEngine;

public class WebGLCanvasResizer : MonoBehaviour
{
    void Start()
    {
        // 시작할 때 해상도 동기화
        UpdateResolution();
    }

    void Update()
    {
        // 브라우저 창 크기가 변경되는 것을 감지하기 위해 프레임마다 체크 (또는 주기적 체크)
        if (Screen.width != LastWidth || Screen.height != LastHeight)
        {
            UpdateResolution();
        }
    }

    private int LastWidth;
    private int LastHeight;

    void UpdateResolution()
    {
        LastWidth = Screen.width;
        LastHeight = Screen.height;

        // 현재 브라우저 창 크기에 맞게 Screen 해상도 재설정
        Screen.SetResolution(LastWidth, LastHeight, false);
    }
}