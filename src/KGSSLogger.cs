using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;



class KGSSLogger
{
    public static void Log(String s)
    {
#if (DEBUG)
        Debug.Log(s);
#endif
    }

    public static string FormatClassName(string s)
    {
        return "[" + s + "]";
    }
}

