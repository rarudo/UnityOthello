using UnityEngine;
using System.Collections;

public class RestartButton : MonoBehaviour {

    public void OnClick() {
        Application.LoadLevel("Main"); // シーンの名前かインデックスを指定
    }
}
