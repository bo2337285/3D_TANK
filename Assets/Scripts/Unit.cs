using UnityEngine;

public class Unit : MonoBehaviour {
    // 攻击力,防御力,血量
    public float atk = 0;
    public float def = 0;
    public float maxHP = 100;
    public float HP = 0;
    public GameObject deathFX;
    public UnitSetting unitSetting;
    public bool isAlive;

    protected virtual void Start () {
        HP = maxHP;
        // 自己去GameManager那注册一下自己
        GameManager.Instance.RegisterUnit (gameObject);
        isAlive = true;
    }
    private void OnDestroy () {
        // 销毁了要去GameManager那清除记录
        GameManager.Instance.UnRegisterUnit (gameObject);
    }
    public virtual void ApplyDamage (float damage) {
        if (HP > damage) {
            HP -= damage;
        } else {
            Destruct ();
        }
    }
    public void Destruct () {
        if (deathFX != null) {
            Instantiate (deathFX, transform.position, transform.rotation);
        }
        gameObject.SetActive (false);
        isAlive = false;
        // FIXME 何时清除比较合适?
        // Destroy (gameObject);
    }
    // TODO 考虑下如何不写在基类里
    // 用于过滤扫到的目标
    public virtual bool CanAttackFilter (RaycastHit hit) {
        return CanAttackFilter (hit.collider.gameObject);
    }
    public virtual bool CanAttackFilter (GameObject filterObj) {
        int layerIdx = filterObj.layer;
        return layerIdx != gameObject.layer && LayerManager.isCanAttackLayer (layerIdx);
    }
    public virtual void BeHurt (Battle battle) {
        ApplyDamage (battle.damage);
    }
}