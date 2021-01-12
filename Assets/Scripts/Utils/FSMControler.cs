using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FSMControler : MonoBehaviour {
    #region 属性
    public string _name;
    private FSMState currState;
    private List<FSMState> stateList = new List<FSMState> ();
    private float deltaTime = Time.deltaTime;
    #endregion

    /// <summary>
    /// 启用状态
    /// </summary>
    /// <param name="state">开始状态</param>
    public void StartState (FSMState state) {
        if (stateList.Contains (state)) {
            currState = state;
            state.Enter ();
            ListenChangeState ();
        } else {
            Debug.LogError (String.Format ("不存在该状态{0}", state.Name));
        }
    }
    public void StartState (string stateName) {
        FSMState state = SearchState (stateName);
        if (state != null) {
            StartState ();
        } else {
            Debug.LogError (String.Format ("不存在该状态{0}", stateName));
        }
    }
    public void StartState () {
        if (stateList.Count > 0) {
            StartState (stateList[0]);
        } else {
            Debug.LogError ("未添加状态");
        }
    }
    public void ListenChangeState () {
        StartCoroutine (ChangeState ());
    }
    IEnumerator ChangeState () {
        while (currState != null) {
            FSMState nextState = currState.ListenStateChange ();
            if (nextState != null) {
                currState.Exit ();
                currState = nextState;
                nextState.Enter ();
            }
            yield return new WaitForSeconds (deltaTime);
        }
    }
    public FSMState SearchState (string stateName) {
        foreach (var item in stateList) {
            if (item.Name == stateName) return item;
        }
        return null;
    }
    public void AddState (FSMState state) {
        foreach (var item in stateList) {
            if (item.Name == state.Name) {
                Debug.LogError ("已存在同名状态");
                return;
            }
        }
        state.controler = this;
        stateList.Add (state);
    }
    public void RemoveState (FSMState state) {
        if (state != null) stateList.Remove (state);
    }
    public void RemoveState (string stateName) {
        stateList.Remove (SearchState (stateName));
    }
    public void StopState () {
        currState.Exit ();
        currState = null;
        StopCoroutine (ChangeState ());
    }
}