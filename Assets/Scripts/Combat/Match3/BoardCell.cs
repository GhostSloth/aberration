using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCell : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public void SetCoord(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
}
