using GhostSloth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardObject : MonoBehaviour
{
    public int ColNum => board.colNum;
    public int RowNum => board.rowNum;

    [SerializeField]
    private Transform cellParent;

    [SerializeField]
    private MutagenFactory mutagenFactory;

    [SerializeField]
    private BoardCell mutagenPrefab;

    private Board board;
    private ViewManager viewManager = new ViewManager();

    [SerializeField]
    private bool isUpdatingBoard = false;

    public bool Swap(Vector2Int a, Vector2Int b)
    {
        if (viewManager.IsInAnimation || isUpdatingBoard) { return false; }

        isUpdatingBoard = board.Swap(a.x, a.y, b.x, b.y);

        if (isUpdatingBoard)
        {
            StartCoroutine(viewManager.Spaw(board[a.y,a.x].obj.transform, board[b.y, b.x].obj.transform));
        }

        return isUpdatingBoard;
    }

    private void Awake()
    {
        board = new Board(5, 5);
    }

    private void Start()
    {
        viewManager.animEnded = CheckMatches;
        viewManager.moveDownEnded = OnMoveDownEnd;
        SetUpGrid();
    }

    private void Update()
    {
        viewManager.Update();
    }

    private void SetUpGrid()
    {
        for (int y = 0; y < ColNum; y++)
        {
            for (int x = 0; x < RowNum; x++)
            {
                var cell = Instantiate(mutagenPrefab, cellParent);
                cell.SetCoord(x, y);
                cell.transform.position = new Vector3(x, y);

                board[y, x].obj = mutagenFactory.GetMutagen(board[y, x].type);

                board[y, x].obj.transform.position = new(x, y);
            }
        }
    }

    private void CheckMatches()
    {
        var matches = board.CheckRows();
        matches.AddRange(board.CheckCols());

        int[] mutagens = new int[5];

        foreach (var match in matches)
        {
            for (int y = match.startY; y <= match.endY; ++y)
            {
                for (int x = match.startX; x <= match.endX; ++x)
                {
                    if (board[y, x] == null) { continue; }

                    ++mutagens[(int)board[y, x].type - 1];

                    Destroy(board[y, x].obj.gameObject);
                    board[y, x] = null;
                }
            }
        }

        for (int i = 0; i < mutagens.Length; ++i)
        {
            if (mutagens[i] == 0) { continue; }

            // add mutagens
        }

        if (matches.Count > 0)
        {
            StartMoveDown();
            return;
        }

        isUpdatingBoard = false;
    }

    private void StartMoveDown()
    {
        viewManager.InitMoveDown();

        for (int x = 0; x < board.rowNum; ++x)
        {
            for (int y = 1; y < board.colNum; ++y)
            {
                if (board[y, x] != null && board[y - 1, x] == null)
                {
                    viewManager.AddToMoveDown(board[y, x].obj.transform);
                    board[y - 1, x] = board[y, x];
                    board[y, x] = null;
                }
            }
        }

        viewManager.StartMoveDown();
    }

    private void OnMoveDownEnd(bool moved)
    {
        if (SpawnNewMutagens() || moved)
        {
            StartMoveDown();
        }
        else
        {
            CheckMatches();
        }
    }

    private bool SpawnNewMutagens()
    {
        int y = board.colNum - 1;
        bool spawned = false;

        for (int x = 0; x < board.rowNum; ++x)
        {
            if (board[y, x] == null)
            {
                board[y, x] = new MutagenData(GetRndMutagenType());
                board[y, x].obj = mutagenFactory.GetMutagen(board[y, x].type);
                board[y, x].obj.transform.position = new(x, y);

                spawned = true;
            }
        }

        return spawned;
    }

    private MutagenType GetRndMutagenType()
    {
        return (MutagenType)Random.Range(1, 6);
    }

}
