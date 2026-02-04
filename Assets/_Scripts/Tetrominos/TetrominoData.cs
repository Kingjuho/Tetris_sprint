using UnityEngine;
using UnityEngine.Tilemaps;

public enum TetrominoType { I, O, T, J, L, S, Z }

[System.Serializable]
public class TetrominoData
{
    public TetrominoType type;               // 테트로미노 타입
    public Tile tile;                        // 타일 그래픽
    public Vector2Int[] cells;               // 테트로미노 블록의 상대 좌표

    public void Initialize()
    {
        cells = Data.Cells[type];
    }
}

public static class Data
{
    /** 테트로미노 블럭 좌표 **/
    public static readonly System.Collections.Generic.Dictionary<TetrominoType, Vector2Int[]> Cells = new()
    {
        { TetrominoType.I, new Vector2Int[] { new(-1, 1), new(0, 1), new(1, 1), new(2, 1) } },
        { TetrominoType.J, new Vector2Int[] { new(-1, 1), new(-1, 0), new(0, 0), new(1, 0) } },
        { TetrominoType.L, new Vector2Int[] { new(1, 1), new(-1, 0), new(0, 0), new(1, 0) } },
        { TetrominoType.O, new Vector2Int[] { new(0, 1), new(1, 1), new(0, 0), new(1, 0) } },
        { TetrominoType.S, new Vector2Int[] { new(0, 1), new(1, 1), new(-1, 0), new(0, 0) } },
        { TetrominoType.T, new Vector2Int[] { new(0, 1), new(-1, 0), new(0, 0), new(1, 0) } },
        { TetrominoType.Z, new Vector2Int[] { new(-1, 1), new(0, 1), new(0, 0), new(1, 0) } },
    };

    /** 테트로미노 색상 **/
    public static readonly System.Collections.Generic.Dictionary<TetrominoType, Color> Colors = new()
    {
        { TetrominoType.I, Color.cyan },
        { TetrominoType.O, Color.yellow },
        { TetrominoType.T, Color.magenta },
        { TetrominoType.S, Color.green },
        { TetrominoType.Z, Color.red },
        { TetrominoType.J, Color.blue },
        { TetrominoType.L, new Color(1f, 0.5f, 0f) }
    };
}