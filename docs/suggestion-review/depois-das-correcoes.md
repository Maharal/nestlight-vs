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

## Segunda rodada: a ordem quando nenhuma regra decide

GraphQL, GLSL e WGSL não mudaram na primeira rodada: o problema delas não era uma regra errada, era a **ordem quando o prefixo é vazio ou curto** (lista alfabética de palavras-chave antes dos nomes do arquivo). Para medir isso gerei um corpus de **500 trechos para cada linguagem** (código no estilo de aplicações reais, escrito por geradores) e rodei os experimentos E34 e E35 (ver `docs/experiments.md`). Resultado adotado: onde nenhuma regra decide o lugar, as palavras do arquivo vêm antes das palavras-chave, e as palavras-chave vêm na ordem de quanto código as usa, com as 12 mais usadas ainda na frente.

Nos mesmos 800 exemplos (529 em que a palavra existe no arquivo):

| | Original | Depois das regras | Depois da ordem |
|---|---|---|---|
| Em 1º lugar | 281 | 303 | **339** |
| Nos 5 primeiros | 382 | 395 | **444** |
| Nos 20 primeiros | 445 | 453 | **474** |
| Ctrl+Espaço, nos 20 primeiros (de 110) | 42 | 51 | **71** |

| Linguagem | Em 1º: original → depois da ordem | Nos 5 primeiros: original → depois da ordem |
|---|---|---|
| GLSL | 36 → **56** | 55 → **78** |
| WGSL | 37 → **48** | 52 → **70** |
| GraphQL | 35 → **39** | 46 → **51** |
| SQL | 56 → **66** | 76 → **83** |
| HTML | 36 → 46 | 51 → **60** |
| CSS, JSON, YAML | sem mudança relevante | sem mudança relevante |

**Cuidado com este número:** a ordem das palavras-chave por uso foi aprendida de código **gerado por mim**, não de projetos reais. Os 800 exemplos foram escritos à mão (outra fonte), por isso servem de conferência, mas quem escreveu tudo sou eu. O próximo passo natural é o plugin aprender essa ordem com os arquivos do próprio usuário.

## Terceira rodada: palavras de 2 letras e o ruído da correção de erro

- **Palavras de 2 letras (E36 e E38).** Passaram a ser oferecidas, depois de todas as palavras mais longas. Os casos que não tinham solução na revisão agora têm: `id` depois de `c.` e `p.` ([SQL-5](sql.md#sql-5), [SQL-86](sql.md#sql-86)), `ci` e `db` em YAML, `uv` em GLSL e WGSL, `in` e `id` em WGSL. Nos 800 exemplos a palavra fica em 1º lugar em 342 casos (339 antes) e entre os 5 primeiros em 456 (444 antes). O primeiro critério do E36 era impossível de atingir (a linha de base já era 85%, e eu pedi +15), então o experimento foi fechado e refeito como E38 com um critério que dá para cumprir; os números são os mesmos.
- **Ruído da correção de erro com 3 letras (E37).** Não mudou. Com 3 letras digitadas a correção acerta 9 de cada 10 erros e mostra algo sem relação em 3 de cada 10 palavras novas; nenhuma das travas que testei separa os dois sem perder quase toda a correção. Fica como está até haver dados de uso real.

## Quarta rodada: o vocabulário que faltava

Os 15 casos em que a palavra procurada nem existia no vocabulário agora têm resposta, na primeira ou segunda posição na maioria: `es` ([GLSL-1](glsl.md#glsl-1)), `main` ([GLSL-15](glsl.md#glsl-15), [GLSL-54](glsl.md#glsl-54)), `gl_FragColor` ([GLSL-98](glsl.md#glsl-98)), `builtin` ([WGSL-19](wgsl.md#wgsl-19), [WGSL-82](wgsl.md#wgsl-82)), `vertex` ([WGSL-25](wgsl.md#wgsl-25)), `fragment` ([WGSL-44](wgsl.md#wgsl-44)), `uniform` ([WGSL-8](wgsl.md#wgsl-8)), `read` e `read_write` ([WGSL-64](wgsl.md#wgsl-64), [WGSL-69](wgsl.md#wgsl-69)), `global_invocation_id` ([WGSL-83](wgsl.md#wgsl-83)), `region` ([HTML-63](html.md#html-63)) e `currentColor` ([HTML-85](html.md#html-85)). Os números de "palavra em 1º lugar" nos 529 casos alcançáveis não mudam porque esses 15 já estavam fora da conta (a palavra não existia em lugar nenhum).

Como foi feito: palavras que a linguagem tem mas o realce não colore (`main`, `gl_...`) ficam numa lista só do completar; os lugares novos (depois de `#` e de `@`, dentro de `@builtin(` e de `var<`) são regras de posição; e os valores de atributos HTML e de propriedades CSS são tabelas. Ver "The missing vocabulary" em `docs/experiments.md`.

## O que ainda não foi resolvido

- O ruído da correção de erro em prefixos de 3 letras (E37).
- Funções de SQL (`count`, `coalesce`...) já são oferecidas, mas depois das colunas; ainda não há as funções de GLSL além das embutidas que o realce conhece.
