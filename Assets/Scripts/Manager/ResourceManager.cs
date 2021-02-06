using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {
    public static ResourceManager Instance;
    public UnitProps enemyInfo;
    public UnitProps playerInfo;
    public GameObject playerGObj;
    public GameObject enemyGObj;
    public List<Mission> missions;
    void Awake () {
        Instance = this;
        LoadResource ();
    }
    void LoadResource () {
        // 加载单位属性模板
        StartCoroutine (LoadUnitProps ());
        // 加载预制体
        StartCoroutine (LoadPrefabs ());
        // 加载预制体
        StartCoroutine (LoadMissions ());
        // 加载地图资源
        // etc
    }
    IEnumerator LoadMissions () {
        Object[] _missions = Resources.LoadAll ("Missions");
        // Debug.Log (_missions.Length);
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
    IEnumerator LoadUnitProps () {
        enemyInfo = ((UnitPropsSetting) Resources.Load ("Settings/EnemyProps")).props;
        playerInfo = ((UnitPropsSetting) Resources.Load ("Settings/PlayerProps")).props;
        yield return new WaitForEndOfFrame ();
        // Debug.Log ("<color=green> UnitProps Load Success </color>");
    }

}