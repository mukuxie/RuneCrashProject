using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoIndex : MonoBehaviour
{
    public Button goindexBtn;

    private void Awake()
    {
        goindexBtn.onClick.AddListener(() =>
        {
            // 登录按钮被点击，进入Game场景
            SceneManager.LoadScene(0);
        });
    }
}
