//盤面の情報を保持して、次置けるところを出力
//盤面のデーターはint型で保持されるフォーマットは以下
//-1    = 何も置かれていない
//0     = 黒が置かれている
//1     = 白が置かれている

using UnityEngine;
using System.Collections;

public class Tile_data : MonoBehaviour {

    public int[,] data;
    private Selector selector;

    // Use this for initialization
    void Start() {
        selector = new Selector();
        data = new int[10, 10];   //わかりやすくするために8*8を9*9でつかう
        for (int x = 0; x < 10; x++) {
            for (int y = 0; y < 10; y++) {
                data[x, y] = -1;   //全てのマスは空
            }
        }
    }

    public void OnClick() {
        //Debugger.Array2D(data);
        Debug.Log("x" + 4 + " y" + 4 + " " + data[5, 3]);
        Debug.Log("x" + 4 + " y" + 3 + " " + data[4, 4]);
        Debug.Log("x" + 5 + " y" + 4 + " " + data[3, 5]);
    }

    public int[,] GetTileData()
    {
        return data;
    }

    //現在のターンで石が置ける場所を二次元配列bool型で取得する
    //AI判断用の配列は[8][8]とする
    public bool [,] GetPlaceableArray()
    {
        bool[,] placableArray = new bool[8,8];
        selector.Verification(1, 1);
        //for (int y = 1; y < 9; y++)
        //{
        //    for (int x = 1; x < 9; x++)
        //    {
        //        //表示用の10*10の配列からAI用の8*8の配列へ
        //        if (selector.Verification( x, y)){}
        //            placableArray[y - 1,x - 1] = true;
        //        placableArray[y - 1,x - 1] = false;
        //    }
        //}
        return placableArray;
    }

}
