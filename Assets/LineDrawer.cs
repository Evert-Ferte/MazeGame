using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// [RequireComponent(typeof(LineRenderer))]
public class LineDrawer : MonoBehaviour {
    // TODO maybe a good idea to indicate if we are drawing, could be done by simply giving the line another color
    
    public MazeGenerationController mazeController;
    public Material lineMaterial;
    public float lineColorSpeedFactor;
    
    private LineRenderer line;
    private bool isDrawing;  // TODO implement; make sure the camera doesnt move while drawing
    private Vector2 previousCellPosition;

    public List<LineRenderer> oldLines = new();

    private int linesPlaced;

    private void Update() {
        if (Input.GetMouseButtonDown(0))
            OnFirstContact();
        if (Input.GetMouseButton(0))
            OnDrawing();
        if (Input.GetMouseButtonUp(0))
            OnDrawingEnd();

        UpdateVisuals();
    }
    
    private void UpdateVisuals() {
        Color newColor = GetNextColorGradientColor();
        if (line != null) {
            line.startColor = newColor;
            line.endColor = newColor;
        }

        foreach (LineRenderer oldLine in oldLines) {
            oldLine.startColor = newColor;
            oldLine.endColor = newColor;
        }
    }

    private Color GetNextColorGradientColor() {
        float value = (Time.time * lineColorSpeedFactor) % 1;
        return Color.HSVToRGB(value, 1, 1);
    }

    private void OnFirstContact() {
        Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // // Check if a line exists
        // if (line.positionCount == 0) {
        //     Vector2 mazeEntrance = mazeController.GetEntranceWorldPosition();
        //
        //     // Check if we clicked the entrance
        //     if (Mathf.Abs(mazeEntrance.x - clickPos.x) <= 0.5f) {
        //         if (Mathf.Abs(mazeEntrance.y - clickPos.y) <= 0.5f) {
        //             line.positionCount += 2;
        //             line.SetPosition(line.positionCount - 1, mazeEntrance);
        //             line.SetPosition(line.positionCount - 2, new Vector3(mazeEntrance.x, mazeEntrance.y + 1));
        //         }
        //     }
        // }
        // else {
        //     // TODO implement
        // }

        previousCellPosition = new (Mathf.Round(clickPos.x + 0.5f) - 0.5f, Mathf.Round(clickPos.y));

        // Check where we clicked, on an existing line? if not, return

        /*
         * One of three things can happen:
         * 1) Clicked on maze entrance and no line has been placed? Create a new line where clicked (entrance)
         * 2) Clicked on an existing line, and we are going towards a spot where we have not drawn yet, continue drawing
         * 3) Clicked not on a line, then do nothing
         *
         * No line exists   : Then we can only start drawing at the entrance
         * Line exists      : Then we can only start drawing at an existing line, towards a spot where we have not drawn yet
         *
         * Snap to grid (1, 1)
         */
    }

    private void OnDrawing() {
        // if (!isDrawing) return;
        
        // check if we are on a new cell
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 clampedMousePos = new (Mathf.Round(mouseWorldPos.x + 0.5f) - 0.5f, Mathf.Round(mouseWorldPos.y));

        // See if we are over a new cell
        if (clampedMousePos != previousCellPosition) {
            // Debug.Log(Mathf.Abs(clampedMousePos.x - previousCellPosition.x) + " - " +
            //           Mathf.Abs(clampedMousePos.y - previousCellPosition.y) + " - " +
            //           (Mathf.Abs(clampedMousePos.x - previousCellPosition.x) == 1 ^ Mathf.Abs(clampedMousePos.y - previousCellPosition.y) == 1) + " - " +
            //           clampedMousePos + " - " + previousCellPosition);
            // Then see if the new cell is a neighbouring tile from last tile we DREW on
            if (Mathf.Abs(clampedMousePos.x - previousCellPosition.x) == 1 ^ 
                Mathf.Abs(clampedMousePos.y - previousCellPosition.y) == 1) {
                OnNewCell(previousCellPosition, clampedMousePos);
            }
            else {
                // Debug.Log("not neighbours");
            }
        }
    }

    private void OnDrawingEnd() {
        if (line == null) return;
        oldLines.Add(line);
        line = null;
    }

    private void OnNewCell(Vector2 oldCell, Vector2 newCell) {
        // Return if the position we are drawing towards is confined within the maze
        Vector2Int newCellPos = mazeController.WorldToMazePosition(newCell.x, newCell.y);
        if (newCellPos.x < 0 || newCellPos.x > mazeController.size.x - 1 || newCellPos.y < 0 || newCellPos.y > mazeController.size.y - 1)
            return;

        Vector2Int oldCellPos = mazeController.WorldToMazePosition(oldCell.x, oldCell.y);

        int[,] maze = mazeController.GetMaze();

        int dir = 0;
        int[] dirs = { 0b1000, 0b0100, 0b0010, 0b0001 };  // up, right, down, left
        if (newCellPos.x > oldCellPos.x) dir = 1;
        else if (newCell.x < oldCell.x) dir = 3;
        if (newCellPos.y > oldCellPos.y) dir = 2;
        else if (newCell.y < oldCell.y) dir = 0;
        
        int newPosVal = maze[newCellPos.y, newCellPos.x];
        int oldPosVal = maze[oldCellPos.y, oldCellPos.x];

        // Check if the cell we want to go to and were we are, are connected
        if ((oldPosVal & dirs[dir]) != dirs[dir])
            return;
        int oppositeDir = dir + (dir < 2 ? 2 : -2);
        if ((newPosVal & dirs[oppositeDir]) != dirs[oppositeDir])
            return;

        // We can only move 1 cell in a horizontal or vertical manner
        Vector2 dif = oldCellPos - newCellPos;
        if (Mathf.Abs(dif.x + dif.y) != 1)
            return;
        
        // check if we are on the previous cell, if so, undo the previous line (we need to save the new and old cell visited)

        // check if we are at the exit

        if (line == null) {
            GameObject newLineObject = Instantiate(new GameObject("dont spawn this"), transform);
            LineRenderer lineRenderer = newLineObject.AddComponent<LineRenderer>();
            line = lineRenderer;
            line.positionCount = 0;
            line.widthCurve = AnimationCurve.Constant(0, 1, 0.25f);
            line.startColor = new Color(0, 0.8078f, 1, 1);
            line.endColor = new Color(0, 0.8078f, 1, 1);
            line.alignment = LineAlignment.View;
            line.useWorldSpace = false;
            line.shadowCastingMode = ShadowCastingMode.Off;
            line.allowOcclusionWhenDynamic = false;
            line.material = lineMaterial;
            
            line.positionCount++;
            line.SetPosition(0, oldCell);

            linesPlaced++;
            
            // Check if we clicked the entrance
            Vector2 mazeEntrance = mazeController.GetEntranceWorldPosition();
            if (Mathf.Abs(mazeEntrance.x - newCell.x) <= 0.5f) {
                if (Mathf.Abs(mazeEntrance.y - newCell.y) <= 0.5f) {
                    line.positionCount += 2;
                    line.SetPosition(line.positionCount - 1, mazeEntrance);
                    line.SetPosition(line.positionCount - 2, new Vector3(mazeEntrance.x, mazeEntrance.y + 1));
                }
            }
        }
        
        line.positionCount++;
        line.SetPosition(line.positionCount - 1, newCell);
        previousCellPosition = newCell;
    }
}