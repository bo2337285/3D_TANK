using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBody : MonoBehaviour {
    public float destroyTime = 2f;
    void Start () {
        Invoke ("Destruct", destroyTime);
    }
    void Destruct () {
        Destroy (gameObject);
    }

}