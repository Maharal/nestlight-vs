using NestLight.Common;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>
    /// General properties that hold for any input. The analyzer runs on every keystroke,
    /// on code that is almost always incomplete, so it must be tolerant and fast.
    /// </summary>
    public class RobustnessTests
    {
        private const string Sample = @"
import { LitElement, html, css } from 'lit';
const sheet = /* css */ `:host { display: block; }`;
class MyEl extends LitElement {
  static styles = css`
    /* c */
    :host { display: block; --gap: 8px; color: var(--text, #333); }
    .card > h1::before, #main:not(.x) {
      margin: 0 auto !important;
      padding: ${gap}px 1.5rem;
      background: url(img/a.png) no-repeat;
      font-family: ""Open Sans"", sans-serif;
      &:hover { color: rgb(0 0 0 / 50%); }
    }
    @media (min-width: 600px) { .a { width: calc(100% - 2rem); } }
    ${shared}
  `;
  render() {
    return html`
      <!-- c -->
      <style>p { color: red; --x: 1; }</style>
      <h1 class=""t ${this.cls}"" style=""color: blue; margin: ${m}px"">Hi ${this.name}</h1>
      <button ?disabled=${!this.ok} .value=${this.v} @click=${this._go}>Go</button>
      <ul>${this.items.map(i => html`<li style=""top: 0"">${i}</li>`)}</ul>
    `;
  }
}";

        private const string FlatSample =
            "html`<div style=\"a: ${x}px\" class=\"${c}\" ${spread}>${text}</div>`; css`a { b: ${v}; ${m} c: d }`";

        [Fact]
        public void Every_prefix_and_suffix_is_analyzed_without_errors_or_overlaps()
        {
            for (int n = 0; n <= Sample.Length; n++)
            {
                foreach (string s in new[] { Sample.Substring(0, n), Sample.Substring(n) })
                {
                    var toks = Lexer.Lex(s);
                    foreach (var t in toks)
                        Assert.True(t.Start >= 0 && t.Length > 0 && t.End <= s.Length, "token outside the text, n=" + n);
                    Assert.False(Lexer.HasOverlap(toks), "overlapping tokens, n=" + n);
                }
            }
        }

        [Fact]
        public void Tokens_are_always_inside_a_template()
        {
            var templates = Lexer.Templates(Sample);
            foreach (var t in Lexer.Lex(Sample))
                Assert.True(templates.Any(tp => t.Start >= tp.Start && t.End <= tp.End),
                    "token outside a template: " + t);
        }

        [Fact]
        public void Html_and_css_tokens_never_cover_expression_text()
        {
            var templates = Lexer.Templates(FlatSample);
            var exprs = templates.SelectMany(tp => tp.Interpolations).ToList();
            Assert.Equal(6, exprs.Count);

            foreach (var t in Lexer.Lex(FlatSample))
            {
                if (t.Type == ClassificationNames.Expression || t.Type == ClassificationNames.ExprDelimiter) continue;
                foreach (var e in exprs)
                    Assert.False(t.Start < e.End && e.Start < t.End, "token overlaps an expression: " + t);
            }
        }

        [Fact]
        public void Analysis_is_deterministic()
        {
            Assert.Equal(Lexer.Seq(Sample), Lexer.Seq(Sample));
        }

        [Fact]
        public void Large_file_is_analyzed_quickly()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < 300; i++) sb.Append(Sample).Append("\nconst x").Append(i).Append(" = 1;\n");

            var sw = Stopwatch.StartNew();
            var toks = Lexer.Lex(sb.ToString());
            sw.Stop();

            Assert.True(toks.Count > 10000);
            Assert.False(Lexer.HasOverlap(toks));
            Assert.True(sw.ElapsedMilliseconds < 5000, "slow analysis: " + sw.ElapsedMilliseconds + " ms");
        }

        [Fact]
        public void Classification_names_are_unique_and_namespaced()
        {
            var names = typeof(ClassificationNames)
                .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.IsLiteral)
                .Select(f => (string)f.GetRawConstantValue())
                .ToList();

            Assert.True(names.Count >= 20);
            Assert.Equal(names.Count, names.Distinct().Count());
            Assert.True(names.All(n => n.StartsWith("template.")));
        }
    }
}
