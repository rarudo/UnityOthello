//盤面の情報を保持して、次置けるところを出力
//盤面のデーターはint型で保持されるフォーマットは以下
//-1    = 何も置かれていない
//0     = 黒が置かれている
//1     = 白が置かれている

using UnityEngine;
using System.Collections;

public class Tile_data : MonoBehaviour {

    public int[,] data;

    // Use this for initialization
    void Start() {
        data = new int[10, 10];   //わかりやすくするために8*8を9*9でつかう
        for (int x = 0; x < 10; x++) {
            for (int y = 0; y < 10; y++) {
                data[x, y] = -1;   //全てのマスは空
            }
        }
    }

    public void OnClick() {
        //Debugger.Array2D(data);
        Debug.Log("x" + 4 + " y" + 4 + " " + data[4, 4]);
        Debug.Log("x" + 4 + " y" + 3 + " " + data[4, 3]);
        Debug.Log("x" + 5 + " y" + 4 + " " + data[5, 4]);
    }
}
