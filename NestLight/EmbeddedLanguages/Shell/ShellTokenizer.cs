using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.EmbeddedLanguages
{
    /// <summary>Shell/Bash: keywords, builtins, variables, strings, numbers, operators, redirections and comments.</summary>
    internal sealed class ShellTokenizer : IEmbeddedLanguageTokenizer
    {
        private const char Mask = TextUtil.Mask;
        private static readonly string[] ShellIds = { "shell", "bash", "sh", "zsh" };

        internal static readonly HashSet<string> Keywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "if", "then", "else", "elif", "fi", "for", "while", "until", "do", "done",
            "case", "esac", "in", "function", "select", "time", "coproc",
            "return", "exit", "break", "continue", "declare", "local", "export",
            "readonly", "typeset", "unset", "shift", "trap", "eval", "exec",
            "source", "set", "shopt"
        };

        internal static readonly HashSet<string> Builtins = new HashSet<string>(StringComparer.Ordinal)
        {
            "echo", "printf", "read", "test", "true", "false", "cd", "pwd", "pushd", "popd",
            "alias", "unalias", "type", "which", "command", "builtin", "enable", "help",
            "let", "expr", "basename", "dirname", "realpath", "readlink",
            "getopts", "wait", "jobs", "bg", "fg", "kill", "disown", "suspend",
            "umask", "ulimit", "hash", "complete", "compgen", "compopt",
            "mapfile", "readarray", "caller"
        };

        internal static readonly HashSet<string> CommonCommands = new HashSet<string>(StringComparer.Ordinal)
        {
            "grep", "sed", "awk", "find", "xargs", "sort", "uniq", "wc", "head", "tail",
            "cat", "tee", "cut", "tr", "paste", "join", "diff", "comm", "less", "more",
            "cp", "mv", "rm", "mkdir", "rmdir", "touch", "chmod", "chown", "chgrp", "ln",
            "ls", "stat", "file", "tar", "gzip", "gunzip", "zip", "unzip", "bzip2",
            "curl", "wget", "ssh", "scp", "rsync", "git", "docker", "make", "cmake",
            "npm", "yarn", "pip", "python", "node", "java", "gcc", "go", "cargo", "dotnet",
            "apt", "yum", "dnf", "brew", "pacman", "snap",
            "systemctl", "journalctl", "service", "crontab",
            "ps", "top", "htop", "df", "du", "free", "mount", "umount",
            "ifconfig", "ip", "ping", "netstat", "ss", "nslookup", "dig", "host",
            "date", "cal", "sleep", "env", "printenv", "id", "whoami", "groups", "sudo", "su",
            "man", "info", "apropos", "tput", "clear", "reset"
        };

        public IReadOnlyList<string> Ids { get { return ShellIds; } }

        public void Tokenize(char[] m, int from, int to, TokenSink emit)
        {
            int i = from;
            bool lineStart = true;
            bool firstWord = true;
            while (i < to)
            {
                char c = m[i];
                if (c == '\n' || c == '\r') { lineStart = true; firstWord = true; i++; continue; }
                if (c == ' ' || c == '\t') { i++; continue; }

                if (c == '#' && (lineStart || i == from || IsSpaceOrNewline(m, i - 1)))
                {
                    int e = TextUtil.LineEnd(m, i, to);
                    emit(i, e, ClassificationNames.ShellComment);
                    i = e;
                    lineStart = false;
                    continue;
                }
                lineStart = false;

                if (c == '$')
                {
                    int end = Variable(m, i, to);
                    if (end > i + 1)
                    {
                        emit(i, end, ClassificationNames.ShellVariable);
                        i = end;
                        firstWord = false;
                        continue;
                    }
                }

                if (c == '\'' || c == '"')
                {
                    int end = QuotedEnd(m, i, to, c);
                    emit(i, end, ClassificationNames.ShellString);
                    i = end;
                    firstWord = false;
                    continue;
                }

                if (c == '<' || c == '>')
                {
                    int end = Redirection(m, i, to);
                    emit(i, end, ClassificationNames.ShellOperator);
                    i = end;
                    firstWord = false;
                    continue;
                }

                if (c == '|' || c == '&' || c == ';')
                {
                    int end = i + 1;
                    if (end < to && m[end] == c) end++;
                    emit(i, end, ClassificationNames.ShellOperator);
                    i = end;
                    firstWord = true;
                    continue;
                }

                if (char.IsDigit(c) && (i == from || !TextUtil.IsWordChar(m[i - 1])))
                {
                    int end = i + 1;
                    while (end < to && char.IsDigit(m[end])) end++;
                    if (end < to && (m[end] == '>' || m[end] == '<'))
                    {
                        int redir = Redirection(m, end, to);
                        emit(i, redir, ClassificationNames.ShellOperator);
                        i = redir;
                        firstWord = false;
                        continue;
                    }
                    if (end == to || !TextUtil.IsWordChar(m[end]))
                    {
                        emit(i, end, ClassificationNames.ShellNumber);
                        i = end;
                        firstWord = false;
                        continue;
                    }
                }

                if (TextUtil.IsWordStart(c) || c == '-' || c == '.' || c == '/' || c == '~')
                {
                    int end = i;
                    if (c == '-')
                    {
                        end++;
                        if (end < to && m[end] == '-') end++;
                        while (end < to && (TextUtil.IsWordChar(m[end]) || m[end] == '-')) end++;
                        if (end > i + 1)
                        {
                            emit(i, end, ClassificationNames.ShellFlag);
                            firstWord = false;
                            i = end;
                            continue;
                        }
                        end = i;
                    }

                    while (end < to && (TextUtil.IsWordChar(m[end]) || m[end] == '-' || m[end] == '.' || m[end] == '/' || m[end] == '~')) end++;
                    if (end > i)
                    {
                        string word = TextUtil.Substring(m, i, end);
                        if (Keywords.Contains(word))
                        {
                            emit(i, end, ClassificationNames.ShellKeyword);
                            firstWord = true;
                        }
                        else if (firstWord && Builtins.Contains(word))
                        {
                            emit(i, end, ClassificationNames.ShellBuiltin);
                        }
                        else if (firstWord && CommonCommands.Contains(word))
                        {
                            emit(i, end, ClassificationNames.ShellCommand);
                        }
                        firstWord = false;
                        i = end;
                        continue;
                    }
                }

                if ("(){}!".IndexOf(c) >= 0)
                {
                    emit(i, i + 1, ClassificationNames.ShellOperator);
                    if (c == '(' || c == '{') firstWord = true;
                }
                i++;
                firstWord = false;
            }
        }

        private static int Variable(char[] m, int i, int to)
        {
            int p = i + 1;
            if (p >= to) return i + 1;
            if (m[p] == '{')
            {
                p++;
                while (p < to && m[p] != '}') p++;
                return p < to ? p + 1 : to;
            }
            if (m[p] == '(' || m[p] == '[')
            {
                char close = m[p] == '(' ? ')' : ']';
                int depth = 1;
                p++;
                while (p < to && depth > 0)
                {
                    if (m[p] == m[i + 1]) depth++;
                    else if (m[p] == close) depth--;
                    p++;
                }
                return p;
            }
            if ("?$!#@*-_0123456789".IndexOf(m[p]) >= 0) return p + 1;
            while (p < to && (char.IsLetterOrDigit(m[p]) || m[p] == '_')) p++;
            return p;
        }

        private static int QuotedEnd(char[] m, int i, int to, char quote)
        {
            int p = i + 1;
            while (p < to)
            {
                if (quote == '"' && m[p] == '\\') { p += 2; continue; }
                if (m[p] == quote) return p + 1;
                p++;
            }
            return to;
        }

        private static int Redirection(char[] m, int i, int to)
        {
            int p = i;
            if (m[p] == '<')
            {
                p++;
                if (p < to && m[p] == '<') { p++; if (p < to && m[p] == '-') p++; }
                else if (p < to && m[p] == '&') p++;
            }
            else if (m[p] == '>')
            {
                p++;
                if (p < to && (m[p] == '>' || m[p] == '&' || m[p] == '|')) p++;
            }
            return p;
        }

        private static bool IsSpaceOrNewline(char[] m, int i)
        {
            return m[i] == ' ' || m[i] == '\t' || m[i] == '\n' || m[i] == '\r';
        }
    }
}
