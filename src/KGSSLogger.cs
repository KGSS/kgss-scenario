﻿using System;
using System.Collections.Generic;
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

    public static string ListToString(List<int> l)
    {
        string ret = "[";
        foreach (int i in l)
        {
            ret +=  i + ",";
        }

        ret = ret.Remove(ret.LastIndexOf(","));
        ret += "]";

        return ret;
    }
}
