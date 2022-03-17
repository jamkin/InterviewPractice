namespace CTCI.Problems.Chapters._1
{
    using Algorithm.Extensions;
    using Algorithm.Decorations;
    using CTCI.Decorations;
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;

    /// <summary>
    /// String questions from Chapter 1
    /// </summary>
    public static class StringQuestions
    {
        [CtciProblem(Name = "Is Unique", Chapter = 1, Problem = 1)]
        public static bool IsUnique(string str)
        {
            if (str is null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            return str.IsUnique();
        }

        [CtciProblem(Name = "Check Permutation", Chapter = 1, Problem = 2)]
        public static bool CheckPermutation(string first, string second)
        {
            if (first is null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (second is null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            if (first.Length != second.Length)
            {
                return false;
            }

            return first.IsPermutationOf(second);
        }

        [CtciProblem(
            Name = "URLify",
            Chapter = 1, Problem = 3,
            Description = 
@"Write a method to replace all spaces in a string with '%20'. You may assume that the string
 has sufficient space at the end to hold the additional characters, and that you are given the ""true""
 length of the string. (Note: if implementing in Java, please use a character array so that you can
 perform this operation in place.)
 EXAMPLE
 Input:    ""Mr John Smith    "", 13
 Output:   ""Mr%20John%20Smith""")]
        public static string URLify(char[] url, int length)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (length < 0 || length > url.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length), length.ToString());
            }

            throw new NotImplementedException();
        }

        [CtciProblem(
            Name = "Palindrome Permutation",
            Chapter = 1, Problem = 4,
            Description =
@"Given a string, write a function to check if it is a permutation of a palindrome.
  A palindrome is a word or phrase that is the same forwards as backwards. A
  permutation is a rearrangement of letters. The palindrome does not need to be
  limited to just dictionary words.
  EXAMPLE
  Input:   Tact Coa
  Output:  True (permutations: ""taco cat"", ""atco cta"", etc.")]
        public static bool PalindromePermutation(string str)
        {
            if (str is null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            throw new NotImplementedException();
        }

        [CtciProblem(
            Name = "One Away",
            Chapter = 1, Problem = 5,
            Description =
@"There are three types of edits than can be performed on strings: insert a character,
  remove a character, or replace a character. Given two strings, write a function to
  check if they are one edit (or zero edits) away.
  EXAMPLE
  pale,   ple  -> true
  pales,  pale -> true
  pale,   bale -> true
  pale,   bake -> false")]
        public static bool OneAWay(string first, string second)
        {
            if (first is null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (second is null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            throw new NotImplementedException();
        }

        [AlgorithmImplementationStyle(Flavor = AlgorithmImplementationStyle.LINQy)]
        [CtciProblem(
            Name = "String Compression",
            Chapter = 1, Problem = 6,
            Description =
@"Implement a metjod to perform basic string compression using the counts
  of repeated characters. For example, the string aabcccccaaa would become
  a2b1c5a3. If the ""compressed"" string would not become smaller than the
  original string, your method should return the original string. You can
  assume the string has only uppercase and lowercase letters (a - z).")]
        public static string StringCompression(string str)
        {
            if (str is null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            return CompressString_Optimized(str);
        }

        [CtciProblem(
            Name = "String Rotation",
            Chapter = 1, Problem = 9,
            Description =
@"Assume you have a method isSubstring which checks if one word is a substring
  of another. Given two strings, s1 and s2, write code to check if s2 is a rotation
  of s1 using only one call to isSubstring (e.g., ""waterbottle"" is a rotation of ""erbottlewat""")]
        public static bool StringRotation(string first, string second)
        {
            if (first is null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (second is null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            throw new NotImplementedException();
        }

        private static string CompressString_LINQy(string str)
        {

            // This one is LINQy as heck, but at the expense of efficiency.
            return str
                .Select(c =>
                {
                    ValidateIsLetter(c);
                    return c;
                })
                .AdjacentCounts()
                .SelectMany(kvp =>
                {
                    (char character, int count) = (kvp.Key, kvp.Value);
                    return count > 3
                        ? Enumerable<string>.CreateFrom(count.ToString(), character.ToString())
                        : Enumerable<string>.CreateFrom(character.ToString());
                })
                .Combine();
        }

        private static string CompressString_Optimized(in string str)
        {
            if (str.Length < 3)
            {
                return str;
            }

            // TODO(?) return original string if not compressed at all
            int n = str.Length;         
            var compstr = new char[n]; // buffer
            int i0 = 0, i1 = 1, j = i0;
            ValidateIsLetter(str[i0]); // fencepost fix
            for (; ;)
            {
                if (i1 == n) // 1 off-the-end
                {
                    AddChars(compstr, ref j, str[i0], i1 - i0);
                    break;
                }
                else if (str[i0] != str[i1])
                {
                    ValidateIsLetter(str[i1]);
                    AddChars(compstr, ref j, str[i0], i1 - i0);
                    i0 = i1;
                }
                ++i1;
            }
            return new string(compstr, startIndex: 0, length: j);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddChars(in char[] chars, ref int i, in char c, int count)
        {
            int _i = i;
            if (count == 1)
            {
                chars[i++] = c;
            }
            else if (count == 2)
            {
                chars[i++] = chars[i++] = c;
            }
            else
            {
                var countStr = count.ToString(); // TODO(?) digits 1-by-1 to avoid string allocation
                for (int j = 0, m = countStr.Length; j < m; ++j)
                {
                    chars[i++] = countStr[j];
                }
                chars[i++] = c;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ValidateIsLetter(in char c)
        {
            if (!char.IsLetter(c))
            {
                throw new InvalidOperationException($"Cannot compress string because it contains non-letter '{c}'.");
            }
        }
    }
}
