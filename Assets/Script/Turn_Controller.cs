//現在何ターン目かカウントするのと、現在どちらのチームの番か

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Turn_Controller : MonoBehaviour {

    public GameObject game_master;    //GameMasterObject

    public int turn;    //現在何ターン目か
    public char now_team;

    public Text show_turn;
    public Text show_team;

    public int null_counter;   //これが0になったらゲームが終了され、集計に入る
    public int show_null_counter;   //for Debug

    public GameObject win_b;
    public GameObject win_w;
    public GameObject result;
    public Text result_text;
    public GameObject restart_button;

    void Start() {
        StartCoroutine("Lisner");
        StartCoroutine("End_Lisner");
        now_team = 'w';
    }

    IEnumerator End_Lisner() {
        int turn_buf;   //比較用
        turn_buf = turn;

        null_counter = 0;

        while (true) {
            //turnが進んだときに読み込み
            if (turn != turn_buf) {
                //現在のチームが石を置けるかどうかを調べる
                //もしも置けない場合は次のチームに進める
                bool put_judge = false;
                int possible_place = 0;
                switch (now_team) {
                    case 'b':
                        for (int i = 1; i < 10; i++) {
                            for (int j = 1; j < 10; j++) {
                                if (game_master.GetComponent<Tile_data>().data[i, j] == -1) {
                                    if (Verification(i, j, 0, 1) == true) {
                                        put_judge = true;
                                        possible_place++;
                                    }
                                }
                            }
                        }
                        break;
                    case 'w':
                        for (int i = 1; i < 10; i++) {
                            for (int j = 1; j < 10; j++) {
                                if (game_master.GetComponent<Tile_data>().data[i, j] == -1) {
                                    if (Verification(i, j, 1, 0) == true) {
                                        put_judge = true;
                                        possible_place++;
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        Debug.LogError("Error");
                        break;
                }
                //Debug.Log("possible_p : " + possible_place + " team:" + now_team);
                possible_place = 0;
                //Debug.Log(put_judge);

                //置ける数が少なくなってきた時（何も無いマス２４マス以下）全てのマスが置けるかどうかを調べる
                bool master_judge = false;  //完全に置けなくなった時falseのまま検出し、ゲーム終了
                if (null_counter <= 24) {
                    for (int i = 1; i < 10; i++) {
                        for (int j = 1; j < 10; j++) {
                            //一回でもおけたらゲーム続行
                            if (Verification(i, j, 1, 0) == true) {
                                master_judge = true;
                            }
                            if (Verification(i, j, 0, 1) == true) {
                                master_judge = true;
                            }
                        }
                    }
                }

                for (int i = 1; i < 9; i++) {
                    for (int j = 1; j < 9; j++) {
                        if (game_master.GetComponent<Tile_data>().data[i, j] == -1) {
                            null_counter++;
                        }
                    }
                }
                turn_buf = turn;    //ターンの更新

                if (null_counter == 0 || master_judge == false) {
                    Debug.Log("ゲームが終了しました");
                    StartCoroutine("Aggregate");
                    yield break;
                }

                show_null_counter = null_counter;   //for Debug 最後のnull_counterの数を確認できるようにする
                null_counter = 0;

                if (put_judge == false) {
                    //置けなかった場合ターンを進める
                    turn++;
                    switch (now_team) {
                        case 'b':
                            now_team = 'w';
                            break;
                        case 'w':
                            now_team = 'b';
                            break;
                    }
                }
            }
            yield return new WaitForSeconds(0.001f);
        }
    }

    //集計用
    IEnumerator Aggregate() {
        //Debug.Log("集計したお");
        //集計用
        int b_result = 0;
        int w_result = 0;

        for (int i = 1; i < 9; i++) {
            for (int j = 1; j < 9; j++) {
                if(game_master.GetComponent<Tile_data>().data[i, j] == 0) {
                    b_result++;
                }
                if (game_master.GetComponent<Tile_data>().data[i, j] == 1) {
                    w_result++;
                }
            }
        }

        string text = "[result] Black: " + b_result + " White: " + w_result;
        Debug.Log(text);
        result_text.text = text;
        result.SetActive(true);

        if (b_result > w_result) {
            win_b.SetActive(true);
        } else if(b_result < w_result) {
            win_w.SetActive(true);
        } else {
            Debug.Log("引き分け");
        }

        restart_button.SetActive(true); //リスタートボタンの有効化

        yield return 0;
    }

    IEnumerator Lisner() {
        while (true) {
            show_turn.text = "Turn:" + turn.ToString();
            switch (now_team) {
                case 'b':
                    show_team.text = "Turn: Black";
                    break;
                case 'w':
                    show_team.text = "Turn: While";
                    break;
            }
            yield return new WaitForSeconds(0.001f);
        }
    }

    public void OnClick() {
        turn++;
        switch (now_team) {
            case 'b':
                now_team = 'w';
                break;
            case 'w':
                now_team = 'b';
                break;
        }
    }

    //Pieceを置けるかどうかの確認
    bool Verification(int x, int y, int attack_team, int enemy_team) {
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
}
