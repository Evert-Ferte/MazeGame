using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// Art style:
// - simplistic
// - hand drawn
public class MazeGenerationController : MonoBehaviour {
    public Vector2Int size = new (25, 25);
    [Space]
    
    public Transform deadEndTop;
    public Transform deadEndRight;
    public Transform deadEndBottom;
    public Transform deadEndLeft;
    [Space] public Transform straightHorizontal;
    public Transform straightVertical;
    [Space] public Transform cornerTopRight;
    public Transform cornerRightBottom;
    public Transform cornerBottomLeft;
    public Transform cornerLeftTop;
    [Space] public Transform tSplitLeftTopRight;
    public Transform tSplitTopRightBottom;
    public Transform tSplitRightBottomLeft;
    public Transform tSplitBottomLeftTop;
    [Space] public Transform crossroad;

    private int[,] maze;
    private Vector2Int entrance = new (1, 0);
    private Vector2Int exit;
    
    private void Start() {
        CreateMaze();
    }

    private void CreateMaze() {
        maze = new int[size.y, size.x];
        exit = new(size.x - 2, size.y - 1);
        
        int startPosX = Random.Range(2, size.x - 2);
        int startPosY = Random.Range(2, size.y - 2);
        
        Generator(startPosX, startPosY, maze);

        // Place the start and finish
        // TODO maybe pick a random start, and make the end the opposite of this
        maze[entrance.y, entrance.x] |= 0b1000;
        maze[exit.y, exit.x] |= 0b0010;
        
        DrawMaze(maze);
        
        // Resize the camera so it fully shows the maze
        FindObjectOfType<CameraController>().ResizeCameraToShowContent(size.x);
    }

    private void Generator(int cX, int cY, int[,] maze) {
        bool valid = false;

        if (cY != 0)
            if (maze[cY - 1, cX] == 0)
                valid = true;
        if (cY != size.y - 1)
            if (maze[cY + 1, cX] == 0)
                valid = true;
        if (cX != 0)
            if (maze[cY, cX - 1] == 0)
                valid = true;
        if (cX != size.x - 1)
            if (maze[cY, cX + 1] == 0)
                valid = true;

        if (!valid) return;
        
        List<int> li = new() {0, 1, 2, 3};  // 0 = up, 1 = right, 2 = down, 3 = left
        if (cX == 0) li.Remove(3);
        else if (cX == size.x - 1) li.Remove(1);
        if (cY == 0) li.Remove(0);
        else if (cY == size.y - 1) li.Remove(2);
            
        int[] dirs = { 0b1000, 0b0100, 0b0010, 0b0001 };

        while (li.Count > 0) {
            int nY, nX;
            int dir = li[Random.Range(0, li.Count)];
            li.Remove(dir);

            if (dir == 0) nY = cY - 1;
            else if (dir == 2) nY = cY + 1;
            else nY = cY;

            if (dir == 3) nX = cX - 1;
            else if (dir == 1) nX = cX + 1;
            else nX = cX;

            if (maze[nY, nX] == 0) {
                maze[cY, cX] |= dirs[dir];
                maze[nY, nX] |= dirs[dir + (dir < 2 ? 2 : -2)];
                Generator(nX, nY, maze);
            }
        }
    }

    private void DrawMaze(int[,] maze) {
        for (int i = 0; i < maze.GetLength(0); i++) {
            for (int j = 0; j < maze.GetLength(1); j++) {
                Transform objToSpawn;
            
                // Dead ends
                if (maze[i, j] == 8) objToSpawn = deadEndTop;
                else if (maze[i, j] == 4) objToSpawn = deadEndRight;
                else if (maze[i, j] == 2) objToSpawn = deadEndBottom;
                else if (maze[i, j] == 1) objToSpawn = deadEndLeft;
                // Straights
                else if (maze[i, j] == 10) objToSpawn = straightVertical;
                else if (maze[i, j] == 5) objToSpawn = straightHorizontal;
                // Corners
                else if (maze[i, j] == 12) objToSpawn = cornerTopRight;
                else if (maze[i, j] == 6) objToSpawn = cornerRightBottom;
                else if (maze[i, j] == 3) objToSpawn = cornerBottomLeft;
                else if (maze[i, j] == 9) objToSpawn = cornerLeftTop;
                // T-splits
                else if (maze[i, j] == 13) objToSpawn = tSplitLeftTopRight;
                else if (maze[i, j] == 14) objToSpawn = tSplitTopRightBottom;
                else if (maze[i, j] == 7) objToSpawn = tSplitRightBottomLeft;
                else if (maze[i, j] == 11) objToSpawn = tSplitBottomLeftTop;
                // Crossroad
                else objToSpawn = crossroad;

                Vector2 pos = MazeToWorldPosition(j, i);
                Instantiate(objToSpawn, pos, Quaternion.identity, this.transform);
            }
        }
    }

    private Vector2 MazeToWorldPosition(float x, float y) {
        return new (x - size.x / 2f + (size.x % 2 == 0 ? 0.5f : 0), (y - size.y / 2f) * -1);
    }

    public Vector2Int WorldToMazePosition(float x, float y) {
        return new((int)(x + size.x / 2f), (int)(size.y - (y + size.y / 2f)));
    }

    public int[,] GetMaze() { return maze; }
    public Vector2Int GetEntranceMazePosition() { return entrance; }
    public Vector2 GetEntranceWorldPosition() { return MazeToWorldPosition(entrance.x, entrance.y); }
    public Vector2Int GetExitMazePosition() { return exit; }
    public Vector2 GetExitWorldPosition() { return MazeToWorldPosition(exit.x, exit.y); }
}
