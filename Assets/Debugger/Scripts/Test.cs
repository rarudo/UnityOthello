using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Test : MonoBehaviour {
    void Start()
    {
        // print array of strings to the console
        string[] words = new string[5] { "one", "two", "three", "four", "five" };
        Debugger.Array<string>(words);

        // print array of intgers to the console
        int[] numbers = new int[5] { 1, 2, 3, 4, 5 };
        Debugger.Array<int>(numbers);

        // print array of floats to the console
        float[] floats = new float[5] { 1.42f, 31.43f, 56.0f, 78.34f, 100.99f };
        Debugger.Array<float>(floats);

        // print two-dimensional array of strings to the console
        string[,] twoArray = new string[,]
	    {
	        {"dog", "cat"},
	        {"day", "night"},
	        {"white", "black"},
	        {"happy", "sad"},
	        {"peace", "war"}
	    };

        Debugger.Array2D<string>(twoArray);

        // print three-dimensional array of strings to the console
        int[, ,] threeArray = new int[3, 5, 4];
        threeArray[0, 0, 0] = 1;
        threeArray[0, 1, 0] = 2;
        threeArray[0, 2, 0] = 3;
        threeArray[0, 3, 0] = 4;
        threeArray[0, 4, 0] = 5;
        threeArray[1, 1, 1] = 2;
        threeArray[2, 2, 2] = 3;
        threeArray[2, 2, 3] = 4;

        Debugger.Array3D<int>(threeArray);

        // print list of strings to the console
        List<string> wordsList = new List<string>();
        wordsList.Add("First word");
        wordsList.Add("Second word");
        wordsList.Add("Third word");
        wordsList.Add("Fourth word");
        wordsList.Add("Fifth word");

        Debugger.List<string>(wordsList);

        // print list of Vector3 to the console
        List<Vector3> vectorsList = new List<Vector3>();
        vectorsList.Add(Vector3.back);
        vectorsList.Add(Vector3.down);
        vectorsList.Add(Vector3.forward);
        vectorsList.Add(Vector3.left);
        vectorsList.Add(Vector3.one);
        vectorsList.Add(Vector3.right);
        vectorsList.Add(Vector3.up);
        vectorsList.Add(Vector3.zero);

        Debugger.List<Vector3>(vectorsList);

        // print dictionary to the console
        Dictionary<int, string> dict = new Dictionary<int, string>();
        dict.Add(0, "zero");
        dict.Add(1, "one");
        dict.Add(2, "two");
        dict.Add(3, "three");
        dict.Add(4, "four");

        Debugger.Dictionary<int, string>(dict);

        // print some value to the console
        string someWord = "ABCDEFGHIJKL";
        int someInt = 3314;
        Debugger.QuickLog<string>(someWord);
        Debugger.QuickLog<int>(someInt);

        Debugger.Log(" ", "This", "is", "an", "example");

        // print exception details
		/*
        int[] empty = new int[1];
        try
        {
            Debugger.QuickLog<int>(empty[2]);
        }
        catch (Exception e)
        {
            Debugger.ExceptionDetails(e);
        }*/
    }
}
