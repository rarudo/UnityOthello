//Pieceの位置を指定マスに移動させる

using UnityEngine;
using System.Collections;

public class Piece_move_controller : MonoBehaviour {

    //この変数に入れた座標に移動させる
    public int x;
    public int y;

    public Animation WtoB;
    public Animation BtoW;

    public int team;   //現在どちらのチームの所有か 0:黒 1:白

	// Use this for initialization
	void Start () {
        StartCoroutine("Lisner");
	}

    IEnumerator Lisner() {
        while (true) {
            this.gameObject.GetComponent<Transform>().position = new Vector3(x * 10 + 5, 0, y * 10 - 5);
            yield return new WaitForSeconds(0.001f);

            switch (team) {
                case 0:
                    this.transform.rotation = Quaternion.Euler(0, 0, 0);    //ここをアニメーションに代替する
                    break;
                case 1:
                    this.transform.rotation = Quaternion.Euler(180, 0, 0);
                    break;
                default:
                    Debug.LogError("どちらのチームにも所属していません : team = " + team);
                    break;
            }
        }
    }
}
