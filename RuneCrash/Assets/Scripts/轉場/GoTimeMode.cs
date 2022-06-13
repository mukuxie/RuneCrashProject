using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoTimeMode : MonoBehaviour
{
    public Button goTimeBtn;

    private void Awake()
    {
        goTimeBtn.onClick.AddListener(() =>
        {
            // 登录按钮被点击，进入Game场景
            SceneManager.LoadScene(3);
        });
    }
}
