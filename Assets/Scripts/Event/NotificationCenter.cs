using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NotificationCenter的拓展类，在这里弄出多个INotificationCenter的子类，
// 分别处理不同的消息转发，便于消息分组
public class NotificationCenter : INotificationCenter {
    private static INotificationCenter singleton;

    // private event EventHandler GameOver;
    // private event EventHandler ScoreAdd;
    // private event EventHandler TouchItems;

    private NotificationCenter () : base () {
        // 在这里添加需要分发的各种消息
        // eventTable["GameOver"] = GameOver;
        // eventTable["ScoreAdd"] = ScoreAdd;
        // eventTable["TouchItems"] = TouchItems;
    }

    public static INotificationCenter GetInstance () {
        if (singleton == null)
            singleton = new NotificationCenter ();
        return singleton;
    }
}

// NotificationCenter的抽象基类
public abstract class INotificationCenter {

    protected Dictionary<int, EventHandler> eventTable;

    protected INotificationCenter () {
        eventTable = new Dictionary<int, EventHandler> ();
    }

    // PostNotification -- 将名字为name，发送者为sender，参数为e的消息发送出去
    public void PostNotification (EventEnum name) {
        this.PostNotification (name, null, EventArgs.Empty);
    }
    public void PostNotification (EventEnum name, object sender) {
        this.PostNotification (name, sender, EventArgs.Empty);
    }
    public void PostNotification (EventEnum name, object sender, EventArgs e) {
        int _name = (int) name;
        if (eventTable.ContainsKey (_name)) {
            eventTable[_name] (sender, e);
        }
    }

    // 添加或者移除了一个回调函数。
    public void AddEventHandler (EventEnum name, EventHandler handler) {
        int _name = (int) name;
        if (eventTable.ContainsKey (_name)) {
            eventTable[_name] += handler;
        } else {
            eventTable.Add (_name, handler);
        }
    }
    public void RemoveEventHandler (EventEnum name, EventHandler handler) {
        int _name = (int) name;
        if (eventTable.ContainsKey (_name)) {
            eventTable[_name] -= handler;
        }
    }
}