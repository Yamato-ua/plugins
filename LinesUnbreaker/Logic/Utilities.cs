using System;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Nikse.SubtitleEdit.PluginLogic
{
    internal static class Utilities
    {
        #region MyRegion
        public static bool Contains(this string s, char c)
        {
            return s.Length > 0 && s.IndexOf(c) > 0;
        }
        public static string[] SplitToLines(this string s)
        {
            return s.Replace(Environment.NewLine, "\n").Replace('\r', '\n').Split('\n');
        }
        public static bool StartsWith(this string s, char c)
        {
            return s.Length > 0 && s[0] == c;
        }
        public static bool EndsWith(this string s, char c)
        {
            return s.Length > 0 && s[s.Length - 1] == c;
        }
        #endregion

        public static string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public static bool IsInteger(string s)
        {
            int i;
            return int.TryParse(s, out i);
        }

        public static string RemoveHtmlFontTag(string s)
        {
            s = Regex.Replace(s, "(?i)</?font>", string.Empty);
            var idx = s.IndexOf("<font", StringComparison.OrdinalIgnoreCase);
            while (idx >= 0)
            {
                var endIdx = s.IndexOf('>', 5);
                if (endIdx < 5)
                    break;
                s = s.Remove(idx, endIdx - idx + 1);
                idx = s.IndexOf("<font", idx, StringComparison.OrdinalIgnoreCase);
            }
            return s;
        }

        public static string RemoveHtmlTags(string s, bool alsoSsa = false)
        {
            int idx;
            // {\an4}
            if (alsoSsa)
            {
                const string find = "{\\";
                idx = s.IndexOf(find, StringComparison.Ordinal);
                while (idx >= 0)
                {
                    var endIdx = s.IndexOf('}', idx + 2);
                    if (endIdx < idx)
                        break;
                    s = s.Remove(idx, endIdx - idx + 1);
                    idx = s.IndexOf(find, idx, StringComparison.Ordinal);
                }
            }

            if (string.IsNullOrEmpty(s) || !s.Contains('<'))
                return s;
            //s = Regex.Replace(s, "(?i)</?[iіbu]>", string.Empty);
            idx = s.IndexOf('<');
            while (idx >= 0)
            {
                var endIdx = s.IndexOf('>', idx);
                if (endIdx < idx)
                    break;
                s = s.Remove(idx, endIdx - idx + 1);
                idx = s.IndexOf('<', idx);
            }
            return RemoveHtmlFontTag(s).Trim();
        }

        public static string GetHtmlColorCode(Color color)
        {
            return string.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
        }

        public static bool IsDialog(string text)
        {
            return false;
        }

        public static bool IsMood(string text)
        {
            return false;
        }

        public static bool IsNarrator(string text)
        {
            return false;
        }

        public static string UnbreakLine(string text)
        {
            if (!text.Contains(Environment.NewLine))
                return text;

            var singleLine = text.Replace(Environment.NewLine, " ");
            while (singleLine.Contains("  "))
                singleLine = singleLine.Replace("  ", " ");

            if (singleLine.Contains("</")) // Fix tag
            {
                singleLine = singleLine.Replace("</i> <i>", " ");
                singleLine = singleLine.Replace("</i><i>", " ");

                singleLine = singleLine.Replace("</b> <b>", " ");
                singleLine = singleLine.Replace("</b><b>", " ");

                singleLine = singleLine.Replace("</u> <u>", " ");
                singleLine = singleLine.Replace("</u><u>", " ");
            }
            return singleLine;
        }
    }
}