using System;
using System.Linq;
using System.Collections.Generic;

/* *****************************************************************************
 * File:    GenericExtensions.cs
 * Author:  Philip Pierce - Thursday, September 18, 2014
 * Description:
 *  Generic (T) Extensions
 *  
 * History:
 *  Thursday, September 18, 2014 - Created
 * ****************************************************************************/

/// <summary>
/// Generic (T) Extensions
/// </summary>
public static class GenericEx
{
    #region AsList

    /// <summary>
    /// Wraps the given object into a List{T} and returns the list.
    /// </summary>
    /// <param name="tobject">The object to be wrapped.</param>
    /// <typeparam name="T">Refers the object to be returned as List{T}.</typeparam>
    /// <returns>Returns List{T}.</returns>
    public static List<T> AsList<T>(this T tobject)
    {
        return new List<T> { tobject };
    }

    // AsList
    #endregion
}