using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit {
    #region 属性
    public float moveSpeed;
    public float roateSpeed;
    public float mouseRoateSpeed;
    TankWeapon weapon;
    public float shootCD;
    Boolean canShoot = true;
    Rigidbody rb;
    public Transform turret; // 炮台,用于鼠标瞄准
    #endregion
    #region 钩子函数
    protected override void Start () {
        base.Start ();
        rb = GetComponent<Rigidbody> ();
        weapon = GetComponent<TankWeapon> ();
        // // 自己去GameManager那注册一下自己
        // GameManager.Instance.RegisterUnit (gameObject);
    }
    private void Update () {
        InputAction ();
    }
    void FixedUpdate () {
        Move ();
        Aimed ();
    }
    private void OnDrawGizmos () {
        Gizmos.DrawRay (turret.position, turret.forward * moveSpeed);
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
        // transform.Translate (new Vector3 (0, 0, Input.GetAxis ("Vertical") * moveSpeed * Time.deltaTime));
        // transform.Rotate (new Vector3 (0, Input.GetAxis ("Horizontal") * roateSpeed * Time.deltaTime, 0));
        Vector3 _pos = transform.position;
        Vector3 target = new Vector3 (Input.GetAxis ("Vertical"), 0, Input.GetAxis ("Horizontal") * -1) * moveSpeed * Time.deltaTime;
        transform.Translate (target, Space.World);
        transform.forward = (transform.position - _pos).normalized;
    }
    private void Aimed () {
        if (Input.GetAxis ("Mouse X") != 0) {
            Vector3 point = PhysicsUtils.getMousePos ();
            // turret.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (point - transform.position), roateSpeed * Time.deltaTime);
            // 瞄准位置要与炮头高度一致,避免低头抬头
            turret.LookAt (new Vector3 (point.x, turret.position.y, point.z));
        }
    }
}