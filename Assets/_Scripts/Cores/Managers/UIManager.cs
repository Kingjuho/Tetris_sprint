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

    [Header("UI 설정")]
    [SerializeField] Text timerText;                // 타이머 텍스트
    [SerializeField] Text countdownText;            // 카운트다운 텍스트
    [SerializeField] Text clearTimeText;            // 클리어타임 텍스트
    [SerializeField] Text lineCountText;            // 남은 줄 텍스트
    [SerializeField] GameObject victoryPanel;       // 승리 패널
    [SerializeField] GameObject gameOverPanel;      // 게임 오버 패널



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
        // 포맷팅
        if (timerText != null)
            timerText.text = TimerFormat(timer);
    }

    /** 남은 줄 개수 UI 갱신 **/
    public void UpdateLineCount(int count)
    {
        if (lineCountText != null)
            lineCountText.text = $"Remaining: {Mathf.Clamp(count, 0, 999).ToString()}";
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

    /** 게임 승리 UI 갱신 **/
    public void Victory(float finalTime)
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        if (clearTimeText != null)
            clearTimeText.text = TimerFormat(finalTime);
    }

    /** 타이머 포맷 함수 **/
    public string TimerFormat(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        int milliseconds = Mathf.FloorToInt((time * 100F) % 100F);

        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
}
