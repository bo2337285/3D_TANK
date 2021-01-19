using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GamePlayState {
    GameStart,
    GameOver,
    GameMissionStart,
    GameMissionStay,
    GameMissionEnd,
}
public class GamePlayManager : MonoBehaviour {
    [HideInInspector] public static GamePlayManager Instance;
    [SerializeField] int gameLevel = 0, gameMaxLevel = 0;
    [SerializeField] GamePlayState currState;
    [SerializeField] private bool isGameStart, isGameMissionInited, isGameMissionEnd, isGameOver,
    isRestart, isGameMissionConfirmed, isGoGameStart;
    private void Awake () {
        Instance = this;
    }
    public void GameStart () {
        isGameStart = true;
    }
    public void CompleteLevel () {
        isGameMissionEnd = true;
    }
    public void CompleteMissionConfirmed () {
        isGameMissionConfirmed = true;
    }
    public void Init (int startLevel, int maxLevel) {
        gameLevel = startLevel;
        gameMaxLevel = maxLevel;
        InitFSM ();
    }
    private void InitFSM () {
        FSMControler controler = FSMManager.Instance.CreateFSMControler (gameObject.GetInstanceID ().ToString (), gameObject);
        #region 创建状态
        FSMState stateGameStart = new FSMState (GamePlayState.GameStart.ToString (), BeforeGameStart, OnGameStart, AfterGameStart);
        FSMState stateGameOver = new FSMState (GamePlayState.GameOver.ToString (), BeforeGameOver, OnGameOver, AfterGameOver);
        FSMState stateGameMissionStart = new FSMState (GamePlayState.GameMissionStart.ToString (), BeforeGameMissionStart, OnGameMissionStart, AfterGameMissionStart);
        FSMState stateGameMissionStay = new FSMState (GamePlayState.GameMissionStay.ToString (), BeforeGameMissionStay, OnGameMissionStay, AfterGameMissionStay);
        FSMState stateGameMissionEnd = new FSMState (GamePlayState.GameMissionEnd.ToString (), BeforeGameMissionEnd, OnGameMissionEnd, AfterGameMissionEnd);
        #endregion
        #region  建立状态切换条件
        #region stateGameStart
        // 开始游戏,进入第一关
        stateGameStart.RegisterChangeEvt (stateGameMissionStart, (object[] o) => {
            return isGameStart;
        }, gameObject);
        #endregion
        #region stateGameMissionStart
        // 关卡进行中
        stateGameMissionStart.RegisterChangeEvt (stateGameMissionStay, (object[] o) => {
            return isGameMissionInited;
        }, gameObject);
        #endregion
        #region stateGameMissionStay
        // 满足过关条件
        stateGameMissionStay.RegisterChangeEvt (stateGameMissionEnd, (object[] o) => {
            return isGameMissionEnd;
        }, gameObject);
        // 战败
        stateGameMissionStay.RegisterChangeEvt (stateGameOver, (object[] o) => {
            return isGameOver;
        }, gameObject);
        #endregion
        #region stateGameMissionEnd
        // 通关
        stateGameMissionEnd.RegisterChangeEvt (stateGameOver, (object[] o) => {
            return isGameOver;
        }, gameObject);
        // 去下一关
        stateGameMissionEnd.RegisterChangeEvt (stateGameMissionStart, (object[] o) => {
            // 未满足通关条件,切用户确认去下一关
            return !isGameOver && isGameMissionConfirmed;
        }, gameObject);
        #endregion
        #region stateGameOver
        // 重新尝试当前关卡
        stateGameOver.RegisterChangeEvt (stateGameMissionStart, (object[] o) => {
            return isRestart;
        }, gameObject);
        // 回到首页
        stateGameOver.RegisterChangeEvt (stateGameStart, (object[] o) => {
            return isGoGameStart;
        }, gameObject);
        #endregion
        #endregion
        #region 添加状态到controler
        controler.AddState (stateGameStart);
        controler.AddState (stateGameOver);
        controler.AddState (stateGameMissionStart);
        controler.AddState (stateGameMissionStay);
        controler.AddState (stateGameMissionEnd);
        controler.StartState (stateGameStart);
        #endregion

    }
    #region OnGameStart
    private void OnGameStart () { }
    private void BeforeGameStart () {
        currState = GamePlayState.GameStart;
    }
    private void AfterGameStart () {
        Debug.Log ("<color=yellow>Mission Start !</color>");
    }
    #endregion
    #region OnGameOver
    private void OnGameOver () {
        GameManager.Instance.ToggleGameOverDialog (true);
    }
    private void BeforeGameOver () {
        isRestart = false;
        isGameOver = false;
        currState = GamePlayState.GameOver;
    }
    private void AfterGameOver () {
        GameManager.Instance.ToggleGameOverDialog (false);
    }
    #endregion
    #region OnGameMissionStart
    private void OnGameMissionStart () {
        StartCoroutine (GameManager.Instance.InitGameLevel (gameLevel));
        isGameMissionInited = true;
    }
    private void BeforeGameMissionStart () {
        isGameMissionConfirmed = false;
        isGameStart = false;
        isGameMissionInited = false;
        currState = GamePlayState.GameMissionStart;
    }
    private void AfterGameMissionStart () {
        Debug.Log ("<color=yellow> Level " + gameLevel + " Start</color>");
    }
    #endregion
    #region OnGameMissionStay
    private void OnGameMissionStay () {
        // Debug.Log ("Level Running...");
        isGameMissionEnd = GameManager.Instance.CheckGameMission (gameLevel);
    }
    private void BeforeGameMissionStay () {
        currState = GamePlayState.GameMissionStay;
    }
    private void AfterGameMissionStay () {
        if (isGameMissionEnd) {
            Debug.Log ("<color=green>Level Complete !</color>");
        }
    }
    #endregion
    #region OnGameMissionEnd
    private void OnGameMissionEnd () {
        GameManager.Instance.MissionEnd (gameLevel);
        GameManager.Instance.ToggleGameMissionEndDialog (true);
    }
    private void BeforeGameMissionEnd () {
        isGameMissionEnd = false;
        isGameMissionConfirmed = false;
        currState = GamePlayState.GameMissionEnd;
        // 当前关卡>最大关卡, 通关
        if (gameLevel >= gameMaxLevel) isGameOver = true;
    }
    private void AfterGameMissionEnd () {
        GameManager.Instance.ToggleGameMissionEndDialog (false);
        // 未满足通关条件,切用户确认去下一关
        if (!isGameOver && isGameMissionConfirmed) {
            gameLevel++;
            Debug.Log ("<color=yellow>Go to Next Level </color>");
        } else if (isGameOver) {
            // TODO 区分通关提示
            gameLevel = 1;
            Debug.Log ("<color=green> Yeah !!! You have Complete All Missions.</color>");
        }
    }
    #endregion
}