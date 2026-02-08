using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class UIManager : MonoBehaviour
{
    // 싱글톤
    public static UIManager Instance { get; private set; }

    // 뷰어
    public TetrominoViewer holdViewer;
    public TetrominoViewer[] nextViewers;

    public Text timerText;                  // 타이머 텍스트
    public Text countdownText;              // 카운트다운 텍스트
    public GameObject gameOverPanel;        // 게임 오버 패널



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

    /** 타이머 UI 갱신 **/
    public void UpdateTimer(float timer)
    {
        // 분, 초, 밀리초
        int minutes = Mathf.FloorToInt(timer / 60F);
        int seconds = Mathf.FloorToInt(timer % 60F);
        int milliseconds = Mathf.FloorToInt((timer * 100F) % 100F);

        // 포맷팅
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    /** 카운트다운 코루틴 **/
    public IEnumerator Countdown()
    {
        // 카운트 다운
        float count = 3f;
        while (count > 0)
        {
            countdownText.text = Mathf.Ceil(count).ToString();
            countdownText.gameObject.SetActive(true);

            yield return null;

            count -= Time.deltaTime;
        }

        // GO
        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);
        countdownText.gameObject.SetActive(false);
    }

    /** 게임오버 UI 갱신 **/
    public void GameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
}
