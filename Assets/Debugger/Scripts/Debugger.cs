using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class Debugger {
    private static string defaultSeparator = "\n"; // default separator

    /// <summary>
    /// Imitation of Debug.Log(). 
    /// </summary>
    /// <typeparam name="T">Generic type parameter. When you call it, it can be any type: string, int, Vector3 etc.</typeparam>
    /// <param name="msg">Data to be printed. It must be the same type as T.</param>
    public static void QuickLog<T>(T msg)
    {
        Debug.Log(msg.ToString());
    }

    /// <summary>
    /// Prints all values in an array of text. Good for debugging two or more variables in the same call.
    /// </summary>
    /// <param name="separator">Separator</param>
    /// <param name="strings">Data to be printed.</param>
    public static void Log(string separator, params string[] strings)
    {
        string value = null;
        for (int i = 0; i < strings.Length; i++)
        {
            if (separator != null)
                value += String.Format(strings[i] + "{0}", i < strings.Length - 1 ? separator : "");
            else
                value += String.Format(strings[i] + "{0}", i < strings.Length - 1 ? defaultSeparator : "");
        }
        Debug.Log(value);
    }

    /// <summary>
    /// Prints all values in an array separated with the default separator.
    /// </summary>
    /// <typeparam name="T">Generic type parameter. When you call it, it can be any type: string, int, Vector3 etc.</typeparam>
    /// <param name="array">Array of T to be printed.</param>
    public static void Array<T>(T[] array)
    {
        Array<T>(null, array);
    }

    /// <summary>
    /// Prints all values in an array.
    /// </summary>
    /// <typeparam name="T">Generic type parameter. When you call it, it can be any type: string, int, Vector3 etc.</typeparam>
    /// <param name="separator">Separator</param>
    /// <param name="array">Array of T to be printed.</param>
    public static void Array<T>(string separator, T[] array)
    {
        string value = null;
        for (int i = 0; i < array.Length; i++)
        {
            if (separator != null)
                value += String.Format(array[i].ToString() + "{0}", i < array.Length - 1 ? separator : "");
            else
                value += String.Format(array[i].ToString() + "{0}", i < array.Length - 1 ? defaultSeparator : "");
        }
        Debug.Log(value);
    }

    /// <summary>
    /// Prints all values in a two-dimensional array separated with the default separator.
    /// </summary>
    /// <typeparam name="T">Generic type parameter. When you call it, it can be any type: string, int, Vector3 etc.</typeparam>
    /// <param name="array">Array of T to be printed.</param>
    public static void Array2D<T>(T[,] array)
    {
        Array2D<T>(null, array);
    }

    /// <summary>
    /// Prints all values in a two-dimensional array.
    /// </summary>
    /// <typeparam name="T">Generic type parameter. When you call it, it can be any type: string, int, Vector3 etc.</typeparam>
    /// <param name="separator">Separator</param>
    /// <param name="array">Array of T to be printed.</param>
    public static void Array2D<T>(string separator, T[,] array)
    {
        string value = null;
        for (int i = 0; i < array.GetUpperBound(0); i++)
        {   
            for (int j = 0; j < array.GetUpperBound(1); j++) {
                if (separator != null) 
                    value += String.Format(array[i, 0].ToString() + ", " + array[i, 1].ToString() + "{0}", i*j < array.Length - 1 ? separator : "");
                else
                    value += String.Format(array[i, 0].ToString() + ", " + array[i, 1].ToString() + "{0}", i*j < array.Length - 1 ? defaultSeparator : "");
            }
        }
        Debug.Log(value);
    }

    /// <summary>
    /// Prints all values in a three-dimensional array separated with the default separator.
    /// </summary>
    /// <typeparam name="T">Generic type parameter. When you call it, it can be any type: string, int, Vector3 etc.</typeparam>
    /// <param name="array">Array of T to be printed.</param>
    public static void Array3D<T>(T[,,] array)
    {
        Array3D<T>(null, array);
    }

    /// <summary>
    /// Prints all values in a three-dimensional array.
    /// </summary>
    /// <typeparam name="T">Generic type parameter. When you call it, it can be any type: string, int, Vector3 etc.</typeparam>
    /// <param name="separator">Separator</param>
    /// <param name="array">Array of T to be printed.</param>
    public static void Array3D<T>(string separator, T[,,] array)
    {
        string value = null;
        int r = 1;
        for (int i = 0; i < array.GetLength(2); i++)
        {
            for (int y = 0; y < array.GetLength(1); y++)
            {
                for (int x = 0; x < array.GetLength(0); x++)
                {
                    if (separator != null)
                        value += String.Format(r.ToString() + ". " + "[" + x.ToString() + ", " + y.ToString() + ", " + i.ToString() + "] =" + array[x, y, i] + "{0}", i * y * x < array.Length - 1 ? separator : "");
                    else
                        value += String.Format(r.ToString() + ". " + "[" + x.ToString() + ", " + y.ToString() + ", " + i.ToString() + "] =" + array[x, y, i] + "{0}", i * y * x < array.Length - 1 ? defaultSeparator : "");
                    r++;
                }
            }
        }
        Debug.Log(value);
        Debug.Log(array.Length);
    }

    /// <summary>
    /// Prints all values in a list separated with the default separator
    /// </summary>
    /// <typeparam name="T">Generic type parameter. When you call it, it can be any type: string, int, Vector3 etc.</typeparam>
    /// <param name="list">List of T to be printed.</param>
    public static void List<T>(List<T> list)
    {
        List<T>(null, list);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Generic type parameter. When you call it, it can be any type: string, int, Vector3 etc.</typeparam>
    /// <param name="separator">Separator</param>
    /// <param name="list">List of T to be printed.</param>
    public static void List<T>(string separator, List<T> list)
    {
        string value = null;
        for (int i = 0; i < list.Count; i++)
        {
            if (separator != null)
                value += String.Format(list[i].ToString() + "{0}", i < list.Count - 1 ? separator : "");
            else
                value += String.Format(list[i].ToString() + "{0}", i < list.Count - 1 ? defaultSeparator : "");
        }
        Debug.Log(value);
    }

    /// <summary>
    /// Prints all keys and values in a dictionary separated with the default separator
    /// </summary>
    /// <typeparam name="T">Generic type parameter. When you call it, it can be any type: string, int, Vector3 etc.</typeparam>
    /// <typeparam name="K">Generic type parameter. When you call it, it can be any type: string, int, Vector3 etc.</typeparam>
    /// <param name="dictionary">Dictionary with key of T and values of K to be printed.</param>
    public static void Dictionary<T, K>(Dictionary<T, K> dictionary)
    {
        Dictionary<T, K>(null, dictionary);
    }
    
    /// <summary>
    /// Prints all keys and values in a dictionary.
    /// </summary>
    /// <typeparam name="T">Generic type parameter. When you call it, it can be any type: string, int, Vector3 etc.</typeparam>
    /// <typeparam name="K">Generic type parameter. When you call it, it can be any type: string, int, Vector3 etc.</typeparam>
    /// <param name="separator">Separator</param>
    /// <param name="dictionary">Dictionary with key of T and values of K to be printed.</param>
    public static void Dictionary<T, K>(string separator, Dictionary<T, K> dictionary)
    {
        string value = null;
        int i = 0;
        foreach (KeyValuePair<T, K> d in dictionary)
        {
            if (separator != null)
                value += String.Format(d.Key.ToString() + ", " + d.Value.ToString() + "{0}", i < dictionary.Count - 1 ? separator : "");
            else
                value += String.Format(d.Key.ToString() + ", " + d.Value.ToString() + "{0}", i < dictionary.Count - 1 ? defaultSeparator : "");

            i++;
        }
        Debug.Log(value);
    }

    /// <summary>
    /// Prints exception with details.
    /// </summary>
    /// <param name="e">Exception to be printed</param>
    public static void ExceptionDetails(Exception e)
    {
        string ex = "";
        ex += e.ToString();
        ex += "\nInnerException: " + e.InnerException;
        ex += "\nMessage: " + e.Message;
        ex += "\nSource: " + e.Source;
        ex += "\nStackTrace: " + e.StackTrace;
        ex += "\nTargetSite: " + e.TargetSite;
        Debug.Log(ex);
    }
}
