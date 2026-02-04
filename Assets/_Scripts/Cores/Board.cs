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

    void Awake()
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

    /** 줄 삭제 메인 함수 **/
    public void ClearLines()
    {
        // 바닥부터 모든 행 검사
        for (int y = 0; y < height; y++)
        {
            if (IsLineFull(y))
            {
                ClearLine(y);
                MoveRowsDown(y);
                y--;
            }
        }
    }

    /** 특정 행이 꽉 찼는지 검사 **/
    bool IsLineFull(int y)
    {
        for (int x = 0; x < width; x++)
        {
            // 비어있는 칸이 하나라도 있으면 false 반환
            if (grid[x, y] == null)
                return false;
        }

        return true;
    }

    /** 특정 행 삭제 **/
    void ClearLine(int y)
    {
        for (int x = 0; x < width; x++)
        {
            // 블록 오브젝트 삭제 및 그리드 초기화
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    /** 특정 행 위의 모든 행을 한 칸씩 내리기 **/
    void MoveRowsDown(int emptyRow)
    {
        for (int y = emptyRow + 1; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 빈 칸 건너뛰기
                if (grid[x, y] == null) continue;

                // 블록을 한 칸 아래로 이동
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;

                // 실제 위치 업데이트
                grid[x, y - 1].position += Vector3.down;
            }
        }
    }
}
