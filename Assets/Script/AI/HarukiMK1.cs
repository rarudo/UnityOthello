using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HarukiMK1 : MonoBehaviour
{
    public char MyTeam;
    private Selector _selector;
    private Tile_data _tileData;
    private Turn_Controller _turnController;
    private List<Vector2> _solution = new List<Vector2>();

    //

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

    private struct evaluationStruct{
        public evaluationStruct(int x,int y,int evaluation)
        {
            X = x;
            Y = y;
            Evaluation = evaluation;
        }
        //x座標、y座標、重みを格納する
         public int X;
         public int Y;
         public int Evaluation;
    }



    //自分のターンが始まるまで待機
    IEnumerator WaitTurn()
    {
            yield return new WaitForSeconds(0.1f);
       // while (_turnController.now_team != MyTeam)
       // {
       //     yield return new WaitForSeconds(1f);
       // }
    }

    //評価開始
    IEnumerator Assessment()
    {
        //構造体の情報をリストに追加
        List<evaluationStruct> evaluationList =new List<evaluationStruct>();
        yield return WaitTurn();
        Debug.Log("俺のた～ん");
        //早く置きすぎると面白くないので１秒待つ
        yield return new WaitForSeconds(1f);
        //一番置ける場所の中で、一番いい点数を取得する
        int bestEvaluation = -100;
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                //置ける場合
                if (_selector.Verification(x+1, y+1))
                {
                    int evaluation = _evaluationArray[y, x];
                    //おける中で一番評価の高い点数を保存
                    if (bestEvaluation < evaluation)
                        bestEvaluation = evaluation;
                    //置ける場所と評価を格納
                    evaluationStruct es = new evaluationStruct(x,y,evaluation);
                    //置ける場所と評価をが格納された構造体をListに格納
                    evaluationList.Add(es);
                }
            }
        }

        foreach (evaluationStruct es in evaluationList)
        {
            //評価が一番高いもののx,y座標から置く
            if (es.Evaluation == bestEvaluation)
            {
                _selector.PutPieceForAi(es.X +1,es.Y+1);
                yield break;
            }
        }
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
	    MyTeam = 'b';
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
