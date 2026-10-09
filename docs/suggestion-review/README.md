# Revisão das sugestões: 800 exemplos, 100 por linguagem

Esta pasta responde a uma pergunta: **as 20 primeiras sugestões do plugin são razoáveis para o lugar onde o cursor está?** Para cada uma das 8 linguagens com completar (SQL, CSS, HTML, GraphQL, JSON, YAML, GLSL, WGSL) há 100 exemplos, com a entrada, a saída e o meu veredito. Quem quiser ler caso a caso abre o arquivo da linguagem; quem quiser só o resumo lê esta página.

| Linguagem | Exemplos | Quem lê |
|---|---|---|
| SQL | 100 | [sql.md](sql.md) |
| CSS | 100 | [css.md](css.md) |
| HTML (e SVG) | 100 | [html.md](html.md) |
| GraphQL | 100 | [graphql.md](graphql.md) |
| JSON | 100 | [json.md](json.md) |
| YAML | 100 | [yaml.md](yaml.md) |
| GLSL | 100 | [glsl.md](glsl.md) |
| WGSL | 100 | [wgsl.md](wgsl.md) |

Os dados crus (entrada e saída de todos os casos em JSON, e os vereditos em texto) estão em [dados/](dados/).

> **As páginas e este resumo descrevem o plugin antes das correções.** O que foi corrigido depois, e o que mudou nos mesmos 800 exemplos, está em [depois-das-correcoes.md](depois-das-correcoes.md).

## Como foi feito

1. **Os arquivos de entrada foram escritos à mão**, como um desenvolvedor escreve: código JavaScript com strings de cada linguagem dentro (consultas SQL em um serviço, CSS de componentes, templates HTML, manifestos YAML, shaders). São de 2 a 5 arquivos por linguagem e aparecem no começo de cada página. Não vieram do gerador dos experimentos, para a revisão não testar o motor contra as suposições dele mesmo.
2. **Cada exemplo pega uma palavra real do arquivo e finge que a pessoa está digitando ela.** Os 100 casos de cada linguagem são espalhados por todas as palavras das strings e seguem um ciclo: 1 letra digitada, 2 letras, 3 letras, **nenhuma letra (Ctrl+Espaço)**; e até um caso em cada dez tem **erro de digitação** (duas letras trocadas ou uma faltando). O resto da palavra é apagado e **o texto depois do cursor fica**, como em uma edição de verdade.
3. **O motor roda do jeito que o plugin roda**: a mesma montagem do plugin, com a palavra anterior, as palavras da mesma linguagem e a gramática ligadas (o esquema SQL e a ordem por contagem estão desligados, como no plugin). A lista tem no máximo 100 itens, como no editor; aqui mostro os 20 primeiros.
4. **Eu li as 800 saídas e dei um veredito a cada uma**:
   - ✅ **Bom**: a lista faz sentido para o lugar e a palavra procurada, se existe no arquivo, está bem colocada.
   - ⚠️ **Razoável, com ressalva**: serve, mas tem ruído, a palavra certa está longe ou falta algo que a linguagem tem.
   - ❌ **Ruim**: a palavra certa não pode aparecer, ou a lista não tem a ver com o lugar.

   O veredito é julgamento meu, não uma medida. Cada comentário diz o porquê.

Para refazer: `dotnet run -c Release --project NestLight.Experiments -- --review <pasta>` gera as entradas e saídas; `python3 scripts/review_report.py <pasta>/cases.json dados/vereditos <saida>` monta as páginas a partir dos vereditos.

## Placar

| Linguagem | ✅ Bom | ⚠️ Razoável | ❌ Ruim | Palavra existe no arquivo | Em 1º lugar | Nos 5 primeiros | Nos 20 primeiros |
|---|---|---|---|---|---|---|---|
| SQL | 69 | 26 | 5 | 90 | 56 | 76 | 83 |
| CSS | 58 | 30 | 12 | 68 | 35 | 48 | 55 |
| HTML | 72 | 28 | 0 | 67 | 36 | 51 | 58 |
| GraphQL | 61 | 39 | 0 | 59 | 35 | 46 | 54 |
| JSON | 73 | 27 | 0 | 19 | 14 | 18 | 18 |
| YAML | 74 | 23 | 3 | 43 | 32 | 36 | 39 |
| GLSL | 44 | 36 | 20 | 94 | 36 | 55 | 73 |
| WGSL | 50 | 30 | 20 | 89 | 37 | 52 | 65 |
| **Total** | **501** | **239** | **60** | | | | |

