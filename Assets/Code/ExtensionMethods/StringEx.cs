using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

/* *****************************************************************************
 * File:    StringExtensions.cs
 * Author:  Philip Pierce - Tuesday, September 09, 2014
 * Description:
 *  Extensions for strings
 *  
 * History:
 *  Tuesday, September 09, 2014 - Created
 * ****************************************************************************/

/// <summary>
/// Extensions for strings
/// </summary>
public static class StringEx
{
    #region IsInt

    /// <summary>
    /// Returns true if value is an int
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsInt(this string value)
    {
        try
        {
            int tempInt;
            return int.TryParse(value, out tempInt);
        }

        catch (Exception)
        {
            return false;
        }
    }

    // IsInt
    #endregion

    #region Take

    /// <summary>
    /// Like LINQ take - takes the first x characters
    /// </summary>
    /// <param name="value">the string</param>
    /// <param name="count">number of characters to take</param>
    /// <param name="ellipsis">true to add ellipsis (...) at the end of the string</param>
    /// <returns></returns>
    public static string Take(this string value, int count, bool ellipsis = false)
    {
        // get number of characters we can actually take
        int lengthToTake = Math.Min(count, value.Length);

        // Take and return
        return (ellipsis && lengthToTake < value.Length) ?
            string.Format("{0}...", value.Substring(0, lengthToTake)) :
            value.Substring(0, lengthToTake);
    }

    // Take
    #endregion

    #region Skip

    /// <summary>
    /// like LINQ skip - skips the first x characters and returns the remaining string
    /// </summary>
    /// <param name="value">the string</param>
    /// <param name="count">number of characters to skip</param>
    /// <returns></returns>
    public static string Skip(this string value, int count)
    {
        return value.Substring(Math.Min(count, value.Length) - 1);
    }

    // Skip
    #endregion

    #region Reverse

    /// <summary>
    /// Reverses the string
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Reverse(this string input)
    {
        char[] chars = input.ToCharArray();
        Array.Reverse(chars);
        return new String(chars);
    }

    // Reverse
    #endregion

    #region IsNullOrEmpty

