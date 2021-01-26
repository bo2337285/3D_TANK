using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour {
    public INotificationCenter notification;
    private Dictionary<EventEnum, EventHandler> handlers;

    void Awake () {
        handlers = new Dictionary<EventEnum, EventHandler> ();
        notification = NotificationCenter.GetInstance ();
    }

    void OnDestroy () {
        foreach (KeyValuePair<EventEnum, EventHandler> kvp in handlers) {
            notification.RemoveEventHandler (kvp.Key, kvp.Value);
        }
    }

    public void listen (EventEnum name, EventHandler handler) {
        notification.AddEventHandler (name, handler);
        handlers.Add (name, handler);
    }
    public void dispatch (EventEnum name) {
        notification.PostNotification (name);
    }
    public void dispatch (EventEnum name, object sender) {
        notification.PostNotification (name, sender);
    }
    public void dispatch (EventEnum name, object sender, EventArgs e) {
        notification.PostNotification (name, sender, e);
    }
}