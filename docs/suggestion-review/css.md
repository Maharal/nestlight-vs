# CSS: 100 exemplos

Resultado: ✅ 58 bons · ⚠️ 30 razoáveis com ressalva · ❌ 12 ruins.

Como ler: em cada exemplo, `▮` marca onde está o cursor. A lista é o que o plugin mostraria (as 20 primeiras). `[a]` = palavra que já existe no arquivo; `[~]` = sugestão "parecida" (corrige erro de digitação); sem marca = palavra-chave da linguagem. O veredito e o comentário são a minha análise. "Lugar na gramática" é o nome interno da regra de posição que o plugin aplicou (`sql:table`, `css:value:display`...); `(no rule)` quer dizer que o plugin não tem regra para aquele lugar e usa só o que foi digitado.

## Índice (para varrer rápido)

| # | Situação | Digitado | Palavra procurada | Posição | Lugar na gramática | Veredito |
|---|---|---|---|---|---|---|
| [1](#css-1) | 1 letra | `b` | `button` | 5 | `css:selector` | ⚠️ |
| [2](#css-2) | 2 letras | `po` | `position` | 2 | `css:property` | ✅ |
| [3](#css-3) | 3 letras | `rel` | `relative` | 1 | `css:value:position` | ✅ |
| [4](#css-4) | Ctrl+Espaço | (nada) | `display` | 71 | `css:property` | ❌ |
| [5](#css-5) | 1 letra | `i` | `inline-block` | 2 | `css:value:display` | ✅ |
| [6](#css-6) | 2 letras | `bo` | `border` | 1 | `css:property` | ✅ |
| [7](#css-7) | 3 letras | `sol` | `solid` | 1 | `css:value:border` | ✅ |
| [8](#css-8) | Ctrl+Espaço | (nada) | `transparent` | 22 | `css:value:border` | ⚠️ |
| [9](#css-9) | 1 letra | `b` | `border-radius` | 27 | `css:property` | ⚠️ |
| [10](#css-10) | erro: faltando | `bakgr` | `background` | 1 | `css:property` | ✅ |
| [11](#css-11) | 3 letras | `acc` | `accent` | — | `(no rule)` | ✅ |
| [12](#css-12) | Ctrl+Espaço | (nada) | `color` | 62 | `css:property` | ❌ |
| [13](#css-13) | 1 letra | `w` | `white` | 1 | `css:value:color` | ✅ |
| [14](#css-14) | 2 letras | `cu` | `cursor` | 1 | `css:property` | ✅ |
| [15](#css-15) | 3 letras | `poi` | `pointer` | 1 | `css:value:cursor` | ✅ |
| [16](#css-16) | Ctrl+Espaço | (nada) | `background-color` | 70 | `css:value:transition` | ❌ |
| [17](#css-17) | 1 letra | `e` | `ease-in-out` | — | `css:value:transition` | ⚠️ |
| [18](#css-18) | 2 letras | `tr` | `transform` | 3 | `css:value:transition` | ⚠️ |
| [19](#css-19) | 3 letras | `but` | `button` | 1 | `css:selector` | ✅ |
| [20](#css-20) | erro: trocadas | `hvoe` | `hover` | — | `css:selector` | ⚠️ |
| [21](#css-21) | 1 letra | `v` | `var` | 1 | `css:value:background` | ✅ |
| [22](#css-22) | 2 letras | `ac` | `accent-dark` | — | `(no rule)` | ✅ |
| [23](#css-23) | 3 letras | `tra` | `transform` | 1 | `css:property` | ✅ |
| [24](#css-24) | Ctrl+Espaço | (nada) | `translateY` | — | `css:value:transform` | ⚠️ |
| [25](#css-25) | 1 letra | `b` | `button` | 5 | `css:selector` | ⚠️ |
| [26](#css-26) | 2 letras | `op` | `opacity` | 1 | `css:property` | ✅ |
| [27](#css-27) | 3 letras | `cur` | `cursor` | 1 | `css:property` | ✅ |
| [28](#css-28) | Ctrl+Espaço | (nada) | `not-allowed` | 6 | `css:value:cursor` | ✅ |
| [29](#css-29) | 1 letra | `g` | `gallery` | 4 | `css:selector` | ⚠️ |
| [30](#css-30) | erro: faltando | `dipla` | `display` | 1 | `css:property` | ✅ |
| [31](#css-31) | 3 letras | `gri` | `grid-template-columns` | 15 | `css:property` | ⚠️ |
| [32](#css-32) | Ctrl+Espaço | (nada) | `repeat` | — | `css:value:grid-template-columns` | ⚠️ |
| [33](#css-33) | 1 letra | `a` | `auto-fill` | — | `(no rule)` | ⚠️ |
| [34](#css-34) | 2 letras | `mi` | `minmax` | — | `(no rule)` | ⚠️ |
| [35](#css-35) | 2 letras | `ga` | `gap` | 1 | `css:property` | ✅ |
| [36](#css-36) | Ctrl+Espaço | (nada) | `auto` | 6 | `css:value:margin` | ⚠️ |
| [37](#css-37) | 1 letra | `m` | `max-width` | 8 | `css:property` | ⚠️ |
| [38](#css-38) | 2 letras | `ga` | `gallery` | 2 | `css:selector` | ⚠️ |
| [39](#css-39) | 2 letras | `im` | `img` | 1 | `css:selector` | ✅ |
| [40](#css-40) | erro: trocadas | `wdit` | `width` | 2 | `css:property` | ⚠️ |
| [41](#css-41) | 1 letra | `a` | `auto` | 1 | `css:value:height` | ✅ |
| [42](#css-42) | 2 letras | `ob` | `object-fit` | 1 | `css:property` | ✅ |
| [43](#css-43) | 3 letras | `cov` | `cover` | 1 | `css:value:object-fit` | ✅ |
| [44](#css-44) | Ctrl+Espaço | (nada) | `border-radius` | 41 | `css:property` | ❌ |
| [45](#css-45) | 1 letra | `m` | `max-width` | 8 | `(no rule)` | ⚠️ |
| [46](#css-46) | 2 letras | `gr` | `grid-template-columns` | 15 | `css:property` | ⚠️ |
| [47](#css-47) | 2 letras | `ga` | `gap` | 1 | `css:property` | ✅ |
| [48](#css-48) | Ctrl+Espaço | (nada) | `fade-in` | fora | `css:selector` | ❌ |
| [49](#css-49) | 1 letra | `f` | `from` | — | `css:property` | ❌ |
| [50](#css-50) | erro: faltando | `opcit` | `opacity` | 1 | `css:property` | ✅ |
| [51](#css-51) | 3 letras | `sca` | `scale` | 1 | `css:value:transform` | ✅ |
| [52](#css-52) | Ctrl+Espaço | (nada) | `to` | — | `css:property` | ❌ |
| [53](#css-53) | 1 letra | `o` | `opacity` | 3 | `css:property` | ⚠️ |
| [54](#css-54) | 2 letras | `tr` | `transform` | 1 | `css:property` | ✅ |
| [55](#css-55) | 3 letras | `sca` | `scale` | 1 | `css:value:transform` | ✅ |
| [56](#css-56) | Ctrl+Espaço | (nada) | `position` | fora | `css:property` | ❌ |
| [57](#css-57) | 1 letra | `f` | `fixed` | 1 | `css:value:position` | ✅ |
| [58](#css-58) | 2 letras | `to` | `top` | 1 | `css:property` | ✅ |
| [59](#css-59) | 3 letras | `lef` | `left` | 1 | `css:property` | ✅ |
| [60](#css-60) | erro: trocadas | `zi-nde` | `z-index` | 1 | `css:property` | ✅ |
| [61](#css-61) | 1 letra | `h` | `hidden` | 1 | `css:value:overflow` | ✅ |
| [62](#css-62) | 2 letras | `an` | `animation` | 1 | `css:property` | ✅ |
| [63](#css-63) | 3 letras | `fad` | `fade-in` | 1 | `css:value:animation` | ✅ |
| [64](#css-64) | Ctrl+Espaço | (nada) | `ease-out` | — | `css:value:animation` | ⚠️ |
| [65](#css-65) | 1 letra | `b` | `box-shadow` | 42 | `css:property` | ⚠️ |
| [66](#css-66) | 2 letras | `mo` | `modal__title` | — | `css:selector` | ✅ |
| [67](#css-67) | 3 letras | `fon` | `font-size` | 5 | `css:property` | ✅ |
| [68](#css-68) | Ctrl+Espaço | (nada) | `font-weight` | 92 | `css:property` | ❌ |
| [69](#css-69) | 1 letra | `b` | `bold` | 1 | `css:value:font-weight` | ✅ |
| [70](#css-70) | erro: faltando | `tet-a` | `text-align` | 1 | `css:property` | ✅ |
| [71](#css-71) | 3 letras | `tex` | `text-transform` | 10 | `css:property` | ⚠️ |
| [72](#css-72) | Ctrl+Espaço | (nada) | `uppercase` | 2 | `css:value:text-transform` | ✅ |
| [73](#css-73) | 1 letra | `l` | `letter-spacing` | 2 | `css:property` | ✅ |
| [74](#css-74) | 2 letras | `bo` | `body` | 1 | `css:selector` | ✅ |
| [75](#css-75) | 3 letras | `mar` | `margin` | 1 | `css:property` | ✅ |
| [76](#css-76) | Ctrl+Espaço | (nada) | `text-decoration` | fora | `css:property` | ❌ |
| [77](#css-77) | 1 letra | `n` | `none` | 1 | `css:value:text-decoration` | ✅ |
| [78](#css-78) | 2 letras | `wo` | `word-break` | 1 | `css:property` | ✅ |
| [79](#css-79) | 3 letras | `bre` | `break-word` | 2 | `css:value:word-break` | ✅ |
| [80](#css-80) | erro: trocadas | `wihte-` | `white-space` | 1 | `css:property` | ✅ |
| [81](#css-81) | 1 letra | `t` | `text-overflow` | 10 | `css:property` | ⚠️ |
| [82](#css-82) | 2 letras | `el` | `ellipsis` | — | `css:value:text-overflow` | ⚠️ |
| [83](#css-83) | 2 letras | `na` | `nav` | 1 | `css:selector` | ✅ |
| [84](#css-84) | Ctrl+Espaço | (nada) | `ul` | — | `css:selector` | ⚠️ |
| [85](#css-85) | 1 letra | `l` | `list-style` | 4 | `css:property` | ✅ |
| [86](#css-86) | 2 letras | `di` | `display` | 2 | `css:property` | ✅ |
| [87](#css-87) | 3 letras | `fle` | `flex` | 1 | `css:value:display` | ✅ |
| [88](#css-88) | Ctrl+Espaço | (nada) | `flex-wrap` | 81 | `css:property` | ❌ |
| [89](#css-89) | 1 letra | `w` | `wrap` | 1 | `css:value:flex-wrap` | ✅ |
| [90](#css-90) | erro: faltando | `viibi` | `visibility` | 1 | `css:property` | ✅ |
| [91](#css-91) | 3 letras | `inp` | `input` | 1 | `css:selector` | ✅ |
| [92](#css-92) | Ctrl+Espaço | (nada) | `type` | — | `css:selector` | ⚠️ |
| [93](#css-93) | 1 letra | `t` | `text` | 5 | `css:selector` | ⚠️ |
| [94](#css-94) | 2 letras | `fo` | `focus` | — | `css:selector` | ❌ |
| [95](#css-95) | 3 letras | `out` | `outline` | 1 | `css:property` | ✅ |
| [96](#css-96) | Ctrl+Espaço | (nada) | `var` | — | `css:value:outline` | ⚠️ |
| [97](#css-97) | 1 letra | `a` | `accent` | — | `(no rule)` | ✅ |
| [98](#css-98) | 2 letras | `bo` | `box-sizing` | 31 | `css:property` | ⚠️ |
| [99](#css-99) | 3 letras | `bor` | `border-box` | 1 | `css:value:box-sizing` | ✅ |
| [100](#css-100) | erro: trocadas | `pionte` | `pointer-events` | 1 | `css:property` | ✅ |

Posição: lugar da palavra procurada na lista; `—` = a palavra não existe em outro lugar do arquivo; `fora` = existe mas não está na lista.

## Os arquivos usados como entrada

Escritos à mão como um desenvolvedor escreveria (código JavaScript com strings da linguagem). Nada foi gerado pelo gerador dos experimentos.

### Documento D1

```js
const Layout = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: space-between;
  min-height: 100vh;
  padding: 16px 24px;
  background-color: #fafafa;
  color: #222;
  font-family: system-ui, sans-serif;
`;
```

### Documento D2

```js
export const buttonStyles = css`
  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }
`;
```

### Documento D3

```js
const grid = css`
  .gallery {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 12px;
    margin: 0 auto;
    max-width: 1200px;
  }
  .gallery img {
    width: 100%;
    height: auto;
    object-fit: cover;
    border-radius: 8px;
  }
  @media (max-width: 600px) {
    .gallery {
      grid-template-columns: 1fr;
      gap: 8px;
    }
  }
`;
```

### Documento D4

```js
const motion = css`
  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }
`;
```

### Documento D5

```js
const typography = css`
  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }
`;
```

## Os exemplos

### CSS-1
<a id="css-1"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 2 · **lugar na gramática:** `css:selector`

**Entrada** (a string onde está o cursor):

```text

  .b▮ {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `button`: aparece em 5º lugar de 50

**Saída** (as 20 primeiras sugestões):

```text
 1 base                   11 backdrop-filter
 2 blockquote             12 backface-visibility
 3 body                   13 background-attachment
 4 br                     14 background-blend-mode
 5 button                 15 background-clip
 6 border [a]             16 background-image
 7 border-radius [a]      17 background-origin
 8 background [a]         18 background-position
 9 background-color [a]   19 background-repeat
10 buttonStyles [a]       20 background-size
```

**Veredito:** ⚠️ Razoável, com ressalva. `.b`: depois de um ponto espera-se um nome de classe, mas tags HTML (base, blockquote, body...) vêm primeiro; `button` é o 5º.

---

### CSS-2
<a id="css-2"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 3 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  .button {
    po▮: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `position`: aparece em 2º lugar de 3

**Saída** (as 20 primeiras sugestões):

```text
 1 pointer-events
 2 position
 3 pointer [a]
```

**Veredito:** ✅ Bom. `pointer-events`, `position`.

---

### CSS-3
<a id="css-3"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 3 · **lugar na gramática:** `css:value:position`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: rel▮;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `relative`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 relative
```

**Veredito:** ✅ Bom. `relative` é o único item.

---

### CSS-4
<a id="css-4"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 4 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    ▮: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `display`: aparece em 71º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 align-content               11 animation-name
 2 align-items                 12 animation-play-state
 3 align-self                  13 animation-timing-function
 4 all                         14 aspect-ratio
 5 animation                   15 backdrop-filter
 6 animation-delay             16 backface-visibility
 7 animation-direction         17 background
 8 animation-duration          18 background-attachment
 9 animation-fill-mode         19 background-blend-mode
10 animation-iteration-count   20 background-clip
```

**Veredito:** ❌ Ruim. Pedido explícito numa posição de propriedade vazia: propriedades em ordem alfabética cortadas em 100; `display` é o 71º.

---

### CSS-5
<a id="css-5"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 4 · **lugar na gramática:** `css:value:display`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: i▮;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `inline-block`: aparece em 2º lugar de 8

**Saída** (as 20 primeiras sugestões):

```text
 1 inline
 2 inline-block
 3 inline-flex
 4 inline-grid
 5 inherit
 6 initial
 7 inset
 8 isolation
```

**Veredito:** ✅ Bom. `inline`, `inline-block` (valores de display).

---

### CSS-6
<a id="css-6"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 6 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    bo▮: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `border`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 border                       11 border-left
 2 border-bottom                12 border-left-color
 3 border-bottom-color          13 border-left-style
 4 border-bottom-left-radius    14 border-left-width
 5 border-bottom-right-radius   15 border-radius
 6 border-bottom-style          16 border-right
 7 border-bottom-width          17 border-right-color
 8 border-collapse              18 border-right-style
 9 border-color                 19 border-right-width
10 border-image                 20 border-spacing
```

**Veredito:** ✅ Bom. `border` em primeiro.

---

### CSS-7
<a id="css-7"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 6 · **lugar na gramática:** `css:value:border`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px sol▮ transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `solid`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 solid
```

**Veredito:** ✅ Bom. `solid` é o único item.

---

### CSS-8
<a id="css-8"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 6 · **lugar na gramática:** `css:value:border`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid ▮;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `transparent`: aparece em 22º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 inherit        11 grid
 2 initial        12 absolute
 3 unset          13 relative
 4 revert         14 fixed
 5 none           15 sticky
 6 auto           16 hidden
 7 block          17 visible
 8 inline         18 solid
 9 inline-block   19 dashed
10 flex           20 dotted
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `solid`: lista genérica de valores; `transparent` é o 22º.

---

### CSS-9
<a id="css-9"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 7 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    b▮: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `border-radius`: aparece em 27º lugar de 46 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 backdrop-filter         11 background-repeat
 2 backface-visibility     12 background-size
 3 background              13 border
 4 background-attachment   14 border-bottom
 5 background-blend-mode   15 border-bottom-color
 6 background-clip         16 border-bottom-left-radius
 7 background-color        17 border-bottom-right-radius
 8 background-image        18 border-bottom-style
 9 background-origin       19 border-bottom-width
10 background-position     20 border-collapse
```

**Veredito:** ⚠️ Razoável, com ressalva. `b`: `border-radius` é o 27º numa lista alfabética.

---

### CSS-10
<a id="css-10"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D2, linha 8 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    bakgr▮: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `background`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 background [~]
 2 background-attachment [~]
 3 background-blend-mode [~]
 4 background-clip [~]
 5 background-color [~]
 6 background-image [~]
 7 background-origin [~]
 8 background-position [~]
 9 background-repeat [~]
10 background-size [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `background` em primeiro.

---

### CSS-11
<a id="css-11"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--acc▮);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `accent`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 --accent-dark [a]
```

**Veredito:** ✅ Bom. `--accent-dark` oferecido para `var(--acc`.

---

### CSS-12
<a id="css-12"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 9 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    ▮: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `color`: aparece em 62º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 align-content               11 animation-name
 2 align-items                 12 animation-play-state
 3 align-self                  13 animation-timing-function
 4 all                         14 aspect-ratio
 5 animation                   15 backdrop-filter
 6 animation-delay             16 backface-visibility
 7 animation-direction         17 background
 8 animation-duration          18 background-attachment
 9 animation-fill-mode         19 background-blend-mode
10 animation-iteration-count   20 background-clip
```

**Veredito:** ❌ Ruim. Pedido explícito numa posição de propriedade vazia: `color` é o 62º.

---

### CSS-13
<a id="css-13"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 9 · **lugar na gramática:** `css:value:color`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: w▮;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `white`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 white
 2 white-space
 3 widows
 4 width
 5 will-change
 6 word-break
 7 word-spacing
 8 word-wrap
 9 writing-mode
```

**Veredito:** ✅ Bom. `white` em primeiro.

---

### CSS-14
<a id="css-14"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 10 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cu▮: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `cursor`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 cursor
 2 currentColor
```

**Veredito:** ✅ Bom. `cursor` em primeiro.

---

### CSS-15
<a id="css-15"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 10 · **lugar na gramática:** `css:value:cursor`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: poi▮;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `pointer`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 pointer
 2 pointer-events
```

**Veredito:** ✅ Bom. `pointer` em primeiro.

---

### CSS-16
<a id="css-16"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 11 · **lugar na gramática:** `css:value:transition`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: ▮ 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `background-color`: aparece em 70º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 inherit        11 grid
 2 initial        12 absolute
 3 unset          13 relative
 4 revert         14 fixed
 5 none           15 sticky
 6 auto           16 hidden
 7 block          17 visible
 8 inline         18 solid
 9 inline-block   19 dashed
10 flex           20 dotted
```

**Veredito:** ❌ Ruim. Pedido explícito em `transition:`: lista genérica de valores; `background-color` é o 70º (nomes de propriedade não são oferecidos depois de `transition`).

---

### CSS-17
<a id="css-17"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 11 · **lugar na gramática:** `css:value:transition`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s e▮, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `ease-in-out`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 export [a]
 2 empty-cells
```

**Veredito:** ⚠️ Razoável, com ressalva. `e` numa transição: `export` do código hospedeiro e `empty-cells`; falta `ease-in-out` (funções de temporização não estão no vocabulário).

---

### CSS-18
<a id="css-18"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 11 · **lugar na gramática:** `css:value:transition`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, tr▮ 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `transform`: aparece em 3º lugar de 11

**Saída** (as 20 primeiras sugestões):

```text
 1 transparent                  11 translate
 2 transition [a]
 3 transform [a]
 4 translateY [a]
 5 transform-origin
 6 transform-style
 7 transition-delay
 8 transition-duration
 9 transition-property
10 transition-timing-function
```

**Veredito:** ⚠️ Razoável, com ressalva. `tr` numa transição: `transparent` primeiro, `transform` em 3º.

---

### CSS-19
<a id="css-19"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 13 · **lugar na gramática:** `css:selector`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .but▮:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `button`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 button
 2 buttonStyles [a]
```

**Veredito:** ✅ Bom. `button` em primeiro.

---

### CSS-20
<a id="css-20"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D2, linha 13 · **lugar na gramática:** `css:selector`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hvoe▮ {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `hover`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ⚠️ Razoável, com ressalva. Pseudo-classes como `:hover` não estão no vocabulário: nada é oferecido.

---

### CSS-21
<a id="css-21"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 14 · **lugar na gramática:** `css:value:background`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: v▮(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `var`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 var [a]
 2 visible
 3 vertical-align
 4 visibility
```

**Veredito:** ✅ Bom. `var` em primeiro.

---

### CSS-22
<a id="css-22"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 14 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--ac▮);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `accent-dark`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 --accent [a]
```

**Veredito:** ✅ Bom. `--accent` oferecido.

---

### CSS-23
<a id="css-23"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 15 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    tra▮: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `transform`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 transform                    11 transparent [a]
 2 transform-origin
 3 transform-style
 4 transition
 5 transition-delay
 6 transition-duration
 7 transition-property
 8 transition-timing-function
 9 translate
10 translateY [a]
```

**Veredito:** ✅ Bom. `transform` em primeiro.

---

### CSS-24
<a id="css-24"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 15 · **lugar na gramática:** `css:value:transform`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: ▮(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `translateY`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 inherit        11 grid
 2 initial        12 absolute
 3 unset          13 relative
 4 revert         14 fixed
 5 none           15 sticky
 6 auto           16 hidden
 7 block          17 visible
 8 inline         18 solid
 9 inline-block   19 dashed
10 flex           20 dotted
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `transform:`: valores genéricos; faltam as funções de transform (translate, rotate, scale).

---

### CSS-25
<a id="css-25"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 17 · **lugar na gramática:** `css:selector`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .b▮:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `button`: aparece em 5º lugar de 50

**Saída** (as 20 primeiras sugestões):

```text
 1 base                   11 backdrop-filter
 2 blockquote             12 backface-visibility
 3 body                   13 background-attachment
 4 br                     14 background-blend-mode
 5 button                 15 background-clip
 6 background [a]         16 background-image
 7 background-color [a]   17 background-origin
 8 border-radius [a]      18 background-position
 9 border [a]             19 background-repeat
10 buttonStyles [a]       20 background-size
```

**Veredito:** ⚠️ Razoável, com ressalva. `.b:disabled`: tags HTML antes de nomes de classe; `button` em 5º.

---

### CSS-26
<a id="css-26"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 18 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    op▮: 0.5;
    cursor: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `opacity`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 opacity
```

**Veredito:** ✅ Bom. `opacity` é o único item.

---

### CSS-27
<a id="css-27"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 19 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cur▮: not-allowed;
  }

```

**Palavra que a pessoa ia digitar:** `cursor`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 cursor
 2 currentColor
```

**Veredito:** ✅ Bom. `cursor` em primeiro.

---

### CSS-28
<a id="css-28"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 19 · **lugar na gramática:** `css:value:cursor`

**Entrada** (a string onde está o cursor):

```text

  .button {
    position: relative;
    display: inline-block;
    padding: 8px 16px;
    border: 1px solid transparent;
    border-radius: 4px;
    background: var(--accent);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s ease-in-out, transform 0.1s;
  }
  .button:hover {
    background: var(--accent-dark);
    transform: translateY(-1px);
  }
  .button:disabled {
    opacity: 0.5;
    cursor: ▮;
  }

```

**Palavra que a pessoa ia digitar:** `not-allowed`: aparece em 6º lugar de 100

**Saída** (as 20 primeiras sugestões):

```text
 1 pointer [a]   11 auto
 2 default       12 block
 3 text          13 center
 4 move          14 currentColor
 5 wait          15 dashed
 6 not-allowed   16 dotted
 7 grab          17 fixed
 8 crosshair     18 hidden
 9 help          19 inherit
10 absolute      20 initial
```

**Veredito:** ✅ Bom. Valores de cursor, `not-allowed` em 6º.

---

### CSS-29
<a id="css-29"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 2 · **lugar na gramática:** `css:selector`

**Entrada** (a string onde está o cursor):

```text

  .g▮ {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 12px;
    margin: 0 auto;
    max-width: 1200px;
  }
  .gallery img {
    width: 100%;
    height: auto;
    object-fit: cover;
    border-radius: 8px;
  }
  @media (max-width: 600px) {
    .gallery {
      grid-template-columns: 1fr;
      gap: 8px;
    }
  }

```

**Palavra que a pessoa ia digitar:** `gallery`: aparece em 4º lugar de 18

**Saída** (as 20 primeiras sugestões):

```text
 1 grid [a]                    11 grid-column-start
 2 grid-template-columns [a]   12 grid-gap
 3 gap [a]                     13 grid-row
 4 gallery [a]                 14 grid-row-end
 5 grid-area                   15 grid-row-start
 6 grid-auto-columns           16 grid-template
 7 grid-auto-flow              17 grid-template-areas
 8 grid-auto-rows              18 grid-template-rows
 9 grid-column
10 grid-column-end
```

**Veredito:** ⚠️ Razoável, com ressalva. `.g`: as propriedades `grid`, `grid-template-columns` e `gap` vêm antes da classe `gallery` (4ª).

---

### CSS-30
<a id="css-30"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D3, linha 3 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  .gallery {
    dipla▮: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 12px;
    margin: 0 auto;
    max-width: 1200px;
  }
  .gallery img {
    width: 100%;
    height: auto;
    object-fit: cover;
    border-radius: 8px;
  }
  @media (max-width: 600px) {
    .gallery {
      grid-template-columns: 1fr;
      gap: 8px;
    }
  }

```

**Palavra que a pessoa ia digitar:** `display`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 display [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `display`.

---

### CSS-31
<a id="css-31"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 4 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  .gallery {
    display: grid;
    gri▮: repeat(auto-fill, minmax(200px, 1fr));
    gap: 12px;
    margin: 0 auto;
    max-width: 1200px;
  }
  .gallery img {
    width: 100%;
    height: auto;
    object-fit: cover;
    border-radius: 8px;
  }
  @media (max-width: 600px) {
    .gallery {
      grid-template-columns: 1fr;
      gap: 8px;
    }
  }

```

**Palavra que a pessoa ia digitar:** `grid-template-columns`: aparece em 15º lugar de 16

**Saída** (as 20 primeiras sugestões):

```text
 1 grid                11 grid-row-end
 2 grid-area           12 grid-row-start
 3 grid-auto-columns   13 grid-template
 4 grid-auto-flow      14 grid-template-areas
 5 grid-auto-rows      15 grid-template-columns
 6 grid-column         16 grid-template-rows
 7 grid-column-end
 8 grid-column-start
 9 grid-gap
10 grid-row
```

**Veredito:** ⚠️ Razoável, com ressalva. `gri`: `grid-template-columns` é a 15ª de 16 (alfabética), embora o arquivo a use.

---

### CSS-32
<a id="css-32"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 4 · **lugar na gramática:** `css:value:grid-template-columns`

**Entrada** (a string onde está o cursor):

```text

  .gallery {
    display: grid;
    grid-template-columns: ▮(auto-fill, minmax(200px, 1fr));
    gap: 12px;
    margin: 0 auto;
    max-width: 1200px;
  }
  .gallery img {
    width: 100%;
    height: auto;
    object-fit: cover;
    border-radius: 8px;
  }
  @media (max-width: 600px) {
    .gallery {
      grid-template-columns: 1fr;
      gap: 8px;
    }
  }

```

**Palavra que a pessoa ia digitar:** `repeat`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 inherit        11 grid
 2 initial        12 absolute
 3 unset          13 relative
 4 revert         14 fixed
 5 none           15 sticky
 6 auto           16 hidden
 7 block          17 visible
 8 inline         18 solid
 9 inline-block   19 dashed
10 flex           20 dotted
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `grid-template-columns:`: valores genéricos; faltam funções (repeat, minmax).

---

### CSS-33
<a id="css-33"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  .gallery {
    display: grid;
    grid-template-columns: repeat(a▮, minmax(200px, 1fr));
    gap: 12px;
    margin: 0 auto;
    max-width: 1200px;
  }
  .gallery img {
    width: 100%;
    height: auto;
    object-fit: cover;
    border-radius: 8px;
  }
  @media (max-width: 600px) {
    .gallery {
      grid-template-columns: 1fr;
      gap: 8px;
    }
  }

```

**Palavra que a pessoa ia digitar:** `auto-fill`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 absolute              11 animation-iteration-count
 2 align-content         12 animation-name
 3 align-items           13 animation-play-state
 4 align-self            14 animation-timing-function
 5 all                   15 aspect-ratio
 6 animation             16 auto
 7 animation-delay
 8 animation-direction
 9 animation-duration
10 animation-fill-mode
```

**Veredito:** ⚠️ Razoável, com ressalva. `repeat(a`: nomes de propriedade (align-content...) antes de `auto`; `auto-fill` não está no vocabulário.

---

### CSS-34
<a id="css-34"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  .gallery {
    display: grid;
    grid-template-columns: repeat(auto-fill, mi▮(200px, 1fr));
    gap: 12px;
    margin: 0 auto;
    max-width: 1200px;
  }
  .gallery img {
    width: 100%;
    height: auto;
    object-fit: cover;
    border-radius: 8px;
  }
  @media (max-width: 600px) {
    .gallery {
      grid-template-columns: 1fr;
      gap: 8px;
    }
  }

```

**Palavra que a pessoa ia digitar:** `minmax`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 min-height
 2 min-width
 3 mix-blend-mode
```

**Veredito:** ⚠️ Razoável, com ressalva. `mi(`: `minmax` é uma função que falta; min-height/min-width são oferecidas.

---

### CSS-35
<a id="css-35"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 5 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  .gallery {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    ga▮: 12px;
    margin: 0 auto;
    max-width: 1200px;
  }
  .gallery img {
    width: 100%;
    height: auto;
    object-fit: cover;
    border-radius: 8px;
  }
  @media (max-width: 600px) {
    .gallery {
      grid-template-columns: 1fr;
      gap: 8px;
    }
  }

```

**Palavra que a pessoa ia digitar:** `gap`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 gap
 2 gallery [a]
```

**Veredito:** ✅ Bom. `gap` em primeiro.

---

### CSS-36
<a id="css-36"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 6 · **lugar na gramática:** `css:value:margin`

**Entrada** (a string onde está o cursor):

```text

  .gallery {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 12px;
    margin: 0 ▮;
    max-width: 1200px;
  }
  .gallery img {
    width: 100%;
    height: auto;
    object-fit: cover;
    border-radius: 8px;
  }
  @media (max-width: 600px) {
    .gallery {
      grid-template-columns: 1fr;
      gap: 8px;
    }
  }

```

**Palavra que a pessoa ia digitar:** `auto`: aparece em 6º lugar de 100

**Saída** (as 20 primeiras sugestões):

```text
 1 inherit        11 grid
 2 initial        12 absolute
 3 unset          13 relative
 4 revert         14 fixed
 5 none           15 sticky
 6 auto           16 hidden
 7 block          17 visible
 8 inline         18 solid
 9 inline-block   19 dashed
10 flex           20 dotted
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `margin: 0 `: valores genéricos; `auto` é o 6º.

---

### CSS-37
<a id="css-37"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 7 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  .gallery {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 12px;
    margin: 0 auto;
    m▮: 1200px;
  }
  .gallery img {
    width: 100%;
    height: auto;
    object-fit: cover;
    border-radius: 8px;
  }
  @media (max-width: 600px) {
    .gallery {
      grid-template-columns: 1fr;
      gap: 8px;
    }
  }

```

**Palavra que a pessoa ia digitar:** `max-width`: aparece em 8º lugar de 13

**Saída** (as 20 primeiras sugestões):

```text
 1 margin          11 mix-blend-mode
 2 margin-bottom   12 minmax [a]
 3 margin-left     13 media [a]
 4 margin-right
 5 margin-top
 6 mask
 7 max-height
 8 max-width
 9 min-height
10 min-width
```

**Veredito:** ⚠️ Razoável, com ressalva. `m`: `max-width` é o 8º, em ordem alfabética.

---

### CSS-38
<a id="css-38"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 9 · **lugar na gramática:** `css:selector`

**Entrada** (a string onde está o cursor):

```text

  .gallery {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 12px;
    margin: 0 auto;
    max-width: 1200px;
  }
  .ga▮ img {
    width: 100%;
    height: auto;
    object-fit: cover;
    border-radius: 8px;
  }
  @media (max-width: 600px) {
    .gallery {
      grid-template-columns: 1fr;
      gap: 8px;
    }
  }

```

**Palavra que a pessoa ia digitar:** `gallery`: aparece em 2º lugar de 2

**Saída** (as 20 primeiras sugestões):

```text
 1 gap [a]
 2 gallery [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `.ga img`: a propriedade `gap` antes da classe `gallery`.

---

### CSS-39
<a id="css-39"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 9 · **lugar na gramática:** `css:selector`

**Entrada** (a string onde está o cursor):

```text

  .gallery {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 12px;
    margin: 0 auto;
    max-width: 1200px;
  }
  .gallery im▮ {
    width: 100%;
    height: auto;
    object-fit: cover;
    border-radius: 8px;
  }
  @media (max-width: 600px) {
    .gallery {
      grid-template-columns: 1fr;
      gap: 8px;
    }
  }

```

**Palavra que a pessoa ia digitar:** `img`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 img
```

**Veredito:** ✅ Bom. `img` é o único item.

---

### CSS-40
<a id="css-40"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D3, linha 10 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  .gallery {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 12px;
    margin: 0 auto;
    max-width: 1200px;
  }
  .gallery img {
    wdit▮: 100%;
    height: auto;
    object-fit: cover;
    border-radius: 8px;
  }
  @media (max-width: 600px) {
    .gallery {
      grid-template-columns: 1fr;
      gap: 8px;
    }
  }

```

**Palavra que a pessoa ia digitar:** `width`: aparece em 2º lugar de 3

**Saída** (as 20 primeiras sugestões):

```text
 1 white-space [~]
 2 width [~]
 3 writing-mode [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. Erro `wdit`: white-space~ primeiro, `width` em 2º.

---

### CSS-41
<a id="css-41"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 11 · **lugar na gramática:** `css:value:height`

**Entrada** (a string onde está o cursor):

```text

  .gallery {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 12px;
    margin: 0 auto;
    max-width: 1200px;
  }
  .gallery img {
    width: 100%;
    height: a▮;
    object-fit: cover;
    border-radius: 8px;
  }
  @media (max-width: 600px) {
    .gallery {
      grid-template-columns: 1fr;
      gap: 8px;
    }
  }

```

**Palavra que a pessoa ia digitar:** `auto`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 auto                  11 animation-duration
 2 absolute              12 animation-fill-mode
 3 auto-fill [a]         13 animation-iteration-count
 4 align-content         14 animation-name
 5 align-items           15 animation-play-state
 6 align-self            16 animation-timing-function
 7 all                   17 aspect-ratio
 8 animation
 9 animation-delay
10 animation-direction
```

**Veredito:** ✅ Bom. `auto` em primeiro.

---

### CSS-42
<a id="css-42"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 12 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  .gallery {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 12px;
    margin: 0 auto;
    max-width: 1200px;
  }
  .gallery img {
    width: 100%;
    height: auto;
    ob▮: cover;
    border-radius: 8px;
  }
  @media (max-width: 600px) {
    .gallery {
      grid-template-columns: 1fr;
      gap: 8px;
    }
  }

```

**Palavra que a pessoa ia digitar:** `object-fit`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 object-fit
 2 object-position
```

**Veredito:** ✅ Bom. `object-fit`, `object-position`.

---

### CSS-43
<a id="css-43"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 12 · **lugar na gramática:** `css:value:object-fit`

**Entrada** (a string onde está o cursor):

```text

  .gallery {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 12px;
    margin: 0 auto;
    max-width: 1200px;
  }
  .gallery img {
    width: 100%;
    height: auto;
    object-fit: cov▮;
    border-radius: 8px;
  }
  @media (max-width: 600px) {
    .gallery {
      grid-template-columns: 1fr;
      gap: 8px;
    }
  }

```

**Palavra que a pessoa ia digitar:** `cover`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 cover
```

**Veredito:** ✅ Bom. `cover` é o único item.

---

### CSS-44
<a id="css-44"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 13 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  .gallery {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 12px;
    margin: 0 auto;
    max-width: 1200px;
  }
  .gallery img {
    width: 100%;
    height: auto;
    object-fit: cover;
    ▮: 8px;
  }
  @media (max-width: 600px) {
    .gallery {
      grid-template-columns: 1fr;
      gap: 8px;
    }
  }

```

**Palavra que a pessoa ia digitar:** `border-radius`: aparece em 41º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 align-content               11 animation-name
 2 align-items                 12 animation-play-state
 3 align-self                  13 animation-timing-function
 4 all                         14 aspect-ratio
 5 animation                   15 backdrop-filter
 6 animation-delay             16 backface-visibility
 7 animation-direction         17 background
 8 animation-duration          18 background-attachment
 9 animation-fill-mode         19 background-blend-mode
10 animation-iteration-count   20 background-clip
```

**Veredito:** ❌ Ruim. Pedido explícito numa posição de propriedade vazia: `border-radius` é o 41º.

---

### CSS-45
<a id="css-45"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 15 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  .gallery {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 12px;
    margin: 0 auto;
    max-width: 1200px;
  }
  .gallery img {
    width: 100%;
    height: auto;
    object-fit: cover;
    border-radius: 8px;
  }
  @media (m▮: 600px) {
    .gallery {
      grid-template-columns: 1fr;
      gap: 8px;
    }
  }

```

**Palavra que a pessoa ia digitar:** `max-width`: aparece em 8º lugar de 13

**Saída** (as 20 primeiras sugestões):

```text
 1 margin          11 mix-blend-mode
 2 margin-bottom   12 media [a]
 3 margin-left     13 minmax [a]
 4 margin-right
 5 margin-top
 6 mask
 7 max-height
 8 max-width
 9 min-height
10 min-width
```

**Veredito:** ⚠️ Razoável, com ressalva. Característica de media query `(m`: todas as propriedades; `max-width` em 8º.

---

### CSS-46
<a id="css-46"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 17 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  .gallery {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 12px;
    margin: 0 auto;
    max-width: 1200px;
  }
  .gallery img {
    width: 100%;
    height: auto;
    object-fit: cover;
    border-radius: 8px;
  }
  @media (max-width: 600px) {
    .gallery {
      gr▮: 1fr;
      gap: 8px;
    }
  }

```

**Palavra que a pessoa ia digitar:** `grid-template-columns`: aparece em 15º lugar de 16

**Saída** (as 20 primeiras sugestões):

```text
 1 grid                11 grid-row-end
 2 grid-area           12 grid-row-start
 3 grid-auto-columns   13 grid-template
 4 grid-auto-flow      14 grid-template-areas
 5 grid-auto-rows      15 grid-template-columns
 6 grid-column         16 grid-template-rows
 7 grid-column-end
 8 grid-column-start
 9 grid-gap
10 grid-row
```

**Veredito:** ⚠️ Razoável, com ressalva. `gr`: `grid-template-columns` é a 15ª de 16.

---

### CSS-47
<a id="css-47"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 18 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  .gallery {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 12px;
    margin: 0 auto;
    max-width: 1200px;
  }
  .gallery img {
    width: 100%;
    height: auto;
    object-fit: cover;
    border-radius: 8px;
  }
  @media (max-width: 600px) {
    .gallery {
      grid-template-columns: 1fr;
      ga▮: 8px;
    }
  }

```

**Palavra que a pessoa ia digitar:** `gap`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 gap
 2 gallery [a]
```

**Veredito:** ✅ Bom. `gap` em primeiro.

---

### CSS-48
<a id="css-48"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 2 · **lugar na gramática:** `css:selector`

**Entrada** (a string onde está o cursor):

```text

  @keyframes ▮ {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `fade-in`: **não aparece** na lista (100 itens)

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

**Veredito:** ❌ Ruim. `@keyframes `: tags HTML são oferecidas onde se espera um nome novo.

---

### CSS-49
<a id="css-49"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 3 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    f▮ { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `from`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 fill             11 font
 2 filter           12 font-family
 3 flex             13 font-feature-settings
 4 flex-basis       14 font-kerning
 5 flex-direction   15 font-size
 6 flex-flow        16 font-size-adjust
 7 flex-grow        17 font-stretch
 8 flex-shrink      18 font-style
 9 flex-wrap        19 font-variant
10 float            20 font-weight
```

**Veredito:** ❌ Ruim. Seletor de keyframe `f`: propriedades são oferecidas (fill, filter, flex...); faltam `from` e `to`.

---

### CSS-50
<a id="css-50"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D4, linha 3 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opcit▮: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `opacity`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 opacity [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `opacity`.

---

### CSS-51
<a id="css-51"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 3 · **lugar na gramática:** `css:value:transform`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: sca▮(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `scale`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 scale
```

**Veredito:** ✅ Bom. `scale` é o único item.

---

### CSS-52
<a id="css-52"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 4 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    ▮ { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `to`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 align-content               11 animation-name
 2 align-items                 12 animation-play-state
 3 align-self                  13 animation-timing-function
 4 all                         14 aspect-ratio
 5 animation                   15 backdrop-filter
 6 animation-delay             16 backface-visibility
 7 animation-direction         17 background
 8 animation-duration          18 background-attachment
 9 animation-fill-mode         19 background-blend-mode
10 animation-iteration-count   20 background-clip
```

**Veredito:** ❌ Ruim. Pedido explícito em `to {`: a lista de propriedades; `to` não é oferecido.

---

### CSS-53
<a id="css-53"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 4 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { o▮: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `opacity`: aparece em 3º lugar de 13

**Saída** (as 20 primeiras sugestões):

```text
 1 object-fit        11 overflow-wrap
 2 object-position   12 overflow-x
 3 opacity           13 overflow-y
 4 order
 5 outline
 6 outline-color
 7 outline-offset
 8 outline-style
 9 outline-width
10 overflow
```

**Veredito:** ⚠️ Razoável, com ressalva. `o`: `opacity` em 3º (object-fit, object-position primeiro).

---

### CSS-54
<a id="css-54"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D4, linha 4 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; tr▮: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `transform`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 transform
 2 transform-origin
 3 transform-style
 4 transition
 5 transition-delay
 6 transition-duration
 7 transition-property
 8 transition-timing-function
 9 translate
10 transparent
```

**Veredito:** ✅ Bom. `transform` em primeiro.

---

### CSS-55
<a id="css-55"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 4 · **lugar na gramática:** `css:value:transform`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: sca▮(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `scale`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 scale
```

**Veredito:** ✅ Bom. `scale` é o único item.

---

### CSS-56
<a id="css-56"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 7 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    ▮: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `position`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 align-content               11 animation-name
 2 align-items                 12 animation-play-state
 3 align-self                  13 animation-timing-function
 4 all                         14 aspect-ratio
 5 animation                   15 backdrop-filter
 6 animation-delay             16 backface-visibility
 7 animation-direction         17 background
 8 animation-duration          18 background-attachment
 9 animation-fill-mode         19 background-blend-mode
10 animation-iteration-count   20 background-clip
```

**Veredito:** ❌ Ruim. Pedido explícito numa posição de propriedade vazia: `position` fica fora das 100 primeiras.

---

### CSS-57
<a id="css-57"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 7 · **lugar na gramática:** `css:value:position`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: f▮;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `fixed`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 fixed             11 flex-flow
 2 fade-in [a]       12 flex-grow
 3 from [a]          13 flex-shrink
 4 font-size [a]     14 flex-wrap
 5 font-weight [a]   15 float
 6 fill              16 font
 7 filter            17 font-family
 8 flex              18 font-feature-settings
 9 flex-basis        19 font-kerning
10 flex-direction    20 font-size-adjust
```

**Veredito:** ✅ Bom. `fixed` em primeiro.

---

### CSS-58
<a id="css-58"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D4, linha 8 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    to▮: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `top`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 top
 2 touch-action
```

**Veredito:** ✅ Bom. `top`, `touch-action`.

---

### CSS-59
<a id="css-59"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 9 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    lef▮: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `left`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 left
```

**Veredito:** ✅ Bom. `left` é o único item.

---

### CSS-60
<a id="css-60"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D4, linha 10 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    zi-nde▮: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `z-index`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 z-index [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `z-index`.

---

### CSS-61
<a id="css-61"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 11 · **lugar na gramática:** `css:value:overflow`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: h▮;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `hidden`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 hidden
 2 height
 3 hyphens
```

**Veredito:** ✅ Bom. `hidden` em primeiro.

---

### CSS-62
<a id="css-62"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D4, linha 12 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    an▮: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `animation`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 animation
 2 animation-delay
 3 animation-direction
 4 animation-duration
 5 animation-fill-mode
 6 animation-iteration-count
 7 animation-name
 8 animation-play-state
 9 animation-timing-function
```

**Veredito:** ✅ Bom. `animation` em primeiro.

---

### CSS-63
<a id="css-63"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 12 · **lugar na gramática:** `css:value:animation`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fad▮ 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `fade-in`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 fade-in [a]
```

**Veredito:** ✅ Bom. `fade-in` é o único item.

---

### CSS-64
<a id="css-64"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 12 · **lugar na gramática:** `css:value:animation`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ▮;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `ease-out`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 inherit        11 grid
 2 initial        12 absolute
 3 unset          13 relative
 4 revert         14 fixed
 5 none           15 sticky
 6 auto           16 hidden
 7 block          17 visible
 8 inline         18 solid
 9 inline-block   19 dashed
10 flex           20 dotted
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de uma duração: valores genéricos; faltam palavras de easing.

---

### CSS-65
<a id="css-65"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 13 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    b▮: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `box-shadow`: aparece em 42º lugar de 45 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 backdrop-filter         11 background-repeat
 2 backface-visibility     12 background-size
 3 background              13 border
 4 background-attachment   14 border-bottom
 5 background-blend-mode   15 border-bottom-color
 6 background-clip         16 border-bottom-left-radius
 7 background-color        17 border-bottom-right-radius
 8 background-image        18 border-bottom-style
 9 background-origin       19 border-bottom-width
10 background-position     20 border-collapse
```

**Veredito:** ⚠️ Razoável, com ressalva. `b`: `box-shadow` é o 42º de 45.

---

### CSS-66
<a id="css-66"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D4, linha 15 · **lugar na gramática:** `css:selector`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .mo▮ {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `modal__title`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 modal [a]
 2 motion [a]
```

**Veredito:** ✅ Bom. `modal` em primeiro.

---

### CSS-67
<a id="css-67"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 16 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    fon▮: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `font-size`: aparece em 5º lugar de 10

**Saída** (as 20 primeiras sugestões):

```text
 1 font
 2 font-family
 3 font-feature-settings
 4 font-kerning
 5 font-size
 6 font-size-adjust
 7 font-stretch
 8 font-style
 9 font-variant
10 font-weight
```

**Veredito:** ✅ Bom. `font-size` em 5º entre as propriedades font-*.

---

### CSS-68
<a id="css-68"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 17 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    ▮: bold;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `font-weight`: aparece em 92º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 align-content               11 animation-name
 2 align-items                 12 animation-play-state
 3 align-self                  13 animation-timing-function
 4 all                         14 aspect-ratio
 5 animation                   15 backdrop-filter
 6 animation-delay             16 backface-visibility
 7 animation-direction         17 background
 8 animation-duration          18 background-attachment
 9 animation-fill-mode         19 background-blend-mode
10 animation-iteration-count   20 background-clip
```

**Veredito:** ❌ Ruim. Pedido explícito numa posição de propriedade vazia: `font-weight` é o 92º.

---

### CSS-69
<a id="css-69"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 17 · **lugar na gramática:** `css:value:font-weight`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: b▮;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `bold`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 bold                    11 background-color
 2 bolder                  12 background-image
 3 block                   13 background-origin
 4 box-shadow [a]          14 background-position
 5 backdrop-filter         15 background-repeat
 6 backface-visibility     16 background-size
 7 background              17 border
 8 background-attachment   18 border-bottom
 9 background-blend-mode   19 border-bottom-color
10 background-clip         20 border-bottom-left-radius
```

**Veredito:** ✅ Bom. `bold` em primeiro.

---

### CSS-70
<a id="css-70"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D4, linha 18 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    tet-a▮: center;
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `text-align`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 text-align [~]
 2 text-align-last [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `text-align`.

---

### CSS-71
<a id="css-71"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 19 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    tex▮: uppercase;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `text-transform`: aparece em 10º lugar de 10

**Saída** (as 20 primeiras sugestões):

```text
 1 text-align
 2 text-align-last
 3 text-decoration
 4 text-decoration-color
 5 text-decoration-line
 6 text-decoration-style
 7 text-indent
 8 text-overflow
 9 text-shadow
10 text-transform
```

**Veredito:** ⚠️ Razoável, com ressalva. `tex`: `text-transform` é o último (10º, alfabética).

---

### CSS-72
<a id="css-72"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 19 · **lugar na gramática:** `css:value:text-transform`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: ▮;
    letter-spacing: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `uppercase`: aparece em 2º lugar de 100

**Saída** (as 20 primeiras sugestões):

```text
 1 none           11 dotted
 2 uppercase      12 fixed
 3 lowercase      13 hidden
 4 capitalize     14 inherit
 5 absolute       15 initial
 6 auto           16 inline
 7 block          17 inline-block
 8 center         18 relative
 9 currentColor   19 revert
10 dashed         20 solid
```

**Veredito:** ✅ Bom. `none`, `uppercase`.

---

### CSS-73
<a id="css-73"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 20 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  @keyframes fade-in {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }
  .modal {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1000;
    overflow: hidden;
    animation: fade-in 0.25s ease-out;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  }
  .modal__title {
    font-size: 1.25rem;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    l▮: 0.05em;
  }

```

**Palavra que a pessoa ia digitar:** `letter-spacing`: aparece em 2º lugar de 7

**Saída** (as 20 primeiras sugestões):

```text
 1 left
 2 letter-spacing
 3 line-height
 4 list-style
 5 list-style-image
 6 list-style-position
 7 list-style-type
```

**Veredito:** ✅ Bom. `letter-spacing` em 2º.

---

### CSS-74
<a id="css-74"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D5, linha 2 · **lugar na gramática:** `css:selector`

**Entrada** (a string onde está o cursor):

```text

  bo▮ {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `body`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 body                         11 border-collapse
 2 box-sizing [a]               12 border-color
 3 border-box [a]               13 border-image
 4 border                       14 border-left
 5 border-bottom                15 border-left-color
 6 border-bottom-color          16 border-left-style
 7 border-bottom-left-radius    17 border-left-width
 8 border-bottom-right-radius   18 border-radius
 9 border-bottom-style          19 border-right
10 border-bottom-width          20 border-right-color
```

**Veredito:** ✅ Bom. `body` em primeiro.

---

### CSS-75
<a id="css-75"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D5, linha 3 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  body {
    mar▮: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `margin`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 margin
 2 margin-bottom
 3 margin-left
 4 margin-right
 5 margin-top
```

**Veredito:** ✅ Bom. `margin` em primeiro.

---

### CSS-76
<a id="css-76"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D5, linha 5 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    ▮: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `text-decoration`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 align-content               11 animation-name
 2 align-items                 12 animation-play-state
 3 align-self                  13 animation-timing-function
 4 all                         14 aspect-ratio
 5 animation                   15 backdrop-filter
 6 animation-delay             16 backface-visibility
 7 animation-direction         17 background
 8 animation-duration          18 background-attachment
 9 animation-fill-mode         19 background-blend-mode
10 animation-iteration-count   20 background-clip
```

**Veredito:** ❌ Ruim. Pedido explícito numa posição de propriedade vazia: `text-decoration` fica fora das 100 primeiras.

---

### CSS-77
<a id="css-77"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 5 · **lugar na gramática:** `css:value:text-decoration`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: n▮;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `none`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 none
 2 nowrap [a]
 3 nav [a]
```

**Veredito:** ✅ Bom. `none` em primeiro.

---

### CSS-78
<a id="css-78"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D5, linha 6 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    wo▮: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `word-break`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 word-break
 2 word-spacing
 3 word-wrap
```

**Veredito:** ✅ Bom. `word-break` em primeiro.

---

### CSS-79
<a id="css-79"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D5, linha 6 · **lugar na gramática:** `css:value:word-break`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: bre▮;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `break-word`: aparece em 2º lugar de 2

**Saída** (as 20 primeiras sugestões):

```text
 1 break-all
 2 break-word
```

**Veredito:** ✅ Bom. `break-word` em 2º de 2.

---

### CSS-80
<a id="css-80"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D5, linha 7 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    wihte-▮: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `white-space`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 white-space [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `white-space`.

---

### CSS-81
<a id="css-81"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 8 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    t▮: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `text-overflow`: aparece em 10º lugar de 27

**Saída** (as 20 primeiras sugestões):

```text
 1 tab-size                11 text-shadow
 2 table-layout            12 text-transform
 3 text-align              13 top
 4 text-align-last         14 touch-action
 5 text-decoration         15 transform
 6 text-decoration-color   16 transform-origin
 7 text-decoration-line    17 transform-style
 8 text-decoration-style   18 transition
 9 text-indent             19 transition-delay
10 text-overflow           20 transition-duration
```

**Veredito:** ⚠️ Razoável, com ressalva. `t`: `text-overflow` é o 10º.

---

### CSS-82
<a id="css-82"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D5, linha 8 · **lugar na gramática:** `css:value:text-overflow`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: el▮;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `ellipsis`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ⚠️ Razoável, com ressalva. `ellipsis` não está no vocabulário: nada é oferecido.

---

### CSS-83
<a id="css-83"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D5, linha 10 · **lugar na gramática:** `css:selector`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  na▮ ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `nav`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 nav
```

**Veredito:** ✅ Bom. `nav` é o único item.

---

### CSS-84
<a id="css-84"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D5, linha 10 · **lugar na gramática:** `css:selector`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ▮ {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `ul`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

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

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `nav `: tags HTML em ordem alfabética; `ul` fica além das 100 primeiras.

---

### CSS-85
<a id="css-85"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 11 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    l▮: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `list-style`: aparece em 4º lugar de 7

**Saída** (as 20 primeiras sugestões):

```text
 1 left
 2 letter-spacing
 3 line-height
 4 list-style
 5 list-style-image
 6 list-style-position
 7 list-style-type
```

**Veredito:** ✅ Bom. `list-style` em 4º de 7.

---

### CSS-86
<a id="css-86"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D5, linha 12 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    di▮: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `display`: aparece em 2º lugar de 2

**Saída** (as 20 primeiras sugestões):

```text
 1 direction
 2 display
```

**Veredito:** ✅ Bom. `display` em 2º de 2.

---

### CSS-87
<a id="css-87"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D5, linha 12 · **lugar na gramática:** `css:value:display`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: fle▮;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `flex`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 flex
 2 flex-wrap [a]
 3 flex-basis
 4 flex-direction
 5 flex-flow
 6 flex-grow
 7 flex-shrink
```

**Veredito:** ✅ Bom. `flex` em primeiro.

---

### CSS-88
<a id="css-88"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D5, linha 13 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    ▮: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `flex-wrap`: aparece em 81º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 align-content               11 animation-name
 2 align-items                 12 animation-play-state
 3 align-self                  13 animation-timing-function
 4 all                         14 aspect-ratio
 5 animation                   15 backdrop-filter
 6 animation-delay             16 backface-visibility
 7 animation-direction         17 background
 8 animation-duration          18 background-attachment
 9 animation-fill-mode         19 background-blend-mode
10 animation-iteration-count   20 background-clip
```

**Veredito:** ❌ Ruim. Pedido explícito numa posição de propriedade vazia: `flex-wrap` é o 81º.

---

### CSS-89
<a id="css-89"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 13 · **lugar na gramática:** `css:value:flex-wrap`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: w▮;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `wrap`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 wrap
 2 wrap-reverse
 3 white-space [a]
 4 word-break [a]
 5 widows
 6 width
 7 will-change
 8 word-spacing
 9 word-wrap
10 writing-mode
```

**Veredito:** ✅ Bom. `wrap` em primeiro.

---

### CSS-90
<a id="css-90"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D5, linha 14 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    viibi▮: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `visibility`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 visibility [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `visibility`.

---

### CSS-91
<a id="css-91"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D5, linha 16 · **lugar na gramática:** `css:selector`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  inp▮[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `input`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 input
```

**Veredito:** ✅ Bom. `input` é o único item.

---

### CSS-92
<a id="css-92"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D5, linha 16 · **lugar na gramática:** `css:selector`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[▮="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `type`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

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

**Veredito:** ⚠️ Razoável, com ressalva. `input[`: tags HTML em ordem alfabética onde se espera um nome de atributo.

---

### CSS-93
<a id="css-93"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 16 · **lugar na gramática:** `css:selector`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="t▮"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `text`: aparece em 5º lugar de 40

**Saída** (as 20 primeiras sugestões):

```text
 1 table      11 title
 2 tbody      12 tr
 3 td         13 track
 4 template   14 tspan
 5 text       15 type [a]
 6 textarea   16 text-overflow [a]
 7 tfoot      17 text-decoration [a]
 8 th         18 typography [a]
 9 thead      19 tab-size
10 time       20 table-layout
```

**Veredito:** ⚠️ Razoável, com ressalva. Dentro das aspas de um seletor de atributo: tags são oferecidas; `text` é o 5º.

---

### CSS-94
<a id="css-94"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D5, linha 16 · **lugar na gramática:** `css:selector`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:fo▮ {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `focus`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 footer                  11 font-variant
 2 form                    12 font-weight
 3 font
 4 font-family
 5 font-feature-settings
 6 font-kerning
 7 font-size
 8 font-size-adjust
 9 font-stretch
10 font-style
```

**Veredito:** ❌ Ruim. `:fo` (uma pseudo-classe): footer, form e propriedades font; pseudo-classes como `focus` não existem no vocabulário.

---

### CSS-95
<a id="css-95"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D5, linha 17 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    out▮: 2px solid var(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `outline`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 outline
 2 outline-color
 3 outline-offset
 4 outline-style
 5 outline-width
```

**Veredito:** ✅ Bom. `outline` em primeiro.

---

### CSS-96
<a id="css-96"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D5, linha 17 · **lugar na gramática:** `css:value:outline`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid ▮(--accent);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `var`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 inherit        11 grid
 2 initial        12 absolute
 3 unset          13 relative
 4 revert         14 fixed
 5 none           15 sticky
 6 auto           16 hidden
 7 block          17 visible
 8 inline         18 solid
 9 inline-block   19 dashed
10 flex           20 dotted
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `solid `: valores genéricos; falta a função `var`.

---

### CSS-97
<a id="css-97"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--a▮);
    box-sizing: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `accent`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; o nome não aparece em outro lugar do arquivo.

---

### CSS-98
<a id="css-98"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D5, linha 18 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    bo▮: border-box;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `box-sizing`: aparece em 31º lugar de 33 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 border                       11 border-left
 2 border-bottom                12 border-left-color
 3 border-bottom-color          13 border-left-style
 4 border-bottom-left-radius    14 border-left-width
 5 border-bottom-right-radius   15 border-radius
 6 border-bottom-style          16 border-right
 7 border-bottom-width          17 border-right-color
 8 border-collapse              18 border-right-style
 9 border-color                 19 border-right-width
10 border-image                 20 border-spacing
```

**Veredito:** ⚠️ Razoável, com ressalva. `bo`: `box-sizing` é o 31º de 33.

---

### CSS-99
<a id="css-99"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D5, linha 18 · **lugar na gramática:** `css:value:box-sizing`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: bor▮;
    pointer-events: auto;
  }

```

**Palavra que a pessoa ia digitar:** `border-box`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 border-box                   11 border-image
 2 border                       12 border-left
 3 border-bottom                13 border-left-color
 4 border-bottom-color          14 border-left-style
 5 border-bottom-left-radius    15 border-left-width
 6 border-bottom-right-radius   16 border-radius
 7 border-bottom-style          17 border-right
 8 border-bottom-width          18 border-right-color
 9 border-collapse              19 border-right-style
10 border-color                 20 border-right-width
```

**Veredito:** ✅ Bom. `border-box` em primeiro.

---

### CSS-100
<a id="css-100"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D5, linha 19 · **lugar na gramática:** `css:property`

**Entrada** (a string onde está o cursor):

```text

  body {
    margin: 0;
    line-height: 1.5;
    text-decoration: none;
    word-break: break-word;
    white-space: nowrap;
    text-overflow: ellipsis;
  }
  nav ul {
    list-style: none;
    display: flex;
    flex-wrap: wrap;
    visibility: visible;
  }
  input[type="text"]:focus {
    outline: 2px solid var(--accent);
    box-sizing: border-box;
    pionte▮: auto;
  }

```

**Palavra que a pessoa ia digitar:** `pointer-events`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 pointer-events [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `pointer-events`.

---
