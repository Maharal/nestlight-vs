using System;
using System.Collections.Generic;

namespace NestLight.Common
{
    internal struct RenameReference
    {
        public readonly int Start;
        public readonly int Length;
        public readonly string Type;

        public RenameReference(int start, int length, string type) { Start = start; Length = length; Type = type; }
    }

    internal static class RenameEngine
    {
        public static IReadOnlyList<RenameReference> FindReferences(IReadOnlyList<Token> tokens, string text, int position)
        {
            Token current = TokenLookup.AtPosition(tokens, position);
            if (current == null) return null;

            string target = null;
            string prefix = null;

            if (current.Type == ClassificationNames.CssSelectorClass)
            {
                target = text.Substring(current.Start, current.Length);
                prefix = ".";
            }
            else if (current.Type == ClassificationNames.CssSelectorId)
            {
                target = text.Substring(current.Start, current.Length);
                prefix = "#";
            }
            else if (current.Type == ClassificationNames.AttributeValue)
            {
                Token attr = TokenLookup.PrecedingOfType(tokens, current.Start,
                    ClassificationNames.Attribute, ClassificationNames.AttributeEvent,
                    ClassificationNames.AttributeProperty, ClassificationNames.AttributeBoolean);
                if (attr == null) return null;
                string attrName = text.Substring(attr.Start, attr.Length).ToLowerInvariant();
                if (attrName == "class" || attrName == "classname")
                {
                    string value = text.Substring(current.Start, current.Length).Trim('"', '\'');
                    string[] classes = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    int localOffset = position - current.Start;
                    string unquoted = text.Substring(current.Start, current.Length);
                    if (unquoted.Length > 0 && (unquoted[0] == '"' || unquoted[0] == '\'')) localOffset--;
                    int at = 0;
                    foreach (string cls in classes)
                    {
                        int idx = value.IndexOf(cls, at, StringComparison.Ordinal);
                        if (localOffset >= idx && localOffset < idx + cls.Length)
                        {
                            target = "." + cls;
                            prefix = ".";
                            break;
                        }
                        at = idx + cls.Length;
                    }
                }
                else if (attrName == "id")
                {
                    string value = text.Substring(current.Start, current.Length).Trim('"', '\'');
                    target = "#" + value;
                    prefix = "#";
                }
            }

            if (target == null) return null;

            var references = new List<RenameReference>();
            string selectorType = prefix == "." ? ClassificationNames.CssSelectorClass : ClassificationNames.CssSelectorId;
            string attrType = prefix == "." ? "class" : "id";
            string nameOnly = target.Substring(1);

            for (int i = 0; i < tokens.Count; i++)
            {
                Token t = tokens[i];
                if (t.Start + t.Length > text.Length) continue;

                if (t.Type == selectorType)
                {
                    string candidate = text.Substring(t.Start, t.Length);
                    if (string.Equals(candidate, target, StringComparison.OrdinalIgnoreCase))
                        references.Add(new RenameReference(t.Start + 1, t.Length - 1, t.Type));
                }
                else if (t.Type == ClassificationNames.AttributeValue)
                {
                    Token prevAttr = TokenLookup.PrecedingOfType(tokens, t.Start,
                        ClassificationNames.Attribute, ClassificationNames.AttributeEvent,
                        ClassificationNames.AttributeProperty, ClassificationNames.AttributeBoolean);
                    if (prevAttr == null) continue;
                    string pAttrName = text.Substring(prevAttr.Start, prevAttr.Length).ToLowerInvariant();
                    bool isClass = (pAttrName == "class" || pAttrName == "classname") && prefix == ".";
                    bool isId = pAttrName == "id" && prefix == "#";
                    if (!isClass && !isId) continue;

                    string raw = text.Substring(t.Start, t.Length);
                    int quoteOffset = 0;
                    if (raw.Length > 0 && (raw[0] == '"' || raw[0] == '\'')) quoteOffset = 1;
                    string value = raw.Trim('"', '\'');

                    if (isId && string.Equals(value, nameOnly, StringComparison.OrdinalIgnoreCase))
                    {
                        references.Add(new RenameReference(t.Start + quoteOffset, value.Length, t.Type));
                    }
                    else if (isClass)
                    {
                        string[] parts = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        int searchFrom = 0;
                        foreach (string part in parts)
                        {
                            if (string.Equals(part, nameOnly, StringComparison.OrdinalIgnoreCase))
                            {
                                int idx = value.IndexOf(part, searchFrom, StringComparison.Ordinal);
                                references.Add(new RenameReference(t.Start + quoteOffset + idx, part.Length, t.Type));
                            }
                            searchFrom = value.IndexOf(part, searchFrom, StringComparison.Ordinal) + part.Length;
                        }
                    }
                }
            }

            return references.Count > 0 ? references : null;
        }
    }
}
