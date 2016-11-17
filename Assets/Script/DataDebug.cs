using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DataDebug : MonoBehaviour {

    public GameObject game_master;
    public Text dataDebugText;

    private string text;
	
	// Update is called once per frame
	void Update () {
        text = "";
	    for(int y = 8; y > 0; y--) {
            for (int x = 1; x < 9; x ++) {
                switch(game_master.GetComponent<Tile_data>().data[x, y]) {
                    case 0:
                        text += "0 ";
                        break;
                    case 1:
                        text += "1 ";
                        break;
                    case -1:
                        text += "□ ";
                        break;
                }
                if(x == 8) {
                    text += "\n";
                }
            }
        }

        dataDebugText.text = text;

    }
}
