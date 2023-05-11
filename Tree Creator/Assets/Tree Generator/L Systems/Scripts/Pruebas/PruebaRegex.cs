using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using org.matheval;

public class PruebaRegex : MonoBehaviour
{
    public string input;
    public string search;

    public int startIndex;
    public int length;

    public char[] listChar;


    void Start()
    {
        search = Regex.Replace(input, "hola", "");
        print(search);
        print(search.Length);


        // var match = Regex.Matches(input, search);
        // // print( regex.Replace(input, reemplazo));
        // for (int i = 0; i < match.Count; i++)
        // {
        //     print(match[i].Value);
        // }
        // // print(input.Contains(regex));
    }
}
