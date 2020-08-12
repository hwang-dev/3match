using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    public GameObject tilePrefab;
    public GameObject[] gamePiecePrefabs;

    public float swapTime = 0.5f;
    public int boardSize;

    private Tile[,] m_allTiles;
    private GamePiece[,] m_allGamePieces;

    Tile m_clickedTile;
    Tile m_targetTile;

    // Start is called before the first frame update
    void Start()
    {
        m_allTiles = new Tile[width, height];
        m_allGamePieces = new GamePiece[width, height];

        SetupTiles();
        SetupCamera();
        FillRandom();
    }


    void SetupTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var tile = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity, this.transform) as GameObject;
                if (tile != null)
                {
                    tile.name = $"Tile({i},{j})";
                    var component = tile.GetComponent<Tile>();
                    if (component != null)
                    {
                        m_allTiles[i, j] = component;
                        m_allTiles[i, j].Init(i, j, this);
                    }
                }
            }
        }
    }

    void SetupCamera()
    {
        Camera.main.transform.position = new Vector3((float)(width - 1) / 2f, (float)(height - 1) / 2f, -10f);
        float ratio = (float)Screen.width / (float)Screen.height;

        float verticalSize = (float)height / 2f + (float)boardSize;
        float horizontalSize = ((float)width / 2f + (float)boardSize) / ratio;

        Camera.main.orthographicSize = verticalSize > horizontalSize ? verticalSize : horizontalSize;
    }

    GameObject GetRandomGamePiece()
    {
        int randomIndex = UnityEngine.Random.Range(0, gamePiecePrefabs.Length);

        if (gamePiecePrefabs[randomIndex] == null)
        {
            Debug.Log("----> ");
        }

        return gamePiecePrefabs[randomIndex];
    }

    public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        if (gamePiece == null)
        {
            return;
        }

        gamePiece.transform.position = new Vector3(x, y, 0);
        gamePiece.transform.rotation = Quaternion.identity;

        if (IsWithBounds(x, y)) m_allGamePieces[x, y] = gamePiece;

        gamePiece.SetCoord(x, y);
    }

    void FillRandom()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject randomPiece = Instantiate(GetRandomGamePiece(), Vector3.zero, Quaternion.identity) as GameObject;
                if (randomPiece != null)
                {
                    PlaceGamePiece(randomPiece.GetComponent<GamePiece>(), i, j);
                    randomPiece.GetComponent<GamePiece>().Init(this);
                    randomPiece.transform.parent = this.transform;
                }
            }
        }
    }

    public void ClickTile(Tile tile)
    {
        if (m_clickedTile == null)
        {
            m_clickedTile = tile;
            Debug.Log($"----> clicked Tile: {m_clickedTile.name}");
        }
    }
    
    bool IsWithBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    public void DragToTile(Tile tile)
    {
        if (m_clickedTile != null && IsNextTo(m_clickedTile, m_targetTile))
            m_targetTile = tile;
    }

    public void ReleaseTile()
    {
        if (m_clickedTile != null && m_targetTile != null)
        {
            SwitchTiles(m_clickedTile, m_targetTile);
        }

        m_clickedTile = null;
        m_targetTile = null;
    }

    private void SwitchTiles(Tile clickedTile, Tile targetTile)
    {
        GamePiece clickedPices = m_allGamePieces[clickedTile.xIndex, clickedTile.yIndex];
        GamePiece targetPieces = m_allGamePieces[targetTile.xIndex, targetTile.yIndex];

        clickedPices.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
        targetPieces.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);
    }

    private bool IsNextTo(Tile start, Tile end)
    {
        if (Mathf.Abs(start.xIndex - end.xIndex) == 1 && start.yIndex == end.yIndex) return true;
        if (Mathf.Abs(start.yIndex - end.yIndex) == 1 && start.xIndex == end.yIndex) return true;

        return false;
    }

    List<GamePiece> FindMatches(int startX, int startY, Vector2 SearchDirection, int minLength = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();
        GamePiece startPiece = null;

        if (IsWithBounds(startX, startY))
            startPiece = m_allGamePieces[startX, startY];
 
        if (startPiece != null)
            matches.Add(startPiece);
        else
            return null;

        int nextX = 0;
        int nextY = 0;


        int maxValue = width > height ? width : height;

        for (int i = 0; i < maxValue - 1; i++)
        {
            nextX = startX + (int)Mathf.Clamp(SearchDirection.x, -1, 1) * i;
            nextY = startY + (int)Mathf.Clamp(SearchDirection.y, -1, 1) * i;

            if (!IsWithBounds(nextX, nextY))
                break;

            GamePiece nextPices = m_allGamePieces[nextX, nextY];

            if (nextPices.matchValue == startPiece.matchValue && !matches.Contains(nextPices))
                matches.Add(nextPices);
            else
                break;
        }

        if (matches.Count >= minLength)
            return matches;

        return null;
    }
}
