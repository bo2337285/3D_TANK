using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Unit {
    #region 属性
    public float moveSpeed;
    public float roateSpeed;
    public float mouseRoateSpeed;
    TankWeapon weapon;
    public float shootCD;
    Boolean canShoot = true;
    Rigidbody rb;
    #endregion
    #region 钩子函数
    void Start () {
        rb = GetComponent<Rigidbody> ();
        weapon = GetComponent<TankWeapon> ();
    }
    private void Update () {
        InputAction ();
    }
    void FixedUpdate () {
        Move ();
    }
    private void OnDrawGizmos () {
        Gizmos.DrawRay (transform.position, transform.forward * moveSpeed);
    }
    #endregion
    private void InputAction () {
        if (Input.GetButtonDown ("Reset")) {
            // 重置x,z轴旋转
            transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0);
            rb.velocity = Vector3.zero;
        }
        // 攻击
        if (Input.GetButtonDown ("Fire")) {
            Attack ();
        }
    }
    private void Attack () {
        if (canShoot) {
            StartCoroutine (Shoot ());
        }
    }
    private IEnumerator Shoot () {
        weapon.Shoot ();
        canShoot = false;
        yield return new WaitForSeconds (shootCD);
        canShoot = true;
    }
    private void Move () {
        transform.Translate (new Vector3 (0, 0, Input.GetAxis ("Vertical") * moveSpeed * Time.deltaTime));
        transform.Rotate (new Vector3 (0, Input.GetAxis ("Horizontal") * roateSpeed * Time.deltaTime + Input.GetAxis ("Mouse X") * mouseRoateSpeed, 0));
    }
}