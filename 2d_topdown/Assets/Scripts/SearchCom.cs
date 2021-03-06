using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SearchCom : MonoBehaviour
{
    public GameObject SearchComUI;
    public GameObject inventory;
    public GameObject tutorialSet;
    public GameObject thirdFmark;
    //------------------------------------------------------
    public int currentScreen;      // 0 : login, 1 : main, 2 : searcherList, 3 : comaList, 4 : comaDetail, 5 : Watch
    //=====================================================
    // ** 로그인 **
    public GameObject Login;
    public bool loginEnd = false;

    //=====================================================
    // ** 메인 화면 **
    public GameObject Main;
    //------------------------------------------------------
    public void BtnSearcher()
    {
        if (!SwitchManager.Instance.switchdata["Tutorial_SearchCom"].off)
            return;
        currentScreen = 2;
        Main.SetActive(false);
        SearcherList.SetActive(true);
        backBtn.GetComponent<Button>().interactable = true;
    }

    public void BtnComa()
    {
        if (!SwitchManager.Instance.switchdata["Tutorial_SearchCom"].off)
            return;
        currentScreen = 3;
        Main.SetActive(false);
        Coma.SetActive(true);
        backBtn.GetComponent<Button>().interactable = true;
    }

    public void BtnWatch()
    {
        if (!SwitchManager.Instance.switchdata["Tutorial_SearchCom"].off)
            return;
        currentScreen = 5;
        Main.SetActive(false);
        Watch.SetActive(true);
        backBtn.GetComponent<Button>().interactable = true;
    }

    //=====================================================

    public GameObject SearcherList;
    public GameObject ListHS;

    //=====================================================
    public ComaData comaData;
    public GameObject Coma;
    public GameObject DetailComa;
    public Text nameTxt;
    public Text birthTxt;
    public Text districtTxt;
    public Text responseTxt;
    public Text searcherTxt;
    public Text noteTxt;
    string currClickedID;
    //------------------------------------------------------
    public void ClickComa()
    {
        GameObject btn = EventSystem.current.currentSelectedGameObject;

        comaDataForm cdf = comaData.comaID(btn.name);
        nameTxt.text = "이름 : " + cdf.name;
        birthTxt.text = "생년월일 : " + cdf.birth;
        districtTxt.text = "구역 : " + cdf.dis;
        responseTxt.text = "담당자 : " + cdf.response;
        searcherTxt.text = "수색자 : " + cdf.search;
        noteTxt.text = "가장 기억에 남는 물건/장소/사건 : " + cdf.note;

        if (btn.name == "456871" && SwitchManager.Instance.switchdata["A1Maze_InitTalk"].off) {
            searcherTxt.text = "수색자 : 서재하(수색)";
        } else
            searcherTxt.text = "수색자 : " + cdf.search;

        currentScreen = 4;
        currClickedID = btn.name;
        Coma.SetActive(false);
        DetailComa.SetActive(true);
        requestBtn.GetComponent<Button>().interactable = true;
    }

    //=====================================================
    public GameObject Watch;
    //------------------------------------------------------
    public void ClickWatch()
    {
        GameObject btn = EventSystem.current.currentSelectedGameObject;

        if (btn.name == "740682") {     // 김채은(송지운)
            SceneManager.Instance.PlaySystemTalk(20, 400);
        } else if (btn.name == "521791") {      // 박형서(강선우)
            GameManager.Instance.dontSave = true;

            //안내멘트

            StartCoroutine(FadeAndChangeMap2());
            
        }
        
        Watch.SetActive(false);
        SearchComUI.SetActive(false);
        currentScreen = 0;
        SwitchManager.Instance.ing = false;

    }

    IEnumerator FadeAndChangeMap2()
    {
        GameManager.Instance.currTime = 8;
        GameManager.Instance.fadeEffect.nameStr = "강선우";
        GameManager.Instance.fadeEffect.timeStr = "2030-03-06(수) 오후  3:10:00";
        GameManager.Instance.fadeEffect.OnFade(FadeState.FadeOut);
        yield return new WaitUntil(() => GameManager.Instance.fadeEffect.endFade == true);
        //=====================================================
        GameManager.Instance.OffPlayer(GameManager.Instance.player);
        GameManager.Instance.ChangeMap(8, "P_Sunwoo", new Vector3(2, 1, 0), "up", 4);
    }
    //=====================================================
    public GameObject ChooseBox;
    GameObject requestBtn;
    GameObject backBtn;
    //------------------------------------------------------
    public void BtnInventory()
    {
        inventory.SetActive(true);
    }

    public void BtnEnd()
    {
        SearchComUI.SetActive(false);
        SwitchManager.Instance.ing = false;

        switch(currentScreen) {
            case 1:
                Main.SetActive(false);
                break;
            case 2:
                SearcherList.SetActive(false);
                break;
            case 3:
                Coma.SetActive(false);
                break;
            case 4:
                DetailComa.SetActive(false);
                break;
            case 5:
                Watch.SetActive(false);
                break;
        }
        
        backBtn.GetComponent<Button>().interactable = false;
    }

    public void BtnBack()
    {
        switch(currentScreen) {
            case 2:
                backBtn.GetComponent<Button>().interactable = false;
                SearcherList.SetActive(false);
                Main.SetActive(true);
                currentScreen = 1;
                break;
            case 3:
                backBtn.GetComponent<Button>().interactable = false;
                Coma.SetActive(false);
                Main.SetActive(true);
                currentScreen = 1;
                break;
            case 4:
                DetailComa.SetActive(false);
                Coma.SetActive(true);
                currentScreen = 3;
                requestBtn.GetComponent<Button>().interactable = false;
                break;
            case 5:
                backBtn.GetComponent<Button>().interactable = false;
                Watch.SetActive(false);
                Main.SetActive(true);
                currentScreen = 1;
                break;
        }
    }

    public void BtnRequest()
    {
        if (currClickedID == "456871") {
            BtnEnd();

            StartCoroutine(FadeAndChangeMap());

            SwitchManager.Instance.switchdata["A1Maze_InitTalk"].on = true;
            SwitchManager.Instance.switchdata["Searchcom_signupJihyeon"].on = true;
        } else {
            GameManager.Instance.currSceneIndex = 400;
            GameManager.Instance.currCutIndex = 0;
            GameManager.Instance.SystemAction();
        }
    }

    IEnumerator FadeAndChangeMap()
    {
        GameManager.Instance.currTime = 1;
        GameManager.Instance.fadeEffect.nameStr = "서재하";
        GameManager.Instance.fadeEffect.timeStr = "2030-03-06(수)\n오후  3:41:00";
        GameManager.Instance.fadeEffect.OnFade(FadeState.FadeOut);
        yield return new WaitUntil(() => GameManager.Instance.fadeEffect.endFade == true);

        GameManager.Instance.OffPlayer(GameManager.Instance.player);
        GameManager.Instance.ChangeMap(4, "P_Jaeha", new Vector3(27.5f, -2, 0), "up", 3);
    }

    //=====================================================

    void OnEnable() {
        requestBtn = ChooseBox.transform.Find("Btn1").gameObject;
        requestBtn.GetComponent<Button>().interactable = false;
        backBtn = ChooseBox.transform.Find("Btn3").gameObject;
        backBtn.GetComponent<Button>().interactable = false;
    }

    void Update() {
        if (SwitchManager.Instance.switchdata["SecondF_movingSM"].off) {
            ListHS.transform.Find("state").gameObject.GetComponent<Text>().text = "업무중";
        }

        if (SwitchManager.Instance.switchdata["ThirdF_SearchCom"].on) {
            SwitchManager.Instance.switchdata["ThirdF_SearchCom"].on = false;

            SwitchManager.Instance.switchdata["Tutorial_OffMarkSearchCom"].on = true;

            SearchComUI.SetActive(true);
            Login.SetActive(true);
            SwitchManager.Instance.ing = true;
        }
        
        if (SwitchManager.Instance.switchdata["Tutorial_SearchCom"].on && !Login.activeSelf) {
            SwitchManager.Instance.switchdata["Tutorial_SearchCom"].on = false;
            StartCoroutine(Tutorial());
        }

        if (SwitchManager.Instance.ing) {
            if (loginEnd) {
                currentScreen = 1;
                Main.SetActive(true);
                loginEnd = false;
            }
        }

        IEnumerator Tutorial()
        {
            Debug.Log("???");
            //=====================================================
            yield return new WaitForSeconds(0.5f);
            //-----------------------------------------------------
            tutorialSet.transform.Find("mark").gameObject.SetActive(true);
            SceneManager.Instance.PlaySystemTalk(30, 600);
            yield return new WaitUntil(() => GameManager.Instance.isSystem == false);
            //-----------------------------------------------------
            tutorialSet.transform.Find("mark").gameObject.transform.position = new Vector3(Main.transform.Find("Btn_Coma").gameObject.transform.position.x, tutorialSet.transform.Find("mark").gameObject.transform.position.y, 0);
            SceneManager.Instance.PlaySystemTalk(40, 600);
            yield return new WaitUntil(() => GameManager.Instance.isSystem == false);
            //-----------------------------------------------------
            tutorialSet.transform.Find("mark").gameObject.transform.position = new Vector3(Main.transform.Find("Btn_Watch").gameObject.transform.position.x, Main.transform.Find("Btn_Watch").gameObject.transform.position.y, 0) + new Vector3(0, 1, 0);
            SceneManager.Instance.PlaySystemTalk(50, 600);
            yield return new WaitUntil(() => GameManager.Instance.isSystem == false);
            //=====================================================
            SwitchManager.Instance.switchdata["Tutorial_SearchCom"].off = true;
            tutorialSet.transform.Find("mark").gameObject.SetActive(false);
        }
    }
}
