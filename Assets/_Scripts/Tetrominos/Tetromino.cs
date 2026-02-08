using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Tetromino : MonoBehaviour
{
    public TetrominoData data { get; private set; } // 테트로미노 데이터
    public Transform[] cells { get; private set; } // 테트로미노 블록 4개

    TetrisInput _input;     // 인풋 시스템
    float _lastFallTime;    // 마지막 낙하 시간
    Ghost _ghost;           // 고스트 테트로미노
    int _rotationIndex = 0; // 현재 회전 상태(0: 스폰, 1: R, 2: 180, 3: L)

    [Header("조작감 설정")]
    [SerializeField] float _dasDelay = 0.2f;        // DAS
    [SerializeField] float _arrDelay = 0.02f;       // ARR
    [SerializeField] float _softDropSpeed = 0.1f;   // ARR(Soft Drop)
    [SerializeField] float _lockDelay = 0.5f;       // Extended Placement

    // 타이머
    float _moveTimer;       // ARR 타이머
    float _softDropTimer;   // ARR(Soft Drop) 타이머
    float _lockTimer;       // Lock 타이머

    // 이전 프레임의 입력 상태
    int _prevX;             
    bool _isSoftDropping;

    void Awake()
    {
        _input = new TetrisInput();
        _lastFallTime = Time.time;
    }

    /** 초기화 함수 **/
    public void Initialize(TetrominoType tetromino)
    {
        // 데이터 초기화
        _rotationIndex = 0;
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

        // 고스트 테트로미노 생성 및 초기화
        if (_ghost != null) Destroy(_ghost.gameObject);

        GameObject ghostObj = new GameObject("Ghost");
        _ghost = ghostObj.AddComponent<Ghost>();

        _ghost.Initialize(this);

        // 게임 오버 체크
        if (!Board.Instance.IsValidPosition(transform))
        {
            Debug.Log("게임 오버");
            this.enabled = false;

            if (GameManager.Instance != null)
                GameManager.Instance.GameOver();

            Destroy(gameObject);
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

        // 고스트 위치 업데이트
        if (_ghost != null)
            _ghost.UpdateGhostPosition(transform);

        // 입력 처리
        HandleMoveInput();
        HandleSoftDropInput();

        // Lock 딜레이 처리
        if (IsGrounded())
        {
            _lockTimer += Time.deltaTime;
            if (_lockTimer >= _lockDelay) Lock();
        }
        else
            _lockTimer = 0f;
    }

    /** 바닥에 닿아있는 지 체크 **/
    bool IsGrounded()
    {
        transform.position += Vector3.down;
        bool isValid = Board.Instance.IsValidPosition(transform);
        transform.position -= Vector3.down;

        return !isValid;
    }

    /** 좌우 이동 핸들러 **/
    void HandleMoveInput()
    {
        // 현재 입력값 구하기
        Vector2 inputVec = _input.Gameplay.Move.ReadValue<Vector2>();
        int x = (int)Mathf.Round(inputVec.x);

        // 방향이 바뀌었을 때
        if (x != _prevX)
        {
            if (x != 0)
            {
                // 즉시 이동 (다음 이동은 das 딜레이 이후)
                Move(new Vector3(x, 0, 0));
                _moveTimer = Time.time + _dasDelay;
            }

            // 상태 저장
            _prevX = x;
        }
        // 방향은 안 바꿨지만 키는 계속 누르고 있을 때
        else if (x != 0)
        {
            // 타이머에 맞춰서 이동
            if (Time.time > _moveTimer)
            {
                Move(new Vector3(x, 0, 0));
                _moveTimer = Time.time + _arrDelay;
            }
        }
    }

    /** 소프트 드롭 핸들러 **/
    void HandleSoftDropInput()
    {
        bool isPressed = _input.Gameplay.SoftDrop.IsPressed();

        if (isPressed)
        {
            // 처음 눌렸거나, 타이머가 다 됐을 때
            if (!_isSoftDropping || Time.time > _softDropTimer)
            {
                Move(Vector3.down);
                _softDropTimer = Time.time + _softDropSpeed;
                _lastFallTime = Time.time;
                _isSoftDropping = true;
            }
            else
                _isSoftDropping = false;
        }
    }

    /** 이벤트 등록 **/
    void OnEnable()
    {
        _input.Enable();
        _input.Gameplay.RotateLeft.performed += OnRotateLeftPerformed;
        _input.Gameplay.RotateRight.performed += OnRotateRightPerformed;
        _input.Gameplay.HardDrop.performed += OnHardDropPerformed;
        _input.Gameplay.Hold.performed += OnHoldPerformed;
    }

    /** 이벤트 해제 **/
    void OnDisable()
    {
        _input.Gameplay.RotateLeft.performed -= OnRotateLeftPerformed;
        _input.Gameplay.RotateRight.performed -= OnRotateRightPerformed;
        _input.Gameplay.HardDrop.performed -= OnHardDropPerformed;
        _input.Gameplay.Hold.performed -= OnHoldPerformed;
        _input.Disable();
    }

    /** 오브젝트 파괴 시 콜백 함수 **/
    void OnDestroy()
    {
        // 고스트 오브젝트 파괴
        if (_ghost != null) Destroy(_ghost.gameObject);
    }

    /**** 콜백 함수 ****/
    /** 회전 **/
    void OnRotateRightPerformed(InputAction.CallbackContext ctx)
    {
        Rotate(-90);
    }
    void OnRotateLeftPerformed(InputAction.CallbackContext ctx)
    {
        Rotate(90);
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
        }
        else
            // 이동에 성공했을 땐 리셋
            _lockTimer = 0f;
    }

    /** 테트로미노 회전 함수 **/
    void Rotate(float angle)
    {
        int originalRotation = _rotationIndex;

        // 회전 인덱스 계산
        // 1: 왼쪽, -1: 오른쪽
        int direction = (angle < 0) ? 1 : -1;
        int newRotation = (_rotationIndex + direction + 4) % 4;

        // 회전 여부 판단
        transform.Rotate(0, 0, angle);
        if (TestWallKicks(_rotationIndex, newRotation))
        {
            // 성공
            _rotationIndex = newRotation;
            _lockTimer = 0f;
        }
        else
            // 실패
            transform.Rotate(0, 0, -angle);
    }

    /** Wall Kick 유효성 검사 **/
    bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        // Wall Kick 데이터 불러오기
        int wallKickIndex = SRSData.GetWallKickIndex(rotationIndex, rotationDirection);
        Vector2Int[] tests = null;

        // I는 다른 데이터 사용, O는 바로 리턴
        if (data.type == TetrominoType.I)
            tests = SRSData.WallKicksI[wallKickIndex];
        else if (data.type == TetrominoType.O)
            return true;
        else
            tests = SRSData.WallKicksOther[wallKickIndex];

        // 회전 테스트
        for (int i = 0; i < tests.Length; i++)
        {
            Vector2Int translation = tests[i];
            transform.position += new Vector3(translation.x, translation.y, 0);

            if (Board.Instance.IsValidPosition(transform))
                return true;
            else
                transform.position -= (Vector3Int)translation;
        }

        return false;
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

        // 고스트 오브젝트 파괴
        if (_ghost != null) Destroy(_ghost.gameObject);

        // 다음 테트로미노 스폰
        if (Spawner.Instance != null)
            Spawner.Instance.SpawnNextTetromino();
    }
}
