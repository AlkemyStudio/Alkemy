using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class DebugArray
    {
        public static void LogArray<T>(IEnumerable<T> array)
        {
            // Log array with indentation
            string arrayString = array.Aggregate("[\n", (current, t) => current + "    " + t + ",\n");
            arrayString += "]";
            Debug.Log(arrayString);
        }
    }
}