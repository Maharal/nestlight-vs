using System;
using System.Collections.Generic;
using System.Linq;

namespace NestLight.Tests
{
    internal sealed class Tok
    {
        public string Type;
        public string Text;
        public int Start;
        public int Length;
        public int End { get { return Start + Length; } }
        public override string ToString() { return Type + "|" + Text + "|"; }
    }

    /// <summary>Reproduces the classifier pipeline (scan + tokenization + sorting), without depending on VS.</summary>
    internal static class Lexer
    {
        public static string H(string html) { return "html`" + html + "`"; }
        public static string C(string css) { return "css`" + css + "`"; }

        public static List<TemplateInfo> Templates(string code)
        {
            return TplScanner.FindTemplates(code);
        }

        public static List<Tok> Lex(string code)
        {
            var templates = TplScanner.FindTemplates(code);
            var raw = new List<TplToken>();
            foreach (var t in templates) TplHtmlTokenizer.Tokenize(code, t, templates, raw);
            raw.Sort((a, b) => a.Start.CompareTo(b.Start));
            return raw.Select(t => new Tok
            {
                Type = t.Type,
                Text = code.Substring(t.Start, t.Length),
                Start = t.Start,
                Length = t.Length
            }).ToList();
        }

        public static string[] Texts(List<Tok> toks, string type)
        {
            return toks.Where(t => t.Type == type).Select(t => t.Text).ToArray();
        }

        public static string[] Texts(string code, string type)
        {
            return Texts(Lex(code), type);
        }

        /// <summary>Full sequence in the "type|text" format (type without the "template." prefix).</summary>
        public static string[] Seq(string code)
        {
            return Lex(code).Select(t => t.Type.Replace("template.", "") + "|" + t.Text).ToArray();
        }

        public static bool HasOverlap(List<Tok> toks)
        {
            for (int i = 1; i < toks.Count; i++)
                if (toks[i].Start < toks[i - 1].End) return true;
            return false;
        }
    }
}
