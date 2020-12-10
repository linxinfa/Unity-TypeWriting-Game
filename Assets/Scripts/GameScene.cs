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
        EventDispatcher.Instance.Regist(EventNameDef.EVENT_KEY_BINGO_INDEX, OnEventKeyBingoIndex);
        EventDispatcher.Instance.Regist(EventNameDef.EVENT_COMBO, OnEventCombo);
        EventDispatcher.Instance.Regist(EventNameDef.EVENT_PLAY_ANI, OnEventPlayAni);
        EventDispatcher.Instance.Regist(EventNameDef.EVENT_UPDATE_SCORE, OnEventUpdateScore);
        EventDispatcher.Instance.Regist(EventNameDef.EVENT_RESTART_GAME, OnEventRestartGame);
        EventDispatcher.Instance.Regist(EventNameDef.EVENT_GAMEOVER, OnEventGameOver);

        m_aniCtrler = new CharacterAniCtrler();
        m_aniCtrler.Init(anitor);

        StartGame();
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.UnRegist(EventNameDef.EVENT_KEY_BINGO_INDEX, OnEventKeyBingoIndex);
        EventDispatcher.Instance.UnRegist(EventNameDef.EVENT_COMBO, OnEventCombo);
        EventDispatcher.Instance.UnRegist(EventNameDef.EVENT_PLAY_ANI, OnEventPlayAni);
        EventDispatcher.Instance.UnRegist(EventNameDef.EVENT_UPDATE_SCORE, OnEventUpdateScore);
        EventDispatcher.Instance.UnRegist(EventNameDef.EVENT_RESTART_GAME, OnEventRestartGame);
        EventDispatcher.Instance.UnRegist(EventNameDef.EVENT_GAMEOVER, OnEventGameOver);
    }

    private void StartGame()
    {
        GameMgr.Instance.Init();
        TextEffect.Init();
        bloodSlider.maxValue = GameMgr.Instance.blood;
        bloodSlider.value = GameMgr.Instance.blood;
        bloodImage.enabled = true;
        keyGrid.CreateKeyList(GameMgr.Instance.keyList);

        comboText.gameObject.SetActive(false);
        scoreText.text = "0";
        gameOverDlg.Hide();
    }

    public KeyCode getKeyDownCode()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    //Debug.Log(keyCode.ToString());
                    return keyCode;
                }
            }
        }
        return KeyCode.None;
    }

    void Update()
    {
        if (GameMgr.Instance.gameOver) return;
        GameMgr.Instance.UpdateComboTimer();

        bloodSlider.value = GameMgr.Instance.blood;
        GameMgr.Instance.blood -= GameMgr.Instance.hardLevel;

        var keyCode = getKeyDownCode();
        if (KeyCode.None == keyCode) return;
        GameMgr.Instance.OnKey(keyCode);
    }

    private void LateUpdate()
    {
        m_aniCtrler.LateUpdate();
    }

    private void OnEventKeyBingoIndex(params object[] args)
    {
        int index = (int)args[0];
        KeyCode oldKey = (KeyCode)args[1];
        KeyCode newKey = (KeyCode)args[2];
        keyGrid.UpdateKeyByIndex(index, oldKey, newKey);
    }

    private void OnEventCombo(params object[] args)
    {
        var combo = (int)args[0];
        comboText.text = "连击" + combo;
        comboText.gameObject.SetActive(combo >= 3);
    }

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

    private void OnEventUpdateScore(params object[] args)
    {
        var score = (int)args[0];
        scoreText.text = score.ToString();
    }

    private void OnEventGameOver(params object[] args)
    {
        bloodImage.enabled = false;
        gameOverDlg.Show(GameMgr.Instance.score);
    }

    private void OnEventRestartGame(params object[] args)
    {
        m_aniCtrler.PlayReviveImmediately();
        StartGame();
    }
}
