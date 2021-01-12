using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    public int health = 100;
    public GameObject deathFX;

    public virtual void ApplyDamage (int damage) {
        if (health > damage) {
            health -= damage;
        } else {
            Destruct ();
        }
    }
    public void Destruct () {
        if (deathFX != null) {
            Instantiate (deathFX, transform.position, transform.rotation);
        }
        Destroy (gameObject);
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
}