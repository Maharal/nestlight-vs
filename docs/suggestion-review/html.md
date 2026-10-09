# HTML: 100 exemplos

Resultado: ✅ 72 bons · ⚠️ 28 razoáveis com ressalva · ❌ 0 ruins.

Como ler: em cada exemplo, `▮` marca onde está o cursor. A lista é o que o plugin mostraria (as 20 primeiras). `[a]` = palavra que já existe no arquivo; `[~]` = sugestão "parecida" (corrige erro de digitação); sem marca = palavra-chave da linguagem. O veredito e o comentário são a minha análise. "Lugar na gramática" é o nome interno da regra de posição que o plugin aplicou (`sql:table`, `css:value:display`...); `(no rule)` quer dizer que o plugin não tem regra para aquele lugar e usa só o que foi digitado.

## Índice (para varrer rápido)

| # | Situação | Digitado | Palavra procurada | Posição | Lugar na gramática | Veredito |
|---|---|---|---|---|---|---|
| [1](#html-1) | 1 letra | `f` | `form` | 6 | `html:tag` | ✅ |
| [2](#html-2) | 2 letras | `lo` | `login` | 3 | `html:value:class` | ✅ |
| [3](#html-3) | 3 letras | `met` | `method` | 1 | `html:attribute:form` | ✅ |
| [4](#html-4) | Ctrl+Espaço | (nada) | `h2` | 45 | `html:tag` | ⚠️ |
| [5](#html-5) | 1 letra | `S` | `Sign` | 2 | `html:text` | ✅ |
| [6](#html-6) | 1 letra | `h` | `h2` | 2 | `html:tag` | ✅ |
| [7](#html-7) | 3 letras | `ema` | `email` | 1 | `html:value:for` | ✅ |
| [8](#html-8) | Ctrl+Espaço | (nada) | `label` | 60 | `html:tag` | ⚠️ |
| [9](#html-9) | 1 letra | `e` | `email` | 1 | `html:value:type` | ✅ |
| [10](#html-10) | erro: faltando | `emi` | `email` | 3 | `html:value:id` | ✅ |
| [11](#html-11) | 3 letras | `pla` | `placeholder` | 1 | `html:attribute:input` | ✅ |
| [12](#html-12) | Ctrl+Espaço | (nada) | `com` | — | `html:value:placeholder` | ✅ |
| [13](#html-13) | 1 letra | `l` | `label` | 1 | `html:tag` | ✅ |
| [14](#html-14) | 2 letras | `pa` | `password` | 1 | `html:value:for` | ✅ |
| [15](#html-15) | 3 letras | `inp` | `input` | 1 | `html:tag` | ✅ |
| [16](#html-16) | Ctrl+Espaço | (nada) | `password` | 4 | `html:value:type` | ✅ |
| [17](#html-17) | 1 letra | `n` | `name` | 1 | `html:attribute:input` | ✅ |
| [18](#html-18) | 2 letras | `mi` | `minlength` | — | `html:attribute:input` | ✅ |
| [19](#html-19) | 3 letras | `typ` | `type` | 1 | `html:attribute:button` | ✅ |
| [20](#html-20) | erro: trocadas | `cals` | `class` | 1 | `html:attribute:button` | ✅ |
| [21](#html-21) | 1 letra | `d` | `disabled` | 1 | `html:attribute:button` | ✅ |
| [22](#html-22) | 1 letra | `i` | `in` | 1 | `html:text` | ✅ |
| [23](#html-23) | 3 letras | `for` | `forgot` | 1 | `html:value:href` | ✅ |
| [24](#html-24) | Ctrl+Espaço | (nada) | `login__forgot` | — | `html:value:class` | ⚠️ |
| [25](#html-25) | 1 letra | `r` | `rel` | 1 | `html:attribute:a` | ✅ |
| [26](#html-26) | 2 letras | `Fo` | `Forgot` | 2 | `html:text` | ✅ |
| [27](#html-27) | 3 letras | `for` | `form` | 1 | `html:tag` | ✅ |
| [28](#html-28) | Ctrl+Espaço | (nada) | `class` | 1 | `html:attribute:table` | ✅ |
| [29](#html-29) | 1 letra | `t` | `tr` | 12 | `html:tag` | ⚠️ |
| [30](#html-30) | erro: faltando | `scp` | `scope` | 4 | `html:attribute:th` | ⚠️ |
| [31](#html-31) | 1 letra | `t` | `th` | 8 | `html:tag` | ⚠️ |
| [32](#html-32) | Ctrl+Espaço | (nada) | `scope` | 1 | `html:attribute:th` | ✅ |
| [33](#html-33) | 1 letra | `t` | `th` | 8 | `html:tag` | ⚠️ |
| [34](#html-34) | 2 letras | `co` | `colspan` | 1 | `html:attribute:th` | ✅ |
| [35](#html-35) | 1 letra | `t` | `tr` | 12 | `html:tag` | ⚠️ |
| [36](#html-36) | Ctrl+Espaço | (nada) | `tbody` | fora | `html:tag` | ⚠️ |
| [37](#html-37) | 1 letra | `t` | `tr` | 12 | `html:tag` | ⚠️ |
| [38](#html-38) | 1 letra | `t` | `td` | 3 | `html:tag` | ✅ |
| [39](#html-39) | 1 letra | `t` | `td` | 1 | `html:tag` | ✅ |
| [40](#html-40) | erro: trocadas | `cals` | `class` | 2 | `html:attribute:button` | ⚠️ |
| [41](#html-41) | 1 letra | `b` | `button` | 5 | `html:tag` | ✅ |
| [42](#html-42) | 1 letra | `t` | `tr` | 12 | `html:tag` | ⚠️ |
| [43](#html-43) | 3 letras | `sit` | `site-header` | — | `html:value:class` | ✅ |
| [44](#html-44) | Ctrl+Espaço | (nada) | `class` | 1 | `html:attribute:nav` | ✅ |
| [45](#html-45) | 1 letra | `M` | `Main` | 1 | `html:value:aria-label` | ⚠️ |
| [46](#html-46) | 2 letras | `cl` | `class` | 1 | `html:attribute:ul` | ✅ |
| [47](#html-47) | 3 letras | `cla` | `class` | 1 | `html:attribute:li` | ✅ |
| [48](#html-48) | Ctrl+Espaço | (nada) | `href` | 1 | `html:attribute:a` | ✅ |
| [49](#html-49) | 1 letra | `l` | `li` | 1 | `html:tag` | ✅ |
| [50](#html-50) | erro: faltando | `sie-n` | `site-nav__item` | 1 | `html:value:class` | ✅ |
| [51](#html-51) | 3 letras | `cla` | `class` | 1 | `html:attribute:a` | ✅ |
| [52](#html-52) | Ctrl+Espaço | (nada) | `Docs` | 3 | `html:text` | ⚠️ |
| [53](#html-53) | 1 letra | `c` | `class` | 1 | `html:attribute:li` | ✅ |
| [54](#html-54) | 2 letras | `hr` | `href` | 1 | `html:attribute:a` | ✅ |
| [55](#html-55) | 1 letra | `l` | `li` | 1 | `html:tag` | ✅ |
| [56](#html-56) | Ctrl+Espaço | (nada) | `nav` | 73 | `html:tag` | ⚠️ |
| [57](#html-57) | 1 letra | `l` | `logo` | 1 | `html:value:src` | ✅ |
| [58](#html-58) | 2 letras | `al` | `alt` | 1 | `html:attribute:img` | ✅ |
| [59](#html-59) | 3 letras | `hei` | `height` | 1 | `html:attribute:img` | ✅ |
| [60](#html-60) | Ctrl+Espaço | (nada) | `lazy` | 1 | `html:value:loading` | ✅ |
| [61](#html-61) | 1 letra | `c` | `class` | 1 | `html:attribute:section` | ✅ |
| [62](#html-62) | 1 letra | `i` | `id` | 1 | `html:attribute:section` | ✅ |
| [63](#html-63) | 3 letras | `reg` | `region` | — | `html:value:role` | ⚠️ |
| [64](#html-64) | Ctrl+Espaço | (nada) | `h3` | 45 | `html:tag` | ⚠️ |
| [65](#html-65) | 1 letra | `h` | `h3` | 3 | `html:tag` | ✅ |
| [66](#html-66) | 2 letras | `ca` | `card__body` | — | `html:value:class` | ✅ |
| [67](#html-67) | 3 letras | `not` | `note` | 1 | `html:value:name` | ✅ |
| [68](#html-68) | Ctrl+Espaço | (nada) | `cols` | 3 | `html:attribute:textarea` | ⚠️ |
| [69](#html-69) | 1 letra | `n` | `note` | 2 | `html:value:placeholder` | ✅ |
| [70](#html-70) | erro: faltando | `seec` | `select` | 2 | `html:tag` | ✅ |
| [71](#html-71) | 3 letras | `cla` | `class` | 1 | `html:attribute:select` | ✅ |
| [72](#html-72) | Ctrl+Espaço | (nada) | `option` | 79 | `html:tag` | ⚠️ |
| [73](#html-73) | 1 letra | `s` | `selected` | 1 | `html:attribute:option` | ✅ |
| [74](#html-74) | 2 letras | `op` | `option` | 2 | `html:tag` | ✅ |
| [75](#html-75) | 3 letras | `blu` | `blue` | 1 | `html:value:value` | ✅ |
| [76](#html-76) | Ctrl+Espaço | (nada) | `option` | 78 | `html:tag` | ⚠️ |
| [77](#html-77) | 1 letra | `s` | `src` | 1 | `html:attribute:video` | ✅ |
| [78](#html-78) | 2 letras | `mp` | `mp4` | — | `html:value:src` | ✅ |
| [79](#html-79) | 3 letras | `mut` | `muted` | 1 | `html:attribute:video` | ✅ |
| [80](#html-80) | erro: trocadas | `scetio` | `section` | 1 | `html:tag` | ✅ |
| [81](#html-81) | 1 letra | `h` | `http` | — | `html:value:xmlns` | ⚠️ |
| [82](#html-82) | 1 letra | `w` | `w3` | — | `html:value:xmlns` | ⚠️ |
| [83](#html-83) | 3 letras | `wid` | `width` | 1 | `html:attribute:svg` | ✅ |
| [84](#html-84) | Ctrl+Espaço | (nada) | `fill` | 5 | `html:attribute:svg` | ⚠️ |
| [85](#html-85) | 1 letra | `c` | `currentColor` | — | `html:value:stroke` | ⚠️ |
| [86](#html-86) | 1 letra | `c` | `cx` | 1 | `html:attribute:circle` | ✅ |
| [87](#html-87) | 2 letras | `M1` | `M12` | — | `html:value:d` | ✅ |
| [88](#html-88) | Ctrl+Espaço | (nada) | `rect` | 89 | `html:tag` | ⚠️ |
| [89](#html-89) | 1 letra | `r` | `rx` | 1 | `html:attribute:rect` | ✅ |
| [90](#html-90) | 1 letra | `x` | `x1` | 1 | `html:attribute:line` | ✅ |
| [91](#html-91) | 1 letra | `y` | `y2` | 2 | `html:attribute:line` | ✅ |
| [92](#html-92) | Ctrl+Espaço | (nada) | `dialog` | 29 | `html:tag` | ⚠️ |
| [93](#html-93) | 1 letra | `c` | `class` | 1 | `html:attribute:dialog` | ✅ |
| [94](#html-94) | 2 letras | `Ar` | `Are` | — | `html:text` | ⚠️ |
| [95](#html-95) | 3 letras | `but` | `button` | 1 | `html:tag` | ✅ |
| [96](#html-96) | Ctrl+Espaço | (nada) | `dialog__ok` | — | `html:value:class` | ✅ |
| [97](#html-97) | 1 letra | `O` | `OK` | — | `html:text` | ⚠️ |
| [98](#html-98) | 2 letras | `bu` | `button` | 1 | `html:tag` | ✅ |
| [99](#html-99) | 3 letras | `typ` | `type` | 1 | `html:attribute:button` | ✅ |
| [100](#html-100) | erro: trocadas | `Cnace` | `Cancel` | — | `html:text` | ✅ |

Posição: lugar da palavra procurada na lista; `—` = a palavra não existe em outro lugar do arquivo; `fora` = existe mas não está na lista.

## Os arquivos usados como entrada

Escritos à mão como um desenvolvedor escreveria (código JavaScript com strings da linguagem). Nada foi gerado pelo gerador dos experimentos.

### Documento D1

```js
export const loginForm = html`
  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>
`;
```

### Documento D2

```js
export function table(rows) {
  return html`
    <table class="data-table">
      <thead>
        <tr><th scope="col">Name</th><th scope="col">Role</th><th colspan="2">Actions</th></tr>
      </thead>
      <tbody>
        ${rows.map(r => html`<tr><td>${r.name}</td><td>${r.role}</td><td><button class="edit">Edit</button></td></tr>`)}
      </tbody>
    </table>
  `;
}
```

### Documento D3

```js
const header = html`
  <header class="site-header">
    <nav class="site-nav" aria-label="Main">
      <ul class="site-nav__list">
        <li class="site-nav__item"><a href="/">Home</a></li>
        <li class="site-nav__item"><a href="/docs" class="active">Docs</a></li>
        <li class="site-nav__item"><a href="/blog">Blog</a></li>
      </ul>
    </nav>
    <img src="/logo.svg" alt="Logo" width="120" height="32" loading="lazy">
  </header>
`;
```

### Documento D4

```js
const card = html`
  <section class="card" id="featured" role="region" tabindex="0">
    <h3 class="card__title">${title}</h3>
    <p class="card__body">${body}</p>
    <textarea name="note" rows="3" cols="40" placeholder="Add a note"></textarea>
    <select name="color" class="card__select">
      <option value="red" selected>Red</option>
      <option value="blue">Blue</option>
    </select>
    <video src="/intro.mp4" controls autoplay muted></video>
  </section>
`;
```

### Documento D5

```js
const icon = svg`
  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24" fill="none" stroke="currentColor">
    <circle cx="12" cy="12" r="10"/>
    <path d="M12 6v6l4 2" stroke-width="2"/>
    <rect x="3" y="3" width="18" height="18" rx="2"/>
    <line x1="4" y1="4" x2="20" y2="20"/>
  </svg>
`;

const dialog = html`
  <dialog id="confirm" class="dialog">
    <p>Are you sure?</p>
    <button class="dialog__ok" type="button">OK</button>
    <button class="dialog__cancel" type="button">Cancel</button>
  </dialog>
`;
```

## Os exemplos

### HTML-1
<a id="html-1"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 2 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <f▮ class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `form`: aparece em 6º lugar de 8

**Saída** (as 20 primeiras sugestões):

```text
 1 fieldset
 2 figcaption
 3 figure
 4 filter
 5 footer
 6 form
 7 for [a]
 8 forgot [a]
```

**Veredito:** ✅ Bom. `form` é a 6ª entre as tags f..., ou seja, ordem alfabética.

---

### HTML-2
<a id="html-2"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 2 · **lugar na gramática:** `html:value:class`

**Entrada** (a string onde está o cursor):

```text

  <form class="lo▮" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `login`: aparece em 3º lugar de 4

**Saída** (as 20 primeiras sugestões):

```text
 1 login__title [a]
 2 login__forgot [a]
 3 login [a]
 4 loginForm [a]
```

**Veredito:** ✅ Bom. `login` é o 3º, depois de dois nomes mais longos com o mesmo prefixo.

---

### HTML-3
<a id="html-3"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 2 · **lugar na gramática:** `html:attribute:form`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" met▮="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `method`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 method
 2 meta
 3 meter
```

**Veredito:** ✅ Bom. `method` em primeiro.

---

### HTML-4
<a id="html-4"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 3 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <▮ class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `h2`: aparece em 45º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 class [a]   11 blockquote
 2 a           12 body
 3 abbr        13 br
 4 address     14 button
 5 area        15 canvas
 6 article     16 caption
 7 aside       17 circle
 8 audio       18 cite
 9 b           19 clipPath
10 base        20 code
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `<`: `class` (a palavra depois do cursor) primeiro, depois tags em ordem alfabética; `h2` é o 45º.

---

### HTML-5
<a id="html-5"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 3 · **lugar na gramática:** `html:text`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">S▮ in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `Sign`: aparece em 2º lugar de 19

**Saída** (as 20 primeiras sugestões):

```text
 1 submit [a]   11 span
 2 Sign [a]     12 stop
 3 samp         13 strong
 4 script       14 style
 5 search       15 sub
 6 section      16 summary
 7 select       17 sup
 8 slot         18 svg
 9 small        19 symbol
10 source
```

**Veredito:** ✅ Bom. Nó de texto: palavras do arquivo primeiro, `Sign` é o 2º.

---

### HTML-6
<a id="html-6"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 3 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h▮>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `h2`: aparece em 2º lugar de 12

**Saída** (as 20 primeiras sugestões):

```text
 1 h1        11 html
 2 h2        12 href [a]
 3 h3
 4 h4
 5 h5
 6 h6
 7 head
 8 header
 9 hgroup
10 hr
```

**Veredito:** ✅ Bom. Tag de fechamento: h1, h2 primeiro.

---

### HTML-7
<a id="html-7"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 4 · **lugar na gramática:** `html:value:for`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="ema▮">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `email`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 Email [a]
```

**Veredito:** ✅ Bom. `Email` é o único item.

---

### HTML-8
<a id="html-8"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 4 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</▮>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `label`: aparece em 60º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 a            11 body
 2 abbr         12 br
 3 address      13 button
 4 area         14 canvas
 5 article      15 caption
 6 aside        16 circle
 7 audio        17 cite
 8 b            18 clipPath
 9 base         19 code
10 blockquote   20 col
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `</`: tags em ordem alfabética; o elemento que está aberto (`label`) é o 60º.

---

### HTML-9
<a id="html-9"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 5 · **lugar na gramática:** `html:value:type`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="e▮" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `email`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 email
 2 example [a]
 3 export [a]
 4 ellipse
 5 em
 6 embed
```

**Veredito:** ✅ Bom. `email` em primeiro (um valor de `type`).

---

### HTML-10
<a id="html-10"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D1, linha 5 · **lugar na gramática:** `html:value:id`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="emi▮" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `email`: aparece em 3º lugar de 3

**Saída** (as 20 primeiras sugestões):

```text
 1 em [~]
 2 embed [~]
 3 email [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `email`, 3º entre em/embed/email.

---

### HTML-11
<a id="html-11"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 5 · **lugar na gramática:** `html:attribute:input`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" pla▮="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `placeholder`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 placeholder
```

**Veredito:** ✅ Bom. `placeholder` é o único item.

---

### HTML-12
<a id="html-12"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 5 · **lugar na gramática:** `html:value:placeholder`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.▮" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `com`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 example [a]       11 type [a]
 2 required [a]      12 input [a]
 3 you [a]           13 Sign [a]
 4 autofocus [a]     14 minlength [a]
 5 placeholder [a]   15 login__title [a]
 6 email [a]         16 class [a]
 7 label [a]         17 button [a]
 8 for [a]           18 post [a]
 9 name [a]          19 method [a]
10 password [a]      20 submit [a]
```

**Veredito:** ✅ Bom. Dentro de um valor de texto livre: as palavras mais próximas do arquivo.

---

### HTML-13
<a id="html-13"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 6 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <l▮ for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `label`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 label
 2 legend
 3 li
 4 line
 5 linearGradient
 6 link
 7 login__title [a]
 8 login [a]
 9 login__forgot [a]
10 loginForm [a]
```

**Veredito:** ✅ Bom. `label` em primeiro.

---

### HTML-14
<a id="html-14"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 6 · **lugar na gramática:** `html:value:for`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="pa▮">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `password`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 Password [a]
 2 path
```

**Veredito:** ✅ Bom. `Password` em primeiro.

---

### HTML-15
<a id="html-15"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 7 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <inp▮ type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `input`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 input
```

**Veredito:** ✅ Bom. `input` é o único item.

---

### HTML-16
<a id="html-16"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 7 · **lugar na gramática:** `html:value:type`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="▮" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `password`: aparece em 4º lugar de 100

**Saída** (as 20 primeiras sugestões):

```text
 1 submit [a]   11 date
 2 email [a]    12 hidden
 3 text         13 search
 4 password     14 tel
 5 number       15 url
 6 checkbox     16 type [a]
 7 radio        17 input [a]
 8 button       18 name [a]
 9 reset        19 label [a]
10 file         20 minlength [a]
```

**Veredito:** ✅ Bom. Palavras usadas antes com `type=` vêm primeiro, depois os tipos de input; `password` é o 4º.

---

### HTML-17
<a id="html-17"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 7 · **lugar na gramática:** `html:attribute:input`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" n▮="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `name`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 name
 2 noopener [a]
 3 nav
 4 noscript
```

**Veredito:** ✅ Bom. `name` em primeiro.

---

### HTML-18
<a id="html-18"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 7 · **lugar na gramática:** `html:attribute:input`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" mi▮="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `minlength`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 min
```

**Veredito:** ✅ Bom. `min` é o único item (`minlength` não aparece em outro lugar do arquivo).

---

### HTML-19
<a id="html-19"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 8 · **lugar na gramática:** `html:attribute:button`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button typ▮="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `type`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 type
```

**Veredito:** ✅ Bom. `type` é o único item.

---

### HTML-20
<a id="html-20"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D1, linha 8 · **lugar na gramática:** `html:attribute:button`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" cals▮="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `class`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 class [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `class`.

---

### HTML-21
<a id="html-21"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 8 · **lugar na gramática:** `html:attribute:button`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" d▮>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `disabled`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 disabled    11 dialog
 2 dir         12 div
 3 draggable   13 dl
 4 data        14 dt
 5 datalist
 6 dd
 7 defs
 8 del
 9 details
10 dfn
```

**Veredito:** ✅ Bom. `disabled` em primeiro.

---

### HTML-22
<a id="html-22"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 8 · **lugar na gramática:** `html:text`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign i▮</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `in`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 in [a]
 2 input [a]
 3 iframe
 4 img
 5 ins
```

**Veredito:** ✅ Bom. `in` em primeiro (nó de texto).

---

### HTML-23
<a id="html-23"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 9 · **lugar na gramática:** `html:value:href`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/for▮" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `forgot`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 Forgot [a]
 2 form [a]
```

**Veredito:** ✅ Bom. `Forgot` em primeiro.

---

### HTML-24
<a id="html-24"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 9 · **lugar na gramática:** `html:value:class`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="▮" target="_blank" rel="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `login__forgot`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 btn [a]            11 button [a]
 2 login__title [a]   12 your [a]
 3 login [a]          13 Sign [a]
 4 class [a]          14 password [a]
 5 target [a]         15 disabled [a]
 6 forgot [a]         16 btn-primary [a]
 7 _blank [a]         17 form [a]
 8 href [a]           18 submit [a]
 9 rel [a]            19 type [a]
10 noopener [a]       20 required [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito num valor de class: nomes de classe primeiro, depois nomes e valores de atributos (`class`, `target`, `href`) como ruído.

---

### HTML-25
<a id="html-25"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 9 · **lugar na gramática:** `html:attribute:a`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" r▮="noopener">Forgot your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `rel`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 rel
 2 role
 3 required [a]
 4 radialGradient
 5 rect
 6 rp
 7 rt
 8 ruby
```

**Veredito:** ✅ Bom. `rel` em primeiro.

---

### HTML-26
<a id="html-26"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 9 · **lugar na gramática:** `html:text`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Fo▮ your password?</a>
  </form>

```

**Palavra que a pessoa ia digitar:** `Forgot`: aparece em 2º lugar de 4

**Saída** (as 20 primeiras sugestões):

```text
 1 form [a]
 2 forgot [a]
 3 for [a]
 4 footer
```

**Veredito:** ✅ Bom. `form`, `forgot`, `for`, `footer`.

---

### HTML-27
<a id="html-27"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 10 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <form class="login" action="/login" method="post">
    <h2 class="login__title">Sign in</h2>
    <label for="email">Email</label>
    <input type="email" id="email" name="email" placeholder="you@example.com" required autofocus>
    <label for="password">Password</label>
    <input type="password" id="password" name="password" minlength="8" required>
    <button type="submit" class="btn btn-primary" disabled>Sign in</button>
    <a href="/forgot" class="login__forgot" target="_blank" rel="noopener">Forgot your password?</a>
  </for▮>

```

**Palavra que a pessoa ia digitar:** `form`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 form
 2 Forgot [a]
```

**Veredito:** ✅ Bom. `form` em primeiro.

---

### HTML-28
<a id="html-28"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 3 · **lugar na gramática:** `html:attribute:table`

**Entrada** (a string onde está o cursor):

```text

    <table ▮="data-table">
      <thead>
        <tr><th scope="col">Name</th><th scope="col">Role</th><th colspan="2">Actions</th></tr>
      </thead>
      <tbody>
        ${rows.map(r => html`<tr><td>${r.name}</td><td>${r.role}</td><td><button class="edit">Edit</button></td></tr>`)}
      </tbody>
    </table>
  
```

**Palavra que a pessoa ia digitar:** `class`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 class       11 contenteditable
 2 id          12 accesskey
 3 style       13 slot
 4 title       14 spellcheck
 5 lang        15 translate
 6 dir         16 aria-label
 7 hidden      17 aria-hidden
 8 tabindex    18 aria-expanded
 9 role        19 aria-labelledby
10 draggable   20 aria-describedby
```

**Veredito:** ✅ Bom. `class` em primeiro.

---

### HTML-29
<a id="html-29"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 5 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

    <table class="data-table">
      <thead>
        <t▮><th scope="col">Name</th><th scope="col">Role</th><th colspan="2">Actions</th></tr>
      </thead>
      <tbody>
        ${rows.map(r => html`<tr><td>${r.name}</td><td>${r.role}</td><td><button class="edit">Edit</button></td></tr>`)}
      </tbody>
    </table>
  
```

**Palavra que a pessoa ia digitar:** `tr`: aparece em 12º lugar de 14

**Saída** (as 20 primeiras sugestões):

```text
 1 tbody      11 title
 2 table      12 tr
 3 td         13 track
 4 template   14 tspan
 5 text
 6 textarea
 7 tfoot
 8 th
 9 thead
10 time
```

**Veredito:** ⚠️ Razoável, com ressalva. Tag de fechamento `</t`: tags em ordem alfabética, `tr` é a 12ª; o elemento aberto não é priorizado.

---

### HTML-30
<a id="html-30"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D2, linha 5 · **lugar na gramática:** `html:attribute:th`

**Entrada** (a string onde está o cursor):

```text

    <table class="data-table">
      <thead>
        <tr><th scp▮="col">Name</th><th scope="col">Role</th><th colspan="2">Actions</th></tr>
      </thead>
      <tbody>
        ${rows.map(r => html`<tr><td>${r.name}</td><td>${r.role}</td><td><button class="edit">Edit</button></td></tr>`)}
      </tbody>
    </table>
  
```

**Palavra que a pessoa ia digitar:** `scope`: aparece em 4º lugar de 4

**Saída** (as 20 primeiras sugestões):

```text
 1 script [~]
 2 span [~]
 3 sup [~]
 4 scope [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. Erro `scp` num `th`: as tags script/span/sup vêm antes de `scope` (4º).

---

### HTML-31
<a id="html-31"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 5 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

    <table class="data-table">
      <thead>
        <tr><th scope="col">Name</t▮><th scope="col">Role</th><th colspan="2">Actions</th></tr>
      </thead>
      <tbody>
        ${rows.map(r => html`<tr><td>${r.name}</td><td>${r.role}</td><td><button class="edit">Edit</button></td></tr>`)}
      </tbody>
    </table>
  
```

**Palavra que a pessoa ia digitar:** `th`: aparece em 8º lugar de 14

**Saída** (as 20 primeiras sugestões):

```text
 1 table      11 title
 2 tbody      12 tr
 3 td         13 track
 4 template   14 tspan
 5 text
 6 textarea
 7 tfoot
 8 th
 9 thead
10 time
```

**Veredito:** ⚠️ Razoável, com ressalva. Tag de fechamento `</t`: `th` é a 8ª.

---

### HTML-32
<a id="html-32"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 5 · **lugar na gramática:** `html:attribute:th`

**Entrada** (a string onde está o cursor):

```text

    <table class="data-table">
      <thead>
        <tr><th scope="col">Name</th><th ▮="col">Role</th><th colspan="2">Actions</th></tr>
      </thead>
      <tbody>
        ${rows.map(r => html`<tr><td>${r.name}</td><td>${r.role}</td><td><button class="edit">Edit</button></td></tr>`)}
      </tbody>
    </table>
  
```

**Palavra que a pessoa ia digitar:** `scope`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 scope [a]     11 dir
 2 colspan [a]   12 hidden
 3 rowspan       13 tabindex
 4 headers       14 role
 5 align         15 draggable
 6 class         16 contenteditable
 7 id            17 accesskey
 8 style         18 slot
 9 title         19 spellcheck
10 lang          20 translate
```

**Veredito:** ✅ Bom. `scope` em primeiro.

---

### HTML-33
<a id="html-33"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 5 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

    <table class="data-table">
      <thead>
        <tr><th scope="col">Name</th><th scope="col">Role</t▮><th colspan="2">Actions</th></tr>
      </thead>
      <tbody>
        ${rows.map(r => html`<tr><td>${r.name}</td><td>${r.role}</td><td><button class="edit">Edit</button></td></tr>`)}
      </tbody>
    </table>
  
```

**Palavra que a pessoa ia digitar:** `th`: aparece em 8º lugar de 14

**Saída** (as 20 primeiras sugestões):

```text
 1 table      11 title
 2 tbody      12 tr
 3 td         13 track
 4 template   14 tspan
 5 text
 6 textarea
 7 tfoot
 8 th
 9 thead
10 time
```

**Veredito:** ⚠️ Razoável, com ressalva. Tag de fechamento `</t`: `th` é a 8ª.

---

### HTML-34
<a id="html-34"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 5 · **lugar na gramática:** `html:attribute:th`

**Entrada** (a string onde está o cursor):

```text

    <table class="data-table">
      <thead>
        <tr><th scope="col">Name</th><th scope="col">Role</th><th co▮="2">Actions</th></tr>
      </thead>
      <tbody>
        ${rows.map(r => html`<tr><td>${r.name}</td><td>${r.role}</td><td><button class="edit">Edit</button></td></tr>`)}
      </tbody>
    </table>
  
```

**Palavra que a pessoa ia digitar:** `colspan`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 colspan
 2 contenteditable
 3 col [a]
 4 code
 5 colgroup
```

**Veredito:** ✅ Bom. `colspan` em primeiro.

---

### HTML-35
<a id="html-35"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 5 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

    <table class="data-table">
      <thead>
        <tr><th scope="col">Name</th><th scope="col">Role</th><th colspan="2">Actions</th></t▮>
      </thead>
      <tbody>
        ${rows.map(r => html`<tr><td>${r.name}</td><td>${r.role}</td><td><button class="edit">Edit</button></td></tr>`)}
      </tbody>
    </table>
  
```

**Palavra que a pessoa ia digitar:** `tr`: aparece em 12º lugar de 14

**Saída** (as 20 primeiras sugestões):

```text
 1 table      11 title
 2 tbody      12 tr
 3 td         13 track
 4 template   14 tspan
 5 text
 6 textarea
 7 tfoot
 8 th
 9 thead
10 time
```

**Veredito:** ⚠️ Razoável, com ressalva. Tag de fechamento `</t`: `tr` é a 12ª.

---

### HTML-36
<a id="html-36"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 7 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

    <table class="data-table">
      <thead>
        <tr><th scope="col">Name</th><th scope="col">Role</th><th colspan="2">Actions</th></tr>
      </thead>
      <▮>
        ${rows.map(r => html`<tr><td>${r.name}</td><td>${r.role}</td><td><button class="edit">Edit</button></td></tr>`)}
      </tbody>
    </table>
  
```

**Palavra que a pessoa ia digitar:** `tbody`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 tr        11 blockquote
 2 a         12 body
 3 abbr      13 br
 4 address   14 button
 5 area      15 canvas
 6 article   16 caption
 7 aside     17 circle
 8 audio     18 cite
 9 b         19 clipPath
10 base      20 code
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `<`: `tr` primeiro, mas `tbody` fica além das 100 primeiras (lista alfabética).

---

### HTML-37
<a id="html-37"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 8 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text
<t▮><td>${r.name}</td><td>${r.role}</td><td><button class="edit">Edit</button></td></tr>
```

**Palavra que a pessoa ia digitar:** `tr`: aparece em 12º lugar de 14

**Saída** (as 20 primeiras sugestões):

```text
 1 table      11 title
 2 tbody      12 tr
 3 td         13 track
 4 template   14 tspan
 5 text
 6 textarea
 7 tfoot
 8 th
 9 thead
10 time
```

**Veredito:** ⚠️ Razoável, com ressalva. Tag de abertura `<t`: `tr` é a 12ª.

---

### HTML-38
<a id="html-38"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 8 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text
<tr><td>${r.name}</t▮><td>${r.role}</td><td><button class="edit">Edit</button></td></tr>
```

**Palavra que a pessoa ia digitar:** `td`: aparece em 3º lugar de 14

**Saída** (as 20 primeiras sugestões):

```text
 1 table      11 title
 2 tbody      12 tr
 3 td         13 track
 4 template   14 tspan
 5 text
 6 textarea
 7 tfoot
 8 th
 9 thead
10 time
```

**Veredito:** ✅ Bom. Tag de fechamento: `td` em 3º.

---

### HTML-39
<a id="html-39"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 8 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text
<tr><td>${r.name}</td><td>${r.role}</td><t▮><button class="edit">Edit</button></td></tr>
```

**Palavra que a pessoa ia digitar:** `td`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 td         11 title
 2 table      12 tr
 3 tbody      13 track
 4 template   14 tspan
 5 text
 6 textarea
 7 tfoot
 8 th
 9 thead
10 time
```

**Veredito:** ✅ Bom. `td` em primeiro.

---

### HTML-40
<a id="html-40"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D2, linha 8 · **lugar na gramática:** `html:attribute:button`

**Entrada** (a string onde está o cursor):

```text
<tr><td>${r.name}</td><td>${r.role}</td><td><button cals▮="edit">Edit</button></td></tr>
```

**Palavra que a pessoa ia digitar:** `class`: aparece em 2º lugar de 2

**Saída** (as 20 primeiras sugestões):

```text
 1 colspan [~]
 2 class [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. Erro `cals` num botão: `colspan` (atributo de `th`) antes de `class`.

---

### HTML-41
<a id="html-41"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 8 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text
<tr><td>${r.name}</td><td>${r.role}</td><td><button class="edit">Edit</b▮></td></tr>
```

**Palavra que a pessoa ia digitar:** `button`: aparece em 5º lugar de 5

**Saída** (as 20 primeiras sugestões):

```text
 1 base
 2 blockquote
 3 body
 4 br
 5 button
```

**Veredito:** ✅ Bom. Tag de fechamento: `button` é o 5º de 5.

---

### HTML-42
<a id="html-42"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 8 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text
<tr><td>${r.name}</td><td>${r.role}</td><td><button class="edit">Edit</button></td></t▮>
```

**Palavra que a pessoa ia digitar:** `tr`: aparece em 12º lugar de 14

**Saída** (as 20 primeiras sugestões):

```text
 1 table      11 title
 2 tbody      12 tr
 3 td         13 track
 4 template   14 tspan
 5 text
 6 textarea
 7 tfoot
 8 th
 9 thead
10 time
```

**Veredito:** ⚠️ Razoável, com ressalva. Tag de fechamento `</t`: `tr` é a 12ª.

---

### HTML-43
<a id="html-43"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 2 · **lugar na gramática:** `html:value:class`

**Entrada** (a string onde está o cursor):

```text

  <header class="sit▮">
    <nav class="site-nav" aria-label="Main">
      <ul class="site-nav__list">
        <li class="site-nav__item"><a href="/">Home</a></li>
        <li class="site-nav__item"><a href="/docs" class="active">Docs</a></li>
        <li class="site-nav__item"><a href="/blog">Blog</a></li>
      </ul>
    </nav>
    <img src="/logo.svg" alt="Logo" width="120" height="32" loading="lazy">
  </header>

```

**Palavra que a pessoa ia digitar:** `site-header`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 site-nav [a]
 2 site-nav__list [a]
 3 site-nav__item [a]
```

**Veredito:** ✅ Bom. Os outros nomes BEM com o prefixo.

---

### HTML-44
<a id="html-44"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 3 · **lugar na gramática:** `html:attribute:nav`

**Entrada** (a string onde está o cursor):

```text

  <header class="site-header">
    <nav ▮="site-nav" aria-label="Main">
      <ul class="site-nav__list">
        <li class="site-nav__item"><a href="/">Home</a></li>
        <li class="site-nav__item"><a href="/docs" class="active">Docs</a></li>
        <li class="site-nav__item"><a href="/blog">Blog</a></li>
      </ul>
    </nav>
    <img src="/logo.svg" alt="Logo" width="120" height="32" loading="lazy">
  </header>

```

**Palavra que a pessoa ia digitar:** `class`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 class       11 contenteditable
 2 id          12 accesskey
 3 style       13 slot
 4 title       14 spellcheck
 5 lang        15 translate
 6 dir         16 aria-label
 7 hidden      17 aria-hidden
 8 tabindex    18 aria-expanded
 9 role        19 aria-labelledby
10 draggable   20 aria-describedby
```

**Veredito:** ✅ Bom. `class` em primeiro.

---

### HTML-45
<a id="html-45"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 3 · **lugar na gramática:** `html:value:aria-label`

**Entrada** (a string onde está o cursor):

```text

  <header class="site-header">
    <nav class="site-nav" aria-label="M▮">
      <ul class="site-nav__list">
        <li class="site-nav__item"><a href="/">Home</a></li>
        <li class="site-nav__item"><a href="/docs" class="active">Docs</a></li>
        <li class="site-nav__item"><a href="/blog">Blog</a></li>
      </ul>
    </nav>
    <img src="/logo.svg" alt="Logo" width="120" height="32" loading="lazy">
  </header>

```

**Palavra que a pessoa ia digitar:** `Main`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 main
 2 map
 3 mark
 4 mask
 5 menu
 6 meta
 7 meter
```

**Veredito:** ⚠️ Razoável, com ressalva. Valor de texto livre `aria-label="M`: tags (main, map, mark...) são oferecidas; não pertencem ali.

---

### HTML-46
<a id="html-46"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 4 · **lugar na gramática:** `html:attribute:ul`

**Entrada** (a string onde está o cursor):

```text

  <header class="site-header">
    <nav class="site-nav" aria-label="Main">
      <ul cl▮="site-nav__list">
        <li class="site-nav__item"><a href="/">Home</a></li>
        <li class="site-nav__item"><a href="/docs" class="active">Docs</a></li>
        <li class="site-nav__item"><a href="/blog">Blog</a></li>
      </ul>
    </nav>
    <img src="/logo.svg" alt="Logo" width="120" height="32" loading="lazy">
  </header>

```

**Palavra que a pessoa ia digitar:** `class`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 class
 2 clipPath
```

**Veredito:** ✅ Bom. `class` em primeiro.

---

### HTML-47
<a id="html-47"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 5 · **lugar na gramática:** `html:attribute:li`

**Entrada** (a string onde está o cursor):

```text

  <header class="site-header">
    <nav class="site-nav" aria-label="Main">
      <ul class="site-nav__list">
        <li cla▮="site-nav__item"><a href="/">Home</a></li>
        <li class="site-nav__item"><a href="/docs" class="active">Docs</a></li>
        <li class="site-nav__item"><a href="/blog">Blog</a></li>
      </ul>
    </nav>
    <img src="/logo.svg" alt="Logo" width="120" height="32" loading="lazy">
  </header>

```

**Palavra que a pessoa ia digitar:** `class`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 class [a]
```

**Veredito:** ✅ Bom. `class` é o único item.

---

### HTML-48
<a id="html-48"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 5 · **lugar na gramática:** `html:attribute:a`

**Entrada** (a string onde está o cursor):

```text

  <header class="site-header">
    <nav class="site-nav" aria-label="Main">
      <ul class="site-nav__list">
        <li class="site-nav__item"><a ▮="/">Home</a></li>
        <li class="site-nav__item"><a href="/docs" class="active">Docs</a></li>
        <li class="site-nav__item"><a href="/blog">Blog</a></li>
      </ul>
    </nav>
    <img src="/logo.svg" alt="Logo" width="120" height="32" loading="lazy">
  </header>

```

**Palavra que a pessoa ia digitar:** `href`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 href [a]   11 lang
 2 target     12 dir
 3 rel        13 hidden
 4 download   14 tabindex
 5 hreflang   15 role
 6 type       16 draggable
 7 class      17 contenteditable
 8 id         18 accesskey
 9 style      19 slot
10 title      20 spellcheck
```

**Veredito:** ✅ Bom. `href` em primeiro.

---

### HTML-49
<a id="html-49"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 6 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <header class="site-header">
    <nav class="site-nav" aria-label="Main">
      <ul class="site-nav__list">
        <li class="site-nav__item"><a href="/">Home</a></li>
        <l▮ class="site-nav__item"><a href="/docs" class="active">Docs</a></li>
        <li class="site-nav__item"><a href="/blog">Blog</a></li>
      </ul>
    </nav>
    <img src="/logo.svg" alt="Logo" width="120" height="32" loading="lazy">
  </header>

```

**Palavra que a pessoa ia digitar:** `li`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 li
 2 label
 3 legend
 4 line
 5 linearGradient
 6 link
 7 logo [a]
 8 loading [a]
 9 lazy [a]
```

**Veredito:** ✅ Bom. `li` em primeiro.

---

### HTML-50
<a id="html-50"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D3, linha 6 · **lugar na gramática:** `html:value:class`

**Entrada** (a string onde está o cursor):

```text

  <header class="site-header">
    <nav class="site-nav" aria-label="Main">
      <ul class="site-nav__list">
        <li class="site-nav__item"><a href="/">Home</a></li>
        <li class="sie-n▮"><a href="/docs" class="active">Docs</a></li>
        <li class="site-nav__item"><a href="/blog">Blog</a></li>
      </ul>
    </nav>
    <img src="/logo.svg" alt="Logo" width="120" height="32" loading="lazy">
  </header>

```

**Palavra que a pessoa ia digitar:** `site-nav__item`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 site-nav__item [~]
 2 site-nav__list [~]
 3 site-nav [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `site-nav__item`.

---

### HTML-51
<a id="html-51"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 6 · **lugar na gramática:** `html:attribute:a`

**Entrada** (a string onde está o cursor):

```text

  <header class="site-header">
    <nav class="site-nav" aria-label="Main">
      <ul class="site-nav__list">
        <li class="site-nav__item"><a href="/">Home</a></li>
        <li class="site-nav__item"><a href="/docs" cla▮="active">Docs</a></li>
        <li class="site-nav__item"><a href="/blog">Blog</a></li>
      </ul>
    </nav>
    <img src="/logo.svg" alt="Logo" width="120" height="32" loading="lazy">
  </header>

```

**Palavra que a pessoa ia digitar:** `class`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 class
```

**Veredito:** ✅ Bom. `class` é o único item.

---

### HTML-52
<a id="html-52"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 6 · **lugar na gramática:** `html:text`

**Entrada** (a string onde está o cursor):

```text

  <header class="site-header">
    <nav class="site-nav" aria-label="Main">
      <ul class="site-nav__list">
        <li class="site-nav__item"><a href="/">Home</a></li>
        <li class="site-nav__item"><a href="/docs" class="active">▮</a></li>
        <li class="site-nav__item"><a href="/blog">Blog</a></li>
      </ul>
    </nav>
    <img src="/logo.svg" alt="Logo" width="120" height="32" loading="lazy">
  </header>

```

**Palavra que a pessoa ia digitar:** `Docs`: aparece em 3º lugar de 100

**Saída** (as 20 primeiras sugestões):

```text
 1 active [a]           11 logo [a]
 2 class [a]            12 svg [a]
 3 docs [a]             13 alt [a]
 4 href [a]             14 site-nav__list [a]
 5 site-nav__item [a]   15 width [a]
 6 blog [a]             16 height [a]
 7 Home [a]             17 loading [a]
 8 nav [a]              18 Main [a]
 9 img [a]              19 lazy [a]
10 src [a]              20 aria-label [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito num nó de texto: palavras do arquivo, incluindo nomes e valores de atributos (`class`, `href`); `Docs` é o 3º.

---

### HTML-53
<a id="html-53"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 7 · **lugar na gramática:** `html:attribute:li`

**Entrada** (a string onde está o cursor):

```text

  <header class="site-header">
    <nav class="site-nav" aria-label="Main">
      <ul class="site-nav__list">
        <li class="site-nav__item"><a href="/">Home</a></li>
        <li class="site-nav__item"><a href="/docs" class="active">Docs</a></li>
        <li c▮="site-nav__item"><a href="/blog">Blog</a></li>
      </ul>
    </nav>
    <img src="/logo.svg" alt="Logo" width="120" height="32" loading="lazy">
  </header>

```

**Palavra que a pessoa ia digitar:** `class`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 class [a]         11 colgroup
 2 contenteditable
 3 const [a]
 4 canvas
 5 caption
 6 circle
 7 cite
 8 clipPath
 9 code
10 col
```

**Veredito:** ✅ Bom. `class` em primeiro.

---

### HTML-54
<a id="html-54"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 7 · **lugar na gramática:** `html:attribute:a`

**Entrada** (a string onde está o cursor):

```text

  <header class="site-header">
    <nav class="site-nav" aria-label="Main">
      <ul class="site-nav__list">
        <li class="site-nav__item"><a href="/">Home</a></li>
        <li class="site-nav__item"><a href="/docs" class="active">Docs</a></li>
        <li class="site-nav__item"><a hr▮="/blog">Blog</a></li>
      </ul>
    </nav>
    <img src="/logo.svg" alt="Logo" width="120" height="32" loading="lazy">
  </header>

```

**Palavra que a pessoa ia digitar:** `href`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 href [a]
 2 hreflang
```

**Veredito:** ✅ Bom. `href` em primeiro.

---

### HTML-55
<a id="html-55"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 7 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <header class="site-header">
    <nav class="site-nav" aria-label="Main">
      <ul class="site-nav__list">
        <li class="site-nav__item"><a href="/">Home</a></li>
        <li class="site-nav__item"><a href="/docs" class="active">Docs</a></li>
        <li class="site-nav__item"><a href="/blog">Blog</a></l▮>
      </ul>
    </nav>
    <img src="/logo.svg" alt="Logo" width="120" height="32" loading="lazy">
  </header>

```

**Palavra que a pessoa ia digitar:** `li`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 li
 2 label
 3 legend
 4 line
 5 linearGradient
 6 link
 7 logo [a]
 8 loading [a]
 9 lazy [a]
```

**Veredito:** ✅ Bom. Tag de fechamento: `li` em primeiro.

---

### HTML-56
<a id="html-56"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 9 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <header class="site-header">
    <nav class="site-nav" aria-label="Main">
      <ul class="site-nav__list">
        <li class="site-nav__item"><a href="/">Home</a></li>
        <li class="site-nav__item"><a href="/docs" class="active">Docs</a></li>
        <li class="site-nav__item"><a href="/blog">Blog</a></li>
      </ul>
    </▮>
    <img src="/logo.svg" alt="Logo" width="120" height="32" loading="lazy">
  </header>

```

**Palavra que a pessoa ia digitar:** `nav`: aparece em 73º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 a            11 body
 2 abbr         12 br
 3 address      13 button
 4 area         14 canvas
 5 article      15 caption
 6 aside        16 circle
 7 audio        17 cite
 8 b            18 clipPath
 9 base         19 code
10 blockquote   20 col
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `</`: tags em ordem alfabética; o elemento aberto `nav` é o 73º.

---

### HTML-57
<a id="html-57"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 10 · **lugar na gramática:** `html:value:src`

**Entrada** (a string onde está o cursor):

```text

  <header class="site-header">
    <nav class="site-nav" aria-label="Main">
      <ul class="site-nav__list">
        <li class="site-nav__item"><a href="/">Home</a></li>
        <li class="site-nav__item"><a href="/docs" class="active">Docs</a></li>
        <li class="site-nav__item"><a href="/blog">Blog</a></li>
      </ul>
    </nav>
    <img src="/l▮.svg" alt="Logo" width="120" height="32" loading="lazy">
  </header>

```

**Palavra que a pessoa ia digitar:** `logo`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 Logo [a]
 2 loading [a]
 3 lazy [a]
 4 label
 5 legend
 6 li
 7 line
 8 linearGradient
 9 link
```

**Veredito:** ✅ Bom. `Logo` em primeiro.

---

### HTML-58
<a id="html-58"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 10 · **lugar na gramática:** `html:attribute:img`

**Entrada** (a string onde está o cursor):

```text

  <header class="site-header">
    <nav class="site-nav" aria-label="Main">
      <ul class="site-nav__list">
        <li class="site-nav__item"><a href="/">Home</a></li>
        <li class="site-nav__item"><a href="/docs" class="active">Docs</a></li>
        <li class="site-nav__item"><a href="/blog">Blog</a></li>
      </ul>
    </nav>
    <img src="/logo.svg" al▮="Logo" width="120" height="32" loading="lazy">
  </header>

```

**Palavra que a pessoa ia digitar:** `alt`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 alt
```

**Veredito:** ✅ Bom. `alt` é o único item.

---

### HTML-59
<a id="html-59"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 10 · **lugar na gramática:** `html:attribute:img`

**Entrada** (a string onde está o cursor):

```text

  <header class="site-header">
    <nav class="site-nav" aria-label="Main">
      <ul class="site-nav__list">
        <li class="site-nav__item"><a href="/">Home</a></li>
        <li class="site-nav__item"><a href="/docs" class="active">Docs</a></li>
        <li class="site-nav__item"><a href="/blog">Blog</a></li>
      </ul>
    </nav>
    <img src="/logo.svg" alt="Logo" width="120" hei▮="32" loading="lazy">
  </header>

```

**Palavra que a pessoa ia digitar:** `height`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 height
```

**Veredito:** ✅ Bom. `height` é o único item.

---

### HTML-60
<a id="html-60"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 10 · **lugar na gramática:** `html:value:loading`

**Entrada** (a string onde está o cursor):

```text

  <header class="site-header">
    <nav class="site-nav" aria-label="Main">
      <ul class="site-nav__list">
        <li class="site-nav__item"><a href="/">Home</a></li>
        <li class="site-nav__item"><a href="/docs" class="active">Docs</a></li>
        <li class="site-nav__item"><a href="/blog">Blog</a></li>
      </ul>
    </nav>
    <img src="/logo.svg" alt="Logo" width="120" height="32" loading="▮">
  </header>

```

**Palavra que a pessoa ia digitar:** `lazy`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 lazy          11 img [a]
 2 eager         12 nav [a]
 3 loading [a]   13 Blog [a]
 4 header [a]    14 href [a]
 5 height [a]    15 site-nav__item [a]
 6 width [a]     16 class [a]
 7 Logo [a]      17 Docs [a]
 8 alt [a]       18 active [a]
 9 svg [a]       19 Home [a]
10 src [a]       20 site-nav__list [a]
```

**Veredito:** ✅ Bom. `lazy`, `eager` (valores de `loading`).

---

### HTML-61
<a id="html-61"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 2 · **lugar na gramática:** `html:attribute:section`

**Entrada** (a string onde está o cursor):

```text

  <section c▮="card" id="featured" role="region" tabindex="0">
    <h3 class="card__title">${title}</h3>
    <p class="card__body">${body}</p>
    <textarea name="note" rows="3" cols="40" placeholder="Add a note"></textarea>
    <select name="color" class="card__select">
      <option value="red" selected>Red</option>
      <option value="blue">Blue</option>
    </select>
    <video src="/intro.mp4" controls autoplay muted></video>
  </section>

```

**Palavra que a pessoa ia digitar:** `class`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 class              11 canvas
 2 contenteditable    12 caption
 3 card [a]           13 circle
 4 card__title [a]    14 cite
 5 card__body [a]     15 clipPath
 6 cols [a]           16 code
 7 color [a]          17 col
 8 card__select [a]   18 colgroup
 9 controls [a]
10 const [a]
```

**Veredito:** ✅ Bom. `class` em primeiro.

---

### HTML-62
<a id="html-62"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 2 · **lugar na gramática:** `html:attribute:section`

**Entrada** (a string onde está o cursor):

```text

  <section class="card" i▮="featured" role="region" tabindex="0">
    <h3 class="card__title">${title}</h3>
    <p class="card__body">${body}</p>
    <textarea name="note" rows="3" cols="40" placeholder="Add a note"></textarea>
    <select name="color" class="card__select">
      <option value="red" selected>Red</option>
      <option value="blue">Blue</option>
    </select>
    <video src="/intro.mp4" controls autoplay muted></video>
  </section>

```

**Palavra que a pessoa ia digitar:** `id`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 id
 2 intro [a]
 3 iframe
 4 img
 5 input
 6 ins
```

**Veredito:** ✅ Bom. `id` em primeiro.

---

### HTML-63
<a id="html-63"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 2 · **lugar na gramática:** `html:value:role`

**Entrada** (a string onde está o cursor):

```text

  <section class="card" id="featured" role="reg▮" tabindex="0">
    <h3 class="card__title">${title}</h3>
    <p class="card__body">${body}</p>
    <textarea name="note" rows="3" cols="40" placeholder="Add a note"></textarea>
    <select name="color" class="card__select">
      <option value="red" selected>Red</option>
      <option value="blue">Blue</option>
    </select>
    <video src="/intro.mp4" controls autoplay muted></video>
  </section>

```

**Palavra que a pessoa ia digitar:** `region`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 rect [~]
 2 red [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. `role="reg`: papéis não estão na tabela; `rect~` e `red~` (tag/palavra parecida) são ruído.

---

### HTML-64
<a id="html-64"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 3 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <section class="card" id="featured" role="region" tabindex="0">
    <▮ class="card__title">${title}</h3>
    <p class="card__body">${body}</p>
    <textarea name="note" rows="3" cols="40" placeholder="Add a note"></textarea>
    <select name="color" class="card__select">
      <option value="red" selected>Red</option>
      <option value="blue">Blue</option>
    </select>
    <video src="/intro.mp4" controls autoplay muted></video>
  </section>

```

**Palavra que a pessoa ia digitar:** `h3`: aparece em 45º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 a            11 body
 2 abbr         12 br
 3 address      13 button
 4 area         14 canvas
 5 article      15 caption
 6 aside        16 circle
 7 audio        17 cite
 8 b            18 clipPath
 9 base         19 code
10 blockquote   20 col
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `<`: tags em ordem alfabética; `h3` é o 45º.

---

### HTML-65
<a id="html-65"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 3 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <section class="card" id="featured" role="region" tabindex="0">
    <h3 class="card__title">${title}</h▮>
    <p class="card__body">${body}</p>
    <textarea name="note" rows="3" cols="40" placeholder="Add a note"></textarea>
    <select name="color" class="card__select">
      <option value="red" selected>Red</option>
      <option value="blue">Blue</option>
    </select>
    <video src="/intro.mp4" controls autoplay muted></video>
  </section>

```

**Palavra que a pessoa ia digitar:** `h3`: aparece em 3º lugar de 11

**Saída** (as 20 primeiras sugestões):

```text
 1 h1        11 html
 2 h2
 3 h3
 4 h4
 5 h5
 6 h6
 7 head
 8 header
 9 hgroup
10 hr
```

**Veredito:** ✅ Bom. Tag de fechamento: h1, h2, `h3` em 3º.

---

### HTML-66
<a id="html-66"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D4, linha 4 · **lugar na gramática:** `html:value:class`

**Entrada** (a string onde está o cursor):

```text

  <section class="card" id="featured" role="region" tabindex="0">
    <h3 class="card__title">${title}</h3>
    <p class="ca▮">${body}</p>
    <textarea name="note" rows="3" cols="40" placeholder="Add a note"></textarea>
    <select name="color" class="card__select">
      <option value="red" selected>Red</option>
      <option value="blue">Blue</option>
    </select>
    <video src="/intro.mp4" controls autoplay muted></video>
  </section>

```

**Palavra que a pessoa ia digitar:** `card__body`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 card__title [a]
 2 card [a]
 3 card__select [a]
 4 canvas
 5 caption
```

**Veredito:** ✅ Bom. `card__title`, `card`, `card__select`.

---

### HTML-67
<a id="html-67"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 5 · **lugar na gramática:** `html:value:name`

**Entrada** (a string onde está o cursor):

```text

  <section class="card" id="featured" role="region" tabindex="0">
    <h3 class="card__title">${title}</h3>
    <p class="card__body">${body}</p>
    <textarea name="not▮" rows="3" cols="40" placeholder="Add a note"></textarea>
    <select name="color" class="card__select">
      <option value="red" selected>Red</option>
      <option value="blue">Blue</option>
    </select>
    <video src="/intro.mp4" controls autoplay muted></video>
  </section>

```

**Palavra que a pessoa ia digitar:** `note`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 note [a]
```

**Veredito:** ✅ Bom. `note` é o único item.

---

### HTML-68
<a id="html-68"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 5 · **lugar na gramática:** `html:attribute:textarea`

**Entrada** (a string onde está o cursor):

```text

  <section class="card" id="featured" role="region" tabindex="0">
    <h3 class="card__title">${title}</h3>
    <p class="card__body">${body}</p>
    <textarea name="note" rows="3" ▮="40" placeholder="Add a note"></textarea>
    <select name="color" class="card__select">
      <option value="red" selected>Red</option>
      <option value="blue">Blue</option>
    </select>
    <video src="/intro.mp4" controls autoplay muted></video>
  </section>

```

**Palavra que a pessoa ia digitar:** `cols`: aparece em 3º lugar de 100

**Saída** (as 20 primeiras sugestões):

```text
 1 name          11 style
 2 rows          12 title
 3 cols          13 lang
 4 placeholder   14 dir
 5 required      15 hidden
 6 readonly      16 tabindex
 7 disabled      17 role
 8 maxlength     18 draggable
 9 class         19 contenteditable
10 id            20 accesskey
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito dentro de uma tag: atributos que a tag já tem (`name`, `rows`) são oferecidos primeiro; `cols` é o 3º.

---

### HTML-69
<a id="html-69"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 5 · **lugar na gramática:** `html:value:placeholder`

**Entrada** (a string onde está o cursor):

```text

  <section class="card" id="featured" role="region" tabindex="0">
    <h3 class="card__title">${title}</h3>
    <p class="card__body">${body}</p>
    <textarea name="note" rows="3" cols="40" placeholder="Add a n▮"></textarea>
    <select name="color" class="card__select">
      <option value="red" selected>Red</option>
      <option value="blue">Blue</option>
    </select>
    <video src="/intro.mp4" controls autoplay muted></video>
  </section>

```

**Palavra que a pessoa ia digitar:** `note`: aparece em 2º lugar de 4

**Saída** (as 20 primeiras sugestões):

```text
 1 name [a]
 2 note [a]
 3 nav
 4 noscript
```

**Veredito:** ✅ Bom. `name`, `note`.

---

### HTML-70
<a id="html-70"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D4, linha 6 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <section class="card" id="featured" role="region" tabindex="0">
    <h3 class="card__title">${title}</h3>
    <p class="card__body">${body}</p>
    <textarea name="note" rows="3" cols="40" placeholder="Add a note"></textarea>
    <seec▮ name="color" class="card__select">
      <option value="red" selected>Red</option>
      <option value="blue">Blue</option>
    </select>
    <video src="/intro.mp4" controls autoplay muted></video>
  </section>

```

**Palavra que a pessoa ia digitar:** `select`: aparece em 2º lugar de 3

**Saída** (as 20 primeiras sugestões):

```text
 1 section [~]
 2 select [~]
 3 selected [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `select` em 2º.

---

### HTML-71
<a id="html-71"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 6 · **lugar na gramática:** `html:attribute:select`

**Entrada** (a string onde está o cursor):

```text

  <section class="card" id="featured" role="region" tabindex="0">
    <h3 class="card__title">${title}</h3>
    <p class="card__body">${body}</p>
    <textarea name="note" rows="3" cols="40" placeholder="Add a note"></textarea>
    <select name="color" cla▮="card__select">
      <option value="red" selected>Red</option>
      <option value="blue">Blue</option>
    </select>
    <video src="/intro.mp4" controls autoplay muted></video>
  </section>

```

**Palavra que a pessoa ia digitar:** `class`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 class
```

**Veredito:** ✅ Bom. `class` é o único item.

---

### HTML-72
<a id="html-72"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 7 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <section class="card" id="featured" role="region" tabindex="0">
    <h3 class="card__title">${title}</h3>
    <p class="card__body">${body}</p>
    <textarea name="note" rows="3" cols="40" placeholder="Add a note"></textarea>
    <select name="color" class="card__select">
      <▮ value="red" selected>Red</option>
      <option value="blue">Blue</option>
    </select>
    <video src="/intro.mp4" controls autoplay muted></video>
  </section>

```

**Palavra que a pessoa ia digitar:** `option`: aparece em 79º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 value [a]   11 blockquote
 2 a           12 body
 3 abbr        13 br
 4 address     14 button
 5 area        15 canvas
 6 article     16 caption
 7 aside       17 circle
 8 audio       18 cite
 9 b           19 clipPath
10 base        20 code
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `<`: tags em ordem alfabética; `option` é o 79º.

---

### HTML-73
<a id="html-73"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 7 · **lugar na gramática:** `html:attribute:option`

**Entrada** (a string onde está o cursor):

```text

  <section class="card" id="featured" role="region" tabindex="0">
    <h3 class="card__title">${title}</h3>
    <p class="card__body">${body}</p>
    <textarea name="note" rows="3" cols="40" placeholder="Add a note"></textarea>
    <select name="color" class="card__select">
      <option value="red" s▮>Red</option>
      <option value="blue">Blue</option>
    </select>
    <video src="/intro.mp4" controls autoplay muted></video>
  </section>

```

**Palavra que a pessoa ia digitar:** `selected`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 selected      11 small
 2 style         12 source
 3 slot          13 span
 4 spellcheck    14 stop
 5 select [a]    15 strong
 6 src [a]       16 sub
 7 section [a]   17 summary
 8 samp          18 sup
 9 script        19 svg
10 search        20 symbol
```

**Veredito:** ✅ Bom. `selected` em primeiro.

---

### HTML-74
<a id="html-74"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D4, linha 7 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <section class="card" id="featured" role="region" tabindex="0">
    <h3 class="card__title">${title}</h3>
    <p class="card__body">${body}</p>
    <textarea name="note" rows="3" cols="40" placeholder="Add a note"></textarea>
    <select name="color" class="card__select">
      <option value="red" selected>Red</op▮>
      <option value="blue">Blue</option>
    </select>
    <video src="/intro.mp4" controls autoplay muted></video>
  </section>

```

**Palavra que a pessoa ia digitar:** `option`: aparece em 2º lugar de 2

**Saída** (as 20 primeiras sugestões):

```text
 1 optgroup
 2 option
```

**Veredito:** ✅ Bom. `optgroup`, `option`.

---

### HTML-75
<a id="html-75"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 8 · **lugar na gramática:** `html:value:value`

**Entrada** (a string onde está o cursor):

```text

  <section class="card" id="featured" role="region" tabindex="0">
    <h3 class="card__title">${title}</h3>
    <p class="card__body">${body}</p>
    <textarea name="note" rows="3" cols="40" placeholder="Add a note"></textarea>
    <select name="color" class="card__select">
      <option value="red" selected>Red</option>
      <option value="blu▮">Blue</option>
    </select>
    <video src="/intro.mp4" controls autoplay muted></video>
  </section>

```

**Palavra que a pessoa ia digitar:** `blue`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 Blue [a]
```

**Veredito:** ✅ Bom. `Blue` é o único item.

---

### HTML-76
<a id="html-76"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 8 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <section class="card" id="featured" role="region" tabindex="0">
    <h3 class="card__title">${title}</h3>
    <p class="card__body">${body}</p>
    <textarea name="note" rows="3" cols="40" placeholder="Add a note"></textarea>
    <select name="color" class="card__select">
      <option value="red" selected>Red</option>
      <option value="blue">Blue</▮>
    </select>
    <video src="/intro.mp4" controls autoplay muted></video>
  </section>

```

**Palavra que a pessoa ia digitar:** `option`: aparece em 78º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 a            11 body
 2 abbr         12 br
 3 address      13 button
 4 area         14 canvas
 5 article      15 caption
 6 aside        16 circle
 7 audio        17 cite
 8 b            18 clipPath
 9 base         19 code
10 blockquote   20 col
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `</`: tags em ordem alfabética; `option` é o 78º.

---

### HTML-77
<a id="html-77"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 10 · **lugar na gramática:** `html:attribute:video`

**Entrada** (a string onde está o cursor):

```text

  <section class="card" id="featured" role="region" tabindex="0">
    <h3 class="card__title">${title}</h3>
    <p class="card__body">${body}</p>
    <textarea name="note" rows="3" cols="40" placeholder="Add a note"></textarea>
    <select name="color" class="card__select">
      <option value="red" selected>Red</option>
      <option value="blue">Blue</option>
    </select>
    <video s▮="/intro.mp4" controls autoplay muted></video>
  </section>

```

**Palavra que a pessoa ia digitar:** `src`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 src            11 small
 2 style          12 source
 3 slot           13 span
 4 spellcheck     14 stop
 5 select [a]     15 strong
 6 section [a]    16 sub
 7 selected [a]   17 summary
 8 samp           18 sup
 9 script         19 svg
10 search         20 symbol
```

**Veredito:** ✅ Bom. `src` em primeiro.

---

### HTML-78
<a id="html-78"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D4, linha 10 · **lugar na gramática:** `html:value:src`

**Entrada** (a string onde está o cursor):

```text

  <section class="card" id="featured" role="region" tabindex="0">
    <h3 class="card__title">${title}</h3>
    <p class="card__body">${body}</p>
    <textarea name="note" rows="3" cols="40" placeholder="Add a note"></textarea>
    <select name="color" class="card__select">
      <option value="red" selected>Red</option>
      <option value="blue">Blue</option>
    </select>
    <video src="/intro.mp▮" controls autoplay muted></video>
  </section>

```

**Palavra que a pessoa ia digitar:** `mp4`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido dentro de uma URL.

---

### HTML-79
<a id="html-79"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 10 · **lugar na gramática:** `html:attribute:video`

**Entrada** (a string onde está o cursor):

```text

  <section class="card" id="featured" role="region" tabindex="0">
    <h3 class="card__title">${title}</h3>
    <p class="card__body">${body}</p>
    <textarea name="note" rows="3" cols="40" placeholder="Add a note"></textarea>
    <select name="color" class="card__select">
      <option value="red" selected>Red</option>
      <option value="blue">Blue</option>
    </select>
    <video src="/intro.mp4" controls autoplay mut▮></video>
  </section>

```

**Palavra que a pessoa ia digitar:** `muted`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 muted
```

**Veredito:** ✅ Bom. `muted` é o único item.

---

### HTML-80
<a id="html-80"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D4, linha 11 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <section class="card" id="featured" role="region" tabindex="0">
    <h3 class="card__title">${title}</h3>
    <p class="card__body">${body}</p>
    <textarea name="note" rows="3" cols="40" placeholder="Add a note"></textarea>
    <select name="color" class="card__select">
      <option value="red" selected>Red</option>
      <option value="blue">Blue</option>
    </select>
    <video src="/intro.mp4" controls autoplay muted></video>
  </scetio▮>

```

**Palavra que a pessoa ia digitar:** `section`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 section [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `section`.

---

### HTML-81
<a id="html-81"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 2 · **lugar na gramática:** `html:value:xmlns`

**Entrada** (a string onde está o cursor):

```text

  <svg xmlns="h▮://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24" fill="none" stroke="currentColor">
    <circle cx="12" cy="12" r="10"/>
    <path d="M12 6v6l4 2" stroke-width="2"/>
    <rect x="3" y="3" width="18" height="18" rx="2"/>
    <line x1="4" y1="4" x2="20" y2="20"/>
  </svg>

```

**Palavra que a pessoa ia digitar:** `http`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 height [a]   11 hgroup
 2 html [a]     12 hr
 3 h1
 4 h2
 5 h3
 6 h4
 7 h5
 8 h6
 9 head
10 header
```

**Veredito:** ⚠️ Razoável, com ressalva. Dentro de uma URL: `height`, `html` e as tags h são oferecidas; nada ali é útil.

---

### HTML-82
<a id="html-82"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 2 · **lugar na gramática:** `html:value:xmlns`

**Entrada** (a string onde está o cursor):

```text

  <svg xmlns="http://www.w▮.org/2000/svg" viewBox="0 0 24 24" width="24" height="24" fill="none" stroke="currentColor">
    <circle cx="12" cy="12" r="10"/>
    <path d="M12 6v6l4 2" stroke-width="2"/>
    <rect x="3" y="3" width="18" height="18" rx="2"/>
    <line x1="4" y1="4" x2="20" y2="20"/>
  </svg>

```

**Palavra que a pessoa ia digitar:** `w3`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 www [a]
 2 width [a]
 3 wbr
```

**Veredito:** ⚠️ Razoável, com ressalva. Dentro de uma URL: `www`, `width`, `wbr`; nada ali é útil.

---

### HTML-83
<a id="html-83"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D5, linha 2 · **lugar na gramática:** `html:attribute:svg`

**Entrada** (a string onde está o cursor):

```text

  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" wid▮="24" height="24" fill="none" stroke="currentColor">
    <circle cx="12" cy="12" r="10"/>
    <path d="M12 6v6l4 2" stroke-width="2"/>
    <rect x="3" y="3" width="18" height="18" rx="2"/>
    <line x1="4" y1="4" x2="20" y2="20"/>
  </svg>

```

**Palavra que a pessoa ia digitar:** `width`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 width
```

**Veredito:** ✅ Bom. `width` é o único item.

---

### HTML-84
<a id="html-84"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D5, linha 2 · **lugar na gramática:** `html:attribute:svg`

**Entrada** (a string onde está o cursor):

```text

  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24" ▮="none" stroke="currentColor">
    <circle cx="12" cy="12" r="10"/>
    <path d="M12 6v6l4 2" stroke-width="2"/>
    <rect x="3" y="3" width="18" height="18" rx="2"/>
    <line x1="4" y1="4" x2="20" y2="20"/>
  </svg>

```

**Palavra que a pessoa ia digitar:** `fill`: aparece em 5º lugar de 100

**Saída** (as 20 primeiras sugestões):

```text
 1 viewBox               11 title
 2 xmlns                 12 lang
 3 width                 13 dir
 4 height                14 hidden
 5 fill                  15 tabindex
 6 stroke                16 role
 7 preserveAspectRatio   17 draggable
 8 class                 18 contenteditable
 9 id                    19 accesskey
10 style                 20 slot
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito dentro de uma tag: atributos que a tag já tem vêm primeiro; `fill` é o 5º.

---

### HTML-85
<a id="html-85"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 2 · **lugar na gramática:** `html:value:stroke`

**Entrada** (a string onde está o cursor):

```text

  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24" fill="none" stroke="c▮">
    <circle cx="12" cy="12" r="10"/>
    <path d="M12 6v6l4 2" stroke-width="2"/>
    <rect x="3" y="3" width="18" height="18" rx="2"/>
    <line x1="4" y1="4" x2="20" y2="20"/>
  </svg>

```

**Palavra que a pessoa ia digitar:** `currentColor`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 circle [a]    11 col
 2 confirm [a]   12 colgroup
 3 class [a]
 4 Cancel [a]
 5 const [a]
 6 canvas
 7 caption
 8 cite
 9 clipPath
10 code
```

**Veredito:** ⚠️ Razoável, com ressalva. `stroke="c`: nomes de cor não são oferecidos; `circle`, `confirm`, `class` são ruído.

---

### HTML-86
<a id="html-86"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 3 · **lugar na gramática:** `html:attribute:circle`

**Entrada** (a string onde está o cursor):

```text

  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24" fill="none" stroke="currentColor">
    <circle c▮="12" cy="12" r="10"/>
    <path d="M12 6v6l4 2" stroke-width="2"/>
    <rect x="3" y="3" width="18" height="18" rx="2"/>
    <line x1="4" y1="4" x2="20" y2="20"/>
  </svg>

```

**Palavra que a pessoa ia digitar:** `cx`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 cx                 11 caption
 2 cy                 12 cite
 3 class              13 clipPath
 4 contenteditable    14 code
 5 circle [a]         15 col
 6 currentColor [a]   16 colgroup
 7 confirm [a]
 8 Cancel [a]
 9 const [a]
10 canvas
```

**Veredito:** ✅ Bom. `cx`, `cy` primeiro.

---

### HTML-87
<a id="html-87"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D5, linha 4 · **lugar na gramática:** `html:value:d`

**Entrada** (a string onde está o cursor):

```text

  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24" fill="none" stroke="currentColor">
    <circle cx="12" cy="12" r="10"/>
    <path d="M1▮ 6v6l4 2" stroke-width="2"/>
    <rect x="3" y="3" width="18" height="18" rx="2"/>
    <line x1="4" y1="4" x2="20" y2="20"/>
  </svg>

```

**Palavra que a pessoa ia digitar:** `M12`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido dentro de dados de path.

---

### HTML-88
<a id="html-88"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D5, linha 5 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24" fill="none" stroke="currentColor">
    <circle cx="12" cy="12" r="10"/>
    <path d="M12 6v6l4 2" stroke-width="2"/>
    <▮ x="3" y="3" width="18" height="18" rx="2"/>
    <line x1="4" y1="4" x2="20" y2="20"/>
  </svg>

```

**Palavra que a pessoa ia digitar:** `rect`: aparece em 89º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 a            11 body
 2 abbr         12 br
 3 address      13 button
 4 area         14 canvas
 5 article      15 caption
 6 aside        16 circle
 7 audio        17 cite
 8 b            18 clipPath
 9 base         19 code
10 blockquote   20 col
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `<`: tags em ordem alfabética; `rect` é o 89º.

---

### HTML-89
<a id="html-89"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 5 · **lugar na gramática:** `html:attribute:rect`

**Entrada** (a string onde está o cursor):

```text

  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24" fill="none" stroke="currentColor">
    <circle cx="12" cy="12" r="10"/>
    <path d="M12 6v6l4 2" stroke-width="2"/>
    <rect x="3" y="3" width="18" height="18" r▮="2"/>
    <line x1="4" y1="4" x2="20" y2="20"/>
  </svg>

```

**Palavra que a pessoa ia digitar:** `rx`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 rx
 2 ry
 3 role
 4 rect [a]
 5 radialGradient
 6 rp
 7 rt
 8 ruby
```

**Veredito:** ✅ Bom. `rx`, `ry` primeiro.

---

### HTML-90
<a id="html-90"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 6 · **lugar na gramática:** `html:attribute:line`

**Entrada** (a string onde está o cursor):

```text

  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24" fill="none" stroke="currentColor">
    <circle cx="12" cy="12" r="10"/>
    <path d="M12 6v6l4 2" stroke-width="2"/>
    <rect x="3" y="3" width="18" height="18" rx="2"/>
    <line x▮="4" y1="4" x2="20" y2="20"/>
  </svg>

```

**Palavra que a pessoa ia digitar:** `x1`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 x1
 2 x2
 3 xmlns [a]
```

**Veredito:** ✅ Bom. `x1`, `x2` primeiro.

---

### HTML-91
<a id="html-91"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 6 · **lugar na gramática:** `html:attribute:line`

**Entrada** (a string onde está o cursor):

```text

  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24" fill="none" stroke="currentColor">
    <circle cx="12" cy="12" r="10"/>
    <path d="M12 6v6l4 2" stroke-width="2"/>
    <rect x="3" y="3" width="18" height="18" rx="2"/>
    <line x1="4" y1="4" x2="20" y▮="20"/>
  </svg>

```

**Palavra que a pessoa ia digitar:** `y2`: aparece em 2º lugar de 3

**Saída** (as 20 primeiras sugestões):

```text
 1 y1
 2 y2
 3 you [a]
```

**Veredito:** ✅ Bom. `y2` em 2º, depois de `y1`.

---

### HTML-92
<a id="html-92"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D5, linha 11 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <▮ id="confirm" class="dialog">
    <p>Are you sure?</p>
    <button class="dialog__ok" type="button">OK</button>
    <button class="dialog__cancel" type="button">Cancel</button>
  </dialog>

```

**Palavra que a pessoa ia digitar:** `dialog`: aparece em 29º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 a            11 body
 2 abbr         12 br
 3 address      13 button
 4 area         14 canvas
 5 article      15 caption
 6 aside        16 circle
 7 audio        17 cite
 8 b            18 clipPath
 9 base         19 code
10 blockquote   20 col
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `<`: tags em ordem alfabética; `dialog` é o 29º.

---

### HTML-93
<a id="html-93"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 11 · **lugar na gramática:** `html:attribute:dialog`

**Entrada** (a string onde está o cursor):

```text

  <dialog id="confirm" c▮="dialog">
    <p>Are you sure?</p>
    <button class="dialog__ok" type="button">OK</button>
    <button class="dialog__cancel" type="button">Cancel</button>
  </dialog>

```

**Palavra que a pessoa ia digitar:** `class`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 class              11 clipPath
 2 contenteditable    12 code
 3 confirm [a]        13 col
 4 Cancel [a]         14 colgroup
 5 circle [a]
 6 currentColor [a]
 7 const [a]
 8 canvas
 9 caption
10 cite
```

**Veredito:** ✅ Bom. `class` em primeiro.

---

### HTML-94
<a id="html-94"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D5, linha 12 · **lugar na gramática:** `html:text`

**Entrada** (a string onde está o cursor):

```text

  <dialog id="confirm" class="dialog">
    <p>Ar▮ you sure?</p>
    <button class="dialog__ok" type="button">OK</button>
    <button class="dialog__cancel" type="button">Cancel</button>
  </dialog>

```

**Palavra que a pessoa ia digitar:** `Are`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 area
 2 article
```

**Veredito:** ⚠️ Razoável, com ressalva. Texto `Ar`: só nomes de tag (area, article) são oferecidos num nó de texto.

---

### HTML-95
<a id="html-95"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D5, linha 13 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <dialog id="confirm" class="dialog">
    <p>Are you sure?</p>
    <but▮ class="dialog__ok" type="button">OK</button>
    <button class="dialog__cancel" type="button">Cancel</button>
  </dialog>

```

**Palavra que a pessoa ia digitar:** `button`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 button
```

**Veredito:** ✅ Bom. `button` é o único item.

---

### HTML-96
<a id="html-96"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D5, linha 13 · **lugar na gramática:** `html:value:class`

**Entrada** (a string onde está o cursor):

```text

  <dialog id="confirm" class="dialog">
    <p>Are you sure?</p>
    <button class="▮" type="button">OK</button>
    <button class="dialog__cancel" type="button">Cancel</button>
  </dialog>

```

**Palavra que a pessoa ia digitar:** `dialog__ok`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 dialog               11 svg [a]
 2 dialog__cancel [a]   12 line [a]
 3 class [a]            13 height [a]
 4 type [a]             14 width [a]
 5 button [a]           15 rect [a]
 6 sure [a]             16 stroke-width [a]
 7 you [a]              17 v6l4 [a]
 8 Are [a]              18 M12 [a]
 9 confirm [a]          19 path [a]
10 Cancel [a]           20 circle [a]
```

**Veredito:** ✅ Bom. Nomes de classe primeiro (`dialog`, `dialog__cancel`), depois ruído.

---

### HTML-97
<a id="html-97"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 13 · **lugar na gramática:** `html:text`

**Entrada** (a string onde está o cursor):

```text

  <dialog id="confirm" class="dialog">
    <p>Are you sure?</p>
    <button class="dialog__ok" type="button">O▮</button>
    <button class="dialog__cancel" type="button">Cancel</button>
  </dialog>

```

**Palavra que a pessoa ia digitar:** `OK`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 org [a]
 2 object
 3 ol
 4 optgroup
 5 option
 6 output
```

**Veredito:** ⚠️ Razoável, com ressalva. Texto `O`: `org` e tags (object, ol, option...) não são o que um texto precisa.

---

### HTML-98
<a id="html-98"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D5, linha 14 · **lugar na gramática:** `html:tag`

**Entrada** (a string onde está o cursor):

```text

  <dialog id="confirm" class="dialog">
    <p>Are you sure?</p>
    <button class="dialog__ok" type="button">OK</button>
    <bu▮ class="dialog__cancel" type="button">Cancel</button>
  </dialog>

```

**Palavra que a pessoa ia digitar:** `button`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 button
```

**Veredito:** ✅ Bom. `button` é o único item.

---

### HTML-99
<a id="html-99"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D5, linha 14 · **lugar na gramática:** `html:attribute:button`

**Entrada** (a string onde está o cursor):

```text

  <dialog id="confirm" class="dialog">
    <p>Are you sure?</p>
    <button class="dialog__ok" type="button">OK</button>
    <button class="dialog__cancel" typ▮="button">Cancel</button>
  </dialog>

```

**Palavra que a pessoa ia digitar:** `type`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 type
```

**Veredito:** ✅ Bom. `type` é o único item.

---

### HTML-100
<a id="html-100"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D5, linha 14 · **lugar na gramática:** `html:text`

**Entrada** (a string onde está o cursor):

```text

  <dialog id="confirm" class="dialog">
    <p>Are you sure?</p>
    <button class="dialog__ok" type="button">OK</button>
    <button class="dialog__cancel" type="button">Cnace▮</button>
  </dialog>

```

**Palavra que a pessoa ia digitar:** `Cancel`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; a palavra não aparece em outro lugar do arquivo.

---
