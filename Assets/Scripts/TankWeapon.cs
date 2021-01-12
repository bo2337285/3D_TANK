using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankWeapon : MonoBehaviour {
    public GameObject shell;
    public float shellSpeed;
    public Transform shootPoint;
    AudioSource audioSource;

    private void OnEnable () {
        audioSource = GetComponent<AudioSource> ();
    }

    public void Shoot () {
        GameObject _shell = Instantiate (shell, shootPoint.position, shootPoint.rotation) as GameObject;
        Shell _s = _shell.GetComponent<Shell> ();
        // 制定炮弹的发射者,用于传递各种信息
        _s.Init (gameObject);
        Rigidbody _rb = _shell.GetComponent<Rigidbody> ();
        _rb.velocity = shootPoint.forward * shellSpeed;
        audioSource.Play ();
    }
}