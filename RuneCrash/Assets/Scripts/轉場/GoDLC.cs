using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoDLC : MonoBehaviour
{
    public Button godlcBtn;

    private void Awake()
    {
        godlcBtn.onClick.AddListener(() =>
        {
            // 登录按钮被点击，进入Game场景
            SceneManager.LoadScene(4);
        });
    }
}

