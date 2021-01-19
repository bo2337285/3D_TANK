using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MissionType {
    ClearEnemy,
    DefendBase
}
public class Mission : ScriptableObject {
    public string id;
    public string title;
    public string description;
    public MissionType missionType;
    public int enemyTotalCount;
    // public float enemyUpdateInterval;
    // public int itemCountPerAdd;
    protected List<GameObject> enemys;
    public virtual bool IsCompleted () {
        return false;
    }
    public virtual void MissionStart () { }
    public virtual void MissionEnd () { }
    public virtual void UpdateMisson () { }
}
/// <summary>
/// 杀怪任务
/// </summary>
[CreateAssetMenu (menuName = "AssetsSetting/Missions/ClearEnemy")]
public class ClearEnemyMission : Mission {
    public int count = 0;
    public int goal;
    private GameObject player;
    public ClearEnemyMission () {
        missionType = MissionType.ClearEnemy;
    }
    public override void UpdateMisson () {
        if (IsCompleted ()) return;
        int _count = 0;
        for (var i = 0; i < enemys.Count; i++) {
            if (!enemys[i].GetComponent<Unit> ().isAlive) _count++;
        }
        count = _count;
    }
    public override bool IsCompleted () {
        return (count >= goal);
    }
    public override void MissionStart () {
        Debug.Log ("<color=green>MissionStart</color>");
        player = GameManager.Instance.AddPlayer ();
        enemys = GameManager.Instance.AddEnemy (enemyTotalCount);
        // 修改伤害计算方式
        BattleManager.Instance.SetDamageFn (DamageFn);
    }

    private float DamageFn (int giverId, int suffererId) {
        GameObject giver = GameManager.Instance.GetUnitById (giverId);
        GameObject sufferer = GameManager.Instance.GetUnitById (suffererId);
        if (giver != null && sufferer != null) {
            return giver.GetComponent<Unit> ().atk * 2 - giver.GetComponent<Unit> ().def;
        } else {
            return 0f;
        }
    }

    public override void MissionEnd () {
        // 清除战场单位
        // Destroy (player);
        foreach (var enemyGO in enemys) {
            Destroy (enemyGO);
        }
        enemys.Clear ();
        // GC检查回收
        System.GC.Collect ();
    }
}
/// <summary>
/// 防守任务
/// </summary>
[CreateAssetMenu (menuName = "AssetsSetting/Missions/DefendOfBase")]
public class DefendBaseMission : Mission {
    public int timer = 0;
    public DefendBaseMission () {
        missionType = MissionType.DefendBase;
    }

    public void UpdateMisson () {
        if (IsCompleted ()) return;
        timer--;
    }
    public override bool IsCompleted () {
        return (timer <= 0);
    }
    public override void MissionStart () { }
    public override void MissionEnd () { }
}