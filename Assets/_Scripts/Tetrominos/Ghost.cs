using UnityEngine;

public class Ghost : MonoBehaviour
{
    Transform[] _cells; // / 테트로미노 블록 4개

    /** 초기화 함수 **/
    public void Initialize(Tetromino original)
    {
        _cells = new Transform[4];

        for (int i = 0; i < 4; i++)
        {
            // 복제된 테트로미노 생성
            GameObject go = Instantiate(Board.Instance.blockPrefab, transform);
            _cells[i] = go.transform;

            // 위치 동기화
            _cells[i].localPosition = original.cells[i].localPosition;

            // 반투명하게 설정
            SpriteRenderer sprite = go.GetComponent<SpriteRenderer>();
            Color color = original.cells[i].GetComponent<SpriteRenderer>().color;
            color.a = 0.3f;
            sprite.color = color;
        }
    }

    /** 고스트 위치 업데이트 함수 **/
    public void UpdateGhostPosition(Transform original)
    {
        // 원본 테트로미노의 위치, 회전 복사
        transform.position = original.position;
        transform.rotation = original.rotation;

        // Hard drop 위치 계산
        while (Board.Instance.IsValidPosition(transform))
            transform.position += Vector3.down;

        // 마지막 유효 위치로 복구
        transform.position += Vector3.up;
    }
}
