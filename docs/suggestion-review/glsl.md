# GLSL: 100 exemplos

Resultado: ✅ 44 bons · ⚠️ 36 razoáveis com ressalva · ❌ 20 ruins.

Como ler: em cada exemplo, `▮` marca onde está o cursor. A lista é o que o plugin mostraria (as 20 primeiras). `[a]` = palavra que já existe no arquivo; `[~]` = sugestão "parecida" (corrige erro de digitação); sem marca = palavra-chave da linguagem. O veredito e o comentário são a minha análise. "Lugar na gramática" é o nome interno da regra de posição que o plugin aplicou (`sql:table`, `css:value:display`...); `(no rule)` quer dizer que o plugin não tem regra para aquele lugar e usa só o que foi digitado.

## Índice (para varrer rápido)

| # | Situação | Digitado | Palavra procurada | Posição | Lugar na gramática | Veredito |
|---|---|---|---|---|---|---|
| [1](#glsl-1) | 1 letra | `e` | `es` | — | `(no rule)` | ⚠️ |
| [2](#glsl-2) | 2 letras | `pr` | `precision` | 2 | `(no rule)` | ✅ |
| [3](#glsl-3) | 3 letras | `hig` | `highp` | 1 | `(no rule)` | ✅ |
| [4](#glsl-4) | Ctrl+Espaço | (nada) | `uniform` | fora | `(no rule)` | ❌ |
| [5](#glsl-5) | 1 letra | `s` | `sampler2D` | 5 | `(no rule)` | ⚠️ |
| [6](#glsl-6) | 2 letras | `un` | `uniform` | 1 | `(no rule)` | ✅ |
| [7](#glsl-7) | 3 letras | `vec` | `vec2` | 1 | `(no rule)` | ✅ |
| [8](#glsl-8) | Ctrl+Espaço | (nada) | `uniform` | fora | `(no rule)` | ❌ |
| [9](#glsl-9) | 1 letra | `f` | `float` | 6 | `(no rule)` | ⚠️ |
| [10](#glsl-10) | 1 letra | `i` | `in` | 10 | `(no rule)` | ⚠️ |
| [11](#glsl-11) | 3 letras | `vec` | `vec2` | 1 | `(no rule)` | ✅ |
| [12](#glsl-12) | Ctrl+Espaço | (nada) | `out` | fora | `(no rule)` | ❌ |
| [13](#glsl-13) | 1 letra | `v` | `vec4` | 4 | `(no rule)` | ⚠️ |
| [14](#glsl-14) | 2 letras | `vo` | `void` | 1 | `(no rule)` | ✅ |
| [15](#glsl-15) | 3 letras | `mai` | `main` | — | `(no rule)` | ⚠️ |
| [16](#glsl-16) | Ctrl+Espaço | (nada) | `uv` | fora | `(no rule)` | ❌ |
| [17](#glsl-17) | 1 letra | `v` | `vUv` | 7 | `(no rule)` | ⚠️ |
| [18](#glsl-18) | 2 letras | `fl` | `float` | 2 | `(no rule)` | ✅ |
| [19](#glsl-19) | 2 letras | `si` | `sin` | 2 | `(no rule)` | ✅ |
| [20](#glsl-20) | erro: trocadas | `uiTm` | `uTime` | 4 | `(no rule)` | ⚠️ |
| [21](#glsl-21) | 1 letra | `t` | `texel` | 17 | `(no rule)` | ⚠️ |
| [22](#glsl-22) | 2 letras | `te` | `texture` | 2 | `(no rule)` | ✅ |
| [23](#glsl-23) | 2 letras | `vU` | `vUv` | 1 | `(no rule)` | ✅ |
| [24](#glsl-24) | Ctrl+Espaço | (nada) | `vec3` | fora | `(no rule)` | ❌ |
| [25](#glsl-25) | 1 letra | `m` | `mix` | 17 | `(no rule)` | ⚠️ |
| [26](#glsl-26) | 2 letras | `te` | `texel` | 12 | `(no rule)` | ⚠️ |
| [27](#glsl-27) | 3 letras | `vec` | `vec3` | 2 | `(no rule)` | ✅ |
| [28](#glsl-28) | Ctrl+Espaço | (nada) | `pulse` | fora | `(no rule)` | ❌ |
| [29](#glsl-29) | 1 letra | `l` | `length` | 2 | `(no rule)` | ✅ |
| [30](#glsl-30) | 1 letra | `u` | `uv` | fora | `(no rule)` | ❌ |
| [31](#glsl-31) | 3 letras | `vec` | `vec4` | 3 | `(no rule)` | ✅ |
| [32](#glsl-32) | Ctrl+Espaço | (nada) | `clamp` | 26 | `(no rule)` | ⚠️ |
| [33](#glsl-33) | 1 letra | `t` | `texel` | 17 | `(no rule)` | ⚠️ |
| [34](#glsl-34) | 1 letra | `e` | `es` | — | `(no rule)` | ⚠️ |
| [35](#glsl-35) | 1 letra | `i` | `in` | 10 | `(no rule)` | ⚠️ |
| [36](#glsl-36) | Ctrl+Espaço | (nada) | `aPosition` | fora | `(no rule)` | ❌ |
| [37](#glsl-37) | 1 letra | `i` | `in` | 10 | `(no rule)` | ⚠️ |
| [38](#glsl-38) | 2 letras | `aN` | `aNormal` | 2 | `(no rule)` | ✅ |
| [39](#glsl-39) | 1 letra | `i` | `in` | 10 | `(no rule)` | ⚠️ |
| [40](#glsl-40) | erro: trocadas | `aeTxCo` | `aTexCoord` | 1 | `(no rule)` | ✅ |
| [41](#glsl-41) | 1 letra | `u` | `uniform` | 6 | `(no rule)` | ⚠️ |
| [42](#glsl-42) | 2 letras | `uM` | `uModel` | 1 | `(no rule)` | ✅ |
| [43](#glsl-43) | 3 letras | `uni` | `uniform` | 1 | `(no rule)` | ✅ |
| [44](#glsl-44) | Ctrl+Espaço | (nada) | `uView` | fora | `(no rule)` | ❌ |
| [45](#glsl-45) | 1 letra | `u` | `uniform` | 6 | `(no rule)` | ⚠️ |
| [46](#glsl-46) | 2 letras | `uP` | `uProjection` | 1 | `(no rule)` | ✅ |
| [47](#glsl-47) | 3 letras | `uni` | `uniform` | 1 | `(no rule)` | ✅ |
| [48](#glsl-48) | Ctrl+Espaço | (nada) | `uNormalMatrix` | fora | `(no rule)` | ❌ |
| [49](#glsl-49) | 1 letra | `o` | `out` | 1 | `(no rule)` | ✅ |
| [50](#glsl-50) | erro: faltando | `vNrma` | `vNormal` | 1 | `(no rule)` | ✅ |
| [51](#glsl-51) | 2 letras | `ou` | `out` | 1 | `(no rule)` | ✅ |
| [52](#glsl-52) | Ctrl+Espaço | (nada) | `vec2` | fora | `(no rule)` | ❌ |
| [53](#glsl-53) | 1 letra | `v` | `void` | 5 | `(no rule)` | ⚠️ |
| [54](#glsl-54) | 2 letras | `ma` | `main` | — | `(no rule)` | ⚠️ |
| [55](#glsl-55) | 3 letras | `nor` | `normalize` | 1 | `(no rule)` | ✅ |
| [56](#glsl-56) | Ctrl+Espaço | (nada) | `uNormalMatrix` | fora | `(no rule)` | ❌ |
| [57](#glsl-57) | 1 letra | `v` | `vUv` | 8 | `(no rule)` | ⚠️ |
| [58](#glsl-58) | 2 letras | `aT` | `aTexCoord` | 8 | `(no rule)` | ⚠️ |
| [59](#glsl-59) | 3 letras | `uPr` | `uProjection` | 1 | `(no rule)` | ✅ |
| [60](#glsl-60) | erro: trocadas | `uiVe` | `uView` | 4 | `(no rule)` | ⚠️ |
| [61](#glsl-61) | 1 letra | `v` | `vec4` | 4 | `(no rule)` | ⚠️ |
| [62](#glsl-62) | 2 letras | `aP` | `aPosition` | 1 | `(no rule)` | ✅ |
| [63](#glsl-63) | 3 letras | `med` | `mediump` | 1 | `(no rule)` | ✅ |
| [64](#glsl-64) | Ctrl+Espaço | (nada) | `float` | 60 | `(no rule)` | ❌ |
| [65](#glsl-65) | 1 letra | `v` | `vec3` | 1 | `(no rule)` | ✅ |
| [66](#glsl-66) | 2 letras | `uL` | `uLightDirection` | 1 | `(no rule)` | ✅ |
| [67](#glsl-67) | 3 letras | `vec` | `vec3` | 1 | `(no rule)` | ✅ |
| [68](#glsl-68) | Ctrl+Espaço | (nada) | `uBaseColor` | fora | `(no rule)` | ❌ |
| [69](#glsl-69) | 1 letra | `u` | `uniform` | 6 | `(no rule)` | ⚠️ |
| [70](#glsl-70) | erro: faltando | `uSini` | `uShininess` | 1 | `(no rule)` | ✅ |
| [71](#glsl-71) | 3 letras | `var` | `varying` | 1 | `(no rule)` | ✅ |
| [72](#glsl-72) | Ctrl+Espaço | (nada) | `vNormal` | fora | `(no rule)` | ❌ |
| [73](#glsl-73) | 1 letra | `v` | `varying` | 1 | `(no rule)` | ✅ |
| [74](#glsl-74) | 2 letras | `vV` | `vViewDirection` | 1 | `(no rule)` | ✅ |
| [75](#glsl-75) | 3 letras | `flo` | `float` | 1 | `(no rule)` | ✅ |
| [76](#glsl-76) | Ctrl+Espaço | (nada) | `vec3` | fora | `(no rule)` | ❌ |
| [77](#glsl-77) | 1 letra | `n` | `normal` | 5 | `(no rule)` | ⚠️ |
| [78](#glsl-78) | 2 letras | `li` | `light` | 1 | `(no rule)` | ✅ |
| [79](#glsl-79) | 3 letras | `ret` | `return` | 1 | `(no rule)` | ✅ |
| [80](#glsl-80) | Ctrl+Espaço | (nada) | `dot` | 1 | `(no rule)` | ✅ |
| [81](#glsl-81) | 1 letra | `n` | `normalize` | 2 | `(no rule)` | ⚠️ |
| [82](#glsl-82) | 2 letras | `no` | `normalize` | 2 | `(no rule)` | ⚠️ |
| [83](#glsl-83) | 3 letras | `lig` | `light` | 1 | `(no rule)` | ✅ |
| [84](#glsl-84) | Ctrl+Espaço | (nada) | `main` | — | `(no rule)` | ⚠️ |
| [85](#glsl-85) | 1 letra | `f` | `float` | 6 | `(no rule)` | ⚠️ |
| [86](#glsl-86) | 2 letras | `di` | `diffuse` | 3 | `(no rule)` | ⚠️ |
| [87](#glsl-87) | 3 letras | `uLi` | `uLightDirection` | 1 | `(no rule)` | ✅ |
| [88](#glsl-88) | Ctrl+Espaço | (nada) | `vec3` | fora | `(no rule)` | ❌ |
| [89](#glsl-89) | 1 letra | `n` | `normalize` | 2 | `(no rule)` | ⚠️ |
| [90](#glsl-90) | erro: faltando | `uLght` | `uLightDirection` | 1 | `(no rule)` | ✅ |
| [91](#glsl-91) | 3 letras | `flo` | `float` | 1 | `(no rule)` | ✅ |
| [92](#glsl-92) | Ctrl+Espaço | (nada) | `specular` | fora | `(no rule)` | ❌ |
| [93](#glsl-93) | 1 letra | `m` | `max` | 14 | `(no rule)` | ⚠️ |
| [94](#glsl-94) | 2 letras | `do` | `dot` | 1 | `(no rule)` | ✅ |
| [95](#glsl-95) | 3 letras | `hal` | `halfVector` | 1 | `(no rule)` | ✅ |
| [96](#glsl-96) | Ctrl+Espaço | (nada) | `uShininess` | fora | `(no rule)` | ❌ |
| [97](#glsl-97) | 1 letra | `d` | `discard` | 6 | `(no rule)` | ⚠️ |
| [98](#glsl-98) | 2 letras | `gl` | `gl_FragColor` | — | `(no rule)` | ⚠️ |
| [99](#glsl-99) | 3 letras | `uBa` | `uBaseColor` | 1 | `(no rule)` | ✅ |
| [100](#glsl-100) | Ctrl+Espaço | (nada) | `vec3` | fora | `(no rule)` | ❌ |

Posição: lugar da palavra procurada na lista; `—` = a palavra não existe em outro lugar do arquivo; `fora` = existe mas não está na lista.

## Os arquivos usados como entrada

Escritos à mão como um desenvolvedor escreveria (código JavaScript com strings da linguagem). Nada foi gerado pelo gerador dos experimentos.

### Documento D1

```js
const fragment = glsl`
#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}
`;
```

### Documento D2

```js
const vertex = glsl`
#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}
`;
```

### Documento D3

```js
const lighting = glsl`
precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}
`;
```

## Os exemplos

### GLSL-1
<a id="glsl-1"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 e▮
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `es`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 else
 2 EmitVertex
 3 EndPrimitive
 4 equal
 5 exp
 6 exp2
```

**Veredito:** ⚠️ Razoável, com ressalva. `#version 300 e`: `es` não está no vocabulário; else/EmitVertex/exp são oferecidas.

---

### GLSL-2
<a id="glsl-2"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
pr▮ highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `precision`: aparece em 2º lugar de 2

**Saída** (as 20 primeiras sugestões):

```text
 1 precise
 2 precision
```

**Veredito:** ✅ Bom. `precision` é o 2º, depois de `precise`.

---

### GLSL-3
<a id="glsl-3"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision hig▮ float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `highp`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 highp
```

**Veredito:** ✅ Bom. `highp` é o único item.

---

### GLSL-4
<a id="glsl-4"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

▮ sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `uniform`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 sampler2D   11 atomicAdd
 2 abs         12 atomicMax
 3 acos        13 atomicMin
 4 acosh       14 atomic_uint
 5 all         15 attribute
 6 any         16 barrier
 7 asin        17 bitCount
 8 asinh       18 bool
 9 atan        19 break
10 atanh       20 buffer
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto (uniform, out, vec3...) ficam fora de alcance. `uniform` fica fora das 100 primeiras.

---

### GLSL-5
<a id="glsl-5"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform s▮ uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `sampler2D`: aparece em 5º lugar de 26

**Saída** (as 20 primeiras sugestões):

```text
 1 sample                 11 sampler3D
 2 sampler1D              12 samplerBuffer
 3 sampler1DArray         13 samplerCube
 4 sampler1DShadow        14 samplerCubeArray
 5 sampler2D              15 samplerCubeShadow
 6 sampler2DArray         16 shared
 7 sampler2DArrayShadow   17 sign
 8 sampler2DMS            18 sin
 9 sampler2DRect          19 sinh
10 sampler2DShadow        20 smooth
```

**Veredito:** ⚠️ Razoável, com ressalva. `s`: `sampler2D` é o 5º, em ordem alfabética entre os tipos sampler.

---

### GLSL-6
<a id="glsl-6"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
un▮ vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `uniform`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 uniform
 2 unpackUnorm4x8
```

**Veredito:** ✅ Bom. `uniform` em primeiro.

---

### GLSL-7
<a id="glsl-7"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec▮ uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `vec2`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 vec2
 2 vec3
 3 vec4
```

**Veredito:** ✅ Bom. `vec2` em primeiro.

---

### GLSL-8
<a id="glsl-8"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
▮ float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `uniform`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 float     11 atomicAdd
 2 abs       12 atomicMax
 3 acos      13 atomicMin
 4 acosh     14 atomic_uint
 5 all       15 attribute
 6 any       16 barrier
 7 asin      17 bitCount
 8 asinh     18 bool
 9 atan      19 break
10 atanh     20 buffer
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto (uniform, out, vec3...) ficam fora de alcance. `uniform` fica fora das 100 primeiras.

---

### GLSL-9
<a id="glsl-9"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform f▮ uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `float`: aparece em 6º lugar de 14

**Saída** (as 20 primeiras sugestões):

```text
 1 faceforward       11 fract
 2 false             12 fwidth
 3 findLSB           13 fragColor [a]
 4 findMSB           14 fragment [a]
 5 flat
 6 float
 7 floatBitsToInt
 8 floatBitsToUint
 9 floor
10 for
```

**Veredito:** ⚠️ Razoável, com ressalva. `f`: `float` é o 6º (alfabético).

---

### GLSL-10
<a id="glsl-10"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

i▮ vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `in`: aparece em 10º lugar de 31

**Saída** (as 20 primeiras sugestões):

```text
 1 if           11 inout
 2 iimage2D     12 int
 3 iimage3D     13 intBitsToFloat
 4 iimageCube   14 invariant
 5 image2D      15 inverse
 6 image3D      16 inversesqrt
 7 imageCube    17 isampler1D
 8 imageLoad    18 isampler1DArray
 9 imageStore   19 isampler2D
10 in           20 isampler2DArray
```

**Veredito:** ⚠️ Razoável, com ressalva. `i`: `in` é o 10º de 31 (alfabético).

---

### GLSL-11
<a id="glsl-11"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec▮ vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `vec2`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 vec2
 2 vec3
 3 vec4
```

**Veredito:** ✅ Bom. `vec2` em primeiro.

---

### GLSL-12
<a id="glsl-12"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
▮ vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `out`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 vec4      11 atomicAdd
 2 abs       12 atomicMax
 3 acos      13 atomicMin
 4 acosh     14 atomic_uint
 5 all       15 attribute
 6 any       16 barrier
 7 asin      17 bitCount
 8 asinh     18 bool
 9 atan      19 break
10 atanh     20 buffer
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto (uniform, out, vec3...) ficam fora de alcance. `out` fica fora das 100 primeiras.

---

### GLSL-13
<a id="glsl-13"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out v▮ fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `vec4`: aparece em 4º lugar de 8

**Saída** (as 20 primeiras sugestões):

```text
 1 varying
 2 vec2
 3 vec3
 4 vec4
 5 void
 6 volatile
 7 vUv [a]
 8 version [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `v`: `vec4` é o 4º (alfabético).

---

### GLSL-14
<a id="glsl-14"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

vo▮ main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `void`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 void
 2 volatile
```

**Veredito:** ✅ Bom. `void` em primeiro.

---

### GLSL-15
<a id="glsl-15"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void mai▮() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `main`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 mat2 [~]
 2 mat2x2 [~]
 3 mat2x3 [~]
 4 mat2x4 [~]
 5 mat3 [~]
 6 mat3x2 [~]
 7 mat3x3 [~]
 8 mat3x4 [~]
 9 mat4 [~]
10 mat4x2 [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. `mai`: `main` não está no vocabulário; palavras parecidas (mat2, mat2x2...) são ruído.

---

### GLSL-16
<a id="glsl-16"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 13 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 ▮ = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `uv`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 vUv [a]           11 atanh
 2 uResolution [a]   12 atomicAdd
 3 abs               13 atomicMax
 4 acos              14 atomicMin
 5 acosh             15 atomic_uint
 6 all               16 attribute
 7 any               17 barrier
 8 asin              18 bitCount
 9 asinh             19 bool
10 atan              20 break
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto (uniform, out, vec3...) ficam fora de alcance. A variável `uv` tem só 2 letras.

---

### GLSL-17
<a id="glsl-17"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 13 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = v▮ * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `vUv`: aparece em 7º lugar de 8

**Saída** (as 20 primeiras sugestões):

```text
 1 varying
 2 vec2
 3 vec3
 4 vec4
 5 void
 6 volatile
 7 vUv [a]
 8 version [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `v`: `vUv` é o 7º de 8, depois de todas as palavras-chave.

---

### GLSL-18
<a id="glsl-18"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 14 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  fl▮ pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `float`: aparece em 2º lugar de 5

**Saída** (as 20 primeiras sugestões):

```text
 1 flat
 2 float
 3 floatBitsToInt
 4 floatBitsToUint
 5 floor
```

**Veredito:** ✅ Bom. `float` é o 2º, depois de `flat`.

---

### GLSL-19
<a id="glsl-19"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 14 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * si▮(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `sin`: aparece em 2º lugar de 3

**Saída** (as 20 primeiras sugestões):

```text
 1 sign
 2 sin
 3 sinh
```

**Veredito:** ✅ Bom. `sin` é o 2º.

---

### GLSL-20
<a id="glsl-20"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D1, linha 14 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uiTm▮ * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `uTime`: aparece em 4º lugar de 4

**Saída** (as 20 primeiras sugestões):

```text
 1 uimage2D [~]
 2 uimage3D [~]
 3 uimageCube [~]
 4 uTime [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. Erro `uiTm`: três tipos de imagem vêm antes de `uTime~`.

---

### GLSL-21
<a id="glsl-21"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 15 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 t▮ = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `texel`: aparece em 17º lugar de 17

**Saída** (as 20 primeiras sugestões):

```text
 1 tan             11 textureOffset
 2 tanh            12 textureProj
 3 texelFetch      13 textureSize
 4 texture         14 transpose
 5 texture2D       15 true
 6 texture3D       16 trunc
 7 textureCube     17 texel [a]
 8 textureGather
 9 textureGrad
10 textureLod
```

**Veredito:** ⚠️ Razoável, com ressalva. `t`: o local `texel` é o último (17º), depois de todas as funções embutidas.

---

### GLSL-22
<a id="glsl-22"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 15 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = te▮(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `texture`: aparece em 2º lugar de 12

**Saída** (as 20 primeiras sugestões):

```text
 1 texelFetch      11 textureSize
 2 texture         12 texel [a]
 3 texture2D
 4 texture3D
 5 textureCube
 6 textureGather
 7 textureGrad
 8 textureLod
 9 textureOffset
10 textureProj
```

**Veredito:** ✅ Bom. `texture` é o 2º.

---

### GLSL-23
<a id="glsl-23"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 15 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vU▮);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `vUv`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 vUv [a]
```

**Veredito:** ✅ Bom. `vUv` é o único item.

---

### GLSL-24
<a id="glsl-24"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  ▮ color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `vec3`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 color [a]   11 atomicAdd
 2 abs         12 atomicMax
 3 acos        13 atomicMin
 4 acosh       14 atomic_uint
 5 all         15 attribute
 6 any         16 barrier
 7 asin        17 bitCount
 8 asinh       18 bool
 9 atan        19 break
10 atanh       20 buffer
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto (uniform, out, vec3...) ficam fora de alcance. `vec3` fica fora das 100 primeiras.

---

### GLSL-25
<a id="glsl-25"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = m▮(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `mix`: aparece em 17º lugar de 20

**Saída** (as 20 primeiras sugestões):

```text
 1 mat2      11 mat4x3
 2 mat2x2    12 mat4x4
 3 mat2x3    13 matrixCompMult
 4 mat2x4    14 max
 5 mat3      15 mediump
 6 mat3x2    16 min
 7 mat3x3    17 mix
 8 mat3x4    18 mod
 9 mat4      19 modf
10 mat4x2    20 main [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `m`: `mix` é o 17º de 20 (tipos de matriz vêm antes).

---

### GLSL-26
<a id="glsl-26"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(te▮.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `texel`: aparece em 12º lugar de 12

**Saída** (as 20 primeiras sugestões):

```text
 1 texelFetch      11 textureSize
 2 texture         12 texel [a]
 3 texture2D
 4 texture3D
 5 textureCube
 6 textureGather
 7 textureGrad
 8 textureLod
 9 textureOffset
10 textureProj
```

**Veredito:** ⚠️ Razoável, com ressalva. `te`: o local `texel` é o último (12º), depois de todas as funções embutidas.

---

### GLSL-27
<a id="glsl-27"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec▮(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `vec3`: aparece em 2º lugar de 3

**Saída** (as 20 primeiras sugestões):

```text
 1 vec2
 2 vec3
 3 vec4
```

**Veredito:** ✅ Bom. `vec3` é o 2º.

---

### GLSL-28
<a id="glsl-28"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), ▮ * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `pulse`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 abs         11 atomicMax
 2 acos        12 atomicMin
 3 acosh       13 atomic_uint
 4 all         14 attribute
 5 any         15 barrier
 6 asin        16 bitCount
 7 asinh       17 bool
 8 atan        18 break
 9 atanh       19 buffer
10 atomicAdd   20 bvec2
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto (uniform, out, vec3...) ficam fora de alcance. O local `pulse`, bem ao lado, não é alcançado.

---

### GLSL-29
<a id="glsl-29"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, l▮(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `length`: aparece em 2º lugar de 7

**Saída** (as 20 primeiras sugestões):

```text
 1 layout
 2 length
 3 lessThan
 4 lessThanEqual
 5 log
 6 log2
 7 lowp
```

**Veredito:** ✅ Bom. `length` é o 2º, depois de `layout`.

---

### GLSL-30
<a id="glsl-30"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(u▮)));
  fragColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `uv`: **não aparece** na lista (23 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 uimage2D          11 usampler2DArray
 2 uimage3D          12 usampler2DMS
 3 uimageCube        13 usampler2DRect
 4 uint              14 usampler3D
 5 uintBitsToFloat   15 usamplerBuffer
 6 uniform           16 usamplerCube
 7 unpackUnorm4x8    17 usamplerCubeArray
 8 usampler1D        18 uvec2
 9 usampler1DArray   19 uvec3
10 usampler2D        20 uvec4
```

**Veredito:** ❌ Ruim. `u`: `uv` nunca pode ser sugerido (palavras com menos de 3 letras são ignoradas).

---

### GLSL-31
<a id="glsl-31"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec▮(clamp(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `vec4`: aparece em 3º lugar de 3

**Saída** (as 20 primeiras sugestões):

```text
 1 vec2
 2 vec3
 3 vec4
```

**Veredito:** ✅ Bom. `vec4` é o 3º de 3.

---

### GLSL-32
<a id="glsl-32"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(▮(color, 0.0, 1.0), texel.a);
}

```

**Palavra que a pessoa ia digitar:** `clamp`: aparece em 26º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 abs         11 atomicMax
 2 acos        12 atomicMin
 3 acosh       13 atomic_uint
 4 all         14 attribute
 5 any         15 barrier
 6 asin        16 bitCount
 7 asinh       17 bool
 8 atan        18 break
 9 atanh       19 buffer
10 atomicAdd   20 bvec2
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito antes de uma lista de argumentos: `clamp` é o 26º da lista alfabética.

---

### GLSL-33
<a id="glsl-33"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
precision highp float;

uniform sampler2D uTexture;
uniform vec2 uResolution;
uniform float uTime;

in vec2 vUv;
out vec4 fragColor;

void main() {
  vec2 uv = vUv * 2.0 - 1.0;
  float pulse = 0.5 + 0.5 * sin(uTime * 3.0);
  vec4 texel = texture(uTexture, vUv);
  vec3 color = mix(texel.rgb, vec3(1.0, 0.4, 0.2), pulse * smoothstep(0.2, 0.8, length(uv)));
  fragColor = vec4(clamp(color, 0.0, 1.0), t▮.a);
}

```

**Palavra que a pessoa ia digitar:** `texel`: aparece em 17º lugar de 17

**Saída** (as 20 primeiras sugestões):

```text
 1 tan             11 textureOffset
 2 tanh            12 textureProj
 3 texelFetch      13 textureSize
 4 texture         14 transpose
 5 texture2D       15 true
 6 texture3D       16 trunc
 7 textureCube     17 texel [a]
 8 textureGather
 9 textureGrad
10 textureLod
```

**Veredito:** ⚠️ Razoável, com ressalva. `t`: o local `texel` é o último (17º).

---

### GLSL-34
<a id="glsl-34"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 e▮
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `es`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 else
 2 EmitVertex
 3 EndPrimitive
 4 equal
 5 exp
 6 exp2
```

**Veredito:** ⚠️ Razoável, com ressalva. `#version 300 e`: `es` não está no vocabulário; else/EmitVertex/exp são oferecidas.

---

### GLSL-35
<a id="glsl-35"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
i▮ vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `in`: aparece em 10º lugar de 31

**Saída** (as 20 primeiras sugestões):

```text
 1 if           11 inout
 2 iimage2D     12 int
 3 iimage3D     13 intBitsToFloat
 4 iimageCube   14 invariant
 5 image2D      15 inverse
 6 image3D      16 inversesqrt
 7 imageCube    17 isampler1D
 8 imageLoad    18 isampler1DArray
 9 imageStore   19 isampler2D
10 in           20 isampler2DArray
```

**Veredito:** ⚠️ Razoável, com ressalva. `i`: `in` é o 10º de 31 (alfabético).

---

### GLSL-36
<a id="glsl-36"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 ▮;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `aPosition`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 aNormal [a]   11 atanh
 2 vNormal [a]   12 atomicAdd
 3 abs           13 atomicMax
 4 acos          14 atomicMin
 5 acosh         15 atomic_uint
 6 all           16 attribute
 7 any           17 barrier
 8 asin          18 bitCount
 9 asinh         19 bool
10 atan          20 break
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto (uniform, out, vec3...) ficam fora de alcance. `aPosition` não é alcançado.

---

### GLSL-37
<a id="glsl-37"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
i▮ vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `in`: aparece em 10º lugar de 31

**Saída** (as 20 primeiras sugestões):

```text
 1 if           11 inout
 2 iimage2D     12 int
 3 iimage3D     13 intBitsToFloat
 4 iimageCube   14 invariant
 5 image2D      15 inverse
 6 image3D      16 inversesqrt
 7 imageCube    17 isampler1D
 8 imageLoad    18 isampler1DArray
 9 imageStore   19 isampler2D
10 in           20 isampler2DArray
```

**Veredito:** ⚠️ Razoável, com ressalva. `i`: `in` é o 10º de 31 (alfabético).

---

### GLSL-38
<a id="glsl-38"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aN▮;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `aNormal`: aparece em 2º lugar de 2

**Saída** (as 20 primeiras sugestões):

```text
 1 any
 2 aNormal [a]
```

**Veredito:** ✅ Bom. `aNormal` é o 2º, depois de `any`.

---

### GLSL-39
<a id="glsl-39"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
i▮ vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `in`: aparece em 10º lugar de 31

**Saída** (as 20 primeiras sugestões):

```text
 1 if           11 inout
 2 iimage2D     12 int
 3 iimage3D     13 intBitsToFloat
 4 iimageCube   14 invariant
 5 image2D      15 inverse
 6 image3D      16 inversesqrt
 7 imageCube    17 isampler1D
 8 imageLoad    18 isampler1DArray
 9 imageStore   19 isampler2D
10 in           20 isampler2DArray
```

**Veredito:** ⚠️ Razoável, com ressalva. `i`: `in` é o 10º de 31 (alfabético).

---

### GLSL-40
<a id="glsl-40"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D2, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aeTxCo▮;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `aTexCoord`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 aTexCoord [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `aTexCoord~`.

---

### GLSL-41
<a id="glsl-41"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

u▮ mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `uniform`: aparece em 6º lugar de 24

**Saída** (as 20 primeiras sugestões):

```text
 1 uimage2D          11 usampler2DArray
 2 uimage3D          12 usampler2DMS
 3 uimageCube        13 usampler2DRect
 4 uint              14 usampler3D
 5 uintBitsToFloat   15 usamplerBuffer
 6 uniform           16 usamplerCube
 7 unpackUnorm4x8    17 usamplerCubeArray
 8 usampler1D        18 uvec2
 9 usampler1DArray   19 uvec3
10 usampler2D        20 uvec4
```

**Veredito:** ⚠️ Razoável, com ressalva. `u`: `uniform` é o 6º (alfabético).

---

### GLSL-42
<a id="glsl-42"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uM▮;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `uModel`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 uModel [a]
```

**Veredito:** ✅ Bom. `uModel` é o único item.

---

### GLSL-43
<a id="glsl-43"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uni▮ mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `uniform`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 uniform
```

**Veredito:** ✅ Bom. `uniform` é o único item.

---

### GLSL-44
<a id="glsl-44"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 ▮;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `uView`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 uModel [a]        11 atanh
 2 uProjection [a]   12 atomicAdd
 3 abs               13 atomicMax
 4 acos              14 atomicMin
 5 acosh             15 atomic_uint
 6 all               16 attribute
 7 any               17 barrier
 8 asin              18 bitCount
 9 asinh             19 bool
10 atan              20 break
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto (uniform, out, vec3...) ficam fora de alcance. Os nomes vizinhos `uModel` e `uProjection` vêm primeiro, mas `uView` fica fora das 100 primeiras.

---

### GLSL-45
<a id="glsl-45"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
u▮ mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `uniform`: aparece em 6º lugar de 24

**Saída** (as 20 primeiras sugestões):

```text
 1 uimage2D          11 usampler2DArray
 2 uimage3D          12 usampler2DMS
 3 uimageCube        13 usampler2DRect
 4 uint              14 usampler3D
 5 uintBitsToFloat   15 usamplerBuffer
 6 uniform           16 usamplerCube
 7 unpackUnorm4x8    17 usamplerCubeArray
 8 usampler1D        18 uvec2
 9 usampler1DArray   19 uvec3
10 usampler2D        20 uvec4
```

**Veredito:** ⚠️ Razoável, com ressalva. `u`: `uniform` é o 6º (alfabético).

---

### GLSL-46
<a id="glsl-46"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uP▮;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `uProjection`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 uProjection [a]
```

**Veredito:** ✅ Bom. `uProjection` é o único item.

---

### GLSL-47
<a id="glsl-47"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uni▮ mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `uniform`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 uniform
```

**Veredito:** ✅ Bom. `uniform` é o único item.

---

### GLSL-48
<a id="glsl-48"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 ▮;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `uNormalMatrix`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 abs         11 atomicMax
 2 acos        12 atomicMin
 3 acosh       13 atomic_uint
 4 all         14 attribute
 5 any         15 barrier
 6 asin        16 bitCount
 7 asinh       17 bool
 8 atan        18 break
 9 atanh       19 buffer
10 atomicAdd   20 bvec2
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto (uniform, out, vec3...) ficam fora de alcance.

---

### GLSL-49
<a id="glsl-49"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

o▮ vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `out`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 out
 2 outerProduct
```

**Veredito:** ✅ Bom. `out` em primeiro.

---

### GLSL-50
<a id="glsl-50"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D2, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNrma▮;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `vNormal`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 vNormal [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `vNormal~`.

---

### GLSL-51
<a id="glsl-51"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 13 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
ou▮ vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `out`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 out
 2 outerProduct
```

**Veredito:** ✅ Bom. `out` em primeiro.

---

### GLSL-52
<a id="glsl-52"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 13 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out ▮ vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `vec2`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 vUv [a]   11 atanh
 2 vec3      12 atomicAdd
 3 abs       13 atomicMax
 4 acos      14 atomicMin
 5 acosh     15 atomic_uint
 6 all       16 attribute
 7 any       17 barrier
 8 asin      18 bitCount
 9 asinh     19 bool
10 atan      20 break
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto (uniform, out, vec3...) ficam fora de alcance. `vec2` fica fora das 100 primeiras.

---

### GLSL-53
<a id="glsl-53"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 15 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

v▮ main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `void`: aparece em 5º lugar de 10

**Saída** (as 20 primeiras sugestões):

```text
 1 varying
 2 vec2
 3 vec3
 4 vec4
 5 void
 6 volatile
 7 vUv [a]
 8 vNormal [a]
 9 version [a]
10 vertex [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `v`: `void` é o 5º (alfabético).

---

### GLSL-54
<a id="glsl-54"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 15 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void ma▮() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `main`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 mat2      11 mat4x3
 2 mat2x2    12 mat4x4
 3 mat2x3    13 matrixCompMult
 4 mat2x4    14 max
 5 mat3
 6 mat3x2
 7 mat3x3
 8 mat3x4
 9 mat4
10 mat4x2
```

**Veredito:** ⚠️ Razoável, com ressalva. `ma`: `main` não está no vocabulário; tipos de matriz são oferecidos.

---

### GLSL-55
<a id="glsl-55"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = nor▮(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `normalize`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 normalize
```

**Veredito:** ✅ Bom. `normalize` é o único item.

---

### GLSL-56
<a id="glsl-56"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(▮ * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `uNormalMatrix`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 abs         11 atomicMax
 2 acos        12 atomicMin
 3 acosh       13 atomic_uint
 4 all         14 attribute
 5 any         15 barrier
 6 asin        16 bitCount
 7 asinh       17 bool
 8 atan        18 break
 9 atanh       19 buffer
10 atomicAdd   20 bvec2
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto (uniform, out, vec3...) ficam fora de alcance. `uNormalMatrix` não é alcançado.

---

### GLSL-57
<a id="glsl-57"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  v▮ = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `vUv`: aparece em 8º lugar de 10

**Saída** (as 20 primeiras sugestões):

```text
 1 varying
 2 vec2
 3 vec3
 4 vec4
 5 void
 6 volatile
 7 vNormal [a]
 8 vUv [a]
 9 version [a]
10 vertex [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `v`: `vUv` é o 8º de 10, depois de todas as palavras-chave.

---

### GLSL-58
<a id="glsl-58"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aT▮;
  gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `aTexCoord`: aparece em 8º lugar de 8

**Saída** (as 20 primeiras sugestões):

```text
 1 atan
 2 atanh
 3 atomicAdd
 4 atomicMax
 5 atomicMin
 6 atomic_uint
 7 attribute
 8 aTexCoord [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `aT`: `aTexCoord` é o último (8º), depois de `atan` e dos atomic.

---

### GLSL-59
<a id="glsl-59"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 18 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uPr▮ * uView * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `uProjection`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 uProjection [a]
```

**Veredito:** ✅ Bom. `uProjection` é o único item.

---

### GLSL-60
<a id="glsl-60"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D2, linha 18 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uiVe▮ * uModel * vec4(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `uView`: aparece em 4º lugar de 4

**Saída** (as 20 primeiras sugestões):

```text
 1 uvec2 [~]
 2 uvec3 [~]
 3 uvec4 [~]
 4 uView [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. Erro `uiVe`: três tipos de vetor vêm antes de `uView~`.

---

### GLSL-61
<a id="glsl-61"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 18 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * v▮(aPosition, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `vec4`: aparece em 4º lugar de 10

**Saída** (as 20 primeiras sugestões):

```text
 1 varying
 2 vec2
 3 vec3
 4 vec4
 5 void
 6 volatile
 7 vUv [a]
 8 vNormal [a]
 9 version [a]
10 vertex [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `v`: `vec4` é o 4º (alfabético).

---

### GLSL-62
<a id="glsl-62"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 18 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

#version 300 es
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix;

out vec3 vNormal;
out vec2 vUv;

void main() {
  vNormal = normalize(uNormalMatrix * aNormal);
  vUv = aTexCoord;
  gl_Position = uProjection * uView * uModel * vec4(aP▮, 1.0);
}

```

**Palavra que a pessoa ia digitar:** `aPosition`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 aPosition [a]
```

**Veredito:** ✅ Bom. `aPosition` é o único item.

---

### GLSL-63
<a id="glsl-63"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision med▮ float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `mediump`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 mediump
```

**Veredito:** ✅ Bom. `mediump` é o único item.

---

### GLSL-64
<a id="glsl-64"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump ▮;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `float`: aparece em 60º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 abs         11 atomicMax
 2 acos        12 atomicMin
 3 acosh       13 atomic_uint
 4 all         14 attribute
 5 any         15 barrier
 6 asin        16 bitCount
 7 asinh       17 bool
 8 atan        18 break
 9 atanh       19 buffer
10 atomicAdd   20 bvec2
```

**Veredito:** ❌ Ruim. Pedido explícito depois de `precision mediump`: a lista alfabética; `float` é o 60º.

---

### GLSL-65
<a id="glsl-65"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform v▮ uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `vec3`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 vec3
 2 varying
 3 vec2
 4 vec4
 5 void
 6 volatile
 7 vNormal [a]
 8 vViewDirection [a]
```

**Veredito:** ✅ Bom. `vec3` em primeiro.

---

### GLSL-66
<a id="glsl-66"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uL▮;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `uLightDirection`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 uLightDirection [a]
```

**Veredito:** ✅ Bom. `uLightDirection` é o único item.

---

### GLSL-67
<a id="glsl-67"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec▮ uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `vec3`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 vec3
 2 vec2
 3 vec4
```

**Veredito:** ✅ Bom. `vec3` em primeiro.

---

### GLSL-68
<a id="glsl-68"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 ▮;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `uBaseColor`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 uLightDirection [a]   11 any
 2 vNormal [a]           12 asin
 3 vViewDirection [a]    13 asinh
 4 normal [a]            14 atan
 5 light [a]             15 atanh
 6 halfVector [a]        16 atomicAdd
 7 abs                   17 atomicMax
 8 acos                  18 atomicMin
 9 acosh                 19 atomic_uint
10 all                   20 attribute
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto (uniform, out, vec3...) ficam fora de alcance. `uBaseColor` não é alcançado.

---

### GLSL-69
<a id="glsl-69"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
u▮ float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `uniform`: aparece em 6º lugar de 23

**Saída** (as 20 primeiras sugestões):

```text
 1 uimage2D          11 usampler2DArray
 2 uimage3D          12 usampler2DMS
 3 uimageCube        13 usampler2DRect
 4 uint              14 usampler3D
 5 uintBitsToFloat   15 usamplerBuffer
 6 uniform           16 usamplerCube
 7 unpackUnorm4x8    17 usamplerCubeArray
 8 usampler1D        18 uvec2
 9 usampler1DArray   19 uvec3
10 usampler2D        20 uvec4
```

**Veredito:** ⚠️ Razoável, com ressalva. `u`: `uniform` é o 6º (alfabético).

---

### GLSL-70
<a id="glsl-70"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D3, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uSini▮;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `uShininess`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 uShininess [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `uShininess~`.

---

### GLSL-71
<a id="glsl-71"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
var▮ vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `varying`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 varying
```

**Veredito:** ✅ Bom. `varying` é o único item.

---

### GLSL-72
<a id="glsl-72"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 ▮;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `vNormal`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 vViewDirection [a]    11 any
 2 uBaseColor [a]        12 asin
 3 normal [a]            13 asinh
 4 light [a]             14 atan
 5 uLightDirection [a]   15 atanh
 6 halfVector [a]        16 atomicAdd
 7 abs                   17 atomicMax
 8 acos                  18 atomicMin
 9 acosh                 19 atomic_uint
10 all                   20 attribute
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto (uniform, out, vec3...) ficam fora de alcance. `vNormal` não é alcançado.

---

### GLSL-73
<a id="glsl-73"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
v▮ vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `varying`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 varying
 2 vec2
 3 vec3
 4 vec4
 5 void
 6 volatile
 7 vNormal [a]
 8 vViewDirection [a]
```

**Veredito:** ✅ Bom. `varying` em primeiro.

---

### GLSL-74
<a id="glsl-74"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vV▮;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `vViewDirection`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 vViewDirection [a]
```

**Veredito:** ✅ Bom. `vViewDirection` é o único item.

---

### GLSL-75
<a id="glsl-75"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

flo▮ diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `float`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 float
 2 floatBitsToInt
 3 floatBitsToUint
 4 floor
```

**Veredito:** ✅ Bom. `float` em primeiro.

---

### GLSL-76
<a id="glsl-76"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(▮ normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `vec3`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 normal [a]    11 atanh
 2 vNormal [a]   12 atomicAdd
 3 abs           13 atomicMax
 4 acos          14 atomicMin
 5 acosh         15 atomic_uint
 6 all           16 attribute
 7 any           17 barrier
 8 asin          18 bitCount
 9 asinh         19 bool
10 atan          20 break
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto (uniform, out, vec3...) ficam fora de alcance. `vec3` fica fora das 100 primeiras.

---

### GLSL-77
<a id="glsl-77"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 n▮, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `normal`: aparece em 5º lugar de 5

**Saída** (as 20 primeiras sugestões):

```text
 1 noperspective
 2 normalize
 3 not
 4 notEqual
 5 normal [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `n`: o parâmetro `normal` é o último (5º), depois das funções embutidas.

---

### GLSL-78
<a id="glsl-78"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 li▮) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `light`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 light [a]
 2 lighting [a]
```

**Veredito:** ✅ Bom. `light` em primeiro.

---

### GLSL-79
<a id="glsl-79"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  ret▮ max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `return`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 return
```

**Veredito:** ✅ Bom. `return` é o único item.

---

### GLSL-80
<a id="glsl-80"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(▮(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `dot`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 dot       11 atomicAdd
 2 abs       12 atomicMax
 3 acos      13 atomicMin
 4 acosh     14 atomic_uint
 5 all       15 attribute
 6 any       16 barrier
 7 asin      17 bitCount
 8 asinh     18 bool
 9 atan      19 break
10 atanh     20 buffer
```

**Veredito:** ✅ Bom. `dot` em primeiro.

---

### GLSL-81
<a id="glsl-81"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(n▮(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `normalize`: aparece em 2º lugar de 5

**Saída** (as 20 primeiras sugestões):

```text
 1 noperspective
 2 normalize
 3 not
 4 notEqual
 5 normal [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `n`: `normalize` é o 2º (depois de `noperspective`).

---

### GLSL-82
<a id="glsl-82"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), no▮(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `normalize`: aparece em 2º lugar de 5

**Saída** (as 20 primeiras sugestões):

```text
 1 noperspective
 2 normalize
 3 not
 4 notEqual
 5 normal [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `no`: `normalize` é o 2º (depois de `noperspective`).

---

### GLSL-83
<a id="glsl-83"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(lig▮)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `light`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 light [a]
 2 lighting [a]
```

**Veredito:** ✅ Bom. `light` em primeiro.

---

### GLSL-84
<a id="glsl-84"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 14 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void ▮() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `main`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 abs         11 atomicMax
 2 acos        12 atomicMin
 3 acosh       13 atomic_uint
 4 all         14 attribute
 5 any         15 barrier
 6 asin        16 bitCount
 7 asinh       17 bool
 8 atan        18 break
 9 atanh       19 buffer
10 atomicAdd   20 bvec2
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de uma função: as funções embutidas em ordem alfabética; `main` não está no vocabulário.

---

### GLSL-85
<a id="glsl-85"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 15 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  f▮ d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `float`: aparece em 6º lugar de 12

**Saída** (as 20 primeiras sugestões):

```text
 1 faceforward       11 fract
 2 false             12 fwidth
 3 findLSB
 4 findMSB
 5 flat
 6 float
 7 floatBitsToInt
 8 floatBitsToUint
 9 floor
10 for
```

**Veredito:** ⚠️ Razoável, com ressalva. `f`: `float` é o 6º (alfabético).

---

### GLSL-86
<a id="glsl-86"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 15 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = di▮(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `diffuse`: aparece em 3º lugar de 3

**Saída** (as 20 primeiras sugestões):

```text
 1 discard
 2 distance
 3 diffuse [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `di`: a função `diffuse` é a última (3ª), depois de `discard` e `distance`.

---

### GLSL-87
<a id="glsl-87"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 15 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLi▮);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `uLightDirection`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 uLightDirection [a]
```

**Veredito:** ✅ Bom. `uLightDirection` é o único item.

---

### GLSL-88
<a id="glsl-88"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  ▮ halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `vec3`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 halfVector [a]   11 atomicAdd
 2 abs              12 atomicMax
 3 acos             13 atomicMin
 4 acosh            14 atomic_uint
 5 all              15 attribute
 6 any              16 barrier
 7 asin             17 bitCount
 8 asinh            18 bool
 9 atan             19 break
10 atanh            20 buffer
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto (uniform, out, vec3...) ficam fora de alcance. `halfVector` vem primeiro (está depois do cursor), mas `vec3` fica fora das 100 primeiras.

---

### GLSL-89
<a id="glsl-89"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = n▮(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `normalize`: aparece em 2º lugar de 5

**Saída** (as 20 primeiras sugestões):

```text
 1 noperspective
 2 normalize
 3 not
 4 notEqual
 5 normal [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `n`: `normalize` é o 2º (depois de `noperspective`).

---

### GLSL-90
<a id="glsl-90"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D3, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLght▮ + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `uLightDirection`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 uLightDirection [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `uLightDirection~`.

---

### GLSL-91
<a id="glsl-91"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  flo▮ specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `float`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 float
 2 floatBitsToInt
 3 floatBitsToUint
 4 floor
```

**Veredito:** ✅ Bom. `float` em primeiro.

---

### GLSL-92
<a id="glsl-92"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float ▮ = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `specular`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 diffuse [a]      11 atanh
 2 uShininess [a]   12 atomicAdd
 3 abs              13 atomicMax
 4 acos             14 atomicMin
 5 acosh            15 atomic_uint
 6 all              16 attribute
 7 any              17 barrier
 8 asin             18 bitCount
 9 asinh            19 bool
10 atan             20 break
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto (uniform, out, vec3...) ficam fora de alcance. `diffuse` e `uShininess`, próximos, vêm primeiro; `specular` não é alcançado.

---

### GLSL-93
<a id="glsl-93"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(m▮(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `max`: aparece em 14º lugar de 20

**Saída** (as 20 primeiras sugestões):

```text
 1 mat2      11 mat4x3
 2 mat2x2    12 mat4x4
 3 mat2x3    13 matrixCompMult
 4 mat2x4    14 max
 5 mat3      15 mediump
 6 mat3x2    16 min
 7 mat3x3    17 mix
 8 mat3x4    18 mod
 9 mat4      19 modf
10 mat4x2    20 main [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `m`: `max` é o 14º de 20 (tipos de matriz vêm antes).

---

### GLSL-94
<a id="glsl-94"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(do▮(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `dot`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 dot
 2 double
```

**Veredito:** ✅ Bom. `dot` em primeiro.

---

### GLSL-95
<a id="glsl-95"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, hal▮), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `halfVector`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 halfVector [a]
```

**Veredito:** ✅ Bom. `halfVector` é o único item.

---

### GLSL-96
<a id="glsl-96"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), ▮);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `uShininess`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 abs         11 atomicMax
 2 acos        12 atomicMin
 3 acosh       13 atomic_uint
 4 all         14 attribute
 5 any         15 barrier
 6 asin        16 bitCount
 7 asinh       17 bool
 8 atan        18 break
 9 atanh       19 buffer
10 atomicAdd   20 bvec2
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto (uniform, out, vec3...) ficam fora de alcance. `uShininess`, que está ali perto, fica fora das 100 primeiras.

---

### GLSL-97
<a id="glsl-97"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 18 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { d▮; }
  gl_FragColor = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `discard`: aparece em 6º lugar de 17

**Saída** (as 20 primeiras sugestões):

```text
 1 default       11 do
 2 degrees       12 dot
 3 determinant   13 double
 4 dFdx          14 dvec2
 5 dFdy          15 dvec3
 6 discard       16 dvec4
 7 distance      17 diffuse [a]
 8 dmat2
 9 dmat3
10 dmat4
```

**Veredito:** ⚠️ Razoável, com ressalva. `d`: `discard` é o 6º de 17 (alfabético).

---

### GLSL-98
<a id="glsl-98"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 19 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl▮ = vec4(uBaseColor * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `gl_FragColor`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 glsl [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `gl`: só `glsl` (a tag da string) é oferecido; `gl_FragColor` não está no vocabulário.

---

### GLSL-99
<a id="glsl-99"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 19 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBa▮ * d + vec3(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `uBaseColor`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 uBaseColor [a]
```

**Veredito:** ✅ Bom. `uBaseColor` é o único item.

---

### GLSL-100
<a id="glsl-100"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 19 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

precision mediump float;

uniform vec3 uLightDirection;
uniform vec3 uBaseColor;
uniform float uShininess;
varying vec3 vNormal;
varying vec3 vViewDirection;

float diffuse(vec3 normal, vec3 light) {
  return max(dot(normalize(normal), normalize(light)), 0.0);
}

void main() {
  float d = diffuse(vNormal, uLightDirection);
  vec3 halfVector = normalize(uLightDirection + vViewDirection);
  float specular = pow(max(dot(vNormal, halfVector), 0.0), uShininess);
  if (d <= 0.0) { discard; }
  gl_FragColor = vec4(uBaseColor * d + ▮(specular), 1.0);
}

```

**Palavra que a pessoa ia digitar:** `vec3`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 abs         11 atomicMax
 2 acos        12 atomicMin
 3 acosh       13 atomic_uint
 4 all         14 attribute
 5 any         15 barrier
 6 asin        16 bitCount
 7 asinh       17 bool
 8 atan        18 break
 9 atanh       19 buffer
10 atomicAdd   20 bvec2
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto (uniform, out, vec3...) ficam fora de alcance. `vec3` fica fora das 100 primeiras.

---
