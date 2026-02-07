using System.Collections.Generic;
using UnityEngine;

public static class SRSData
{
    // 0: Spawn, 1: Right, 2: Down, 3: Left
    // 회전 전 상태(Index)와 회전 방향에 따라 검사할 5개의 좌표(Vector2Int)를 반환

    // 1. J, L, S, T, Z 데이터
    public static readonly Dictionary<int, Vector2Int[]> WallKicksOther = new Dictionary<int, Vector2Int[]>
    {
        // 0 -> 1 (오른쪽 회전)
        { 0, new Vector2Int[] { new(0, 0), new(-1, 0), new(-1, 1), new(0, -2), new(-1, -2) } },
        // 1 -> 0 (왼쪽 회전)
        { 10, new Vector2Int[] { new(0, 0), new(1, 0), new(1, -1), new(0, 2), new(1, 2) } },
        
        // 1 -> 2
        { 1, new Vector2Int[] { new(0, 0), new(1, 0), new(1, -1), new(0, 2), new(1, 2) } },
        // 2 -> 1
        { 21, new Vector2Int[] { new(0, 0), new(-1, 0), new(-1, 1), new(0, -2), new(-1, -2) } },

        // 2 -> 3
        { 2, new Vector2Int[] { new(0, 0), new(1, 0), new(1, 1), new(0, -2), new(1, -2) } },
        // 3 -> 2
        { 32, new Vector2Int[] { new(0, 0), new(-1, 0), new(-1, -1), new(0, 2), new(-1, 2) } },

        // 3 -> 0
        { 3, new Vector2Int[] { new(0, 0), new(-1, 0), new(-1, -1), new(0, 2), new(-1, 2) } },
        // 0 -> 3
        { 30, new Vector2Int[] { new(0, 0), new(1, 0), new(1, 1), new(0, -2), new(1, -2) } }
    };

    // 2. I 데이터
    public static readonly Dictionary<int, Vector2Int[]> WallKicksI = new Dictionary<int, Vector2Int[]>
    {
        // 0 -> 1
        { 0, new Vector2Int[] { new(0, 0), new(-2, 0), new(1, 0), new(-2, -1), new(1, 2) } },
        // 1 -> 0
        { 10, new Vector2Int[] { new(0, 0), new(2, 0), new(-1, 0), new(2, 1), new(-1, -2) } },

        // 1 -> 2
        { 1, new Vector2Int[] { new(0, 0), new(-1, 0), new(2, 0), new(-1, 2), new(2, -1) } },
        // 2 -> 1
        { 21, new Vector2Int[] { new(0, 0), new(1, 0), new(-2, 0), new(1, -2), new(-2, 1) } },

        // 2 -> 3
        { 2, new Vector2Int[] { new(0, 0), new(2, 0), new(-1, 0), new(2, 1), new(-1, -2) } },
        // 3 -> 2
        { 32, new Vector2Int[] { new(0, 0), new(-2, 0), new(1, 0), new(-2, -1), new(1, 2) } },

        // 3 -> 0
        { 3, new Vector2Int[] { new(0, 0), new(1, 0), new(-2, 0), new(1, -2), new(-2, 1) } },
        // 0 -> 3
        { 30, new Vector2Int[] { new(0, 0), new(-1, 0), new(2, 0), new(-1, 2), new(2, -1) } }
    };

    public static int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        // 정방향: 0 -> 1, 1 -> 2, 2 -> 3, 3 -> 0 (0, 1, 2, 3)
        // 역방향: 1 -> 0, 2 -> 1, 3 -> 2, 0 -> 3 (10, 21, 32, 30)

        if (rotationIndex == 0 && rotationDirection == 1) return 0;
        if (rotationIndex == 1 && rotationDirection == 0) return 10;

        if (rotationIndex == 1 && rotationDirection == 2) return 1;
        if (rotationIndex == 2 && rotationDirection == 1) return 21;

        if (rotationIndex == 2 && rotationDirection == 3) return 2;
        if (rotationIndex == 3 && rotationDirection == 2) return 32;

        if (rotationIndex == 3 && rotationDirection == 0) return 3;
        if (rotationIndex == 0 && rotationDirection == 3) return 30;

        return 0;
    }
}
