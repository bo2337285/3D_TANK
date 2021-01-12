using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 状态
/// </summary>
public class FSMState {
    #region  委托
    /// <summary>
    /// 状态事件委托
    /// </summary>
    public delegate void StateEventHandle ();
    /// <summary>
    /// 状态改变委托
    /// </summary>
    /// <param name="objs"> 对象类型参数</param>
    /// <returns>是否允许改变状态</returns>
    public delegate bool StateChangeHandle (params object[] objs);
    #endregion
    #region StateChange
    /// <summary>
    /// 切换条件参数
    /// </summary>
    public struct StateChange {
        private StateChangeHandle stateChangeHandle;
        private object[] objs;
        public StateChangeHandle ChangeHandle { get { return stateChangeHandle; } }
        public object[] Objs { get { return objs; } }
        public StateChange (StateChangeHandle sh, params object[] objs) {
            this.stateChangeHandle = sh;
            this.objs = objs;
        }
    }
    #endregion
    #region 属性
    private string name;
    /// <summary>
    /// 状态名
    /// </summary>
    /// <returns></returns>
    public string Name { get { return name; } }
    /// <summary>
    /// 状态刷新间隔
    /// </summary>
    private float deltaTime;
    public MonoBehaviour controler;
    /// <summary>
    /// 是否在执行该状态
    /// </summary>
    private bool isRun;
    public bool IsRun { get { return isRun; } }

    //回调
    private event StateEventHandle onEnter;
    private event StateEventHandle onStay;
    private event StateEventHandle onExit;
    /// <summary>
    /// 状态转换字典
    /// </summary>
    private Dictionary<FSMState, StateChange> dictChangeState = null;
    #endregion
    #region 构造方法
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="name">状态名</param>
    /// <param name="onEnter">开始状态回调</param>
    /// <param name="onStay">停留状态回调</param>
    /// <param name="onExit">离开状态回调</param>
    /// <param name="deltaTime">状态Stay时执行回调间隔</param>
    public FSMState (string name, float deltaTime, StateEventHandle onEnter = null, StateEventHandle onStay = null, StateEventHandle onExit = null) {
        this.name = name;
        this.deltaTime = deltaTime;
        this.onEnter = onEnter;
        this.onStay = onStay;
        this.onExit = onExit;
    }
    public FSMState (string name, StateEventHandle onEnter = null, StateEventHandle onStay = null, StateEventHandle onExit = null) {
        this.name = name;
        this.deltaTime = 0.2f;
        this.onEnter = onEnter;
        this.onStay = onStay;
        this.onExit = onExit;
    }

    #endregion
    #region 状态处理
    public virtual void Enter () {
        if (IsRun) return;
        isRun = true;
        onEnter ();
        Stay ();
    }
    public virtual void Stay () {
        controler.StartCoroutine (DoStay ());
    }
    IEnumerator DoStay () {
        while (isRun) {
            onStay ();
            yield return new WaitForSeconds (deltaTime);
        }
    }
    public virtual void Exit () {
        if (!isRun) return;
        isRun = false;
        onExit ();
    }
    #endregion
    #region 监听
    /// <summary>
    /// 监听状态是否更改
    /// </summary>
    /// <returns>更改返回新状态,否则返回null</returns>
    public virtual FSMState ListenStateChange () {
        FSMState currState = null;
        foreach (var item in dictChangeState) {
            var _state = item.Value;
            if (_state.ChangeHandle (_state)) {
                currState = item.Key;
                break;
            }
        }
        return currState;
    }

    #endregion
    #region 注册
    /// <summary>
    /// 注册状态切换
    /// </summary>
    /// <param name="state">状态</param>
    /// <param name="stateChange">状态切换</param>
    public void RegisterChangeEvt (FSMState state, StateChange stateChange) {
        dictChangeState.Add (state, stateChange);
    }
    public void RegisterChangeEvt (FSMState state, StateChangeHandle onStateChange, params object[] objs) {
        RegisterChangeEvt (state, new StateChange (onStateChange, objs));
    }
    public void RegisterChangeEvt (string name, StateEventHandle onEnter, StateEventHandle onStay, StateEventHandle onExit, StateChange stateChange) {
        RegisterChangeEvt (new FSMState (name, onEnter, onStay, onExit), stateChange);
    }
    public void RegisterChangeEvt (string name, StateEventHandle onEnter, StateEventHandle onStay, StateEventHandle onExit, StateChangeHandle onStateChange, params object[] objs) {
        RegisterChangeEvt (new FSMState (name, onEnter, onStay, onExit), new StateChange (onStateChange, objs));
    }

    #endregion
    #region 追加处理
    /// <summary>
    /// 追加Enter状态回调
    /// </summary>
    /// <param name="handler"></param>
    public void AddEnterEvt (StateEventHandle handler) {
        AddEvt (onEnter, handler);
    }
    /// <summary>
    /// 追加Stay状态回调
    /// </summary>
    /// <param name="handler"></param>
    public void AddStayEvt (StateEventHandle handler) {
        AddEvt (onStay, handler);
    }
    /// <summary>
    /// 追加Exit状态回调
    /// </summary>
    /// <param name="handler"></param>
    public void AddExitEvt (StateEventHandle handler) {
        AddEvt (onExit, handler);
    }
    private void AddEvt (StateEventHandle handleDelegate, StateEventHandle handler) {
        handleDelegate += handler;

    }
    #endregion 
    #region 清除处理
    /// <summary>
    /// 清除Enter状态回调
    /// </summary>
    /// <param name="handler"></param>
    public void ClearEnterEvt () {
        ClearEvt (onEnter);
    }
    /// <summary>
    /// 清除Stay状态回调
    /// </summary>
    /// <param name="handler"></param>
    public void ClearStayEvt () {
        ClearEvt (onStay);
    }
    /// <summary>
    /// 清除Exit状态回调
    /// </summary>
    /// <param name="handler"></param>
    public void ClearExitEvt () {
        ClearEvt (onExit);
    }
    private void ClearEvt (StateEventHandle handleDelegate) {
        Delegate[] list = handleDelegate.GetInvocationList ();
        for (var i = list.Length - 1; i >= 0; i--) {
            handleDelegate -= list[i] as StateEventHandle;
        }
    }
    #endregion
}