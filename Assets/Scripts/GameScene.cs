using System;
using UnityEngine;
using UnityEngine.UI;


public class GameScene : MonoBehaviour
{

    public Animator anitor;
    public Text comboText;
    public Text scoreText;
    public Slider bloodSlider;
    public Image bloodImage;
    public GameOverDlg gameOverDlg;
    public KeyGrid keyGrid;

    private CharacterAniCtrler m_aniCtrler;


    private void Awake()
    {
        // 注册事件
        EventDispatcher.Instance.Regist(EventNameDef.EVENT_KEY_BINGO_INDEX, OnEventKeyBingoIndex);
        EventDispatcher.Instance.Regist(EventNameDef.EVENT_COMBO, OnEventCombo);
        EventDispatcher.Instance.Regist(EventNameDef.EVENT_PLAY_ANI, OnEventPlayAni);
        EventDispatcher.Instance.Regist(EventNameDef.EVENT_UPDATE_SCORE, OnEventUpdateScore);
        EventDispatcher.Instance.Regist(EventNameDef.EVENT_RESTART_GAME, OnEventRestartGame);
        EventDispatcher.Instance.Regist(EventNameDef.EVENT_GAMEOVER, OnEventGameOver);

        m_aniCtrler = new CharacterAniCtrler();
        m_aniCtrler.Init(anitor);

        // 开始游戏
        StartGame();
    }

    private void OnDestroy()
    {
        // 注销事件
        EventDispatcher.Instance.UnRegist(EventNameDef.EVENT_KEY_BINGO_INDEX, OnEventKeyBingoIndex);
        EventDispatcher.Instance.UnRegist(EventNameDef.EVENT_COMBO, OnEventCombo);
        EventDispatcher.Instance.UnRegist(EventNameDef.EVENT_PLAY_ANI, OnEventPlayAni);
        EventDispatcher.Instance.UnRegist(EventNameDef.EVENT_UPDATE_SCORE, OnEventUpdateScore);
        EventDispatcher.Instance.UnRegist(EventNameDef.EVENT_RESTART_GAME, OnEventRestartGame);
        EventDispatcher.Instance.UnRegist(EventNameDef.EVENT_GAMEOVER, OnEventGameOver);
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    private void StartGame()
    {
        GameMgr.Instance.Init();
        TextEffect.Init();
        // 初始化血量
        bloodSlider.maxValue = GameMgr.Instance.blood;
        bloodSlider.value = GameMgr.Instance.blood;
        bloodImage.enabled = true;
        // 生成字母盘
        keyGrid.CreateKeyList(GameMgr.Instance.keyList);

        comboText.gameObject.SetActive(false);
        scoreText.text = "0";
        gameOverDlg.Hide();
    }

   

    void Update()
    {
        if (GameMgr.Instance.gameOver) return;
        // 更新连击定时器
        GameMgr.Instance.UpdateComboTimer();

        // 更新血量ui
        bloodSlider.value = GameMgr.Instance.blood;
        GameMgr.Instance.blood -= GameMgr.Instance.hardLevel;

        // 按键判断
        var keyCode = GameMgr.Instance.GetKeyDownCode();
        if (KeyCode.None == keyCode) return;
        GameMgr.Instance.OnKey(keyCode);
    }

    private void LateUpdate()
    {
        // 更新动画控制器
        m_aniCtrler.LateUpdate();
    }

    /// <summary>
    /// 按键正确事件
    /// </summary>
    /// <param name="args"></param>
    private void OnEventKeyBingoIndex(params object[] args)
    {
        int index = (int)args[0];
        KeyCode oldKey = (KeyCode)args[1];
        KeyCode newKey = (KeyCode)args[2];
        keyGrid.UpdateKeyByIndex(index, oldKey, newKey);
    }

    /// <summary>
    /// 连击事件
    /// </summary>
    /// <param name="args"></param>
    private void OnEventCombo(params object[] args)
    {
        var combo = (int)args[0];
        comboText.text = "连击" + combo;
        comboText.gameObject.SetActive(combo >= 3);
    }

    /// <summary>
    /// 播放动画事件
    /// </summary>
    /// <param name="args"></param>
    private void OnEventPlayAni(params object[] args)
    {
        var ani = (string)args[0];
        switch (ani)
        {
            case "idle": m_aniCtrler.PlayAnimation((int)CharacterAniId.Idle); break;
            case "walk": GameMgr.Instance.comboTimer = 0.5f; m_aniCtrler.PlayWalk(); break;
            case "run": GameMgr.Instance.comboTimer = 0.5f; m_aniCtrler.PlayRun(); break;
            case "hit": m_aniCtrler.PlayAnimation((int)CharacterAniId.Hit); break;
            case "die": m_aniCtrler.PlayDieImmediately(); break;
        }
    }

    /// <summary>
    /// 更新得分事件
    /// </summary>
    /// <param name="args"></param>
    private void OnEventUpdateScore(params object[] args)
    {
        var score = (int)args[0];
        scoreText.text = score.ToString();
    }

    /// <summary>
    /// 游戏结束事件
    /// </summary>
    /// <param name="args"></param>
    private void OnEventGameOver(params object[] args)
    {
        bloodImage.enabled = false;
        gameOverDlg.Show(GameMgr.Instance.score);
    }

    /// <summary>
    /// 重新开始游戏事件
    /// </summary>
    /// <param name="args"></param>
    private void OnEventRestartGame(params object[] args)
    {
        m_aniCtrler.PlayReviveImmediately();
        StartGame();
    }
}
