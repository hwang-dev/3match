﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int xIndex;
    public int yIndex;

    Board m_board;

    private void Start()
    {
        
    }

    public void Init(int x, int y, Board board)
    {
        xIndex = x;
        yIndex = y;
        m_board = board;
    }

    public void OnMouseDown()
    {
        if(m_board != null)
        {
            m_board.ClickTile(this);
        }
    }

    public void OnMouseEnter()
    {
        if(m_board != null)
        {
            m_board.DragToTile(this);
        }
    }

    public void OnMouseUp()
    {
        if (m_board != null)
        {
            m_board.ReleaseTile();
        }
    }
}