    /// <summary>
    /// Null or empty check as extension
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }

    // IsNullOrEmpty
    #endregion

    #region IsNOTNullOrEmpty

    /// <summary>
    /// Returns true if the string is Not null or empty
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNOTNullOrEmpty(this string value)
    {
        return (!string.IsNullOrEmpty(value));
    }

    // IsNOTNullOrEmpty
    #endregion

    #region Match

    /// <summary>
    /// Returns true if the pattern matches
    /// </summary>
    /// <param name="value"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static bool Match(this string value, string pattern)
    {
        return Regex.IsMatch(value, pattern);
    }

    // Match
    #endregion

    #region RemoveSpaces

    /// <summary>
    /// Remove white space, not line end
    /// Useful when parsing user input such phone,
    /// price int.Parse("1 000 000".RemoveSpaces(),.....
    /// </summary>
    /// <param name="value"></param>
    public static string RemoveSpaces(this string value)
    {
        return value.Replace(" ", string.Empty);
    }

    // RemoveSpaces
    #endregion

    #region ToStringPretty

    /*
    * Converting a sequence to a nicely-formatted string is a bit of a pain. 
    * The String.Join method definitely helps, but unfortunately it accepts an 
    * array of strings, so it does not compose with LINQ very nicely.
    * 
    * My library includes several overloads of the ToStringPretty operator that 
    * hides the uninteresting code. Here is an example of use:
    * 
    * Console.WriteLine(Enumerable.Range(0, 10).ToStringPretty("From 0 to 9: [", ",", "]"));
    * 
    * The output of this program is:
    * 
    * From 0 to 9: [0,1,2,3,4,5,6,7,8,9]
    */

    /// <summary>
    /// Returns a comma delimited string
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    public static string ToStringPretty<T>(this IEnumerable<T> source)
    {
        return (source == null) ? string.Empty : ToStringPretty(source, ",");
    }

    /// <summary>
    /// Returns a single string, delimited with <paramref name="delimiter"/> from source
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="delimiter"></param>
    /// <returns></returns>
    public static string ToStringPretty<T>(this IEnumerable<T> source, string delimiter)
    {
        return (source == null) ? string.Empty : ToStringPretty(source, string.Empty, delimiter, string.Empty);
    }

    /// <summary>
    /// Returns a delimited string, appending <paramref name="before"/> at the start,
    /// and <paramref name="after"/> at the end of the string
    /// Ex: Enumerable.Range(0, 10).ToStringPretty("From 0 to 9: [", ",", "]")
    /// returns: From 0 to 9: [0,1,2,3,4,5,6,7,8,9]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="before"></param>
    /// <param name="delimiter"></param>
    /// <param name="after"></param>
    /// <returns></returns>
    public static string ToStringPretty<T>(this IEnumerable<T> source, string before, string delimiter, string after)
    {
        if (source == null)
            return string.Empty;

        StringBuilder result = new StringBuilder();
        result.Append(before);

        bool firstElement = true;
        foreach (T elem in source)
        {
            if (firstElement) firstElement = false;
            else result.Append(delimiter);

            result.Append(elem.ToString());
        }

        result.Append(after);
        return result.ToString();
    }

    // ToStringPretty
    #endregion


    #region InvertCase

    /// <summary>
    /// Inverts the case of each character in the given string and returns the new string.
    /// </summary>
    /// <param name="s">The given string.</param>
    /// <returns>The converted string.</returns>
    public static string InvertCase(this string s)
    {
        return new string(
            s.Select(c => char.IsLetter(c) ? (char.IsUpper(c) ?
                  char.ToLower(c) : char.ToUpper(c)) : c).ToArray());
    }

    // InvertCase
    #endregion

    #region IsNullOrEmptyAfterTrimmed

    /// <summary>
    /// Checks whether the given string is null, else if empty after trimmed.
    /// </summary>
    /// <param name="s">The given string.</param>
    /// <returns>True if string is Null or Empty, false otherwise.</returns>
    public static bool IsNullOrEmptyAfterTrimmed(this string s)
    {
        return (s.IsNullOrEmpty() || s.Trim().IsNullOrEmpty());
    }

    // IsNullOrEmptyAfterTrimmed
    #endregion

    #region ToCharList

    /// <summary>
    /// Converts the given string to <see cref="List{Char}"/>.
    /// </summary>
    /// <param name="s">The given string.</param>
    /// <returns>Returns a list of char (or null if string is null).</returns>
    public static List<char> ToCharList(this string s)
    {
        return (s.IsNOTNullOrEmpty()) ? 
            s.ToCharArray().ToList() : 
            null;
    }

    // ToCharList
    #endregion

    #region SubstringFromXToY

    /// <summary>
    /// Extracts the substring starting from 'start' position to 'end' position.
    /// </summary>
    /// <param name="s">The given string.</param>
    /// <param name="start">The start position.</param>
    /// <param name="end">The end position.</param>
    /// <returns>The substring.</returns>
    public static string SubstringFromXToY(this string s, int start, int end)
    {
        if (s.IsNullOrEmpty())
            return string.Empty;

        // if start is past the length of the string
        if (start >= s.Length)
            return string.Empty;

        // if end is beyond the length of the string, reset
        if (end >= s.Length)
            end = s.Length - 1;

        return s.Substring(start, end - start);
    }

    // SubstringFromXToY
    #endregion

    #region RemoveChar

    /// <summary>
    /// Removes the given character from the given string and returns the new string.
    /// </summary>
    /// <param name="s">The given string.</param>
    /// <param name="c">The character to be removed.</param>
    /// <returns>The new string.</returns>
    public static string RemoveChar(this string s, char c)
    {
        return (s.IsNOTNullOrEmpty()) ? s.Replace(c.ToString(), string.Empty) : string.Empty;
    }

    // RemoveChar
    #endregion

    #region GetWordCount

    /// <summary>
    /// Returns the number of words in the given string.
    /// </summary>
    /// <param name="s">The given string.</param>
    /// <returns>The word count.</returns>
    public static int GetWordCount(this string s)
    {
        return (new Regex(@"\w+")).Matches(s).Count;
    }

    // GetWordCount
    #endregion

    #region IsPalindrome

    /// <summary>
    /// Checks whether the given string is a palindrome.
    /// </summary>
    /// <param name="s">The given string.</param>
    /// <returns>True if the given string is palindrome, false otherwise.</returns>
    public static bool IsPalindrome(this string s)
    {
        return s.Equals(s.Reverse());
    }

    // IsPalindrome
    #endregion

    #region IsNotPalindrome

    /// <summary>
    /// Checks whether the given string is NOT a palindrome.
    /// </summary>
    /// <param name="s">The given string.</param>
    /// <returns>True if the given string is NOT palindrome, false otherwise.</returns>
    public static bool IsNotPalindrome(this string s)
    {
        return s.IsPalindrome()==false;
    }

    // IsNotPalindrome
    #endregion

    #region IsValidIPAddress

    /// <summary>
    /// Checks whether the given string is a valid IP address using regular expressions.
    /// </summary>
    /// <param name="s">The given string.</param>
    /// <returns>True if it is a valid IP address, false otherwise.</returns>
    public static bool IsValidIPAddress(this string s)
    {
        return Regex.IsMatch(s, @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b");
    }

    // IsValidIPAddress
    #endregion

    #region ToTitleCase

    /// <summary>
    /// Converts the specified string to title case (except for words that are entirely in uppercase, which are considered to be acronyms).
    /// </summary>
    /// <param name="mText"></param>
    /// <returns></returns>
    public static string ToTitleCase(this string mText)
    {
        if (mText.IsNullOrEmpty()) 
            return mText;

        // get globalization info
        System.Globalization.CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
        System.Globalization.TextInfo textInfo = cultureInfo.TextInfo;

        // convert to title case
        return textInfo.ToTitleCase(mText);
    }

    // ToTitleCase
    #endregion

    /// <summary>
    /// Finds the specified Start Text and the End Text in this string instance, and returns a string
    /// containing all the text starting from startText, to the beginning of endText. (endText is not
    /// included.)
    /// usage: "This is a tester for my cool extension method!!".Subsetstring("tester", "cool",true);
    /// Output: "tester for my "
    /// </summary>
    /// <param name="s">The string to retrieve the subset from.</param>
    /// <param name="startText">The Start Text to begin the Subset from.</param>
    /// <param name="endText">The End Text to where the Subset goes to.</param>
    /// <param name="ignoreCase">Whether or not to ignore case when comparing startText/endText to the string.</param>
    /// <returns>A string containing all the text starting from startText, to the begining of endText.</returns>
    public static string SubsetString(this string s, string startText, string endText, bool ignoreCase)
    {
        if (s.IsNullOrEmpty())
            return string.Empty;
        
        if (startText.IsNullOrEmpty() || endText.IsNullOrEmpty())
            throw new ArgumentException("Start Text and End Text cannot be empty.");

        // set our starting values
        string tempStr = ignoreCase ? s.ToUpperInvariant() : s;
        int start = ignoreCase ? tempStr.IndexOf(startText.ToUpperInvariant()) : tempStr.IndexOf(startText);
        int end = ignoreCase ? tempStr.IndexOf(endText.ToUpperInvariant(), start) : tempStr.IndexOf(endText, start);

        // get the substring
        return SubstringFromXToY(tempStr, start, end);
    }
}
