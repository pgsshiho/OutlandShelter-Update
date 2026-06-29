using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial instance;
    public bool isTutorial = true;
    public int getReosurceCount = 0;
    public GameObject[] tutorialpanel;
    public int[] buildings = {0,0,0};
    public int nowpage = 0;
    void Start()
    {
        
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this; // 자기 자신을 인스턴스에 할당
            // 만약 씬이 바뀌어도 튜토리얼을 유지하고 싶다면 아래 주석을 해제하세요.
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // 혹시라도 씬에 Tutorial 오브젝트가 중복으로 존재하면 파괴해서 하나만 유지
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(nowpage == 1 || nowpage == 2)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                nextpage();
            }
        }
        if(getReosurceCount == 2)
        {
            getReosurceCount = 1231232;
            nextpage();
        }
        if(nowpage == 6 && buildings[0] > 0 && buildings[1] > 0 && buildings[2] > 0)
        {
            nextpage();
        }
        if(nowpage == 10 && Input.GetKeyDown(KeyCode.Alpha3))
        {
            nextpage();
        }
        if(nowpage == 11 && Input.GetKeyDown(KeyCode.R))
        {
            nextpage();
        }
        if(nowpage == 14 && Input.GetKeyDown(KeyCode.K))
        {
            nextpage();
        }
        if(nowpage == 15 && Input.GetKeyDown(KeyCode.G))
        {
            nextpage();
        }
        if(nowpage == 16 && Input.GetKeyDown(KeyCode.Space))
        {
            nextpage();
        }
    }
    public void nextpage()
    {
        Debug.Log(nowpage);
        tutorialpanel[nowpage].SetActive(false);
        nowpage++;
        if (nowpage >= tutorialpanel.Length)
        {
            nowpage = 0;
            this.gameObject.SetActive(false);
            SceneChanger.BG("Mainmenu");
        }
        else
        {
            tutorialpanel[nowpage].SetActive(true);
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tuto"))
        {
            Debug.Log("Player Trigger Entered");
             Destroy(this.GetComponent<SpriteRenderer>());
             Destroy(this.gameObject.GetComponent<Collider2D>());
            nextpage();
        }
    }
}
