using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InputEventArgs : System.EventArgs {
    public float inputHorizontal, inputVertical, mouseX, mouseY;
    public bool attackPressed, resetPressed, mouseDownLeft, mouseDownRight;
}