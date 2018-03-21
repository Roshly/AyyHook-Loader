using System;
using System.Text;

namespace ManualMapInjection.Injection
{
    internal static class Helpers
    {
        internal static string ToStringAnsi(byte[] buffer)
        {
            var sb = new StringBuilder();
            foreach (var t in buffer)
            {
                if (t == 0)
                {
                    break;
                }

                sb.Append((char) t);
            }

            return sb.ToString();
        }

        internal static bool _stricmp(char[] str1, char[] str2)
        {
            var min = Math.Min(str1.Length, str2.Length);
            for (var i = 0; i < min; i++)
            {
                if (str1[i] != str2[i])
                {
                    return false;
                }

                if (str1[i] == 0)
                {
                    break;
                }
            }

            return true;
        }
    }
}