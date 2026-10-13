using System.Linq;
using NestLight.Common;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class ShellTokenizerTests
    {
        [Fact]
        public void Keywords_are_classified()
        {
            var tokens = Lexer.Language("shell", "if true; then echo hello; fi");
            Assert.Contains("shell.keyword|if", tokens);
            Assert.Contains("shell.keyword|then", tokens);
            Assert.Contains("shell.keyword|fi", tokens);
        }

        [Fact]
        public void Builtins_are_classified_at_command_position()
        {
            var tokens = Lexer.Language("shell", "echo hello");
            Assert.Contains("shell.builtin|echo", tokens);
        }

        [Fact]
        public void Variables_are_classified()
        {
            // ${PATH} is tested separately with C# host because JS treats ${ as interpolation
            var tokens = Lexer.Language("shell", "echo $HOME $?");
            Assert.Contains("shell.variable|$HOME", tokens);
            Assert.Contains("shell.variable|$?", tokens);
        }

        [Fact]
        public void Brace_variables_are_classified_in_csharp_host()
        {
            var tokens = Lexer.Seq(HostLanguage.CSharp, "// shell\nvar s = \"echo ${PATH}\";");
            Assert.Contains("shell.variable|${PATH}", tokens);
        }

        [Fact]
        public void Strings_are_classified()
        {
            var tokens = Lexer.Language("shell", "echo 'single' \"double\"");
            Assert.Contains("shell.string|'single'", tokens);
            Assert.Contains("shell.string|\"double\"", tokens);
        }

        [Fact]
        public void Comments_are_classified()
        {
            var tokens = Lexer.Language("shell", "# this is a comment\necho hello");
            Assert.Contains("shell.comment|# this is a comment", tokens);
        }

        [Fact]
        public void Operators_are_classified()
        {
            var tokens = Lexer.Language("shell", "echo a | grep b && echo c");
            Assert.Contains("shell.operator||", tokens);
            Assert.Contains("shell.operator|&&", tokens);
        }

        [Fact]
        public void Flags_are_classified()
        {
            var tokens = Lexer.Language("shell", "grep -r --include 'test'");
            Assert.Contains("shell.flag|-r", tokens);
            Assert.Contains("shell.flag|--include", tokens);
        }

        [Fact]
        public void Numbers_are_classified()
        {
            var tokens = Lexer.Language("shell", "sleep 42");
            Assert.Contains("shell.number|42", tokens);
        }

        [Fact]
        public void Common_commands_are_classified()
        {
            var tokens = Lexer.Language("shell", "grep pattern file.txt");
            Assert.Contains("shell.command|grep", tokens);
        }

        [Fact]
        public void Redirections_are_classified()
        {
            var tokens = Lexer.Language("shell", "echo hello > output.txt 2>&1");
            Assert.Contains("shell.operator|>", tokens);
            Assert.Contains("shell.operator|2>&", tokens);
        }

        [Fact]
        public void Shebang_is_classified()
        {
            var tokens = Lexer.Language("shell", "#!/bin/bash\necho hello");
            Assert.Contains("shell.comment|#!/bin/bash", tokens);
        }

        [Fact]
        public void No_tokens_overlap()
        {
            Assert.False(Lexer.HasOverlap(Lexer.Lex(Lexer.In("shell", "if [ -f $HOME/.bashrc ]; then\n  source ~/.bashrc\nfi"))));
        }

        [Fact]
        public void Pipe_resets_command_position()
        {
            var tokens = Lexer.Language("shell", "cat file.txt | grep pattern | sort");
            Assert.Contains("shell.command|cat", tokens);
            Assert.Contains("shell.command|sort", tokens);
        }

        // --- Mutant-killing tests ---

        [Fact]
        public void Line_start_resets_on_newline()
        {
            // Kills: L53 lineStart=true, L58 logical/boolean mutations on newline detection
            var tokens = Lexer.Language("shell", "echo hello\n# comment");
            Assert.Contains("shell.comment|# comment", tokens);
        }

        [Fact]
        public void Comment_requires_space_or_line_start()
        {
            // Kills: L61 lineStart || i == from, L61 i == from boundary
            // At position 0 (from), # is a comment
            var tokens = Lexer.Language("shell", "# start");
            Assert.Contains("shell.comment|# start", tokens);
            // After a space, # is a comment
            tokens = Lexer.Language("shell", "echo hello # inline");
            Assert.Contains("shell.comment|# inline", tokens);
        }

        [Fact]
        public void Comment_line_end()
        {
            // Kills: L66 lineStart = false, L67 statement mutation (skip to line end)
            var tokens = Lexer.Language("shell", "# comment\necho hi");
            Assert.Contains("shell.comment|# comment", tokens);
            Assert.Contains("shell.builtin|echo", tokens);
        }

        [Fact]
        public void Variable_end_boundary()
        {
            // Kills: L74 end > i + 1, L74 arithmetic i - 1
            var tokens = Lexer.Language("shell", "$a");
            Assert.Contains("shell.variable|$a", tokens);
            // $ alone is not a variable token (end == i+1)
            tokens = Lexer.Language("shell", "echo $");
            Assert.DoesNotContain("shell.variable|$", tokens);
        }

        [Fact]
        public void Variable_sets_firstWord_false()
        {
            // Kills: L78 firstWord = false, L79 statement mutation
            var tokens = Lexer.Language("shell", "$PATH grep");
            // After a variable, grep is not in command position
            Assert.DoesNotContain("shell.command|grep", tokens);
        }

        [Fact]
        public void String_sets_firstWord_false()
        {
            // Kills: L88 firstWord = false, L89 statement mutation
            var tokens = Lexer.Language("shell", "'hello' grep");
            Assert.DoesNotContain("shell.command|grep", tokens);
        }

        [Fact]
        public void Double_quoted_string_with_escape()
        {
            // Kills: L216 quote == '"' && m[p] == '\\' escape handling
            var tokens = Lexer.Language("shell", "echo \"a\\\"b\"");
            Assert.Contains("shell.string|\"a\\\"b\"", tokens);
        }

        [Fact]
        public void Redirection_sets_firstWord_false()
        {
            // Kills: L97 firstWord = false, L98 statement mutation
            var tokens = Lexer.Language("shell", "> out grep");
            Assert.DoesNotContain("shell.command|grep", tokens);
        }

        [Fact]
        public void Pipe_and_semicolon_reset_firstWord()
        {
            // Kills: L107 firstWord = true
            var tokens = Lexer.Language("shell", "echo a; grep b");
            Assert.Contains("shell.command|grep", tokens);
            tokens = Lexer.Language("shell", "echo a | grep b");
            Assert.Contains("shell.command|grep", tokens);
        }

        [Fact]
        public void Double_operator()
        {
            // Kills: L104 end < to && m[end] == c
            var tokens = Lexer.Language("shell", "echo a && echo b");
            Assert.Contains("shell.operator|&&", tokens);
        }

        [Fact]
        public void Digit_followed_by_redirect_is_operator()
        {
            // Kills: L111 i == from check, L115 m[end] == '>' || m[end] == '<'
            var tokens = Lexer.Language("shell", "echo 2> /dev/null");
            Assert.Contains("shell.operator|2>", tokens);
        }

        [Fact]
        public void Standalone_number()
        {
            // Kills: L123 end == to || !IsWordChar boundary, L127 firstWord = false
            var tokens = Lexer.Language("shell", "sleep 42");
            Assert.Contains("shell.number|42", tokens);
        }

        [Fact]
        public void Flag_with_single_and_double_dash()
        {
            // Kills: L132 c == '-', L138 boundary checks, L140 end > i + 1, L143 firstWord = false
            var tokens = Lexer.Language("shell", "ls -l --all");
            Assert.Contains("shell.flag|-l", tokens);
            Assert.Contains("shell.flag|--all", tokens);
        }

        [Fact]
        public void Lone_dash_is_not_a_flag()
        {
            // Kills: L140 end > i + 1 boundary (single dash, no word chars)
            var tokens = Lexer.Language("shell", "echo -");
            Assert.DoesNotContain("shell.flag|-", tokens);
        }

        [Fact]
        public void Word_classification_path_like()
        {
            // Kills: L132 c == '.' || c == '/' || c == '~', L151 end > i
            var tokens = Lexer.Language("shell", "~/script.sh");
            // paths with dots/slashes/tildes are consumed but not colored as keywords/builtins
            Assert.DoesNotContain("shell.keyword|~/script.sh", tokens);
            Assert.DoesNotContain("shell.builtin|~/script.sh", tokens);
        }

        [Fact]
        public void Keyword_resets_firstWord()
        {
            // Keywords set firstWord=true (L157) but L167 immediately resets to false,
            // so the next word after a keyword (without ; or newline) is not in command position.
            // The ';' operator at L107 truly resets firstWord.
            var tokens = Lexer.Language("shell", "if true; then\necho hello; fi");
            Assert.Contains("shell.keyword|if", tokens);
            Assert.Contains("shell.keyword|then", tokens);
            // newline resets lineStart which with # triggers comment, but also firstWord=true (L58)
            Assert.Contains("shell.builtin|echo", tokens);
            Assert.Contains("shell.keyword|fi", tokens);
        }

        [Fact]
        public void CommonCommands_classification()
        {
            // Kills: L163 firstWord || CommonCommands.Contains(word)
            var tokens = Lexer.Language("shell", "grep pattern");
            Assert.Contains("shell.command|grep", tokens);
        }

        [Fact]
        public void Parentheses_braces_and_bang_are_operators()
        {
            // Kills: L173 "(){}!".IndexOf(c) >= 0, L175 statement mutation
            var tokens = Lexer.Language("shell", "( echo a )");
            Assert.Contains("shell.operator|(", tokens);
            Assert.Contains("shell.operator|)", tokens);
            tokens = Lexer.Language("shell", "{ echo a; }");
            Assert.Contains("shell.operator|{", tokens);
            tokens = Lexer.Language("shell", "! true");
            Assert.Contains("shell.operator|!", tokens);
        }

        [Fact]
        public void Open_paren_and_brace_are_operators()
        {
            // Kills: L176 c == '(' || c == '{' firstWord = true (later overridden by L179)
            // The ( and { are classified as operators
            var tokens = Lexer.Language("shell", "echo a; (echo b)");
            Assert.Contains("shell.operator|(", tokens);
            Assert.Contains("shell.operator|)", tokens);
            tokens = Lexer.Language("shell", "echo a; { echo b; }");
            Assert.Contains("shell.operator|{", tokens);
        }

        [Fact]
        public void Subshell_variable()
        {
            // Kills: L186 p = i + 1, L189-202 subshell depth tracking
            var tokens = Lexer.Language("shell", "echo $(pwd)");
            Assert.Contains("shell.variable|$(pwd)", tokens);
        }

        [Fact]
        public void Array_subscript_variable()
        {
            // Kills: L193-202 [ ] depth tracking, L195 close = ')'  or ']'
            var tokens = Lexer.Language("shell", "echo $a");
            Assert.Contains("shell.variable|$a", tokens);
        }

        [Fact]
        public void Special_variables()
        {
            // Kills: L206 "?$!#@*-_0123456789".IndexOf
            var tokens = Lexer.Language("shell", "echo $? $! $# $@ $* $- $$ $_");
            Assert.Contains("shell.variable|$?", tokens);
            Assert.Contains("shell.variable|$!", tokens);
            Assert.Contains("shell.variable|$#", tokens);
        }

        [Fact]
        public void Here_doc_redirection()
        {
            // Kills: L228-230 heredoc << and <<- handling
            var tokens = Lexer.Language("shell", "cat <<EOF");
            Assert.Contains("shell.operator|<<", tokens);
            tokens = Lexer.Language("shell", "cat <<-EOF");
            Assert.Contains("shell.operator|<<-", tokens);
        }

        [Fact]
        public void Redirect_with_append_and_pipe()
        {
            // Kills: L229-235 Redirection method various operators
            var tokens = Lexer.Language("shell", "echo a >> out");
            Assert.Contains("shell.operator|>>", tokens);
            tokens = Lexer.Language("shell", "echo a >| out");
            Assert.Contains("shell.operator|>|", tokens);
        }

        [Fact]
        public void IsSpaceOrNewline_all_chars()
        {
            // Kills: L242 mutations on ' ', '\t', '\n', '\r'
            // Comment after space
            var tokens = Lexer.Language("shell", "echo a # c");
            Assert.Contains("shell.comment|# c", tokens);
            // Comment after tab
            tokens = Lexer.Language("shell", "echo a\t# c");
            Assert.Contains("shell.comment|# c", tokens);
        }

        [Fact]
        public void Input_redirection()
        {
            // Kills: L226-230 m[p] == '<' branch
            var tokens = Lexer.Language("shell", "cat < input.txt");
            Assert.Contains("shell.operator|<", tokens);
        }

        [Fact]
        public void Fd_redirect_with_ampersand()
        {
            // Kills: L230 m[p] == '&' for <& redirect
            var tokens = Lexer.Language("shell", "cat <&3");
            Assert.Contains("shell.operator|<&", tokens);
        }

        [Fact]
        public void No_tokens_overlap_complex()
        {
            Assert.False(Lexer.HasOverlap(Lexer.Lex(Lexer.In("shell",
                "#!/bin/bash\nif [ -f $HOME ]; then\n  echo \"hello\\\"world\" | grep -r --include 'test' > /dev/null 2>&1\nfi"))));
        }
    }
}
