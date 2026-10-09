# YAML: 100 exemplos

Resultado: ✅ 74 bons · ⚠️ 23 razoáveis com ressalva · ❌ 3 ruins.

Como ler: em cada exemplo, `▮` marca onde está o cursor. A lista é o que o plugin mostraria (as 20 primeiras). `[a]` = palavra que já existe no arquivo; `[~]` = sugestão "parecida" (corrige erro de digitação); sem marca = palavra-chave da linguagem. O veredito e o comentário são a minha análise. "Lugar na gramática" é o nome interno da regra de posição que o plugin aplicou (`sql:table`, `css:value:display`...); `(no rule)` quer dizer que o plugin não tem regra para aquele lugar e usa só o que foi digitado.

## Índice (para varrer rápido)

| # | Situação | Digitado | Palavra procurada | Posição | Lugar na gramática | Veredito |
|---|---|---|---|---|---|---|
| [1](#yaml-1) | 1 letra | `a` | `apiVersion` | — | `(no rule)` | ✅ |
| [2](#yaml-2) | 2 letras | `ap` | `apps` | — | `(no rule)` | ✅ |
| [3](#yaml-3) | 1 letra | `v` | `v1` | — | `(no rule)` | ✅ |
| [4](#yaml-4) | Ctrl+Espaço | (nada) | `kind` | — | `(no rule)` | ⚠️ |
| [5](#yaml-5) | 1 letra | `D` | `Deployment` | 1 | `(no rule)` | ✅ |
| [6](#yaml-6) | 2 letras | `me` | `metadata` | 1 | `(no rule)` | ✅ |
| [7](#yaml-7) | 3 letras | `nam` | `name` | 1 | `(no rule)` | ✅ |
| [8](#yaml-8) | Ctrl+Espaço | (nada) | `labels` | 22 | `(no rule)` | ⚠️ |
| [9](#yaml-9) | 1 letra | `a` | `app` | 1 | `(no rule)` | ✅ |
| [10](#yaml-10) | 2 letras | `we` | `web` | 1 | `(no rule)` | ✅ |
| [11](#yaml-11) | 3 letras | `spe` | `spec` | 1 | `(no rule)` | ✅ |
| [12](#yaml-12) | Ctrl+Espaço | (nada) | `replicas` | — | `(no rule)` | ⚠️ |
| [13](#yaml-13) | 1 letra | `s` | `selector` | — | `(no rule)` | ✅ |
| [14](#yaml-14) | 2 letras | `ma` | `matchLabels` | — | `(no rule)` | ✅ |
| [15](#yaml-15) | 2 letras | `we` | `web` | 1 | `(no rule)` | ✅ |
| [16](#yaml-16) | Ctrl+Espaço | (nada) | `template` | — | `(no rule)` | ⚠️ |
| [17](#yaml-17) | 1 letra | `m` | `metadata` | 2 | `(no rule)` | ✅ |
| [18](#yaml-18) | 2 letras | `la` | `labels` | 1 | `(no rule)` | ✅ |
| [19](#yaml-19) | 2 letras | `ap` | `app` | 1 | `(no rule)` | ✅ |
| [20](#yaml-20) | Ctrl+Espaço | (nada) | `web` | 2 | `(no rule)` | ⚠️ |
| [21](#yaml-21) | 1 letra | `c` | `containers` | — | `(no rule)` | ✅ |
| [22](#yaml-22) | 2 letras | `na` | `name` | 1 | `(no rule)` | ✅ |
| [23](#yaml-23) | 2 letras | `we` | `web` | 1 | `(no rule)` | ✅ |
| [24](#yaml-24) | Ctrl+Espaço | (nada) | `image` | — | `(no rule)` | ⚠️ |
| [25](#yaml-25) | 1 letra | `r` | `registry` | — | `(no rule)` | ✅ |
| [26](#yaml-26) | 2 letras | `ex` | `example` | — | `(no rule)` | ✅ |
| [27](#yaml-27) | 2 letras | `co` | `com` | — | `(no rule)` | ✅ |
| [28](#yaml-28) | Ctrl+Espaço | (nada) | `ports` | — | `(no rule)` | ⚠️ |
| [29](#yaml-29) | 1 letra | `c` | `containerPort` | — | `(no rule)` | ✅ |
| [30](#yaml-30) | 2 letras | `en` | `env` | — | `(no rule)` | ✅ |
| [31](#yaml-31) | 3 letras | `nam` | `name` | 1 | `(no rule)` | ✅ |
| [32](#yaml-32) | Ctrl+Espaço | (nada) | `LOG_LEVEL` | — | `(no rule)` | ⚠️ |
| [33](#yaml-33) | 1 letra | `v` | `value` | — | `(no rule)` | ✅ |
| [34](#yaml-34) | 2 letras | `in` | `info` | — | `(no rule)` | ✅ |
| [35](#yaml-35) | 3 letras | `lim` | `limits` | — | `(no rule)` | ✅ |
| [36](#yaml-36) | Ctrl+Espaço | (nada) | `memory` | — | `(no rule)` | ⚠️ |
| [37](#yaml-37) | 1 letra | `c` | `cpu` | — | `(no rule)` | ✅ |
| [38](#yaml-38) | 2 letras | `na` | `name` | 1 | `(no rule)` | ✅ |
| [39](#yaml-39) | 1 letra | `c` | `ci` | fora | `(no rule)` | ❌ |
| [40](#yaml-40) | Ctrl+Espaço | (nada) | `on` | 5 | `(no rule)` | ⚠️ |
| [41](#yaml-41) | 1 letra | `b` | `branches` | — | `(no rule)` | ✅ |
| [42](#yaml-42) | 2 letras | `ma` | `main` | — | `(no rule)` | ✅ |
| [43](#yaml-43) | 3 letras | `pul` | `pull_request` | — | `(no rule)` | ✅ |
| [44](#yaml-44) | Ctrl+Espaço | (nada) | `jobs` | — | `(no rule)` | ⚠️ |
| [45](#yaml-45) | 1 letra | `b` | `build` | — | `(no rule)` | ✅ |
| [46](#yaml-46) | 2 letras | `ru` | `runs-on` | — | `(no rule)` | ✅ |
| [47](#yaml-47) | 3 letras | `ubu` | `ubuntu-latest` | — | `(no rule)` | ✅ |
| [48](#yaml-48) | Ctrl+Espaço | (nada) | `uses` | 28 | `(no rule)` | ⚠️ |
| [49](#yaml-49) | 1 letra | `a` | `actions` | 1 | `(no rule)` | ✅ |
| [50](#yaml-50) | erro: faltando | `chcko` | `checkout` | — | `(no rule)` | ✅ |
| [51](#yaml-51) | 3 letras | `nam` | `name` | 1 | `(no rule)` | ✅ |
| [52](#yaml-52) | Ctrl+Espaço | (nada) | `Install` | — | `(no rule)` | ⚠️ |
| [53](#yaml-53) | 1 letra | `r` | `run` | 1 | `(no rule)` | ✅ |
| [54](#yaml-54) | 2 letras | `np` | `npm` | 1 | `(no rule)` | ✅ |
| [55](#yaml-55) | 3 letras | `nam` | `name` | 1 | `(no rule)` | ✅ |
| [56](#yaml-56) | Ctrl+Espaço | (nada) | `Test` | 15 | `(no rule)` | ⚠️ |
| [57](#yaml-57) | 1 letra | `r` | `run` | 1 | `(no rule)` | ✅ |
| [58](#yaml-58) | 2 letras | `np` | `npm` | 1 | `(no rule)` | ✅ |
| [59](#yaml-59) | 3 letras | `tes` | `test` | 1 | `(no rule)` | ✅ |
| [60](#yaml-60) | Ctrl+Espaço | (nada) | `env` | — | `(no rule)` | ⚠️ |
| [61](#yaml-61) | 1 letra | `t` | `true` | 1 | `(no rule)` | ✅ |
| [62](#yaml-62) | 2 letras | `na` | `name` | 1 | `(no rule)` | ✅ |
| [63](#yaml-63) | 3 letras | `Upl` | `Upload` | — | `(no rule)` | ✅ |
| [64](#yaml-64) | Ctrl+Espaço | (nada) | `coverage` | 20 | `(no rule)` | ⚠️ |
| [65](#yaml-65) | 1 letra | `i` | `if` | — | `(no rule)` | ❌ |
| [66](#yaml-66) | 2 letras | `al` | `always` | — | `(no rule)` | ✅ |
| [67](#yaml-67) | 3 letras | `use` | `uses` | 1 | `(no rule)` | ✅ |
| [68](#yaml-68) | Ctrl+Espaço | (nada) | `upload-artifact` | — | `(no rule)` | ⚠️ |
| [69](#yaml-69) | 1 letra | `w` | `with` | — | `(no rule)` | ✅ |
| [70](#yaml-70) | 2 letras | `na` | `name` | 1 | `(no rule)` | ✅ |
| [71](#yaml-71) | 3 letras | `cov` | `coverage` | 1 | `(no rule)` | ✅ |
| [72](#yaml-72) | Ctrl+Espaço | (nada) | `path` | — | `(no rule)` | ⚠️ |
| [73](#yaml-73) | 1 letra | `c` | `coverage` | 1 | `(no rule)` | ✅ |
| [74](#yaml-74) | 2 letras | `se` | `services` | — | `(no rule)` | ✅ |
| [75](#yaml-75) | 3 letras | `bui` | `build` | — | `(no rule)` | ✅ |
| [76](#yaml-76) | Ctrl+Espaço | (nada) | `api` | 10 | `(no rule)` | ⚠️ |
| [77](#yaml-77) | 1 letra | `p` | `ports` | — | `(no rule)` | ✅ |
| [78](#yaml-78) | 2 letras | `en` | `environment` | — | `(no rule)` | ✅ |
| [79](#yaml-79) | 3 letras | `DAT` | `DATABASE_URL` | — | `(no rule)` | ✅ |
| [80](#yaml-80) | erro: trocadas | `psotgr` | `postgres` | 1 | `(no rule)` | ✅ |
| [81](#yaml-81) | 1 letra | `a` | `app` | — | `(no rule)` | ✅ |
| [82](#yaml-82) | 2 letras | `DE` | `DEBUG` | — | `(no rule)` | ✅ |
| [83](#yaml-83) | 3 letras | `fal` | `false` | 1 | `(no rule)` | ✅ |
| [84](#yaml-84) | Ctrl+Espaço | (nada) | `depends_on` | — | `(no rule)` | ⚠️ |
| [85](#yaml-85) | 1 letra | `d` | `db` | fora | `(no rule)` | ❌ |
| [86](#yaml-86) | 2 letras | `re` | `restart` | — | `(no rule)` | ✅ |
| [87](#yaml-87) | 3 letras | `unl` | `unless-stopped` | — | `(no rule)` | ✅ |
| [88](#yaml-88) | Ctrl+Espaço | (nada) | `image` | — | `(no rule)` | ⚠️ |
| [89](#yaml-89) | 1 letra | `p` | `postgres` | 4 | `(no rule)` | ✅ |
| [90](#yaml-90) | erro: faltando | `voume` | `volumes` | 1 | `(no rule)` | ✅ |
| [91](#yaml-91) | 3 letras | `pgd` | `pgdata` | 1 | `(no rule)` | ✅ |
| [92](#yaml-92) | Ctrl+Espaço | (nada) | `var` | — | `(no rule)` | ⚠️ |
| [93](#yaml-93) | 1 letra | `l` | `lib` | — | `(no rule)` | ✅ |
| [94](#yaml-94) | 2 letras | `po` | `postgresql` | — | `(no rule)` | ✅ |
| [95](#yaml-95) | 3 letras | `hea` | `healthcheck` | — | `(no rule)` | ✅ |
| [96](#yaml-96) | Ctrl+Espaço | (nada) | `test` | — | `(no rule)` | ⚠️ |
| [97](#yaml-97) | 1 letra | `C` | `CMD` | — | `(no rule)` | ✅ |
| [98](#yaml-98) | 2 letras | `pg` | `pg_isready` | — | `(no rule)` | ✅ |
| [99](#yaml-99) | 3 letras | `int` | `interval` | — | `(no rule)` | ✅ |
| [100](#yaml-100) | erro: trocadas | `vloume` | `volumes` | 1 | `(no rule)` | ✅ |

Posição: lugar da palavra procurada na lista; `—` = a palavra não existe em outro lugar do arquivo; `fora` = existe mas não está na lista.

## Os arquivos usados como entrada

Escritos à mão como um desenvolvedor escreveria (código JavaScript com strings da linguagem). Nada foi gerado pelo gerador dos experimentos.

### Documento D1

```js
const deployment = yaml`
apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m
`;
```

### Documento D2

```js
const workflow = yaml`
name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/
`;
```

### Documento D3

```js
const compose = yaml`
services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~
`;
```

## Os exemplos

### YAML-1
<a id="yaml-1"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

a▮: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `apiVersion`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 apps [a]
 2 app [a]
```

**Veredito:** ✅ Bom. `apps`, `app`: palavras que começam com a (a chave não aparece em outro lugar).

---

### YAML-2
<a id="yaml-2"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: ap▮/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `apps`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 apiVersion [a]
 2 app [a]
```

**Veredito:** ✅ Bom. `apiVersion`, `app`.

---

### YAML-3
<a id="yaml-3"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v▮
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `v1`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 value [a]
```

**Veredito:** ✅ Bom. `value` é o único item.

---

### YAML-4
<a id="yaml-4"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
▮: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `kind`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false            11 metadata [a]
 2 no               12 name [a]
 3 null             13 web [a]
 4 off              14 labels [a]
 5 on               15 app [a]
 6 true             16 spec [a]
 7 yes              17 replicas [a]
 8 Deployment [a]   18 selector [a]
 9 apps [a]         19 matchLabels [a]
10 apiVersion [a]   20 template [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: os booleanos do YAML (false, no, null, off, on, true, yes) vêm primeiro, o que não faz sentido numa chave; as palavras mais próximas vêm depois.

---

### YAML-5
<a id="yaml-5"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: D▮
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `Deployment`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 deployment [a]
```

**Veredito:** ✅ Bom. `deployment` (variável do código hospedeiro) é oferecido; o valor `Deployment` não aparece em outro lugar.

---

### YAML-6
<a id="yaml-6"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
me▮:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `metadata`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 metadata [a]
 2 memory [a]
```

**Veredito:** ✅ Bom. `metadata` em primeiro.

---

### YAML-7
<a id="yaml-7"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  nam▮: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `name`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 name [a]
```

**Veredito:** ✅ Bom. `name` é o único item.

---

### YAML-8
<a id="yaml-8"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  ▮:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `labels`: aparece em 22º lugar de 38 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 spec [a]       11 web [a]
 2 template [a]   12 app [a]
 3 image [a]      13 name [a]
 4 false          14 metadata [a]
 5 no             15 replicas [a]
 6 null           16 Deployment [a]
 7 off            17 kind [a]
 8 on             18 selector [a]
 9 true           19 apps [a]
10 yes            20 apiVersion [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: os booleanos do YAML (false, no, null, off, on, true, yes) vêm primeiro, o que não faz sentido numa chave; as palavras mais próximas vêm depois. `labels` é o 22º.

---

### YAML-9
<a id="yaml-9"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    a▮: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `app`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 app [a]
 2 apps [a]
 3 apiVersion [a]
```

**Veredito:** ✅ Bom. `app` em primeiro.

---

### YAML-10
<a id="yaml-10"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: we▮
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `web`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 web [a]
```

**Veredito:** ✅ Bom. `web` é o único item.

---

### YAML-11
<a id="yaml-11"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spe▮:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `spec`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 spec [a]
```

**Veredito:** ✅ Bom. `spec` é o único item.

---

### YAML-12
<a id="yaml-12"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  ▮: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `replicas`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 containers [a]   11 web [a]
 2 false            12 app [a]
 3 no               13 matchLabels [a]
 4 null             14 labels [a]
 5 off              15 name [a]
 6 on               16 metadata [a]
 7 true             17 template [a]
 8 yes              18 Deployment [a]
 9 spec [a]         19 kind [a]
10 selector [a]     20 apps [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: `containers` primeiro, depois os booleanos; a chave não aparece em outro lugar.

---

### YAML-13
<a id="yaml-13"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  s▮:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `selector`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 spec [a]
```

**Veredito:** ✅ Bom. `spec` é oferecido para `s`; a chave não aparece em outro lugar.

---

### YAML-14
<a id="yaml-14"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    ma▮:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `matchLabels`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### YAML-15
<a id="yaml-15"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: we▮
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `web`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 web [a]
```

**Veredito:** ✅ Bom. `web` é o único item.

---

### YAML-16
<a id="yaml-16"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 13 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  ▮:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `template`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 spec [a]     11 web [a]
 2 labels [a]   12 metadata [a]
 3 image [a]    13 app [a]
 4 false        14 matchLabels [a]
 5 no           15 selector [a]
 6 null         16 replicas [a]
 7 off          17 containers [a]
 8 on           18 name [a]
 9 true         19 Deployment [a]
10 yes          20 registry [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: os booleanos do YAML (false, no, null, off, on, true, yes) vêm primeiro, o que não faz sentido numa chave; as palavras mais próximas vêm depois.

---

### YAML-17
<a id="yaml-17"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 14 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    m▮:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `metadata`: aparece em 2º lugar de 3

**Saída** (as 20 primeiras sugestões):

```text
 1 matchLabels [a]
 2 metadata [a]
 3 memory [a]
```

**Veredito:** ✅ Bom. `matchLabels`, `metadata` (2º), `memory`.

---

### YAML-18
<a id="yaml-18"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 15 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      la▮:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `labels`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 labels [a]
```

**Veredito:** ✅ Bom. `labels` é o único item.

---

### YAML-19
<a id="yaml-19"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        ap▮: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `app`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 app [a]
 2 apps [a]
 3 apiVersion [a]
```

**Veredito:** ✅ Bom. `app` em primeiro.

---

### YAML-20
<a id="yaml-20"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: ▮
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `web`: aparece em 2º lugar de 38

**Saída** (as 20 primeiras sugestões):

```text
 1 spec [a]   11 labels [a]
 2 web [a]    12 containers [a]
 3 false      13 metadata [a]
 4 no         14 name [a]
 5 null       15 template [a]
 6 off        16 image [a]
 7 on         17 registry [a]
 8 true       18 matchLabels [a]
 9 yes        19 example [a]
10 app [a]    20 com [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito num valor: os booleanos do YAML vêm antes das palavras do arquivo. `web` é o 2º.

---

### YAML-21
<a id="yaml-21"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 18 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      c▮:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `containers`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 com [a]
 2 containerPort [a]
 3 cpu [a]
 4 const [a]
```

**Veredito:** ✅ Bom. Palavras que começam com c.

---

### YAML-22
<a id="yaml-22"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 19 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - na▮: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `name`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 name [a]
```

**Veredito:** ✅ Bom. `name` é o único item.

---

### YAML-23
<a id="yaml-23"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 19 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: we▮
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `web`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 web [a]
```

**Veredito:** ✅ Bom. `web` é o único item.

---

### YAML-24
<a id="yaml-24"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 20 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          ▮: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `image`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 spec [a]       11 registry [a]
 2 template [a]   12 example [a]
 3 labels [a]     13 web [a]
 4 false          14 name [a]
 5 no             15 com [a]
 6 null           16 containers [a]
 7 off            17 ports [a]
 8 on             18 app [a]
 9 true           19 containerPort [a]
10 yes            20 metadata [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: os booleanos do YAML (false, no, null, off, on, true, yes) vêm primeiro, o que não faz sentido numa chave; as palavras mais próximas vêm depois.

---

### YAML-25
<a id="yaml-25"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 20 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: r▮.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `registry`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 resources [a]
 2 replicas [a]
```

**Veredito:** ✅ Bom. `resources`, `replicas`.

---

### YAML-26
<a id="yaml-26"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 20 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.ex▮.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `example`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### YAML-27
<a id="yaml-27"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 20 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.co▮/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `com`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 containerPort [a]
 2 containers [a]
 3 const [a]
```

**Veredito:** ✅ Bom. Palavras que começam com co.

---

### YAML-28
<a id="yaml-28"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 21 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ▮:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `ports`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false               11 example [a]
 2 no                  12 registry [a]
 3 null                13 image [a]
 4 off                 14 env [a]
 5 on                  15 name [a]
 6 true                16 LOG_LEVEL [a]
 7 yes                 17 containers [a]
 8 containerPort [a]   18 value [a]
 9 web [a]             19 spec [a]
10 com [a]             20 info [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: os booleanos do YAML (false, no, null, off, on, true, yes) vêm primeiro, o que não faz sentido numa chave; as palavras mais próximas vêm depois.

---

### YAML-29
<a id="yaml-29"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 22 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - c▮: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `containerPort`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 com [a]
 2 containers [a]
 3 cpu [a]
 4 const [a]
```

**Veredito:** ✅ Bom. Palavras que começam com c.

---

### YAML-30
<a id="yaml-30"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 23 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          en▮:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `env`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### YAML-31
<a id="yaml-31"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 24 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - nam▮: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `name`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 name [a]
```

**Veredito:** ✅ Bom. `name` é o único item.

---

### YAML-32
<a id="yaml-32"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 24 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: ▮
              value: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `LOG_LEVEL`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 value [a]   11 env [a]
 2 web [a]     12 info [a]
 3 false       13 resources [a]
 4 no          14 containerPort [a]
 5 null        15 limits [a]
 6 off         16 ports [a]
 7 on          17 memory [a]
 8 true        18 com [a]
 9 yes         19 example [a]
10 name [a]    20 registry [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito num valor: os booleanos do YAML vêm antes das palavras do arquivo.

---

### YAML-33
<a id="yaml-33"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 25 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              v▮: info
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `value`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### YAML-34
<a id="yaml-34"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 25 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: in▮
          resources:
            limits:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `info`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### YAML-35
<a id="yaml-35"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 27 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            lim▮:
              memory: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `limits`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### YAML-36
<a id="yaml-36"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 28 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              ▮: 256Mi
              cpu: 500m

```

**Palavra que a pessoa ia digitar:** `memory`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false           11 info [a]
 2 no              12 value [a]
 3 null            13 LOG_LEVEL [a]
 4 off             14 name [a]
 5 on              15 env [a]
 6 true            16 containerPort [a]
 7 yes             17 ports [a]
 8 limits [a]      18 web [a]
 9 cpu [a]         19 com [a]
10 resources [a]   20 example [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: os booleanos do YAML (false, no, null, off, on, true, yes) vêm primeiro, o que não faz sentido numa chave; as palavras mais próximas vêm depois.

---

### YAML-37
<a id="yaml-37"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 29 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

apiVersion: apps/v1
kind: Deployment
metadata:
  name: web
  labels:
    app: web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: web
  template:
    metadata:
      labels:
        app: web
    spec:
      containers:
        - name: web
          image: registry.example.com/web:1.4.2
          ports:
            - containerPort: 8080
          env:
            - name: LOG_LEVEL
              value: info
          resources:
            limits:
              memory: 256Mi
              c▮: 500m

```

**Palavra que a pessoa ia digitar:** `cpu`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 containerPort [a]
 2 com [a]
 3 containers [a]
 4 const [a]
```

**Veredito:** ✅ Bom. Palavras que começam com c.

---

### YAML-38
<a id="yaml-38"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

na▮: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `name`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 name [a]
```

**Veredito:** ✅ Bom. `name` é o único item.

---

### YAML-39
<a id="yaml-39"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: c▮
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `ci`: **não aparece** na lista (3 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 coverage [a]
 2 checkout [a]
 3 const [a]
```

**Veredito:** ❌ Ruim. `name: c`: `ci` nunca pode ser sugerido (palavras com menos de 3 letras são ignoradas).

---

### YAML-40
<a id="yaml-40"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
▮:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `on`: aparece em 5º lugar de 34

**Saída** (as 20 primeiras sugestões):

```text
 1 false          11 main [a]
 2 no             12 pull_request [a]
 3 null           13 jobs [a]
 4 off            14 build [a]
 5 on             15 runs-on [a]
 6 true           16 ubuntu-latest [a]
 7 yes            17 steps [a]
 8 push [a]       18 uses [a]
 9 name [a]       19 actions [a]
10 branches [a]   20 checkout [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: os booleanos do YAML (false, no, null, off, on, true, yes) vêm primeiro, o que não faz sentido numa chave; as palavras mais próximas vêm depois. `on` é o 5º.

---

### YAML-41
<a id="yaml-41"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    b▮: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `branches`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 build [a]
```

**Veredito:** ✅ Bom. `build` é oferecido para `b`.

---

### YAML-42
<a id="yaml-42"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [ma▮]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `main`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### YAML-43
<a id="yaml-43"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pul▮:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `pull_request`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 push [~]
```

**Veredito:** ✅ Bom. `push~` é uma chave irmã plausível.

---

### YAML-44
<a id="yaml-44"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
▮:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `jobs`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false              11 main [a]
 2 no                 12 ubuntu-latest [a]
 3 null               13 branches [a]
 4 off                14 push [a]
 5 on                 15 steps [a]
 6 true               16 name [a]
 7 yes                17 uses [a]
 8 pull_request [a]   18 actions [a]
 9 build [a]          19 checkout [a]
10 runs-on [a]        20 Install [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: os booleanos do YAML (false, no, null, off, on, true, yes) vêm primeiro, o que não faz sentido numa chave; as palavras mais próximas vêm depois.

---

### YAML-45
<a id="yaml-45"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  b▮:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `build`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 branches [a]
```

**Veredito:** ✅ Bom. `branches` é oferecido para `b`.

---

### YAML-46
<a id="yaml-46"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    ru▮: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `runs-on`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 run [a]
```

**Veredito:** ✅ Bom. `run` é oferecido para `ru`.

---

### YAML-47
<a id="yaml-47"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubu▮
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `ubuntu-latest`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### YAML-48
<a id="yaml-48"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - ▮: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `uses`: aparece em 28º lugar de 34 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 name [a]       11 steps [a]
 2 false          12 ubuntu-latest [a]
 3 no             13 runs-on [a]
 4 null           14 Install [a]
 5 off            15 build [a]
 6 on             16 run [a]
 7 true           17 jobs [a]
 8 yes            18 npm [a]
 9 actions [a]    19 pull_request [a]
10 checkout [a]   20 Test [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: os booleanos do YAML (false, no, null, off, on, true, yes) vêm primeiro, o que não faz sentido numa chave; as palavras mais próximas vêm depois. `uses` é o 28º.

---

### YAML-49
<a id="yaml-49"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: a▮/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `actions`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 actions [a]
 2 always [a]
```

**Veredito:** ✅ Bom. `actions`, `always`.

---

### YAML-50
<a id="yaml-50"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D2, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/chcko▮@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `checkout`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### YAML-51
<a id="yaml-51"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - nam▮: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `name`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 name [a]
```

**Veredito:** ✅ Bom. `name` é o único item.

---

### YAML-52
<a id="yaml-52"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: ▮
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `Install`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 run [a]        11 true
 2 Test [a]       12 yes
 3 Upload [a]     13 name [a]
 4 ci [a]         14 npm [a]
 5 coverage [a]   15 checkout [a]
 6 false          16 actions [a]
 7 no             17 uses [a]
 8 null           18 steps [a]
 9 off            19 ubuntu-latest [a]
10 on             20 env [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito num valor: os booleanos do YAML vêm antes das palavras do arquivo.

---

### YAML-53
<a id="yaml-53"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 13 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        r▮: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `run`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 run [a]
 2 runs-on [a]
```

**Veredito:** ✅ Bom. `run`, `runs-on`.

---

### YAML-54
<a id="yaml-54"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 13 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: np▮ ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `npm`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 npm [a]
```

**Veredito:** ✅ Bom. `npm` é o único item.

---

### YAML-55
<a id="yaml-55"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 14 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - nam▮: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `name`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 name [a]
```

**Veredito:** ✅ Bom. `name` é o único item.

---

### YAML-56
<a id="yaml-56"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 14 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: ▮
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `Test`: aparece em 15º lugar de 35

**Saída** (as 20 primeiras sugestões):

```text
 1 run [a]        11 true
 2 Install [a]    12 yes
 3 Upload [a]     13 name [a]
 4 coverage [a]   14 npm [a]
 5 ci [a]         15 test [a]
 6 false          16 env [a]
 7 no             17 checkout [a]
 8 null           18 actions [a]
 9 off            19 uses [a]
10 on             20 steps [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito num valor: os booleanos do YAML vêm antes das palavras do arquivo. `Test` é o 15º.

---

### YAML-57
<a id="yaml-57"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 15 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        r▮: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `run`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 run [a]
 2 runs-on [a]
```

**Veredito:** ✅ Bom. `run`, `runs-on`.

---

### YAML-58
<a id="yaml-58"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 15 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: np▮ test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `npm`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 npm [a]
```

**Veredito:** ✅ Bom. `npm` é o único item.

---

### YAML-59
<a id="yaml-59"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 15 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm tes▮
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `test`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 Test [a]
```

**Veredito:** ✅ Bom. `Test` é o único item.

---

### YAML-60
<a id="yaml-60"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        ▮:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `env`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 run [a]    11 name [a]
 2 false      12 Upload [a]
 3 no         13 coverage [a]
 4 null       14 always [a]
 5 off        15 Install [a]
 6 on         16 uses [a]
 7 true       17 actions [a]
 8 yes        18 upload-artifact [a]
 9 test [a]   19 checkout [a]
10 npm [a]    20 with [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: os booleanos do YAML (false, no, null, off, on, true, yes) vêm primeiro, o que não faz sentido numa chave; as palavras mais próximas vêm depois.

---

### YAML-61
<a id="yaml-61"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: t▮
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `true`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 true
 2 test [a]
```

**Veredito:** ✅ Bom. `true` em primeiro.

---

### YAML-62
<a id="yaml-62"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 18 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - na▮: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `name`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 name [a]
```

**Veredito:** ✅ Bom. `name` é o único item.

---

### YAML-63
<a id="yaml-63"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 18 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upl▮ coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `Upload`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 upload-artifact [a]
```

**Veredito:** ✅ Bom. `upload-artifact` é oferecido.

---

### YAML-64
<a id="yaml-64"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 18 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload ▮
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `coverage`: aparece em 20º lugar de 35

**Saída** (as 20 primeiras sugestões):

```text
 1 if [a]       11 always [a]
 2 false        12 uses [a]
 3 no           13 actions [a]
 4 null         14 env [a]
 5 off          15 upload-artifact [a]
 6 on           16 test [a]
 7 true         17 npm [a]
 8 yes          18 run [a]
 9 Upload [a]   19 with [a]
10 name [a]     20 coverage [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito num valor: os booleanos do YAML vêm antes das palavras do arquivo. `coverage` é o 20º.

---

### YAML-65
<a id="yaml-65"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 19 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        i▮: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `if`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 Install [a]
```

**Veredito:** ❌ Ruim. `i`: `if` nunca pode ser sugerido (menos de 3 letras); `Install` é oferecido no lugar.

---

### YAML-66
<a id="yaml-66"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 19 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: al▮()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `always`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### YAML-67
<a id="yaml-67"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 20 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        use▮: actions/upload-artifact@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `uses`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 uses [a]
```

**Veredito:** ✅ Bom. `uses` é o único item.

---

### YAML-68
<a id="yaml-68"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 20 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/▮@v4
        with:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `upload-artifact`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 checkout [a]   11 with [a]
 2 false          12 always [a]
 3 no             13 name [a]
 4 null           14 coverage [a]
 5 off            15 Upload [a]
 6 on             16 path [a]
 7 true           17 env [a]
 8 yes            18 test [a]
 9 actions [a]    19 npm [a]
10 uses [a]       20 run [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito num valor: os booleanos do YAML vêm antes das palavras do arquivo.

---

### YAML-69
<a id="yaml-69"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 21 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        w▮:
          name: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `with`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 workflow [a]
```

**Veredito:** ✅ Bom. `workflow` é oferecido para `w`; a chave `with` não aparece em outro lugar.

---

### YAML-70
<a id="yaml-70"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 22 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          na▮: coverage
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `name`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 name [a]
```

**Veredito:** ✅ Bom. `name` é o único item.

---

### YAML-71
<a id="yaml-71"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 22 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: cov▮
          path: coverage/

```

**Palavra que a pessoa ia digitar:** `coverage`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 coverage [a]
```

**Veredito:** ✅ Bom. `coverage` é o único item.

---

### YAML-72
<a id="yaml-72"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 23 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          ▮: coverage/

```

**Palavra que a pessoa ia digitar:** `path`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 if [a]         11 with [a]
 2 false          12 upload-artifact [a]
 3 no             13 actions [a]
 4 null           14 uses [a]
 5 off            15 always [a]
 6 on             16 Upload [a]
 7 true           17 env [a]
 8 yes            18 test [a]
 9 coverage [a]   19 npm [a]
10 name [a]       20 run [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: os booleanos do YAML (false, no, null, off, on, true, yes) vêm primeiro, o que não faz sentido numa chave; as palavras mais próximas vêm depois.

---

### YAML-73
<a id="yaml-73"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 23 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

name: ci
on:
  push:
    branches: [main]
  pull_request:
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install
        run: npm ci
      - name: Test
        run: npm test
        env:
          CI: true
      - name: Upload coverage
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: c▮/

```

**Palavra que a pessoa ia digitar:** `coverage`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 coverage [a]
 2 checkout [a]
 3 const [a]
```

**Veredito:** ✅ Bom. `coverage` em primeiro.

---

### YAML-74
<a id="yaml-74"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

se▮:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `services`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### YAML-75
<a id="yaml-75"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    bui▮: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `build`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### YAML-76
<a id="yaml-76"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./▮
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `api`: aparece em 10º lugar de 34

**Saída** (as 20 primeiras sugestões):

```text
 1 ports [a]   11 services [a]
 2 false       12 environment [a]
 3 no          13 DATABASE_URL [a]
 4 null        14 postgres [a]
 5 off         15 app [a]
 6 on          16 DEBUG [a]
 7 true        17 depends_on [a]
 8 yes         18 restart [a]
 9 build [a]   19 unless-stopped [a]
10 api [a]     20 image [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito num valor: os booleanos do YAML vêm antes das palavras do arquivo. `api` é o 10º.

---

### YAML-77
<a id="yaml-77"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    p▮:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `ports`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 postgres [a]
 2 pgdata [a]
 3 postgresql [a]
 4 pg_isready [a]
```

**Veredito:** ✅ Bom. Palavras que começam com p (a chave `ports` não aparece em outro lugar).

---

### YAML-78
<a id="yaml-78"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    en▮:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `environment`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### YAML-79
<a id="yaml-79"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DAT▮: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `DATABASE_URL`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 data [a]
```

**Veredito:** ✅ Bom. `data` é oferecido para `DAT`.

---

### YAML-80
<a id="yaml-80"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D3, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: psotgr▮://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `postgres`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 postgres [~]
 2 postgresql [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `postgres~` em primeiro.

---

### YAML-81
<a id="yaml-81"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/a▮
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `app`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 api [a]
```

**Veredito:** ✅ Bom. `api` é oferecido para `a`.

---

### YAML-82
<a id="yaml-82"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DE▮: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `DEBUG`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 depends_on [a]
```

**Veredito:** ✅ Bom. `depends_on` é oferecido para `DE`.

---

### YAML-83
<a id="yaml-83"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: fal▮
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `false`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 false
```

**Veredito:** ✅ Bom. `false` é o único item.

---

### YAML-84
<a id="yaml-84"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    ▮:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `depends_on`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false         11 unless-stopped [a]
 2 no            12 postgres [a]
 3 null          13 DATABASE_URL [a]
 4 off           14 image [a]
 5 on            15 environment [a]
 6 true          16 volumes [a]
 7 yes           17 pgdata [a]
 8 DEBUG [a]     18 var [a]
 9 restart [a]   19 lib [a]
10 app [a]       20 ports [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: os booleanos do YAML (false, no, null, off, on, true, yes) vêm primeiro, o que não faz sentido numa chave; as palavras mais próximas vêm depois.

---

### YAML-85
<a id="yaml-85"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - d▮
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `db`: **não aparece** na lista (4 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 depends_on [a]
 2 DEBUG [a]
 3 DATABASE_URL [a]
 4 data [a]
```

**Veredito:** ❌ Ruim. `- d`: `db` nunca pode ser sugerido (palavras com menos de 3 letras são ignoradas).

---

### YAML-86
<a id="yaml-86"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    re▮: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `restart`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### YAML-87
<a id="yaml-87"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unl▮
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `unless-stopped`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### YAML-88
<a id="yaml-88"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 14 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    ▮: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `image`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false                11 restart [a]
 2 no                   12 pgdata [a]
 3 null                 13 var [a]
 4 off                  14 lib [a]
 5 on                   15 depends_on [a]
 6 true                 16 postgresql [a]
 7 yes                  17 data [a]
 8 postgres [a]         18 healthcheck [a]
 9 unless-stopped [a]   19 DEBUG [a]
10 volumes [a]          20 app [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: os booleanos do YAML (false, no, null, off, on, true, yes) vêm primeiro, o que não faz sentido numa chave; as palavras mais próximas vêm depois.

---

### YAML-89
<a id="yaml-89"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 14 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: p▮:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `postgres`: aparece em 4º lugar de 5

**Saída** (as 20 primeiras sugestões):

```text
 1 pgdata [a]
 2 postgresql [a]
 3 pg_isready [a]
 4 postgres [a]
 5 ports [a]
```

**Veredito:** ✅ Bom. `postgres` é o 4º de 5 (ordem alfabética de proximidade).

---

### YAML-90
<a id="yaml-90"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D3, linha 15 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    voume▮:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `volumes`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 volumes [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `volumes~`.

---

### YAML-91
<a id="yaml-91"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgd▮:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `pgdata`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 pgdata [a]
```

**Veredito:** ✅ Bom. `pgdata` é o único item.

---

### YAML-92
<a id="yaml-92"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/▮/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `var`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false            11 data [a]
 2 no               12 volumes [a]
 3 null             13 healthcheck [a]
 4 off              14 postgres [a]
 5 on               15 image [a]
 6 true             16 test [a]
 7 yes              17 CMD [a]
 8 lib [a]          18 pg_isready [a]
 9 pgdata [a]       19 unless-stopped [a]
10 postgresql [a]   20 restart [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: os booleanos do YAML (false, no, null, off, on, true, yes) vêm primeiro, o que não faz sentido numa chave; as palavras mais próximas vêm depois.

---

### YAML-93
<a id="yaml-93"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/l▮/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `lib`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### YAML-94
<a id="yaml-94"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/po▮/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `postgresql`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 postgres [a]
 2 ports [a]
```

**Veredito:** ✅ Bom. `postgres`, `ports` (`postgresql` não aparece em outro lugar).

---

### YAML-95
<a id="yaml-95"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    hea▮:
      test: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `healthcheck`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### YAML-96
<a id="yaml-96"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 18 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      ▮: ["CMD", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `test`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 false             11 data [a]
 2 no                12 postgresql [a]
 3 null              13 interval [a]
 4 off               14 lib [a]
 5 on                15 var [a]
 6 true              16 volumes [a]
 7 yes               17 pgdata [a]
 8 CMD [a]           18 postgres [a]
 9 healthcheck [a]   19 image [a]
10 pg_isready [a]    20 unless-stopped [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma chave: os booleanos do YAML (false, no, null, off, on, true, yes) vêm primeiro, o que não faz sentido numa chave; as palavras mais próximas vêm depois.

---

### YAML-97
<a id="yaml-97"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 18 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["C▮", "pg_isready"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `CMD`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 compose [a]
 2 const [a]
```

**Veredito:** ✅ Bom. `compose`, `const`: palavras que começam com C (do código hospedeiro).

---

### YAML-98
<a id="yaml-98"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 18 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg▮"]
      interval: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `pg_isready`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 pgdata [a]
```

**Veredito:** ✅ Bom. `pgdata` é oferecido para `pg`.

---

### YAML-99
<a id="yaml-99"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 19 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      int▮: 10s
volumes:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `interval`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---

### YAML-100
<a id="yaml-100"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D3, linha 20 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

services:
  api:
    build: ./api
    ports:
      - "3000:3000"
    environment:
      DATABASE_URL: postgres://db:5432/app
      DEBUG: false
    depends_on:
      - db
    restart: unless-stopped
  db:
    image: postgres:16
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD", "pg_isready"]
      interval: 10s
vloume▮:
  pgdata: ~

```

**Palavra que a pessoa ia digitar:** `volumes`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 volumes [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `volumes~`.

---
