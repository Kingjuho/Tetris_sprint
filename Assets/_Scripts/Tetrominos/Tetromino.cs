using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Tetromino : MonoBehaviour
{
    public TetrominoData data { get; private set; } // 테트로미노 데이터
    public Transform[] cells { get; private set; } // 테트로미노 블록 4개

    TetrisInput _input;  // 인풋 시스템
    float _lastFallTime;    // 마지막 낙하 시간

    void Awake()
    {
        _input = new TetrisInput();
    }

    /** 초기화 함수 **/
    public void Initialize(TetrominoType tetromino)
    {
        // 데이터 초기화
        data = new TetrominoData { type = tetromino };
        data.Initialize();

        // 색상 설정
        Color color = Data.Colors[tetromino];

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

            // 셀 색상 설정
            cell.GetComponent<SpriteRenderer>().color = color;
        }
    }

    void Update()
    {
        // 일정 시간마다 자동 낙하
        if (Time.time - _lastFallTime >= 1.0f)
        {
            Move(Vector3.down);
            _lastFallTime = Time.time;
        }
    }

    /** 이벤트 등록 **/
    void OnEnable()
    {
        _input.Enable();
        _input.Gameplay.Move.performed += OnMovePerformed;
        _input.Gameplay.RotateLeft.performed += OnRotateLeftPerformed;
        _input.Gameplay.RotateRight.performed += OnRotateRightPerformed;
        _input.Gameplay.SoftDrop.performed += OnSoftDropPerformed;
        _input.Gameplay.HardDrop.performed += OnHardDropPerformed;
        _input.Gameplay.Hold.performed += OnHoldPerformed;
    }

    /** 이벤트 해제 **/
    void OnDisable()
    {
        _input.Gameplay.Move.performed -= OnMovePerformed;
        _input.Gameplay.RotateLeft.performed -= OnRotateLeftPerformed;
        _input.Gameplay.RotateRight.performed -= OnRotateRightPerformed;
        _input.Gameplay.SoftDrop.performed -= OnSoftDropPerformed;
        _input.Gameplay.HardDrop.performed -= OnHardDropPerformed;
        _input.Gameplay.Hold.performed -= OnHoldPerformed;
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
    void OnRotateRightPerformed(InputAction.CallbackContext ctx)
    {
        Rotate(-90);
    }
    void OnRotateLeftPerformed(InputAction.CallbackContext ctx)
    {
        Rotate(90);
    }
    /** 소프트 드롭 **/
    void OnSoftDropPerformed(InputAction.CallbackContext ctx)
    {
        // 한 칸 아래로 이동 후 타이머 리셋
        Move(Vector3.down);
        _lastFallTime = Time.time;
    }
    /** 하드 드롭 **/
    void OnHardDropPerformed(InputAction.CallbackContext ctx)
    {
        // 가능한 한 아래로 이동
        while (Board.Instance.IsValidPosition(transform))
            transform.position += Vector3.down;

        // 마지막 유효 위치로 복구
        transform.position += Vector3.up;

        // 고정
        Lock();
    }
    /** 홀드 **/
    void OnHoldPerformed(InputAction.CallbackContext ctx)
    {
        if (Spawner.Instance != null)
            Spawner.Instance.HoldTetromino(this);
    }

    /** 이동 시도 함수 **/
    void Move(Vector3 pos)
    {
        // 이동 시도
        transform.position += pos;

        // 유효하지 않은 위치라면 원래대로 복구
        if (!Board.Instance.IsValidPosition(transform))
        {
            transform.position -= pos;

            if (pos == Vector3.down) Lock();
        }
    }

    /** 테트로미노 회전 함수 **/
    void Rotate(float angle)
    {
        transform.Rotate(0, 0, angle);
        if (!Board.Instance.IsValidPosition(transform))
        {
            // TODO: SRS(Wall Kick) 구현 예정
            // 유효하지 않으면 원상복구
            transform.Rotate(0, 0, -angle);
        }
    }

    /** 테트로미노 고정 함수 **/
    void Lock()
    {
        foreach (Transform cell in cells)
        {
            // 그리드에 블록 고정
            Vector2 pos = Board.Instance.RoundVector2(cell.position);
            Board.Instance.grid[(int)pos.x, (int)pos.y] = cell;

        }
        
        // 줄 삭제 체크
        Board.Instance.ClearLines();

        // 테트로미노 스크립트 비활성화(임시)
        this.enabled = false;

        // 다음 테트로미노 스폰
        if (Spawner.Instance != null)
            Spawner.Instance.SpawnNextTetromino();
    }
}
