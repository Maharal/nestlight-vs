# GraphQL: 100 exemplos

Resultado: ✅ 61 bons · ⚠️ 39 razoáveis com ressalva · ❌ 0 ruins.

Como ler: em cada exemplo, `▮` marca onde está o cursor. A lista é o que o plugin mostraria (as 20 primeiras). `[a]` = palavra que já existe no arquivo; `[~]` = sugestão "parecida" (corrige erro de digitação); sem marca = palavra-chave da linguagem. O veredito e o comentário são a minha análise. "Lugar na gramática" é o nome interno da regra de posição que o plugin aplicou (`sql:table`, `css:value:display`...); `(no rule)` quer dizer que o plugin não tem regra para aquele lugar e usa só o que foi digitado.

## Índice (para varrer rápido)

| # | Situação | Digitado | Palavra procurada | Posição | Lugar na gramática | Veredito |
|---|---|---|---|---|---|---|
| [1](#graphql-1) | 1 letra | `q` | `query` | 1 | `(no rule)` | ✅ |
| [2](#graphql-2) | 2 letras | `Ge` | `GetUser` | — | `(no rule)` | ⚠️ |
| [3](#graphql-3) | 1 letra | `I` | `ID` | 1 | `(no rule)` | ✅ |
| [4](#graphql-4) | Ctrl+Espaço | (nada) | `Int` | 13 | `(no rule)` | ⚠️ |
| [5](#graphql-5) | 1 letra | `u` | `user` | — | `(no rule)` | ✅ |
| [6](#graphql-6) | 1 letra | `i` | `id` | 1 | `(no rule)` | ⚠️ |
| [7](#graphql-7) | 3 letras | `nam` | `name` | — | `(no rule)` | ✅ |
| [8](#graphql-8) | Ctrl+Espaço | (nada) | `email` | — | `(no rule)` | ⚠️ |
| [9](#graphql-9) | 1 letra | `p` | `posts` | — | `(no rule)` | ✅ |
| [10](#graphql-10) | erro: faltando | `fis` | `first` | 1 | `(no rule)` | ✅ |
| [11](#graphql-11) | 3 letras | `nod` | `node` | — | `(no rule)` | ✅ |
| [12](#graphql-12) | Ctrl+Espaço | (nada) | `id` | 10 | `(no rule)` | ⚠️ |
| [13](#graphql-13) | 1 letra | `t` | `title` | — | `(no rule)` | ✅ |
| [14](#graphql-14) | 2 letras | `pu` | `publishedAt` | — | `(no rule)` | ✅ |
| [15](#graphql-15) | 3 letras | `pag` | `pageInfo` | — | `(no rule)` | ✅ |
| [16](#graphql-16) | Ctrl+Espaço | (nada) | `endCursor` | — | `(no rule)` | ⚠️ |
| [17](#graphql-17) | 1 letra | `m` | `mutation` | 1 | `(no rule)` | ✅ |
| [18](#graphql-18) | 2 letras | `Cr` | `CreatePost` | 2 | `(no rule)` | ✅ |
| [19](#graphql-19) | 3 letras | `Cre` | `CreatePostInput` | — | `(no rule)` | ✅ |
| [20](#graphql-20) | erro: trocadas | `cerate` | `createPost` | 2 | `(no rule)` | ✅ |
| [21](#graphql-21) | 1 letra | `p` | `post` | — | `(no rule)` | ✅ |
| [22](#graphql-22) | 1 letra | `i` | `id` | 1 | `(no rule)` | ⚠️ |
| [23](#graphql-23) | 3 letras | `tit` | `title` | 1 | `(no rule)` | ✅ |
| [24](#graphql-24) | Ctrl+Espaço | (nada) | `body` | — | `(no rule)` | ⚠️ |
| [25](#graphql-25) | 1 letra | `a` | `author` | — | `(no rule)` | ✅ |
| [26](#graphql-26) | 2 letras | `na` | `name` | — | `(no rule)` | ✅ |
| [27](#graphql-27) | 3 letras | `err` | `errors` | — | `(no rule)` | ✅ |
| [28](#graphql-28) | Ctrl+Espaço | (nada) | `field` | — | `(no rule)` | ⚠️ |
| [29](#graphql-29) | 1 letra | `m` | `message` | — | `(no rule)` | ⚠️ |
| [30](#graphql-30) | erro: faltando | `suscr` | `subscription` | 1 | `(no rule)` | ✅ |
| [31](#graphql-31) | 1 letra | `I` | `ID` | 1 | `(no rule)` | ✅ |
| [32](#graphql-32) | Ctrl+Espaço | (nada) | `postAdded` | — | `(no rule)` | ⚠️ |
| [33](#graphql-33) | 1 letra | `a` | `authorId` | 1 | `(no rule)` | ✅ |
| [34](#graphql-34) | 1 letra | `i` | `id` | 1 | `(no rule)` | ⚠️ |
| [35](#graphql-35) | 3 letras | `tit` | `title` | 1 | `(no rule)` | ✅ |
| [36](#graphql-36) | Ctrl+Espaço | (nada) | `type` | 27 | `(no rule)` | ⚠️ |
| [37](#graphql-37) | 1 letra | `U` | `User` | 2 | `(no rule)` | ⚠️ |
| [38](#graphql-38) | 2 letras | `im` | `implements` | 1 | `(no rule)` | ✅ |
| [39](#graphql-39) | 3 letras | `Nod` | `Node` | 1 | `(no rule)` | ✅ |
| [40](#graphql-40) | Ctrl+Espaço | (nada) | `id` | 1 | `(no rule)` | ✅ |
| [41](#graphql-41) | 1 letra | `n` | `name` | — | `(no rule)` | ⚠️ |
| [42](#graphql-42) | 2 letras | `St` | `String` | 1 | `(no rule)` | ✅ |
| [43](#graphql-43) | 3 letras | `ema` | `email` | — | `(no rule)` | ✅ |
| [44](#graphql-44) | Ctrl+Espaço | (nada) | `String` | 25 | `(no rule)` | ⚠️ |
| [45](#graphql-45) | 1 letra | `r` | `role` | 2 | `(no rule)` | ⚠️ |
| [46](#graphql-46) | 2 letras | `po` | `posts` | — | `(no rule)` | ✅ |
| [47](#graphql-47) | 3 letras | `fir` | `first` | — | `(no rule)` | ⚠️ |
| [48](#graphql-48) | Ctrl+Espaço | (nada) | `Int` | 13 | `(no rule)` | ⚠️ |
| [49](#graphql-49) | 1 letra | `a` | `after` | — | `(no rule)` | ✅ |
| [50](#graphql-50) | erro: faltando | `Stin` | `String` | 1 | `(no rule)` | ✅ |
| [51](#graphql-51) | 3 letras | `enu` | `enum` | 1 | `(no rule)` | ✅ |
| [52](#graphql-52) | Ctrl+Espaço | (nada) | `Role` | 38 | `(no rule)` | ⚠️ |
| [53](#graphql-53) | 1 letra | `A` | `ADMIN` | — | `(no rule)` | ✅ |
| [54](#graphql-54) | 2 letras | `ED` | `EDITOR` | — | `(no rule)` | ✅ |
| [55](#graphql-55) | 3 letras | `REA` | `READER` | — | `(no rule)` | ⚠️ |
| [56](#graphql-56) | Ctrl+Espaço | (nada) | `CreatePostInput` | 46 | `(no rule)` | ⚠️ |
| [57](#graphql-57) | 1 letra | `t` | `title` | — | `(no rule)` | ✅ |
| [58](#graphql-58) | 2 letras | `St` | `String` | 1 | `(no rule)` | ✅ |
| [59](#graphql-59) | 3 letras | `bod` | `body` | — | `(no rule)` | ⚠️ |
| [60](#graphql-60) | erro: trocadas | `Srtin` | `String` | 1 | `(no rule)` | ✅ |
| [61](#graphql-61) | 1 letra | `S` | `String` | 5 | `(no rule)` | ⚠️ |
| [62](#graphql-62) | 2 letras | `in` | `interface` | 4 | `(no rule)` | ✅ |
| [63](#graphql-63) | 3 letras | `Nod` | `Node` | 1 | `(no rule)` | ✅ |
| [64](#graphql-64) | Ctrl+Espaço | (nada) | `id` | 1 | `(no rule)` | ✅ |
| [65](#graphql-65) | 1 letra | `I` | `ID` | 1 | `(no rule)` | ✅ |
| [66](#graphql-66) | 2 letras | `Qu` | `Query` | 1 | `(no rule)` | ✅ |
| [67](#graphql-67) | 3 letras | `use` | `user` | 1 | `(no rule)` | ✅ |
| [68](#graphql-68) | Ctrl+Espaço | (nada) | `id` | 9 | `(no rule)` | ⚠️ |
| [69](#graphql-69) | 1 letra | `I` | `ID` | 1 | `(no rule)` | ✅ |
| [70](#graphql-70) | 2 letras | `Us` | `User` | 2 | `(no rule)` | ✅ |
| [71](#graphql-71) | 3 letras | `rol` | `role` | 1 | `(no rule)` | ✅ |
| [72](#graphql-72) | Ctrl+Espaço | (nada) | `Role` | 1 | `(no rule)` | ✅ |
| [73](#graphql-73) | 1 letra | `U` | `User` | 3 | `(no rule)` | ⚠️ |
| [74](#graphql-74) | 2 letras | `ty` | `type` | 1 | `(no rule)` | ✅ |
| [75](#graphql-75) | 3 letras | `Mut` | `Mutation` | 1 | `(no rule)` | ✅ |
| [76](#graphql-76) | Ctrl+Espaço | (nada) | `input` | 12 | `(no rule)` | ⚠️ |
| [77](#graphql-77) | 1 letra | `C` | `CreatePostInput` | 3 | `(no rule)` | ⚠️ |
| [78](#graphql-78) | 2 letras | `Cr` | `CreatePostPayload` | — | `(no rule)` | ✅ |
| [79](#graphql-79) | 3 letras | `fra` | `fragment` | 1 | `(no rule)` | ✅ |
| [80](#graphql-80) | erro: trocadas | `UesrFi` | `UserFields` | 1 | `(no rule)` | ✅ |
| [81](#graphql-81) | 1 letra | `U` | `User` | — | `(no rule)` | ⚠️ |
| [82](#graphql-82) | 1 letra | `i` | `id` | 1 | `(no rule)` | ⚠️ |
| [83](#graphql-83) | 3 letras | `nam` | `name` | — | `(no rule)` | ✅ |
| [84](#graphql-84) | Ctrl+Espaço | (nada) | `avatar` | — | `(no rule)` | ⚠️ |
| [85](#graphql-85) | 1 letra | `s` | `size` | — | `(no rule)` | ⚠️ |
| [86](#graphql-86) | 2 letras | `Ad` | `Admin` | — | `(no rule)` | ✅ |
| [87](#graphql-87) | 3 letras | `per` | `permissions` | — | `(no rule)` | ✅ |
| [88](#graphql-88) | Ctrl+Espaço | (nada) | `query` | 18 | `(no rule)` | ⚠️ |
| [89](#graphql-89) | 1 letra | `S` | `Search` | 7 | `(no rule)` | ⚠️ |
| [90](#graphql-90) | erro: faltando | `Stin` | `String` | 1 | `(no rule)` | ✅ |
| [91](#graphql-91) | 3 letras | `fal` | `false` | 1 | `(no rule)` | ✅ |
| [92](#graphql-92) | Ctrl+Espaço | (nada) | `search` | 33 | `(no rule)` | ⚠️ |
| [93](#graphql-93) | 1 letra | `t` | `term` | 3 | `(no rule)` | ⚠️ |
| [94](#graphql-94) | 2 letras | `Us` | `UserFields` | 3 | `(no rule)` | ✅ |
| [95](#graphql-95) | 3 letras | `pos` | `posts` | — | `(no rule)` | ✅ |
| [96](#graphql-96) | Ctrl+Espaço | (nada) | `id` | 10 | `(no rule)` | ⚠️ |
| [97](#graphql-97) | 1 letra | `t` | `title` | — | `(no rule)` | ✅ |
| [98](#graphql-98) | 2 letras | `le` | `legacyId` | — | `(no rule)` | ✅ |
| [99](#graphql-99) | 3 letras | `rea` | `reason` | — | `(no rule)` | ⚠️ |
| [100](#graphql-100) | Ctrl+Espaço | (nada) | `use` | — | `(no rule)` | ⚠️ |

Posição: lugar da palavra procurada na lista; `—` = a palavra não existe em outro lugar do arquivo; `fora` = existe mas não está na lista.

## Os arquivos usados como entrada

Escritos à mão como um desenvolvedor escreveria (código JavaScript com strings da linguagem). Nada foi gerado pelo gerador dos experimentos.

### Documento D1

```js
export const GET_USER = gql`
  query GetUser($id: ID!, $first: Int = 10) {
    user(id: $id) {
      id
      name
      email
      posts(first: $first) {
        edges { node { id title publishedAt } }
        pageInfo { hasNextPage endCursor }
      }
    }
  }
`;
```

### Documento D2

```js
export const CREATE_POST = gql`
  mutation CreatePost($input: CreatePostInput!) {
    createPost(input: $input) {
      post { id title body author { id name } }
      errors { field message }
    }
  }
`;

export const POST_ADDED = gql`
  subscription OnPostAdded($authorId: ID) {
    postAdded(authorId: $authorId) { id title createdAt }
  }
`;
```

### Documento D3

```js
const typeDefs = graphql`
  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }
`;
```

### Documento D4

```js
export const USER_FIELDS = gql`
  fragment UserFields on User {
    id
    name
    avatar(size: 64)
    ... on Admin { permissions }
  }
`;

export const SEARCH = gql`
  query Search($term: String!, $withPosts: Boolean = false) {
    search(term: $term) {
      ...UserFields
      posts @include(if: $withPosts) { id title }
      legacyId @deprecated(reason: "use id")
    }
  }
`;
```

## Os exemplos

### GraphQL-1
<a id="graphql-1"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  q▮ GetUser($id: ID!, $first: Int = 10) {
    user(id: $id) {
      id
      name
      email
      posts(first: $first) {
        edges { node { id title publishedAt } }
        pageInfo { hasNextPage endCursor }
      }
    }
  }

```

**Palavra que a pessoa ia digitar:** `query`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 query
```

**Veredito:** ✅ Bom. `query` é o único item.

---

### GraphQL-2
<a id="graphql-2"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query Ge▮($id: ID!, $first: Int = 10) {
    user(id: $id) {
      id
      name
      email
      posts(first: $first) {
        edges { node { id title publishedAt } }
        pageInfo { hasNextPage endCursor }
      }
    }
  }

```

**Palavra que a pessoa ia digitar:** `GetUser`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 GET_USER [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. O nome de uma operação é novo; `GET_USER` (uma constante do código hospedeiro) é a única sugestão.

---

### GraphQL-3
<a id="graphql-3"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query GetUser($id: I▮!, $first: Int = 10) {
    user(id: $id) {
      id
      name
      email
      posts(first: $first) {
        edges { node { id title publishedAt } }
        pageInfo { hasNextPage endCursor }
      }
    }
  }

```

**Palavra que a pessoa ia digitar:** `ID`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 ID
 2 implements
 3 include
 4 input
 5 Int
 6 interface
```

**Veredito:** ✅ Bom. `ID` em primeiro (um tipo).

---

### GraphQL-4
<a id="graphql-4"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query GetUser($id: ID!, $first: ▮ = 10) {
    user(id: $id) {
      id
      name
      email
      posts(first: $first) {
        edges { node { id title publishedAt } }
        pageInfo { hasNextPage endCursor }
      }
    }
  }

```

**Palavra que a pessoa ia digitar:** `Int`: aparece em 13º lugar de 45

**Saída** (as 20 primeiras sugestões):

```text
 1 Boolean      11 include
 2 deprecated   12 input
 3 directive    13 Int
 4 enum         14 interface
 5 extend       15 mutation
 6 false        16 null
 7 Float        17 on
 8 fragment     18 query
 9 ID           19 repeatable
10 implements   20 scalar
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito numa posição de tipo: a lista inteira de palavras-chave em ordem alfabética; `Int` é o 13º (não há regra para tipos).

---

### GraphQL-5
<a id="graphql-5"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query GetUser($id: ID!, $first: Int = 10) {
    u▮(id: $id) {
      id
      name
      email
      posts(first: $first) {
        edges { node { id title publishedAt } }
        pageInfo { hasNextPage endCursor }
      }
    }
  }

```

**Palavra que a pessoa ia digitar:** `user`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 union
```

**Veredito:** ✅ Bom. Nada útil existe: `union` é oferecida para um campo que não aparece em outro lugar.

---

### GraphQL-6
<a id="graphql-6"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query GetUser($id: ID!, $first: Int = 10) {
    user(id: $id) {
      i▮
      name
      email
      posts(first: $first) {
        edges { node { id title publishedAt } }
        pageInfo { hasNextPage endCursor }
      }
    }
  }

```

**Palavra que a pessoa ia digitar:** `id`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 ID
 2 implements
 3 include
 4 input
 5 Int
 6 interface
```

**Veredito:** ⚠️ Razoável, com ressalva. Num conjunto de seleção `i` oferece o tipo `ID`, não o campo `id` (maiúsculas são ignoradas e `id` tem menos de 3 letras).

---

### GraphQL-7
<a id="graphql-7"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query GetUser($id: ID!, $first: Int = 10) {
    user(id: $id) {
      id
      nam▮
      email
      posts(first: $first) {
        edges { node { id title publishedAt } }
        pageInfo { hasNextPage endCursor }
      }
    }
  }

```

**Palavra que a pessoa ia digitar:** `name`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; o campo não aparece em outro lugar.

---

### GraphQL-8
<a id="graphql-8"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query GetUser($id: ID!, $first: Int = 10) {
    user(id: $id) {
      id
      name
      ▮
      posts(first: $first) {
        edges { node { id title publishedAt } }
        pageInfo { hasNextPage endCursor }
      }
    }
  }

```

**Palavra que a pessoa ia digitar:** `email`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 posts [a]    11 implements
 2 Boolean      12 include
 3 deprecated   13 input
 4 directive    14 Int
 5 enum         15 interface
 6 extend       16 mutation
 7 false        17 null
 8 Float        18 on
 9 fragment     19 query
10 ID           20 repeatable
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito num conjunto de seleção: `posts` primeiro, depois a lista de palavras-chave (Boolean, deprecated, directive...), que não pertence ali.

---

### GraphQL-9
<a id="graphql-9"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query GetUser($id: ID!, $first: Int = 10) {
    user(id: $id) {
      id
      name
      email
      p▮(first: $first) {
        edges { node { id title publishedAt } }
        pageInfo { hasNextPage endCursor }
      }
    }
  }

```

**Palavra que a pessoa ia digitar:** `posts`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 publishedAt [a]
 2 pageInfo [a]
```

**Veredito:** ✅ Bom. `publishedAt`, `pageInfo` (campos do arquivo).

---

### GraphQL-10
<a id="graphql-10"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D1, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query GetUser($id: ID!, $first: Int = 10) {
    user(id: $id) {
      id
      name
      email
      posts(fis▮: $first) {
        edges { node { id title publishedAt } }
        pageInfo { hasNextPage endCursor }
      }
    }
  }

```

**Palavra que a pessoa ia digitar:** `first`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 first [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `first`.

---

### GraphQL-11
<a id="graphql-11"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query GetUser($id: ID!, $first: Int = 10) {
    user(id: $id) {
      id
      name
      email
      posts(first: $first) {
        edges { nod▮ { id title publishedAt } }
        pageInfo { hasNextPage endCursor }
      }
    }
  }

```

**Palavra que a pessoa ia digitar:** `node`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; o campo não aparece em outro lugar.

---

### GraphQL-12
<a id="graphql-12"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query GetUser($id: ID!, $first: Int = 10) {
    user(id: $id) {
      id
      name
      email
      posts(first: $first) {
        edges { node { ▮ title publishedAt } }
        pageInfo { hasNextPage endCursor }
      }
    }
  }

```

**Palavra que a pessoa ia digitar:** `id`: aparece em 10º lugar de 45

**Saída** (as 20 primeiras sugestões):

```text
 1 title [a]    11 implements
 2 Boolean      12 include
 3 deprecated   13 input
 4 directive    14 Int
 5 enum         15 interface
 6 extend       16 mutation
 7 false        17 null
 8 Float        18 on
 9 fragment     19 query
10 ID           20 repeatable
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito num conjunto de seleção: `title` primeiro, `ID` é o 10º entre as palavras-chave de tipo.

---

### GraphQL-13
<a id="graphql-13"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query GetUser($id: ID!, $first: Int = 10) {
    user(id: $id) {
      id
      name
      email
      posts(first: $first) {
        edges { node { id t▮ publishedAt } }
        pageInfo { hasNextPage endCursor }
      }
    }
  }

```

**Palavra que a pessoa ia digitar:** `title`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 true
 2 type
```

**Veredito:** ✅ Bom. Só as palavras-chave `true`/`type`; o campo `title` não aparece em outro lugar.

---

### GraphQL-14
<a id="graphql-14"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query GetUser($id: ID!, $first: Int = 10) {
    user(id: $id) {
      id
      name
      email
      posts(first: $first) {
        edges { node { id title pu▮ } }
        pageInfo { hasNextPage endCursor }
      }
    }
  }

```

**Palavra que a pessoa ia digitar:** `publishedAt`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; o campo não aparece em outro lugar.

---

### GraphQL-15
<a id="graphql-15"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query GetUser($id: ID!, $first: Int = 10) {
    user(id: $id) {
      id
      name
      email
      posts(first: $first) {
        edges { node { id title publishedAt } }
        pag▮ { hasNextPage endCursor }
      }
    }
  }

```

**Palavra que a pessoa ia digitar:** `pageInfo`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; o campo não aparece em outro lugar.

---

### GraphQL-16
<a id="graphql-16"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 9 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query GetUser($id: ID!, $first: Int = 10) {
    user(id: $id) {
      id
      name
      email
      posts(first: $first) {
        edges { node { id title publishedAt } }
        pageInfo { hasNextPage ▮ }
      }
    }
  }

```

**Palavra que a pessoa ia digitar:** `endCursor`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 Boolean      11 include
 2 deprecated   12 input
 3 directive    13 Int
 4 enum         14 interface
 5 extend       15 mutation
 6 false        16 null
 7 Float        17 on
 8 fragment     18 query
 9 ID           19 repeatable
10 implements   20 scalar
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito num conjunto de seleção: só a lista de palavras-chave; `endCursor` não aparece em outro lugar.

---

### GraphQL-17
<a id="graphql-17"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  m▮ CreatePost($input: CreatePostInput!) {
    createPost(input: $input) {
      post { id title body author { id name } }
      errors { field message }
    }
  }

```

**Palavra que a pessoa ia digitar:** `mutation`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 mutation
 2 message [a]
```

**Veredito:** ✅ Bom. `mutation` em primeiro.

---

### GraphQL-18
<a id="graphql-18"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  mutation Cr▮($input: CreatePostInput!) {
    createPost(input: $input) {
      post { id title body author { id name } }
      errors { field message }
    }
  }

```

**Palavra que a pessoa ia digitar:** `CreatePost`: aparece em 2º lugar de 4

**Saída** (as 20 primeiras sugestões):

```text
 1 CreatePostInput [a]
 2 createPost [a]
 3 createdAt [a]
 4 CREATE_POST [a]
```

**Veredito:** ✅ Bom. `CreatePost` em 2º, depois do mais longo `CreatePostInput`.

---

### GraphQL-19
<a id="graphql-19"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  mutation CreatePost($input: Cre▮!) {
    createPost(input: $input) {
      post { id title body author { id name } }
      errors { field message }
    }
  }

```

**Palavra que a pessoa ia digitar:** `CreatePostInput`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 createPost [a]
 2 createdAt [a]
 3 CREATE_POST [a]
```

**Veredito:** ✅ Bom. Nomes que começam igual são oferecidos.

---

### GraphQL-20
<a id="graphql-20"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D2, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  mutation CreatePost($input: CreatePostInput!) {
    cerate▮(input: $input) {
      post { id title body author { id name } }
      errors { field message }
    }
  }

```

**Palavra que a pessoa ia digitar:** `createPost`: aparece em 2º lugar de 4

**Saída** (as 20 primeiras sugestões):

```text
 1 CreatePostInput [~]
 2 CreatePost [~]
 3 CREATE_POST [~]
 4 createdAt [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `createPost`.

---

### GraphQL-21
<a id="graphql-21"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  mutation CreatePost($input: CreatePostInput!) {
    createPost(input: $input) {
      p▮ { id title body author { id name } }
      errors { field message }
    }
  }

```

**Palavra que a pessoa ia digitar:** `post`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 postAdded [a]
 2 POST_ADDED [a]
```

**Veredito:** ✅ Bom. Nomes que começam com p são oferecidos.

---

### GraphQL-22
<a id="graphql-22"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  mutation CreatePost($input: CreatePostInput!) {
    createPost(input: $input) {
      post { i▮ title body author { id name } }
      errors { field message }
    }
  }

```

**Palavra que a pessoa ia digitar:** `id`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 ID
 2 implements
 3 include
 4 input
 5 Int
 6 interface
```

**Veredito:** ⚠️ Razoável, com ressalva. Num conjunto de seleção `i` oferece o tipo `ID`, não o campo `id`.

---

### GraphQL-23
<a id="graphql-23"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  mutation CreatePost($input: CreatePostInput!) {
    createPost(input: $input) {
      post { id tit▮ body author { id name } }
      errors { field message }
    }
  }

```

**Palavra que a pessoa ia digitar:** `title`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 title [a]
```

**Veredito:** ✅ Bom. `title` é o único item.

---

### GraphQL-24
<a id="graphql-24"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  mutation CreatePost($input: CreatePostInput!) {
    createPost(input: $input) {
      post { id title ▮ author { id name } }
      errors { field message }
    }
  }

```

**Palavra que a pessoa ia digitar:** `body`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 author [a]      11 ID
 2 createdAt [a]   12 implements
 3 Boolean         13 include
 4 deprecated      14 input
 5 directive       15 Int
 6 enum            16 interface
 7 extend          17 mutation
 8 false           18 null
 9 Float           19 on
10 fragment        20 query
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito num conjunto de seleção: dois campos próximos, depois a lista de palavras-chave.

---

### GraphQL-25
<a id="graphql-25"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  mutation CreatePost($input: CreatePostInput!) {
    createPost(input: $input) {
      post { id title body a▮ { id name } }
      errors { field message }
    }
  }

```

**Palavra que a pessoa ia digitar:** `author`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 authorId [a]
```

**Veredito:** ✅ Bom. `authorId` é o único item.

---

### GraphQL-26
<a id="graphql-26"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  mutation CreatePost($input: CreatePostInput!) {
    createPost(input: $input) {
      post { id title body author { id na▮ } }
      errors { field message }
    }
  }

```

**Palavra que a pessoa ia digitar:** `name`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; o campo não aparece em outro lugar.

---

### GraphQL-27
<a id="graphql-27"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  mutation CreatePost($input: CreatePostInput!) {
    createPost(input: $input) {
      post { id title body author { id name } }
      err▮ { field message }
    }
  }

```

**Palavra que a pessoa ia digitar:** `errors`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; o campo não aparece em outro lugar.

---

### GraphQL-28
<a id="graphql-28"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  mutation CreatePost($input: CreatePostInput!) {
    createPost(input: $input) {
      post { id title body author { id name } }
      errors { ▮ message }
    }
  }

```

**Palavra que a pessoa ia digitar:** `field`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 message [a]   11 implements
 2 Boolean       12 include
 3 deprecated    13 input
 4 directive     14 Int
 5 enum          15 interface
 6 extend        16 mutation
 7 false         17 null
 8 Float         18 on
 9 fragment      19 query
10 ID            20 repeatable
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito: `message` primeiro, depois a lista de palavras-chave.

---

### GraphQL-29
<a id="graphql-29"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  mutation CreatePost($input: CreatePostInput!) {
    createPost(input: $input) {
      post { id title body author { id name } }
      errors { field m▮ }
    }
  }

```

**Palavra que a pessoa ia digitar:** `message`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 mutation
```

**Veredito:** ⚠️ Razoável, com ressalva. `m` oferece `mutation` para o nome de um campo.

---

### GraphQL-30
<a id="graphql-30"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D2, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  suscr▮ OnPostAdded($authorId: ID) {
    postAdded(authorId: $authorId) { id title createdAt }
  }

```

**Palavra que a pessoa ia digitar:** `subscription`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 subscription [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `subscription`.

---

### GraphQL-31
<a id="graphql-31"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  subscription OnPostAdded($authorId: I▮) {
    postAdded(authorId: $authorId) { id title createdAt }
  }

```

**Palavra que a pessoa ia digitar:** `ID`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 ID
 2 implements
 3 include
 4 input
 5 Int
 6 interface
```

**Veredito:** ✅ Bom. `ID` em primeiro (um tipo).

---

### GraphQL-32
<a id="graphql-32"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  subscription OnPostAdded($authorId: ID) {
    ▮(authorId: $authorId) { id title createdAt }
  }

```

**Palavra que a pessoa ia digitar:** `postAdded`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 Boolean      11 include
 2 deprecated   12 input
 3 directive    13 Int
 4 enum         14 interface
 5 extend       15 mutation
 6 false        16 null
 7 Float        17 on
 8 fragment     18 query
 9 ID           19 repeatable
10 implements   20 scalar
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de um campo: só a lista de palavras-chave.

---

### GraphQL-33
<a id="graphql-33"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  subscription OnPostAdded($authorId: ID) {
    postAdded(a▮: $authorId) { id title createdAt }
  }

```

**Palavra que a pessoa ia digitar:** `authorId`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 authorId [a]
 2 author [a]
```

**Veredito:** ✅ Bom. `authorId`, `author`.

---

### GraphQL-34
<a id="graphql-34"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  subscription OnPostAdded($authorId: ID) {
    postAdded(authorId: $authorId) { i▮ title createdAt }
  }

```

**Palavra que a pessoa ia digitar:** `id`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 ID
 2 implements
 3 include
 4 input
 5 Int
 6 interface
```

**Veredito:** ⚠️ Razoável, com ressalva. Num conjunto de seleção `i` oferece o tipo `ID`, não o campo `id`.

---

### GraphQL-35
<a id="graphql-35"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  subscription OnPostAdded($authorId: ID) {
    postAdded(authorId: $authorId) { id tit▮ createdAt }
  }

```

**Palavra que a pessoa ia digitar:** `title`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 title [a]
```

**Veredito:** ✅ Bom. `title` é o único item.

---

### GraphQL-36
<a id="graphql-36"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  ▮ User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `type`: aparece em 27º lugar de 50 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 Boolean      11 include
 2 deprecated   12 input
 3 directive    13 Int
 4 enum         14 interface
 5 extend       15 mutation
 6 false        16 null
 7 Float        17 on
 8 fragment     18 query
 9 ID           19 repeatable
10 implements   20 scalar
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito onde vai `type`: palavras-chave em ordem alfabética, `type` é a 27ª.

---

### GraphQL-37
<a id="graphql-37"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type U▮ implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `User`: aparece em 2º lugar de 3

**Saída** (as 20 primeiras sugestões):

```text
 1 union
 2 user [a]
 3 users [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `type U`: `union` primeiro, depois `user`/`users`; espera-se o nome de um tipo novo.

---

### GraphQL-38
<a id="graphql-38"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User im▮ Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `implements`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 implements
```

**Veredito:** ✅ Bom. `implements` é o único item.

---

### GraphQL-39
<a id="graphql-39"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Nod▮ {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `Node`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 Node [a]
```

**Veredito:** ✅ Bom. `Node` é o único item.

---

### GraphQL-40
<a id="graphql-40"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    ▮: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `id`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 ID           11 include
 2 Boolean      12 input
 3 deprecated   13 Int
 4 directive    14 interface
 5 enum         15 mutation
 6 extend       16 null
 7 false        17 on
 8 Float        18 query
 9 fragment     19 repeatable
10 implements   20 scalar
```

**Veredito:** ✅ Bom. `ID` em primeiro.

---

### GraphQL-41
<a id="graphql-41"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    n▮: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `name`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 null
 2 Node [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `n` para o nome de um campo: `null` e `Node`; palavras-chave não pertencem ali.

---

### GraphQL-42
<a id="graphql-42"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: St▮!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `String`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 String
```

**Veredito:** ✅ Bom. `String` é o único item.

---

### GraphQL-43
<a id="graphql-43"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    ema▮: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `email`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; o campo não aparece em outro lugar.

---

### GraphQL-44
<a id="graphql-44"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: ▮
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `String`: aparece em 25º lugar de 50 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 role [a]     11 implements
 2 Boolean      12 include
 3 deprecated   13 input
 4 directive    14 Int
 5 enum         15 interface
 6 extend       16 mutation
 7 false        17 null
 8 Float        18 on
 9 fragment     19 query
10 ID           20 repeatable
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `email: `: `role` primeiro, `String` é o 25º (tipos não vêm primeiro numa posição de tipo).

---

### GraphQL-45
<a id="graphql-45"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    r▮: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `role`: aparece em 2º lugar de 3

**Saída** (as 20 primeiras sugestões):

```text
 1 repeatable
 2 Role [a]
 3 READER [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `r` para o nome de um campo: `repeatable` primeiro, `Role` em 2º.

---

### GraphQL-46
<a id="graphql-46"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    po▮(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `posts`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 PostConnection [a]
```

**Veredito:** ✅ Bom. `PostConnection` oferecido.

---

### GraphQL-47
<a id="graphql-47"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(fir▮: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `first`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 fragment [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. `fir` (nome de argumento): `fragment~` é ruído.

---

### GraphQL-48
<a id="graphql-48"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: ▮, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `Int`: aparece em 13º lugar de 50

**Saída** (as 20 primeiras sugestões):

```text
 1 Boolean      11 include
 2 deprecated   12 input
 3 directive    13 Int
 4 enum         14 interface
 5 extend       15 mutation
 6 false        16 null
 7 Float        17 on
 8 fragment     18 query
 9 ID           19 repeatable
10 implements   20 scalar
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito numa posição de tipo de argumento: a lista de palavras-chave; `Int` é o 13º.

---

### GraphQL-49
<a id="graphql-49"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, a▮: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `after`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 ADMIN [a]
```

**Veredito:** ✅ Bom. `ADMIN` é o único item (sem relação); o argumento não aparece em outro lugar.

---

### GraphQL-50
<a id="graphql-50"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D3, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: Stin▮): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `String`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 String [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `String`.

---

### GraphQL-51
<a id="graphql-51"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enu▮ Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `enum`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 enum
```

**Veredito:** ✅ Bom. `enum` é o único item.

---

### GraphQL-52
<a id="graphql-52"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum ▮ { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `Role`: aparece em 38º lugar de 50 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 Boolean      11 include
 2 deprecated   12 input
 3 directive    13 Int
 4 enum         14 interface
 5 extend       15 mutation
 6 false        16 null
 7 Float        17 on
 8 fragment     18 query
 9 ID           19 repeatable
10 implements   20 scalar
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito onde vai um nome: a lista de palavras-chave; `Role` é o 38º.

---

### GraphQL-53
<a id="graphql-53"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { A▮ EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `ADMIN`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 after [a]
```

**Veredito:** ✅ Bom. Nada relevante; `after` é oferecida como palavra.

---

### GraphQL-54
<a id="graphql-54"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN ED▮ READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `EDITOR`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; o valor não aparece em outro lugar.

---

### GraphQL-55
<a id="graphql-55"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR REA▮ }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `READER`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 repeatable [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. `REA` (valor de enum): `repeatable~` é ruído.

---

### GraphQL-56
<a id="graphql-56"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input ▮ {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `CreatePostInput`: aparece em 46º lugar de 50 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 Boolean      11 include
 2 deprecated   12 input
 3 directive    13 Int
 4 enum         14 interface
 5 extend       15 mutation
 6 false        16 null
 7 Float        17 on
 8 fragment     18 query
 9 ID           19 repeatable
10 implements   20 scalar
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito onde vai um nome: a lista de palavras-chave; `CreatePostInput` é o 46º.

---

### GraphQL-57
<a id="graphql-57"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 13 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    t▮: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `title`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 true
 2 type
 3 tags [a]
 4 typeDefs [a]
```

**Veredito:** ✅ Bom. Palavras-chave e palavras que começam com t.

---

### GraphQL-58
<a id="graphql-58"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 13 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: St▮!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `String`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 String
```

**Veredito:** ✅ Bom. `String` é o único item.

---

### GraphQL-59
<a id="graphql-59"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 14 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    bod▮: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `body`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 Boolean [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. `bod` (nome de campo): `Boolean~` é ruído.

---

### GraphQL-60
<a id="graphql-60"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D3, linha 14 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: Srtin▮
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `String`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 String [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `String`.

---

### GraphQL-61
<a id="graphql-61"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 15 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [S▮!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `String`: aparece em 5º lugar de 6

**Saída** (as 20 primeiras sugestões):

```text
 1 scalar
 2 schema
 3 skip
 4 specifiedBy
 5 String
 6 subscription
```

**Veredito:** ⚠️ Razoável, com ressalva. `[S`: `scalar`, `schema`, `skip`... vêm antes de `String` (5º).

---

### GraphQL-62
<a id="graphql-62"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 18 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  in▮ Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `interface`: aparece em 4º lugar de 4

**Saída** (as 20 primeiras sugestões):

```text
 1 include
 2 input
 3 Int
 4 interface
```

**Veredito:** ✅ Bom. `input`, `interface` entre as palavras-chave in...

---

### GraphQL-63
<a id="graphql-63"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 18 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Nod▮ { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `Node`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 Node [a]
```

**Veredito:** ✅ Bom. `Node` é o único item.

---

### GraphQL-64
<a id="graphql-64"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 18 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { ▮: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `id`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 ID           11 include
 2 Boolean      12 input
 3 deprecated   13 Int
 4 directive    14 interface
 5 enum         15 mutation
 6 extend       16 null
 7 false        17 on
 8 Float        18 query
 9 fragment     19 repeatable
10 implements   20 scalar
```

**Veredito:** ✅ Bom. `ID` em primeiro.

---

### GraphQL-65
<a id="graphql-65"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 18 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: I▮! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `ID`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 ID
 2 implements
 3 include
 4 input
 5 Int
 6 interface
```

**Veredito:** ✅ Bom. `ID` em primeiro.

---

### GraphQL-66
<a id="graphql-66"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 20 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Qu▮ {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `Query`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 query
```

**Veredito:** ✅ Bom. `query` (o tipo se chama Query): maiúsculas são ignoradas.

---

### GraphQL-67
<a id="graphql-67"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 21 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    use▮(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `user`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 User [a]
 2 users [a]
```

**Veredito:** ✅ Bom. `User`, `users`.

---

### GraphQL-68
<a id="graphql-68"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 21 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(▮: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `id`: aparece em 9º lugar de 50

**Saída** (as 20 primeiras sugestões):

```text
 1 Boolean      11 include
 2 deprecated   12 input
 3 directive    13 Int
 4 enum         14 interface
 5 extend       15 mutation
 6 false        16 null
 7 Float        17 on
 8 fragment     18 query
 9 ID           19 repeatable
10 implements   20 scalar
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de um argumento: lista de palavras-chave, `id` é o 9º (como `ID`).

---

### GraphQL-69
<a id="graphql-69"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 21 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: I▮!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `ID`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 ID
 2 implements
 3 include
 4 input
 5 Int
 6 interface
```

**Veredito:** ✅ Bom. `ID` em primeiro.

---

### GraphQL-70
<a id="graphql-70"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 21 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): Us▮
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `User`: aparece em 2º lugar de 2

**Saída** (as 20 primeiras sugestões):

```text
 1 users [a]
 2 user [a]
```

**Veredito:** ✅ Bom. `users`, `user`.

---

### GraphQL-71
<a id="graphql-71"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 22 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(rol▮: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `role`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 Role [a]
```

**Veredito:** ✅ Bom. `Role` é o único item.

---

### GraphQL-72
<a id="graphql-72"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 22 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: ▮): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `Role`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 Role [a]     11 implements
 2 Boolean      12 include
 3 deprecated   13 input
 4 directive    14 Int
 5 enum         15 interface
 6 extend       16 mutation
 7 false        17 null
 8 Float        18 on
 9 fragment     19 query
10 ID           20 repeatable
```

**Veredito:** ✅ Bom. `Role` em primeiro.

---

### GraphQL-73
<a id="graphql-73"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 22 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [U▮!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `User`: aparece em 3º lugar de 3

**Saída** (as 20 primeiras sugestões):

```text
 1 union
 2 users [a]
 3 User [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `[U`: `union` primeiro, `User` em 3º.

---

### GraphQL-74
<a id="graphql-74"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 25 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  ty▮ Mutation {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `type`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 type
 2 typeDefs [a]
```

**Veredito:** ✅ Bom. `type` em primeiro.

---

### GraphQL-75
<a id="graphql-75"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 25 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mut▮ {
    createPost(input: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `Mutation`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 mutation
```

**Veredito:** ✅ Bom. `mutation` é o único item (o tipo se chama `Mutation`).

---

### GraphQL-76
<a id="graphql-76"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 26 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(▮: CreatePostInput!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `input`: aparece em 12º lugar de 50

**Saída** (as 20 primeiras sugestões):

```text
 1 Boolean      11 include
 2 deprecated   12 input
 3 directive    13 Int
 4 enum         14 interface
 5 extend       15 mutation
 6 false        16 null
 7 Float        17 on
 8 fragment     18 query
 9 ID           19 repeatable
10 implements   20 scalar
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de um argumento: lista de palavras-chave, `input` é o 12º.

---

### GraphQL-77
<a id="graphql-77"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 26 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: C▮!): CreatePostPayload
  }

```

**Palavra que a pessoa ia digitar:** `CreatePostInput`: aparece em 3º lugar de 4

**Saída** (as 20 primeiras sugestões):

```text
 1 CreatePostPayload [a]
 2 createPost [a]
 3 CreatePostInput [a]
 4 const [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `C` numa posição de tipo: `CreatePostPayload` e `createPost` antes de `CreatePostInput` (3º).

---

### GraphQL-78
<a id="graphql-78"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 26 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  type User implements Node {
    id: ID!
    name: String!
    email: String
    role: Role!
    posts(first: Int, after: String): PostConnection!
  }

  enum Role { ADMIN EDITOR READER }

  input CreatePostInput {
    title: String!
    body: String
    tags: [String!]
  }

  interface Node { id: ID! }

  type Query {
    user(id: ID!): User
    users(role: Role): [User!]!
  }

  type Mutation {
    createPost(input: CreatePostInput!): Cr▮
  }

```

**Palavra que a pessoa ia digitar:** `CreatePostPayload`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 CreatePostInput [a]
 2 createPost [a]
```

**Veredito:** ✅ Bom. `CreatePostInput`, `createPost`.

---

### GraphQL-79
<a id="graphql-79"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  fra▮ UserFields on User {
    id
    name
    avatar(size: 64)
    ... on Admin { permissions }
  }

```

**Palavra que a pessoa ia digitar:** `fragment`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 fragment
```

**Veredito:** ✅ Bom. `fragment` é o único item.

---

### GraphQL-80
<a id="graphql-80"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D4, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  fragment UesrFi▮ on User {
    id
    name
    avatar(size: 64)
    ... on Admin { permissions }
  }

```

**Palavra que a pessoa ia digitar:** `UserFields`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 UserFields [~]
 2 USER_FIELDS [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `UserFields`.

---

### GraphQL-81
<a id="graphql-81"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 2 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  fragment UserFields on U▮ {
    id
    name
    avatar(size: 64)
    ... on Admin { permissions }
  }

```

**Palavra que a pessoa ia digitar:** `User`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 union
 2 UserFields [a]
 3 use [a]
 4 USER_FIELDS [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `on U`: `union` primeiro, `User` não aparece em outro lugar; espera-se o nome de um tipo.

---

### GraphQL-82
<a id="graphql-82"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  fragment UserFields on User {
    i▮
    name
    avatar(size: 64)
    ... on Admin { permissions }
  }

```

**Palavra que a pessoa ia digitar:** `id`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 ID
 2 implements
 3 include
 4 input
 5 Int
 6 interface
```

**Veredito:** ⚠️ Razoável, com ressalva. Num conjunto de seleção `i` oferece `ID` em vez de `id`.

---

### GraphQL-83
<a id="graphql-83"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 4 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  fragment UserFields on User {
    id
    nam▮
    avatar(size: 64)
    ... on Admin { permissions }
  }

```

**Palavra que a pessoa ia digitar:** `name`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; o campo não aparece em outro lugar.

---

### GraphQL-84
<a id="graphql-84"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  fragment UserFields on User {
    id
    name
    ▮(size: 64)
    ... on Admin { permissions }
  }

```

**Palavra que a pessoa ia digitar:** `avatar`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 Boolean      11 include
 2 deprecated   12 input
 3 directive    13 Int
 4 enum         14 interface
 5 extend       15 mutation
 6 false        16 null
 7 Float        17 on
 8 fragment     18 query
 9 ID           19 repeatable
10 implements   20 scalar
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito: só a lista de palavras-chave.

---

### GraphQL-85
<a id="graphql-85"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  fragment UserFields on User {
    id
    name
    avatar(s▮: 64)
    ... on Admin { permissions }
  }

```

**Palavra que a pessoa ia digitar:** `size`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 scalar
 2 schema
 3 skip
 4 specifiedBy
 5 String
 6 subscription
 7 Search [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `s` (nome de argumento): as palavras-chave scalar/schema/skip..., `String`; `size` não aparece em outro lugar.

---

### GraphQL-86
<a id="graphql-86"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D4, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  fragment UserFields on User {
    id
    name
    avatar(size: 64)
    ... on Ad▮ { permissions }
  }

```

**Palavra que a pessoa ia digitar:** `Admin`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; o tipo não aparece em outro lugar.

---

### GraphQL-87
<a id="graphql-87"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  fragment UserFields on User {
    id
    name
    avatar(size: 64)
    ... on Admin { per▮ }
  }

```

**Palavra que a pessoa ia digitar:** `permissions`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; o campo não aparece em outro lugar.

---

### GraphQL-88
<a id="graphql-88"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  ▮ Search($term: String!, $withPosts: Boolean = false) {
    search(term: $term) {
      ...UserFields
      posts @include(if: $withPosts) { id title }
      legacyId @deprecated(reason: "use id")
    }
  }

```

**Palavra que a pessoa ia digitar:** `query`: aparece em 18º lugar de 47

**Saída** (as 20 primeiras sugestões):

```text
 1 Boolean      11 include
 2 deprecated   12 input
 3 directive    13 Int
 4 enum         14 interface
 5 extend       15 mutation
 6 false        16 null
 7 Float        17 on
 8 fragment     18 query
 9 ID           19 repeatable
10 implements   20 scalar
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito onde vai uma palavra-chave de operação: alfabético; `query` é a 18ª.

---

### GraphQL-89
<a id="graphql-89"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query S▮($term: String!, $withPosts: Boolean = false) {
    search(term: $term) {
      ...UserFields
      posts @include(if: $withPosts) { id title }
      legacyId @deprecated(reason: "use id")
    }
  }

```

**Palavra que a pessoa ia digitar:** `Search`: aparece em 7º lugar de 8

**Saída** (as 20 primeiras sugestões):

```text
 1 scalar
 2 schema
 3 skip
 4 specifiedBy
 5 String
 6 subscription
 7 search [a]
 8 size [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `S` num nome de operação: palavras-chave primeiro, `search` em 7º.

---

### GraphQL-90
<a id="graphql-90"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D4, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query Search($term: Stin▮!, $withPosts: Boolean = false) {
    search(term: $term) {
      ...UserFields
      posts @include(if: $withPosts) { id title }
      legacyId @deprecated(reason: "use id")
    }
  }

```

**Palavra que a pessoa ia digitar:** `String`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 String [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `String`.

---

### GraphQL-91
<a id="graphql-91"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 11 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query Search($term: String!, $withPosts: Boolean = fal▮) {
    search(term: $term) {
      ...UserFields
      posts @include(if: $withPosts) { id title }
      legacyId @deprecated(reason: "use id")
    }
  }

```

**Palavra que a pessoa ia digitar:** `false`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 false
```

**Veredito:** ✅ Bom. `false` é o único item.

---

### GraphQL-92
<a id="graphql-92"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query Search($term: String!, $withPosts: Boolean = false) {
    ▮(term: $term) {
      ...UserFields
      posts @include(if: $withPosts) { id title }
      legacyId @deprecated(reason: "use id")
    }
  }

```

**Palavra que a pessoa ia digitar:** `search`: aparece em 33º lugar de 47 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 Boolean      11 include
 2 deprecated   12 input
 3 directive    13 Int
 4 enum         14 interface
 5 extend       15 mutation
 6 false        16 null
 7 Float        17 on
 8 fragment     18 query
 9 ID           19 repeatable
10 implements   20 scalar
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito no nome de um campo: só a lista de palavras-chave; `search` é o 33º.

---

### GraphQL-93
<a id="graphql-93"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 12 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query Search($term: String!, $withPosts: Boolean = false) {
    search(t▮: $term) {
      ...UserFields
      posts @include(if: $withPosts) { id title }
      legacyId @deprecated(reason: "use id")
    }
  }

```

**Palavra que a pessoa ia digitar:** `term`: aparece em 3º lugar de 4

**Saída** (as 20 primeiras sugestões):

```text
 1 true
 2 type
 3 term [a]
 4 title [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `t` no nome de um argumento: `true`, `type` primeiro, `term` em 3º.

---

### GraphQL-94
<a id="graphql-94"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D4, linha 13 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query Search($term: String!, $withPosts: Boolean = false) {
    search(term: $term) {
      ...Us▮
      posts @include(if: $withPosts) { id title }
      legacyId @deprecated(reason: "use id")
    }
  }

```

**Palavra que a pessoa ia digitar:** `UserFields`: aparece em 3º lugar de 4

**Saída** (as 20 primeiras sugestões):

```text
 1 use [a]
 2 User [a]
 3 UserFields [a]
 4 USER_FIELDS [a]
```

**Veredito:** ✅ Bom. `UserFields` é o 3º depois de `use` e `User`.

---

### GraphQL-95
<a id="graphql-95"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 14 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query Search($term: String!, $withPosts: Boolean = false) {
    search(term: $term) {
      ...UserFields
      pos▮ @include(if: $withPosts) { id title }
      legacyId @deprecated(reason: "use id")
    }
  }

```

**Palavra que a pessoa ia digitar:** `posts`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; o campo não aparece em outro lugar.

---

### GraphQL-96
<a id="graphql-96"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 14 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query Search($term: String!, $withPosts: Boolean = false) {
    search(term: $term) {
      ...UserFields
      posts @include(if: $withPosts) { ▮ title }
      legacyId @deprecated(reason: "use id")
    }
  }

```

**Palavra que a pessoa ia digitar:** `id`: aparece em 10º lugar de 47

**Saída** (as 20 primeiras sugestões):

```text
 1 title [a]    11 implements
 2 Boolean      12 include
 3 deprecated   13 input
 4 directive    14 Int
 5 enum         15 interface
 6 extend       16 mutation
 7 false        17 null
 8 Float        18 on
 9 fragment     19 query
10 ID           20 repeatable
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito num conjunto de seleção: `title` primeiro, `id` é o 10º como `ID`.

---

### GraphQL-97
<a id="graphql-97"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 14 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query Search($term: String!, $withPosts: Boolean = false) {
    search(term: $term) {
      ...UserFields
      posts @include(if: $withPosts) { id t▮ }
      legacyId @deprecated(reason: "use id")
    }
  }

```

**Palavra que a pessoa ia digitar:** `title`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 true
 2 type
 3 term [a]
```

**Veredito:** ✅ Bom. Palavras-chave e `term`; o campo `title` não aparece em outro lugar.

---

### GraphQL-98
<a id="graphql-98"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D4, linha 15 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query Search($term: String!, $withPosts: Boolean = false) {
    search(term: $term) {
      ...UserFields
      posts @include(if: $withPosts) { id title }
      le▮ @deprecated(reason: "use id")
    }
  }

```

**Palavra que a pessoa ia digitar:** `legacyId`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; o campo não aparece em outro lugar.

---

### GraphQL-99
<a id="graphql-99"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 15 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query Search($term: String!, $withPosts: Boolean = false) {
    search(term: $term) {
      ...UserFields
      posts @include(if: $withPosts) { id title }
      legacyId @deprecated(rea▮: "use id")
    }
  }

```

**Palavra que a pessoa ia digitar:** `reason`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 repeatable [~]
```

**Veredito:** ⚠️ Razoável, com ressalva. `rea` (nome de argumento): `repeatable~` é ruído.

---

### GraphQL-100
<a id="graphql-100"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 15 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  query Search($term: String!, $withPosts: Boolean = false) {
    search(term: $term) {
      ...UserFields
      posts @include(if: $withPosts) { id title }
      legacyId @deprecated(reason: "▮ id")
    }
  }

```

**Palavra que a pessoa ia digitar:** `use`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 ID           11 include
 2 Boolean      12 input
 3 deprecated   13 Int
 4 directive    14 interface
 5 enum         15 mutation
 6 extend       16 null
 7 false        17 on
 8 Float        18 query
 9 fragment     19 repeatable
10 implements   20 scalar
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito dentro de uma string: a lista de palavras-chave é oferecida onde nada deveria ser.

---
