# JSON: 100 exemplos

Resultado: ✅ 73 bons · ⚠️ 27 razoáveis com ressalva · ❌ 0 ruins.

Como ler: em cada exemplo, `▮` marca onde está o cursor. A lista é o que o plugin mostraria (as 20 primeiras). `[a]` = palavra que já existe no arquivo; `[~]` = sugestão "parecida" (corrige erro de digitação); sem marca = palavra-chave da linguagem. O veredito e o comentário são a minha análise. "Lugar na gramática" é o nome interno da regra de posição que o plugin aplicou (`sql:table`, `css:value:display`...); `(no rule)` quer dizer que o plugin não tem regra para aquele lugar e usa só o que foi digitado.

## Índice (para varrer rápido)

| # | Situação | Digitado | Palavra procurada | Posição | Lugar na gramática | Veredito |
|---|---|---|---|---|---|---|
| [1](#json-1) | 1 letra | `n` | `name` | — | `(no rule)` | ⚠️ |
| [2](#json-2) | 2 letras | `ne` | `nestlight` | — | `(no rule)` | ✅ |
| [3](#json-3) | 3 letras | `dem` | `demo` | — | `(no rule)` | ⚠️ |
| [4](#json-4) | Ctrl+Espaço | (nada) | `version` | — | `(no rule)` | ⚠️ |
| [5](#json-5) | 1 letra | `p` | `private` | — | `(no rule)` | ⚠️ |
| [6](#json-6) | 2 letras | `tr` | `true` | 1 | `(no rule)` | ✅ |
| [7](#json-7) | 3 letras | `scr` | `scripts` | — | `(no rule)` | ⚠️ |
| [8](#json-8) | Ctrl+Espaço | (nada) | `build` | — | `(no rule)` | ⚠️ |
| [9](#json-9) | 1 letra | `t` | `tsc` | — | `(no rule)` | ✅ |
| [10](#json-10) | 2 letras | `te` | `test` | — | `(no rule)` | ✅ |
| [11](#json-11) | 3 letras | `jes` | `jest` | 1 | `(no rule)` | ✅ |
| [12](#json-12) | Ctrl+Espaço | (nada) | `coverage` | — | `(no rule)` | ⚠️ |
| [13](#json-13) | 1 letra | `l` | `lint` | — | `(no rule)` | ✅ |
| [14](#json-14) | 2 letras | `es` | `eslint` | — | `(no rule)` | ✅ |
| [15](#json-15) | 2 letras | `sr` | `src` | — | `(no rule)` | ✅ |
| [16](#json-16) | Ctrl+Espaço | (nada) | `dependencies` | — | `(no rule)` | ⚠️ |
| [17](#json-17) | 1 letra | `e` | `express` | — | `(no rule)` | ✅ |
| [18](#json-18) | 2 letras | `lo` | `lodash` | — | `(no rule)` | ✅ |
| [19](#json-19) | 3 letras | `dev` | `devDependencies` | — | `(no rule)` | ✅ |
| [20](#json-20) | erro: trocadas | `tpyesc` | `typescript` | — | `(no rule)` | ✅ |
| [21](#json-21) | 1 letra | `j` | `jest` | 1 | `(no rule)` | ✅ |
| [22](#json-22) | 2 letras | `st` | `status` | — | `(no rule)` | ✅ |
| [23](#json-23) | 1 letra | `o` | `ok` | — | `(no rule)` | ✅ |
| [24](#json-24) | Ctrl+Espaço | (nada) | `data` | — | `(no rule)` | ⚠️ |
| [25](#json-25) | 1 letra | `u` | `userId` | — | `(no rule)` | ✅ |
| [26](#json-26) | 2 letras | `di` | `displayName` | — | `(no rule)` | ✅ |
| [27](#json-27) | 2 letras | `Ad` | `Ada` | — | `(no rule)` | ✅ |
| [28](#json-28) | Ctrl+Espaço | (nada) | `Lovelace` | — | `(no rule)` | ⚠️ |
| [29](#json-29) | 1 letra | `e` | `emailVerified` | — | `(no rule)` | ✅ |
| [30](#json-30) | 2 letras | `tr` | `true` | 1 | `(no rule)` | ✅ |
| [31](#json-31) | 3 letras | `las` | `lastLogin` | — | `(no rule)` | ⚠️ |
| [32](#json-32) | Ctrl+Espaço | (nada) | `null` | 2 | `(no rule)` | ✅ |
| [33](#json-33) | 1 letra | `r` | `roles` | — | `(no rule)` | ✅ |
| [34](#json-34) | 2 letras | `ad` | `admin` | — | `(no rule)` | ✅ |
| [35](#json-35) | 3 letras | `edi` | `editor` | — | `(no rule)` | ✅ |
| [36](#json-36) | Ctrl+Espaço | (nada) | `preferences` | — | `(no rule)` | ⚠️ |
| [37](#json-37) | 1 letra | `t` | `theme` | — | `(no rule)` | ✅ |
| [38](#json-38) | 2 letras | `da` | `dark` | — | `(no rule)` | ✅ |
| [39](#json-39) | 3 letras | `not` | `notifications` | — | `(no rule)` | ✅ |
| [40](#json-40) | erro: trocadas | `flas` | `false` | 1 | `(no rule)` | ✅ |
| [41](#json-41) | 1 letra | `l` | `language` | — | `(no rule)` | ✅ |
| [42](#json-42) | 1 letra | `e` | `en` | — | `(no rule)` | ✅ |
| [43](#json-43) | 3 letras | `err` | `errors` | — | `(no rule)` | ✅ |
| [44](#json-44) | Ctrl+Espaço | (nada) | `server` | — | `(no rule)` | ⚠️ |
| [45](#json-45) | 1 letra | `h` | `host` | 1 | `(no rule)` | ✅ |
| [46](#json-46) | 2 letras | `lo` | `localhost` | — | `(no rule)` | ✅ |
| [47](#json-47) | 3 letras | `por` | `port` | 1 | `(no rule)` | ✅ |
| [48](#json-48) | Ctrl+Espaço | (nada) | `secure` | — | `(no rule)` | ⚠️ |
| [49](#json-49) | 1 letra | `f` | `false` | 1 | `(no rule)` | ✅ |
| [50](#json-50) | erro: faltando | `daaba` | `database` | — | `(no rule)` | ✅ |
| [51](#json-51) | 3 letras | `hos` | `host` | 1 | `(no rule)` | ✅ |
| [52](#json-52) | Ctrl+Espaço | (nada) | `db` | — | `(no rule)` | ⚠️ |
| [53](#json-53) | 1 letra | `i` | `internal` | — | `(no rule)` | ✅ |
| [54](#json-54) | 2 letras | `po` | `port` | 2 | `(no rule)` | ✅ |
| [55](#json-55) | 3 letras | `poo` | `poolSize` | — | `(no rule)` | ⚠️ |
| [56](#json-56) | Ctrl+Espaço | (nada) | `ssl` | — | `(no rule)` | ⚠️ |
| [57](#json-57) | 1 letra | `t` | `true` | 1 | `(no rule)` | ✅ |
| [58](#json-58) | 2 letras | `fe` | `features` | — | `(no rule)` | ✅ |
| [59](#json-59) | 3 letras | `bet` | `betaSearch` | — | `(no rule)` | ✅ |
| [60](#json-60) | Ctrl+Espaço | (nada) | `true` | 3 | `(no rule)` | ✅ |
| [61](#json-61) | 1 letra | `l` | `legacyExport` | — | `(no rule)` | ✅ |
| [62](#json-62) | 2 letras | `fa` | `false` | 1 | `(no rule)` | ✅ |
| [63](#json-63) | 3 letras | `mai` | `maintenanceBanner` | — | `(no rule)` | ✅ |
| [64](#json-64) | Ctrl+Espaço | (nada) | `null` | 2 | `(no rule)` | ✅ |
| [65](#json-65) | 1 letra | `l` | `logging` | — | `(no rule)` | ✅ |
| [66](#json-66) | 2 letras | `le` | `level` | — | `(no rule)` | ✅ |
| [67](#json-67) | 3 letras | `inf` | `info` | — | `(no rule)` | ⚠️ |
| [68](#json-68) | Ctrl+Espaço | (nada) | `destination` | — | `(no rule)` | ⚠️ |
| [69](#json-69) | 1 letra | `s` | `stdout` | — | `(no rule)` | ✅ |
| [70](#json-70) | erro: faltando | `itm` | `items` | — | `(no rule)` | ✅ |
| [71](#json-71) | 3 letras | `tot` | `total` | — | `(no rule)` | ✅ |
| [72](#json-72) | Ctrl+Espaço | (nada) | `hasMore` | — | `(no rule)` | ⚠️ |
| [73](#json-73) | 1 letra | `f` | `false` | 1 | `(no rule)` | ✅ |
| [74](#json-74) | 2 letras | `na` | `name` | — | `(no rule)` | ✅ |
| [75](#json-75) | 3 letras | `nes` | `nestlight` | — | `(no rule)` | ✅ |
| [76](#json-76) | Ctrl+Espaço | (nada) | `demo` | — | `(no rule)` | ⚠️ |
| [77](#json-77) | 1 letra | `v` | `version` | — | `(no rule)` | ✅ |
| [78](#json-78) | 2 letras | `pr` | `private` | — | `(no rule)` | ✅ |
| [79](#json-79) | 3 letras | `tru` | `true` | 1 | `(no rule)` | ✅ |
| [80](#json-80) | erro: trocadas | `srcipt` | `scripts` | — | `(no rule)` | ✅ |
| [81](#json-81) | 1 letra | `b` | `build` | — | `(no rule)` | ✅ |
| [82](#json-82) | 2 letras | `ts` | `tsc` | — | `(no rule)` | ✅ |
| [83](#json-83) | 3 letras | `tes` | `test` | — | `(no rule)` | ⚠️ |
| [84](#json-84) | Ctrl+Espaço | (nada) | `jest` | 22 | `(no rule)` | ⚠️ |
| [85](#json-85) | 1 letra | `c` | `coverage` | — | `(no rule)` | ✅ |
| [86](#json-86) | 2 letras | `li` | `lint` | — | `(no rule)` | ✅ |
| [87](#json-87) | 3 letras | `esl` | `eslint` | — | `(no rule)` | ✅ |
| [88](#json-88) | Ctrl+Espaço | (nada) | `src` | — | `(no rule)` | ⚠️ |
| [89](#json-89) | 1 letra | `d` | `dependencies` | — | `(no rule)` | ✅ |
| [90](#json-90) | erro: faltando | `exres` | `express` | — | `(no rule)` | ✅ |
| [91](#json-91) | 3 letras | `lod` | `lodash` | — | `(no rule)` | ✅ |
| [92](#json-92) | Ctrl+Espaço | (nada) | `devDependencies` | — | `(no rule)` | ⚠️ |
| [93](#json-93) | 1 letra | `t` | `typescript` | — | `(no rule)` | ✅ |
| [94](#json-94) | 2 letras | `je` | `jest` | 1 | `(no rule)` | ✅ |
| [95](#json-95) | 3 letras | `sta` | `status` | — | `(no rule)` | ✅ |
| [96](#json-96) | Ctrl+Espaço | (nada) | `ok` | — | `(no rule)` | ⚠️ |
| [97](#json-97) | 1 letra | `d` | `data` | — | `(no rule)` | ✅ |
| [98](#json-98) | 2 letras | `us` | `userId` | — | `(no rule)` | ✅ |
| [99](#json-99) | 3 letras | `dis` | `displayName` | — | `(no rule)` | ✅ |
| [100](#json-100) | Ctrl+Espaço | (nada) | `Ada` | — | `(no rule)` | ⚠️ |

Posição: lugar da palavra procurada na lista; `—` = a palavra não existe em outro lugar do arquivo; `fora` = existe mas não está na lista.

## Os arquivos usados como entrada

Escritos à mão como um desenvolvedor escreveria (código JavaScript com strings da linguagem). Nada foi gerado pelo gerador dos experimentos.

### Documento D1

```js
const packageJson = json`
{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}
`;
```

### Documento D2

```js
const response = json`
{
  "status": "ok",
  "data": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}
`;
```

### Documento D3

```js
const config = json`
{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}
`;

const empty = json`{ "items": [], "total": 0, "hasMore": false }`;
```

## Os exemplos

### JSON-1
<a id="json-1"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "n▮": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `name`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 null
 2 nestlight [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `"n`: `null` (palavra-chave de valor) é oferecida para o nome de uma chave.

---

### JSON-2
<a id="json-2"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "ne▮-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `nestlight`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-3
<a id="json-3"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-dem▮",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `demo`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 dependencies [~]
 2 devDependencies [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. `dem` dentro de uma string: `dependencies~` e `devDependencies~` são ruído (palavras parecidas para um prefixo que não é um erro).

---

### JSON-4
<a id="json-4"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "▮": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `version`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false           11 test [a]
 2 null            12 jest [a]
 3 true            13 coverage [a]
 4 demo [a]        14 lint [a]
 5 nestlight [a]   15 eslint [a]
 6 private [a]     16 src [a]
 7 name [a]        17 dependencies [a]
 8 scripts [a]     18 express [a]
 9 build [a]       19 lodash [a]
10 tsc [a]         20 devDependencies [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: `true`/`false`/`null` vêm primeiro (são valores, não chaves), depois palavras de chaves e valores misturadas.

---

### JSON-5
<a id="json-5"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "p▮": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `private`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 packageJson [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `"p`: só uma variável do código hospedeiro (`packageJson`) é oferecida; a chave não aparece em outro lugar.

---

### JSON-6
<a id="json-6"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": tr▮,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `true`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 true
```

**Veredito:** ✅ Bom. `true` é o único item.

---

### JSON-7
<a id="json-7"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scr▮": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `scripts`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 src [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. `scr`: `src~` é ruído (palavra parecida para uma chave sem relação).

---

### JSON-8
<a id="json-8"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "▮": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `build`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false          11 version [a]
 2 null           12 eslint [a]
 3 true           13 demo [a]
 4 tsc [a]        14 src [a]
 5 scripts [a]    15 nestlight [a]
 6 test [a]       16 dependencies [a]
 7 jest [a]       17 name [a]
 8 private [a]    18 express [a]
 9 coverage [a]   19 lodash [a]
10 lint [a]       20 devDependencies [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: `true`/`false`/`null` vêm primeiro (são valores, não chaves), depois palavras de chaves e valores misturadas.

---

### JSON-9
<a id="json-9"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "t▮ -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `tsc`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 true
 2 test [a]
 3 typescript [a]
```

**Veredito:** ✅ Bom. `true`, `test`, `typescript`: palavras que começam com t.

---

### JSON-10
<a id="json-10"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "te▮": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `test`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-11
<a id="json-11"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jes▮ --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `jest`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 jest [a]
```

**Veredito:** ✅ Bom. `jest` é o único item.

---

### JSON-12
<a id="json-12"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --▮",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `coverage`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false        11 dependencies [a]
 2 null         12 scripts [a]
 3 true         13 express [a]
 4 jest [a]     14 private [a]
 5 lint [a]     15 lodash [a]
 6 test [a]     16 version [a]
 7 eslint [a]   17 demo [a]
 8 src [a]      18 nestlight [a]
 9 tsc [a]      19 devDependencies [a]
10 build [a]    20 name [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito dentro de um valor de texto: `true`/`false`/`null` vêm primeiro, o que não faz sentido dentro de uma string.

---

### JSON-13
<a id="json-13"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "l▮": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `lint`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 lodash [a]
```

**Veredito:** ✅ Bom. `lodash` é oferecido para `l`; a chave `lint` não aparece em outro lugar.

---

### JSON-14
<a id="json-14"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "es▮ src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `eslint`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-15
<a id="json-15"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint sr▮"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `src`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-16
<a id="json-16"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "▮": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `dependencies`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false          11 test [a]
 2 null           12 devDependencies [a]
 3 true           13 tsc [a]
 4 express [a]    14 build [a]
 5 src [a]        15 typescript [a]
 6 eslint [a]     16 scripts [a]
 7 lint [a]       17 private [a]
 8 coverage [a]   18 version [a]
 9 lodash [a]     19 demo [a]
10 jest [a]       20 nestlight [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: `true`/`false`/`null` vêm primeiro (são valores, não chaves), depois palavras de chaves e valores misturadas.

---

### JSON-17
<a id="json-17"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "e▮": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `express`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 eslint [a]
```

**Veredito:** ✅ Bom. `eslint` é oferecido para `e`; a chave `express` não aparece em outro lugar.

---

### JSON-18
<a id="json-18"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 13 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lo▮": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `lodash`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-19
<a id="json-19"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 15 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "dev▮": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `devDependencies`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 dependencies [~]
 2 demo [~]
```

**Veredito:** ✅ Bom. `dependencies~` é uma chave irmã plausível; `devDependencies` não aparece em outro lugar.

---

### JSON-20
<a id="json-20"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D1, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "tpyesc▮": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `typescript`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-21
<a id="json-21"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "j▮": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `jest`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 jest [a]
 2 json [a]
```

**Veredito:** ✅ Bom. `jest`, `json`.

---

### JSON-22
<a id="json-22"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "st▮": "ok",
  "data": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `status`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-23
<a id="json-23"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "o▮",
  "data": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `ok`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-24
<a id="json-24"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "▮": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `data`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false               11 roles [a]
 2 null                12 admin [a]
 3 true                13 editor [a]
 4 userId [a]          14 preferences [a]
 5 status [a]          15 theme [a]
 6 displayName [a]     16 dark [a]
 7 Ada [a]             17 notifications [a]
 8 Lovelace [a]        18 language [a]
 9 emailVerified [a]   19 errors [a]
10 lastLogin [a]       20 json [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: `true`/`false`/`null` vêm primeiro (são valores, não chaves), depois palavras de chaves e valores misturadas.

---

### JSON-25
<a id="json-25"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "u▮": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `userId`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-26
<a id="json-26"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "userId": 42,
    "di▮": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `displayName`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-27
<a id="json-27"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "userId": 42,
    "displayName": "Ad▮ Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `Ada`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 admin [a]
```

**Veredito:** ✅ Bom. `admin` é oferecido para `Ad`.

---

### JSON-28
<a id="json-28"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "userId": 42,
    "displayName": "Ada ▮",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `Lovelace`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false               11 status [a]
 2 null                12 admin [a]
 3 true                13 editor [a]
 4 Ada [a]             14 preferences [a]
 5 displayName [a]     15 theme [a]
 6 emailVerified [a]   16 dark [a]
 7 userId [a]          17 notifications [a]
 8 lastLogin [a]       18 language [a]
 9 data [a]            19 errors [a]
10 roles [a]           20 json [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito dentro de um valor de texto: `true`/`false`/`null` vêm primeiro, o que não faz sentido dentro de uma string.

---

### JSON-29
<a id="json-29"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "e▮": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `emailVerified`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 editor [a]
 2 errors [a]
```

**Veredito:** ✅ Bom. `editor`, `errors`.

---

### JSON-30
<a id="json-30"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": tr▮,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `true`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 true
```

**Veredito:** ✅ Bom. `true` é o único item.

---

### JSON-31
<a id="json-31"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "las▮": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `lastLogin`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 language [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. `las`: `language~` é ruído.

---

### JSON-32
<a id="json-32"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": ▮,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `null`: aparece em 2º lugar de 23

**Saída** (as 20 primeiras sugestões):

```text
 1 false               11 Ada [a]
 2 null                12 displayName [a]
 3 true                13 theme [a]
 4 lastLogin [a]       14 dark [a]
 5 roles [a]           15 userId [a]
 6 admin [a]           16 notifications [a]
 7 editor [a]          17 data [a]
 8 emailVerified [a]   18 language [a]
 9 preferences [a]     19 status [a]
10 Lovelace [a]        20 errors [a]
```

**Veredito:** ✅ Bom. Pedido explícito numa posição de valor: `false`, `null`, `true` primeiro; `null` é o 2º.

---

### JSON-33
<a id="json-33"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "r▮": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `roles`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 response [a]
```

**Veredito:** ✅ Bom. `response` (variável do código hospedeiro) é o único item.

---

### JSON-34
<a id="json-34"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["ad▮", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `admin`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 Ada [a]
```

**Veredito:** ✅ Bom. `Ada` é oferecido para `ad`.

---

### JSON-35
<a id="json-35"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "edi▮"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `editor`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-36
<a id="json-36"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "▮": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `preferences`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false               11 language [a]
 2 null                12 emailVerified [a]
 3 true                13 Lovelace [a]
 4 editor [a]          14 errors [a]
 5 theme [a]           15 Ada [a]
 6 admin [a]           16 displayName [a]
 7 dark [a]            17 userId [a]
 8 roles [a]           18 data [a]
 9 notifications [a]   19 status [a]
10 lastLogin [a]       20 json [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: `true`/`false`/`null` vêm primeiro (são valores, não chaves), depois palavras de chaves e valores misturadas.

---

### JSON-37
<a id="json-37"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "t▮": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `theme`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 true
```

**Veredito:** ✅ Bom. `true` é o único item.

---

### JSON-38
<a id="json-38"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "da▮",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `dark`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 data [a]
```

**Veredito:** ✅ Bom. `data` é o único item.

---

### JSON-39
<a id="json-39"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "not▮": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `notifications`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-40
<a id="json-40"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D2, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": flas▮,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `false`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 false [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `false`.

---

### JSON-41
<a id="json-41"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 13 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "l▮": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `language`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 lastLogin [a]
 2 Lovelace [a]
```

**Veredito:** ✅ Bom. `lastLogin`, `Lovelace`.

---

### JSON-42
<a id="json-42"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 13 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "e▮"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `en`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 errors [a]
 2 editor [a]
 3 emailVerified [a]
```

**Veredito:** ✅ Bom. Palavras que começam com e.

---

### JSON-43
<a id="json-43"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "err▮": []
}

```

**Palavra que a pessoa ia digitar:** `errors`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-44
<a id="json-44"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "▮": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `server`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false           11 ssl [a]
 2 null            12 features [a]
 3 true            13 betaSearch [a]
 4 host [a]        14 legacyExport [a]
 5 localhost [a]   15 maintenanceBanner [a]
 6 port [a]        16 logging [a]
 7 secure [a]      17 level [a]
 8 database [a]    18 info [a]
 9 internal [a]    19 destination [a]
10 poolSize [a]    20 stdout [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: `true`/`false`/`null` vêm primeiro (são valores, não chaves), depois palavras de chaves e valores misturadas.

---

### JSON-45
<a id="json-45"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "h▮": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `host`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 host [a]
 2 hasMore [a]
```

**Veredito:** ✅ Bom. `host`, `hasMore`.

---

### JSON-46
<a id="json-46"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "lo▮", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `localhost`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 logging [a]
```

**Veredito:** ✅ Bom. `logging` é o único item.

---

### JSON-47
<a id="json-47"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "por▮": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `port`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 port [a]
```

**Veredito:** ✅ Bom. `port` é o único item.

---

### JSON-48
<a id="json-48"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "▮": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `secure`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false           11 ssl [a]
 2 null            12 features [a]
 3 true            13 betaSearch [a]
 4 port [a]        14 legacyExport [a]
 5 database [a]    15 maintenanceBanner [a]
 6 localhost [a]   16 logging [a]
 7 host [a]        17 level [a]
 8 internal [a]    18 info [a]
 9 server [a]      19 destination [a]
10 poolSize [a]    20 stdout [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: `true`/`false`/`null` vêm primeiro (são valores, não chaves), depois palavras de chaves e valores misturadas.

---

### JSON-49
<a id="json-49"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": f▮ },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `false`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 false
 2 features [a]
```

**Veredito:** ✅ Bom. `false` em primeiro.

---

### JSON-50
<a id="json-50"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D3, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "daaba▮": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `database`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-51
<a id="json-51"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "hos▮": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `host`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 host [a]
```

**Veredito:** ✅ Bom. `host` é o único item.

---

### JSON-52
<a id="json-52"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "▮.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `db`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 localhost [a]   11 ssl [a]
 2 false           12 features [a]
 3 null            13 betaSearch [a]
 4 true            14 server [a]
 5 internal [a]    15 legacyExport [a]
 6 host [a]        16 maintenanceBanner [a]
 7 port [a]        17 logging [a]
 8 database [a]    18 level [a]
 9 poolSize [a]    19 info [a]
10 secure [a]      20 destination [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito dentro de uma string: `localhost` primeiro, depois `true`/`false`/`null`, que não pertencem a uma string.

---

### JSON-53
<a id="json-53"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.i▮", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `internal`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 info [a]
 2 items [a]
```

**Veredito:** ✅ Bom. `info`, `items`.

---

### JSON-54
<a id="json-54"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "po▮": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `port`: aparece em 2º lugar de 2

**Saída** (as 20 primeiras sugestões):

```text
 1 poolSize [a]
 2 port [a]
```

**Veredito:** ✅ Bom. `poolSize`, `port` (a procurada é a 2ª).

---

### JSON-55
<a id="json-55"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poo▮": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `poolSize`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 port [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. `poo`: `port~` é ruído.

---

### JSON-56
<a id="json-56"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "▮": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `ssl`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false              11 database [a]
 2 null               12 maintenanceBanner [a]
 3 true               13 secure [a]
 4 poolSize [a]       14 logging [a]
 5 features [a]       15 localhost [a]
 6 port [a]           16 level [a]
 7 betaSearch [a]     17 info [a]
 8 internal [a]       18 server [a]
 9 legacyExport [a]   19 destination [a]
10 host [a]           20 stdout [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: `true`/`false`/`null` vêm primeiro (são valores, não chaves), depois palavras de chaves e valores misturadas.

---

### JSON-57
<a id="json-57"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": t▮ },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `true`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 true
 2 total [a]
```

**Veredito:** ✅ Bom. `true` em primeiro.

---

### JSON-58
<a id="json-58"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "fe▮": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `features`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-59
<a id="json-59"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "bet▮": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `betaSearch`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-60
<a id="json-60"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": ▮, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `true`: aparece em 3º lugar de 28

**Saída** (as 20 primeiras sugestões):

```text
 1 false                   11 level [a]
 2 null                    12 port [a]
 3 true                    13 internal [a]
 4 betaSearch [a]          14 info [a]
 5 legacyExport [a]        15 destination [a]
 6 features [a]            16 host [a]
 7 maintenanceBanner [a]   17 stdout [a]
 8 ssl [a]                 18 database [a]
 9 poolSize [a]            19 secure [a]
10 logging [a]             20 items [a]
```

**Veredito:** ✅ Bom. Pedido explícito numa posição de valor: `false`, `null`, `true`, depois palavras; `true` é o 3º.

---

### JSON-61
<a id="json-61"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "l▮": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `legacyExport`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 logging [a]
 2 level [a]
 3 localhost [a]
```

**Veredito:** ✅ Bom. Palavras que começam com l.

---

### JSON-62
<a id="json-62"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": fa▮, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `false`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 false
```

**Veredito:** ✅ Bom. `false` é o único item.

---

### JSON-63
<a id="json-63"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "mai▮": null },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `maintenanceBanner`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-64
<a id="json-64"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": ▮ },
  "logging": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `null`: aparece em 2º lugar de 28

**Saída** (as 20 primeiras sugestões):

```text
 1 false                   11 betaSearch [a]
 2 null                    12 features [a]
 3 true                    13 items [a]
 4 maintenanceBanner [a]   14 ssl [a]
 5 logging [a]             15 poolSize [a]
 6 level [a]               16 total [a]
 7 info [a]                17 hasMore [a]
 8 legacyExport [a]        18 port [a]
 9 destination [a]         19 internal [a]
10 stdout [a]              20 host [a]
```

**Veredito:** ✅ Bom. Pedido explícito numa posição de valor: `null` é o 2º.

---

### JSON-65
<a id="json-65"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "l▮": { "level": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `logging`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 level [a]
 2 legacyExport [a]
 3 localhost [a]
```

**Veredito:** ✅ Bom. Palavras que começam com l.

---

### JSON-66
<a id="json-66"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "le▮": "info", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `level`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 legacyExport [a]
```

**Veredito:** ✅ Bom. `legacyExport` é oferecido para `le`.

---

### JSON-67
<a id="json-67"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "inf▮", "destination": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `info`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 internal [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. `inf`: `internal~` é ruído.

---

### JSON-68
<a id="json-68"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "▮": "stdout" }
}

```

**Palavra que a pessoa ia digitar:** `destination`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false                   11 hasMore [a]
 2 null                    12 legacyExport [a]
 3 true                    13 betaSearch [a]
 4 info [a]                14 features [a]
 5 stdout [a]              15 ssl [a]
 6 level [a]               16 poolSize [a]
 7 logging [a]             17 port [a]
 8 items [a]               18 internal [a]
 9 maintenanceBanner [a]   19 host [a]
10 total [a]               20 database [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: `true`/`false`/`null` vêm primeiro (são valores, não chaves), depois palavras de chaves e valores misturadas.

---

### JSON-69
<a id="json-69"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "s▮" }
}

```

**Palavra que a pessoa ia digitar:** `stdout`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 ssl [a]
 2 secure [a]
 3 server [a]
```

**Veredito:** ✅ Bom. `ssl`, `secure`, `server`.

---

### JSON-70
<a id="json-70"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D3, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text
{ "itm▮": [], "total": 0, "hasMore": false }
```

**Palavra que a pessoa ia digitar:** `items`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-71
<a id="json-71"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text
{ "items": [], "tot▮": 0, "hasMore": false }
```

**Palavra que a pessoa ia digitar:** `total`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-72
<a id="json-72"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text
{ "items": [], "total": 0, "▮": false }
```

**Palavra que a pessoa ia digitar:** `hasMore`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false             11 maintenanceBanner [a]
 2 null              12 legacyExport [a]
 3 true              13 betaSearch [a]
 4 total [a]         14 features [a]
 5 items [a]         15 ssl [a]
 6 stdout [a]        16 poolSize [a]
 7 destination [a]   17 port [a]
 8 info [a]          18 internal [a]
 9 level [a]         19 host [a]
10 logging [a]       20 database [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: `true`/`false`/`null` vêm primeiro (são valores, não chaves), depois palavras de chaves e valores misturadas.

---

### JSON-73
<a id="json-73"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text
{ "items": [], "total": 0, "hasMore": f▮ }
```

**Palavra que a pessoa ia digitar:** `false`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 false
 2 features [a]
```

**Veredito:** ✅ Bom. `false` em primeiro.

---

### JSON-74
<a id="json-74"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "na▮": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `name`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-75
<a id="json-75"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nes▮-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `nestlight`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-76
<a id="json-76"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-▮",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `demo`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false           11 test [a]
 2 null            12 jest [a]
 3 true            13 coverage [a]
 4 nestlight [a]   14 lint [a]
 5 version [a]     15 eslint [a]
 6 name [a]        16 src [a]
 7 private [a]     17 dependencies [a]
 8 scripts [a]     18 express [a]
 9 build [a]       19 lodash [a]
10 tsc [a]         20 devDependencies [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito dentro de um valor de texto: `true`/`false`/`null` vêm primeiro, o que não faz sentido dentro de uma string.

---

### JSON-77
<a id="json-77"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "v▮": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `version`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-78
<a id="json-78"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "pr▮": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `private`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-79
<a id="json-79"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": tru▮,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `true`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 true
```

**Veredito:** ✅ Bom. `true` é o único item.

---

### JSON-80
<a id="json-80"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D1, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "srcipt▮": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `scripts`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-81
<a id="json-81"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "b▮": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `build`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-82
<a id="json-82"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "ts▮ -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `tsc`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-83
<a id="json-83"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "tes▮": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `test`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 tsc [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. `tes`: `tsc~` é ruído.

---

### JSON-84
<a id="json-84"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "▮ --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `jest`: aparece em 22º lugar de 25 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 false          11 scripts [a]
 2 null           12 dependencies [a]
 3 true           13 private [a]
 4 coverage [a]   14 express [a]
 5 test [a]       15 version [a]
 6 lint [a]       16 lodash [a]
 7 tsc [a]        17 demo [a]
 8 eslint [a]     18 nestlight [a]
 9 build [a]      19 name [a]
10 src [a]        20 devDependencies [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito dentro de um valor de texto: `true`/`false`/`null` vêm primeiro, o que não faz sentido dentro de uma string.

---

### JSON-85
<a id="json-85"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --c▮",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `coverage`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 const [a]
```

**Veredito:** ✅ Bom. `const` (palavra do código hospedeiro) é o único item.

---

### JSON-86
<a id="json-86"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "li▮": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `lint`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-87
<a id="json-87"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "esl▮ src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `eslint`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-88
<a id="json-88"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint ▮"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `src`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false              11 lodash [a]
 2 null               12 tsc [a]
 3 true               13 build [a]
 4 eslint [a]         14 scripts [a]
 5 dependencies [a]   15 devDependencies [a]
 6 lint [a]           16 private [a]
 7 coverage [a]       17 typescript [a]
 8 express [a]        18 version [a]
 9 jest [a]           19 demo [a]
10 test [a]           20 nestlight [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito dentro de um valor de texto: `true`/`false`/`null` vêm primeiro, o que não faz sentido dentro de uma string.

---

### JSON-89
<a id="json-89"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "d▮": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `dependencies`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 devDependencies [a]
 2 demo [a]
```

**Veredito:** ✅ Bom. `devDependencies`, `demo`.

---

### JSON-90
<a id="json-90"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D1, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "exres▮": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `express`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-91
<a id="json-91"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 13 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lod▮": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `lodash`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-92
<a id="json-92"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 15 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "▮": {
    "typescript": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `devDependencies`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false              11 lint [a]
 2 null               12 coverage [a]
 3 true               13 test [a]
 4 typescript [a]     14 tsc [a]
 5 lodash [a]         15 build [a]
 6 jest [a]           16 scripts [a]
 7 express [a]        17 private [a]
 8 dependencies [a]   18 version [a]
 9 src [a]            19 demo [a]
10 eslint [a]         20 nestlight [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: `true`/`false`/`null` vêm primeiro (são valores, não chaves), depois palavras de chaves e valores misturadas.

---

### JSON-93
<a id="json-93"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "t▮": "^5.0.0",
    "jest": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `typescript`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 true
 2 test [a]
 3 tsc [a]
```

**Veredito:** ✅ Bom. `true`, `test`, `tsc`.

---

### JSON-94
<a id="json-94"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "name": "nestlight-demo",
  "version": "1.2.0",
  "private": true,
  "scripts": {
    "build": "tsc -p .",
    "test": "jest --coverage",
    "lint": "eslint src"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0",
    "je▮": "^29.0.0"
  }
}

```

**Palavra que a pessoa ia digitar:** `jest`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 jest [a]
```

**Veredito:** ✅ Bom. `jest` é o único item.

---

### JSON-95
<a id="json-95"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "sta▮": "ok",
  "data": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `status`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-96
<a id="json-96"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "▮",
  "data": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `ok`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false               11 lastLogin [a]
 2 null                12 roles [a]
 3 true                13 admin [a]
 4 status [a]          14 editor [a]
 5 data [a]            15 preferences [a]
 6 userId [a]          16 theme [a]
 7 displayName [a]     17 dark [a]
 8 Ada [a]             18 notifications [a]
 9 Lovelace [a]        19 language [a]
10 emailVerified [a]   20 errors [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito dentro de um valor de texto: `true`/`false`/`null` vêm primeiro, o que não faz sentido dentro de uma string.

---

### JSON-97
<a id="json-97"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "d▮": {
    "userId": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `data`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 displayName [a]
 2 dark [a]
```

**Veredito:** ✅ Bom. `displayName`, `dark`.

---

### JSON-98
<a id="json-98"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "us▮": 42,
    "displayName": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `userId`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-99
<a id="json-99"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "userId": 42,
    "dis▮": "Ada Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `displayName`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### JSON-100
<a id="json-100"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

{
  "status": "ok",
  "data": {
    "userId": 42,
    "displayName": "▮ Lovelace",
    "emailVerified": true,
    "lastLogin": null,
    "roles": ["admin", "editor"],
    "preferences": {
      "theme": "dark",
      "notifications": false,
      "language": "en"
    }
  },
  "errors": []
}

```

**Palavra que a pessoa ia digitar:** `Ada`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 Lovelace [a]        11 roles [a]
 2 false               12 admin [a]
 3 null                13 editor [a]
 4 true                14 preferences [a]
 5 displayName [a]     15 theme [a]
 6 emailVerified [a]   16 dark [a]
 7 userId [a]          17 notifications [a]
 8 data [a]            18 language [a]
 9 lastLogin [a]       19 errors [a]
10 status [a]          20 json [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito dentro de uma string: `Lovelace` (a palavra seguinte) primeiro, depois `true`/`false`/`null`.

---
