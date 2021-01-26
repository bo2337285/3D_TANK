using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TargetEventArgs : System.EventArgs {
    public GameObject _target;
    public object[] args;

    public TargetEventArgs (GameObject t) {
        _target = t;
    }
}