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
ICON = {"G": "✅ Bom", "M": "⚠️ Razoável, com ressalva", "B": "❌ Ruim"}
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
    return "\n".join(r for r in rows if r) or "(nenhuma sugestão)"


def meant(case):
    if case["rank"]:
        where = "em 1º lugar" if case["rank"] == 1 else f"em {case['rank']}º lugar de {case['total']}"
        return f"`{case['intended']}`: aparece {where}" + ("" if case["rank"] <= 20 else " (fora dos 20 primeiros mostrados)")
    if not case["reachable"]:
        return f"`{case['intended']}`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la"
    return f"`{case['intended']}`: **não aparece** na lista ({case['total']} itens)"


def kind_text(case):
    c = case["category"]
    if c.startswith("typing: "):
        n = c.split(": ")[1].split(" ")[0]
        return f"digitando ({n} letra{'' if n == '1' else 's'} já digitada{'' if n == '1' else 's'})"
    if c.startswith("explicit"):
        return "pedido explícito (Ctrl+Espaço), nada digitado ainda"
    if "swapped" in c:
        return "erro de digitação: duas letras trocadas"
    return "erro de digitação: uma letra faltando"


def short_kind(case):
    c = case["category"]
    if c.startswith("typing: "):
        n = c.split(": ")[1].split(" ")[0]
        return n + (" letra" if n == "1" else " letras")
    if c.startswith("explicit"):
        return "Ctrl+Espaço"
    return "erro: trocadas" if "swapped" in c else "erro: faltando"


def write_language(lang, data, verdicts, out_dir):
    name = NAMES[lang]
    cases = data["cases"]
    counts = {"G": 0, "M": 0, "B": 0}
    for c in cases:
        counts[verdicts[c["id"]][0]] += 1

    lines = [f"# {name}: 100 exemplos", ""]
    lines.append(f"Resultado: ✅ {counts['G']} bons · ⚠️ {counts['M']} razoáveis com ressalva · ❌ {counts['B']} ruins.")
    lines.append("")
    lines.append("Como ler: em cada exemplo, `▮` marca onde está o cursor. A lista é o que o plugin mostraria (as 20 primeiras). "
                 "`[a]` = palavra que já existe no arquivo; `[~]` = sugestão \"parecida\" (corrige erro de digitação); sem marca = palavra-chave da linguagem. "
                 "O veredito e o comentário são a minha análise. \"Lugar na gramática\" é o nome interno da regra de posição que o plugin aplicou "
                 "(`sql:table`, `css:value:display`...); `(no rule)` quer dizer que o plugin não tem regra para aquele lugar e usa só o que foi digitado.")
    lines.append("")
    lines.append("## Índice (para varrer rápido)")
    lines.append("")
    lines.append("| # | Situação | Digitado | Palavra procurada | Posição | Lugar na gramática | Veredito |")
    lines.append("|---|---|---|---|---|---|---|")
    for c in cases:
        pos = str(c["rank"]) if c["rank"] else ("—" if not c["reachable"] else "fora")
        kind = verdicts[c["id"]][0]
        short = {"G": "✅", "M": "⚠️", "B": "❌"}[kind]
        typed = f"`{c['typed']}`" if c["typed"] else "(nada)"
        situacao = short_kind(c)
        lines.append(f"| [{c['id']}](#{lang}-{c['id']}) | {situacao} | {typed} | `{c['intended']}` | {pos} | `{c['place']}` | {short} |")
    lines.append("")
    lines.append("Posição: lugar da palavra procurada na lista; `—` = a palavra não existe em outro lugar do arquivo; `fora` = existe mas não está na lista.")
    lines.append("")
    lines.append("## Os arquivos usados como entrada")
    lines.append("")
    lines.append("Escritos à mão como um desenvolvedor escreveria (código JavaScript com strings da linguagem). Nada foi gerado pelo gerador dos experimentos.")
    lines.append("")
    for i, doc in enumerate(data["documents"], 1):
        lines.append(f"### Documento D{i}")
        lines.append("")
        lines.append("```js")
        lines.append(doc)
        lines.append("```")
        lines.append("")
    lines.append("## Os exemplos")
    lines.append("")
    for c in cases:
        kind, comment = verdicts[c["id"]]
        lines.append(f"### {name}-{c['id']}")
        lines.append(f'<a id="{lang}-{c["id"]}"></a>')
        lines.append("")
        lines.append(f"**Situação:** {kind_text(c)} · **documento** D{c['doc']}, linha {c['line']} · **lugar na gramática:** `{c['place']}`")
        lines.append("")
        lines.append("**Entrada** (a string onde está o cursor):")
        lines.append("")
        lines.append("```text")
        lines.append(c["input"])
        lines.append("```")
        lines.append("")
        lines.append(f"**Palavra que a pessoa ia digitar:** {meant(c)}")
        lines.append("")
        lines.append("**Saída** (as 20 primeiras sugestões):")
        lines.append("")
        lines.append("```text")
        lines.append(top_block(c["top"]))
        lines.append("```")
        lines.append("")
        lines.append(f"**Veredito:** {ICON[kind]}. {comment}")
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
