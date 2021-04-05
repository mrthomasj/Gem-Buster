using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    [SerializeField] private GameObject tilePrefab;
    public GameObject[] gems;
    private BgTile[,] board;
    public GameObject[,] gemStates;
    // Start is called before the first frame update
    void Start()
    {
        board = new BgTile[width, height];
        gemStates = new GameObject[width, height];
        BoardSetup();
    }

    private void BoardSetup()
    {
        for(int w = 0; w < width; w++)
        {
            for(int h = 0; h < height; h++)
            {
                Vector2 tempPos = new Vector2(w, h);
                var boardTile = Instantiate(tilePrefab, tempPos, Quaternion.identity, this.transform);
                boardTile.name = $"({w}, {h})";
                int gemToUse = Random.Range(0, gems.Length);
                int maxIterations = 0;
                while (MatchesAt(w, h, gems[gemToUse]) && maxIterations < 100)
                {
                    gemToUse = Random.Range(0, gems.Length);
                    maxIterations++;
                }
                var gem = Instantiate(gems[gemToUse], tempPos, Quaternion.identity, this.transform);
                gem.name = $"({w}, {h})";
                gemStates[w, h] = gem;
            }
        }
    }

    private bool MatchesAt(int col, int row, GameObject gem)
    {
        if(col > 1 && row > 1)
        {
            if(gemStates[col -1, row].tag == gem.tag && gemStates[col -2, row].tag == gem.tag)
            {
                return true;
            }
            
            if(gemStates[col, row - 1].tag == gem.tag && gemStates[col, row - 2].tag == gem.tag)
            {
                return true;
            }
        }
        else if ( col <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if(gemStates[col, row -1].tag == gem.tag && gemStates[col, row -2].tag == gem.tag)
                {
                    return true;
                }
            }
            if(col > 1)
            {
                if (gemStates[col - 1, row].tag == gem.tag && gemStates[col - 1, row].tag == gem.tag)
                {
                    return true;
                } 
            }
        }

        return false;
    }

    private void DestroyMatchAt(int col, int row)
    {
        if(gemStates[col, row].GetComponent<Gem>().isMatch)
        {
            Destroy(gemStates[col, row]);
            gemStates[col, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for(int w = 0; w < width; w++)
        {
            for(int h = 0; h < height; h++)
            {
                if(gemStates[w, h] != null)
                {
                    DestroyMatchAt(w, h);
                }
            }
        }
        StartCoroutine("CollapseColumnsCoroutine");
    }

    private IEnumerator CollapseColumnsCoroutine()
    {
        int nullCount = 0;
        for(int w = 0; w < width; w++)
        {
            for(int h = 0;  h < height; h++)
            {
                if(gemStates[w,h] == null)
                {
                    nullCount++;
                }else if(nullCount > 0)
                {
                    gemStates[w, h].GetComponent<Gem>().row -= nullCount;
                    gemStates[w, h] = null;
                }
            }
            nullCount = 0;
        }
        
        yield return new WaitForSeconds(.5f);

        StartCoroutine("FillBoardCoroutine");
    }

    private void RefillBoard()
    {
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                if (gemStates[w, h] == null)
                {
                    Vector2 tempPos = new Vector2(w, h);
                    int gemToUse = Random.Range(0, gems.Length);
                    GameObject piece = Instantiate(gems[gemToUse], tempPos, Quaternion.identity);
                    gemStates[w, h] = piece;
                }
            }
        }
    }


    private bool MatchesOnBoard()
    {
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            { 
                if(gemStates[w,h] != null)
                {
                    if (gemStates[w, h].GetComponent<Gem>().isMatch)
                    {
                        return true;
                    }
                }
            }
        }
                return false;
    }

    private IEnumerator FillBoardCoroutine()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);
        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
    }
}
