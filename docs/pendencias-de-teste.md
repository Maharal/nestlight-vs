# Pendências de teste

Relatório da revisão dos testes (cobertura, mutação, experimentos). Medido em 2026-10-10 em `net8.0` no Linux; o CI roda em `net48` no Windows.

## 1. Onde estamos

| Medida | Resultado |
|---|---|
| Testes unitários | 1425 passam, 1 pulado (SQL em várias linhas, ver item A1) |
| Cobertura de linhas | 99,8% (só 8 linhas sem cobertura, ver seção 3) |
| Cobertura de branches | 96,7% (4521 de 4674) |
| Cobertura de métodos | 100% |
| Teste de mutação | **não concluído**: foi interrompido por pedido, para rodar na máquina do autor |

A cobertura é muito alta. O que ela não diz é se os testes *verificam* o resultado: isso é o que o teste de mutação mede (seção 2).

## 2. Teste de mutação (a rodar)

O Stryker gerou 9745 mutantes: 1585 foram descartados (724 não compilam, 710 em blocos já cobertos, 117 sem nenhum teste que os alcance) e 8160 seriam testados. Rodar levaria de 1 h a 1 h 30 com 4 núcleos.

Como rodar (está em [`scripts/mutation`](../scripts/mutation/README.md)):

```
dotnet tool install -g dotnet-stryker
cd scripts/mutation/Tests
dotnet test
dotnet stryker
```

O `stryker-config.json` já usa `concurrency: 16`. O relatório HTML sai em `StrykerOutput/<hora>/reports/mutation-report.html`.

**O que fazer com o resultado.** Cada mutante *Survived* é um buraco: o teste não falha quando o código muda. Ordem sugerida: primeiro `Completion/CompletionEngine`, `Completion/Languages/SqlSchema`, `Completion/Languages/CssCompletion` e `Highlighting/IncrementalHostScan` (têm mais lógica e mais branches); depois os tokenizers. Mutantes *Timeout* contam como mortos. Muitos sobreviventes em constantes de vocabulário (listas de palavras-chave) são esperados e baratos de ignorar.

## 3. Cobertura: o que falta

Sobraram 3 trechos sem cobertura. Nenhum é um caso de teste esquecido:

- `Completion/CompletionEngine.cs:399`: o último desempate de `Compare` (mesmo comprimento, mesma distância e mesma posição, só a caixa diferente). Provavelmente inalcançável: duas palavras diferentes não ocupam a mesma posição. Remover o desempate ou documentar.
- `Completion/Languages/HtmlCompletion.cs:76-79`: o ramo `nameEnd == site.Start` depois do nome da tag. O trecho de cima (`nameStart == site.Start`) já trata o mesmo caso, então o de baixo parece código morto. Conferir e remover.
- `Detection/DetectionOptions.cs:84-87`: o `catch` de `Load`/`Save` quando o arquivo está bloqueado. Há um teste novo que o exercita com `FileShare.None`; na primeira rodada em Windows confirmar que ele passa lá também.

## 4. Problemas encontrados pelos experimentos (a corrigir)

Achados pelo EA35 e EA36 (ver [experiments.md](experiments.md)), com snippets aleatórios por seed.

| # | Problema | Dado medido | Onde |
|---|---|---|---|
| A1 | SQL em várias linhas quase não é detectado: o detector procura ` from ` com espaço dos dois lados, e `FROM` no começo da linha não casa | 51% de acerto (contra 100% em uma linha) | `SqlDetector.Score` |
| A2 | CSS que começa com comentário `/* */` nunca é detectado: `/` não está em `CanStartWith` e a regra não pula comentário | 71% de acerto | `CssDetector` |
| A3 | GraphQL que começa com comentário `#` é confundido com CSS (a regra de CSS aceita `#` no início) | 59 de 200 snippets detectados como outra linguagem | `GraphQlDetector`, `CssDetector` |
| A4 | Prosa que começa com verbo SQL é detectada como SQL ("Select an option from the list"); `{name} has joined {room}` é detectado como GraphQL | 6 de 38 strings comuns com falso positivo (meta: ≤ 1%) | `SqlDetector`, `GraphQlDetector` |
| A5 | HTML muda de resposta a cada tag que fecha enquanto se digita | até 23 mudanças por snippet; 0% dos snippets com ≤ 2 mudanças | `HtmlDetector` |
| A6 | SQL, CSS e GraphQL às vezes nunca são reconhecidos ao digitar (decorre de A1 a A3) | 56%, 28% e 30% | idem |

