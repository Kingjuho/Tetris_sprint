using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    // 싱글톤
    public static UIManager Instance { get; private set; }

    // 뷰어
    public TetrominoViewer holdViewer;
    public TetrominoViewer[] nextViewers;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /** Hold UI 갱신 **/
    public void UpdateHold(TetrominoType type) => holdViewer.SetTetromino(type);

    /** Next UI 갱신 **/
    public void UpdateNext(List<TetrominoType> nextList)
    {
        // 보여줄 수 있는 뷰어 개수만큼만 반복
        for (int i = 0; i < nextViewers.Length; i++)
        {
            if (i < nextList.Count)
            {
                nextViewers[i].SetTetromino(nextList[i]);
            }
        }
    }
}
