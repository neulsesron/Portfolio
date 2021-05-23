﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    [Header("Stage")]
    public int stage;
    bool isEnd;


    [Header("UI")]
    public Text scoreTxt;
    public Text highScoreTxt;
    public Text endScoreTxt;
    public Text over_highScoreTxt;
    public Text over_endScoreTxt;
    public GameObject gameOver;
    public GameObject gameClear;
    public GameObject gameClear2;
    public Image[] lifeImg;
    public Image[] boomImg;
    public Animator stageAnim;
    public Animator fadeAnim;


    [Header("매니저")]
    public GameObject player;
    Player playerLogic;
    public ObjectManager objectManager;
    LevelManager levelManager;


    [Header("Enemy")]
    string[] enemyObjs;
    public Transform[] spawnPoints;
    public float nextSpawnDelay;
    public float curSpawnDelay;
    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;
    //=====================

    private void Awake() {
        playerLogic = player.GetComponent<Player>();
        enemyObjs = new string[] { "EnemyL", "EnemyM", "EnemyS", "Boss", "Star" };
        spawnList = new List<Spawn>();
    }

    private void Start() {
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        stage = levelManager.stageNum;
        StageStart();
    }
    
    void Update() 
    {
        if (!isEnd) {
            curSpawnDelay += Time.deltaTime;

            if(curSpawnDelay > nextSpawnDelay && !spawnEnd) {
                SpawnEnemy();
                curSpawnDelay = 0;
            }

            // score
            scoreTxt.text = string.Format("{0:n0}", playerLogic.score);
        }
    }


    public void StageStart()
    {
        // fain in
        fadeAnim.SetTrigger("In");

        if (stage != 99) {
            // ui
            stageAnim.SetTrigger("On");
            stageAnim.GetComponent<Text>().text = "Stage " + stage;
        }

        // enemy spawn
        ReadSpawnFile();
    }

    public void StageEnd()
    {
        // fade out
        fadeAnim.SetTrigger("Out");

        // player reposition
        player.transform.position = new Vector3(0, -3.5f, 0);
        
        GameOver();
    }

    void ReadSpawnFile()
    {
        // 초기화
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        // 읽기
        TextAsset textFile = Resources.Load("stage" + stage) as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        while (stringReader != null) {
            string line = stringReader.ReadLine();

            if (line == null)
                break;

            // 생성
            Spawn spawnData = new Spawn();
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);

            spawnList.Add(spawnData);
        }

        stringReader.Close();

        nextSpawnDelay = spawnList[0].delay;
    }

    void SpawnEnemy()
    {
        int enemyIndex = 0;
        switch (spawnList[spawnIndex].type) {
            case "L":
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "S":
                enemyIndex = 2;
                break;
            case "B":
                enemyIndex = 3;
                break;
            case "T":
                enemyIndex = 4;
                break;
        }

        int enemyPoint = spawnList[spawnIndex].point;
        if (enemyPoint == 9) // 랜덤 위치
            enemyPoint = Random.Range(0, 5);


        GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();

        enemy.transform.position = spawnPoints[enemyPoint].position;
        

        if (enemyIndex == 3) { // 보스
            Boss bossLogic = enemy.GetComponent<Boss>();
            bossLogic.player = player;
            bossLogic.objectManager = objectManager;
            bossLogic.gameManager = this;

            rigid.velocity = new Vector2(0, bossLogic.speed * (-1));
        } else if (enemyIndex == 4) { // 별
            rigid.velocity = new Vector2(0, -2f);
        } else { // enemy
            Enemy enemyLogic = enemy.GetComponent<Enemy>();
            enemyLogic.player = player;
            enemyLogic.objectManager = objectManager;
            enemyLogic.gameManager = this;

            if (enemyPoint == 5) { // 왼쪽->오른쪽
                rigid.velocity = new Vector2(enemyLogic.speed, -1);
            } else if (enemyPoint == 6) { // 오른쪽->왼쪽
                rigid.velocity = new Vector2(enemyLogic.speed * (-1), -1);
            } else // 아래로
                rigid.velocity = new Vector2(0, enemyLogic.speed * (-1));
        }

        // 다음 스폰
        spawnIndex++;
        if (spawnIndex == spawnList.Count) {
            spawnEnd = true;
            return;
        }

        nextSpawnDelay = spawnList[spawnIndex].delay;
    }

    // ** UI **
    public void UpdateLife(int life)
    {
        for (int i = 0; i < 3; i++) {
            lifeImg[i].color = new Color(1, 1, 1, 0);
        }

        for (int i = 0; i < life; i++) {
            lifeImg[i].color = new Color(1, 1, 1, 1);
        }
    }

    public void UpdateBoom(int boom)
    {
        for (int i = 0; i < 2; i++) {
            boomImg[i].color = new Color(1, 1, 1, 0);
        }

        for (int i = 0; i < boom; i++) {
            boomImg[i].color = new Color(1, 1, 1, 1);
        }
    }

    // ** player Respawn **
    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerExe", 0.2f);
    }
    
    void RespawnPlayerExe()
    {
        player.transform.position = new Vector3(0, -3.5f, 0);
        player.SetActive(true);

        playerLogic.isHit = false;
    }

    public void CallExplosion(Vector3 pos, string type)
    {
        GameObject explosion = objectManager.MakeObj("Explosion");
        Explosion explosionLogic = explosion.GetComponent<Explosion>();

        explosion.transform.position = pos;
        explosionLogic.StartExplosion(type);
    }

    // ** game control **
    public void GameOver()
    {
        if (isEnd)
            return;

        isEnd = true;

        if (stage == 99) {
            if (playerLogic.score > levelManager.unlimitScore)
                levelManager.unlimitScore = playerLogic.score;

            over_highScoreTxt.text = "High Score : " + levelManager.unlimitScore;
        } else {
            // 별 개수 업데이트
            if (playerLogic.currStarNum > levelManager.starCntArr[stage - 1])
                levelManager.starCntArr[stage - 1] = playerLogic.currStarNum;

            // 하이스코어 업데이트
            if (playerLogic.score > levelManager.highScoreArr[stage - 1])
                levelManager.highScoreArr[stage - 1] = playerLogic.score;

            over_highScoreTxt.text = "High Score : " + levelManager.highScoreArr[stage - 1];
        }
            
        over_endScoreTxt.text = "Score : " + playerLogic.score;

        gameOver.SetActive(true);
    }

    public void GameClear()
    {
        if (isEnd)
            return;

        isEnd = true;
        
        // 별 개수 업데이트
        if (playerLogic.currStarNum > levelManager.starCntArr[stage - 1])
            levelManager.starCntArr[stage - 1] = playerLogic.currStarNum;
        
        if (levelManager.starCntArr[stage - 1] == 3)
            levelManager.allClearArr[stage - 1] = true;
        
        // 하이스코어 업데이트
        if (playerLogic.score > levelManager.highScoreArr[stage - 1])
            levelManager.highScoreArr[stage - 1] = playerLogic.score;

        highScoreTxt.text = "High Score : " + levelManager.highScoreArr[stage - 1];
        endScoreTxt.text = "Score : " + playerLogic.score;

        gameClear.SetActive(true);
    }

    // 무한 모드 일때 엔딩 화면
    public void GameClear2()
    {
        if (isEnd)
            return;

        isEnd = true;

        if (playerLogic.score > levelManager.unlimitScore)
            levelManager.unlimitScore = playerLogic.score;

        gameClear2.transform.Find("highScoreTxt").GetComponent<Text>().text = "High Score : " + levelManager.unlimitScore;
        gameClear2.transform.Find("endScoreTxt").GetComponent<Text>().text = "Score : " + playerLogic.score;

        gameClear2.SetActive(true);
    }

    public void GameRetry()
    {
        isEnd = false;

        SceneManager.LoadScene("LevelScene");
    }

    public void BackToSelect()
    {
        isEnd = false;

        if (stage == 99)
            SceneManager.LoadScene(0);
        else
            SceneManager.LoadScene(2);
    }

    public void NextStage()
    {
        levelManager.stageNum++;

        SceneManager.LoadScene(1);
    }

    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void Continue()
    {
        Time.timeScale = 1;
    }
}
