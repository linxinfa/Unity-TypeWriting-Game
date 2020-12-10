using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 文本特效
/// </summary>
public class TextEffect : MonoBehaviour
{

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        if(null != s_root)
        {
            Destroy(s_root.gameObject);
            s_root = null;
        }
        
        s_objPool.Clear();
        var canvas = GameObject.Find("Canvas");
        if (null != canvas)
        {
            var rootObj = new GameObject("EffectRoot");
            s_root = rootObj.transform;
            s_root.SetParent(canvas.transform, false);
        }
    }


    /// <summary>
    /// 显示特效
    /// </summary>
    /// <param name="text"></param>
    /// <param name="pos"></param>
    public static void Show(string text, Vector3 pos)
    {
        if (null == s_prefab)
        {
            s_prefab = Resources.Load<GameObject>("TextEffect");
        }

        TextEffect bhv = null;
        if (s_objPool.Count > 0)
        {
            // 从对象池中取对象，
            bhv = s_objPool.Dequeue();
        }
        else
        {
            var obj = Instantiate(s_prefab);
            obj.transform.SetParent(s_root, false);
            bhv = obj.GetComponent<TextEffect>();
        }
        bhv.gameObject.SetActive(true);
        bhv.transform.position = pos;
        bhv.keyText.text = text;
    }

    /// <summary>
    /// 动画结束事件的响应函数
    /// </summary>
    public void OnAnimationEnd()
    {
        gameObject.SetActive(false);
        // 对象回收
        s_objPool.Enqueue(this);
    }

    private static GameObject s_prefab;
    /// <summary>
    /// 对象池
    /// </summary>
    private static Queue<TextEffect> s_objPool = new Queue<TextEffect>();
    /// <summary>
    /// 根节点
    /// </summary>
    private static Transform s_root;

    /// <summary>
    /// 文字组件
    /// </summary>
    public Text keyText;
}
