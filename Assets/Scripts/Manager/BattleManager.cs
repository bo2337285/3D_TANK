using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
/// <summary>
/// 记录打架,暂时只记录打架双方和伤害   
/// </summary>
public struct Battle {
    // 用ID是避免gameobject被删了
    // 施与者
    public int giverId;
    // 承受者
    public int suffererId;
    public float damage;
    public Battle (int giverId, int suffererId) {
        this.giverId = giverId;
        this.suffererId = suffererId;
        this.damage = BattleManager.Instance.GetDamage (giverId, suffererId);
    }
    public override string ToString () {
        return string.Format ("{0} -> {1} :Damage({2})", giverId, suffererId, damage);
    }
}
public class BattleManager : MonoBehaviour {
    public static BattleManager Instance;
    public List<Battle> battleDictSerialize = new List<Battle> ();
    // 打人字典
    public Dictionary<int, List<Battle>> giverBattleDict = new Dictionary<int, List<Battle>> ();
    // 被打字典
    public Dictionary<int, List<Battle>> suffererBattleDict = new Dictionary<int, List<Battle>> ();

    public delegate float _GetDamage (int giverId, int suffererId);
    private event _GetDamage DamageFn;

    private void Awake () {
        Instance = this;
    }
    public void SetDamageFn (_GetDamage DamageFn) {
        this.DamageFn = DamageFn;
    }

    public float GetDamage (int giverId, int suffererId) {
        GameObject giver = GameManager.Instance.GetUnitById (giverId);
        GameObject sufferer = GameManager.Instance.GetUnitById (suffererId);
        if (giver != null && sufferer != null) {
            // 如果被其他类修改了DamageFn,则用之
            if (DamageFn != null) {
                return DamageFn (giverId, suffererId);
            } else {
                // 否则就常规地攻减防计算
                return giver.GetComponent<Unit> ().props.atk - sufferer.GetComponent<Unit> ().props.def;
            }
        } else {
            return 0f;
        }
    }

    public void AddBattle (Battle battle) {
        int giverId = battle.giverId;
        int suffererId = battle.suffererId;
        if (!giverBattleDict.ContainsKey (giverId)) giverBattleDict.Add (giverId, new List<Battle> ());
        if (!suffererBattleDict.ContainsKey (suffererId)) suffererBattleDict.Add (suffererId, new List<Battle> ());
        List<Battle> giverBattles = giverBattleDict[giverId];
        List<Battle> suffererBattles = suffererBattleDict[suffererId];
        giverBattles.Add (battle);
        suffererBattles.Add (battle);
        battleDictSerialize.Add (battle);
        GameObject sufferer = GameManager.Instance.GetUnitById (suffererId);
        // 通知挨打者你挨打了
        if (sufferer != null) {
            // Debug.Log (sufferer);
            sufferer.SendMessage ("BeHurt", battle, SendMessageOptions.RequireReceiver);
        }
    }

}