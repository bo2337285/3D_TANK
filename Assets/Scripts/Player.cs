using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit {
    #region 属性
    private Observer observer;
    private float inputHorizontal = 0, inputVertical = 0, mouseX = 0, mouseY = 0;
    private bool attackPressed, resetPressed;
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
    // private void Awake () {
    //     observer = ObserverManager.Instance.CreateObserver (gameObject);
    // }
    protected override void Start () {
        base.Start ();
        rb = GetComponent<Rigidbody> ();
        weapon = GetComponent<TankWeapon> ();
        observer = ObserverManager.Instance.CreateObserver (gameObject);
        // 监听用户操作
        observer.listen (EventEnum.Input, onInput);
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
    void onInput (object sender, EventArgs e) {
        InputEventArgs _e = (InputEventArgs) e;
        inputHorizontal = _e.inputHorizontal;
        inputVertical = _e.inputVertical;
        mouseX = _e.mouseX;
        mouseY = _e.mouseY;
        attackPressed = _e.attackPressed;
    }
    private void InputAction () {
        if (resetPressed) {
            // 重置x,z轴旋转
            transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0);
            rb.velocity = Vector3.zero;
        }
        // 攻击
        if (attackPressed) {
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

        // Vector3 _pos = transform.position;
        // // Vector3 target = new Vector3 (Input.GetAxis ("Vertical"), 0, Input.GetAxis ("Horizontal") * -1) * moveSpeed * Time.deltaTime;
        // Vector3 target = new Vector3 (inputVertical, 0, inputHorizontal * -1) * moveSpeed * Time.deltaTime;
        // transform.Translate (target, Space.World);
        // transform.forward = (transform.position - _pos).normalized;

        Vector3 direction = Vector3.forward * inputVertical + Vector3.right * inputHorizontal;
        rb.AddForce (direction * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
        transform.forward = direction.normalized;
    }
    private void Aimed () {
        if (mouseX != 0) {
            Vector3 point = PhysicsUtils.getMousePos ();
            // 瞄准位置要与炮头高度一致,避免低头抬头
            turret.LookAt (new Vector3 (point.x, turret.position.y, point.z));
        }
    }
}