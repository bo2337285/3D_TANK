using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName = "AssetsSetting/UnitSetting")]
public class UnitSetting : ScriptableObject {
    public string unitName;
    public float hp;
    public float sp;
    public float atk;
    public float def;
}