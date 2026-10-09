# WGSL: 100 exemplos

Resultado: ✅ 50 bons · ⚠️ 30 razoáveis com ressalva · ❌ 20 ruins.

Como ler: em cada exemplo, `▮` marca onde está o cursor. A lista é o que o plugin mostraria (as 20 primeiras). `[a]` = palavra que já existe no arquivo; `[~]` = sugestão "parecida" (corrige erro de digitação); sem marca = palavra-chave da linguagem. O veredito e o comentário são a minha análise. "Lugar na gramática" é o nome interno da regra de posição que o plugin aplicou (`sql:table`, `css:value:display`...); `(no rule)` quer dizer que o plugin não tem regra para aquele lugar e usa só o que foi digitado.

## Índice (para varrer rápido)

| # | Situação | Digitado | Palavra procurada | Posição | Lugar na gramática | Veredito |
|---|---|---|---|---|---|---|
| [1](#wgsl-1) | 1 letra | `s` | `struct` | 12 | `(no rule)` | ⚠️ |
| [2](#wgsl-2) | 2 letras | `Un` | `Uniforms` | 2 | `(no rule)` | ✅ |
| [3](#wgsl-3) | 2 letras | `mv` | `mvp` | 1 | `(no rule)` | ✅ |
| [4](#wgsl-4) | Ctrl+Espaço | (nada) | `f32` | 45 | `(no rule)` | ⚠️ |
| [5](#wgsl-5) | 1 letra | `t` | `time` | 30 | `(no rule)` | ⚠️ |
| [6](#wgsl-6) | 2 letras | `gr` | `group` | 1 | `(no rule)` | ✅ |
| [7](#wgsl-7) | 3 letras | `bin` | `binding` | 1 | `(no rule)` | ✅ |
| [8](#wgsl-8) | Ctrl+Espaço | (nada) | `uniform` | — | `(no rule)` | ⚠️ |
| [9](#wgsl-9) | 1 letra | `u` | `uniforms` | 2 | `(no rule)` | ✅ |
| [10](#wgsl-10) | erro: faltando | `gru` | `group` | 1 | `(no rule)` | ✅ |
| [11](#wgsl-11) | 3 letras | `bin` | `binding` | 1 | `(no rule)` | ✅ |
| [12](#wgsl-12) | Ctrl+Espaço | (nada) | `mySampler` | fora | `(no rule)` | ❌ |
| [13](#wgsl-13) | 1 letra | `s` | `sampler` | 1 | `(no rule)` | ✅ |
| [14](#wgsl-14) | 2 letras | `gr` | `group` | 1 | `(no rule)` | ✅ |
| [15](#wgsl-15) | 2 letras | `va` | `var` | 1 | `(no rule)` | ✅ |
| [16](#wgsl-16) | Ctrl+Espaço | (nada) | `myTexture` | fora | `(no rule)` | ❌ |
| [17](#wgsl-17) | 1 letra | `f` | `f32` | 2 | `(no rule)` | ✅ |
| [18](#wgsl-18) | 2 letras | `st` | `struct` | 3 | `(no rule)` | ✅ |
| [19](#wgsl-19) | 3 letras | `bui` | `builtin` | — | `(no rule)` | ⚠️ |
| [20](#wgsl-20) | erro: trocadas | `psoiti` | `position` | 1 | `(no rule)` | ✅ |
| [21](#wgsl-21) | 1 letra | `v` | `vec4` | 12 | `(no rule)` | ⚠️ |
| [22](#wgsl-22) | 2 letras | `f3` | `f32` | 1 | `(no rule)` | ✅ |
| [23](#wgsl-23) | 1 letra | `u` | `uv` | fora | `(no rule)` | ❌ |
| [24](#wgsl-24) | Ctrl+Espaço | (nada) | `vec2` | 1 | `(no rule)` | ✅ |
| [25](#wgsl-25) | 1 letra | `v` | `vertex` | — | `(no rule)` | ⚠️ |
| [26](#wgsl-26) | 1 letra | `f` | `fn` | 6 | `(no rule)` | ⚠️ |
| [27](#wgsl-27) | 3 letras | `vs_` | `vs_main` | — | `(no rule)` | ✅ |
| [28](#wgsl-28) | Ctrl+Espaço | (nada) | `pos` | fora | `(no rule)` | ❌ |
| [29](#wgsl-29) | 1 letra | `v` | `vec3` | 7 | `(no rule)` | ⚠️ |
| [30](#wgsl-30) | erro: faltando | `loati` | `location` | 1 | `(no rule)` | ✅ |
| [31](#wgsl-31) | 1 letra | `u` | `uv` | fora | `(no rule)` | ❌ |
| [32](#wgsl-32) | Ctrl+Espaço | (nada) | `f32` | 1 | `(no rule)` | ✅ |
| [33](#wgsl-33) | 1 letra | `V` | `VertexOutput` | 17 | `(no rule)` | ⚠️ |
| [34](#wgsl-34) | 2 letras | `ou` | `out` | 1 | `(no rule)` | ✅ |
| [35](#wgsl-35) | 3 letras | `Ver` | `VertexOutput` | 1 | `(no rule)` | ✅ |
| [36](#wgsl-36) | Ctrl+Espaço | (nada) | `position` | fora | `(no rule)` | ❌ |
| [37](#wgsl-37) | 1 letra | `u` | `uniforms` | 2 | `(no rule)` | ⚠️ |
| [38](#wgsl-38) | 2 letras | `ve` | `vec4` | 11 | `(no rule)` | ⚠️ |
| [39](#wgsl-39) | 2 letras | `f3` | `f32` | 1 | `(no rule)` | ✅ |
| [40](#wgsl-40) | Ctrl+Espaço | (nada) | `pos` | fora | `(no rule)` | ❌ |
| [41](#wgsl-41) | 1 letra | `u` | `uv` | fora | `(no rule)` | ❌ |
| [42](#wgsl-42) | 1 letra | `u` | `uv` | fora | `(no rule)` | ❌ |
| [43](#wgsl-43) | 2 letras | `ou` | `out` | 1 | `(no rule)` | ✅ |
| [44](#wgsl-44) | Ctrl+Espaço | (nada) | `fragment` | — | `(no rule)` | ⚠️ |
| [45](#wgsl-45) | 1 letra | `f` | `fs_main` | — | `(no rule)` | ⚠️ |
| [46](#wgsl-46) | 1 letra | `i` | `in` | fora | `(no rule)` | ❌ |
| [47](#wgsl-47) | 3 letras | `loc` | `location` | 1 | `(no rule)` | ✅ |
| [48](#wgsl-48) | Ctrl+Espaço | (nada) | `vec4` | fora | `(no rule)` | ❌ |
| [49](#wgsl-49) | 1 letra | `l` | `let` | 2 | `(no rule)` | ✅ |
| [50](#wgsl-50) | erro: faltando | `coo` | `color` | 8 | `(no rule)` | ⚠️ |
| [51](#wgsl-51) | 3 letras | `myT` | `myTexture` | 1 | `(no rule)` | ✅ |
| [52](#wgsl-52) | Ctrl+Espaço | (nada) | `mySampler` | fora | `(no rule)` | ❌ |
| [53](#wgsl-53) | 1 letra | `i` | `in` | fora | `(no rule)` | ❌ |
| [54](#wgsl-54) | 2 letras | `re` | `return` | 4 | `(no rule)` | ✅ |
| [55](#wgsl-55) | 3 letras | `vec` | `vec4` | 11 | `(no rule)` | ⚠️ |
| [56](#wgsl-56) | Ctrl+Espaço | (nada) | `color` | fora | `(no rule)` | ❌ |
| [57](#wgsl-57) | 1 letra | `r` | `rgb` | — | `(no rule)` | ⚠️ |
| [58](#wgsl-58) | 2 letras | `si` | `sin` | 2 | `(no rule)` | ✅ |
| [59](#wgsl-59) | 3 letras | `uni` | `uniforms` | 1 | `(no rule)` | ✅ |
| [60](#wgsl-60) | erro: trocadas | `cloo` | `color` | 1 | `(no rule)` | ✅ |
| [61](#wgsl-61) | 1 letra | `g` | `group` | 1 | `(no rule)` | ✅ |
| [62](#wgsl-62) | 2 letras | `va` | `var` | 1 | `(no rule)` | ✅ |
| [63](#wgsl-63) | 3 letras | `sto` | `storage` | 1 | `(no rule)` | ✅ |
| [64](#wgsl-64) | Ctrl+Espaço | (nada) | `read` | — | `(no rule)` | ⚠️ |
| [65](#wgsl-65) | 1 letra | `a` | `array` | 7 | `(no rule)` | ⚠️ |
| [66](#wgsl-66) | 2 letras | `f3` | `f32` | 1 | `(no rule)` | ✅ |
| [67](#wgsl-67) | 3 letras | `bin` | `binding` | 1 | `(no rule)` | ✅ |
| [68](#wgsl-68) | Ctrl+Espaço | (nada) | `var` | fora | `(no rule)` | ❌ |
| [69](#wgsl-69) | 1 letra | `r` | `read_write` | — | `(no rule)` | ⚠️ |
| [70](#wgsl-70) | erro: faltando | `oupu` | `output` | 1 | `(no rule)` | ✅ |
| [71](#wgsl-71) | 2 letras | `f3` | `f32` | 1 | `(no rule)` | ✅ |
| [72](#wgsl-72) | Ctrl+Espaço | (nada) | `const` | 25 | `(no rule)` | ⚠️ |
| [73](#wgsl-73) | 1 letra | `u` | `u32` | 1 | `(no rule)` | ✅ |
| [74](#wgsl-74) | 1 letra | `f` | `fn` | 6 | `(no rule)` | ⚠️ |
| [75](#wgsl-75) | 3 letras | `val` | `value` | 1 | `(no rule)` | ✅ |
| [76](#wgsl-76) | Ctrl+Espaço | (nada) | `f32` | 45 | `(no rule)` | ⚠️ |
| [77](#wgsl-77) | 1 letra | `f` | `f32` | 2 | `(no rule)` | ✅ |
| [78](#wgsl-78) | 2 letras | `va` | `value` | 2 | `(no rule)` | ✅ |
| [79](#wgsl-79) | 3 letras | `val` | `value` | 1 | `(no rule)` | ✅ |
| [80](#wgsl-80) | erro: trocadas | `wrokgr` | `workgroup_size` | 2 | `(no rule)` | ⚠️ |
| [81](#wgsl-81) | 1 letra | `f` | `fn` | 6 | `(no rule)` | ⚠️ |
| [82](#wgsl-82) | 2 letras | `bu` | `builtin` | — | `(no rule)` | ✅ |
| [83](#wgsl-83) | 3 letras | `glo` | `global_invocation_id` | — | `(no rule)` | ⚠️ |
| [84](#wgsl-84) | Ctrl+Espaço | (nada) | `vec3` | fora | `(no rule)` | ❌ |
| [85](#wgsl-85) | 1 letra | `u` | `u32` | 1 | `(no rule)` | ✅ |
| [86](#wgsl-86) | 2 letras | `in` | `index` | 2 | `(no rule)` | ✅ |
| [87](#wgsl-87) | 1 letra | `i` | `id` | fora | `(no rule)` | ❌ |
| [88](#wgsl-88) | Ctrl+Espaço | (nada) | `index` | fora | `(no rule)` | ❌ |
| [89](#wgsl-89) | 1 letra | `a` | `arrayLength` | 8 | `(no rule)` | ⚠️ |
| [90](#wgsl-90) | erro: faltando | `inu` | `input` | 3 | `(no rule)` | ✅ |
| [91](#wgsl-91) | 2 letras | `va` | `var` | 1 | `(no rule)` | ✅ |
| [92](#wgsl-92) | Ctrl+Espaço | (nada) | `total` | fora | `(no rule)` | ❌ |
| [93](#wgsl-93) | 1 letra | `f` | `for` | 7 | `(no rule)` | ⚠️ |
| [94](#wgsl-94) | 2 letras | `va` | `var` | 1 | `(no rule)` | ✅ |
| [95](#wgsl-95) | 3 letras | `tot` | `total` | 1 | `(no rule)` | ✅ |
| [96](#wgsl-96) | Ctrl+Espaço | (nada) | `total` | fora | `(no rule)` | ❌ |
| [97](#wgsl-97) | 1 letra | `i` | `input` | 5 | `(no rule)` | ⚠️ |
| [98](#wgsl-98) | 2 letras | `in` | `index` | 3 | `(no rule)` | ⚠️ |
| [99](#wgsl-99) | 3 letras | `out` | `output` | 1 | `(no rule)` | ✅ |
| [100](#wgsl-100) | erro: trocadas | `idne` | `index` | 1 | `(no rule)` | ✅ |

Posição: lugar da palavra procurada na lista; `—` = a palavra não existe em outro lugar do arquivo; `fora` = existe mas não está na lista.

## Os arquivos usados como entrada

Escritos à mão como um desenvolvedor escreveria (código JavaScript com strings da linguagem). Nada foi gerado pelo gerador dos experimentos.

### Documento D1

```js
const shader = wgsl`
struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}
`;
```

### Documento D2

```js
const compute = wgsl`
@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}
`;
```

## Os exemplos

### WGSL-1
<a id="wgsl-1"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

s▮ Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `struct`: aparece em 12º lugar de 14

**Saída** (as 20 primeiras sugestões):

```text
 1 sampler              11 storageBarrier
 2 sampler_comparison   12 struct
 3 saturate             13 switch
 4 select               14 shader [a]
 5 sign
 6 sin
 7 sinh
 8 smoothstep
 9 sqrt
10 step
```

**Veredito:** ⚠️ Razoável, com ressalva. `s`: `struct` é a 12ª de 14 (alfabética, depois das funções embutidas).

---

### WGSL-2
<a id="wgsl-2"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Un▮ {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `Uniforms`: aparece em 2º lugar de 2

**Saída** (as 20 primeiras sugestões):

```text
 1 uniform [a]
 2 uniforms [a]
```

**Veredito:** ✅ Bom. `uniforms`, `uniform` (palavras do arquivo).

---

### WGSL-3
<a id="wgsl-3"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mv▮ : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `mvp`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 mvp [a]
```

**Veredito:** ✅ Bom. `mvp` é o único item.

---

### WGSL-4
<a id="wgsl-4"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<▮>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `f32`: aparece em 45º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 abs           11 atan
 2 acos          12 atan2
 3 acosh         13 atanh
 4 alias         14 atomic
 5 all           15 atomicAdd
 6 any           16 atomicLoad
 7 array         17 atomicStore
 8 arrayLength   18 bitcast
 9 asin          19 bool
10 asinh         20 break
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito dentro de `<>` depois de um tipo matriz: a lista alfabética; `f32` é o 45º.

---

### WGSL-5
<a id="wgsl-5"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  t▮ : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `time`: aparece em 30º lugar de 30 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 tan                    11 texture_2d
 2 tanh                   12 texture_2d_array
 3 textureDimensions      13 texture_3d
 4 textureLoad            14 texture_cube
 5 textureSample          15 texture_cube_array
 6 textureSampleBias      16 texture_depth_2d
 7 textureSampleCompare   17 texture_depth_2d_array
 8 textureSampleLevel     18 texture_depth_cube
 9 textureStore           19 texture_depth_cube_array
10 texture_1d             20 texture_depth_multisampled_2d
```

**Veredito:** ⚠️ Razoável, com ressalva. `t`: o campo `time` é o último (30º), depois de todas as palavras-chave.

---

### WGSL-6
<a id="wgsl-6"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@gr▮(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `group`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 group [a]
```

**Veredito:** ✅ Bom. `group` é o único item (nomes de atributo não estão no vocabulário; a palavra vem do arquivo).

---

### WGSL-7
<a id="wgsl-7"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @bin▮(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `binding`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 binding [a]
```

**Veredito:** ✅ Bom. `binding` é o único item.

---

### WGSL-8
<a id="wgsl-8"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<▮> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `uniform`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 abs           11 atan
 2 acos          12 atan2
 3 acosh         13 atanh
 4 alias         14 atomic
 5 all           15 atomicAdd
 6 any           16 atomicLoad
 7 array         17 atomicStore
 8 arrayLength   18 bitcast
 9 asin          19 bool
10 asinh         20 break
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito dentro de `var<>`: a lista alfabética; `uniform` não aparece em outro lugar e as classes de armazenamento não estão no vocabulário.

---

### WGSL-9
<a id="wgsl-9"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> u▮ : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `uniforms`: aparece em 2º lugar de 3

**Saída** (as 20 primeiras sugestões):

```text
 1 u32
 2 Uniforms [a]
 3 uniform [a]
```

**Veredito:** ✅ Bom. `Uniforms`, `uniform` oferecidos; `uniforms` é o 2º.

---

### WGSL-10
<a id="wgsl-10"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D1, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@gru▮(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `group`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 group [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `group~`.

---

### WGSL-11
<a id="wgsl-11"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @bin▮(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `binding`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 binding [a]
```

**Veredito:** ✅ Bom. `binding` é o único item.

---

### WGSL-12
<a id="wgsl-12"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var ▮ : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `mySampler`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 myTexture [a]   11 asin
 2 out [a]         12 asinh
 3 abs             13 atan
 4 acos            14 atan2
 5 acosh           15 atanh
 6 alias           16 atomic
 7 all             17 atomicAdd
 8 any             18 atomicLoad
 9 array           19 atomicStore
10 arrayLength     20 bitcast
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto ficam fora de alcance. `mySampler` fica fora das 100 primeiras.

---

### WGSL-13
<a id="wgsl-13"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : s▮;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `sampler`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 sampler              11 storageBarrier
 2 sampler_comparison   12 struct
 3 saturate             13 switch
 4 select               14 shader [a]
 5 sign
 6 sin
 7 sinh
 8 smoothstep
 9 sqrt
10 step
```

**Veredito:** ✅ Bom. `sampler` em primeiro.

---

### WGSL-14
<a id="wgsl-14"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@gr▮(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `group`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 group [a]
```

**Veredito:** ✅ Bom. `group` é o único item.

---

### WGSL-15
<a id="wgsl-15"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) va▮ myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `var`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 var
```

**Veredito:** ✅ Bom. `var` é o único item.

---

### WGSL-16
<a id="wgsl-16"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var ▮ : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `myTexture`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 mySampler [a]   11 asin
 2 out [a]         12 asinh
 3 abs             13 atan
 4 acos            14 atan2
 5 acosh           15 atanh
 6 alias           16 atomic
 7 all             17 atomicAdd
 8 any             18 atomicLoad
 9 array           19 atomicStore
10 arrayLength     20 bitcast
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto ficam fora de alcance. O vizinho `mySampler` é oferecido, mas `myTexture` fica fora das 100 primeiras.

---

### WGSL-17
<a id="wgsl-17"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f▮>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `f32`: aparece em 2º lugar de 11

**Saída** (as 20 primeiras sugestões):

```text
 1 f16            11 fs_main [a]
 2 f32
 3 false
 4 floor
 5 fma
 6 fn
 7 for
 8 fract
 9 fwidth
10 fragment [a]
```

**Veredito:** ✅ Bom. `f32` é o 2º, depois de `f16`.

---

### WGSL-18
<a id="wgsl-18"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

st▮ VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `struct`: aparece em 3º lugar de 3

**Saída** (as 20 primeiras sugestões):

```text
 1 step
 2 storageBarrier
 3 struct
```

**Veredito:** ✅ Bom. `struct` é o 3º de 3.

---

### WGSL-19
<a id="wgsl-19"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @bui▮(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `builtin`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 bitcast [~]
 2 binding [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. `@bui`: nomes de atributo (`builtin`) não estão no vocabulário; `bitcast~` e `binding~` são ruído.

---

### WGSL-20
<a id="wgsl-20"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D1, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(psoiti▮) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `position`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 position [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `position~`.

---

### WGSL-21
<a id="wgsl-21"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : v▮<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `vec4`: aparece em 12º lugar de 19

**Saída** (as 20 primeiras sugestões):

```text
 1 var       11 vec3u
 2 vec2      12 vec4
 3 vec2f     13 vec4f
 4 vec2h     14 vec4h
 5 vec2i     15 vec4i
 6 vec2u     16 vec4u
 7 vec3      17 VertexOutput [a]
 8 vec3f     18 vertex [a]
 9 vec3h     19 vs_main [a]
10 vec3i
```

**Veredito:** ⚠️ Razoável, com ressalva. `v`: `vec4` é o 12º de 19 (alfabético, depois de `vec2f` e companhia).

---

### WGSL-22
<a id="wgsl-22"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f3▮>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `f32`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 f32
```

**Veredito:** ✅ Bom. `f32` é o único item.

---

### WGSL-23
<a id="wgsl-23"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 13 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) u▮ : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `uv`: **não aparece** na lista (3 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 u32
 2 uniforms [a]
 3 uniform [a]
```

**Veredito:** ❌ Ruim. `u`: o campo `uv` nunca pode ser sugerido (palavras com menos de 3 letras são ignoradas).

---

### WGSL-24
<a id="wgsl-24"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 13 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : ▮<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `vec2`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 vec2          11 asinh
 2 abs           12 atan
 3 acos          13 atan2
 4 acosh         14 atanh
 5 alias         15 atomic
 6 all           16 atomicAdd
 7 any           17 atomicLoad
 8 array         18 atomicStore
 9 arrayLength   19 bitcast
10 asin          20 bool
```

**Veredito:** ✅ Bom. `vec2` em primeiro (já veio depois de `:` antes).

---

### WGSL-25
<a id="wgsl-25"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@v▮
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `vertex`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 var       11 vec3u
 2 vec2      12 vec4
 3 vec2f     13 vec4f
 4 vec2h     14 vec4h
 5 vec2i     15 vec4i
 6 vec2u     16 vec4u
 7 vec3      17 vs_main [a]
 8 vec3f     18 VertexOutput [a]
 9 vec3h
10 vec3i
```

**Veredito:** ⚠️ Razoável, com ressalva. `@v`: `vertex` é um atributo e não está no vocabulário; tipos vetoriais são oferecidos.

---

### WGSL-26
<a id="wgsl-26"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
f▮ vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `fn`: aparece em 6º lugar de 11

**Saída** (as 20 primeiras sugestões):

```text
 1 f16            11 fs_main [a]
 2 f32
 3 false
 4 floor
 5 fma
 6 fn
 7 for
 8 fract
 9 fwidth
10 fragment [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `f`: `fn` é o 6º de 11 (alfabético).

---

### WGSL-27
<a id="wgsl-27"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_▮(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `vs_main`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; o nome da função não aparece em outro lugar.

---

### WGSL-28
<a id="wgsl-28"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) ▮ : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `pos`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 abs           11 atan
 2 acos          12 atan2
 3 acosh         13 atanh
 4 alias         14 atomic
 5 all           15 atomicAdd
 6 any           16 atomicLoad
 7 array         17 atomicStore
 8 arrayLength   18 bitcast
 9 asin          19 bool
10 asinh         20 break
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto ficam fora de alcance. `pos` tem só 3 letras mas não é alcançado.

---

### WGSL-29
<a id="wgsl-29"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : v▮<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `vec3`: aparece em 7º lugar de 19

**Saída** (as 20 primeiras sugestões):

```text
 1 var       11 vec3u
 2 vec2      12 vec4
 3 vec2f     13 vec4f
 4 vec2h     14 vec4h
 5 vec2i     15 vec4i
 6 vec2u     16 vec4u
 7 vec3      17 vs_main [a]
 8 vec3f     18 vertex [a]
 9 vec3h     19 VertexOutput [a]
10 vec3i
```

**Veredito:** ⚠️ Razoável, com ressalva. `v`: `vec3` é o 7º de 19.

---

### WGSL-30
<a id="wgsl-30"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D1, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @loati▮(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `location`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 location [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `location~`.

---

### WGSL-31
<a id="wgsl-31"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) u▮ : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `uv`: **não aparece** na lista (3 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 u32
 2 uniforms [a]
 3 uniform [a]
```

**Veredito:** ❌ Ruim. `u`: o parâmetro `uv` nunca pode ser sugerido (palavras com menos de 3 letras são ignoradas).

---

### WGSL-32
<a id="wgsl-32"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<▮>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `f32`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 f32           11 asinh
 2 abs           12 atan
 3 acos          13 atan2
 4 acosh         14 atanh
 5 alias         15 atomic
 6 all           16 atomicAdd
 7 any           17 atomicLoad
 8 array         18 atomicStore
 9 arrayLength   19 bitcast
10 asin          20 bool
```

**Veredito:** ✅ Bom. `f32` em primeiro.

---

### WGSL-33
<a id="wgsl-33"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> V▮ {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `VertexOutput`: aparece em 17º lugar de 19

**Saída** (as 20 primeiras sugestões):

```text
 1 var       11 vec3u
 2 vec2      12 vec4
 3 vec2f     13 vec4f
 4 vec2h     14 vec4h
 5 vec2i     15 vec4i
 6 vec2u     16 vec4u
 7 vec3      17 VertexOutput [a]
 8 vec3f     18 vs_main [a]
 9 vec3h     19 vertex [a]
10 vec3i
```

**Veredito:** ⚠️ Razoável, com ressalva. `V`: `VertexOutput` é o 17º de 19, depois de todos os tipos vetoriais.

---

### WGSL-34
<a id="wgsl-34"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 18 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var ou▮ : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `out`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 out [a]
```

**Veredito:** ✅ Bom. `out` é o único item.

---

### WGSL-35
<a id="wgsl-35"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 18 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : Ver▮;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `VertexOutput`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 VertexOutput [a]
 2 vertex [a]
```

**Veredito:** ✅ Bom. `VertexOutput` em primeiro.

---

### WGSL-36
<a id="wgsl-36"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 19 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.▮ = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `position`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 uv [a]        11 asinh
 2 abs           12 atan
 3 acos          13 atan2
 4 acosh         14 atanh
 5 alias         15 atomic
 6 all           16 atomicAdd
 7 any           17 atomicLoad
 8 array         18 atomicStore
 9 arrayLength   19 bitcast
10 asin          20 bool
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto ficam fora de alcance. Depois de `out.` esperam-se os campos da struct; `uv` vem primeiro mas `position` fica fora de alcance.

---

### WGSL-37
<a id="wgsl-37"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 19 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = u▮.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `uniforms`: aparece em 2º lugar de 3

**Saída** (as 20 primeiras sugestões):

```text
 1 u32
 2 uniforms [a]
 3 uniform [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `u`: `uniforms` é o 2º, depois de `u32`.

---

### WGSL-38
<a id="wgsl-38"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 19 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * ve▮<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `vec4`: aparece em 11º lugar de 17

**Saída** (as 20 primeiras sugestões):

```text
 1 vec2      11 vec4
 2 vec2f     12 vec4f
 3 vec2h     13 vec4h
 4 vec2i     14 vec4i
 5 vec2u     15 vec4u
 6 vec3      16 VertexOutput [a]
 7 vec3f     17 vertex [a]
 8 vec3h
 9 vec3i
10 vec3u
```

**Veredito:** ⚠️ Razoável, com ressalva. `ve`: `vec4` é o 11º de 17.

---

### WGSL-39
<a id="wgsl-39"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 19 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f3▮>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `f32`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 f32
```

**Veredito:** ✅ Bom. `f32` é o único item.

---

### WGSL-40
<a id="wgsl-40"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 19 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(▮, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `pos`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 color [a]     11 asinh
 2 abs           12 atan
 3 acos          13 atan2
 4 acosh         14 atanh
 5 alias         15 atomic
 6 all           16 atomicAdd
 7 any           17 atomicLoad
 8 array         18 atomicStore
 9 arrayLength   19 bitcast
10 asin          20 bool
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto ficam fora de alcance. `pos` não é alcançado; `color` (palavra próxima) é oferecida.

---

### WGSL-41
<a id="wgsl-41"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 20 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.u▮ = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `uv`: **não aparece** na lista (3 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 u32
 2 uniforms [a]
 3 uniform [a]
```

**Veredito:** ❌ Ruim. `out.u`: `uv` nunca pode ser sugerido (palavras com menos de 3 letras são ignoradas).

---

### WGSL-42
<a id="wgsl-42"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 20 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = u▮;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `uv`: **não aparece** na lista (3 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 u32
 2 uniforms [a]
 3 uniform [a]
```

**Veredito:** ❌ Ruim. `= u`: `uv` nunca pode ser sugerido (palavras com menos de 3 letras são ignoradas).

---

### WGSL-43
<a id="wgsl-43"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 21 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return ou▮;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `out`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 out [a]
```

**Veredito:** ✅ Bom. `out` é o único item.

---

### WGSL-44
<a id="wgsl-44"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 24 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@▮
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `fragment`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 fn            11 asinh
 2 abs           12 atan
 3 acos          13 atan2
 4 acosh         14 atanh
 5 alias         15 atomic
 6 all           16 atomicAdd
 7 any           17 atomicLoad
 8 array         18 atomicStore
 9 arrayLength   19 bitcast
10 asin          20 bool
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `@`: `fn` primeiro (a palavra depois do cursor), depois a lista alfabética; `fragment` não está no vocabulário.

---

### WGSL-45
<a id="wgsl-45"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 25 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn f▮(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `fs_main`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 f16
 2 f32
 3 false
 4 floor
 5 fma
 6 fn
 7 for
 8 fract
 9 fwidth
10 fragment [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `f`: `fs_main` não aparece em outro lugar; `fn` e outras são oferecidas.

---

### WGSL-46
<a id="wgsl-46"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 25 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(i▮ : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `in`: **não aparece** na lista (3 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 i32
 2 if
 3 inverseSqrt
```

**Veredito:** ❌ Ruim. `i`: o parâmetro `in` nunca pode ser sugerido (palavras com menos de 3 letras são ignoradas).

---

### WGSL-47
<a id="wgsl-47"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 25 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @loc▮(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `location`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 location [a]
```

**Veredito:** ✅ Bom. `location` é o único item.

---

### WGSL-48
<a id="wgsl-48"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 25 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) ▮<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `vec4`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 abs           11 atan
 2 acos          12 atan2
 3 acosh         13 atanh
 4 alias         14 atomic
 5 all           15 atomicAdd
 6 any           16 atomicLoad
 7 array         17 atomicStore
 8 arrayLength   18 bitcast
 9 asin          19 bool
10 asinh         20 break
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto ficam fora de alcance. `vec4` fica fora das 100 primeiras.

---

### WGSL-49
<a id="wgsl-49"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 26 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  l▮ color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `let`: aparece em 2º lugar de 6

**Saída** (as 20 primeiras sugestões):

```text
 1 length
 2 let
 3 log
 4 log2
 5 loop
 6 location [a]
```

**Veredito:** ✅ Bom. `let` é o 2º, depois de `length`.

---

### WGSL-50
<a id="wgsl-50"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D1, linha 26 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let coo▮ = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `color`: aparece em 8º lugar de 8

**Saída** (as 20 primeiras sugestões):

```text
 1 const [~]
 2 const_assert [~]
 3 continue [~]
 4 continuing [~]
 5 cos [~]
 6 cosh [~]
 7 cross [~]
 8 color [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. Erro `coo`: `color~` é o último (8º), depois de sete palavras-chave que começam igual.

---

### WGSL-51
<a id="wgsl-51"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 26 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myT▮, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `myTexture`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 myTexture [a]
```

**Veredito:** ✅ Bom. `myTexture` é o único item.

---

### WGSL-52
<a id="wgsl-52"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 26 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, ▮, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `mySampler`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 abs           11 atan
 2 acos          12 atan2
 3 acosh         13 atanh
 4 alias         14 atomic
 5 all           15 atomicAdd
 6 any           16 atomicLoad
 7 array         17 atomicStore
 8 arrayLength   18 bitcast
 9 asin          19 bool
10 asinh         20 break
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto ficam fora de alcance. `mySampler` fica fora das 100 primeiras.

---

### WGSL-53
<a id="wgsl-53"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 26 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, i▮.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `in`: **não aparece** na lista (3 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 i32
 2 if
 3 inverseSqrt
```

**Veredito:** ❌ Ruim. `i`: `in` nunca pode ser sugerido (palavras com menos de 3 letras são ignoradas).

---

### WGSL-54
<a id="wgsl-54"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 27 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  re▮ vec4<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `return`: aparece em 4º lugar de 4

**Saída** (as 20 primeiras sugestões):

```text
 1 reflect
 2 refract
 3 requires
 4 return
```

**Veredito:** ✅ Bom. `return` é o 4º de 4.

---

### WGSL-55
<a id="wgsl-55"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 27 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec▮<f32>(color.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `vec4`: aparece em 11º lugar de 15

**Saída** (as 20 primeiras sugestões):

```text
 1 vec2      11 vec4
 2 vec2f     12 vec4f
 3 vec2h     13 vec4h
 4 vec2i     14 vec4i
 5 vec2u     15 vec4u
 6 vec3
 7 vec3f
 8 vec3h
 9 vec3i
10 vec3u
```

**Veredito:** ⚠️ Razoável, com ressalva. `vec`: `vec4` é o 11º de 15 (alfabético).

---

### WGSL-56
<a id="wgsl-56"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 27 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(▮.rgb * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `color`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 pos [a]       11 asinh
 2 abs           12 atan
 3 acos          13 atan2
 4 acosh         14 atanh
 5 alias         15 atomic
 6 all           16 atomicAdd
 7 any           17 atomicLoad
 8 array         18 atomicStore
 9 arrayLength   19 bitcast
10 asin          20 bool
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto ficam fora de alcance. `color` fica fora das 100 primeiras.

---

### WGSL-57
<a id="wgsl-57"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 27 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.r▮ * abs(sin(uniforms.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `rgb`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 radians
 2 reflect
 3 refract
 4 requires
 5 return
 6 round
```

**Veredito:** ⚠️ Razoável, com ressalva. `r`: `rgb` não aparece em outro lugar (swizzles não estão no vocabulário); `return`, `round`... são oferecidas.

---

### WGSL-58
<a id="wgsl-58"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 27 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(si▮(uniforms.time)), color.a);
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

### WGSL-59
<a id="wgsl-59"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 27 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uni▮.time)), color.a);
}

```

**Palavra que a pessoa ia digitar:** `uniforms`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 uniforms [a]
 2 uniform [a]
```

**Veredito:** ✅ Bom. `uniforms` em primeiro.

---

### WGSL-60
<a id="wgsl-60"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D1, linha 27 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

struct Uniforms {
  mvp : mat4x4<f32>,
  time : f32,
};

@group(0) @binding(0) var<uniform> uniforms : Uniforms;
@group(0) @binding(1) var mySampler : sampler;
@group(0) @binding(2) var myTexture : texture_2d<f32>;

struct VertexOutput {
  @builtin(position) position : vec4<f32>,
  @location(0) uv : vec2<f32>,
};

@vertex
fn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>) -> VertexOutput {
  var out : VertexOutput;
  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);
  out.uv = uv;
  return out;
}

@fragment
fn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {
  let color = textureSample(myTexture, mySampler, in.uv);
  return vec4<f32>(color.rgb * abs(sin(uniforms.time)), cloo▮.a);
}

```

**Palavra que a pessoa ia digitar:** `color`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 color [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `color~`.

---

### WGSL-61
<a id="wgsl-61"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@g▮(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `group`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 group [a]
 2 global_invocation_id [a]
```

**Veredito:** ✅ Bom. `group` em primeiro.

---

### WGSL-62
<a id="wgsl-62"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) va▮<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `var`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 var
 2 value [a]
```

**Veredito:** ✅ Bom. `var` em primeiro.

---

### WGSL-63
<a id="wgsl-63"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<sto▮, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `storage`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 storage [a]
 2 storageBarrier
```

**Veredito:** ✅ Bom. `storage` em primeiro.

---

### WGSL-64
<a id="wgsl-64"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, ▮> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `read`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 read_write [a]   11 asinh
 2 abs              12 atan
 3 acos             13 atan2
 4 acosh            14 atanh
 5 alias            15 atomic
 6 all              16 atomicAdd
 7 any              17 atomicLoad
 8 array            18 atomicStore
 9 arrayLength      19 bitcast
10 asin             20 bool
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `read, `: `read_write` primeiro (a palavra depois do cursor); `read` não aparece em outro lugar e os modos de acesso não estão no vocabulário.

---

### WGSL-65
<a id="wgsl-65"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : a▮<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `array`: aparece em 7º lugar de 17

**Saída** (as 20 primeiras sugestões):

```text
 1 abs           11 atan
 2 acos          12 atan2
 3 acosh         13 atanh
 4 alias         14 atomic
 5 all           15 atomicAdd
 6 any           16 atomicLoad
 7 array         17 atomicStore
 8 arrayLength
 9 asin
10 asinh
```

**Veredito:** ⚠️ Razoável, com ressalva. `a`: `array` é o 7º de 17 (alfabético).

---

### WGSL-66
<a id="wgsl-66"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f3▮>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `f32`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 f32
```

**Veredito:** ✅ Bom. `f32` é o único item.

---

### WGSL-67
<a id="wgsl-67"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @bin▮(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `binding`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 binding [a]
```

**Veredito:** ✅ Bom. `binding` é o único item.

---

### WGSL-68
<a id="wgsl-68"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) ▮<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `var`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 abs           11 atan
 2 acos          12 atan2
 3 acosh         13 atanh
 4 alias         14 atomic
 5 all           15 atomicAdd
 6 any           16 atomicLoad
 7 array         17 atomicStore
 8 arrayLength   18 bitcast
 9 asin          19 bool
10 asinh         20 break
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto ficam fora de alcance. `var` fica fora das 100 primeiras.

---

### WGSL-69
<a id="wgsl-69"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, r▮> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `read_write`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 read [a]
 2 radians
 3 reflect
 4 refract
 5 requires
 6 return
 7 round
```

**Veredito:** ⚠️ Razoável, com ressalva. `r`: `read` primeiro; `read_write` não aparece em outro lugar.

---

### WGSL-70
<a id="wgsl-70"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D2, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> oupu▮ : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `output`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 output [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `output~`.

---

### WGSL-71
<a id="wgsl-71"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f3▮>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `f32`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 f32
```

**Veredito:** ✅ Bom. `f32` é o único item.

---

### WGSL-72
<a id="wgsl-72"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

▮ WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `const`: aparece em 25º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 WORKGROUP_SIZE [a]   11 asinh
 2 abs                  12 atan
 3 acos                 13 atan2
 4 acosh                14 atanh
 5 alias                15 atomic
 6 all                  16 atomicAdd
 7 any                  17 atomicLoad
 8 array                18 atomicStore
 9 arrayLength          19 bitcast
10 asin                 20 bool
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito numa palavra de declaração: a palavra depois do cursor primeiro; `const` é o 25º (alfabético).

---

### WGSL-73
<a id="wgsl-73"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u▮ = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `u32`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 u32
```

**Veredito:** ✅ Bom. `u32` é o único item.

---

### WGSL-74
<a id="wgsl-74"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

f▮ square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `fn`: aparece em 6º lugar de 9

**Saída** (as 20 primeiras sugestões):

```text
 1 f16
 2 f32
 3 false
 4 floor
 5 fma
 6 fn
 7 for
 8 fract
 9 fwidth
```

**Veredito:** ⚠️ Razoável, com ressalva. `f`: `fn` é o 6º de 9 (alfabético).

---

### WGSL-75
<a id="wgsl-75"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(val▮ : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `value`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 value [a]
```

**Veredito:** ✅ Bom. `value` é o único item.

---

### WGSL-76
<a id="wgsl-76"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : ▮) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `f32`: aparece em 45º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 abs           11 atan
 2 acos          12 atan2
 3 acosh         13 atanh
 4 alias         14 atomic
 5 all           15 atomicAdd
 6 any           16 atomicLoad
 7 array         17 atomicStore
 8 arrayLength   18 bitcast
 9 asin          19 bool
10 asinh         20 break
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `value :`: a lista alfabética; `f32` é o 45º.

---

### WGSL-77
<a id="wgsl-77"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f▮ {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `f32`: aparece em 2º lugar de 9

**Saída** (as 20 primeiras sugestões):

```text
 1 f16
 2 f32
 3 false
 4 floor
 5 fma
 6 fn
 7 for
 8 fract
 9 fwidth
```

**Veredito:** ✅ Bom. `f32` é o 2º.

---

### WGSL-78
<a id="wgsl-78"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return va▮ * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `value`: aparece em 2º lugar de 2

**Saída** (as 20 primeiras sugestões):

```text
 1 var
 2 value [a]
```

**Veredito:** ✅ Bom. `var`, `value`.

---

### WGSL-79
<a id="wgsl-79"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * val▮;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `value`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 value [a]
```

**Veredito:** ✅ Bom. `value` é o único item.

---

### WGSL-80
<a id="wgsl-80"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D2, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @wrokgr▮(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `workgroup_size`: aparece em 2º lugar de 2

**Saída** (as 20 primeiras sugestões):

```text
 1 workgroupBarrier [~]
 2 WORKGROUP_SIZE [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. Erro `wrokgr`: `workgroupBarrier~` antes de `WORKGROUP_SIZE~`; `workgroup_size` é um atributo, não está no vocabulário.

---

### WGSL-81
<a id="wgsl-81"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
f▮ main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `fn`: aparece em 6º lugar de 9

**Saída** (as 20 primeiras sugestões):

```text
 1 f16
 2 f32
 3 false
 4 floor
 5 fma
 6 fn
 7 for
 8 fract
 9 fwidth
```

**Veredito:** ⚠️ Razoável, com ressalva. `f`: `fn` é o 6º de 9 (alfabético).

---

### WGSL-82
<a id="wgsl-82"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@bu▮(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `builtin`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; `builtin` não está no vocabulário e não aparece em outro lugar.

---

### WGSL-83
<a id="wgsl-83"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(glo▮) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `global_invocation_id`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 group [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. `glo`: `group~` é ruído; `global_invocation_id` não aparece em outro lugar.

---

### WGSL-84
<a id="wgsl-84"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : ▮<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `vec3`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 abs           11 atan
 2 acos          12 atan2
 3 acosh         13 atanh
 4 alias         14 atomic
 5 all           15 atomicAdd
 6 any           16 atomicLoad
 7 array         17 atomicStore
 8 arrayLength   18 bitcast
 9 asin          19 bool
10 asinh         20 break
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto ficam fora de alcance. `vec3` fica fora das 100 primeiras.

---

### WGSL-85
<a id="wgsl-85"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u▮>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `u32`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 u32
```

**Veredito:** ✅ Bom. `u32` é o único item.

---

### WGSL-86
<a id="wgsl-86"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 13 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let in▮ = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `index`: aparece em 2º lugar de 3

**Saída** (as 20 primeiras sugestões):

```text
 1 inverseSqrt
 2 index [a]
 3 input [a]
```

**Veredito:** ✅ Bom. `index` é o 2º (depois de `inverseSqrt`).

---

### WGSL-87
<a id="wgsl-87"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 13 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = i▮.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `id`: **não aparece** na lista (5 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 i32
 2 if
 3 inverseSqrt
 4 index [a]
 5 input [a]
```

**Veredito:** ❌ Ruim. `i`: `id` nunca pode ser sugerido (palavras com menos de 3 letras são ignoradas).

---

### WGSL-88
<a id="wgsl-88"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 14 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (▮ >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `index`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 abs           11 atan
 2 acos          12 atan2
 3 acosh         13 atanh
 4 alias         14 atomic
 5 all           15 atomicAdd
 6 any           16 atomicLoad
 7 array         17 atomicStore
 8 arrayLength   18 bitcast
 9 asin          19 bool
10 asinh         20 break
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto ficam fora de alcance. `index` fica fora das 100 primeiras.

---

### WGSL-89
<a id="wgsl-89"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 14 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= a▮(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `arrayLength`: aparece em 8º lugar de 17

**Saída** (as 20 primeiras sugestões):

```text
 1 abs           11 atan
 2 acos          12 atan2
 3 acosh         13 atanh
 4 alias         14 atomic
 5 all           15 atomicAdd
 6 any           16 atomicLoad
 7 array         17 atomicStore
 8 arrayLength
 9 asin
10 asinh
```

**Veredito:** ⚠️ Razoável, com ressalva. `a`: `arrayLength` é o 8º de 17 (alfabético).

---

### WGSL-90
<a id="wgsl-90"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D2, linha 14 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&inu▮)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `input`: aparece em 3º lugar de 3

**Saída** (as 20 primeiras sugestões):

```text
 1 inverseSqrt [~]
 2 index [~]
 3 input [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `input~` (3º).

---

### WGSL-91
<a id="wgsl-91"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  va▮ total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `var`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 var
 2 value [a]
```

**Veredito:** ✅ Bom. `var` em primeiro.

---

### WGSL-92
<a id="wgsl-92"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 17 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var ▮ : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `total`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 abs           11 atan
 2 acos          12 atan2
 3 acosh         13 atanh
 4 alias         14 atomic
 5 all           15 atomicAdd
 6 any           16 atomicLoad
 7 array         17 atomicStore
 8 arrayLength   18 bitcast
 9 asin          19 bool
10 asinh         20 break
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto ficam fora de alcance. `total` fica fora das 100 primeiras.

---

### WGSL-93
<a id="wgsl-93"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 18 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  f▮ (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `for`: aparece em 7º lugar de 9

**Saída** (as 20 primeiras sugestões):

```text
 1 f16
 2 f32
 3 false
 4 floor
 5 fma
 6 fn
 7 for
 8 fract
 9 fwidth
```

**Veredito:** ⚠️ Razoável, com ressalva. `f`: `for` é o 7º de 9 (alfabético).

---

### WGSL-94
<a id="wgsl-94"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 18 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (va▮ i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `var`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 var
 2 value [a]
```

**Veredito:** ✅ Bom. `var` em primeiro.

---

### WGSL-95
<a id="wgsl-95"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 19 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    tot▮ = total + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `total`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 total [a]
```

**Veredito:** ✅ Bom. `total` é o único item.

---

### WGSL-96
<a id="wgsl-96"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 19 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = ▮ + square(input[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `total`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 abs           11 atan
 2 acos          12 atan2
 3 acosh         13 atanh
 4 alias         14 atomic
 5 all           15 atomicAdd
 6 any           16 atomicLoad
 7 array         17 atomicStore
 8 arrayLength   18 bitcast
 9 asin          19 bool
10 asinh         20 break
```

**Veredito:** ❌ Ruim. Pedido explícito: a lista de palavras-chave é alfabética e cortada em 100, então os nomes do próprio arquivo e as palavras-chave do fim do alfabeto ficam fora de alcance. `total` fica fora das 100 primeiras.

---

### WGSL-97
<a id="wgsl-97"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 19 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(i▮[index] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `input`: aparece em 5º lugar de 5

**Saída** (as 20 primeiras sugestões):

```text
 1 i32
 2 if
 3 inverseSqrt
 4 index [a]
 5 input [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `i`: `input` é o último (5º), depois de `i32`, `if`, `inverseSqrt` e `index`.

---

### WGSL-98
<a id="wgsl-98"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 19 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[in▮] + f32(i));
  }
  output[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `index`: aparece em 3º lugar de 3

**Saída** (as 20 primeiras sugestões):

```text
 1 inverseSqrt
 2 input [a]
 3 index [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `in`: `input` é o 3º de 3 (`inverseSqrt` primeiro).

---

### WGSL-99
<a id="wgsl-99"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 21 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  out▮[index] = total;
}

```

**Palavra que a pessoa ia digitar:** `output`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 output [a]
```

**Veredito:** ✅ Bom. `output` é o único item.

---

### WGSL-100
<a id="wgsl-100"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D2, linha 21 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

@group(0) @binding(0) var<storage, read> input : array<f32>;
@group(0) @binding(1) var<storage, read_write> output : array<f32>;

const WORKGROUP_SIZE : u32 = 64u;

fn square(value : f32) -> f32 {
  return value * value;
}

@compute @workgroup_size(64)
fn main(@builtin(global_invocation_id) id : vec3<u32>) {
  let index = id.x;
  if (index >= arrayLength(&input)) {
    return;
  }
  var total : f32 = 0.0;
  for (var i : u32 = 0u; i < 4u; i = i + 1u) {
    total = total + square(input[index] + f32(i));
  }
  output[idne▮] = total;
}

```

**Palavra que a pessoa ia digitar:** `index`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 index [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `index~`.

---
