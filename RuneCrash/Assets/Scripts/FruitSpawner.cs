﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FruitSpawner : MonoBehaviour
{
    /// <summary>
    /// 水果预设
    /// </summary>
    public GameObject[] fruitPrefabs;

    /// <summary>
    /// 生成出来的水果二维数组
    /// </summary>
    public ArrayList fruitList;

    /// <summary>
    /// 水果的根节点
    /// </summary>
    private Transform m_fruitRoot;

    /// <summary>
    /// 被选中的水果
    /// </summary>
    private FruitItem m_curSelectFruit;

    /// <summary>
    /// 手指水平滑动量
    /// </summary>
    private float m_fingerMoveX;
    /// <summary>
    /// 手指竖直滑动量
    /// </summary>
    private float m_fingerMoveY;

    /// <summary>
    /// 需要消除掉的水果数组
    /// </summary>
    private ArrayList m_matchFruits;

    /// <summary>
    /// 玩家可以滑動了嗎
    /// </summary>
    private bool m_canSlide = false;

    private void Awake()
    {
        m_fruitRoot = transform;
        EventDispatcher.instance.Regist(EventDef.EVENT_FRUIT_SELECTED, OnFruitSelected);

        ChangeRunePrefab();
    }

    private void OnDestroy()
    {
        EventDispatcher.instance.UnRegist(EventDef.EVENT_FRUIT_SELECTED, OnFruitSelected);
    }

    /// <summary>
    /// 依照傳進來的資料更新盧恩符文圖案
    /// </summary>
    private void ChangeRunePrefab()
    {
        try
        {
            for (int i = 0; i < fruitPrefabs.Length; i++)
            {
                fruitPrefabs[i] = RuneListContainer.Instance.opRuneData.runeList[i];
            }
        }
        catch
        {
            print("<color='red'>DEBUG MODE...</color> NOT START FROM MAIN MENU.");
        }
    }

    /// <summary>
    /// 水果被点击
    /// </summary>
    private void OnFruitSelected(params object[] args)
    {
        // 把被点击的水果对象缓存起来
        m_curSelectFruit = args[0] as FruitItem;
    }

    private void Start()
    {
        SpawnFruitArrayList();

        // 首次生成水果后，执行一次自动检测
        StartCoroutine(AutoMatchAgain());

    }

    private void Update()
    {
        if (null == m_curSelectFruit) return;
        if (Input.GetMouseButtonUp(0))
        {
            // 手指抬起，释放当前选中的水果对象
            m_curSelectFruit = null;
            return;
        }


#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(0))
#else
        if(1 == Input.touchCount && Input.touches[0].phase == TouchPhase.Moved)
#endif
        {
            m_fingerMoveX = Input.GetAxis("Mouse X");
            m_fingerMoveY = Input.GetAxis("Mouse Y");
        }


        // 滑动量太小，不处理
        if (Mathf.Abs(m_fingerMoveX) < 0.3f && Mathf.Abs(m_fingerMoveY) < 0.3f)
            return;
        // 關閉滑動，不處理
        if (!m_canSlide)
            return;
        OnFruitMove();

        // 關閉滑動
        m_canSlide = false;
        m_fingerMoveX = 0;
        m_fingerMoveY = 0;
    }

    /// <summary>
    /// 水果滑动响应
    /// </summary>
    private void OnFruitMove()
    {
        if (Mathf.Abs(m_fingerMoveX) > Mathf.Abs(m_fingerMoveY))
        {
            //拋出Combo清除
            EventDispatcher.instance.DispatchEvent(EventDef.EVENT_SET_COMBO, "Clean");
            //横向滑动
            var targetItem = GetFruitItem(m_curSelectFruit.rowIndex, m_curSelectFruit.columIndex + (m_fingerMoveX > 0 ? 1 : -1));
            if (null != targetItem)
            {
                StartCoroutine(ExchangeAndMatch(m_curSelectFruit, targetItem));
            }
            else
            {
                m_curSelectFruit = null;
            }
        }
        else if (Mathf.Abs(m_fingerMoveX) < Mathf.Abs(m_fingerMoveY))
        {
            //拋出Combo清除
            EventDispatcher.instance.DispatchEvent(EventDef.EVENT_SET_COMBO, "Clean");
            //纵向滑动
            var targetItem = GetFruitItem(m_curSelectFruit.rowIndex + (m_fingerMoveY > 0 ? 1 : -1), m_curSelectFruit.columIndex);
            if (null != targetItem)
            {
                StartCoroutine(ExchangeAndMatch(m_curSelectFruit, targetItem));
            }
            else
            {
                m_curSelectFruit = null;
            }
        }

    }

    /// <summary>
    /// 根据行号列号获取水果对象
    /// </summary>
    private FruitItem GetFruitItem(int rowIndex, int columIndex)
    {
        if (rowIndex < 0 || rowIndex >= fruitList.Count) return null;
        var temp = fruitList[rowIndex] as ArrayList;
        if (columIndex < 0 || columIndex >= temp.Count) return null;
        return temp[columIndex] as FruitItem;
    }

    /// <summary>
    /// 根据行号列号设置水果对象
    /// </summary>
    private void SetFruitItem(int rowIndex, int columIndex, FruitItem item)
    {
        var temp = fruitList[rowIndex] as ArrayList;
        temp[columIndex] = item;
    }


    /// <summary>
    /// 交换水果并检测是否可以消除
    /// </summary>
    IEnumerator ExchangeAndMatch(FruitItem item1, FruitItem item2)
    {
        Exchange(item1, item2);
        yield return new WaitForSeconds(0.3f);
        if (CheckHorizontalMatch() || CheckVerticalMatch())
        {
            // 消除匹配的水果
            RemoveMatchFruit();
            yield return new WaitForSeconds(0.2f);

            //上面的水果掉落下来，
            DropDownOtherFruit();

            m_matchFruits = new ArrayList();

            yield return new WaitForSeconds(0.6f);
            // 再次执行一次检测
            StartCoroutine(AutoMatchAgain());
        }
        else
        {
            // 没有任何水果匹配，交换回来
            Exchange(item1, item2);

            // 恢復滑動
            m_canSlide = true;
        }
    }

    /// <summary>
    /// 自动递归检测水果消除
    /// </summary>
    /// <returns></returns>
    IEnumerator AutoMatchAgain()
    {
        // 每次檢查水果消除之前都檢查一次 可不可以消除
        if (CheckDeadEnd())
        {
            ResetAllFruit();
            yield break;
        }

        if (CheckHorizontalMatch() || CheckVerticalMatch())
        {
            RemoveMatchFruit();
            yield return new WaitForSeconds(0.2f);
            DropDownOtherFruit();

            m_matchFruits = new ArrayList();

            yield return new WaitForSeconds(0.6f);

            StartCoroutine(AutoMatchAgain());
        }

        // 恢復滑動
        m_canSlide = true;
    }

    /// <summary>
    /// 交换水果
    /// </summary>
    private void Exchange(FruitItem item1, FruitItem item2)
    {

        SetFruitItem(item1.rowIndex, item1.columIndex, item2);
        SetFruitItem(item2.rowIndex, item2.columIndex, item1);

        // print(item1 + ", " + item2);

        int tmp = 0;
        tmp = item1.rowIndex;
        item1.rowIndex = item2.rowIndex;
        item2.rowIndex = tmp;

        tmp = item1.columIndex;
        item1.columIndex = item2.columIndex;
        item2.columIndex = tmp;

        item1.UpdatePosition(item1.rowIndex, item1.columIndex, true);
        item2.UpdatePosition(item2.rowIndex, item2.columIndex, true);

        m_curSelectFruit = null;
    }


    /// <summary>
    /// 横线检测水果
    /// </summary>
    private bool CheckHorizontalMatch()
    {
        bool isMatch = false;
        for (int rowIndex = 0; rowIndex < GlobalDef.ROW_COUNT; ++rowIndex)
        {
            for (int columIndex = 0; columIndex < GlobalDef.COLUM_COUNT - 2; ++columIndex)
            {
                var item1 = GetFruitItem(rowIndex, columIndex);
                var item2 = GetFruitItem(rowIndex, columIndex + 1);
                var item3 = GetFruitItem(rowIndex, columIndex + 2);
                if (item1.fruitType == item2.fruitType && item2.fruitType == item3.fruitType)
                {
                    isMatch = true;
                    AddMatchFruit(item1);
                    AddMatchFruit(item2);
                    AddMatchFruit(item3);
                }
            }
        }
        return isMatch;
    }

    /// <summary>
    /// 纵向检测水果
    /// </summary>
    /// <returns></returns>
    private bool CheckVerticalMatch()
    {
        bool isMatch = false;
        for (int columIndex = 0; columIndex < GlobalDef.COLUM_COUNT; ++columIndex)
        {
            for (int rowIndex = 0; rowIndex < GlobalDef.ROW_COUNT - 2; ++rowIndex)
            {
                var item1 = GetFruitItem(rowIndex, columIndex);
                var item2 = GetFruitItem(rowIndex + 1, columIndex);
                var item3 = GetFruitItem(rowIndex + 2, columIndex);
                if (item1.fruitType == item2.fruitType && item2.fruitType == item3.fruitType)
                {
                    isMatch = true;
                    AddMatchFruit(item1);
                    AddMatchFruit(item2);
                    AddMatchFruit(item3);
                }
            }
        }
        return isMatch;
    }

    /// <summary>
    /// 添加匹配的水果到缓存中（匹配的水果会被消除掉)
    /// </summary>
    private void AddMatchFruit(FruitItem item)
    {
        if (null == m_matchFruits)
            m_matchFruits = new ArrayList();
        int index = m_matchFruits.IndexOf(item);
        if (-1 == index)
            m_matchFruits.Add(item);
    }

    /// <summary>
    /// 消除水果
    /// </summary>
    private void RemoveMatchFruit()
    {
        for (int i = 0; i < m_matchFruits.Count; ++i)
        {
            var item = m_matchFruits[i] as FruitItem;
            item.DestroyFruitBg();
        }

        if (m_matchFruits.Count > 0)
        {
            //拋出COMBO++
            EventDispatcher.instance.DispatchEvent(EventDef.EVENT_SET_COMBO, "Add");
        }
    }

    /// <summary>
    /// 消除掉的水果上方的水果下落
    /// </summary>
    private void DropDownOtherFruit()
    {
        for (int i = 0; i < m_matchFruits.Count; ++i)
        {
            var fruit = m_matchFruits[i] as FruitItem;
            for (int j = fruit.rowIndex + 1; j < GlobalDef.ROW_COUNT; ++j)
            {
                var dropdownFruit = GetFruitItem(j, fruit.columIndex);
                dropdownFruit.rowIndex--;
                SetFruitItem(dropdownFruit.rowIndex, dropdownFruit.columIndex, dropdownFruit);
                dropdownFruit.UpdatePosition(dropdownFruit.rowIndex, dropdownFruit.columIndex, true);
            }
            ReuseRemovedFruit(fruit);
        }
    }

    /// <summary>
    /// 复用被消除的水果，作为新水果放到顶部
    /// </summary>
    private void ReuseRemovedFruit(FruitItem fruit)
    {
        // 随机一个水果类型
        var fruitType = Random.Range(0, fruitPrefabs.Length);
        fruit.rowIndex = GlobalDef.ROW_COUNT;
        fruit.CreateFruitBg(fruitType, fruitPrefabs[fruitType]);
        // 先放到最顶部外面
        fruit.UpdatePosition(fruit.rowIndex, fruit.columIndex);
        // 让其下落一格
        fruit.rowIndex--;
        SetFruitItem(fruit.rowIndex, fruit.columIndex, fruit);
        fruit.UpdatePosition(fruit.rowIndex, fruit.columIndex, true);
    }

    /// <summary>
    /// 生成多行多列水果
    /// </summary>
    private void SpawnFruitArrayList()
    {
        fruitList = new ArrayList();
        for (int rowIndex = 0; rowIndex < GlobalDef.ROW_COUNT; ++rowIndex)
        {
            ArrayList temp = new ArrayList();

            for (int columIndex = 0; columIndex < GlobalDef.COLUM_COUNT; ++columIndex)
            {
                var type = Random.Range(0, fruitPrefabs.Length);
                int[] cantUseType = new int[2];
                cantUseType[0] = ReturnCantUseCol(temp, type, columIndex);
                cantUseType[1] = ReturnCantUseRow(rowIndex, columIndex);

                while (cantUseType[0] == type || cantUseType[1] == type)
                {
                    type = Random.Range(0, fruitPrefabs.Length);
                }

                var item = AddRandomFruitItem(rowIndex, columIndex, type);
                //一顆一單位
                temp.Add(item);
            }

            //一排一單位
            fruitList.Add(temp);
        }
    }

    int ReturnCantUseCol(ArrayList temp, int type, int columIndex)
    {
        if (columIndex - 2 < 0) { return 99; }
        var a = temp[columIndex - 2] as FruitItem;
        var b = temp[columIndex - 1] as FruitItem;
        if (a.fruitType == b.fruitType)
        {
            return b.fruitType;
        }

        return 99;
    }

    int ReturnCantUseRow(int row, int col)
    {
        if (row - 2 < 0) { return 99; }
        var a = GetFruitItem(row - 2, col);
        var b = GetFruitItem(row - 1, col);
        if (a.fruitType == b.fruitType)
        {
            return b.fruitType;
        }

        return 99;
    }

    /// <summary>
    /// 随机一个水果
    /// </summary>
    private FruitItem AddRandomFruitItem(int rowIndex, int columIndex, int type)
    {
        // 随机一个水果类型
        var fruitType = type;
        var item = new GameObject("item");
        item.transform.SetParent(m_fruitRoot, false);
        item.AddComponent<BoxCollider2D>().size = Vector2.one * GlobalDef.CELL_SIZE;
        var bhv = item.AddComponent<FruitItem>();
        bhv.UpdatePosition(rowIndex, columIndex);
        bhv.CreateFruitBg(fruitType, fruitPrefabs[fruitType]);
        return bhv;
    }

    // NkE1

    /// <summary>
    /// 重製所有版面
    /// </summary>
    public void ResetAllFruit()
    {
        for (int i = m_fruitRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(m_fruitRoot.GetChild(i).gameObject);
        }

        SpawnFruitArrayList();

        StartCoroutine(AutoMatchAgain());
    }

    #region 檢查死局
    /// <summary>
    /// 檢查版面是否沒東西可消
    /// </summary>
    private bool CheckDeadEnd()
    {
        // 往右邊旋轉
        for (int i = 0; i < GlobalDef.ROW_COUNT; ++i)
        {
            for (int j = 0; j < GlobalDef.COLUM_COUNT - 1; ++j)
            {
                var item1 = GetFruitItem(i, j) as FruitItem;
                var item2 = GetFruitItem(i, j + 1) as FruitItem;

                ExchangeWithoutUpdate(item1, item2);

                if (CheckHorizontalMatch() || CheckVerticalMatch())
                {
                    // 刪除m_matchFruits
                    m_matchFruits = new ArrayList();

                    ExchangeWithoutUpdate(item1, item2);

                    // 還有未消方塊 return
                    print("Continued");
                    return false;
                }
                ExchangeWithoutUpdate(item1, item2);
            }
        }

        for (int i = 0; i < GlobalDef.ROW_COUNT - 1; ++i)
        {
            for (int j = 0; j < GlobalDef.COLUM_COUNT; ++j)
            {
                var item1 = GetFruitItem(i, j) as FruitItem;
                var item2 = GetFruitItem(i + 1, j) as FruitItem;

                ExchangeWithoutUpdate(item1, item2);

                if (CheckHorizontalMatch() || CheckVerticalMatch())
                {
                    // 刪除m_matchFruits
                    m_matchFruits = new ArrayList();

                    ExchangeWithoutUpdate(item1, item2);

                    // 還有未消方塊
                    print("Continued");
                    return false;
                }
                ExchangeWithoutUpdate(item1, item2);
            }
        }

        print("U Failer");
        return true;
    }

    /// <summary>
    /// 內部ITEM交換不更改圖片
    /// </summary>
    private void ExchangeWithoutUpdate(FruitItem item1, FruitItem item2)
    {
        SetFruitItem(item1.rowIndex, item1.columIndex, item2);
        SetFruitItem(item2.rowIndex, item2.columIndex, item1);

        int tmp = 0;
        tmp = item1.rowIndex;
        item1.rowIndex = item2.rowIndex;
        item2.rowIndex = tmp;

        tmp = item1.columIndex;
        item1.columIndex = item2.columIndex;
        item2.columIndex = tmp;

        // item1.UpdatePosition(item1.rowIndex, item1.columIndex, true);
        // item2.UpdatePosition(item2.rowIndex, item2.columIndex, true);
    }

    /// <summary>
    /// 自动递归检测水果消除
    /// </summary>
    /// <returns></returns>
    IEnumerator FirstMatch()
    {
        // 每次檢查水果消除之前都檢查一次 可不可以消除
        if (CheckDeadEnd())
        {
            ResetAllFruit();
            yield break;
        }

        if (CheckHorizontalMatch())
        {
            RemoveMatchFruit();
            yield return new WaitForSeconds(0.2f);
            DropDownOtherFruit();

            m_matchFruits = new ArrayList();

            yield return new WaitForSeconds(0.6f);

            StartCoroutine(AutoMatchAgain());
        }

        if (CheckVerticalMatch())
        {
            
        }

        // 恢復滑動
        m_canSlide = true;
    }


    #endregion
}

