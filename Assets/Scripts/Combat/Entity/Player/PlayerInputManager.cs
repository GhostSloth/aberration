using GhostSloth.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public System.Action usedAction;

    public bool IsSlctdCellNull => slctdCell.x == -1 || slctdCell.y == -1;

    [SerializeField]
    private BoardObject board;

    [SerializeField]
    private Vector2Int slctdCell;

    private ITurnInfo turnInfo;

    public void SetTurnInfo(ITurnInfo turnInfo)
    {
        this.turnInfo = turnInfo;
    }

    public void UseSkill()
    {
        usedAction.Invoke();
    }

    private void Update()
    {
        if (turnInfo.IsEnemyTurn) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            MouseDown();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            MouseUp();
        }
    }

    private void MouseDown()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.OverlapPoint(pos);

        if (hit)
        {
            var mutagen = hit.gameObject.GetComponent<BoardCell>();
            slctdCell.x = mutagen.X;
            slctdCell.y = mutagen.Y;
        }
    }

    private void MouseUp()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.OverlapPoint(pos);

        if (hit)
        {
            var slot = hit.gameObject.GetComponent<BoardCell>();
            if (board.Swap(slctdCell, new(slot.X, slot.Y)))
            {
                usedAction?.Invoke();
            }
        }

        ResetSlctdCell();
    }

    private void ResetSlctdCell()
    {
        slctdCell.x = slctdCell.y = -1;
    }
}
