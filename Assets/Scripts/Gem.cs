using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [Header("Board Variables")]
    public int column; // x pos
    public int row; //  y pos
    public int prevColumn;
    public int prevRow;
    public int targetX;
    public int targetY;

    public bool isMatch = false;
    private Board board;
    private GameObject targetGem;
    private Vector2 startTouchPos;
    private Vector2 finalTouchPos;

    private Vector2 tempPosition;

    public float swipeResist = 1f;
    public float swipeAngle = 0f;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;

        row = targetY;
        column = targetX;
        prevColumn = column;
        prevRow = row;
    }

    // Update is called once per frame
    void Update()
    {
        CheckMatch();
        if (isMatch)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.color = new Color(2f, 2f, 2f, .7f);
        }
        targetX = column;
        targetY = row;

        MoveGem();
        
    }

    private void OnMouseDown()
    {
        startTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(startTouchPos);
    }

    private void OnMouseUp()
    {
        finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GetAngle();
        
    }

    private void GetAngle()
    {
        if(Mathf.Abs(finalTouchPos.y - startTouchPos.y) > swipeResist || Mathf.Abs(finalTouchPos.x - startTouchPos.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPos.y - startTouchPos.y, finalTouchPos.x - startTouchPos.x) * Mathf.Rad2Deg;
            Debug.Log(swipeAngle);
            MovePieces();
        }
        
    }

    private void MovePieces()
    {
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width -1)
        {
            //Right Swipe
            targetGem = board.gemStates[column + 1, row];
            targetGem.GetComponent<Gem>().column -= 1;
            column += 1;
        }else if(swipeAngle > 45 && swipeAngle <= 135 && row < board.height)
        {
            //Up Swipe
            targetGem = board.gemStates[column, row+1];
            targetGem.GetComponent<Gem>().row -= 1;
            row += 1;
        }else if(swipeAngle > 135 || swipeAngle <= -135 && column > 0)
        {
            //Left Swipe
            targetGem = board.gemStates[column - 1, row];
            targetGem.GetComponent<Gem>().column += 1;
            column -= 1;
        }else if(swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //Down Swipe
            targetGem = board.gemStates[column, row -1];
            targetGem.GetComponent<Gem>().row += 1;
            row -= 1;
        }
    }

    private void MoveGem()
    {
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //Move towards target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if(board.gemStates[column, row] != this.gameObject)
            {
                board.gemStates[column, row] = this.gameObject;
            }
        }
        else
        {
            //Set position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            //board.gemStates[column, row] = this.gameObject;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //Move towards target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.gemStates[column, row] != this.gameObject)
            {
                board.gemStates[column, row] = this.gameObject;
            }
        }
        else
        {
            //Set position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            //board.gemStates[column, row] = this.gameObject;
        }

        StartCoroutine("CheckMoveCoroutine");
    }

    private void CheckMatch()
    {
        if(column > 0 && column < board.width -1)
        {
            GameObject leftGem1 = board.gemStates[column - 1, row];
            GameObject rightGem1 = board.gemStates[column + 1, row];
            if(leftGem1 != null && rightGem1 != null)
            {
                if (leftGem1.tag == this.gameObject.tag && rightGem1.tag == this.gameObject.tag)
                {
                    leftGem1.GetComponent<Gem>().isMatch = true;
                    rightGem1.GetComponent<Gem>().isMatch = true;
                    isMatch = true;
                }
            }
        }

        if (row > 0 && row < board.height - 1)
        {
            GameObject downGem1 = board.gemStates[column, row - 1];
            GameObject upGem1 = board.gemStates[column, row + 1];
            if(downGem1 != null && upGem1 != null)
            {
                if (downGem1.tag == this.gameObject.tag && upGem1.tag == this.gameObject.tag)
                {
                    downGem1.GetComponent<Gem>().isMatch = true;
                    upGem1.GetComponent<Gem>().isMatch = true;
                    isMatch = true;
                }
            }
        }
    }

    public IEnumerator CheckMoveCoroutine()
    {
        
        yield return new WaitForSeconds(.5f);
        if (targetGem != null)
        {
            var targetGemScript = targetGem.GetComponent<Gem>();
            if (!isMatch && !targetGemScript.isMatch)
            {
                targetGemScript.row = row;
                targetGemScript.column = column;

                row = prevRow;
                column = prevColumn;
            }
            else
            {
                board.DestroyMatches();
            }
            targetGem = null;
        }
        
    }
}
