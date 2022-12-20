using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerationController : MonoBehaviour {
    public int size = 25;

    private void Start() {
        CreateMaze();
    }

    private void CreateMaze() {
        float[,] maze = new float[size, size];

        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                if (i % 2 == 1 || j % 2 == 1)
                    maze[i, j] = 0;
                if (i == 0 || j == 0 || i == size - 1 || j == size - 1)
                    maze[i, j] = 0.5f;
            }
        }

        int startPosX = Random.Range(2, size - 2);
        int startPosY = Random.Range(2, size - 2);
        
        Generator(startPosX, startPosY, maze);

        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                if (maze[i, j] == 0.5f)
                    maze[i, j] = 1;
            }
        }

        // TODO dit is denk ik om de start en finish te zetten
        maze[1, 2] = 1;
        maze[size - 2, size - 3] = 1;

        DrawMaze(maze);
    }

    private void Generator(int startX, int startY, float[,] maze) {
        maze[startY, startX] = 0.5f;
        
        // if (maze[startY - 2, startX] == 0.5f && maze[startY + 2, startX] )
        if (maze[startY - 2, startX] == 0.5 &&
            maze[startY + 2, startX] == 0.5 &&
            maze[startY, startX - 2] == 0.5 &&
            maze[startY, startX + 2] == 0.5) {
            
        }
        else {
            List<int> li = new() {0, 1, 2, 3};  // 0 = up, 1 = down, 2 = left, 3 = right

            int nY = 0;
            int mY = 0;
            int nX = 0;
            int mX = 0;

            while (li.Count > 0) {
                int direction = li[Random.Range(0, li.Count)];
                li.Remove(direction);
                
                if (direction == 0) {
                    nY = startY - 2;
                    mY = startY - 1;
                }
                else if (direction == 1) {
                    nY = startY + 2;
                    mY = startY + 1;
                }
                else {
                    nY = startY;
                    mY = startY;
                }
                
                if (direction == 2) {
                    nX = startX - 2;
                    mX = startX - 1;
                }
                else if (direction == 3) {
                    nX = startX + 2;
                    mX = startX + 1;
                }
                else {
                    nX = startX;
                    mX = startX;
                }

                if (maze[nY, nX] != 0.5f) {
                    maze[mY, mX] = 0.5f;
                    Generator(nX, nY, maze);
                }
            }
        }
    }

    private void DrawMaze(float[,] maze) {
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                Debug.Log(maze[i, j]);
            }
        }
    }
}