using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 防守任务
/// </summary>
[CreateAssetMenu (menuName = "AssetsSetting/Missions/DefendOfBase")]
public class DefendBaseMission : Mission {
    public int timer = 0;
    public DefendBaseMission () {
        missionType = MissionType.DefendBase;
    }

    public override void UpdateMisson () {
        if (IsCompleted ()) return;
        timer--;
    }
    public override bool IsCompleted () {
        return (timer <= 0);
    }
    public override void MissionStart () { }
    public override void MissionEnd () { }
}