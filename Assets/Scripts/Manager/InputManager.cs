using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InputManager : MonoBehaviour {

    public static InputManager Instance;
    public Joystick joystickLeft, joystickRight;
    public Canvas joystickUI;
    private Observer observer;
    private InputEventArgs inputEventArgs;
    private bool attackPressed, resetPressed; //UI 按钮

    private void Awake () {
        // if (instance == null) {
        //     instance = this;
        // } else {
        //     if (instance != this) {
        //         Destroy (gameObject);
        //     }
        // }
        // DontDestroyOnLoad (gameObject);
        Instance = this;
    }

    void Start () {
        init ();
    }

    void Update () {
        inputControler ();
    }

    void init () {
        observer = ObserverManager.Instance.CreateObserver (gameObject);
        inputEventArgs = new InputEventArgs ();
        // 控制虚拟按键
        // #if UNITY_ANDROID || UNITY_IOS
        //         Debug.Log (SystemInfo.deviceType);
        //         joystickUI.enabled = true;
        // #endif
    }

    void inputControler () {
        attackPressed = (joystickRight.Horizontal != 0 || joystickRight.Vertical != 0);

        inputEventArgs.mouseDownLeft = Input.GetMouseButtonDown (0);
        inputEventArgs.mouseDownRight = Input.GetMouseButtonDown (1);
        inputEventArgs.attackPressed = attackPressed || Input.GetButtonDown ("Fire") || inputEventArgs.mouseDownLeft;
        inputEventArgs.resetPressed = resetPressed || Input.GetButtonDown ("Reset") || inputEventArgs.mouseDownLeft;
        inputEventArgs.inputHorizontal = Input.GetAxis ("Horizontal") != 0 ? Input.GetAxis ("Horizontal") : joystickLeft.Horizontal;
        inputEventArgs.inputVertical = Input.GetAxis ("Vertical") != 0 ? Input.GetAxis ("Vertical") : joystickLeft.Vertical;
        inputEventArgs.mouseX = Input.GetAxis ("Mouse X") != 0 ? Input.GetAxis ("Mouse X") : joystickRight.Horizontal;
        inputEventArgs.mouseY = Input.GetAxis ("Mouse Y") != 0 ? Input.GetAxis ("Mouse Y") : joystickRight.Vertical;

        observer.dispatch (EventEnum.Input, gameObject, inputEventArgs);
    }

}