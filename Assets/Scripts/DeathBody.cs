using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBody : MonoBehaviour {
    public float destroyTime = 2f;
    Component[] mrList;
    float process = -1;
    void Start () {
        mrList = GetComponentsInChildren (typeof (MeshRenderer));
        Destroy (gameObject, destroyTime);
    }
    private void Update () {
        foreach (MeshRenderer item in mrList) {
            if (item != null) {
                item.material.SetFloat ("Process", Mathf.Lerp (process, destroyTime, Time.deltaTime));
            }
        }
        process += Time.deltaTime;
    }
}