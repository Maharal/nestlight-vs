using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>
    /// Properties that hold for any input and any host: the analyzer runs on every keystroke, on code that is
    /// almost always incomplete, so it must never throw, never produce a bad token and stay fast.
    /// </summary>
    public class AllHostsRobustnessTests
    {
        private const string JavaScript = @"
const re = /[`'""]+\/`/gi; const half = total / 2 / n;
const q2 = s.replace(/`/g, '').split(/'/);
const q = sql`SELECT * FROM t WHERE id = ${id} -- c`;
const j = /* json */ `{""a"": [1, true, ${x}]}`;
const g = gql`query Q($id: ID!) { user(id: $id) @skip(if: false) { name } }`;
// language=yaml
const y = `a: 1
b: [x, ""y""] # c`;
const x = xml`<a b=""c""><![CDATA[x]]><!-- c --></a>`;
const m = md`# T ${t}
*a* **b** \`c\` [l](u)
- i`;
const r = regex`^(?<y>\\d{4})-[a-z]+$`;
const s = glsl`#version 300 es
uniform vec3 c; void main() { gl_FragColor = vec4(c, 1.0); }`;
const w = wgsl`@vertex fn main() -> @builtin(position) vec4f { return vec4f(1.0); }`;
const h = html`<ul>${items.map(i => html`<li class=""${c}"" style=""a:${b}"">${i}</li>`)}</ul>`;
const c = css`:host { --a: ${v}; color: var(--a, #fff); &:hover { margin: 0 auto !important; } }`;
const k = css`a:not(.b, #c) > li:nth-child(2n+1)::before, input[type=""text"" i]:is(:focus, :hover) ~ .x { color: red }
@media screen and (min-width: 600px) and (hover: hover) { .a { margin: 0 } } @supports (display: grid) and not (display: ${d}) { .b { top: 1e3ms } }
@keyframes s { from { opacity: 0 } 50% { opacity: .5 } } @container card (width > 400px) { .c { d: e } } @layer a, b; @import url(x.css) layer(base);`;
";

        private const string CSharp = @"
// html
var a = $""<ul>{ string.Join("""", xs.Select(x => /* html */ $""<li class='{x.C}'>{x.Name:N2}</li>"")) }</ul>"";
// sql
var b = @""SELECT ""a"" FROM t WHERE x = '1'"";
// css
var c = $$""""""
    a { color: {{color}}; }
    @media (x) { b { c: d } }
    """""";
// yaml
var d = ""a: 1\nb: \""x\"""";
var e = '""'; var f = '\''; // language=html
var g = $@""<b>{{literal}} {value}</b>"";
";

        private const string Python = @"
# language=html
a = f'''<ul>{ ''.join(
    # html
    f'<li class=""{c!r}"">{i:>{w}}</li>' for i in xs) }</ul>'''
# sql
b = rb'select \d from t'
# regex
c = r""^(?P<n>\w+)\s{2,}$""
# json
d = ""{\""a\"": [1, 2]}""
e = t'<b>{{x}} {y}</b>'  # html
";

        private const string Cpp = @"
// language=html
auto a = R""x(<ul class=""a""><li>1</li></ul>)x"";
/* sql */ const char* b = u8R""(select 1 from t where x = '1')"";
// glsl
auto c = LR""(#version 300 es
void main() { gl_Position = vec4(1.0); })"";
int n = 1'000'000; char q = '""';
";

        public static IEnumerable<object[]> Samples()
        {
            yield return new object[] { HostLanguage.JavaScript, JavaScript };
            yield return new object[] { HostLanguage.CSharp, CSharp };
            yield return new object[] { HostLanguage.Python, Python };
            yield return new object[] { HostLanguage.Cpp, Cpp };
        }

        private static void AssertWellFormed(HostLanguage host, string text, string context)
        {
            List<Tok> toks = Lexer.Lex(host, text);
            for (int i = 0; i < toks.Count; i++)
            {
                Tok t = toks[i];
                Assert.True(t.Start >= 0 && t.Length > 0 && t.End <= text.Length, "token outside the text: " + t + " " + context);
                Assert.True(i == 0 || toks[i - 1].End <= t.Start, "overlapping or unsorted tokens at " + t + " " + context);
            }
        }

        [Theory]
        [MemberData(nameof(Samples))]
        public void The_samples_produce_tokens_of_many_languages(HostLanguage host, string sample)
        {
            var types = Lexer.Lex(host, sample).Select(t => t.Type.Split('.')[1]).Distinct().ToList();
            Assert.True(types.Count >= 3, "only " + string.Join(",", types));
            AssertWellFormed(host, sample, "(whole sample)");
        }

        [Theory]
        [MemberData(nameof(Samples))]
        public void Every_prefix_and_suffix_is_analyzed_without_errors_or_bad_tokens(HostLanguage host, string sample)
        {
            for (int n = 0; n <= sample.Length; n++)
            {
                AssertWellFormed(host, sample.Substring(0, n), "(prefix " + n + ")");
                AssertWellFormed(host, sample.Substring(n), "(suffix " + n + ")");
            }
        }

        [Theory]
        [MemberData(nameof(Samples))]
        public void Deleting_any_single_character_never_breaks_the_analysis(HostLanguage host, string sample)
        {
            for (int n = 0; n < sample.Length; n++)
                AssertWellFormed(host, sample.Remove(n, 1), "(without char " + n + ")");
        }

        [Theory]
        [MemberData(nameof(Samples))]
        public void Tokens_are_always_inside_an_embedded_string(HostLanguage host, string sample)
        {
            var strings = Lexer.Scan(host, sample);
            foreach (var t in Lexer.Lex(host, sample))
            {
                Assert.True(strings.Any(s => t.Start >= s.Start && t.End <= s.End), "token outside a string: " + t);
            }
        }

        [Theory]
        [MemberData(nameof(Samples))]
        public void Language_tokens_never_cover_the_interpolations_of_their_own_string(HostLanguage host, string sample)
        {
            var strings = Lexer.Scan(host, sample);

            foreach (var t in Lexer.Lex(host, sample))
            {
                if (t.Type.StartsWith("template.expression")) continue;
                // the owner is the innermost string around the token (a nested string sits inside the holes of its parent)
                var owner = strings.Where(x => t.Start >= x.Start && t.End <= x.End).OrderBy(x => x.Start).Last();
                foreach (var h in owner.Interpolations)
                    Assert.False(t.Start < h.End && h.Start < t.End, "token over an interpolation: " + t);
            }
        }

        [Theory]
        [MemberData(nameof(Samples))]
        public void Analysis_is_deterministic(HostLanguage host, string sample)
        {
            Assert.Equal(Lexer.Seq(host, sample), Lexer.Seq(host, sample));
        }

        [Theory]
        [MemberData(nameof(Samples))]
        public void Large_files_are_analyzed_quickly(HostLanguage host, string sample)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < 300; i++) sb.Append(sample).Append("\n\n");
            string text = sb.ToString();

            var sw = Stopwatch.StartNew();
            var toks = Lexer.Lex(host, text);
            sw.Stop();

            Assert.True(toks.Count > 1000);
            Assert.True(sw.ElapsedMilliseconds < 5000, host + " analysis took " + sw.ElapsedMilliseconds + " ms");
        }

        [Theory]
        [InlineData(HostLanguage.JavaScript, "html`")]
        [InlineData(HostLanguage.CSharp, "// html\n$\"{")]
        [InlineData(HostLanguage.Python, "# html\nf'{")]
        [InlineData(HostLanguage.Cpp, "// html\nR\"(")]
        public void Pathological_nesting_does_not_blow_the_stack(HostLanguage host, string opener)
        {
            string text = string.Concat(Enumerable.Repeat(opener, 400));
            AssertWellFormed(host, text, "(deep nesting)");
        }

        [Theory]
        [InlineData(HostLanguage.JavaScript)]
        [InlineData(HostLanguage.CSharp)]
        [InlineData(HostLanguage.Python)]
        [InlineData(HostLanguage.Cpp)]
        public void Random_noise_made_of_the_characters_that_matter_never_breaks_the_analysis(HostLanguage host)
        {
            string[] pieces =
            {
                "`", "\"", "'", "$", "@", "{", "}", "(", ")", "[", "]", "/", "*", "#", "\\", "<", ">", "=", ":", "-", "R\"", "f'",
                "\"\"\"", "'''", "{{", "}}", "${", "//", "/*", "*/", "\n", " ", "html", "css", "sql", "json", "yaml", "regex", "language=",
                "// html\n", "# sql\n", "/* css */", "<a b=\"", "<!--", "-->", "select ", "{a:", "x", "1", "!", "?", ",", ";", "&", "|"
            };
            var random = new Random(20260615);
            for (int round = 0; round < 400; round++)
            {
                var sb = new StringBuilder();
                int length = random.Next(1, 60);
                for (int k = 0; k < length; k++) sb.Append(pieces[random.Next(pieces.Length)]);
                AssertWellFormed(host, sb.ToString(), "(round " + round + ": " + sb.ToString().Replace("\n", "\\n") + ")");
            }
        }

        [Theory]
        [InlineData("html")]
        [InlineData("css")]
        [InlineData("sql")]
        [InlineData("json")]
        [InlineData("graphql")]
        [InlineData("xml")]
        [InlineData("markdown")]
        [InlineData("yaml")]
        [InlineData("regex")]
        [InlineData("glsl")]
        [InlineData("wgsl")]
        public void Random_noise_never_breaks_any_language_tokenizer(string id)
        {
            string[] pieces =
            {
                "<", ">", "</", "/>", "<!--", "-->", "<![CDATA[", "]]>", "<?", "?>", "\"", "'", "{", "}", "(", ")", "[", "]", ":", ";",
                ",", "#", "*", "_", "`", "```", "~~~", "-", "- ", "\\", "\n", "  ", "\n  ", "@", "$", "!", "?", "|", "&", "=", ".", "...",
                "a", "b1", "select ", "query ", "type ", "fragment ", "on ", "true", "null", "1", "1.5e", "0x", "&x", "*x", "!!t",
                "/*", "*/", "//", "--", "\u0001", "\u0001\u0001"
            };
            var tokenizer = Pipeline.Languages.Find(id);
            var random = new Random(7);
            for (int round = 0; round < 600; round++)
            {
                var sb = new StringBuilder();
                int length = random.Next(1, 40);
                for (int k = 0; k < length; k++) sb.Append(pieces[random.Next(pieces.Length)]);
                char[] text = sb.ToString().ToCharArray();

                int last = -1;
                tokenizer.Tokenize(text, 0, text.Length, (a, b, type) =>
                {
                    Assert.True(a >= 0 && b <= text.Length && a < b, id + ": bad token [" + a + "," + b + ") in " + sb);
                    Assert.True(a >= last, id + ": unsorted or overlapping token [" + a + "," + b + ") in " + sb);
                    last = b;
                });
            }
        }
    }
}
