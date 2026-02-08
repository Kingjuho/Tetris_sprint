using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // 싱글톤
    public static GameManager Instance { get; private set; }

    [Header("설정")]
    public float restartHoldTime = 1.0f;    // 리스타트
    public int targetLines = 40;            // 목표 줄 개수

    private float _timer;                   // 타이머
    private bool _isPlaying = false;        // 현재 상태
    private int _clearedLines = 0;          // 총 지워진 줄 개수
    private float _restartTimer = 0f;       // 리스타트 타이머

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(GameStartSequence());
    }

    private void Update()
    {
        // 타이머 작동
        if (_isPlaying)
        {
            _timer += Time.deltaTime;
            UpdateTimerUI();
        }

        // R키 재시작
        HandleRestartInput();
    }

    /** 게임 시작 코루틴 **/
    IEnumerator GameStartSequence()
    {
        // 스포너 잠시 멈춤
        _isPlaying = false;
        if (Spawner.Instance != null) Spawner.Instance.enabled = false;

        // UI 갱신
        if (UIManager.Instance != null)
        {
            // 남은 줄 개수
            UIManager.Instance.UpdateLineCount(targetLines);

            // 카운트 다운
            yield return StartCoroutine(UIManager.Instance.Countdown());
        }

        // 게임 시작
        _isPlaying = true;
        if (Spawner.Instance != null)
        {
            Spawner.Instance.enabled = true;
            Spawner.Instance.SpawnNextTetromino(); // 첫 블럭 소환
        }
    }

    /** 게임 오버 **/
    public void GameOver()
    {
        GameStop();

        // UI 갱신
        if (UIManager.Instance != null)
            UIManager.Instance.GameOver();
    }

    /** 게임 승리 **/
    void Victory()
    {
        GameStop();

        if (Spawner.Instance != null)
            Spawner.Instance.enabled = false;

        if (UIManager.Instance != null)
            UIManager.Instance.Victory(_timer);
    }

    /** 게임 정지 **/
    void GameStop()
    {
        _isPlaying = false;

        // 스포너 비활성화
        if (Spawner.Instance != null)
            Spawner.Instance.enabled = false;

        // 현재 떨어지고 있는 블럭 비활성화 (중력/입력 차단)
        Tetromino current = FindFirstObjectByType<Tetromino>();
        if (current != null)
            current.enabled = false;
    }

    /** 지운 줄 개수 갱신 **/
    public void OnLinesCleared(int lines)
    {
        // 게임 중이 아니면 리턴
        if (!_isPlaying) return;

        _clearedLines += lines;

        // UI 갱신
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateLineCount(targetLines - _clearedLines);

        // 승리 조건 체크
        if (_clearedLines >= targetLines)
            Victory();
    }

    /** 타이머 UI 변경 함수 호출용 **/
    void UpdateTimerUI()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateTimer(_timer);
    }

    /** 게임 재시작 인풋 핸들러 **/
    void HandleRestartInput()
    {
        if (UnityEngine.InputSystem.Keyboard.current.rKey.isPressed)
        {
            // 1초 넘으면 재시작
            _restartTimer += Time.deltaTime;
            if (_restartTimer >= restartHoldTime)
                GameRestart();
        }
        else
            _restartTimer = 0f; // 손 떼면 초기화
    }

    /** 게임 재시작 **/
    void GameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
