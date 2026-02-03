using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }

    [Header("기본 설정")]
    // 테트리스 표준 규격
    public int width = 10;
    public int height = 20;
    // 블록 프리팹
    public GameObject blockPrefab;

    // 그리드 배열
    public Transform[,] grid;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // 그리드 초기화
        grid = new Transform[width, height];
    }

    /** 좌표 반올림 함수 **/
    public Vector2 RoundVector2(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

    /** 좌표 범위 검사 함수 **/
    public bool IsValidPosition(Transform Tetromino)
    {
        foreach (Transform block in Tetromino)
        {
            // 블록의 위치를 반올림
            Vector2 pos = RoundVector2(block.position);
            int x = (int)pos.x;
            int y = (int)pos.y;


            // 좌표가 그리드 범위 내에 있는지 확인
            // y 상한선 체크는 게임오버 로직에서 처리
            if (x < 0 || x >= width || y < 0) return false;

            // 해당 좌표에 이미 블록이 존재하는지 확인
            if (y < height && grid[x, y] != null) return false;
        }

        return true;
    }
}
