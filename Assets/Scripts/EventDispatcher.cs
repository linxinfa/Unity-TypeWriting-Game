using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 事件委托
/// </summary>
public delegate void MyEventHandler(params object[] objs);

/// <summary>
/// 事件管理器
/// </summary>
public class EventDispatcher
{
    /// <summary>
    /// 注册事件的响应函数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="handler"></param>
    public void Regist(string type, MyEventHandler handler)
    {
        if (handler == null)
            return;

        if (!listeners.ContainsKey(type))
        {
            listeners.Add(type, new Dictionary<int, MyEventHandler>());
        }
        var handlerDic = listeners[type];
        var handlerHash = handler.GetHashCode();
        if (handlerDic.ContainsKey(handlerHash))
        {
            handlerDic.Remove(handlerHash);
        }
        listeners[type].Add(handler.GetHashCode(), handler);
    }

    /// <summary>
    /// 注销事件的响应函数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="handler"></param>
    public void UnRegist(string type, MyEventHandler handler)
    {
        if (handler == null)
            return;

        if (listeners.ContainsKey(type))
        {
            listeners[type].Remove(handler.GetHashCode());
            if (null == listeners[type] || 0 == listeners[type].Count)
            {
                listeners.Remove(type);
            }
        }
    }

    /// <summary>
    /// 抛出事件，触发之前注册过的响应函数
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="objs"></param>
    public void DispatchEvent(string evt, params object[] objs)
    {
        if (listeners.ContainsKey(evt))
        {
            var handlerDic = listeners[evt];
            if (handlerDic != null && 0 < handlerDic.Count)
            {
                var dic = new Dictionary<int, MyEventHandler>(handlerDic);
                foreach (var f in dic.Values)
                {
                    try
                    {
                        f(objs);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogErrorFormat(szErrorMessage, evt, ex.Message, ex.StackTrace);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 清理事件
    /// </summary>
    /// <param name="key"></param>
    public void ClearEvents(string key)
    {
        if (listeners.ContainsKey(key))
        {
            listeners.Remove(key);
        }
    }

    /// <summary>
    /// 事件监听缓存
    /// </summary>
    private Dictionary<string, Dictionary<int, MyEventHandler>> listeners = new Dictionary<string, Dictionary<int, MyEventHandler>>();
    private readonly string szErrorMessage = "DispatchEvent Error, Event:{0}, Error:{1}, {2}";

    /// <summary>
    /// 单例模式
    /// </summary>
    private static EventDispatcher s_instance;
    public static EventDispatcher Instance
    {
        get
        {
            if (null == s_instance)
                s_instance = new EventDispatcher();
            return s_instance;
        }
    }
}
