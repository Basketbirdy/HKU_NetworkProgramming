using UnityEngine;
using System;

public static class TypeUtils
{
    public static bool CompareType<T>(Type typeToCompare)
    {
        if(typeToCompare == typeof(T)) { return true; }
        return false;
    }
}
