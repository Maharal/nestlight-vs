using System.Collections.Generic;

namespace NestLight.Common
{
    /// <summary>A piece of a host string that the host language evaluates (<c>${x}</c>, <c>{x}</c>, <c>{{x}}</c>).</summary>
    internal struct Interpolation
    {
        /// <summary>Index of the opening delimiter.</summary>
        public readonly int Start;
        /// <summary>Index right after the closing delimiter (end of text when the interpolation is still open).</summary>
        public readonly int End;
        public readonly int OpenLength;
        /// <summary>0 when the closing delimiter is missing.</summary>
        public readonly int CloseLength;

        public Interpolation(int start, int end, int openLength, int closeLength)
        {
            Start = start; End = end; OpenLength = openLength; CloseLength = closeLength;
        }

        public bool Closed { get { return CloseLength > 0; } }
        public int InnerStart { get { return Start + OpenLength; } }
        public int InnerEnd { get { return End - CloseLength; } }
    }

    /// <summary>A source sequence that stands for a single character in the string value (<c>""</c>, <c>\"</c>, <c>{{</c>).</summary>
    internal struct EscapeSequence
    {
        public readonly int Start;
        public readonly int Length;
        public readonly char Value;

        public EscapeSequence(int start, int length, char value)
        {
            Start = start; Length = length; Value = value;
        }
    }

    /// <summary>A string literal of the host that holds code of another language.</summary>
    internal sealed class EmbeddedString
    {
        public EmbeddedString(string embeddedLanguageId)
        {
            EmbeddedLanguageId = embeddedLanguageId;
        }

        /// <summary>Lower-case language id or alias that marked the string (<c>html</c>, <c>sql</c>...), or the one guessed from its content; null while a string nobody marked has not been guessed.</summary>
        public string EmbeddedLanguageId { get; internal set; }

        /// <summary>First character of the literal, including prefixes and the opening quote.</summary>
        public int OuterStart;
        /// <summary>First character of the content.</summary>
        public int Start;
        /// <summary>Index right after the content (the closing quote, or the end of the text when unclosed).</summary>
        public int End;
        /// <summary>Index right after the closing quote.</summary>
        public int OuterEnd;

        /// <summary>In source order. Never overlap each other.</summary>
        public readonly List<Interpolation> Interpolations = new List<Interpolation>();
        /// <summary>In source order. Never overlap each other or the interpolations.</summary>
        public readonly List<EscapeSequence> Escapes = new List<EscapeSequence>();
    }

    internal sealed class Token
    {
        public readonly int Start;
        public readonly int Length;
        public readonly string Type;

        public Token(int start, int length, string type)
        {
            Start = start; Length = length; Type = type;
        }

        public int End { get { return Start + Length; } }
    }
}
