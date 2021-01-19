using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerManager {

    static int[] canAttackLayerList = { 9, 11, 12 };

    public static bool isCanAttackLayer (int layerIdx) {
        return Array.IndexOf<Int32> (canAttackLayerList, layerIdx) > -1;
    }
}