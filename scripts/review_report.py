#!/usr/bin/env python3
"""Turns the cases written by `dotnet run --project NestLight.Experiments -- --review <dir>` and the verdicts of a person into Markdown
pages, one per language, to be read.

usage: review_report.py CASES.json VERDICTS_DIR OUT_DIR

VERDICTS_DIR holds one file per language (sql.txt, css.txt...), one line per case: `id|G|comment`, with G (good), M (mixed) or B (bad).
Standard library only.
"""
import json
import os
import sys

NAMES = {"sql": "SQL", "css": "CSS", "html": "HTML", "graphql": "GraphQL", "json": "JSON", "yaml": "YAML", "glsl": "GLSL", "wgsl": "WGSL"}
ICON = {"G": "✅ Good", "M": "⚠️ Mixed", "B": "❌ Bad"}
MARK = {"keyword": "", "word": " [a]", "similar": " [~]"}


def load_verdicts(path):
    verdicts = {}
    with open(path, encoding="utf-8") as f:
        for line in f:
            line = line.rstrip("\n")
            if not line:
                continue
            number, kind, comment = line.split("|", 2)
            verdicts[int(number)] = (kind, comment)
    return verdicts


def top_block(items):
    cells = [f"{i + 1:>2} {t['text']}{MARK[t['kind']]}" for i, t in enumerate(items)]
    width = max([len(c) for c in cells[:10]] + [10]) + 3
    rows = []
    for i in range(10):
        left = cells[i] if i < len(cells) else ""
        right = cells[i + 10] if i + 10 < len(cells) else ""
        rows.append((left.ljust(width) + right).rstrip())
    return "\n".join(r for r in rows if r) or "(no suggestions)"


def meant(case):
    if case["rank"]:
        where = "first" if case["rank"] == 1 else f"number {case['rank']} of {case['total']}"
        return f"`{case['intended']}`: appears {where}" + ("" if case["rank"] <= 20 else " (outside the first 20 shown)")
    if not case["reachable"]:
        return f"`{case['intended']}`: the word is not written anywhere else in the file and is not in the vocabulary, so no suggestion could get it right"
    return f"`{case['intended']}`: **does not appear** in the list ({case['total']} items)"


def kind_text(case):
    c = case["category"]
    if c.startswith("typing: "):
        n = c.split(": ")[1].split(" ")[0]
        return f"typing ({n} letter{'' if n == '1' else 's'} typed)"
    if c.startswith("explicit"):
        return "explicit request (Ctrl+Space), nothing typed yet"
    if "swapped" in c:
        return "typo: two letters swapped"
    return "typo: a letter missing"


def short_kind(case):
    c = case["category"]
    if c.startswith("typing: "):
        n = c.split(": ")[1].split(" ")[0]
        return n + (" letter" if n == "1" else " letters")
    if c.startswith("explicit"):
        return "Ctrl+Space"
    return "typo: swapped" if "swapped" in c else "typo: missing"


def write_language(lang, data, verdicts, out_dir):
    name = NAMES[lang]
    cases = data["cases"]
    counts = {"G": 0, "M": 0, "B": 0}
    for c in cases:
        counts[verdicts[c["id"]][0]] += 1

    lines = [f"# {name}: 100 examples", ""]
    lines.append(f"Result: ✅ {counts['G']} good · ⚠️ {counts['M']} mixed · ❌ {counts['B']} bad.")
    lines.append("")
    lines.append("How to read: in each example, `▮` marks the caret. The list is what the plugin would show (the first 20). "
                 "`[a]` = a word that already exists in the file; `[~]` = a \"similar\" suggestion (corrects a typo); no mark = a keyword of the language. "
                 "The verdict and the comment are the reviewer's analysis. \"Place in the grammar\" is the internal name of the position rule the plugin applied "
                 "(`sql:table`, `css:value:display`...); `(no rule)` means the plugin has no rule for that place and uses only what was typed.")
    lines.append("")
    lines.append("## Index (to scan quickly)")
    lines.append("")
    lines.append("| # | Situation | Typed | Word sought | Position | Place in the grammar | Verdict |")
    lines.append("|---|---|---|---|---|---|---|")
    for c in cases:
        pos = str(c["rank"]) if c["rank"] else ("—" if not c["reachable"] else "out")
        kind = verdicts[c["id"]][0]
        short = {"G": "✅", "M": "⚠️", "B": "❌"}[kind]
        typed = f"`{c['typed']}`" if c["typed"] else "(nothing)"
        situation = short_kind(c)
        lines.append(f"| [{c['id']}](#{lang}-{c['id']}) | {situation} | {typed} | `{c['intended']}` | {pos} | `{c['place']}` | {short} |")
    lines.append("")
    lines.append("Position: place of the word sought in the list; `—` = the word is not written anywhere else in the file; `out` = it exists but is not in the list.")
    lines.append("")
    lines.append("## The files used as input")
    lines.append("")
    lines.append("Written by hand the way a developer would write them (JavaScript code with strings of the language). Nothing here was made by the generator of the experiments.")
    lines.append("")
    for i, doc in enumerate(data["documents"], 1):
        lines.append(f"### Document D{i}")
        lines.append("")
        lines.append("```js")
        lines.append(doc)
        lines.append("```")
        lines.append("")
    lines.append("## The examples")
    lines.append("")
    for c in cases:
        kind, comment = verdicts[c["id"]]
        lines.append(f"### {name}-{c['id']}")
        lines.append(f'<a id="{lang}-{c["id"]}"></a>')
        lines.append("")
        lines.append(f"**Situation:** {kind_text(c)} · **document** D{c['doc']}, line {c['line']} · **place in the grammar:** `{c['place']}`")
        lines.append("")
        lines.append("**Input** (the string the caret is in):")
        lines.append("")
        lines.append("```text")
        lines.append(c["input"])
        lines.append("```")
        lines.append("")
        lines.append(f"**The word the person meant to type:** {meant(c)}")
        lines.append("")
        lines.append("**Output** (the first 20 suggestions):")
        lines.append("")
        lines.append("```text")
        lines.append(top_block(c["top"]))
        lines.append("```")
        lines.append("")
        lines.append(f"**Verdict:** {ICON[kind]}. {comment}")
        lines.append("")
        lines.append("---")
        lines.append("")
    with open(os.path.join(out_dir, lang + ".md"), "w", encoding="utf-8") as f:
        f.write("\n".join(lines))
    return counts


def main():
    cases_path, verdicts_dir, out_dir = sys.argv[1:4]
    os.makedirs(out_dir, exist_ok=True)
    data = json.load(open(cases_path, encoding="utf-8"))
    stats = {}
    for lang in NAMES:
        verdicts = load_verdicts(os.path.join(verdicts_dir, lang + ".txt"))
        counts = write_language(lang, data[lang], verdicts, out_dir)
        cases = data[lang]["cases"]
        reachable = [c for c in cases if c["reachable"]]
        stats[lang] = {
            "counts": counts,
            "cases": len(cases),
            "reachable": len(reachable),
            "first": sum(1 for c in reachable if c["rank"] == 1),
            "top5": sum(1 for c in reachable if 1 <= c["rank"] <= 5),
            "top20": sum(1 for c in reachable if 1 <= c["rank"] <= 20),
            "absent": sum(1 for c in reachable if c["rank"] == 0),
        }
    json.dump(stats, open(os.path.join(out_dir, "stats.json"), "w"), indent=1)
    print(json.dumps(stats, indent=1))


if __name__ == "__main__":
    main()
