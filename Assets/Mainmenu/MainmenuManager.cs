using UnityEngine;

public class MainmenuManager : MonoBehaviour
{
    public static System.Action OnAnyButtonClicked;
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject multiChooseMenuPanel;
    [SerializeField] GameObject characterChoosePanel;
    public static bool isMan;
    [SerializeField] GameObject mapChoosePanel;
    [SerializeField] GameObject settingPanel;
    [SerializeField] GameObject credits;
    [SerializeField] GameObject page1;
    [SerializeField] GameObject page2;
    [SerializeField] GameObject page3;
    [SerializeField] GameObject page4;
    [SerializeField] GameObject page5;
    [SerializeField] GameObject page6;
    [SerializeField] GameObject page7;
    [SerializeField] GameObject manual;

    public void Update()
    {
        if (credits == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape)){ 
                 credits.SetActive(false);
            } 
        }
        if (settingPanel == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) { settingPanel.SetActive(false); mainMenuPanel.SetActive(true); }
        }
    }

    public void Starts()
    {
        OnAnyButtonClicked?.Invoke();
        mainMenuPanel.SetActive(false);
        multiChooseMenuPanel.SetActive(true);
    }

    public void Manual()
    {
        OnAnyButtonClicked?.Invoke();
        manual.SetActive(true);
    }

    public void Credit()
    {
        OnAnyButtonClicked?.Invoke();
        credits.SetActive(true);
    }

    public void QuitTheGame()
    {
        Application.Quit();
    }

    public void Setting()
    {
        OnAnyButtonClicked?.Invoke();
        settingPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void Multi()
    {
        OnAnyButtonClicked?.Invoke();

        Notion.Log("업데이트 예정입니다");
    }

    public void Solo()
    {
        OnAnyButtonClicked?.Invoke();
        multiChooseMenuPanel.SetActive(false);
        characterChoosePanel.SetActive(true);
    }

    public void GoBack()
    {
        OnAnyButtonClicked?.Invoke();
        mainMenuPanel.SetActive(this);
        settingPanel.SetActive(false);
        mapChoosePanel.SetActive(false);
        characterChoosePanel.SetActive(false);
        multiChooseMenuPanel.SetActive(false);
        credits.SetActive(false);
        page1.SetActive(false);
        page2.SetActive(false);
        page3.SetActive(false);
        page4.SetActive(false);
        page5.SetActive(false);
        page6.SetActive(false);
        page7.SetActive(false);
        manual.SetActive(false);
    }

    public void Man()
    {
        OnAnyButtonClicked?.Invoke();
        characterChoosePanel.SetActive(false);
        isMan = true;
        mapChoosePanel.SetActive(true);
    }

    public void Girl()
    {
        OnAnyButtonClicked?.Invoke();
        characterChoosePanel.SetActive(false);
        isMan = false;
        mapChoosePanel.SetActive(true);
    }

    public void GoMapA()
    {
        OnAnyButtonClicked?.Invoke();
        SceneChanger.BG("MapA");
    }

    public void GoMapB()
    {
        OnAnyButtonClicked?.Invoke();
        SceneChanger.BG("MapB");
    }

    public void GoMapC()
    {
        OnAnyButtonClicked?.Invoke();
        SceneChanger.BG("MapC");
    }

    public void Page1()
    {
        page1.SetActive(true);
        page2.SetActive(false);
    }

    public void Page2()
    {
        page2.SetActive(true);
        page3.SetActive(false);
        page1.SetActive(false);
    }

    public void Page3()
    {
        page3.SetActive(true);
        page2.SetActive(false);
        page4.SetActive(false);
    }
    public void Page4()
    {
        page4.SetActive(true);
        page3.SetActive(false);
        page5.SetActive(false);
    }
    public void Page5()
    {
        page5.SetActive(true);
        page4.SetActive(false);
        page6.SetActive(false);
    }
    public void Page6()
    {
        page6.SetActive(true);
        page5.SetActive(false);
        page7.SetActive(false);
    }
    public void Page7()
    {
        page7.SetActive(true);
        page6.SetActive(false);
    }
}
