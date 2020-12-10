using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

/// <summary>
/// 游戏管理
/// </summary>
public class GameMgr
{
    public void Init()
    {
        score = 0;
        blood = MAX_BLOOD;
        comboCnt = 0;
        comboTimer = 0;
        gameOver = false;
        m_keyList.Clear();

        GenKeys();
    }

    /// <summary>
    /// 生成字母盘
    /// </summary>
    private void GenKeys()
    {
        for (int i = 0; i < 16; ++i)
        {
            m_keyList.Add(GenOneKey());
        }
    }

    /// <summary>
    /// 生成一个字母
    /// </summary>
    /// <returns></returns>
    private KeyCode GenOneKey()
    {
        var key = (KeyCode)UnityEngine.Random.Range((int)KeyCode.A, (int)KeyCode.Z);
        for(int i=0,cnt=m_keyList.Count;i<cnt;++i)
        {
            if(m_keyList[i] == key)
            {
                // 如果生成的字母已存在，则递归生成
                return GenOneKey();
            }
        }
        return key;
    }

    /// <summary>
    /// 获取按键类型
    /// </summary>
    /// <returns></returns>
    public KeyCode GetKeyDownCode()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    return keyCode;
                }
            }
        }
        return KeyCode.None;
    }

    /// <summary>
    /// 按键按下
    /// </summary>
    /// <param name="pressKey"></param>
    /// <returns></returns>
    public bool OnKey(KeyCode pressKey)
    {
        var bingoIndex = IsKeyBingo(pressKey);
        if (-1 != bingoIndex)
        {
            OnKeyBingo(bingoIndex);
            return true;
        }
        else
        {
            OnKeyError();
            return false;
        }
    }

    /// <summary>
    /// 判断按键是否在字母盘中
    /// </summary>
    /// <param name="key">按键</param>
    /// <returns></returns>
    private int IsKeyBingo(KeyCode key)
    {
        for (int i = 0, cnt = m_keyList.Count; i < cnt; ++i)
        {
            if (m_keyList[i] == key)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// 按键正确
    /// </summary>
    private void OnKeyBingo(int bingoIndex)
    {
        // 加连击
        ++comboCnt;

        if (comboCnt >= 3)
        {
            // 加血加分，连击加持
            blood += 150;
            if (blood > MAX_BLOOD)
                blood = MAX_BLOOD;
            score += 20;
            EventDispatcher.Instance.DispatchEvent(EventNameDef.EVENT_PLAY_ANI, "run");
        }
        else
        {
            // 加血加分
            blood += 50;
            score += 10;
            EventDispatcher.Instance.DispatchEvent(EventNameDef.EVENT_PLAY_ANI, "walk");
        }

        // 生成新的字母
        var oldKey = m_keyList[bingoIndex];
        var newKey = GenOneKey();
        m_keyList[bingoIndex] = newKey;

        EventDispatcher.Instance.DispatchEvent(EventNameDef.EVENT_KEY_BINGO_INDEX, bingoIndex, oldKey, newKey);
        EventDispatcher.Instance.DispatchEvent(EventNameDef.EVENT_COMBO, comboCnt);
        EventDispatcher.Instance.DispatchEvent(EventNameDef.EVENT_UPDATE_SCORE, score);
    }

    /// <summary>
    /// 按键错误
    /// </summary>
    private void OnKeyError()
    {
        // 连击中断
        comboCnt = 0;
        // 扣血
        blood -= 30;
        EventDispatcher.Instance.DispatchEvent(EventNameDef.EVENT_COMBO, comboCnt);
        EventDispatcher.Instance.DispatchEvent(EventNameDef.EVENT_PLAY_ANI, "hit");
    }

    /// <summary>
    /// 连击定时器
    /// </summary>
    public void UpdateComboTimer()
    {
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            // 连击断开
            if (comboTimer <= 0)
            {
                EventDispatcher.Instance.DispatchEvent(EventNameDef.EVENT_PLAY_ANI, "idle");
                comboCnt = 0;
                EventDispatcher.Instance.DispatchEvent(EventNameDef.EVENT_COMBO, comboCnt);
            }
        }
    }

    /// <summary>
    /// 难度等级
    /// </summary>
    public int hardLevel { get; set; }

    /// <summary>
    /// 得分
    /// </summary>
    public int score { get; set; }

    private const int MAX_BLOOD = 1500;
    /// <summary>
    /// 血量
    /// </summary>
    public int blood
    {
        get { return m_blood; }
        set
        {
            m_blood = value;
            if (m_blood <= 0)
            {
                gameOver = true;
                EventDispatcher.Instance.DispatchEvent(EventNameDef.EVENT_PLAY_ANI, "die");
                EventDispatcher.Instance.DispatchEvent(EventNameDef.EVENT_GAMEOVER);
            }
        }
    }
    private int m_blood = 0;

    /// <summary>
    /// 连击数量
    /// </summary>
    public int comboCnt { get; set; }
    /// <summary>
    /// 连击定时器
    /// </summary>
    public float comboTimer { get; set; }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public bool gameOver { get; private set; }

    /// <summary>
    /// 按键列表
    /// </summary>
    public List<KeyCode> keyList { get { return m_keyList; } }
    private List<KeyCode> m_keyList = new List<KeyCode>();



    /// <summary>
    /// 单例模式
    /// </summary>
    private static GameMgr s_instance;
    public static GameMgr Instance
    {
        get
        {
            if (null == s_instance)
                s_instance = new GameMgr();
            return s_instance;
        }
    }
}
