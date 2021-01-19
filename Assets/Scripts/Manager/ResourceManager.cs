using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {
    public static ResourceManager Instance;
    public UnitSetting enemyInfo;
    public UnitSetting playerInfo;
    public GameObject playerGObj;
    public GameObject enemyGObj;
    public List<Mission> missions;
    void Awake () {
        Instance = this;
        LoadResource ();
    }
    void LoadResource () {
        // 加载单位属性模板
        StartCoroutine (LoadUnitSetting ());
        // 加载预制体
        StartCoroutine (LoadPrefabs ());
        // 加载预制体
        StartCoroutine (LoadMissions ());
        // 加载地图资源
        // etc
    }
    IEnumerator LoadMissions () {
        Object[] _missions = Resources.LoadAll ("Missions");
        Debug.Log (_missions.Length);
        foreach (var item in _missions) {
            missions.Add (item as Mission);
        }
        yield return new WaitForEndOfFrame ();
    }
    IEnumerator LoadPrefabs () {
        enemyGObj = Resources.Load ("Prefabs/Enemy") as GameObject;
        playerGObj = Resources.Load ("Prefabs/Player") as GameObject;
        yield return new WaitForEndOfFrame ();
        // Debug.Log ("<color=green> Prefabs Load Success </color>");
    }
    IEnumerator LoadUnitSetting () {
        enemyInfo = Resources.Load ("Settings/EnemyInfo") as UnitSetting;
        playerInfo = Resources.Load ("Settings/PlayerInfo") as UnitSetting;
        yield return new WaitForEndOfFrame ();
        // Debug.Log ("<color=green> UnitSetting Load Success </color>");
    }

}