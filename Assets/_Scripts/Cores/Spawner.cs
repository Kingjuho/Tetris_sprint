using UnityEngine;

public class Spawner : MonoBehaviour
{
    // 싱글톤
    public static Spawner Instance { get; private set; }

    // 테트로미노 프리팹
    public GameObject tetrominoPrefab;

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
        // 씬 중앙 상단에 생성
        GameObject go = Instantiate(tetrominoPrefab, transform.position, Quaternion.identity);

        // 랜덤한 테트로미노 타입 선택
        // TODO: 나중에 7-Bag 알고리즘으로 변경
        int randomIndex = Random.Range(0, 7);
        
        // 테트로미노 초기화
        Tetromino tetromino = go.GetComponent<Tetromino>();
        tetromino.Initialize((TetrominoType)randomIndex);
    }
}
