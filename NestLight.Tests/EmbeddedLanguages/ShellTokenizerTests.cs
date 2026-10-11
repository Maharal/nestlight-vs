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
    }
}
