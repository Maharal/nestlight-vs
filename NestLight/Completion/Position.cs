using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NestLight.Completion
{
    /// <summary>
    /// What the grammar of the language says about the place of the caret: the words that belong there come first, the words that
    /// do not belong come last or are not offered. The places are told apart with a few characters of look-behind, not a parser.
    /// </summary>
    internal sealed class Position
    {
        /// <summary>A name for the place, for tests and reports (<c>sql:table</c>, <c>css:value:display</c>).</summary>
        public readonly string Name;
        /// <summary>Offered first, in this order, even when the vocabulary of the language does not have them (the tags in a selector).</summary>
        public readonly IReadOnlyList<string> Expected;
        /// <summary>Offered after the words of the document when <see cref="WordsFirst"/> is set, right after <see cref="Expected"/> otherwise (the functions of SQL).</summary>
        public readonly IReadOnlyList<string> Secondary;
        /// <summary>The words of the document (a table, a column, a class) go before the rest of the keywords.</summary>
        public readonly bool WordsFirst;
        /// <summary>The keywords that do not belong here go after the words of the document; null: none.</summary>
        public readonly Func<string, bool> Unlikely;
        /// <summary>The keywords of the language do not belong here at all (a class name, a JSON key, a string): only the lists above and the words of the document are offered.</summary>
        public readonly bool OnlyWords;
        /// <summary>The list in <see cref="Expected"/> is long and alphabetical (the properties of CSS): the most used come first.</summary>
        public readonly bool PriorOrder;
        /// <summary>What the place asks of the schema of the document (SQL only): a table, a column of the tables in the statement, a member of a qualifier.</summary>
        public readonly PlaceRole Role;
        /// <summary>For <see cref="PlaceRole.Member"/>, the word before the dot (an alias or a table).</summary>
        public readonly string Qualifier;

        public Position(string name, IEnumerable<string> expected = null, bool wordsFirst = false, Func<string, bool> unlikely = null,
            PlaceRole role = PlaceRole.None, string qualifier = null, bool onlyWords = false, IEnumerable<string> secondary = null, bool priorOrder = false)
        {
            Name = name;
            Expected = expected == null ? new string[0] : expected as IReadOnlyList<string> ?? expected.ToList(); // a list that is already one is kept (the order by use is cached by it)
            Secondary = secondary == null ? new string[0] : secondary as IReadOnlyList<string> ?? secondary.ToList();
            WordsFirst = wordsFirst;
            Unlikely = unlikely;
            Role = role;
            Qualifier = qualifier;
            OnlyWords = onlyWords;
            PriorOrder = priorOrder;
        }
    }

    internal enum PlaceRole { None, Table, Column, Member }
}
