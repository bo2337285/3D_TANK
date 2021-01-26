using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObserverManager : MonoBehaviour {
    public static ObserverManager Instance;
    public Dictionary<string, Observer> dict = new Dictionary<string, Observer> ();
    void Awake () {
        Instance = this;
    }
    public Observer CreateObserver (GameObject gObj) {
        Observer observer = gObj.AddComponent<Observer> ();
        dict.Add (gObj.GetInstanceID ().ToString (), observer);
        return observer;
    }

}