using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GamePanel : MonoBehaviour
{
    /// <summary>
    /// 总得分Text
    /// </summary>
    public TextMeshProUGUI totalScoreText;
    /// <summary>
    /// home按钮
    /// </summary>
    public Button homeBtn;
    /// <summary>
    /// COMBO
    /// </summary>
    public TextMeshProUGUI nowComboText;


    /// <summary>
    /// 总得分
    /// </summary>
    private int m_totalScore;

    /// <summary>
    /// 連擊數
    /// </summary>
    private int m_combo;

    private void Awake()
    {
        // 注册加分事件
        EventDispatcher.instance.Regist(EventDef.EVENT_ADD_SCORE, OnAddScore);
        EventDispatcher.instance.Regist(EventDef.EVENT_SET_COMBO, OnSetCombo);
    }

    private void OnDestroy()
    {
        // 注销加分事件
        EventDispatcher.instance.UnRegist(EventDef.EVENT_ADD_SCORE, OnAddScore);
        EventDispatcher.instance.UnRegist(EventDef.EVENT_SET_COMBO, OnSetCombo);
    }

    /// <summary>
    /// 加分事件
    /// </summary>
    private void OnAddScore(params object[] args)
    {
        // 更新总分显示
        m_totalScore += (int)args[0] * MultiplyCombo();
        totalScoreText.text = m_totalScore.ToString();
    }

    /// <summary>
    /// 加Combo事件
    /// </summary>
    private void OnSetCombo(params object[] args)
    {
        switch ((string)args[0])
        {
            case "Clean":
                m_combo = 0;
                break;

            case "Add":
                m_combo ++;
                break;

            default:
                break;
        }
        nowComboText.text = m_combo.ToString();
    }

    public int MultiplyCombo()
    {
        if (m_combo >= 9)
        {
            return 5;
        }
        else if (m_combo >= 4)
        {
            return 4;
        }
        else if (m_combo >= 2)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }
}
