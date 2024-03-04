using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GhostSloth
{
    public enum MutagenType { none, blue, green, red, yellow, purple }

    public class MutagenData
    {
        public MutagenType type;
        public MutagenGameObject obj;

        public MutagenData(MutagenType type)
        {
            this.type = type;
        }
    }

    public class Board
    {
        public readonly int colNum;
        public readonly int rowNum;

        private MutagenData[,] grid;

        public Board(int colNum = 5, int rowNum = 5)
        {
            this.colNum = colNum;
            this.rowNum = rowNum;

            GenerateGrid();
        }

        public void GenerateGrid()
        {
            grid = new MutagenData[colNum, rowNum];

            for (int y = 0; y < colNum; ++y)
            {
                for (int x = 0; x < rowNum; ++x)
                {
                    grid[y, x] = new((MutagenType)Random.Range(1, 6));
                }
            }

            PreventMatchInRow();
            PreventMatchInCol();
        }

        public MutagenData this[int y, int x]
        {
            get { return grid[y, x]; }
            set { grid[y, x] = value; }
        }

        public List<Match> CheckRows()
        {
            List<Match> matches = new List<Match>();
            for (int y = 0; y < colNum; ++y)
            {
                var curType = grid[y, 0]?.type ?? MutagenType.none;
                int count = 1;
                int start = 0;
                for (int x = 1; x < rowNum; ++x)
                {
                    if (grid[y, x]?.type == curType)
                    {
                        ++count;
                    }
                    else
                    {
                        if (count >= 3 && curType != MutagenType.none)
                        {
                            matches.Add(new Match(start, y, x - 1, y, curType));
                        }

                        count = 1;
                        start = x;
                        curType = grid[y, x]?.type ?? MutagenType.none;
                    }
                }

                if (count >= 3 && curType != MutagenType.none)
                {
                    matches.Add(new Match(start, y, rowNum - 1, y, curType));
                }
            }
            return matches;
        }

        public List<Match> CheckCols()
        {
            List<Match> matches = new List<Match>();

            for (int x = 0; x < rowNum; ++x)
            {
                var curType = grid[0, x].type;
                int count = 1;
                int start = 0;

                for (int y = 1; y < colNum; ++y)
                {
                    if (grid[y, x]?.type == curType)
                    {
                        ++count;
                    }
                    else
                    {
                        if (count >= 3 && curType != MutagenType.none)
                        {
                            matches.Add(new(x, start, x, y - 1, curType));
                        }

                        count = 1;
                        start = y;
                        curType = grid[y, x]?.type ?? MutagenType.none;
                    }
                }

                if (count >= 3 && curType != MutagenType.none)
                {
                    matches.Add(new(x, start, x, colNum - 1, curType));
                }
            }

            return matches;
        }
        
        public bool Swap(int aX, int aY, int bX, int bY)
        {
            if (GetDistance(aX, aY, bX, bY) > 1) { return false; }

            var type = grid[aY, aX];
            grid[aY, aX] = grid[bY, bX];
            grid[bY, bX] = type;

            return true;
        }

        private void PreventMatchInRow()
        {
            for (int y = 0; y < colNum; ++y)
            {
                var curType = grid[y, 0].type;
                int count = 1;

                for (int x = 1; x < rowNum; ++x)
                {
                    if (curType == grid[y, x].type)
                    {
                        ++count;

                        if (count >= 3)
                        {
                            int rnd = (((int)curType - 1) + Random.Range(1, 5)) % 5;
                            grid[y, x].type = (MutagenType)(rnd + 1);

                            count = 1;
                            curType = grid[y, x].type;
                        }
                    }
                    else
                    {
                        count = 1;
                        curType = grid[y, x].type;
                    }
                }
            }
        }

        private void PreventMatchInCol()
        {
            for (int x = 0; x < rowNum; ++x)
            {
                var curType = grid[0, x].type;
                int count = 1;

                for (int y = 0; y < colNum; ++y)
                {
                    if (curType == grid[y, x].type)
                    {
                        ++count;

                        if (count >= 3)
                        {
                            int rnd = (((int)curType - 1) + Random.Range(1, 5)) % 5;
                            grid[y, x].type = (MutagenType)(rnd + 1);

                            count = 1;
                            curType = grid[y, x].type;
                        }
                    }
                    else
                    {
                        count = 1;
                        curType = grid[y, x].type;
                    }
                }
            }
        }

        private int GetDistance(int aX, int aY, int bX, int bY)
        {
            return Mathf.Abs(aX - bX) + Mathf.Abs(aY - bY);
        }
    }

    public struct Match
    {
        public int startX, startY, endX, endY;
        public MutagenType type;

        public Match(int startX, int startY, int endX, int endY, MutagenType type)
        {
            this.startX = startX;
            this.startY = startY;
            this.endX = endX;
            this.endY = endY;
            this.type = type;
        }
    }

    public class ViewManager
    {
        public bool IsInAnimation { get; private set; }
        public System.Action animEnded;
        public System.Action<bool> moveDownEnded;

        private List<Transform> moveDown = new List<Transform>();
        private List<Vector2> moveDownStart = new List<Vector2>();
        private List<Vector2> moveDownEnd = new List<Vector2>();

        private const float downDur = .175f;
        private float downT;

        public IEnumerator Spaw(Transform a, Transform b)
        {
            IsInAnimation = true;

            float dur = .15f;
            float t = 0;
            Vector2 start = a.position;
            Vector2 end = b.position;

            while (t < dur)
            {
                t += Time.deltaTime;

                a.position = Vector2.Lerp(start, end, t / dur);
                b.position = Vector2.Lerp(end, start, t / dur);

                yield return null;
            }

            a.position = end;
            b.position = start;

            IsInAnimation = false;

            animEnded?.Invoke();
        }

        public void InitMoveDown()
        {
            moveDown.Clear();
            moveDownStart.Clear();
            moveDownEnd.Clear();

            downT = 0;
        }

        public void AddToMoveDown(Transform mutagen)
        {
            moveDown.Add(mutagen);
            moveDownStart.Add(mutagen.position);
            moveDownEnd.Add(mutagen.position + Vector3.down);
        }

        public void StartMoveDown()
        {
            if (moveDown.Count == 0)
            {
                IsInAnimation = false;
                moveDownEnded?.Invoke(false);
                return;
            }

            IsInAnimation = true;
        }

        public void Update()
        {
            if (IsInAnimation && moveDown.Count > 0)
            {
                downT += Time.deltaTime;

                if (downT < downDur)
                {
                    float elipse = downT / downDur;
                    for (int i = 0; i < moveDown.Count; ++i)
                    {
                        moveDown[i].position = Vector2.Lerp(moveDownStart[i], moveDownEnd[i], elipse);
                    }
                }
                else
                {
                    for (int i = 0; i < moveDown.Count; ++i)
                    {
                        moveDown[i].position = moveDownEnd[i];
                    }
                    EndMoveDown();
                }
            }
        }
    
        private void EndMoveDown()
        {
            IsInAnimation = false;
            InitMoveDown();
            moveDownEnded?.Invoke(true);
        }
    }
}
