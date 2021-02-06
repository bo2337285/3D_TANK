using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            return giver.GetComponent<Unit> ().props.atk * 2 - sufferer.GetComponent<Unit> ().props.def;
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