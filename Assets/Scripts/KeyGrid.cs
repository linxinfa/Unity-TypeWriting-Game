using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyGrid : MonoBehaviour
{
    public GameObject item;
    private List<Text> m_uiTextList = new List<Text>();

    private void Awake()
    {
        item.SetActive(false);
    }

    public void CreateKeyList(List<KeyCode> list)
    {
        for (int i = 0, cnt = list.Count; i < cnt; ++i)
        {
            Text uiText = null;
            if (m_uiTextList.Count > i)
            {
                uiText = m_uiTextList[i];
            }
            else
            {
                var obj = GameObject.Instantiate<GameObject>(item);
                obj.SetActive(true);
                obj.transform.SetParent(item.transform.parent, false);
                var textTrans = obj.transform.Find("Text");
                uiText = textTrans.GetComponent<Text>();
                m_uiTextList.Add(uiText);
            }
            uiText.text = list[i].ToString();
        }
    }

    public void UpdateKeyByIndex(int index, KeyCode oldKey,  KeyCode newKey)
    {
        m_uiTextList[index].text = newKey.ToString();

        TextEffect.Show(oldKey.ToString(), m_uiTextList[index].transform.position);
    }
}
