using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextEffect : MonoBehaviour
{
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


    public static void Show(string text, Vector3 pos)
    {
        if (null == s_prefab)
        {
            s_prefab = Resources.Load<GameObject>("TextEffect");
        }

        TextEffect bhv = null;
        if (s_objPool.Count > 0)
        {
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

    public void OnAnimationEnd()
    {
        gameObject.SetActive(false);
        // 对象回收
        s_objPool.Enqueue(this);
    }

    private static GameObject s_prefab;
    private static Queue<TextEffect> s_objPool = new Queue<TextEffect>();
    private static Transform s_root;

    public Text keyText;
}
