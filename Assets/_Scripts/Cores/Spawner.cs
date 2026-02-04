using UnityEngine;

public class Spawner : MonoBehaviour
{
    // 싱글톤
    public static Spawner Instance { get; private set; }

    // 테트로미노 프리팹
    public GameObject tetrominoPrefab;

    // 홀드 데이터
    TetrominoType _heldType;    // 홀드된 테트로미노 타입
    bool _isHoldFull = false;   // 홀드 칸의 블럭 유무
    bool _canHold = true;       // 홀드 가능 여부

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        SpawnNextTetromino();
    }

    /** 다음 테트로미노 스폰 함수 **/
    public void SpawnNextTetromino()
    {
        // 홀드 가능 상태로 변경
        _canHold = true;

        // 랜덤 테트로미노 생성
        SpawnTetromino(GetRandomTetromino());
    }

    /** 테트로미노 스폰 함수 **/
    void SpawnTetromino(TetrominoType type)
    {
        // 씬 중앙 상단에 생성
        GameObject go = Instantiate(tetrominoPrefab, transform.position, Quaternion.identity);

        // 테트로미노 초기화
        Tetromino tetromino = go.GetComponent<Tetromino>();
        tetromino.Initialize(type);
    }

    /** 테트로미노 랜덤 생성 함수 **/
    TetrominoType GetRandomTetromino()
    {
        // TODO: 나중에 7-Bag 알고리즘으로 변경
        return (TetrominoType)Random.Range(0, 7);
    }

    /** 현재 테트로미노 홀드 함수 **/
    public void HoldTetromino(Tetromino currentTetromino)
    {
        // 이미 홀드한 경우 종료
        if (!_canHold) return;

        // 현재 블럭 타입 저장 후 제거
        TetrominoType currentType = currentTetromino.data.type;
        Destroy(currentTetromino.gameObject);

        // 홀드 로직 수행
        if (!_isHoldFull)
        {
            // 홀드 칸이 비어있는 경우
            _heldType = currentType;
            _isHoldFull = true;
            SpawnTetromino(GetRandomTetromino());
        }
        else
        {
            // 홀드 칸이 차있는 경우
            (currentType, _heldType) = (_heldType, currentType);
            SpawnTetromino(currentType);
        }

        // 홀드 불가 상태로 변경
        _canHold = false;
    }
}
