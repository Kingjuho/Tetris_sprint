using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    // 싱글톤
    public static Spawner Instance { get; private set; }

    // 테트로미노 프리팹
    public GameObject tetrominoPrefab;

    // 7-Bag 데이터
    Stack<TetrominoType> _bag = new Stack<TetrominoType>();

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
        // 리스트 안에 아무것도 없으면 리필
        if (_bag.Count == 0) RefillBag();

        return _bag.Pop();
    }

    /** 7-Bag 리필 함수 **/
    void RefillBag()
    {
        // 임시 배열 생성
        TetrominoType[] tempArray = new TetrominoType[7];

        // 배열에 모든 테트로미노 타입 추가
        for (int i = 0; i < tempArray.Length; i++)
            tempArray[i] = (TetrominoType)i;

        // 셔플(피셔 예이츠 알고리즘)
        int n = tempArray.Length;

        while (n > 1)
        {
            n--;

            // 0부터 n 사이의 무작위 인덱스 선택
            int k = Random.Range(0, n + 1);

            // 스왑
            (tempArray[k], tempArray[n]) = (tempArray[n], tempArray[k]);
        }

        // 셔플된 배열을 스택으로 변환
        _bag = new Stack<TetrominoType>(tempArray);
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
