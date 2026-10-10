# EA06: análise do crescimento superlinear

EA06 mediu que 8x mais strings marcadas multiplicam o tempo por 13–18x (linear seria 8x, quadrático 64x). Este documento registra onde o custo superlinear está e o que fazer.

## O que foi medido

| Host       | Crescimento (8x strings) |
|------------|--------------------------|
| JavaScript | 18.36x                   |
| Python     | 14.14x                   |
| CSharp     | 13.56x                   |
| Cpp        | 8.96x                    |

O critério era menos de 10x. Cpp quase atende; os outros três não.

## Onde está o custo

### 1. O sort final dos tokens — `HighlightEngine.cs:35`

```csharp
tokens.Sort((a, b) => a.Start.CompareTo(b.Start));
```

O pipeline acumula todos os tokens de todas as strings numa lista e ordena no final. Com S strings produzindo ~T tokens cada, o sort custa O(S·T · log(S·T)). Quando S cresce 8x, o custo cresce como 8 · (log(8·N) / log(N)) para N = S·T — o que dá ~10–11x para os tamanhos do benchmark (3.000 a 27.000 tokens). Esse é o fator dominante.

É a única operação do engine cujo custo depende do total de tokens e não do trabalho por string.

### 2. O sort dos scanners de JS, C# e Python

Os scanners de JavaScript (`JavaScriptHostScanner.cs:30`), C# (`CSharpHostScanner.cs:29`) e Python (`PythonHostScanner.cs:35`) ordenam a lista de strings encontradas antes de devolvê-la:

```csharp
result.Sort((a, b) => a.OuterStart.CompareTo(b.OuterStart));
```

O scanner de C++ não faz isso — as raw strings não têm interpolações, então o scan linear já as encontra em ordem. Esse segundo sort O(S · log S) compõe com o sort dos tokens e explica por que Cpp fica em 8.96x (quase linear) enquanto os outros ficam em 13–18x.

### 3. Pressão de GC no `Decode` — `HighlightEngine.cs:60–95`

Cada string aloca 4 arrays (dois `List<>` internos + dois `ToArray()`). Com muitas strings, as alocações acumulam e disparam coletas gen2 — o relatório mostra 0 coletas gen2 até 400k chars e 3–5 a partir de 800k. Cada coleta gen2 percorre o heap inteiro, e o heap também cresceu. Isso explica por que o último dobro (400k → 800k) pula de ~2.2x para 3.87x em JavaScript.

## O que NÃO é quadrático

Todas as operações por string são lineares ou O(T · log H):

- **`Decode`**: percorre a string uma vez, avança ponteiros de interpolação e escape sem voltar.
- **`AddClipped`**: busca binária O(log H) por token para achar a primeira interpolação relevante. EA07 confirmou que escala bem (14.22x para 10x interpolações, critério atendido).
- **`EmitInterpolations`**: busca binária `FirstStartingAtOrAfter` O(log S) por interpolação, e o loop interno sai assim que a string aninhada passa do fim da expressão.
- **Scanners**: percorrem o texto uma vez, O(N).
- **Tokenizers**: cada um é O(n) sobre o texto decodificado.

## Correção proposta

O sort é desnecessário se os tokens forem emitidos em ordem de documento. As condições já existem:

1. As strings vêm em ordem de `OuterStart` (os scanners garantem isso).
2. Dentro de cada string, `EmitInterpolations` e `EmitLanguage` produzem tokens em ordem de posição no documento.
3. Strings consecutivas não se sobrepõem (exceto nesting tratado por `EmitInterpolations`).

Portanto, concatenar os tokens de cada string em sequência já produz uma lista ordenada — basta remover o `tokens.Sort()` da linha 35. Se os scanners de JS, C# e Python puderem manter a ordem durante o scan (em vez de ordenar no final), o segundo sort também desaparece.

O custo passa de O(N · log N) para O(N) no número total de tokens, e o crescimento para 8x strings fica em ~8x.

A pressão de GC do `Decode` (item 3) é um problema separado e menor; pode ser tratado depois, reutilizando buffers entre strings.

## Testes para proteger o invariante

Depois de remover o sort, a correção depende de um invariante: os tokens saem em ordem de documento sem precisar de ordenação. Se alguém mudar um scanner ou o engine e quebrar essa ordem, nada hoje impede a regressão — `HasOverlap` em `RobustnessTests` verifica que tokens não se sobrepõem, mas não verifica explicitamente que estão em ordem crescente de `Start`.

Três testes protegem a correção:

### 1. Os tokens de `Highlight` saem em ordem de `Start` (`HighlightEngineTests`)

Depois de cada chamada a `Highlight`, verificar que `tokens[i].Start >= tokens[i-1].Start` para todo `i`. É o invariante central: se este teste falha, o classificador entrega spans fora de ordem ao editor.

Colocar no `HighlightEngineTests` usando o `FakeScanner` com várias strings em posições diferentes, com e sem interpolações, com e sem nesting. Também adicionar a mesma asserção nos testes de robustez (`RobustnessTests`), que já percorrem todos os prefixos de código real.

### 2. Os scanners devolvem strings em ordem de `OuterStart` (`AllHostsRobustnessTests`)

Para cada host, sobre o mesmo corpus de robustez (todos os prefixos e sufixos), verificar que `strings[i].OuterStart >= strings[i-1].OuterStart`. É a pré-condição do engine: se o scanner devolver strings fora de ordem, os tokens saem fora de ordem.

### 3. Dentro de cada string, os tokens do tokenizer saem em ordem (`EmbeddedLanguages/*Tests`)

Para cada tokenizer, sobre entradas de tamanhos variados, verificar que os tokens emitidos via `TokenSink` têm `a` (start) crescente. É a pré-condição de `EmitLanguage`: se o tokenizer emitir fora de ordem, os tokens daquela string saem fora de ordem.

### Onde colocar

Os testes 1 e 2 entram nos testes existentes de robustez, como asserções adicionais nos loops que já existem — não precisam de novos arquivos. O teste 3 entra nos testes de cada tokenizer, como um `[Theory]` parametrizado por tamanho de entrada.