As quatro últimas colunas contam só os casos em que a palavra procurada **existe em outro lugar do arquivo ou é palavra-chave** (as outras não têm como ser sugeridas). Em JSON, por exemplo, quase toda chave aparece uma vez só.

Por situação, somando as 8 linguagens:

| Situação | Casos | ✅ | ⚠️ | ❌ |
|---|---|---|---|---|
| Digitando 1 a 3 letras | 573 | 430 (75%) | 128 | 15 |
| Erro de digitação | 55 | 47 (85%) | 8 | 0 |
| **Ctrl+Espaço, nada digitado** | 172 | **24 (14%)** | 103 | 45 |

Dos 110 pedidos explícitos em que a palavra existia no arquivo, **só 42 tiveram a palavra entre as 20 primeiras sugestões**.

## O que funciona bem

- **Digitar o começo de uma palavra** funciona na grande maioria dos casos (75% bons). Nomes que se repetem no arquivo (colunas, classes, chaves de YAML, nomes de variável de shader) aparecem em primeiro quando o prefixo já os separa.
- **Erro de digitação**: dos 46 casos em que a palavra existia no arquivo, a certa ficou em 1º lugar em 35 e entre as 5 primeiras em 45 (por exemplo [SQL-20](sql.md#sql-20) `VLAUE`→`VALUES`, [CSS-10](css.md#css-10) `bakgr`→`background`, [GraphQL-30](graphql.md#graphql-30) `suscr`→`subscription`).
- **Gramática de SQL, CSS e HTML**: o começo de um comando ([SQL-44](sql.md#sql-44)), `AS`, `INTO`, `BY`, a tabela depois de `FROM` ([SQL-13](sql.md#sql-13), [SQL-97](sql.md#sql-97)), os valores de uma propriedade ([CSS-5](css.md#css-5), [CSS-69](css.md#css-69)), os atributos de cada tag ([HTML-21](html.md#html-21), [HTML-48](html.md#html-48)) e os valores de `type` ([HTML-16](html.md#html-16)).
- **Quando não há o que sugerir, não sugere**: nome novo que não existe em outro lugar quase sempre dá lista vazia ou pequena.

## Os problemas, do mais importante ao menos

### 1. Ctrl+Espaço sem nada digitado quase nunca ajuda

Só 14% dos pedidos explícitos foram bons. Onde o motor não tem uma regra do lugar, a lista é o vocabulário da linguagem **em ordem alfabética, cortado nos 100 primeiros**. Resultado: as palavras do próprio arquivo, que estão bem ali ao lado, não aparecem, e as palavras-chave do fim do alfabeto também não.

- **GLSL e WGSL** (20 ruins cada): `uniform`, `out`, `vec3`, `float` ficam fora da lista ([GLSL-4](glsl.md#glsl-4), [GLSL-12](glsl.md#glsl-12), [GLSL-24](glsl.md#glsl-24)); variáveis declaradas a duas linhas dali não são alcançadas ([GLSL-28](glsl.md#glsl-28), [WGSL-92](wgsl.md#wgsl-92)).
- **CSS, nome de propriedade vazio**: `display` é a 71ª, `color` a 62ª, `font-weight` a 92ª, `position` nem aparece ([CSS-4](css.md#css-4), [CSS-12](css.md#css-12), [CSS-68](css.md#css-68), [CSS-56](css.md#css-56)).
- **GraphQL**: a mesma lista de palavras-chave em qualquer lugar, dentro de um conjunto de seleção, num tipo ou no nome de um argumento ([GraphQL-8](graphql.md#graphql-8), [GraphQL-44](graphql.md#graphql-44), [GraphQL-76](graphql.md#graphql-76)).
- **HTML depois de `<`**: tags em ordem alfabética, `h2` é a 45ª, `option` a 79ª ([HTML-4](html.md#html-4), [HTML-72](html.md#html-72)).

*Ideia:* com o prefixo vazio, ordenar por uso e proximidade (as palavras do arquivo primeiro, depois as mais comuns da linguagem) em vez de alfabeticamente.

### 2. Palavras de 1 e 2 letras nunca são sugeridas

O tamanho mínimo é 3 letras. Isso derruba `id`, a coluna mais comum de qualquer banco: em [SQL-5](sql.md#sql-5) e [SQL-86](sql.md#sql-86), `c.i` e `p.i` oferecem `if`, `in`, `index` (e em SQL-5 até `import`, do código JavaScript), mas não `id`. O mesmo com `ci`, `db`, `if`, `uv`, `in` em [YAML-39](yaml.md#yaml-39), [YAML-85](yaml.md#yaml-85), [YAML-65](yaml.md#yaml-65), [GLSL-30](glsl.md#glsl-30), [WGSL-23](wgsl.md#wgsl-23), [WGSL-46](wgsl.md#wgsl-46), [WGSL-87](wgsl.md#wgsl-87). Em GraphQL, `i` oferece o tipo `ID` quando a pessoa quer o campo `id` ([GraphQL-6](graphql.md#graphql-6), [GraphQL-22](graphql.md#graphql-22), [GraphQL-34](graphql.md#graphql-34)).

*Ideia:* aceitar palavras de 2 letras quando o lugar ajuda (depois de `.`, depois de uma palavra que já as precedeu).

### 3. Vocabulário que falta

Palavras que a linguagem tem e o plugin não conhece, então nunca as oferece:

- **SQL**: funções (`SUM`, `COUNT`, `COALESCE`, `NOW`, `RANK`, `date_trunc`) — [SQL-9](sql.md#sql-9), [SQL-53](sql.md#sql-53), [SQL-59](sql.md#sql-59), [SQL-98](sql.md#sql-98).
- **CSS**: pseudo-classes (`:hover`, `:focus`) — [CSS-20](css.md#css-20), [CSS-94](css.md#css-94); funções (`repeat`, `minmax`, `translate`, `scale`) — [CSS-24](css.md#css-24), [CSS-32](css.md#css-32), [CSS-34](css.md#css-34); palavras de `easing` e `ellipsis` — [CSS-17](css.md#css-17), [CSS-82](css.md#css-82).
- **GLSL**: `main`, `gl_FragColor`, `es` — [GLSL-1](glsl.md#glsl-1), [GLSL-15](glsl.md#glsl-15), [GLSL-98](glsl.md#glsl-98).
- **WGSL**: os atributos (`@builtin`, `@vertex`, `@fragment`, `@workgroup_size`) e os modos (`uniform`, `read_write`) — [WGSL-19](wgsl.md#wgsl-19), [WGSL-25](wgsl.md#wgsl-25), [WGSL-64](wgsl.md#wgsl-64), [WGSL-80](wgsl.md#wgsl-80).
- **HTML**: papéis ARIA (`region`) e cores de SVG (`currentColor`) — [HTML-63](html.md#html-63), [HTML-85](html.md#html-85).

### 4. Em GLSL e WGSL as palavras-chave vêm antes dos nomes do arquivo

Não há regra de posição nessas linguagens, e as palavras do arquivo só aparecem depois de todas as palavras-chave. O efeito: a variável que a pessoa acabou de declarar é a **última** da lista. Em [GLSL-21](glsl.md#glsl-21), `t` oferece 16 funções e tipos antes de `texel`; em [WGSL-5](wgsl.md#wgsl-5), `time` é a 30ª; em [WGSL-97](wgsl.md#wgsl-97), `input` é a 5ª depois de `i32`, `if`, `inverseSqrt`, `index`.

*Ideia:* nessas linguagens, uma palavra que já está no arquivo e começa com o que foi digitado deveria vir antes das palavras-chave.

### 5. Regras de posição erradas ou ausentes

- **SQL**: depois de `GROUP BY x` oferece `ASC`/`DESC` e `HAVING` é a 63ª ([SQL-16](sql.md#sql-16)); depois de `)` de uma função de janela, o `ORDER BY` de dentro do `OVER( )` é tomado como a cláusula ([SQL-62](sql.md#sql-62)); dentro de um literal entre aspas oferece `from`/`where` ([SQL-72](sql.md#sql-72)); numa definição de coluna não há regra ([SQL-32](sql.md#sql-32), [SQL-33](sql.md#sql-33), [SQL-36](sql.md#sql-36)); depois de `CASE` o `WHEN` é o 10º ([SQL-68](sql.md#sql-68)).
- **CSS**: depois de `.` aparecem tags HTML antes dos nomes de classe ([CSS-1](css.md#css-1), [CSS-25](css.md#css-25), [CSS-29](css.md#css-29)); dentro de `@keyframes` e `@media` o motor acha que está em uma declaração ([CSS-45](css.md#css-45), [CSS-48](css.md#css-48), [CSS-49](css.md#css-49), [CSS-52](css.md#css-52)); depois de `transition:` não aparecem nomes de propriedade ([CSS-16](css.md#css-16), [CSS-18](css.md#css-18)).
- **HTML**: numa tag de fechamento o elemento aberto deveria vir primeiro, mas vem a lista alfabética (`tr` é a 12ª em [HTML-29](html.md#html-29), `option` a 78ª em [HTML-76](html.md#html-76), `nav` a 73ª em [HTML-56](html.md#html-56)); atributos que a tag já tem são oferecidos de novo ([HTML-68](html.md#html-68), [HTML-84](html.md#html-84)); em URLs e textos livres aparecem tags ([HTML-81](html.md#html-81), [HTML-45](html.md#html-45), [HTML-94](html.md#html-94)).
- **GraphQL**: nenhuma regra, então campos, tipos e nomes recebem a mesma lista ([GraphQL-41](graphql.md#graphql-41), [GraphQL-61](graphql.md#graphql-61)).

### 6. `true`, `false`, `null` (e `yes`, `no`, `on`, `off`) antes de tudo, em nome de chave

Em JSON e YAML, num pedido explícito no nome de uma chave, essas palavras vêm primeiro mesmo sendo valores ([JSON-4](json.md#json-4), [JSON-16](json.md#json-16), [YAML-4](yaml.md#yaml-4), [YAML-16](yaml.md#yaml-16)). Dentro de uma string também ([JSON-12](json.md#json-12), [JSON-28](json.md#json-28), [GraphQL-100](graphql.md#graphql-100)).

### 7. Palavras "parecidas" que são só ruído

Com 3 letras e a palavra procurada não existindo em outro lugar, a segunda etapa (corrigir erro de digitação) inventa sugestões sem relação: [GraphQL-47](graphql.md#graphql-47) `fir`→`fragment`, [GraphQL-59](graphql.md#graphql-59) `bod`→`Boolean`, [JSON-7](json.md#json-7) `scr`→`src`, [JSON-83](json.md#json-83) `tes`→`tsc`, [GLSL-15](glsl.md#glsl-15) `mai`→`mat2`... Ao todo cerca de 15 casos. O experimento EA_23 só mediu esse ruído quando o prefixo está certo.

### 8. A palavra que já está depois do cursor aparece em 1º lugar

Em pedidos explícitos o motor trata a palavra que vem logo depois do cursor como "a que seguiu a palavra anterior" e a oferece primeiro ([SQL-4](sql.md#sql-4), [SQL-24](sql.md#sql-24), [SQL-56](sql.md#sql-56), [SQL-84](sql.md#sql-84), [SQL-92](sql.md#sql-92), [HTML-4](html.md#html-4)). Em parte é efeito do método (apaguei a palavra do meio), mas o mesmo acontece quando alguém digita antes de uma palavra que já existe.

## Limites desta análise

- **Um único revisor, e o veredito é opinião.** Outra pessoa poderia mudar uns 10% das classificações entre ✅ e ⚠️. Os ❌ são os casos em que a palavra certa não pode aparecer ou a lista não tem relação com o lugar; esses são mais objetivos.
- **Só 2 a 5 arquivos por linguagem**, todos JavaScript como hospedeiro (o motor é o mesmo nos outros hospedeiros). Arquivos grandes ou muito diferentes podem se comportar diferente.
- **A "palavra procurada" é a que estava no arquivo original.** Uma sugestão diferente pode ser igualmente boa (e o veredito leva isso em conta); por outro lado, quando a palavra não existe em outro lugar, nenhuma sugestão baseada no arquivo poderia acertá-la, e isso aparece como "—" na tabela.
- **Os pedidos explícitos são um caso mais difícil que o uso comum**: na prática quase todo mundo digita pelo menos uma letra. O placar de 14% vale para o Ctrl+Espaço no vazio, não para o uso diário.
- **Nada foi visto dentro do Visual Studio.** Isto é o que o motor devolve; a ordem na tela depende do código do editor, que não foi compilado nem rodado aqui.
