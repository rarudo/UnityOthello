//ゲームの初期化

using UnityEngine;
using System.Collections;

public class Game_starter : MonoBehaviour {

    public GameObject Game_Master;

    public GameObject first_tile44;
    public GameObject first_tile55;
    public GameObject first_tile45;
    public GameObject first_tile54;

    public GameObject Piece;
    public GameObject Pieces_parent;    //親オブジェクト

    //リセットボタンを押された時
    public void OnClick() {

        GameObject.Find("GameStart").SetActive(false);  //ボタンの無効化

        Game_Master.GetComponent<Turn_Controller>().turn = 1;   //ターンの初期化
        Game_Master.GetComponent<Turn_Controller>().now_team = 'w';

        GameObject a = GameObject.Instantiate(Piece) as GameObject;//インスタンス化
        a.GetComponent<Piece_move_controller>().x = 4;
        a.GetComponent<Piece_move_controller>().y = 4;
        a.GetComponent<Piece_move_controller>().team = 0;
        a.transform.parent = Pieces_parent.transform;
        Game_Master.GetComponent<Tile_data>().data[4, 4] = 0;
        first_tile44.tag = "Tile_disable";
        a.tag = "Piece";
        a.gameObject.name = "_44";

        a = GameObject.Instantiate(Piece) as GameObject;//インスタンス化
        a.GetComponent<Piece_move_controller>().x = 5;
        a.GetComponent<Piece_move_controller>().y = 5;
        a.GetComponent<Piece_move_controller>().team = 0;
        a.transform.parent = Pieces_parent.transform;
        Game_Master.GetComponent<Tile_data>().data[5, 5] = 0;
        first_tile55.tag = "Tile_disable";
        a.tag = "Piece";
        a.gameObject.name = "_55";

        a = GameObject.Instantiate(Piece) as GameObject;//インスタンス化
        a.GetComponent<Piece_move_controller>().x = 4;
        a.GetComponent<Piece_move_controller>().y = 5;
        a.GetComponent<Piece_move_controller>().team = 1;
        a.transform.parent = Pieces_parent.transform;
        Game_Master.GetComponent<Tile_data>().data[4, 5] = 1;
        first_tile45.tag = "Tile_disable";
        a.tag = "Piece";
        a.gameObject.name = "_45";

        a = GameObject.Instantiate(Piece) as GameObject;//インスタンス化
        a.GetComponent<Piece_move_controller>().x = 5;
        a.GetComponent<Piece_move_controller>().y = 4;
        a.GetComponent<Piece_move_controller>().team = 1;
        a.transform.parent = Pieces_parent.transform;
        Game_Master.GetComponent<Tile_data>().data[5, 4] = 1;
        first_tile54.tag = "Tile_disable";
        a.tag = "Piece";
        a.gameObject.name = "_54";

        //Debugger.Array2D(Game_Master.GetComponent<Tile_data>().data);

        // //テスト用
        // a = GameObject.Instantiate(Piece) as GameObject;//インスタンス化
        // a.GetComponent<Piece_move_controller>().x = 3;
        // a.GetComponent<Piece_move_controller>().y = 6;
        // a.GetComponent<Piece_move_controller>().team = 0;
        // a.transform.parent = Pieces_parent.transform;
        // Game_Master.GetComponent<Tile_data>().data[3, 6] = 0;
        //GameObject.Find("63").tag = "Tile_disable";
        // a.tag = "Piece";
        // a.gameObject.name = "test";
    }

    //void Update() {
    //    Debug.Log("44" + Game_Master.GetComponent<Tile_data>().data[4, 4]);
    //    Debug.Log("45" + Game_Master.GetComponent<Tile_data>().data[4, 5]);
    //    Debug.Log("54" + Game_Master.GetComponent<Tile_data>().data[5, 4]);
    //    Debug.Log("55" + Game_Master.GetComponent<Tile_data>().data[5, 5]);
    //}
}
