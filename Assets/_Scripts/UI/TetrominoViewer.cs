using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TetrominoViewer : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] float cellSize = 30f;  // UI에서 블록 한 칸의 크기
    [SerializeField] Sprite blockSprite;    // 블록 스프라이트

    private List<Image> _cellImages = new List<Image>();    // 블록 이미지 리스트

    private void Awake()
    {
        // 미리 컴포넌트 4개 대기시켜놓기
        for (int i = 0; i < 4; i++)
        {
            GameObject go = new GameObject($"Cell_{i}");
            go.transform.SetParent(transform, false);

            Image img = go.AddComponent<Image>();
            img.sprite = blockSprite;

            // 중앙 정렬
            img.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            img.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            img.rectTransform.pivot = new Vector2(0.5f, 0.5f);

            _cellImages.Add(img);
            go.SetActive(false);
        }
    }

    /** UI 영역에 테트로미노를 그리는 함수 **/
    public void SetTetromino(TetrominoType type)
    {
        // 해당 타입의 데이터(좌표) 가져오기
        Vector2Int[] positions = Data.Cells[type];
        Color color = Data.Colors[type];

        // 블럭 4개의 평균 좌표를 구해서 그만큼 반대로 이동
        Rect bounds = GetBounds(positions);
        Vector2 centerOffset = bounds.center;

        // 셀 배치
        for (int i = 0; i < 4; i++)
        {
            // 미리 대기시켜놓은 컴포넌트 활성화 및 색상 변경
            Image img = _cellImages[i];
            img.gameObject.SetActive(true);
            img.color = color;

            // cellSize를 곱해서 UI 좌표로 변환
            float x = positions[i].x * cellSize;
            float y = positions[i].y * cellSize;

            // 중앙 정렬 보정 (offset만큼 빼줌)
            x -= centerOffset.x * cellSize;
            y -= centerOffset.y * cellSize;

            // 적용
            img.rectTransform.anchoredPosition = new Vector2(x, y);
            img.rectTransform.sizeDelta = new Vector2(cellSize, cellSize);
        }
    }

    /** UI 영역의 테트로미노를 지우는 함수 **/
    public void Clear()
    {
        foreach (var img in _cellImages)
        {
            img.gameObject.SetActive(false);
        }
    }

    /** 테트로미노의 영역을 계산하는 헬퍼 함수 **/
    Rect GetBounds(Vector2Int[] positions)
    {
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);

        foreach (var pos in positions)
        {
            if (pos.x < min.x) min.x = pos.x;
            if (pos.y < min.y) min.y = pos.y;
            if (pos.x > max.x) max.x = pos.x;
            if (pos.y > max.y) max.y = pos.y;
        }

        // 너비와 높이는 max - min, 중심은 min + size/2
        return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }
}
