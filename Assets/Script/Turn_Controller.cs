//現在何ターン目かカウントするのと、現在どちらのチームの番か

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Turn_Controller : MonoBehaviour {

    public int turn;    //現在何ターン目か
    public char now_team;

    public Text show_turn;
    public Text show_team;

    void Start() {
        StartCoroutine("Lisner");
        now_team = 'w';
    }
	
    IEnumerator Lisner() {
        while (true) {
            show_turn.text = "Turn:" + turn.ToString();
            show_team.text = "Team:" + now_team;
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
}
