using System.Collections.Generic;
using UnityEngine;

public class WinChecker : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject orangePiece1Prefab; 
    public GameObject orangePiece2Prefab;
    public GameObject orangePiece3Prefab;
    public GameObject orangePiece4Prefab;
    public GameObject gameObjectToDisable;
    private TestBoard testBoard;
    private TimerManager timerManager;
    private Sprite[] winningSprites; 
    private bool gameEnded = false;

    void Start()
    {
        testBoard = FindObjectOfType<TestBoard>();
        timerManager = FindObjectOfType<TimerManager>();

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }


        if (gameObjectToDisable != null)
        {
            gameObjectToDisable.SetActive(true); 
        }


        winningSprites = new Sprite[4];
        winningSprites[0] = orangePiece1Prefab.GetComponent<SpriteRenderer>().sprite; 
        winningSprites[1] = orangePiece2Prefab.GetComponent<SpriteRenderer>().sprite; 
        winningSprites[2] = orangePiece3Prefab.GetComponent<SpriteRenderer>().sprite; 
        winningSprites[3] = orangePiece4Prefab.GetComponent<SpriteRenderer>().sprite; 

    }

    public void CheckWin()
    {
        if (gameEnded || (timerManager != null && timerManager.IsGameOver)) return;

        if (testBoard == null || testBoard.orangePieces == null)
        {
            return;
        }

        Dictionary<Vector2Int, Sprite> piecePositions = new Dictionary<Vector2Int, Sprite>();
        foreach (var piece in testBoard.orangePieces)
        {
            if (piece == null)
            {
                continue;
            }
            SpriteRenderer renderer = piece.GetComponent<SpriteRenderer>();
            if (renderer != null && renderer.sprite != null)
            {
                Vector2Int gridPos = WorldToGridPos(piece.transform.position, testBoard.gridSize, testBoard.tileSize);
                if (piecePositions.ContainsKey(gridPos))
                {
                    return; 
                }
                piecePositions[gridPos] = renderer.sprite;

            }

        }

        bool hasWon = false;
        for (int row = 0; row <= testBoard.gridSize - 2; row++)
        {
            for (int col = 0; col <= testBoard.gridSize - 2; col++)
            {

                if (piecePositions.TryGetValue(new Vector2Int(row, col), out Sprite sprite1) &&
                    piecePositions.TryGetValue(new Vector2Int(row, col + 1), out Sprite sprite2) &&
                    piecePositions.TryGetValue(new Vector2Int(row + 1, col), out Sprite sprite3) &&
                    piecePositions.TryGetValue(new Vector2Int(row + 1, col + 1), out Sprite sprite4))
                {
  
                    if (sprite1 == winningSprites[0] &&
                        sprite2 == winningSprites[1] &&
                        sprite3 == winningSprites[2] && 
                        sprite4 == winningSprites[3])   
                    {
                        hasWon = true;
                        break;
                    }
                }
            }
            if (hasWon) break;
        }


        if (hasWon)
        {
            EndGame(true);
        }
    }

    private void EndGame(bool won)
    {
        gameEnded = true;
        if (timerManager != null)
        {
            timerManager.StopTimer(); 
        }

        if (gameObjectToDisable != null)
        {
            gameObjectToDisable.SetActive(false); 
        }

        if (won)
        {
            if (winPanel != null)
            {
                winPanel.SetActive(true);
            }
        }
    }

    public void ResetGame()
    {
        gameEnded = false;
        if (winPanel != null) winPanel.SetActive(false);
        if (gameObjectToDisable != null) gameObjectToDisable.SetActive(true); 
    }

    private Vector2Int WorldToGridPos(Vector2 worldPos, int gridSize, int tileSize)
    {
        int row = Mathf.RoundToInt(worldPos.y / tileSize + (gridSize - 1) / 2);
        int col = Mathf.RoundToInt(worldPos.x / tileSize + (gridSize - 1) / 2);
        return new Vector2Int(row, col);
    }
}