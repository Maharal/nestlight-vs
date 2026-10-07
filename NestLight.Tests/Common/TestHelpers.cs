using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Highlighting;

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

    /// <summary>The real pipeline, built by the composition root, without depending on Visual Studio.</summary>
    internal static class Pipeline
    {
        public static readonly ILanguageRegistry Languages = NestLightComposition.CreateLanguages();

        public static IHostScanner Scanner(HostLanguage host)
        {
            return NestLightComposition.CreateScanner(host, Languages);
        }

        public static IHighlighter Highlighter(HostLanguage host)
        {
            return new HighlightEngine(Scanner(host), Languages);
        }
    }

    /// <summary>Runs code of a host through the pipeline and exposes the result as text.</summary>
    internal static class Lexer
    {
        // ---- code builders (JavaScript host) ----
        public static string H(string html) { return "html`" + html + "`"; }
        public static string C(string css) { return "css`" + css + "`"; }
        public static string In(string id, string code) { return id + "`" + code + "`"; }

        // ---- scanning ----
        public static IReadOnlyList<EmbeddedString> Templates(string code)
        {
            return Scan(HostLanguage.JavaScript, code);
        }

        public static IReadOnlyList<EmbeddedString> Scan(HostLanguage host, string code)
        {
            return Pipeline.Scanner(host).Scan(code);
        }

        /// <summary>The canonical id of the language of the string (svg and htm are html).</summary>
        public static string Family(EmbeddedString s)
        {
            return Pipeline.Languages.Find(s.LanguageId).Ids[0];
        }

        // ---- highlighting ----
        public static List<Tok> Lex(string code)
        {
            return Lex(HostLanguage.JavaScript, code);
        }

        public static List<Tok> Lex(HostLanguage host, string code)
        {
            return Pipeline.Highlighter(host).Highlight(code).Select(t => new Tok
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

        public static string[] Texts(HostLanguage host, string code, string type)
        {
            return Texts(Lex(host, code), type);
        }

        /// <summary>Full sequence in the "type|text" format (type without the "template." prefix).</summary>
        public static string[] Seq(string code)
        {
            return Seq(HostLanguage.JavaScript, code);
        }

        public static string[] Seq(HostLanguage host, string code)
        {
            return Lex(host, code).Select(t => t.Type.Replace("template.", "") + "|" + t.Text).ToArray();
        }

        /// <summary>Sequence of an embedded language: the code goes in a JavaScript template tagged with <paramref name="id"/>,
        /// and the interpolation tokens are left out.</summary>
        public static string[] Language(string id, string code)
        {
            return Lex(In(id, code))
                .Where(t => !t.Type.StartsWith("template.expression"))
                .Select(t => t.Type.Replace("template.", "") + "|" + t.Text).ToArray();
        }

        public static bool HasOverlap(List<Tok> toks)
        {
            for (int i = 1; i < toks.Count; i++)
                if (toks[i].Start < toks[i - 1].End) return true;
            return false;
        }
    }
}
