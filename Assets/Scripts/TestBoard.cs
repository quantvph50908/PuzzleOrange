using System.Collections.Generic;
using UnityEngine;

public class TestBoard : MonoBehaviour
{
    public int gridSize = 4;
    public int tileSize = 1; 
    public GameObject orangePiece1Prefab;
    public GameObject orangePiece2Prefab; 
    public GameObject orangePiece3Prefab; 
    public GameObject orangePiece4Prefab;
    public WinChecker winChecker; 
    private TimerManager timerManager; 

    public List<GameObject> orangePieces = new List<GameObject>(); 
    private List<Sprite> initialSprites = new List<Sprite>(); 
    private List<(int row, int col, GameObject prefab, bool isMovable)> initialSetup = new List<(int, int, GameObject, bool)>();
    private GameObject selectedPiece;
    private Vector2 startMousePos;

    void Start()
    {
        timerManager = FindObjectOfType<TimerManager>(); 

        initialSetup.Add((0, 2, orangePiece1Prefab, true)); 
        initialSetup.Add((1, 2, orangePiece2Prefab, false));
        initialSetup.Add((3, 0, orangePiece3Prefab, false)); 
        initialSetup.Add((2, 2, orangePiece4Prefab, false)); 

        SetupLevel1();
    }

    void Update()
    {
        // Bắt đầu kéo: click chuột trái
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null)
            {
                selectedPiece = hit.collider.gameObject;
                startMousePos = mousePos;
            }
        }
        // Kết thúc kéo: thả chuột
        else if (Input.GetMouseButtonUp(0) && selectedPiece != null)
        {
            Vector2 endMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 delta = endMousePos - startMousePos;

            // Xác định hướng kéo (4 hướng) dựa trên delta thực tế
            Vector2Int direction = Vector2Int.zero;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                direction = delta.x > 0 ? new Vector2Int(0, 1) : new Vector2Int(0, -1); // Phải hoặc trái
            }
            else if (Mathf.Abs(delta.y) > 0)
            {
                direction = delta.y > 0 ? new Vector2Int(1, 0) : new Vector2Int(-1, 0); // Lên hoặc xuống
            }

            // Chỉ di chuyển nếu có hướng rõ ràng và delta đủ lớn
            if (direction != Vector2Int.zero && (Mathf.Abs(delta.x) > 0.5f || Mathf.Abs(delta.y) > 0.5f))
            {
                MovePieces(direction);
            }

            // Reset sau khi thả chuột
            selectedPiece = null;

            if (winChecker != null) winChecker.CheckWin();
        }
    }

    void SetupLevel1()
    {

        foreach (var setup in initialSetup)
        {
            AddOrangePiece(setup.row, setup.col, setup.prefab, setup.isMovable);
        }

 
        initialSprites.Clear();
        foreach (var piece in orangePieces)
        {
            SpriteRenderer renderer = piece.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                initialSprites.Add(renderer.sprite);
            }
        }
    }

    void AddOrangePiece(int row, int col, GameObject prefab, bool isMovable = false)
    {

        int x = col - (gridSize - 1) / 2; 
        int y = row - (gridSize - 1) / 2;
        GameObject piece = Instantiate(prefab, new Vector3(x * tileSize, y * tileSize, -1), Quaternion.identity);
        piece.name = $"Piece_{row}_{col}";
        orangePieces.Add(piece);
        piece.AddComponent<BoxCollider2D>().size = new Vector2(tileSize - 0.1f, tileSize - 0.1f);
    }



    void MovePieces(Vector2Int direction)
    {

        List<(GameObject piece, Vector2Int gridPos)> piecePositions = new List<(GameObject, Vector2Int)>();
        foreach (var piece in orangePieces)
        {
            Vector2 pos = piece.transform.position;
            Vector2Int gridPos = new Vector2Int(
                Mathf.RoundToInt(pos.y / tileSize + (gridSize - 1) / 2),
                Mathf.RoundToInt(pos.x / tileSize + (gridSize - 1) / 2)
            );
            piecePositions.Add((piece, gridPos));
        }

        GameObject[] fixedBlocksInScene = GameObject.FindGameObjectsWithTag("FixedBlock");
        List<Vector2Int> blockPositions = new List<Vector2Int>();
        foreach (var block in fixedBlocksInScene)
        {
            Vector2 pos = block.transform.position;
            Vector2Int gridPos = new Vector2Int(
                Mathf.RoundToInt(pos.y / tileSize + (gridSize - 1) / 2),
                Mathf.RoundToInt(pos.x / tileSize + (gridSize - 1) / 2)
            );
            blockPositions.Add(gridPos);
        }


        List<bool> canMove = new List<bool>();
        for (int i = 0; i < orangePieces.Count; i++)
        {
            var (piece, gridPos) = piecePositions[i];
            int newRow = gridPos.x + direction.x;
            int newCol = gridPos.y + direction.y; 

            if (newRow < 0 || newRow >= gridSize || newCol < 0 || newCol >= gridSize)
            {
                canMove.Add(false);
                continue;
            }

 
            bool isBlockedByPiece = false;
            foreach (var (otherPiece, otherPos) in piecePositions)
            {
                if (otherPiece != piece && otherPos.x == newRow && otherPos.y == newCol)
                {
                    isBlockedByPiece = true;
                    break;
                }
            }

            bool isBlockedByBlock = false;
            foreach (var blockPos in blockPositions)
            {
                if (blockPos.x == newRow && blockPos.y == newCol)
                {
                    isBlockedByBlock = true;
                    break;
                }
            }

            canMove.Add(!isBlockedByPiece && !isBlockedByBlock);
        }

        for (int i = 0; i < orangePieces.Count; i++)
        {
            if (canMove[i])
            {
                var (piece, gridPos) = piecePositions[i];
                int newRow = gridPos.x + direction.x;
                int newCol = gridPos.y + direction.y;
                int newX = newCol - (gridSize - 1) / 2;
                int newY = newRow - (gridSize - 1) / 2;
                piece.transform.position = new Vector3(newX * tileSize, newY * tileSize, piece.transform.position.z);
            }
        }
    }
}