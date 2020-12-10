using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverDlg : MonoBehaviour
{
    public Text scoreText;
    public Button backBtn;
    public Button restartBtn;

    private void Awake()
    {
        backBtn.onClick.AddListener(() => {
            SceneManager.LoadScene(0);
        });
        restartBtn.onClick.AddListener(() => {
            EventDispatcher.Instance.DispatchEvent(EventNameDef.EVENT_RESTART_GAME);
            Hide();
        });
    }

    public void Show(int score)
    {
        gameObject.SetActive(true);
        scoreText.text = "本次得分" + score;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }


}
