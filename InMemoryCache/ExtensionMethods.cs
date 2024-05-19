﻿using System;
using System.Collections.Generic;

namespace InMemoryCache
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class ExtensionMethods
    {
        public static bool IsSortedBy<T>(this List<T> list, Comparison<T> comparison)
        {
            for (int i = 1; i < list.Count; i++)
            {
                if (comparison(list[i - 1], list[i]) > 0)
                    return false;
            }
            return true;
        }
    }
}
