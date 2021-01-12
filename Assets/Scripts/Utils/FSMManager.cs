using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMManager : MonoBehaviour {
    public static FSMManager Instance;
    public Dictionary<string, FSMControler> dict = new Dictionary<string, FSMControler> ();
    void Awake () {
        Instance = this;
    }
    public FSMControler CreateFSMControler (string name, GameObject gObj) {
        FSMControler controler = gObj.AddComponent<FSMControler> ();
        dict.Add (name, controler);
        return controler;
    }

}