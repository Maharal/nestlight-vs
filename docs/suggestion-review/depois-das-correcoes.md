# Depois das correções

As páginas de cada linguagem ([sql.md](sql.md), [css.md](css.md)...) mostram o plugin **antes** das correções. Depois delas, refiz os mesmos 800 exemplos (as mesmas entradas) e comparei a posição da palavra procurada. Os vereditos ✅ ⚠️ ❌ das páginas **não** foram refeitos: valem para o estado anterior.

## O que foi corrigido

| Problema da revisão | Correção |
|---|---|
| SQL: depois de `GROUP BY x` oferecia `ASC`/`DESC` | oferece `HAVING`, `ORDER`, `LIMIT`, `UNION`, `OFFSET` |
| SQL: o `ORDER BY` dentro de `OVER( )` virava a cláusula do SELECT | cada parêntese tem a sua cláusula; ao fechar, volta a de fora |
| SQL: dentro de uma string entre aspas ou de um comentário oferecia `from`/`where` | dentro de literal e de comentário só as palavras do arquivo |
| SQL: definição de coluna sem regra | nome da coluna (sem palavras-chave), depois tipos (`int`, `text`, `varchar`...), depois restrições (`not`, `primary`, `references`...) |
| SQL: `ON CONFLICT`, `ALTER TABLE x ...`, `CASE ...` | `conflict`, `add`/`drop`/`alter`/`rename`, `when` |
| SQL: faltavam funções | `count`, `sum`, `coalesce`, `now`, `rank`, `date_trunc`... depois das colunas |
| CSS: depois de `.` e `#` aparecem tags | só nomes que existem no arquivo |
| CSS: `:` num seletor não tinha pseudo-classes | `:hover`, `:focus`, `::before`... |
| CSS: `@media`, `@keyframes` tratados como bloco de declarações | blocos de at-rule têm seletores; `@keyframes` tem `from`/`to`; `@media (` oferece `width`, `max-width`... |
| CSS: `transition:`/`animation:`/`transform:` sem os valores certos | propriedades e easing, `infinite`/`alternate`..., funções `translate`, `repeat`, `linear-gradient`... |
| HTML: `</` oferecia as tags em ordem alfabética | o elemento que está aberto primeiro (ignora tags void, comentários e `>` dentro de aspas) |
| HTML: atributos que a tag já tem eram oferecidos de novo | só os que faltam (também considera os que estão depois do cursor) |
| HTML: tags em valores de atributo, URLs e texto | nenhuma tag nesses lugares |
| JSON/YAML: `true`/`false`/`null`/`yes`/`no` em nome de chave | chave e string: só palavras do arquivo; valor: continuam as palavras-chave |
| A palavra que já está depois do cursor aparecia em 1º lugar | não conta mais como "palavra que seguiu a anterior" |

## Resultado nos mesmos 800 exemplos

Entre os casos em que a palavra procurada existe no arquivo (529):

| | Antes | Depois |
|---|---|---|
| Em 1º lugar | 281 | **303** |
| Nos 5 primeiros | 382 | **395** |
| Nos 20 primeiros | 445 | **453** |

| Situação | Casos | Em 1º antes → depois | Nos 20 primeiros antes → depois |
|---|---|---|---|
| Ctrl+Espaço | 110 | 12 → **20** | 42 → **51** |
| Digitando 1 a 3 letras | 373 | 234 → **248** | 357 → 356 |
| Erro de digitação | 46 | 35 → 35 | 46 → 46 |

| Linguagem | Em 1º antes → depois | Nos 20 primeiros antes → depois |
|---|---|---|
| SQL | 56 → **65** | 83 → **87** |
| HTML | 36 → **47** | 58 → **60** |
| CSS | 35 → 36 | 55 → 56 |
| YAML | 32 → 33 | 39 → 39 |
| JSON | 14 → 14 | 18 → 19 |
| GraphQL, GLSL, WGSL | sem mudança | sem mudança |

62 casos melhoraram, 735 ficaram iguais (muitos nem têm palavra alcançável) e 3 pioraram:

- [CSS-93](css.md#css-93) `input[type="t▮"]`: antes `text` aparecia como tag, por coincidência; dentro das aspas de um seletor de atributo agora só vêm palavras do arquivo.
- [HTML-45](html.md#html-45) `aria-label="M▮"`: antes `main` aparecia como tag; num valor de texto livre não deve aparecer tag.
- [YAML-40](yaml.md#yaml-40) `on` no nome de uma chave: `on` (do GitHub Actions) deixou de ser oferecida como palavra-chave num nome de chave. É o preço de tirar `yes`/`no`/`off` desses lugares.

## O que as correções não resolvem

GraphQL, GLSL e WGSL não mudaram: o problema delas não é uma regra errada, é a **ordem quando o prefixo é vazio ou curto** (lista alfabética de palavras-chave antes dos nomes do arquivo) e o **mínimo de 3 letras**. Isso precisa de um experimento, que é o próximo passo.
