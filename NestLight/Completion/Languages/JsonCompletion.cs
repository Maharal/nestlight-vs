using System.Collections.Generic;

namespace NestLight.Completion
{
    /// <summary>JSON: <c>true</c>, <c>false</c> and <c>null</c>, and a grammar that tells keys from values.</summary>
    internal sealed class JsonCompletion : CompletionLanguage
    {
        public JsonCompletion() : base(new[] { "json" }, new[] { "true", "false", "null" }) { }

        // ---- the grammar --------------------------------------------------------------------------------------

        protected override Position ReadPosition(string text, int floor, CompletionSite site)
        {
            // a key is a string right after '{' or ',' inside an object; the keywords (true, false, null) are values
            var stack = new List<char>();
            bool inString = false, isKey = false;
            char previous = ' ';
            for (int i = floor; i < site.Start; i++)
            {
                char c = text[i];
                if (inString)
                {
                    if (c == '\\') i++;
                    else if (c == '"') { inString = false; previous = '"'; }
                    continue;
                }
                if (c == '"') { inString = true; isKey = stack.Count > 0 && stack[stack.Count - 1] == '{' && (previous == '{' || previous == ','); continue; }
                if (char.IsWhiteSpace(c)) continue;
                if (c == '{' || c == '[') stack.Add(c);
                else if ((c == '}' || c == ']') && stack.Count > 0) stack.RemoveAt(stack.Count - 1);
                previous = c;
            }
            if (inString) return new Position(isKey ? "json:key" : "json:string", onlyWords: true);
            return null; // a value outside a string: true, false and null belong there
        }
    }
}
