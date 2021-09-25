﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public GameManager gameManager;
    public Animator JR_monitorAnimator;
    public TypingEffect textBoxTE;
    //============================
    public Text Btn1Txt;
    public Text Btn2Txt;
    public Text Btn3Txt;
    public Text Btn4Txt;
    public Text SelectBoxTxt;
    //---------------------------
    public bool JR_newMail;
    public bool D_newMail;
    //============================

    private void Start() {
        gameManager.selectIndex = -1;
        D_newMail = false;
    }

    public void PlayTalk(int _talkNum, int _sceneIndex, string _allignment = "UL")
    {
        gameManager.currSceneIndex = _sceneIndex;
        gameManager.currCutIndex = _talkNum;
        textBoxTE.allignment = _allignment;
        gameManager.Action();
    }

    public void PlaySystemTalk(int _talkNum, int _sceneIndex)
    {
        gameManager.currSceneIndex = _sceneIndex;
        gameManager.currCutIndex = _talkNum;
        gameManager.SystemAction();
    }

    public void PlayObj(int _obj, int _Index, string _allignment = "UL")
    {
        gameManager.objIndex = _Index;
        gameManager.objCutIndex = _obj;
        textBoxTE.allignment = _allignment;
        gameManager.InterAction();
    }

    public GameObject PlayNPC(string _who, Vector3 _pos)
    {
        return gameManager.SpawnNPC(_who, _pos);
    }

    public GameObject spawnNPC(string _who, Vector3 _pos)
    {
        return gameManager.SpawnRandomNPC(_who, _pos);
    }

    public void SetSelectBtnTxt(string _btn1, string _btn2, string _btn3, string _btn4)
    {
        Btn1Txt.text = _btn1;
        Btn2Txt.text = _btn2;
        Btn3Txt.text = _btn3;
        Btn4Txt.text = _btn4;
        gameManager.selectBox.SetActive(true);
    }    

    public void SetSelectTxt(string _txt = "")
    {
        SelectBoxTxt.text = _txt;
    }
}