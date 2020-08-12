using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    public int xIndex;
    public int yIndex;

    Board m_board;

    bool m_isMoving = false;
    public InterpType interpolation = InterpType.SmootherStep;

    public enum InterpType
    {
        Linear,
        EaseOut,
        EaseIn,
        SmoothStep,
        SmootherStep
    }

    public MatchValue matchValue;
    public enum MatchValue
    {
        Yellow,
        Blue,
        Magenta,
        Indigo,
        Green,
        Teal,
        Red,
        Cyan,
        Wild
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    Move((int)this.transform.position.x + 1, (int)this.transform.position.y, 1f);
        //}
    }

    public void Init(Board board)
    {
        m_board = board;
    }

    public void SetCoord(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    public void Move(int destX, int destY, float timeToMove)
    {
        if (m_isMoving == false)
            StartCoroutine(MoveRoutine(new Vector3(destX, destY, 0), timeToMove));
    }

    IEnumerator MoveRoutine(Vector3 dest, float timeToMove)
    {
        Vector3 startPos = transform.position;
        bool reachDest = false;
        float elapsedTime = 0f;
        m_isMoving = true;

        while (reachDest == false)
        {
            // do something
            if (Vector3.Distance(transform.position, dest) < 0.01f)
            {
                reachDest = true;

                if (m_board != null) m_board.PlaceGamePiece(this, (int)dest.x, (int)dest.y);
            }

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp(elapsedTime / timeToMove, 0f, 1f);

            switch (interpolation)
            {
                case InterpType.Linear:
                    break;
                case InterpType.EaseIn:
                    t = 1 - Mathf.Cos(t * Mathf.PI * 0.5f);
                    break;
                case InterpType.EaseOut:
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);
                    break;
                case InterpType.SmoothStep:
                    t = t * t * (3 - 2 * t);
                    break;
                case InterpType.SmootherStep:
                    t = t * t * t * (t * (t * 6 - 15) + 10);
                    break;
            }

            this.transform.position = Vector3.Lerp(startPos, dest, t);

            // wait until next frame
            yield return null;
        }

        yield break;
    }
}