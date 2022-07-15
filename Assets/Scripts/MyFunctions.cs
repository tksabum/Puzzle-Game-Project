using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFunctions {
    public class MyFunctions
    {
        public static bool SameAtLeastOne<T>(params T[] values)
        {
            for (int i = 1; i < values.Length; i++)
            {
                if (values[0].Equals(values[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}