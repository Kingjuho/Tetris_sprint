using UnityEngine;
using UnityEngine.InputSystem;

public class Tetromino : MonoBehaviour
{
    public TetrominoData data { get; private set; } // 테트로미노 데이터
    public Transform[] cells { get; private set; } // 테트로미노 블록 4개

    TetrisInput _input;  // 인풋 시스템
    float _lastFallTime;    // 마지막 낙하 시간

    private void Awake()
    {
        _input = new TetrisInput();
    }

    /** 초기화 함수 **/
    public void Initialize(TetrominoType tetromino)
    {
        // 데이터 초기화
        data = new TetrominoData { type = tetromino };
        data.Initialize();

        // 셀 초기화
        if (cells == null) cells = new Transform[4];

        // 기존 자식 오브젝트 제거
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        // 새로운 블록 생성
        for (int i = 0; i < 4; i++)
        {
            GameObject cell = Instantiate(Board.Instance.blockPrefab, transform);
            cells[i] = cell.transform;

            // 셀 위치 설정
            cells[i].localPosition = (Vector3Int)data.cells[i];
        }
    }

    private void Start()
    {
        Initialize(TetrominoType.T);
    }

    /** 이벤트 등록 **/
    private void OnEnable()
    {
        _input.Enable();
        _input.Gameplay.Move.performed += OnMovePerformed;
        _input.Gameplay.Rotate.performed += OnRotatePerformed;
        _input.Gameplay.Drop.performed += OnDropPerformed;
    }

    /** 이벤트 해제 **/
    private void OnDisable()
    {
        _input.Gameplay.Move.performed -= OnMovePerformed;
        _input.Gameplay.Rotate.performed -= OnRotatePerformed;
        _input.Gameplay.Drop.performed -= OnDropPerformed;
        _input.Disable();
    }

    /**** 콜백 함수 ****/
    /** 좌우 이동 **/
    void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        Debug.Log($"Input Received: {ctx.ReadValue<Vector2>()}");

        Vector2 input = ctx.ReadValue<Vector2>();
        if (Mathf.Abs(input.x) > 0.5f) 
            Move(new Vector3(Mathf.Sign(input.x), 0, 0));
    }
    /** 회전 **/
    void OnRotatePerformed(InputAction.CallbackContext ctx)
    {
        // TODO: 회전 방향을 입력에 따라 다르게 할 수 있어야 함
        transform.Rotate(0, 0, -90);
        if (!Board.Instance.IsValidPosition(transform))
        {
            // 유효하지 않은 위치라면 원래대로 복구
            transform.Rotate(0, 0, 90);
        }
    }
    /** 수동 낙하 **/
    void OnDropPerformed(InputAction.CallbackContext ctx)
    {
        Move(Vector3.down);
        _lastFallTime = Time.time;  // 타이머 리셋
    }

    private void Update()
    {
        // 일정 시간마다 자동 낙하
        if (Time.time - _lastFallTime >= 1.0f)
        {
            Move(Vector3.down);
            _lastFallTime = Time.time;
        }
    }

    /** 이동 시도 함수 **/
    private void Move(Vector3 pos)
    {
        // 이동 시도
        transform.position += pos;

        // 유효하지 않은 위치라면 원래대로 복구
        if (!Board.Instance.IsValidPosition(transform))
        {
            transform.position -= pos;

            if (pos.y < 0)
            {
                // TODO: Board에 블록 고정 및 새로운 Piece 생성 로직 추가
                enabled = false; // 더 이상 이동 불가, Piece 비활성화
            }
        }
    }
}
