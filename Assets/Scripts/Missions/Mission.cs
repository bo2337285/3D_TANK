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

