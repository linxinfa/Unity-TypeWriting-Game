using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EntryScene : MonoBehaviour
{
    public Button startGameBtn;
    public ToggleGroup tglGroup;


    void Start()
    {
        startGameBtn.onClick.AddListener(() =>
        {
            // 根据勾选，缓存难度等级
            foreach (var item in tglGroup.ActiveToggles())
            {
                GameMgr.Instance.hardLevel = int.Parse(item.name);
                break;
            }

            // 进入Game场景
            SceneManager.LoadScene(1);
        });
    }


}
