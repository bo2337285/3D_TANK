using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour {
    public GameObject explosionFX;
    public float explosionRadius;
    public float explosionForce;
    public GameObject creator; // 创建子弹的对象
    public float shellSpeed;
    public delegate bool CanAttackFilter (GameObject filterObj);
    private Rigidbody rb;
    public AudioClip shellAudioClip;
    void Awake () {
        rb = GetComponent<Rigidbody> ();
    }
    public virtual void Shoot (Vector3 dir) {
        Debug.Log (rb);
        rb.velocity = dir * shellSpeed;
    }
    public void Init (GameObject _creator) {
        creator = _creator;
    }
    void HitColl (Collider coll, CanAttackFilter filter) {
        if (!filter (coll.gameObject)) return;
        // Debug.Log ("Hurt!", coll.gameObject);
        Rigidbody _rb = coll.GetComponent<Rigidbody> ();
        if (_rb != null) { // 过滤没刚体的
            _rb.AddExplosionForce (explosionForce, transform.position, explosionRadius);
        }
        // 伤害
        BattleManager.Instance.AddBattle (new Battle (creator.GetInstanceID (), coll.gameObject.GetInstanceID ()));
    }
    private void OnTriggerEnter (Collider other) {
        // 打到自己不算
        if (other.gameObject == creator) return;
        // 生成爆炸效果
        GameObject _explosionFX = Instantiate (explosionFX, transform.position, transform.rotation) as GameObject;
        float destoryTime = _explosionFX.GetComponent<ParticleSystem> ().main.duration; // 读取粒子系统的持续时间
        Destroy (gameObject);
        Destroy (_explosionFX, destoryTime);

        // 获取子弹附近explosionRadius范围内的碰撞器
        Collider[] colliders = Physics.OverlapSphere (transform.position, explosionRadius);
        // 获取创造者的可攻击过滤器
        // FIXME creator销毁后这里会报错
        Unit unit = creator.GetComponent<Unit> ();
        if (unit != null && colliders.Length > 0) {
            for (var i = 0; i < colliders.Length; i++) {
                Collider coll = colliders[i];
                // TODO 重载不用传filter的方法
                HitColl (coll, unit.CanAttackFilter);
            }
        }
    }
}