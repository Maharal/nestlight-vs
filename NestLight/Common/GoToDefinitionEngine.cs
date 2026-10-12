using System;
using System.Collections.Generic;

namespace NestLight.Common
{
    internal struct DefinitionResult
    {
        public readonly int Start;
        public readonly int Length;
        public readonly int FileIndex;

        public DefinitionResult(int start, int length, int fileIndex = -1) { Start = start; Length = length; FileIndex = fileIndex; }
    }

    internal struct ExternalFile
    {
        public readonly IReadOnlyList<Token> Tokens;
        public readonly string Text;

        public ExternalFile(IReadOnlyList<Token> tokens, string text) { Tokens = tokens; Text = text; }
    }

    internal static class GoToDefinitionEngine
    {
        public static DefinitionResult? FindDefinition(IReadOnlyList<Token> tokens, string text, int position)
        {
            return FindDefinition(tokens, text, position, null);
        }

        public static DefinitionResult? FindDefinition(IReadOnlyList<Token> tokens, string text, int position, IReadOnlyList<ExternalFile> externalFiles)
        {
            Token current = TokenLookup.AtPosition(tokens, position);
            if (current == null) return null;

            string target = null;
            string targetType = null;

            if (current.Type == ClassificationNames.AttributeValue)
            {
                Token attr = TokenLookup.PrecedingOfType(tokens, current.Start,
                    ClassificationNames.Attribute, ClassificationNames.AttributeEvent,
                    ClassificationNames.AttributeProperty, ClassificationNames.AttributeBoolean);

                if (attr != null)
                {
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
                                targetType = ClassificationNames.CssSelectorClass;
                                break;
                            }
                            at = idx + cls.Length;
                        }
                    }
                    else if (attrName == "id")
                    {
                        string value = text.Substring(current.Start, current.Length).Trim('"', '\'');
                        target = "#" + value;
                        targetType = ClassificationNames.CssSelectorId;
                    }
                }
            }
            else if (current.Type == ClassificationNames.CssSelectorClass)
            {
                target = text.Substring(current.Start, current.Length);
                targetType = ClassificationNames.CssSelectorClass;
            }
            else if (current.Type == ClassificationNames.CssSelectorId)
            {
                target = text.Substring(current.Start, current.Length);
                targetType = ClassificationNames.CssSelectorId;
            }

            if (target == null) return null;

            Token definition = TokenLookup.FindDefinition(tokens, target, current, targetType, text);
            if (definition != null)
                return new DefinitionResult(definition.Start, definition.Length);

            if (externalFiles != null)
            {
                for (int f = 0; f < externalFiles.Count; f++)
                {
                    var ext = externalFiles[f];
                    Token extDef = TokenLookup.FindDefinition(ext.Tokens, target, null, targetType, ext.Text);
                    if (extDef != null)
                        return new DefinitionResult(extDef.Start, extDef.Length, f);
                }
            }

            return null;
        }
    }
}
