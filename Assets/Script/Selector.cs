//クリックを読み込み　選択された場所が置けるかどうかを判断

using UnityEngine;
using System.Collections;
using UnityEngine.WSA;

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

    public bool[] verification_result; //置けるかどうか判定用 コルーチンで使用する
    private bool[] verification_end; //置けるかどうか判定用 コルーチンの判定が終了したか判定用

    private int possible_area;  //白と黒チームの両方の置ける場所を1 それ以外は0

    //フィールドで保持しておく
    private Tile_data _tileData;
    private Turn_Controller _turnController;

    void Start()
    {
        //最初にインスタンス化する
        _tileData = GetComponent<Tile_data>();
        _turnController = GetComponent<Turn_Controller>();
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

    public bool IsEnemyTurn()
    {
        if (enemy_team == 1)
        {
            return true;
        }
        return false;
    }

    IEnumerator Team_Lisner() {
        while (true) {
            switch (_turnController.now_team) {
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
                if (select_obj.GetComponent<Renderer>().material.color == possible_color)
                {
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
    //事前にVerificationメソッドでおける反対が出てること
    public void PutPieceForAi(int x, int y)
    {
        //verificationを呼んでフラグを立てる
        Verification(x, y);
        PutPiece(x,y,attack_team);
    }

    public void PutPiece(int x, int y, int atTeam) {
        GameObject a = GameObject.Instantiate(Piece) as GameObject;//インスタンス化
        a.GetComponent<Piece_move_controller>().x = x;
        a.GetComponent<Piece_move_controller>().y = y;
        a.GetComponent<Piece_move_controller>().team = attack_team;
        a.transform.parent = Pieces_parent.transform;

        if (attack_team == 0) {
            _tileData.data[x, y] = 0;
            Debug.Log(x + "," + y + " " + 0);
        } else if (attack_team == 1) {
            _tileData.data[x, y] = 1;
            Debug.Log(x + "," + y + " " + 1);
        }

        GameObject.Find((y.ToString()) + (x.ToString())).tag = "Tile_disable";
        a.tag = "Piece";
        a.gameObject.name = "_" + x.ToString() + y.ToString();

        //SEの再生
        MainCamera.GetComponent<AudioSource>().PlayOneShot(MainCamera.GetComponent<AudioSource>().clip);

        Reversal(x, y);  //反転作業

        _turnController.turn++;
        switch (_turnController.now_team) {
            case 'b':
                _turnController.now_team = 'w';
                break;
            case 'w':
                _turnController.now_team = 'b';
                break;
        }

    }

    //指定されたx,y座標にPieceが置けるかどうかを調べる
    public bool Verification(int x, int y) {
        bool result = false;
        if (_tileData.data[x, y] != -1) {
            goto VerificationEND;   //もうすでに盤面に石が置かれているので判定強制終了
        }
        verification_result = new bool[8] { false, false, false, false, false, false, false, false };    //初期化　のちどれかがtrueになったら配置可能判定がでる
        verification_end = new bool[8] { false, false, false, false, false, false, false, false };  //初期化　これが全てtrueにならないと次の判断フェーズに行かない

        //判定用コルーチンの一斉開始
        StartCoroutine(Up_find(x, y));                  //0
        StartCoroutine(UpRight_find(x, y));         //1
        StartCoroutine(Right_find(x, y));               //2
        StartCoroutine(DownRight_find(x, y));       //3
        StartCoroutine(Down_find(x, y));             //4
        StartCoroutine(DownLeft_find(x, y));        //5
        StartCoroutine(Left_find(x, y));                //6
        StartCoroutine(UpLeft_find(x, y));              //7

        //判定が出るのを待つ
        while (verification_end[0] == true &&
                verification_end[1] == true &&
                verification_end[2] == true &&
                verification_end[3] == true &&
                verification_end[4] == true &&
                verification_end[5] == true &&
                verification_end[6] == true &&
                verification_end[7] == true) {
            //全ての判定が終了しないと次の判定に行かない
        }

        //判定が出てきた後どれか１方向でもtrueになっていれば配置可能判定を出す
        if (verification_result[0] == true ||
            verification_result[1] == true ||
            verification_result[2] == true ||
            verification_result[3] == true ||
            verification_result[4] == true ||
            verification_result[5] == true ||
            verification_result[6] == true ||
            verification_result[7] == true) {
            result = true;
        }

        VerificationEND:    //置こうとした場所がもう石がすでに置かれていた時に飛んでくる
        return result;
    }

    IEnumerator Up_find(int x, int y) {
        if (_tileData.data[x, y + 1] == enemy_team) {    //1マス目が敵石であった時
            for (int i = 2; i < 9; i++) {
                try {
                    if (_tileData.data[x, y + i] == attack_team) {   //2マス目以降に自分の駒があった時はtrueにする
                        verification_result[0] = true;  //置ける判定をして終了
                        goto UP_FIND_END;
                    } else if (_tileData.data[x, y + i] == enemy_team) { //2マス目以降に敵石があったときは無視

                    } else if ((_tileData.data[x, y + i] == -1)) {  //2マス目以降が何も置かれていない状況であればこの方向には置けない
                        goto UP_FIND_END;
                    }
                } catch (System.IndexOutOfRangeException) {

                }
            }
        }
        UP_FIND_END:
        yield return null;
    }

    IEnumerator UpRight_find(int x, int y) {
        if (_tileData.data[x + 1, y + 1] == enemy_team) {    //1マス目が敵石であった時
            for (int i = 2; i < 9; i++) {
                try {
                    if (_tileData.data[x + i, y + i] == attack_team) {   //2マス目以降に自分の駒があった時はtrueにする
                        verification_result[1] = true;  //置ける判定をして終了
                        goto UPRIGHT_FIND_END;
                    } else if (_tileData.data[x + i, y + i] == enemy_team) { //2マス目以降に敵石があったときは無視

                    } else if ((_tileData.data[x + i, y + i] == -1)) {  //2マス目以降が何も置かれていない状況であればこの方向には置けない
                        goto UPRIGHT_FIND_END;
                    }
                } catch (System.IndexOutOfRangeException) {

                }
            }
        }
        UPRIGHT_FIND_END:
        yield return null;
    }

    IEnumerator Right_find(int x, int y) {
        if (_tileData.data[x + 1, y] == enemy_team) {    //1マス目が敵石であった時
            for (int i = 2; i < 9; i++) {
                try {
                    if (_tileData.data[x + i, y] == attack_team) {   //2マス目以降に自分の駒があった時はtrueにする
                        verification_result[2] = true;  //置ける判定をして終了
                        goto RIGHT_FIND_END;
                    } else if (_tileData.data[x + i, y] == enemy_team) { //2マス目以降に敵石があったときは無視

                    } else if ((_tileData.data[x + i, y] == -1)) {  //2マス目以降が何も置かれていない状況であればこの方向には置けない
                        goto RIGHT_FIND_END;
                    }
                } catch (System.IndexOutOfRangeException) {

                }
            }
        }
        RIGHT_FIND_END:
        yield return null;
    }

    IEnumerator DownRight_find(int x, int y) {
        if (_tileData.data[x + 1, y - 1] == enemy_team) {    //1マス目が敵石であった時
            for (int i = 2; i < 9; i++) {
                try {
                    if (_tileData.data[x + i, y - i] == attack_team) {   //2マス目以降に自分の駒があった時はtrueにする
                        verification_result[3] = true;  //置ける判定をして終了
                        goto DOWNRIGHT_FIND_END;
                    } else if (_tileData.data[x + i, y - i] == enemy_team) { //2マス目以降に敵石があったときは無視

                    } else if ((_tileData.data[x + i, y - i] == -1)) {  //2マス目以降が何も置かれていない状況であればこの方向には置けない
                        goto DOWNRIGHT_FIND_END;
                    }
                } catch (System.IndexOutOfRangeException) {

                }
            }
        }
        DOWNRIGHT_FIND_END:
        yield return null;
    }

    IEnumerator Down_find(int x, int y) {
        if (_tileData.data[x, y - 1] == enemy_team) {    //1マス目が敵石であった時
            for (int i = 2; i < 9; i++) {
                try {
                    if (_tileData.data[x, y - i] == attack_team) {   //2マス目以降に自分の駒があった時はtrueにする
                        verification_result[4] = true;  //置ける判定をして終了
                        goto DOWN_FIND_END;
                    } else if (_tileData.data[x, y - i] == enemy_team) { //2マス目以降に敵石があったときは無視

                    } else if ((_tileData.data[x, y - i] == -1)) {  //2マス目以降が何も置かれていない状況であればこの方向には置けない
                        goto DOWN_FIND_END;
                    }
                } catch (System.IndexOutOfRangeException) {

                }
            }
        }
        DOWN_FIND_END:
        yield return null;
    }

    IEnumerator DownLeft_find(int x, int y) {
        if (_tileData.data[x - 1, y - 1] == enemy_team) {    //1マス目が敵石であった時
            for (int i = 2; i < 9; i++) {
                try {
                    if (_tileData.data[x - i, y - i] == attack_team) {   //2マス目以降に自分の駒があった時はtrueにする
                        verification_result[5] = true;  //置ける判定をして終了
                        goto DOWNREFT_FIND_END;
                    } else if (_tileData.data[x - i, y - i] == enemy_team) { //2マス目以降に敵石があったときは無視

                    } else if ((_tileData.data[x - i, y - i] == -1)) {  //2マス目以降が何も置かれていない状況であればこの方向には置けない
                        goto DOWNREFT_FIND_END;
                    }
                } catch (System.IndexOutOfRangeException) {

                }
            }
        }
        DOWNREFT_FIND_END:
        yield return null;
    }

    IEnumerator Left_find(int x, int y) {
        if (_tileData.data[x - 1, y] == enemy_team) {    //1マス目が敵石であった時
            for (int i = 2; i < 9; i++) {
                try {
                    if (_tileData.data[x - i, y] == attack_team) {   //2マス目以降に自分の駒があった時はtrueにする
                        verification_result[6] = true;  //置ける判定をして終了
                        goto LEFT_FIND_END;
                    } else if (_tileData.data[x - i, y] == enemy_team) { //2マス目以降に敵石があったときは無視

                    } else if ((_tileData.data[x - i, y] == -1)) {  //2マス目以降が何も置かれていない状況であればこの方向には置けない
                        goto LEFT_FIND_END;
                    }
                } catch (System.IndexOutOfRangeException) {

                }
            }
        }
        LEFT_FIND_END:
        yield return null;
    }

    IEnumerator UpLeft_find(int x, int y) {
        if (_tileData.data[x - 1, y + 1] == enemy_team) {    //1マス目が敵石であった時
            for (int i = 2; i < 9; i++) {
                try {
                    if (_tileData.data[x - i, y + i] == attack_team) {   //2マス目以降に自分の駒があった時はtrueにする
                        verification_result[7] = true;  //置ける判定をして終了
                        goto UPLEFT_FIND_END;
                    } else if (_tileData.data[x - i, y + i] == enemy_team) { //2マス目以降に敵石があったときは無視

                    } else if ((_tileData.data[x - i, y + i] == -1)) {  //2マス目以降が何も置かれていない状況であればこの方向には置けない
                        goto UPLEFT_FIND_END;
                    }
                } catch (System.IndexOutOfRangeException) {

                }
            }
        }
        UPLEFT_FIND_END:
        yield return null;
    }

    void Reversal(int x, int y) {
        if (verification_result[0] == true)
            StartCoroutine(Up_reversal(x, y));
        if (verification_result[1] == true)
            StartCoroutine(UpRight_reversal(x, y));
        if (verification_result[2] == true)
            StartCoroutine(Right_reversal(x, y));
        if (verification_result[3] == true)
            StartCoroutine(DownRight_reversal(x, y));
        if (verification_result[4] == true)
            StartCoroutine(Down_reversal(x, y));
        if (verification_result[5] == true)
            StartCoroutine(DownLeft_reversal(x, y));
        if (verification_result[6] == true)
            StartCoroutine(Left_reversal(x, y));
        if (verification_result[7] == true)
            StartCoroutine(UpLeft_reversal(x, y));
    }

    //反転用コルーチン
    IEnumerator Up_reversal(int x, int y) {
        for(int i = 1; i<9; i++) {
            try {
                if (_tileData.data[x, y + i] == enemy_team) {
                    GameObject.Find("_" + (x.ToString()) + ((y + i).ToString())).GetComponent<Piece_move_controller>().team = attack_team;  //自分のチームに石を反転
                    _tileData.data[x, y + i] = attack_team; //dataも更新
                } else if (_tileData.data[x, y + i] != enemy_team) {
                    goto UP_REVERSAL_END;
                }
            } catch (System.IndexOutOfRangeException) {

            }
        }
        UP_REVERSAL_END:
        yield return null;
    }

    IEnumerator UpRight_reversal(int x, int y) {
        for (int i = 1; i < 9; i++) {
            try {
                if (_tileData.data[x + i, y + i] == enemy_team) {
                    GameObject.Find("_" + ((x + i).ToString()) + ((y + i).ToString())).GetComponent<Piece_move_controller>().team = attack_team;  //自分のチームに石を反転
                    _tileData.data[x + i, y + i] = attack_team; //dataも更新
                } else if (_tileData.data[x + i, y + i] != enemy_team) {
                    goto UPRIGHT_REVERSAL_END;
                }
            } catch (System.IndexOutOfRangeException) {

            }
        }
        UPRIGHT_REVERSAL_END:
        yield return null;
    }

    IEnumerator Right_reversal(int x, int y) {
        for (int i = 1; i < 9; i++) {
            try {
                if (_tileData.data[x + i, y] == enemy_team) {
                    GameObject.Find("_" + ((x + i).ToString()) + (y.ToString())).GetComponent<Piece_move_controller>().team = attack_team;  //自分のチームに石を反転
                    _tileData.data[x + i, y] = attack_team; //dataも更新
                } else if (_tileData.data[x + i, y] != enemy_team) {
                    goto RIGHT_REVERSAL_END;
                }
            } catch (System.IndexOutOfRangeException) {

            }
        }
        RIGHT_REVERSAL_END:
        yield return null;
    }

    IEnumerator DownRight_reversal(int x, int y) {
        for (int i = 1; i < 9; i++) {
            try {
                if (_tileData.data[x + i, y - i] == enemy_team) {
                    GameObject.Find("_" + ((x + i).ToString()) + ((y - i).ToString())).GetComponent<Piece_move_controller>().team = attack_team;  //自分のチームに石を反転
                    _tileData.data[x + i, y - i] = attack_team; //dataも更新
                } else if (_tileData.data[x + i, y - i] != enemy_team) {
                    goto DOWNRIGHT_REVERSAL_END;
                }
            } catch (System.IndexOutOfRangeException) {

            }
        }
        DOWNRIGHT_REVERSAL_END:
        yield return null;
    }

    IEnumerator Down_reversal(int x, int y) {
        for (int i = 1; i < 9; i++) {
            try {
                if (_tileData.data[x, y - i] == enemy_team) {
                    GameObject.Find("_" + (x.ToString()) + ((y - i).ToString())).GetComponent<Piece_move_controller>().team = attack_team;  //自分のチームに石を反転
                    _tileData.data[x, y - i] = attack_team; //dataも更新
                } else if (_tileData.data[x, y - i] != enemy_team) {
                    goto DOWN_REVERSAL_END;
                }
            } catch (System.IndexOutOfRangeException) {

            }
        }
        DOWN_REVERSAL_END:
        yield return null;
    }

    IEnumerator DownLeft_reversal(int x, int y) {
        for (int i = 1; i < 9; i++) {
            try {
                if (_tileData.data[x - i, y - i] == enemy_team) {
                    GameObject.Find("_" + ((x - i).ToString()) + ((y - i).ToString())).GetComponent<Piece_move_controller>().team = attack_team;  //自分のチームに石を反転
                    _tileData.data[x - i, y - i] = attack_team; //dataも更新
                } else if (_tileData.data[x - i, y - i] != enemy_team) {
                    goto DOWNLEFT_REVERSAL_END;
                }
            } catch (System.IndexOutOfRangeException) {

            }
        }
        DOWNLEFT_REVERSAL_END:
        yield return null;
    }

    IEnumerator Left_reversal(int x, int y) {
        for (int i = 1; i < 9; i++) {
            try {
                if (_tileData.data[x - i, y] == enemy_team) {
                    GameObject.Find("_" + ((x - i).ToString()) + (y.ToString())).GetComponent<Piece_move_controller>().team = attack_team;  //自分のチームに石を反転
                    _tileData.data[x - i, y] = attack_team; //dataも更新
                }else if(_tileData.data[x - i, y] != enemy_team) {
                    goto LEFT_REVERSAL_END;
                }
            } catch (System.IndexOutOfRangeException) {

            }
        }
        LEFT_REVERSAL_END:
        yield return null;
    }

    IEnumerator UpLeft_reversal(int x, int y) {
        for (int i = 1; i < 9; i++) {
            try {
                if (_tileData.data[x - i, y + i] == enemy_team) {
                    GameObject.Find("_" + ((x - i).ToString()) + ((y + i).ToString())).GetComponent<Piece_move_controller>().team = attack_team;  //自分のチームに石を反転
                    _tileData.data[x - i, y + i] = attack_team; //dataも更新
                } else if (_tileData.data[x - i, y + i] != enemy_team) {
                    goto UPLEFT_REVERSAL_END;
                }
            } catch (System.IndexOutOfRangeException) {

            }
        }
        UPLEFT_REVERSAL_END:
        yield return null;
    }
}