﻿//置ける場所を全取得
//取得後それぞれのマスの点数比較
//

using UnityEngine;
using System.Collections;

public class Ai : MonoBehaviour {

    private int[,] weighting;

    // Use this for initialization
    void Start() {
        weighting = new int[10, 10] { {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                                    {0, 30, -12, 0, -1, -1, 0, -12, 30, 0},
                                                    {0, -12, -15, -3, -3, -3, -3, -15, 12, 0},
                                                    { 0, 0, -3, 0, -1, -1, 0, -3, 0, 0},
                                                    {0, -1, -3, -1, -1, -1, -1, -3,  -1, 0},
                                                    {0, -1, -3, -1, -1, -1, -1, -3,  -1, 0},
                                                    {0, 0, -3, 0, -1, -1, 0, -3, 0, 0},
                                                    {0, -12, -15, -3, -3, -3, -3, -15, 12, 0},
                                                    {0, 30, -12, 0, -1, -1, 0, -12, 30, 0},
                                                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}};
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //評価を開始する
    void Assessment() {

    }
}