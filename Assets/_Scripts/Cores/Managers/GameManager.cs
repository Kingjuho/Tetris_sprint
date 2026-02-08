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

    private float _timer;                   // 타이머
    private bool _isPlaying = false;        // 현재 상태
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

        // 카운트다운 UI 갱신
        if (UIManager.Instance != null)
            yield return StartCoroutine(UIManager.Instance.Countdown());

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
        _isPlaying = false;

        // UI 갱신
        if (UIManager.Instance != null)
            UIManager.Instance.GameOver();
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
