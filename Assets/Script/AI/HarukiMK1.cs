using UnityEngine;
using System.Collections;

public class HarukiMK1 : MonoBehaviour
{
    public int MyTeam = 0;
    private Selector _selector;
    private Tile_data _tileData;
    private Turn_Controller _turnController;

    private int[,] _evaluationArray = {
        { 30,-12,  0, -1, -1,  0,-12, 30},
        {-12,-15, -3, -3, -3, -3,-15,-12},
        {  0, -3,  0, -1, -1,  0, -3,  0},
        { -1, -3, -1, -1, -1, -1, -3, -1},
        { -1, -3, -1, -1, -1, -1, -3, -1},
        {  0, -3,  0, -1, -1,  0, -3,  0},
        {-12,-15, -3, -3, -3, -3,-15,-12},
        { 30,-12,  0, -1, -1,  0,-12, 30}
    };

    //自分のターンが始まるまで待機
    IEnumerator WaitTurn()
    {
        while (_turnController.now_team == MyTeam)
        {
            yield return new WaitForSeconds(1f);
        }
    }

    //評価開始
    IEnumerator Assessment()
    {
        yield return WaitTurn();
        Debug.Log("俺のた～ん");
        yield return new WaitForSeconds(2f);
        bool[,] placableArray = _tileData.GetPlaceableArray();
        //for (int y = 1; y < 10; y++)
        //{
        //    string shuturyoku = "";
        //    for (int x = 1; x < 10; x++)
        //    {
        //        shuturyoku = shuturyoku + placableArray[y, x];
        //    }
        //        Debug.Log(shuturyoku);
        //}
    }

    IEnumerator Loop()
    {
        while (true)
        {
            yield return Assessment();
        }
    }

    // Use this for initialization
	void Start ()
	{
	    //インスタンス化
	    _selector = GetComponent<Selector>();
	    _tileData = GetComponent<Tile_data>();
	    _turnController = GetComponent<Turn_Controller>();
	    StartCoroutine(Loop());
	}

	// Update is called once per frame
	void Update () {

	}
}
