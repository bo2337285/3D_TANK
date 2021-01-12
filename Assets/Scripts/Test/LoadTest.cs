using UnityEngine;
public class LoadTest : MonoBehaviour {
    // [RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void BeforeRuntimeInitializeOnLoadMethod () {
        Debug.Log ("BeforeRuntimeInitializeOnLoadMethod");
    }

    // [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeInitializeOnLoadMethod () {
        Debug.Log ("OnRuntimeInitializeOnLoadMethod");
    }

    // [RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AfterRuntimeInitializeOnLoadMethod () {
        Debug.Log ("AfterRuntimeInitializeOnLoadMethod");
    }

    private void Awake () {
        Debug.Log ("Awake");
    }

    private void Start () {
        Debug.Log ("Start");

    }

    bool isUpdate = true;
    private void Update () {
        if (isUpdate) {
            Debug.Log ("Update");
            isUpdate = false;
        }

    }
}