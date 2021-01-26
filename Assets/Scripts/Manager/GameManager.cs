using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    #region 属性
    [HideInInspector] public static GameManager Instance;
    private GameObject EnemyGroups;
    private GameObject player;
    public GameObject goGameLevel;
    public GameObject gameMissionEndDialog;
    public GameObject gameOverDialog;
    private TMP_Text textGameLevel;
    public Dictionary<int, GameObject> unitDict = new Dictionary<int, GameObject> ();
    public List<Mission> missions;
    public int beginLevel = 1;

    #endregion
    #region 钩子函数
    private void Awake () {
        Instance = this;
    }
    private void Start () {
        Init ();
    }
    #endregion
    #region 函数
    private void Init () {
        // // 创建玩家
        // player = GameObject.Find ("Player");
        // if (player == null) {
        //     player = ResourceManager.Instance.playerGObj;
        //     Instantiate<GameObject> (player, Vector3.zero, Quaternion.identity);
        // }
        // 创建初始敌人分组
        EnemyGroups = GameObject.Find ("EnemyGroups");
        if (EnemyGroups == null) {
            EnemyGroups = new GameObject ("EnemyGroups");
        }
        textGameLevel = goGameLevel.GetComponent<TMP_Text> ();
        // 初始化游戏流程
        // TODO 获取配置的任务,后续需要排个序
        // missions = ResourceManager.Instance.missions;
        GetMissions ();
        if (missions.Count > 0) {
            GamePlayManager.Instance.Init (beginLevel - 1, missions.Count - 1);
            GamePlayManager.Instance.GameStart ();
        } else {
            Debug.LogWarning ("No Missions!");
        }
    }
    private void GetMissions () {
        // FIXME 回头再调整任务配置这块
        foreach (var item in ResourceManager.Instance.missions) {
            if (item.missionType == MissionType.ClearEnemy) {
                ClearEnemyMission _item = (ClearEnemyMission) item;
                ClearEnemyMission _m = new ClearEnemyMission ();
                _m.id = _item.id;
                _m.title = _item.title;
                _m.description = _item.description;
                _m.enemyTotalCount = _item.enemyTotalCount;
                _m.goal = _item.goal;
                missions.Add (_m);
            }
        }
    }
    public GameObject GetUnitById (int id) {
        return unitDict.ContainsKey (id) ? unitDict[id] : null;
    }
    public void Restart () {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
    }
    public GameObject AddPlayer () {
        player = GameObject.Find ("Player");
        if (player == null) {
            player = ResourceManager.Instance.playerGObj;
            Instantiate<GameObject> (player, Vector3.zero, Quaternion.identity);
        }
        return player;
    }
    public List<GameObject> AddEnemy (int count) {
        List<GameObject> enemyList = new List<GameObject> ();
        for (var i = 0; i < count; i++) {
            enemyList.Add (AddEnemy ());
        }
        return enemyList;
    }
    public GameObject AddEnemy () {
        // FIXME 随机点不能保证NavMeshAgent有足够位置放
        // Vector3 randomPoint = PhysicsUtils.GetRandomPointInMap (new Vector2 (20f, 60f), 20f, Vector3.zero);
        Vector3 randomPoint = PhysicsUtils.GetNavMeshRandomPos ();
        GameObject _enemy = Instantiate<GameObject> (ResourceManager.Instance.enemyGObj, randomPoint, Quaternion.identity, EnemyGroups.transform);
        return _enemy;
    }
    public void RegisterUnit (GameObject unit) {
        unitDict.Add (unit.GetInstanceID (), unit);
    }
    public void UnRegisterUnit (GameObject unit) {
        unitDict.Remove (unit.GetInstanceID ());
    }
    public IEnumerator InitGameLevel (int level) {
        InitGameMission (level);
        yield return StartCoroutine (ShowGameLevel (level));
    }
    public void MissionEnd (int level) {
        missions[level].MissionEnd ();
    }
    public void InitGameMission (int level) {
        missions[level].MissionStart ();
    }
    public bool CheckGameMission (int level) {
        Mission currMission = missions[level];
        currMission.UpdateMisson ();
        return currMission.IsCompleted ();
    }
    IEnumerator ShowGameLevel (int level) {
        goGameLevel.SetActive (true);
        Time.timeScale = 0; //暂停游戏
        textGameLevel.text = "Level " + (level + 1).ToString ();
        // 这里不能用WaitForSeconds,因为暂停了游戏
        // yield return new WaitForSeconds (1f);
        yield return new WaitForSecondsRealtime (1f);
        goGameLevel.SetActive (false);
        Time.timeScale = 1; //恢复游戏
    }
    public void ToggleGameMissionEndDialog (bool flag) {
        gameMissionEndDialog.SetActive (flag);
    }
    public void ToggleGameOverDialog (bool flag) {
        gameOverDialog.SetActive (flag);
    }
    #endregion
}