Combinado com o autor: corrigir A1 a A4 depois do teste de mutação. A5 pede uma decisão de produto (é aceitável o realce de HTML mudar a cada `>`?) e A6 deve sumir sozinho com as correções.

Depois de corrigir: reativar o teste `A_sql_statement_written_on_several_lines_is_recognized` (hoje `Skip`), reexecutar `--only EA35,EA36` e conferir que A4 ficou em ≤ 1%.

## 5. Experimentos sugeridos e ainda não feitos

Automáticos (rodam sem Visual Studio):

1. **Sensibilidade ao tamanho da string**: abaixo de quantos caracteres a detecção passa a errar (hoje `MinLength = 8`)?
2. **Detecção com edição incremental**: o `IncrementalHostScan` tem teste de propriedade com edições aleatórias, mas sem a detecção automática ligada.
3. **Memória em sessão longa**: milhares de snapshots seguidos sem vazamento no cache (o EA05 só mede uma chamada).
4. **Corpus de código real**: rodar a detecção sobre arquivos de projetos de código aberto, para trocar "código escrito por nós" por "código de verdade" (o EA35 avisa desse limite).

Manuais (artefatos para ler):

1. **EM02, já criado**: `dotnet run -c Release --project NestLight.Experiments -- --gallery` e abrir `index.html`. Ler com atenção: o `.` entre alias e coluna em SQL (`e.status`) fica sem cor; `>` de seletor CSS também.
2. **EM03, código real e sujo**: arquivos reais, ler os falsos positivos de detecção e de completion.
3. **EM04, completion por posição**: para cada linguagem, a lista do que é sugerido em cada ponto da gramática, para um revisor dizer se faz sentido.

## 6. Testes manuais dentro do Visual Studio (exigem o IDE)

Nada disso é automatizável sem Visual Studio. Ficam fora do `docs/experiments.md`, como a regra do projeto manda.

1. A extensão instala e carrega no Visual Studio 2022 e no 2026 sem aviso no log de atividade.
2. O realce aparece nos quatro hosts (C#, JavaScript/TypeScript, Python, C/C++) e **muda de tema** (claro/escuro) com as cores certas.
3. O realce acompanha a digitação em arquivo grande (alguns milhares de linhas) sem travar a interface.
4. Colar um bloco grande e desfazer: o realce volta ao estado anterior.
5. A lista de completion abre dentro de uma string marcada, não abre no código do host nem dentro de `${...}`, e o ícone de cada sugestão aparece.
6. A página de opções da detecção automática salva e lê o arquivo; mudar a opção reflete no editor aberto, sem reiniciar.
7. Arquivo com string não fechada, aspas mal formadas e mistura de tabs/espaços: nada de exceção no log.
8. Abrir dois arquivos do mesmo host em abas diferentes: o realce de um não vaza para o outro.

## 7. Ambiente

- O projeto de testes alvo é `net48`, que só roda no Windows (como no CI). Aqui foi rodado em `net8.0` sobrescrevendo `-p:TargetFramework=net8.0` depois de `dotnet restore` com o mesmo parâmetro.
- Foi adicionado `coverlet.collector` ao projeto de testes. Para medir cobertura do código linkado é preciso `IncludeTestAssembly=true` em um `.runsettings`, porque o código da extensão é compilado dentro do assembly de testes.
