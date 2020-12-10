// CharacterAniLogic.cs

using UnityEngine;
using System.Collections.Generic;

// 角色动画逻辑
public class CharacterAniCtrler
{
    private const string STR_SPEED = "Speed";
    private const string STR_ACTION = "Action";


    private const string STR_IDLE = "idle";
    private const string STR_WALK = "walk";
    private const string STR_RUN = "run";
    private const string STR_DEATH = "die";

    /// <summary>
    /// 角色动画Animator组件
    /// </summary>
    private Animator m_animator;

    /// <summary>
    /// 动画队列
    /// </summary>
    private Queue<int> m_animQueue = new Queue<int>();
    private AnimatorClipInfo[] mClips = null;

    /// <summary>
    /// 是否阵亡
    /// </summary>
    public bool IsDeath = false;

    public void Init(Animator ani)
    {
        m_animator = ani;
    }

    /// <summary>
    /// 每帧调用
    /// </summary>
    public void LateUpdate()
    {
        if (m_animator == null)
        {
            return;
        }
        if (!m_animator.isInitialized || m_animator.IsInTransition(0))
        {
            return;
        }
        if (null == mClips)
            mClips = m_animator.GetCurrentAnimatorClipInfo(0);
        if (null == mClips || mClips.Length == 0)
            return;

        int actionID = m_animator.GetInteger(STR_ACTION);
        if (actionID > 0)
        {
            //将Action复位
            m_animator.SetInteger(STR_ACTION, 0);
        }

        //将剩余队列的动作重新拿出来播放
        PlayRemainAction();
    }

    /// <summary>
    /// 将剩余队列的动作重新拿出来播放
    /// </summary>
    void PlayRemainAction()
    {
        if (IsDeath) return;
        if (m_animQueue.Count > 0)
        {
            PlayAnimation(m_animQueue.Dequeue());
        }
    }

    /// <summary>
    /// 立即播放阵亡动画
    /// </summary>
    public void PlayDieImmediately()
    {
        PlayAniImmediately(STR_DEATH);
        ClearAnimQueue();
        IsDeath = true;
    }

    /// <summary>
    /// 立即播放复活动画
    /// </summary>
    public void PlayReviveImmediately()
    {
        IsDeath = false;
        ClearAnimQueue();
        PlayAniImmediately(STR_IDLE);
    }

    /// <summary>
    /// 立即播放某个动画
    /// </summary>
    /// <param name="name">动画名称</param>
    private void PlayAniImmediately(string name)
    {
        if (IsDeath) return;
        m_animator.CrossFade(name, 0.1f, 0);
    }

    /// <summary>
    /// 播放不同动作ID
    /// </summary>
    /// <param name="isJump"></param>
    /// 
    public void PlayAnimation(int actionID)
    {
        if (IsDeath) return;
        if (m_animator == null)
            return;
        if (!m_animator.isInitialized || m_animator.IsInTransition(0))
        {
            // 如果正在过渡，则先塞到队列中
            m_animQueue.Enqueue(actionID);
            return;
        }
        m_animator.SetInteger(STR_ACTION, actionID);
    }

    public void PlayWalk()
    {
        PlayAniImmediately(STR_WALK);
    }

    public void PlayRun()
    {
        PlayAniImmediately(STR_RUN);
    }

    /// <summary>
    /// 清除缓存的行为队列
    /// </summary>
    public void ClearAnimQueue()
    {
        if (m_animQueue.Count > 0)
        {
            m_animQueue.Clear();
        }
    }
}

/// <summary>
/// 状态定义，默认为Idle状态。
/// </summary>
public enum CharacterAniId
{
    Idle = 1,
    Walk = 2,
    Run = 3,


    Hit = 4,
    Death = 5,
}
