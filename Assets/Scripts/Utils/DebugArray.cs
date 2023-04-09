using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class DebugArray
    {
        /// <summary>
        /// Log array with indentation
        /// </summary>
        /// <param name="array">The array to log</param>
        /// <typeparam name="T">The type of the array</typeparam>
        public static void LogArray<T>(IEnumerable<T> array)
        {
            // Log array with indentation
            string arrayString = array.Aggregate("[\n", (current, t) => current + "    " + t + ",\n");
            arrayString += "]";
            Debug.Log(arrayString);
        }
    }
}