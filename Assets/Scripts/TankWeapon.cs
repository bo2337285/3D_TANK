using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankWeapon : MonoBehaviour {
    public GameObject defautShell;
    public GameObject buffShell;
    public float shellSpeed;
    public Transform shootPoint;
    AudioSource audioSource;

    private void OnEnable () {
        audioSource = GetComponent<AudioSource> ();
    }

    public void Shoot () {
        GameObject _shell = Instantiate (defautShell, shootPoint.position, shootPoint.rotation) as GameObject;
        Shell _s = _shell.GetComponent<Shell> ();
        // 制定炮弹的发射者,用于传递各种信息
        _s.Init (gameObject);
        _s.Shoot (shootPoint.forward);
        audioSource.clip = _s.shellAudioClip;
        audioSource.Play ();
    }
}