//クリックを読み込み　選択された場所が置けるかどうかを判断

using UnityEngine;
using System.Collections;

public class Selector : MonoBehaviour {

    public GameObject game_master;  //GameMasterObject
    public GameObject Pieces_parent;    //Pieceたちの親オブジェクト
    public GameObject Piece;    //PieceObject
    public GameObject MainCamera;   //MainCameraObject

    public int attack_team; //現在どちらの攻撃か 0:黒 1:白
    public int enemy_team;  //敵 attak_teamの反対の値
    public GameObject obj;  //現在選択されているもの
    public GameObject select_obj;  //最後にクリックされたオブジェクト
    public Color possible_color;    //選択可能なところにカーソルをあわせた時

    public int posX;
    public int posY;

    private GameObject obj_buf;
    private Color color_buf;

    private RaycastHit hit;
    private Ray ray;

    private int possible_area;  //白と黒チームの両方の置ける場所を1 それ以外は0

    void Start() {
        StartCoroutine("Team_Lisner");
        StartCoroutine("Click_Lisner");
        StartCoroutine("Enemy_Lisner");
    }

    // Update is called once per frame
    void Update() {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);    //カメラからray発射
        hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit)) {
            obj = hit.collider.gameObject;
        }
        if (obj != null) {
            if (obj.tag == "Tile") {
                if (obj_buf != obj) {
                    if (obj_buf != null)
                        obj_buf.GetComponent<Renderer>().material.color = color_buf;
                    obj_buf = obj;
                    color_buf = obj.GetComponent<Renderer>().material.color;

                    posX = int.Parse(obj.name[1].ToString());
                    posY = int.Parse(obj.name[0].ToString());

                    if (Verification(posX, posY)) {
                        obj.GetComponent<Renderer>().material.color = possible_color;
                    } else {
                        obj.GetComponent<Renderer>().material.color = new Color(255f / 255f, 162f / 255f, 45f / 255f, 255f);
                    }
                }
            } else if (obj.tag == "Outer" || obj.tag == "Tile_disable" || obj.tag == "Piece") {
                if (obj_buf != null) {
                    if (obj_buf != obj) {
                        obj_buf.GetComponent<Renderer>().material.color = color_buf;
                    }
                }
            }
        }
    }

    IEnumerator Team_Lisner() {
        while (true) {
            switch (game_master.GetComponent<Turn_Controller>().now_team) {
                case 'b':
                    attack_team = 0;
                    break;
                case 'w':
                    attack_team = 1;
                    break;
                default:
                    Debug.LogError("Team_Lisner_Error");
                    break;
            }
            yield return new WaitForSeconds(0.001f);
        }
    }

    IEnumerator Click_Lisner() {
        while (true) {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);    //カメラからray発射
            hit = new RaycastHit();
            if (Input.GetMouseButtonDown(0) && obj.tag == "Tile") {
                select_obj = obj;
                if (select_obj.GetComponent<Renderer>().material.color == possible_color) {
                    //配置可能範囲をクリックされたときにPieceを置く処理をする
                    PutPiece(int.Parse(select_obj.name[1].ToString()), int.Parse(select_obj.name[0].ToString()), attack_team);
                }
            }
            yield return new WaitForSeconds(0.001f);
        }
    }

    IEnumerator Enemy_Lisner() {
        while (true) {
            if (attack_team == 1) {
                enemy_team = 0;
            } else if (attack_team == 0) {
                enemy_team = 1;
            }
            yield return new WaitForSeconds(0.001f);
        }
    }

    //指定された座標にPieceを配置し、反転可能なPieceに反転命令を送っていく
    //xとyは座標、atTeamは置くPieceが白か黒かを指定
    void PutPiece(int x, int y, int atTeam) {
        GameObject a = GameObject.Instantiate(Piece) as GameObject;//インスタンス化
        a.GetComponent<Piece_move_controller>().x = x;
        a.GetComponent<Piece_move_controller>().y = y;
        a.GetComponent<Piece_move_controller>().team = attack_team;
        a.transform.parent = Pieces_parent.transform;

        if (attack_team == 0) {
            game_master.GetComponent<Tile_data>().data[x, y] = 0;
        } else if (attack_team == 1) {
            game_master.GetComponent<Tile_data>().data[x, y] = 1;
        }

        GameObject.Find((y.ToString()) + (x.ToString())).tag = "Tile_disable";
        a.tag = "Piece";
        a.gameObject.name = "_" + x.ToString() + y.ToString();

        //SEの再生
        MainCamera.GetComponent<AudioSource>().PlayOneShot(MainCamera.GetComponent<AudioSource>().clip);

        Reversal(x, y);  //反転作業

        game_master.GetComponent<Turn_Controller>().turn++;
        switch (game_master.GetComponent<Turn_Controller>().now_team) {
            case 'b':
                game_master.GetComponent<Turn_Controller>().now_team = 'w';
                break;
            case 'w':
                game_master.GetComponent<Turn_Controller>().now_team = 'b';
                break;
        }

    }

    //Pieceを置けるかどうかの確認
    bool Verification(int x, int y) {
        bool result = false;
        try {
            //上の確認
            if (game_master.GetComponent<Tile_data>().data[x, y + 1] == enemy_team) {
                for (int i = 2; i < 9; i++) {
                    try {
                        if (game_master.GetComponent<Tile_data>().data[x, y + i] == attack_team) {
                            //Debug.Log("上");
                            result = true;
                            goto returnFase;
                        } else if (game_master.GetComponent<Tile_data>().data[x, y + i] == -1) {
                            break;
                        }
                    } catch (System.IndexOutOfRangeException) {
                        //Debug.LogError("範囲外検出");
                    }
                }
            }
            //右上の確認
            if (game_master.GetComponent<Tile_data>().data[x + 1, y + 1] == enemy_team) {
                for (int i = 2; i < 9; i++) {
                    try {
                        if (game_master.GetComponent<Tile_data>().data[x + i, y + i] == attack_team) {
                            //Debug.Log((x + i) + " " + (y + i) + " あった" + game_master.GetComponent<Tile_data>().data[x + i, y + i]);
                            //Debug.Log("右上");
                            result = true;
                            goto returnFase;
                        } else if (game_master.GetComponent<Tile_data>().data[x + i, y + i] == -1) {
                            break;
                        }
                    } catch (System.IndexOutOfRangeException) {
                        //Debug.LogError("範囲外検出");
                    }
                }
            }
            //右の確認
            if (game_master.GetComponent<Tile_data>().data[x + 1, y] == enemy_team) {
                for (int i = 2; i < 9; i++) {
                    try {
                        if (game_master.GetComponent<Tile_data>().data[x + i, y] == attack_team) {
                            //Debug.Log("右");
                            result = true;
                            goto returnFase;
                        } else if (game_master.GetComponent<Tile_data>().data[x + i, y] == -1) {
                            break;
                        }
                    } catch (System.IndexOutOfRangeException) {
                        //Debug.LogError("範囲外検出");
                    }
                }
            }
            //右下確認
            if (game_master.GetComponent<Tile_data>().data[x + 1, y - 1] == enemy_team) {
                for (int i = 2; i < 9; i++) {
                    try {
                        if (game_master.GetComponent<Tile_data>().data[x + i, y - i] == attack_team) {
                            //Debug.Log("右下");
                            result = true;
                            goto returnFase;
                        } else if (game_master.GetComponent<Tile_data>().data[x + i, y - i] == -1) {
                            break;
                        }
                    } catch (System.IndexOutOfRangeException) {
                        //Debug.LogError("範囲外検出");
                    }
                }
            }
            //下の確認
            if (game_master.GetComponent<Tile_data>().data[x, y - 1] == enemy_team) {
                for (int i = 2; i < 9; i++) {
                    try {
                        if (game_master.GetComponent<Tile_data>().data[x, y - i] == attack_team) {
                            //Debug.Log("下");
                            result = true;
                            goto returnFase;
                        } else if (game_master.GetComponent<Tile_data>().data[x, y - i] == -1) {
                            break;
                        }
                    } catch (System.IndexOutOfRangeException) {
                        //Debug.LogError("範囲外検出");
                    }
                }
            }
            //左下確認
            if (game_master.GetComponent<Tile_data>().data[x - 1, y - 1] == enemy_team) {
                for (int i = 2; i < 9; i++) {
                    try {
                        if (game_master.GetComponent<Tile_data>().data[x - i, y - i] == attack_team) {
                            //Debug.Log("左下");
                            result = true;
                            goto returnFase;
                        } else if (game_master.GetComponent<Tile_data>().data[x - i, y - i] == -1) {
                            break;
                        }
                    } catch (System.IndexOutOfRangeException) {
                        //Debug.LogError("範囲外検出");
                    }
                }
            }
            //左確認
            if (game_master.GetComponent<Tile_data>().data[x - 1, y] == enemy_team) {
                for (int i = 2; i < 9; i++) {
                    try {
                        if (game_master.GetComponent<Tile_data>().data[x - i, y] == attack_team) {
                            //Debug.Log("左");
                            result = true;
                            goto returnFase;
                        } else if (game_master.GetComponent<Tile_data>().data[x - i, y] == -1) {
                            break;
                        }
                    } catch (System.IndexOutOfRangeException) {
                        //Debug.LogError("範囲外検出");
                    }
                }
            }
            //左上確認
            if (game_master.GetComponent<Tile_data>().data[x - 1, y + 1] == enemy_team) {
                for (int i = 2; i < 9; i++) {
                    try {
                        if (game_master.GetComponent<Tile_data>().data[x - i, y + i] == attack_team) {
                            //Debug.Log((x - i) + " " + (y + i) + " あった" + game_master.GetComponent<Tile_data>().data[x - i, y + i]);
                            //Debug.Log("左上");
                            result = true;
                            goto returnFase;
                        } else if (game_master.GetComponent<Tile_data>().data[x + i, y + i] == -1) {
                            break;
                        }
                    } catch (System.IndexOutOfRangeException) {
                        //Debug.LogError("範囲外検出");
                    }
                }
            }
        } catch (System.IndexOutOfRangeException) {
            //Debug.LogError("1マス目範囲外検出");
        }
        returnFase:
        return result;
    }

    //Pieceの色反転
    void Reversal(int x, int y) {
        int found;
        //上の反転できるところまで探す
        found = 0;
        if (game_master.GetComponent<Tile_data>().data[x, y + 1] == enemy_team) {
            for (int j = 1; j < 9; j++) {
                try {
                    if (game_master.GetComponent<Tile_data>().data[x, y + j] == attack_team) {
                        found = j + 1;
                        break;
                    }
                } catch (System.IndexOutOfRangeException) {
                    //Debug.LogError("上範囲外検出");
                }
            }
            //反転作業施行
            for (int k = 1; k < found; k++) {
                //Tile_dataにデータを格納
                game_master.GetComponent<Tile_data>().data[x, y + k] = attack_team;
                try {
                    GameObject.Find("_" + (x.ToString()) + ((y + k).ToString())).GetComponent<Piece_move_controller>().team = attack_team;
                } catch (System.NullReferenceException) {
                    //Debug.LogError("上反転物未検出" + " " + x + " " + (y + k));
                }
            }
        }

        //右上の反転できるところまで探す
        found = 0;
        if (game_master.GetComponent<Tile_data>().data[x + 1, y + 1] == enemy_team) {
            for (int j = 1; j < 9; j++) {
                try {
                    if (game_master.GetComponent<Tile_data>().data[x + j, y + j] == attack_team) {
                        found = j + 1;
                        break;
                    }
                } catch (System.IndexOutOfRangeException) {
                    //Debug.LogError("右上範囲外検出");
                }
            }
            //反転作業施行
            for (int k = 1; k < found; k++) {
                //Tile_dataにデータを格納
                game_master.GetComponent<Tile_data>().data[x + k, y + k] = attack_team;
                try {
                    GameObject.Find("_" + ((x + k).ToString()) + ((y + k).ToString())).GetComponent<Piece_move_controller>().team = attack_team;
                } catch (System.NullReferenceException) {
                    //Debug.LogError("右上反転物未検出" + " " + (x + k) + " " + (y + k));
                }
            }
        }

        //右の反転できるところまで探す
        found = 0;
        if (game_master.GetComponent<Tile_data>().data[x + 1, y] == enemy_team) {
            for (int j = 1; j < 9; j++) {
                try {
                    if (game_master.GetComponent<Tile_data>().data[x + j, y] == attack_team) {
                        found = j + 1;
                        break;
                    }
                } catch (System.IndexOutOfRangeException) {
                    ;//Debug.LogError("右範囲外検出");
                }
            }
            //反転作業施行
            for (int k = 1; k < found; k++) {
                //Tile_dataにデータを格納
                game_master.GetComponent<Tile_data>().data[x + k, y] = attack_team;
                try {
                    GameObject.Find("_" + ((x + k).ToString()) + (y.ToString())).GetComponent<Piece_move_controller>().team = attack_team;
                } catch (System.NullReferenceException) {
                    //Debug.LogError("右反転物未検出" + " " + (x + k) + " " + y);
                }
            }
        }

        //右下の反転できるところまで探す
        found = 0;
        if (game_master.GetComponent<Tile_data>().data[x + 1, y - 1] == enemy_team) {
            for (int j = 1; j < 9; j++) {
                try {
                    if (game_master.GetComponent<Tile_data>().data[x + j, y - j] == attack_team) {
                        found = j + 1;
                        break;
                    }
                } catch (System.IndexOutOfRangeException) {
                    //Debug.LogError("右下範囲外検出");
                }
            }
            //反転作業施行
            for (int k = 1; k < found; k++) {
                //Tile_dataにデータを格納
                game_master.GetComponent<Tile_data>().data[x + k, y - k] = attack_team;
                try {
                    GameObject.Find("_" + ((x + k).ToString()) + ((y - k).ToString())).GetComponent<Piece_move_controller>().team = attack_team;
                } catch (System.NullReferenceException) {
                    //Debug.LogError("右下反転物未検出" + " " + (x + k) + " " + (y - k));
                }
            }
        }

        //下の反転できるところまで探す
        found = 0;
        if (game_master.GetComponent<Tile_data>().data[x, y - 1] == enemy_team) {
            for (int j = 1; j < 9; j++) {
                try {
                    if (game_master.GetComponent<Tile_data>().data[x, y - j] == attack_team) {
                        found = j + 1;
                        break;
                    }
                } catch (System.IndexOutOfRangeException) {
                    //Debug.LogError("下範囲外検出");
                }
            }
            //反転作業施行
            for (int k = 1; k < found; k++) {
                //Tile_dataにデータを格納
                game_master.GetComponent<Tile_data>().data[x, y - k] = attack_team;
                try {
                    GameObject.Find("_" + (x.ToString()) + ((y - k).ToString())).GetComponent<Piece_move_controller>().team = attack_team;
                } catch (System.NullReferenceException) {
                    //Debug.LogError("下反転物未検出" + " " + x + " " + (y - k));
                }
            }
        }

        //左下の反転できるところまで探す
        found = 0;
        if (game_master.GetComponent<Tile_data>().data[x - 1, y - 1] == enemy_team) {
            for (int j = 1; j < 9; j++) {
                try {
                    if (game_master.GetComponent<Tile_data>().data[x - j, y - j] == attack_team) {
                        found = j + 1;
                        break;
                    }
                } catch (System.IndexOutOfRangeException) {
                    //Debug.LogError("左下範囲外検出");
                }
            }
            //反転作業施行
            for (int k = 1; k < found; k++) {
                //Tile_dataにデータを格納
                game_master.GetComponent<Tile_data>().data[x - k, y - k] = attack_team;
                try {
                    GameObject.Find("_" + ((x - k).ToString()) + ((y - k).ToString())).GetComponent<Piece_move_controller>().team = attack_team;
                } catch (System.NullReferenceException) {
                    //Debug.LogError("左下反転物未検出" + " " + (x - k) + " " + (y - k));
                }
            }
        }

        //左の反転できるところまで探す
        found = 0;
        if (game_master.GetComponent<Tile_data>().data[x - 1, y] == enemy_team) {
            for (int j = 1; j < 9; j++) {
                try {
                    if (game_master.GetComponent<Tile_data>().data[x - j, y] == attack_team) {
                        found = j + 1;
                        break;
                    }
                } catch (System.IndexOutOfRangeException) {
                    //Debug.LogError("左範囲外検出");
                }
            }
            //反転作業施行
            for (int k = 1; k < found; k++) {
                //Tile_dataにデータを格納
                game_master.GetComponent<Tile_data>().data[x - k, y] = attack_team;
                try {
                    GameObject.Find("_" + ((x - k).ToString()) + (y.ToString())).GetComponent<Piece_move_controller>().team = attack_team;
                } catch (System.NullReferenceException) {
                    //Debug.LogError("左反転物未検出" + " " + (x - k) + " " + y);
                }
            }
        }

        //左上の反転できるところまで探す
        found = 0;
        if (game_master.GetComponent<Tile_data>().data[x - 1, y + 1] == enemy_team) {
            for (int j = 1; j < 9; j++) {
                try {
                    if (game_master.GetComponent<Tile_data>().data[x - j, y + j] == attack_team) {
                        found = j + 1;
                        break;
                    }
                } catch (System.IndexOutOfRangeException) {
                    //Debug.LogError("左上範囲外検出");
                }
            }
            //反転作業施行
            for (int k = 1; k < found; k++) {
                //Tile_dataにデータを格納
                game_master.GetComponent<Tile_data>().data[x - k, y + k] = attack_team;
                try {
                    GameObject.Find("_" + ((x - k).ToString()) + ((y + k).ToString())).GetComponent<Piece_move_controller>().team = attack_team;
                } catch (System.NullReferenceException) {
                    //Debug.LogError("左上反転物未検出" + " " + (x - k) + " " + (y + k));
                }
            }
        }
    }
}