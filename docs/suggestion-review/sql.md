# SQL: 100 exemplos

Resultado: ✅ 69 bons · ⚠️ 26 razoáveis com ressalva · ❌ 5 ruins.

Como ler: em cada exemplo, `▮` marca onde está o cursor. A lista é o que o plugin mostraria (as 20 primeiras). `[a]` = palavra que já existe no arquivo; `[~]` = sugestão "parecida" (corrige erro de digitação); sem marca = palavra-chave da linguagem. O veredito e o comentário são a minha análise. "Lugar na gramática" é o nome interno da regra de posição que o plugin aplicou (`sql:table`, `css:value:display`...); `(no rule)` quer dizer que o plugin não tem regra para aquele lugar e usa só o que foi digitado.

## Índice (para varrer rápido)

| # | Situação | Digitado | Palavra procurada | Posição | Lugar na gramática | Veredito |
|---|---|---|---|---|---|---|
| [1](#sql-1) | 1 letra | `S` | `SELECT` | 1 | `sql:statement` | ✅ |
| [2](#sql-2) | 2 letras | `to` | `total` | 1 | `sql:member` | ✅ |
| [3](#sql-3) | 3 letras | `cus` | `customer_name` | — | `(no rule)` | ⚠️ |
| [4](#sql-4) | Ctrl+Espaço | (nada) | `JOIN` | 3 | `sql:continue-from` | ⚠️ |
| [5](#sql-5) | 1 letra | `i` | `id` | fora | `sql:member` | ❌ |
| [6](#sql-6) | 2 letras | `cu` | `customer_id` | 1 | `sql:member` | ✅ |
| [7](#sql-7) | 3 letras | `ORD` | `ORDER` | 1 | `sql:continue-where` | ✅ |
| [8](#sql-8) | Ctrl+Espaço | (nada) | `DESC` | 3 | `sql:continue-order` | ✅ |
| [9](#sql-9) | 1 letra | `d` | `date_trunc` | — | `sql:after-select` | ⚠️ |
| [10](#sql-10) | 1 letra | `A` | `AS` | 1 | `sql:continue-select` | ✅ |
| [11](#sql-11) | 1 letra | `A` | `AS` | 1 | `sql:continue-select` | ✅ |
| [12](#sql-12) | Ctrl+Espaço | (nada) | `total` | 1 | `sql:expression` | ⚠️ |
| [13](#sql-13) | 1 letra | `o` | `orders` | 1 | `sql:table` | ✅ |
| [14](#sql-14) | 2 letras | `ye` | `year` | 1 | `sql:expression` | ✅ |
| [15](#sql-15) | 3 letras | `GRO` | `GROUP` | 1 | `sql:continue-from` | ✅ |
| [16](#sql-16) | Ctrl+Espaço | (nada) | `HAVING` | 63 | `sql:continue-group` | ❌ |
| [17](#sql-17) | 1 letra | `O` | `ORDER` | 2 | `sql:continue-having` | ✅ |
| [18](#sql-18) | 2 letras | `IN` | `INTO` | 1 | `sql:after-insert` | ✅ |
| [19](#sql-19) | 3 letras | `dis` | `display_name` | 1 | `sql:expression` | ✅ |
| [20](#sql-20) | erro: trocadas | `VLAUE` | `VALUES` | 1 | `sql:continue-into` | ✅ |
| [21](#sql-21) | 1 letra | `C` | `CONFLICT` | 11 | `sql:expression` | ⚠️ |
| [22](#sql-22) | 2 letras | `NO` | `NOTHING` | 2 | `(no rule)` | ✅ |
| [23](#sql-23) | 2 letras | `SE` | `SET` | 1 | `sql:continue-update` | ✅ |
| [24](#sql-24) | Ctrl+Espaço | (nada) | `WHERE` | 2 | `sql:continue-set` | ⚠️ |
| [25](#sql-25) | 1 letra | `F` | `FROM` | 1 | `sql:after-delete` | ✅ |
| [26](#sql-26) | 2 letras | `la` | `last_seen` | — | `sql:expression` | ✅ |
| [27](#sql-27) | 3 letras | `day` | `days` | 1 | `sql:continue-where` | ✅ |
| [28](#sql-28) | Ctrl+Espaço | (nada) | `IN` | 7 | `sql:continue-where` | ⚠️ |
| [29](#sql-29) | 1 letra | `u` | `users` | 3 | `sql:table` | ⚠️ |
| [30](#sql-30) | erro: faltando | `fas` | `false` | 1 | `sql:expression` | ✅ |
| [31](#sql-31) | 3 letras | `inv` | `invoices` | 1 | `sql:table` | ✅ |
| [32](#sql-32) | Ctrl+Espaço | (nada) | `PRIMARY` | fora | `(no rule)` | ❌ |
| [33](#sql-33) | 1 letra | `I` | `INTEGER` | 8 | `(no rule)` | ⚠️ |
| [34](#sql-34) | 2 letras | `RE` | `REFERENCES` | 3 | `(no rule)` | ✅ |
| [35](#sql-35) | 3 letras | `VAR` | `VARCHAR` | 1 | `(no rule)` | ✅ |
| [36](#sql-36) | Ctrl+Espaço | (nada) | `UNIQUE` | fora | `(no rule)` | ⚠️ |
| [37](#sql-37) | 1 letra | `N` | `NOT` | 3 | `(no rule)` | ✅ |
| [38](#sql-38) | 2 letras | `is` | `issued_at` | 1 | `sql:expression` | ✅ |
| [39](#sql-39) | 3 letras | `NUL` | `NULL` | 1 | `sql:after-not` | ✅ |
| [40](#sql-40) | erro: trocadas | `BOOLEA` | `BOOLEAN` | 1 | `(no rule)` | ✅ |
| [41](#sql-41) | 1 letra | `D` | `DEFAULT` | 1 | `(no rule)` | ✅ |
| [42](#sql-42) | 2 letras | `IN` | `INDEX` | 1 | `sql:after-create` | ✅ |
| [43](#sql-43) | 3 letras | `inv` | `invoices` | 1 | `sql:expression` | ✅ |
| [44](#sql-44) | Ctrl+Espaço | (nada) | `ALTER` | 7 | `sql:statement` | ✅ |
| [45](#sql-45) | 1 letra | `A` | `ADD` | 1 | `(no rule)` | ✅ |
| [46](#sql-46) | 2 letras | `DR` | `DROP` | 1 | `sql:statement` | ✅ |
| [47](#sql-47) | 3 letras | `ALT` | `ALTER` | 1 | `sql:statement` | ✅ |
| [48](#sql-48) | Ctrl+Espaço | (nada) | `DROP` | 40 | `(no rule)` | ⚠️ |
| [49](#sql-49) | 1 letra | `D` | `DROP` | 2 | `sql:statement` | ✅ |
| [50](#sql-50) | 2 letras | `WI` | `WITH` | 1 | `sql:statement` | ✅ |
| [51](#sql-51) | 3 letras | `cus` | `customer_id` | 1 | `sql:after-select` | ✅ |
| [52](#sql-52) | Ctrl+Espaço | (nada) | `issued_at` | — | `sql:expression` | ⚠️ |
| [53](#sql-53) | 1 letra | `S` | `SUM` | — | `sql:expression` | ⚠️ |
| [54](#sql-54) | 2 letras | `to` | `total` | 2 | `(no rule)` | ✅ |
| [55](#sql-55) | 3 letras | `WHE` | `WHERE` | 1 | `sql:continue-from` | ✅ |
| [56](#sql-56) | Ctrl+Espaço | (nada) | `GROUP` | 58 | `(no rule)` | ⚠️ |
| [57](#sql-57) | 1 letra | `r` | `ranked` | 2 | `sql:expression` | ✅ |
| [58](#sql-58) | 2 letras | `cu` | `customer_id` | 1 | `sql:after-select` | ✅ |
| [59](#sql-59) | 3 letras | `RAN` | `RANK` | — | `sql:expression` | ⚠️ |
| [60](#sql-60) | Ctrl+Espaço | (nada) | `BY` | 2 | `sql:after-partition` | ✅ |
| [61](#sql-61) | 1 letra | `B` | `BY` | 1 | `sql:after-order` | ✅ |
| [62](#sql-62) | 1 letra | `A` | `AS` | 8 | `sql:continue-order` | ⚠️ |
| [63](#sql-63) | 3 letras | `OVE` | `OVER` | 1 | `sql:continue-order` | ✅ |
| [64](#sql-64) | Ctrl+Espaço | (nada) | `customer_id` | 4 | `sql:expression` | ✅ |
| [65](#sql-65) | 1 letra | `m` | `month` | 1 | `sql:expression` | ✅ |
| [66](#sql-66) | 2 letras | `FR` | `FROM` | 1 | `sql:continue-order` | ✅ |
| [67](#sql-67) | 3 letras | `cus` | `customer_id` | 1 | `sql:member` | ✅ |
| [68](#sql-68) | Ctrl+Espaço | (nada) | `WHEN` | 10 | `sql:expression` | ⚠️ |
| [69](#sql-69) | 1 letra | `N` | `NULL` | 2 | `sql:after-is` | ✅ |
| [70](#sql-70) | 2 letras | `WH` | `WHEN` | 2 | `sql:continue-select` | ⚠️ |
| [71](#sql-71) | 3 letras | `THE` | `THEN` | 1 | `sql:continue-select` | ✅ |
| [72](#sql-72) | Ctrl+Espaço | (nada) | `shrinking` | — | `sql:continue-select` | ❌ |
| [73](#sql-73) | 1 letra | `t` | `trend` | — | `(no rule)` | ⚠️ |
| [74](#sql-74) | 2 letras | `po` | `position` | 1 | `sql:member` | ✅ |
| [75](#sql-75) | 3 letras | `mon` | `month` | 1 | `sql:member` | ✅ |
| [76](#sql-76) | Ctrl+Espaço | (nada) | `number` | 1 | `sql:after-select` | ✅ |
| [77](#sql-77) | 1 letra | `i` | `invoices` | 1 | `sql:table` | ✅ |
| [78](#sql-78) | 2 letras | `fa` | `false` | 1 | `sql:expression` | ✅ |
| [79](#sql-79) | 3 letras | `num` | `number` | 1 | `sql:after-select` | ✅ |
| [80](#sql-80) | erro: trocadas | `cerdit` | `credit_notes` | — | `sql:table` | ✅ |
| [81](#sql-81) | 1 letra | `f` | `false` | 2 | `sql:expression` | ⚠️ |
| [82](#sql-82) | 2 letras | `ti` | `title` | 1 | `sql:member` | ✅ |
| [83](#sql-83) | 3 letras | `qua` | `quantity` | 1 | `sql:member` | ✅ |
| [84](#sql-84) | Ctrl+Espaço | (nada) | `from` | 3 | `sql:continue-select` | ⚠️ |
| [85](#sql-85) | 1 letra | `s` | `stock` | 4 | `sql:table` | ⚠️ |
| [86](#sql-86) | 1 letra | `i` | `id` | fora | `sql:member` | ❌ |
| [87](#sql-87) | 3 letras | `fal` | `false` | 1 | `sql:expression` | ✅ |
| [88](#sql-88) | Ctrl+Espaço | (nada) | `sku` | 4 | `sql:member` | ⚠️ |
| [89](#sql-89) | 1 letra | `c` | `coalesce` | 1 | `sql:expression` | ✅ |
| [90](#sql-90) | 1 letra | `b` | `by` | 1 | `sql:after-order` | ✅ |
| [91](#sql-91) | 3 letras | `tit` | `title` | 1 | `sql:member` | ✅ |
| [92](#sql-92) | Ctrl+Espaço | (nada) | `title` | 10 | `sql:after-select` | ⚠️ |
| [93](#sql-93) | 1 letra | `w` | `where` | 1 | `sql:continue-from` | ✅ |
| [94](#sql-94) | 2 letras | `fr` | `from` | 1 | `sql:continue-select` | ✅ |
| [95](#sql-95) | 3 letras | `sto` | `stock` | 1 | `sql:expression` | ✅ |
| [96](#sql-96) | Ctrl+Espaço | (nada) | `and` | 3 | `sql:continue-where` | ⚠️ |
| [97](#sql-97) | 1 letra | `p` | `products` | 1 | `sql:table` | ✅ |
| [98](#sql-98) | 2 letras | `no` | `now` | — | `sql:expression` | ⚠️ |
| [99](#sql-99) | 3 letras | `del` | `delete` | 1 | `sql:statement` | ✅ |
| [100](#sql-100) | erro: trocadas | `wehr` | `where` | 1 | `sql:continue-from` | ✅ |

Posição: lugar da palavra procurada na lista; `—` = a palavra não existe em outro lugar do arquivo; `fora` = existe mas não está na lista.

## Os arquivos usados como entrada

Escritos à mão como um desenvolvedor escreveria (código JavaScript com strings da linguagem). Nada foi gerado pelo gerador dos experimentos.

### Documento D1

```js
import { db } from './db';

const MIN_TOTAL = 100;

export async function recentOrders(customerId, limit) {
  const rows = await db.query(sql`
    SELECT o.id, o.created_at, o.total, c.name AS customer_name
    FROM orders o
    JOIN customers c ON c.id = o.customer_id
    WHERE o.customer_id = ${customerId}
      AND o.status <> 'cancelled'
    ORDER BY o.created_at DESC
    LIMIT ${limit}
  `);
  return rows;
}

export async function revenueByMonth(year) {
  return db.query(sql`
    SELECT date_trunc('month', created_at) AS month,
           COUNT(*) AS orders,
           SUM(total) AS revenue
    FROM orders
    WHERE EXTRACT(year FROM created_at) = ${year}
    GROUP BY month
    HAVING SUM(total) > ${MIN_TOTAL}
    ORDER BY month
  `);
}
```

### Documento D2

```js
export async function createUser(db, user) {
  await db.run(sql`
    INSERT INTO users (email, display_name, password_hash, created_at)
    VALUES (${user.email}, ${user.name}, ${user.hash}, now())
    ON CONFLICT (email) DO NOTHING
  `);
}

export async function renameUser(db, id, name) {
  await db.run(sql`
    UPDATE users
    SET display_name = ${name}, updated_at = now()
    WHERE id = ${id}
  `);
}

export async function removeInactive(db, days) {
  return db.run(sql`
    DELETE FROM sessions
    WHERE last_seen < now() - interval '${days} days'
      AND user_id IN (SELECT id FROM users WHERE active = false)
  `);
}
```

### Documento D3

```js
// migrations/003_billing.js
export const up = [
  sql`
    CREATE TABLE invoices (
      id SERIAL PRIMARY KEY,
      customer_id INTEGER NOT NULL REFERENCES customers (id),
      number VARCHAR(32) NOT NULL UNIQUE,
      amount NUMERIC(12, 2) NOT NULL DEFAULT 0,
      issued_at TIMESTAMP NOT NULL DEFAULT now(),
      paid BOOLEAN NOT NULL DEFAULT false
    )
  `,
  sql`CREATE INDEX idx_invoices_customer ON invoices (customer_id, issued_at)`,
  sql`ALTER TABLE customers ADD COLUMN billing_email VARCHAR(255)`,
];

export const down = [
  sql`DROP INDEX idx_invoices_customer`,
  sql`ALTER TABLE customers DROP COLUMN billing_email`,
  sql`DROP TABLE invoices`,
];
```

### Documento D4

```js
const report = sql`
  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position
`;

const unpaid = sql`
  SELECT number, amount FROM invoices WHERE paid = false
  UNION ALL
  SELECT number, amount FROM credit_notes WHERE applied = false
`;
```

### Documento D5

```js
const stock = sql`
  select p.sku, p.title, coalesce(sum(s.quantity), 0) as on_hand
  from products p
  left join stock s on s.product_id = p.id
  where p.discontinued = false
  group by p.sku, p.title
  having coalesce(sum(s.quantity), 0) < ${threshold}
  order by on_hand asc, p.title
  limit 50
`;

const lowStockNames = sql`
  select title from products
  where exists (select 1 from stock where stock.product_id = products.id and quantity = 0)
`;

const touch = sql`update products set updated_at = now() where sku = ${sku}`;
const purge = sql`delete from stock where quantity = 0 and updated_at < ${cutoff}`;
```

## Os exemplos

### SQL-1
<a id="sql-1"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 7 · **lugar na gramática:** `sql:statement`

**Entrada** (a string onde está o cursor):

```text

    S▮ o.id, o.created_at, o.total, c.name AS customer_name
    FROM orders o
    JOIN customers c ON c.id = o.customer_id
    WHERE o.customer_id = ${customerId}
      AND o.status <> 'cancelled'
    ORDER BY o.created_at DESC
    LIMIT ${limit}
  
```

**Palavra que a pessoa ia digitar:** `SELECT`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 SELECT       11 SUM [a]
 2 SAVEPOINT    12 sql [a]
 3 SCHEMA
 4 SERIAL
 5 SET
 6 SHARE
 7 SIMILAR
 8 SMALLINT
 9 SOME
10 status [a]
```

**Veredito:** ✅ Bom. SELECT em primeiro.

---

### SQL-2
<a id="sql-2"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 7 · **lugar na gramática:** `sql:member`

**Entrada** (a string onde está o cursor):

```text

    SELECT o.id, o.created_at, o.to▮, c.name AS customer_name
    FROM orders o
    JOIN customers c ON c.id = o.customer_id
    WHERE o.customer_id = ${customerId}
      AND o.status <> 'cancelled'
    ORDER BY o.created_at DESC
    LIMIT ${limit}
  
```

**Palavra que a pessoa ia digitar:** `total`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 total [a]
 2 top
```

**Veredito:** ✅ Bom. `total` em primeiro; `top` depois é uma palavra-chave perdida.

---

### SQL-3
<a id="sql-3"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

    SELECT o.id, o.created_at, o.total, c.name AS cus▮
    FROM orders o
    JOIN customers c ON c.id = o.customer_id
    WHERE o.customer_id = ${customerId}
      AND o.status <> 'cancelled'
    ORDER BY o.created_at DESC
    LIMIT ${limit}
  
```

**Palavra que a pessoa ia digitar:** `customer_name`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 customers [a]
 2 customer_id [a]
 3 customerId [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Depois de `AS` espera-se um apelido novo; só aparecem palavras que já existem. Inofensivo, e o apelido não existe em outro lugar do arquivo.

---

### SQL-4
<a id="sql-4"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 9 · **lugar na gramática:** `sql:continue-from`

**Entrada** (a string onde está o cursor):

```text

    SELECT o.id, o.created_at, o.total, c.name AS customer_name
    FROM orders o
    ▮ customers c ON c.id = o.customer_id
    WHERE o.customer_id = ${customerId}
      AND o.status <> 'cancelled'
    ORDER BY o.created_at DESC
    LIMIT ${limit}
  
```

**Palavra que a pessoa ia digitar:** `JOIN`: aparece em 3º lugar de 100

**Saída** (as 20 primeiras sugestões):

```text
 1 customers [a]   11 group
 2 where           12 order
 3 join            13 limit
 4 inner           14 union
 5 left            15 having
 6 right           16 add
 7 full            17 all
 8 cross           18 alter
 9 on              19 analyze
10 as              20 and
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de um apelido de tabela: JOIN é o 3º, mas `customers` (a palavra que já está depois do cursor) vem em primeiro.

---

### SQL-5
<a id="sql-5"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 9 · **lugar na gramática:** `sql:member`

**Entrada** (a string onde está o cursor):

```text

    SELECT o.id, o.created_at, o.total, c.name AS customer_name
    FROM orders o
    JOIN customers c ON c.i▮ = o.customer_id
    WHERE o.customer_id = ${customerId}
      AND o.status <> 'cancelled'
    ORDER BY o.created_at DESC
    LIMIT ${limit}
  
```

**Palavra que a pessoa ia digitar:** `id`: **não aparece** na lista (13 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 import [a]   11 interval
 2 if           12 into
 3 ilike        13 is
 4 in
 5 index
 6 inner
 7 insert
 8 int
 9 integer
10 intersect
```

**Veredito:** ❌ Ruim. `c.i`: `id` nunca pode ser sugerido (palavras com menos de 3 letras são ignoradas) e `import` do código hospedeiro aparece no lugar.

---

### SQL-6
<a id="sql-6"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 10 · **lugar na gramática:** `sql:member`

**Entrada** (a string onde está o cursor):

```text

    SELECT o.id, o.created_at, o.total, c.name AS customer_name
    FROM orders o
    JOIN customers c ON c.id = o.customer_id
    WHERE o.cu▮ = ${customerId}
      AND o.status <> 'cancelled'
    ORDER BY o.created_at DESC
    LIMIT ${limit}
  
```

**Palavra que a pessoa ia digitar:** `customer_id`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 customer_id [a]
 2 customers [a]
 3 customer_name [a]
 4 customerId [a]
```

**Veredito:** ✅ Bom. `customer_id` em primeiro. `customers` e `customerId` não são colunas de `o`, mas vêm depois.

---

### SQL-7
<a id="sql-7"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 12 · **lugar na gramática:** `sql:continue-where`

**Entrada** (a string onde está o cursor):

```text

    SELECT o.id, o.created_at, o.total, c.name AS customer_name
    FROM orders o
    JOIN customers c ON c.id = o.customer_id
    WHERE o.customer_id = ${customerId}
      AND o.status <> 'cancelled'
    ORD▮ BY o.created_at DESC
    LIMIT ${limit}
  
```

**Palavra que a pessoa ia digitar:** `ORDER`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 ORDER
 2 orders [a]
```

**Veredito:** ✅ Bom. ORDER em primeiro.

---

### SQL-8
<a id="sql-8"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 12 · **lugar na gramática:** `sql:continue-order`

**Entrada** (a string onde está o cursor):

```text

    SELECT o.id, o.created_at, o.total, c.name AS customer_name
    FROM orders o
    JOIN customers c ON c.id = o.customer_id
    WHERE o.customer_id = ${customerId}
      AND o.status <> 'cancelled'
    ORDER BY o.created_at ▮
    LIMIT ${limit}
  
```

**Palavra que a pessoa ia digitar:** `DESC`: aparece em 3º lugar de 100

**Saída** (as 20 primeiras sugestões):

```text
 1 limit     11 any
 2 asc       12 as
 3 desc      13 begin
 4 offset    14 between
 5 nulls     15 bigint
 6 add       16 bigserial
 7 all       17 blob
 8 alter     18 bool
 9 analyze   19 boolean
10 and       20 by
```

**Veredito:** ✅ Bom. Depois da coluna o motor oferece asc/desc/limit; DESC é o 3º.

---

### SQL-9
<a id="sql-9"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 20 · **lugar na gramática:** `sql:after-select`

**Entrada** (a string onde está o cursor):

```text

    SELECT d▮('month', created_at) AS month,
           COUNT(*) AS orders,
           SUM(total) AS revenue
    FROM orders
    WHERE EXTRACT(year FROM created_at) = ${year}
    GROUP BY month
    HAVING SUM(total) > ${MIN_TOTAL}
    ORDER BY month
  
```

**Palavra que a pessoa ia digitar:** `date_trunc`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 distinct   11 drop
 2 DESC [a]
 3 database
 4 date
 5 decimal
 6 declare
 7 default
 8 delete
 9 do
10 double
```

**Veredito:** ⚠️ Razoável, com ressalva. `d` na lista do SELECT: `distinct` primeiro, `date` depois. A função `date_trunc` não está no vocabulário (faltam nomes de função).

---

### SQL-10
<a id="sql-10"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 20 · **lugar na gramática:** `sql:continue-select`

**Entrada** (a string onde está o cursor):

```text

    SELECT date_trunc('month', created_at) A▮ month,
           COUNT(*) AS orders,
           SUM(total) AS revenue
    FROM orders
    WHERE EXTRACT(year FROM created_at) = ${year}
    GROUP BY month
    HAVING SUM(total) > ${MIN_TOTAL}
    ORDER BY month
  
```

**Palavra que a pessoa ia digitar:** `AS`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 AS
 2 ADD
 3 ALL
 4 ALTER
 5 ANALYZE
 6 AND
 7 ANY
 8 ASC
 9 async [a]
10 await [a]
```

**Veredito:** ✅ Bom. AS em primeiro.

---

### SQL-11
<a id="sql-11"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 21 · **lugar na gramática:** `sql:continue-select`

**Entrada** (a string onde está o cursor):

```text

    SELECT date_trunc('month', created_at) AS month,
           COUNT(*) A▮ orders,
           SUM(total) AS revenue
    FROM orders
    WHERE EXTRACT(year FROM created_at) = ${year}
    GROUP BY month
    HAVING SUM(total) > ${MIN_TOTAL}
    ORDER BY month
  
```

**Palavra que a pessoa ia digitar:** `AS`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 AS
 2 ADD
 3 ALL
 4 ALTER
 5 ANALYZE
 6 AND
 7 ANY
 8 ASC
 9 async [a]
10 await [a]
```

**Veredito:** ✅ Bom. AS em primeiro.

---

### SQL-12
<a id="sql-12"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 22 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text

    SELECT date_trunc('month', created_at) AS month,
           COUNT(*) AS orders,
           SUM(▮) AS revenue
    FROM orders
    WHERE EXTRACT(year FROM created_at) = ${year}
    GROUP BY month
    HAVING SUM(total) > ${MIN_TOTAL}
    ORDER BY month
  
```

**Palavra que a pessoa ia digitar:** `total`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 total [a]     11 created_at [a]
 2 SUM [a]       12 date_trunc [a]
 3 revenue [a]   13 GROUP [a]
 4 FROM [a]      14 SELECT [a]
 5 orders [a]    15 HAVING [a]
 6 COUNT [a]     16 ORDER [a]
 7 WHERE [a]     17 LIMIT [a]
 8 EXTRACT [a]   18 DESC [a]
 9 year [a]      19 cancelled [a]
10 month [a]     20 status [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Dentro de `SUM(`: `total` em primeiro é bom, mas depois palavras-chave de cláusula como FROM e WHERE aparecem como "palavras" na frente das outras colunas.

---

### SQL-13
<a id="sql-13"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 23 · **lugar na gramática:** `sql:table`

**Entrada** (a string onde está o cursor):

```text

    SELECT date_trunc('month', created_at) AS month,
           COUNT(*) AS orders,
           SUM(total) AS revenue
    FROM o▮
    WHERE EXTRACT(year FROM created_at) = ${year}
    GROUP BY month
    HAVING SUM(total) > ${MIN_TOTAL}
    ORDER BY month
  
```

**Palavra que a pessoa ia digitar:** `orders`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 orders [a]
 2 ORDER [a]
 3 offset
 4 on
 5 only
 6 or
 7 outer
 8 over
```

**Veredito:** ✅ Bom. `orders` em primeiro.

---

### SQL-14
<a id="sql-14"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D1, linha 24 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text

    SELECT date_trunc('month', created_at) AS month,
           COUNT(*) AS orders,
           SUM(total) AS revenue
    FROM orders
    WHERE EXTRACT(ye▮ FROM created_at) = ${year}
    GROUP BY month
    HAVING SUM(total) > ${MIN_TOTAL}
    ORDER BY month
  
```

**Palavra que a pessoa ia digitar:** `year`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 year [a]
```

**Veredito:** ✅ Bom. `year` é o único item.

---

### SQL-15
<a id="sql-15"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D1, linha 25 · **lugar na gramática:** `sql:continue-from`

**Entrada** (a string onde está o cursor):

```text

    SELECT date_trunc('month', created_at) AS month,
           COUNT(*) AS orders,
           SUM(total) AS revenue
    FROM orders
    WHERE EXTRACT(year FROM created_at) = ${year}
    GRO▮ BY month
    HAVING SUM(total) > ${MIN_TOTAL}
    ORDER BY month
  
```

**Palavra que a pessoa ia digitar:** `GROUP`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 GROUP
```

**Veredito:** ✅ Bom. GROUP é o único item.

---

### SQL-16
<a id="sql-16"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D1, linha 26 · **lugar na gramática:** `sql:continue-group`

**Entrada** (a string onde está o cursor):

```text

    SELECT date_trunc('month', created_at) AS month,
           COUNT(*) AS orders,
           SUM(total) AS revenue
    FROM orders
    WHERE EXTRACT(year FROM created_at) = ${year}
    GROUP BY month
    ▮ SUM(total) > ${MIN_TOTAL}
    ORDER BY month
  
```

**Palavra que a pessoa ia digitar:** `HAVING`: aparece em 63º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 SUM [a]   11 and
 2 asc       12 any
 3 desc      13 as
 4 limit     14 begin
 5 offset    15 between
 6 nulls     16 bigint
 7 add       17 bigserial
 8 all       18 blob
 9 alter     19 bool
10 analyze   20 boolean
```

**Veredito:** ❌ Ruim. Depois de `GROUP BY month` a regra oferece ASC/DESC/LIMIT (a regra de GROUP está errada): HAVING é o 63º.

---

### SQL-17
<a id="sql-17"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D1, linha 27 · **lugar na gramática:** `sql:continue-having`

**Entrada** (a string onde está o cursor):

```text

    SELECT date_trunc('month', created_at) AS month,
           COUNT(*) AS orders,
           SUM(total) AS revenue
    FROM orders
    WHERE EXTRACT(year FROM created_at) = ${year}
    GROUP BY month
    HAVING SUM(total) > ${MIN_TOTAL}
    O▮ BY month
  
```

**Palavra que a pessoa ia digitar:** `ORDER`: aparece em 2º lugar de 8

**Saída** (as 20 primeiras sugestões):

```text
 1 OR
 2 ORDER
 3 OFFSET
 4 ON
 5 ONLY
 6 OUTER
 7 OVER
 8 orders [a]
```

**Veredito:** ✅ Bom. OR e ORDER são os dois primeiros.

---

### SQL-18
<a id="sql-18"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 3 · **lugar na gramática:** `sql:after-insert`

**Entrada** (a string onde está o cursor):

```text

    INSERT IN▮ users (email, display_name, password_hash, created_at)
    VALUES (${user.email}, ${user.name}, ${user.hash}, now())
    ON CONFLICT (email) DO NOTHING
  
```

**Palavra que a pessoa ia digitar:** `INTO`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 INTO
 2 INDEX
 3 INNER
 4 INSERT
 5 INT
 6 INTEGER
 7 INTERSECT
 8 INTERVAL
```

**Veredito:** ✅ Bom. INTO em primeiro.

---

### SQL-19
<a id="sql-19"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 3 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text

    INSERT INTO users (email, dis▮, password_hash, created_at)
    VALUES (${user.email}, ${user.name}, ${user.hash}, now())
    ON CONFLICT (email) DO NOTHING
  
```

**Palavra que a pessoa ia digitar:** `display_name`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 display_name [a]
 2 distinct
```

**Veredito:** ✅ Bom. `display_name` em primeiro.

---

### SQL-20
<a id="sql-20"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D2, linha 4 · **lugar na gramática:** `sql:continue-into`

**Entrada** (a string onde está o cursor):

```text

    INSERT INTO users (email, display_name, password_hash, created_at)
    VLAUE▮ (${user.email}, ${user.name}, ${user.hash}, now())
    ON CONFLICT (email) DO NOTHING
  
```

**Palavra que a pessoa ia digitar:** `VALUES`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 VALUES [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: VALUES.

---

### SQL-21
<a id="sql-21"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 5 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text

    INSERT INTO users (email, display_name, password_hash, created_at)
    VALUES (${user.email}, ${user.name}, ${user.hash}, now())
    ON C▮ (email) DO NOTHING
  
```

**Palavra que a pessoa ia digitar:** `CONFLICT`: aparece em 11º lugar de 14

**Saída** (as 20 primeiras sugestões):

```text
 1 created_at [a]   11 CONFLICT
 2 createUser [a]   12 CONSTRAINT
 3 CALL             13 CREATE
 4 CASCADE          14 CROSS
 5 CASE
 6 CAST
 7 CHAR
 8 CHECK
 9 COLUMN
10 COMMIT
```

**Veredito:** ⚠️ Razoável, com ressalva. `ON C` é lido como condição de join: `created_at` e `createUser` primeiro, CONFLICT é o 11º.

---

### SQL-22
<a id="sql-22"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

    INSERT INTO users (email, display_name, password_hash, created_at)
    VALUES (${user.email}, ${user.name}, ${user.hash}, now())
    ON CONFLICT (email) DO NO▮
  
```

**Palavra que a pessoa ia digitar:** `NOTHING`: aparece em 2º lugar de 3

**Saída** (as 20 primeiras sugestões):

```text
 1 NOT
 2 NOTHING
 3 now [a]
```

**Veredito:** ✅ Bom. NOT, NOTHING, now.

---

### SQL-23
<a id="sql-23"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 12 · **lugar na gramática:** `sql:continue-update`

**Entrada** (a string onde está o cursor):

```text

    UPDATE users
    SE▮ display_name = ${name}, updated_at = now()
    WHERE id = ${id}
  
```

**Palavra que a pessoa ia digitar:** `SET`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 SET
 2 SELECT
 3 SERIAL
 4 sessions [a]
```

**Veredito:** ✅ Bom. SET em primeiro.

---

### SQL-24
<a id="sql-24"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 13 · **lugar na gramática:** `sql:continue-set`

**Entrada** (a string onde está o cursor):

```text

    UPDATE users
    SET display_name = ${name}, updated_at = now()
    ▮ id = ${id}
  
```

**Palavra que a pessoa ia digitar:** `WHERE`: aparece em 2º lugar de 100

**Saída** (as 20 primeiras sugestões):

```text
 1 id [a]      11 as
 2 where       12 asc
 3 from        13 begin
 4 returning   14 between
 5 add         15 bigint
 6 all         16 bigserial
 7 alter       17 blob
 8 analyze     18 bool
 9 and         19 boolean
10 any         20 by
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois das atribuições do SET: `id` (a palavra depois do cursor) primeiro, WHERE em 2º.

---

### SQL-25
<a id="sql-25"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 19 · **lugar na gramática:** `sql:after-delete`

**Entrada** (a string onde está o cursor):

```text

    DELETE F▮ sessions
    WHERE last_seen < now() - interval '${days} days'
      AND user_id IN (SELECT id FROM users WHERE active = false)
  
```

**Palavra que a pessoa ia digitar:** `FROM`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 FROM
 2 FALSE
 3 FETCH
 4 FIRST
 5 FLOAT
 6 FOR
 7 FOREIGN
 8 FULL
 9 FUNCTION
```

**Veredito:** ✅ Bom. FROM em primeiro.

---

### SQL-26
<a id="sql-26"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D2, linha 20 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text

    DELETE FROM sessions
    WHERE la▮ < now() - interval '${days} days'
      AND user_id IN (SELECT id FROM users WHERE active = false)
  
```

**Palavra que a pessoa ia digitar:** `last_seen`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 language
 2 lateral
```

**Veredito:** ✅ Bom. A coluna só aparece aqui, então nada pode estar certo; as palavras-chave oferecidas são inofensivas.

---

### SQL-27
<a id="sql-27"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D2, linha 20 · **lugar na gramática:** `sql:continue-where`

**Entrada** (a string onde está o cursor):

```text

    DELETE FROM sessions
    WHERE last_seen < now() - interval '${days} day▮'
      AND user_id IN (SELECT id FROM users WHERE active = false)
  
```

**Palavra que a pessoa ia digitar:** `days`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 days [a]
```

**Veredito:** ✅ Bom. `days` é o único item.

---

### SQL-28
<a id="sql-28"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D2, linha 21 · **lugar na gramática:** `sql:continue-where`

**Entrada** (a string onde está o cursor):

```text

    DELETE FROM sessions
    WHERE last_seen < now() - interval '${days} days'
      AND user_id ▮ (SELECT id FROM users WHERE active = false)
  
```

**Palavra que a pessoa ia digitar:** `IN`: aparece em 7º lugar de 100

**Saída** (as 20 primeiras sugestões):

```text
 1 and       11 not
 2 or        12 union
 3 group     13 offset
 4 order     14 add
 5 limit     15 all
 6 having    16 alter
 7 in        17 analyze
 8 is        18 any
 9 like      19 as
10 between   20 asc
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito: and/or/group/order/limit/having vêm antes de IN, que é o 7º.

---

### SQL-29
<a id="sql-29"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D2, linha 21 · **lugar na gramática:** `sql:table`

**Entrada** (a string onde está o cursor):

```text

    DELETE FROM sessions
    WHERE last_seen < now() - interval '${days} days'
      AND user_id IN (SELECT id FROM u▮ WHERE active = false)
  
```

**Palavra que a pessoa ia digitar:** `users`: aparece em 3º lugar de 11

**Saída** (as 20 primeiras sugestões):

```text
 1 user_id [a]      11 uuid
 2 updated_at [a]
 3 users [a]
 4 UPDATE [a]
 5 user [a]
 6 union
 7 unique
 8 unlock
 9 use
10 using
```

**Veredito:** ⚠️ Razoável, com ressalva. `FROM u` numa subconsulta: colunas (`user_id`, `updated_at`) vêm antes da tabela `users`, que é a 3ª.

---

### SQL-30
<a id="sql-30"></a>

**Situação:** erro de digitação: uma letra faltando · **documento** D2, linha 21 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text

    DELETE FROM sessions
    WHERE last_seen < now() - interval '${days} days'
      AND user_id IN (SELECT id FROM users WHERE active = fas▮)
  
```

**Palavra que a pessoa ia digitar:** `false`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 false [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: `false`.

---

### SQL-31
<a id="sql-31"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 4 · **lugar na gramática:** `sql:table`

**Entrada** (a string onde está o cursor):

```text

    CREATE TABLE inv▮ (
      id SERIAL PRIMARY KEY,
      customer_id INTEGER NOT NULL REFERENCES customers (id),
      number VARCHAR(32) NOT NULL UNIQUE,
      amount NUMERIC(12, 2) NOT NULL DEFAULT 0,
      issued_at TIMESTAMP NOT NULL DEFAULT now(),
      paid BOOLEAN NOT NULL DEFAULT false
    )
  
```

**Palavra que a pessoa ia digitar:** `invoices`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 invoices [a]
```

**Veredito:** ✅ Bom. `invoices` é o único item.

---

### SQL-32
<a id="sql-32"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 5 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

    CREATE TABLE invoices (
      id SERIAL ▮ KEY,
      customer_id INTEGER NOT NULL REFERENCES customers (id),
      number VARCHAR(32) NOT NULL UNIQUE,
      amount NUMERIC(12, 2) NOT NULL DEFAULT 0,
      issued_at TIMESTAMP NOT NULL DEFAULT now(),
      paid BOOLEAN NOT NULL DEFAULT false
    )
  
```

**Palavra que a pessoa ia digitar:** `PRIMARY`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 key       11 between
 2 add       12 bigint
 3 all       13 bigserial
 4 alter     14 blob
 5 analyze   15 bool
 6 and       16 boolean
 7 any       17 by
 8 as        18 bytea
 9 asc       19 call
10 begin     20 cascade
```

**Veredito:** ❌ Ruim. Pedido explícito numa definição de coluna: sem regra, lista alfabética de palavras-chave cortada em 100; PRIMARY fica fora de alcance.

---

### SQL-33
<a id="sql-33"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

    CREATE TABLE invoices (
      id SERIAL PRIMARY KEY,
      customer_id I▮ NOT NULL REFERENCES customers (id),
      number VARCHAR(32) NOT NULL UNIQUE,
      amount NUMERIC(12, 2) NOT NULL DEFAULT 0,
      issued_at TIMESTAMP NOT NULL DEFAULT now(),
      paid BOOLEAN NOT NULL DEFAULT false
    )
  
```

**Palavra que a pessoa ia digitar:** `INTEGER`: aparece em 8º lugar de 15

**Saída** (as 20 primeiras sugestões):

```text
 1 IF          11 INTO
 2 ILIKE       12 IS
 3 IN          13 invoices [a]
 4 INDEX       14 issued_at [a]
 5 INNER       15 idx_invoices_customer [a]
 6 INSERT
 7 INT
 8 INTEGER
 9 INTERSECT
10 INTERVAL
```

**Veredito:** ⚠️ Razoável, com ressalva. Espera-se um tipo de dado: palavras-chave em ordem alfabética, INTEGER é o 8º.

---

### SQL-34
<a id="sql-34"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

    CREATE TABLE invoices (
      id SERIAL PRIMARY KEY,
      customer_id INTEGER NOT NULL RE▮ customers (id),
      number VARCHAR(32) NOT NULL UNIQUE,
      amount NUMERIC(12, 2) NOT NULL DEFAULT 0,
      issued_at TIMESTAMP NOT NULL DEFAULT now(),
      paid BOOLEAN NOT NULL DEFAULT false
    )
  
```

**Palavra que a pessoa ia digitar:** `REFERENCES`: aparece em 3º lugar de 9

**Saída** (as 20 primeiras sugestões):

```text
 1 REAL
 2 RECURSIVE
 3 REFERENCES
 4 RENAME
 5 REPLACE
 6 RETURN
 7 RETURNING
 8 RETURNS
 9 REVOKE
```

**Veredito:** ✅ Bom. REFERENCES é o 3º entre as palavras-chave RE...

---

### SQL-35
<a id="sql-35"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

    CREATE TABLE invoices (
      id SERIAL PRIMARY KEY,
      customer_id INTEGER NOT NULL REFERENCES customers (id),
      number VAR▮(32) NOT NULL UNIQUE,
      amount NUMERIC(12, 2) NOT NULL DEFAULT 0,
      issued_at TIMESTAMP NOT NULL DEFAULT now(),
      paid BOOLEAN NOT NULL DEFAULT false
    )
  
```

**Palavra que a pessoa ia digitar:** `VARCHAR`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 VARCHAR
```

**Veredito:** ✅ Bom. VARCHAR é o único item.

---

### SQL-36
<a id="sql-36"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 7 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

    CREATE TABLE invoices (
      id SERIAL PRIMARY KEY,
      customer_id INTEGER NOT NULL REFERENCES customers (id),
      number VARCHAR(32) NOT NULL ▮,
      amount NUMERIC(12, 2) NOT NULL DEFAULT 0,
      issued_at TIMESTAMP NOT NULL DEFAULT now(),
      paid BOOLEAN NOT NULL DEFAULT false
    )
  
```

**Palavra que a pessoa ia digitar:** `UNIQUE`: **não aparece** na lista (100 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 default      11 begin
 2 references   12 between
 3 add          13 bigint
 4 all          14 bigserial
 5 alter        15 blob
 6 analyze      16 bool
 7 and          17 boolean
 8 any          18 by
 9 as           19 bytea
10 asc          20 call
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de NOT NULL: DEFAULT e REFERENCES primeiro (já vieram depois de NULL antes), mas UNIQUE é cortada pela lista alfabética.

---

### SQL-37
<a id="sql-37"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 8 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

    CREATE TABLE invoices (
      id SERIAL PRIMARY KEY,
      customer_id INTEGER NOT NULL REFERENCES customers (id),
      number VARCHAR(32) NOT NULL UNIQUE,
      amount NUMERIC(12, 2) N▮ NULL DEFAULT 0,
      issued_at TIMESTAMP NOT NULL DEFAULT now(),
      paid BOOLEAN NOT NULL DEFAULT false
    )
  
```

**Palavra que a pessoa ia digitar:** `NOT`: aparece em 3º lugar de 9

**Saída** (as 20 primeiras sugestões):

```text
 1 NATURAL
 2 NEXT
 3 NOT
 4 NOTHING
 5 NULL
 6 NUMERIC
 7 NVARCHAR
 8 number [a]
 9 now [a]
```

**Veredito:** ✅ Bom. NOT em 3º, NULL e NUMERIC ao redor.

---

### SQL-38
<a id="sql-38"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 9 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text

    CREATE TABLE invoices (
      id SERIAL PRIMARY KEY,
      customer_id INTEGER NOT NULL REFERENCES customers (id),
      number VARCHAR(32) NOT NULL UNIQUE,
      amount NUMERIC(12, 2) NOT NULL DEFAULT 0,
      is▮ TIMESTAMP NOT NULL DEFAULT now(),
      paid BOOLEAN NOT NULL DEFAULT false
    )
  
```

**Palavra que a pessoa ia digitar:** `issued_at`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 issued_at [a]
```

**Veredito:** ✅ Bom. `issued_at` é o único item.

---

### SQL-39
<a id="sql-39"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 9 · **lugar na gramática:** `sql:after-not`

**Entrada** (a string onde está o cursor):

```text

    CREATE TABLE invoices (
      id SERIAL PRIMARY KEY,
      customer_id INTEGER NOT NULL REFERENCES customers (id),
      number VARCHAR(32) NOT NULL UNIQUE,
      amount NUMERIC(12, 2) NOT NULL DEFAULT 0,
      issued_at TIMESTAMP NOT NUL▮ DEFAULT now(),
      paid BOOLEAN NOT NULL DEFAULT false
    )
  
```

**Palavra que a pessoa ia digitar:** `NULL`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 NULL
```

**Veredito:** ✅ Bom. NULL é o único item.

---

### SQL-40
<a id="sql-40"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D3, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

    CREATE TABLE invoices (
      id SERIAL PRIMARY KEY,
      customer_id INTEGER NOT NULL REFERENCES customers (id),
      number VARCHAR(32) NOT NULL UNIQUE,
      amount NUMERIC(12, 2) NOT NULL DEFAULT 0,
      issued_at TIMESTAMP NOT NULL DEFAULT now(),
      paid BOOLEA▮ NOT NULL DEFAULT false
    )
  
```

**Palavra que a pessoa ia digitar:** `BOOLEAN`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 BOOLEAN
```

**Veredito:** ✅ Bom. BOOLEAN é o único item.

---

### SQL-41
<a id="sql-41"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 10 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

    CREATE TABLE invoices (
      id SERIAL PRIMARY KEY,
      customer_id INTEGER NOT NULL REFERENCES customers (id),
      number VARCHAR(32) NOT NULL UNIQUE,
      amount NUMERIC(12, 2) NOT NULL DEFAULT 0,
      issued_at TIMESTAMP NOT NULL DEFAULT now(),
      paid BOOLEAN NOT NULL D▮ false
    )
  
```

**Palavra que a pessoa ia digitar:** `DEFAULT`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 DEFAULT    11 DROP
 2 DATABASE   12 down [a]
 3 DATE
 4 DECIMAL
 5 DECLARE
 6 DELETE
 7 DESC
 8 DISTINCT
 9 DO
10 DOUBLE
```

**Veredito:** ✅ Bom. DEFAULT em primeiro.

---

### SQL-42
<a id="sql-42"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 13 · **lugar na gramática:** `sql:after-create`

**Entrada** (a string onde está o cursor):

```text
CREATE IN▮ idx_invoices_customer ON invoices (customer_id, issued_at)
```

**Palavra que a pessoa ia digitar:** `INDEX`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 INDEX
 2 INNER
 3 INSERT
 4 INT
 5 INTEGER
 6 INTERSECT
 7 INTERVAL
 8 INTO
 9 invoices [a]
```

**Veredito:** ✅ Bom. INDEX em primeiro.

---

### SQL-43
<a id="sql-43"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 13 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text
CREATE INDEX idx_invoices_customer ON inv▮ (customer_id, issued_at)
```

**Palavra que a pessoa ia digitar:** `invoices`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 invoices [a]
```

**Veredito:** ✅ Bom. `invoices` é o único item.

---

### SQL-44
<a id="sql-44"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 14 · **lugar na gramática:** `sql:statement`

**Entrada** (a string onde está o cursor):

```text
▮ TABLE customers ADD COLUMN billing_email VARCHAR(255)
```

**Palavra que a pessoa ia digitar:** `ALTER`: aparece em 7º lugar de 100

**Saída** (as 20 primeiras sugestões):

```text
 1 select     11 explain
 2 insert     12 add
 3 update     13 all
 4 delete     14 analyze
 5 with       15 and
 6 create     16 any
 7 alter      17 as
 8 drop       18 asc
 9 truncate   19 begin
10 merge      20 between
```

**Veredito:** ✅ Bom. Inícios de comando primeiro, ALTER em 7º.

---

### SQL-45
<a id="sql-45"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 14 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text
ALTER TABLE customers A▮ COLUMN billing_email VARCHAR(255)
```

**Palavra que a pessoa ia digitar:** `ADD`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 ADD
 2 ALL
 3 ALTER
 4 ANALYZE
 5 AND
 6 ANY
 7 AS
 8 ASC
 9 amount [a]
```

**Veredito:** ✅ Bom. ADD em primeiro.

---

### SQL-46
<a id="sql-46"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D3, linha 18 · **lugar na gramática:** `sql:statement`

**Entrada** (a string onde está o cursor):

```text
DR▮ INDEX idx_invoices_customer
```

**Palavra que a pessoa ia digitar:** `DROP`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 DROP
```

**Veredito:** ✅ Bom. DROP é o único item.

---

### SQL-47
<a id="sql-47"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D3, linha 19 · **lugar na gramática:** `sql:statement`

**Entrada** (a string onde está o cursor):

```text
ALT▮ TABLE customers DROP COLUMN billing_email
```

**Palavra que a pessoa ia digitar:** `ALTER`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 ALTER
```

**Veredito:** ✅ Bom. ALTER é o único item.

---

### SQL-48
<a id="sql-48"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D3, linha 19 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text
ALTER TABLE customers ▮ COLUMN billing_email
```

**Palavra que a pessoa ia digitar:** `DROP`: aparece em 40º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 column    11 between
 2 add       12 bigint
 3 all       13 bigserial
 4 alter     14 blob
 5 analyze   15 bool
 6 and       16 boolean
 7 any       17 by
 8 as        18 bytea
 9 asc       19 call
10 begin     20 cascade
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `ALTER TABLE customers`: `column` (a palavra depois do cursor) primeiro, depois lista alfabética; DROP é o 40º (não há regra para ALTER TABLE).

---

### SQL-49
<a id="sql-49"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D3, linha 20 · **lugar na gramática:** `sql:statement`

**Entrada** (a string onde está o cursor):

```text
D▮ TABLE invoices
```

**Palavra que a pessoa ia digitar:** `DROP`: aparece em 2º lugar de 12

**Saída** (as 20 primeiras sugestões):

```text
 1 DELETE     11 DOUBLE
 2 DROP       12 down [a]
 3 DATABASE
 4 DATE
 5 DECIMAL
 6 DECLARE
 7 DEFAULT
 8 DESC
 9 DISTINCT
10 DO
```

**Veredito:** ✅ Bom. DELETE, DROP.

---

### SQL-50
<a id="sql-50"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D4, linha 2 · **lugar na gramática:** `sql:statement`

**Entrada** (a string onde está o cursor):

```text

  WI▮ monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `WITH`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 WITH
 2 WINDOW
```

**Veredito:** ✅ Bom. WITH em primeiro.

---

### SQL-51
<a id="sql-51"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 3 · **lugar na gramática:** `sql:after-select`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT cus▮, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `customer_id`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 customer_id [a]
```

**Veredito:** ✅ Bom. `customer_id` é o único item.

---

### SQL-52
<a id="sql-52"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 3 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', ▮) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `issued_at`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 month [a]         11 WHERE [a]
 2 date_trunc [a]    12 WITH [a]
 3 SUM [a]           13 paid [a]
 4 amount [a]        14 true [a]
 5 customer_id [a]   15 GROUP [a]
 6 total [a]         16 ranked [a]
 7 SELECT [a]        17 RANK [a]
 8 FROM [a]          18 OVER [a]
 9 invoices [a]      19 PARTITION [a]
10 monthly [a]       20 ORDER [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Dentro de uma chamada de função: as palavras mais próximas, misturadas com SELECT, FROM e WHERE; não há lógica de colunas primeiro.

---

### SQL-53
<a id="sql-53"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 3 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, S▮(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `SUM`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 SELECT [a]      11 SOME
 2 shrinking [a]
 3 sql [a]
 4 SAVEPOINT
 5 SCHEMA
 6 SERIAL
 7 SET
 8 SHARE
 9 SIMILAR
10 SMALLINT
```

**Veredito:** ⚠️ Razoável, com ressalva. `S(amount)`: SUM não é palavra-chave do vocabulário; aparecem SELECT, `shrinking`, `sql`.

---

### SQL-54
<a id="sql-54"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D4, linha 3 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS to▮
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `total`: aparece em 2º lugar de 2

**Saída** (as 20 primeiras sugestões):

```text
 1 top
 2 total [a]
```

**Veredito:** ✅ Bom. `top` e `total`.

---

### SQL-55
<a id="sql-55"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 5 · **lugar na gramática:** `sql:continue-from`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHE▮ paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `WHERE`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 WHERE
 2 WHEN
```

**Veredito:** ✅ Bom. WHERE em primeiro.

---

### SQL-56
<a id="sql-56"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 6 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    ▮ BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `GROUP`: aparece em 58º lugar de 100 (fora dos 20 primeiros mostrados)

**Saída** (as 20 primeiras sugestões):

```text
 1 by        11 between
 2 add       12 bigint
 3 all       13 bigserial
 4 alter     14 blob
 5 analyze   15 bool
 6 and       16 boolean
 7 any       17 bytea
 8 as        18 call
 9 asc       19 cascade
10 begin     20 case
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de uma condição do WHERE: `by` (a palavra depois do cursor) primeiro; GROUP é o 58º (nada depois de um literal como `true`).

---

### SQL-57
<a id="sql-57"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 7 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), r▮ AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `ranked`: aparece em 2º lugar de 16

**Saída** (as 20 primeiras sugestões):

```text
 1 RANK [a]     11 returns
 2 ranked [a]   12 revoke
 3 report [a]   13 right
 4 real         14 rollback
 5 recursive    15 row
 6 references   16 rows
 7 rename
 8 replace
 9 return
10 returning
```

**Veredito:** ✅ Bom. `ranked` em 2º, depois de `RANK`.

---

### SQL-58
<a id="sql-58"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D4, linha 8 · **lugar na gramática:** `sql:after-select`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT cu▮, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `customer_id`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 customer_id [a]
```

**Veredito:** ✅ Bom. `customer_id` é o único item.

---

### SQL-59
<a id="sql-59"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 9 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RAN▮() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `RANK`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 ranked [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `RAN(`: RANK é uma função e falta no vocabulário; só `ranked` é oferecido.

---

### SQL-60
<a id="sql-60"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 9 · **lugar na gramática:** `sql:after-partition`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION ▮ month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `BY`: aparece em 2º lugar de 100

**Saída** (as 20 primeiras sugestões):

```text
 1 month [a]   11 begin
 2 by          12 between
 3 add         13 bigint
 4 all         14 bigserial
 5 alter       15 blob
 6 analyze     16 bool
 7 and         17 boolean
 8 any         18 bytea
 9 as          19 call
10 asc         20 cascade
```

**Veredito:** ✅ Bom. `by` em 2º, depois da palavra que vem em seguida.

---

### SQL-61
<a id="sql-61"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 9 · **lugar na gramática:** `sql:after-order`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER B▮ total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `BY`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 BY
 2 BEGIN
 3 BETWEEN
 4 BIGINT
 5 BIGSERIAL
 6 BLOB
 7 BOOL
 8 BOOLEAN
 9 BYTEA
```

**Veredito:** ✅ Bom. BY em primeiro.

---

### SQL-62
<a id="sql-62"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 9 · **lugar na gramática:** `sql:continue-order`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) A▮ position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `AS`: aparece em 8º lugar de 10

**Saída** (as 20 primeiras sugestões):

```text
 1 ASC
 2 ADD
 3 ALL
 4 ALTER
 5 ANALYZE
 6 AND
 7 ANY
 8 AS
 9 amount [a]
10 applied [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. `) A position`: ASC primeiro, AS em 8º (o ORDER BY de dentro do OVER( ) é tomado como a cláusula).

---

### SQL-63
<a id="sql-63"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 10 · **lugar na gramática:** `sql:continue-order`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVE▮ (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `OVER`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 OVER
```

**Veredito:** ✅ Bom. OVER é o único item.

---

### SQL-64
<a id="sql-64"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 10 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY ▮ ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `customer_id`: aparece em 4º lugar de 100

**Saída** (as 20 primeiras sugestões):

```text
 1 order                11 monthly [a]
 2 month [a]            12 DESC [a]
 3 total [a]            13 SELECT [a]
 4 customer_id [a]      14 RANK [a]
 5 PARTITION [a]        15 CASE [a]
 6 OVER [a]             16 WHEN [a]
 7 previous_total [a]   17 NULL [a]
 8 LAG [a]              18 THEN [a]
 9 FROM [a]             19 new [a]
10 position [a]         20 ranked [a]
```

**Veredito:** ✅ Bom. Palavras do arquivo, `customer_id` em 4º.

---

### SQL-65
<a id="sql-65"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 10 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY m▮) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `month`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 month [a]
 2 monthly [a]
 3 matched
 4 merge
 5 money
```

**Veredito:** ✅ Bom. `month` em primeiro.

---

### SQL-66
<a id="sql-66"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D4, linha 11 · **lugar na gramática:** `sql:continue-order`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FR▮ monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `FROM`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 FROM
```

**Veredito:** ✅ Bom. FROM é o único item.

---

### SQL-67
<a id="sql-67"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 13 · **lugar na gramática:** `sql:member`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.cus▮, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `customer_id`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 customer_id [a]
```

**Veredito:** ✅ Bom. `customer_id` é o único item.

---

### SQL-68
<a id="sql-68"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 14 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE ▮ r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `WHEN`: aparece em 10º lugar de 100

**Saída** (as 20 primeiras sugestões):

```text
 1 CASE [a]             11 monthly [a]
 2 previous_total [a]   12 FROM [a]
 3 total [a]            13 growing [a]
 4 NULL [a]             14 ORDER [a]
 5 month [a]            15 ELSE [a]
 6 THEN [a]             16 shrinking [a]
 7 new [a]              17 PARTITION [a]
 8 customer_id [a]      18 END [a]
 9 SELECT [a]           19 trend [a]
10 WHEN [a]             20 OVER [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de CASE: palavras próximas; WHEN é o 10º.

---

### SQL-69
<a id="sql-69"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 14 · **lugar na gramática:** `sql:after-is`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS N▮ THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `NULL`: aparece em 2º lugar de 9

**Saída** (as 20 primeiras sugestões):

```text
 1 NOT
 2 NULL
 3 NATURAL
 4 NEXT
 5 NOTHING
 6 NUMERIC
 7 NVARCHAR
 8 new [a]
 9 number [a]
```

**Veredito:** ✅ Bom. NOT, NULL.

---

### SQL-70
<a id="sql-70"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D4, linha 15 · **lugar na gramática:** `sql:continue-select`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WH▮ r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `WHEN`: aparece em 2º lugar de 2

**Saída** (as 20 primeiras sugestões):

```text
 1 WHERE
 2 WHEN
```

**Veredito:** ⚠️ Razoável, com ressalva. WHERE antes de WHEN (WHEN vem depois de um valor de THEN).

---

### SQL-71
<a id="sql-71"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 15 · **lugar na gramática:** `sql:continue-select`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THE▮ 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `THEN`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 THEN
```

**Veredito:** ✅ Bom. THEN é o único item.

---

### SQL-72
<a id="sql-72"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 16 · **lugar na gramática:** `sql:continue-select`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE '▮' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `shrinking`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 from      11 alter
 2 as        12 analyze
 3 where     13 and
 4 order     14 any
 5 group     15 asc
 6 limit     16 begin
 7 union     17 between
 8 into      18 bigint
 9 add       19 bigserial
10 all       20 blob
```

**Veredito:** ❌ Ruim. Dentro de um literal entre aspas: palavras-chave como from/as/where são oferecidas; nada deveria ser oferecido ali.

---

### SQL-73
<a id="sql-73"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 16 · **lugar na gramática:** `(no rule)`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS t▮
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `trend`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 total [a]     11 to
 2 table         12 top
 3 temp          13 transaction
 4 temporary     14 trigger
 5 text          15 true
 6 then          16 truncate
 7 time
 8 timestamp
 9 timestamptz
10 tinyint
```

**Veredito:** ⚠️ Razoável, com ressalva. Depois de `AS` (um apelido): `total`, depois palavras-chave com t (table, temp, text...): ruído para um nome novo.

---

### SQL-74
<a id="sql-74"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D4, linha 18 · **lugar na gramática:** `sql:member`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.po▮ <= 10
  ORDER BY r.month, r.position

```

**Palavra que a pessoa ia digitar:** `position`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 position [a]
```

**Veredito:** ✅ Bom. `position` é o único item.

---

### SQL-75
<a id="sql-75"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 19 · **lugar na gramática:** `sql:member`

**Entrada** (a string onde está o cursor):

```text

  WITH monthly AS (
    SELECT customer_id, date_trunc('month', issued_at) AS month, SUM(amount) AS total
    FROM invoices
    WHERE paid = true
    GROUP BY customer_id, month
  ), ranked AS (
    SELECT customer_id, month, total,
           RANK() OVER (PARTITION BY month ORDER BY total DESC) AS position,
           LAG(total) OVER (PARTITION BY customer_id ORDER BY month) AS previous_total
    FROM monthly
  )
  SELECT r.customer_id, r.month, r.total,
         CASE WHEN r.previous_total IS NULL THEN 'new'
              WHEN r.total > r.previous_total THEN 'growing'
              ELSE 'shrinking' END AS trend
  FROM ranked r
  WHERE r.position <= 10
  ORDER BY r.mon▮, r.position

```

**Palavra que a pessoa ia digitar:** `month`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 month [a]
 2 monthly [a]
 3 money
```

**Veredito:** ✅ Bom. `month` em primeiro.

---

### SQL-76
<a id="sql-76"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D4, linha 23 · **lugar na gramática:** `sql:after-select`

**Entrada** (a string onde está o cursor):

```text

  SELECT ▮, amount FROM invoices WHERE paid = false
  UNION ALL
  SELECT number, amount FROM credit_notes WHERE applied = false

```

**Palavra que a pessoa ia digitar:** `number`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 number [a]        11 position [a]
 2 customer_id [a]   12 false [a]
 3 distinct          13 UNION [a]
 4 all               14 month [a]
 5 SELECT [a]        15 ORDER [a]
 6 amount [a]        16 credit_notes [a]
 7 FROM [a]          17 ranked [a]
 8 invoices [a]      18 applied [a]
 9 WHERE [a]         19 trend [a]
10 paid [a]          20 END [a]
```

**Veredito:** ✅ Bom. `number` em primeiro.

---

### SQL-77
<a id="sql-77"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 23 · **lugar na gramática:** `sql:table`

**Entrada** (a string onde está o cursor):

```text

  SELECT number, amount FROM i▮ WHERE paid = false
  UNION ALL
  SELECT number, amount FROM credit_notes WHERE applied = false

```

**Palavra que a pessoa ia digitar:** `invoices`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 invoices [a]    11 intersect
 2 issued_at [a]   12 interval
 3 if              13 into
 4 ilike           14 is
 5 in
 6 index
 7 inner
 8 insert
 9 int
10 integer
```

**Veredito:** ✅ Bom. `invoices` em primeiro (`issued_at`, uma coluna, em 2º).

---

### SQL-78
<a id="sql-78"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D4, linha 23 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text

  SELECT number, amount FROM invoices WHERE paid = fa▮
  UNION ALL
  SELECT number, amount FROM credit_notes WHERE applied = false

```

**Palavra que a pessoa ia digitar:** `false`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 false [a]
```

**Veredito:** ✅ Bom. `false` é o único item.

---

### SQL-79
<a id="sql-79"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D4, linha 25 · **lugar na gramática:** `sql:after-select`

**Entrada** (a string onde está o cursor):

```text

  SELECT number, amount FROM invoices WHERE paid = false
  UNION ALL
  SELECT num▮, amount FROM credit_notes WHERE applied = false

```

**Palavra que a pessoa ia digitar:** `number`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 number [a]
 2 numeric
```

**Veredito:** ✅ Bom. `number` em primeiro.

---

### SQL-80
<a id="sql-80"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D4, linha 25 · **lugar na gramática:** `sql:table`

**Entrada** (a string onde está o cursor):

```text

  SELECT number, amount FROM invoices WHERE paid = false
  UNION ALL
  SELECT number, amount FROM cerdit▮ WHERE applied = false

```

**Palavra que a pessoa ia digitar:** `credit_notes`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
(nenhuma sugestão)
```

**Veredito:** ✅ Bom. Nada oferecido; o nome da tabela não aparece em outro lugar do arquivo.

---

### SQL-81
<a id="sql-81"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D4, linha 25 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text

  SELECT number, amount FROM invoices WHERE paid = false
  UNION ALL
  SELECT number, amount FROM credit_notes WHERE applied = f▮

```

**Palavra que a pessoa ia digitar:** `false`: aparece em 2º lugar de 9

**Saída** (as 20 primeiras sugestões):

```text
 1 FROM [a]
 2 false [a]
 3 fetch
 4 first
 5 float
 6 for
 7 foreign
 8 full
 9 function
```

**Veredito:** ⚠️ Razoável, com ressalva. `= f`: FROM (palavra-chave de cláusula como palavra) antes de `false`.

---

### SQL-82
<a id="sql-82"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D5, linha 2 · **lugar na gramática:** `sql:member`

**Entrada** (a string onde está o cursor):

```text

  select p.sku, p.ti▮, coalesce(sum(s.quantity), 0) as on_hand
  from products p
  left join stock s on s.product_id = p.id
  where p.discontinued = false
  group by p.sku, p.title
  having coalesce(sum(s.quantity), 0) < ${threshold}
  order by on_hand asc, p.title
  limit 50

```

**Palavra que a pessoa ia digitar:** `title`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 title [a]
 2 time
 3 timestamp
 4 timestamptz
 5 tinyint
```

**Veredito:** ✅ Bom. `title` em primeiro.

---

### SQL-83
<a id="sql-83"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D5, linha 2 · **lugar na gramática:** `sql:member`

**Entrada** (a string onde está o cursor):

```text

  select p.sku, p.title, coalesce(sum(s.qua▮), 0) as on_hand
  from products p
  left join stock s on s.product_id = p.id
  where p.discontinued = false
  group by p.sku, p.title
  having coalesce(sum(s.quantity), 0) < ${threshold}
  order by on_hand asc, p.title
  limit 50

```

**Palavra que a pessoa ia digitar:** `quantity`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 quantity [a]
```

**Veredito:** ✅ Bom. `quantity` é o único item.

---

### SQL-84
<a id="sql-84"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D5, linha 3 · **lugar na gramática:** `sql:continue-select`

**Entrada** (a string onde está o cursor):

```text

  select p.sku, p.title, coalesce(sum(s.quantity), 0) as on_hand
  ▮ products p
  left join stock s on s.product_id = p.id
  where p.discontinued = false
  group by p.sku, p.title
  having coalesce(sum(s.quantity), 0) < ${threshold}
  order by on_hand asc, p.title
  limit 50

```

**Palavra que a pessoa ia digitar:** `from`: aparece em 3º lugar de 100

**Saída** (as 20 primeiras sugestões):

```text
 1 products [a]   11 add
 2 asc            12 all
 3 from           13 alter
 4 as             14 analyze
 5 where          15 and
 6 order          16 any
 7 group          17 begin
 8 limit          18 between
 9 union          19 bigint
10 into           20 bigserial
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito: `products` (a palavra depois do cursor) e `asc` vêm antes de FROM (3º).

---

### SQL-85
<a id="sql-85"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 4 · **lugar na gramática:** `sql:table`

**Entrada** (a string onde está o cursor):

```text

  select p.sku, p.title, coalesce(sum(s.quantity), 0) as on_hand
  from products p
  left join s▮ s on s.product_id = p.id
  where p.discontinued = false
  group by p.sku, p.title
  having coalesce(sum(s.quantity), 0) < ${threshold}
  order by on_hand asc, p.title
  limit 50

```

**Palavra que a pessoa ia digitar:** `stock`: aparece em 4º lugar de 13

**Saída** (as 20 primeiras sugestões):

```text
 1 sum [a]      11 similar
 2 sku [a]      12 smallint
 3 select [a]   13 some
 4 stock [a]
 5 set [a]
 6 sql [a]
 7 savepoint
 8 schema
 9 serial
10 share
```

**Veredito:** ⚠️ Razoável, com ressalva. `JOIN s`: `sum`, `sku`, `select` antes da tabela `stock` (4ª).

---

### SQL-86
<a id="sql-86"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 4 · **lugar na gramática:** `sql:member`

**Entrada** (a string onde está o cursor):

```text

  select p.sku, p.title, coalesce(sum(s.quantity), 0) as on_hand
  from products p
  left join stock s on s.product_id = p.i▮
  where p.discontinued = false
  group by p.sku, p.title
  having coalesce(sum(s.quantity), 0) < ${threshold}
  order by on_hand asc, p.title
  limit 50

```

**Palavra que a pessoa ia digitar:** `id`: **não aparece** na lista (12 itens)

**Saída** (as 20 primeiras sugestões):

```text
 1 if          11 into
 2 ilike       12 is
 3 in
 4 index
 5 inner
 6 insert
 7 int
 8 integer
 9 intersect
10 interval
```

**Veredito:** ❌ Ruim. `p.i`: `id` nunca pode ser sugerido (palavras com menos de 3 letras são ignoradas).

---

### SQL-87
<a id="sql-87"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D5, linha 5 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text

  select p.sku, p.title, coalesce(sum(s.quantity), 0) as on_hand
  from products p
  left join stock s on s.product_id = p.id
  where p.discontinued = fal▮
  group by p.sku, p.title
  having coalesce(sum(s.quantity), 0) < ${threshold}
  order by on_hand asc, p.title
  limit 50

```

**Palavra que a pessoa ia digitar:** `false`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 false
```

**Veredito:** ✅ Bom. `false` é o único item.

---

### SQL-88
<a id="sql-88"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D5, linha 6 · **lugar na gramática:** `sql:member`

**Entrada** (a string onde está o cursor):

```text

  select p.sku, p.title, coalesce(sum(s.quantity), 0) as on_hand
  from products p
  left join stock s on s.product_id = p.id
  where p.discontinued = false
  group by p.▮, p.title
  having coalesce(sum(s.quantity), 0) < ${threshold}
  order by on_hand asc, p.title
  limit 50

```

**Palavra que a pessoa ia digitar:** `sku`: aparece em 4º lugar de 100

**Saída** (as 20 primeiras sugestões):

```text
 1 title [a]          11 where [a]
 2 discontinued [a]   12 product_id [a]
 3 id [a]             13 order [a]
 4 sku [a]            14 stock [a]
 5 group [a]          15 on_hand [a]
 6 having [a]         16 join [a]
 7 false [a]          17 left [a]
 8 coalesce [a]       18 asc [a]
 9 sum [a]            19 products [a]
10 quantity [a]       20 limit [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito depois de `p.`: colunas misturadas com group/having/where/join; `sku` em 4º.

---

### SQL-89
<a id="sql-89"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 7 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text

  select p.sku, p.title, coalesce(sum(s.quantity), 0) as on_hand
  from products p
  left join stock s on s.product_id = p.id
  where p.discontinued = false
  group by p.sku, p.title
  having c▮(sum(s.quantity), 0) < ${threshold}
  order by on_hand asc, p.title
  limit 50

```

**Palavra que a pessoa ia digitar:** `coalesce`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 coalesce [a]   11 commit
 2 const [a]      12 conflict
 3 cutoff [a]     13 constraint
 4 call           14 create
 5 cascade        15 cross
 6 case
 7 cast
 8 char
 9 check
10 column
```

**Veredito:** ✅ Bom. `coalesce` em primeiro.

---

### SQL-90
<a id="sql-90"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 8 · **lugar na gramática:** `sql:after-order`

**Entrada** (a string onde está o cursor):

```text

  select p.sku, p.title, coalesce(sum(s.quantity), 0) as on_hand
  from products p
  left join stock s on s.product_id = p.id
  where p.discontinued = false
  group by p.sku, p.title
  having coalesce(sum(s.quantity), 0) < ${threshold}
  order b▮ on_hand asc, p.title
  limit 50

```

**Palavra que a pessoa ia digitar:** `by`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 by
 2 begin
 3 between
 4 bigint
 5 bigserial
 6 blob
 7 bool
 8 boolean
 9 bytea
```

**Veredito:** ✅ Bom. BY em primeiro.

---

### SQL-91
<a id="sql-91"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D5, linha 8 · **lugar na gramática:** `sql:member`

**Entrada** (a string onde está o cursor):

```text

  select p.sku, p.title, coalesce(sum(s.quantity), 0) as on_hand
  from products p
  left join stock s on s.product_id = p.id
  where p.discontinued = false
  group by p.sku, p.title
  having coalesce(sum(s.quantity), 0) < ${threshold}
  order by on_hand asc, p.tit▮
  limit 50

```

**Palavra que a pessoa ia digitar:** `title`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 title [a]
```

**Veredito:** ✅ Bom. `title` é o único item.

---

### SQL-92
<a id="sql-92"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D5, linha 13 · **lugar na gramática:** `sql:after-select`

**Entrada** (a string onde está o cursor):

```text

  select ▮ from products
  where exists (select 1 from stock where stock.product_id = products.id and quantity = 0)

```

**Palavra que a pessoa ia digitar:** `title`: aparece em 10º lugar de 100

**Saída** (as 20 primeiras sugestões):

```text
 1 from           11 asc [a]
 2 distinct       12 product_id [a]
 3 all            13 on_hand [a]
 4 select [a]     14 order [a]
 5 products [a]   15 and [a]
 6 where [a]      16 quantity [a]
 7 exists [a]     17 sum [a]
 8 limit [a]      18 coalesce [a]
 9 stock [a]      19 having [a]
10 title [a]      20 update [a]
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito na lista do SELECT: `from` primeiro (a palavra depois do cursor), `title` em 10º.

---

### SQL-93
<a id="sql-93"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 14 · **lugar na gramática:** `sql:continue-from`

**Entrada** (a string onde está o cursor):

```text

  select title from products
  w▮ exists (select 1 from stock where stock.product_id = products.id and quantity = 0)

```

**Palavra que a pessoa ia digitar:** `where`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 where
 2 when
 3 window
 4 with
```

**Veredito:** ✅ Bom. WHERE em primeiro.

---

### SQL-94
<a id="sql-94"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D5, linha 14 · **lugar na gramática:** `sql:continue-select`

**Entrada** (a string onde está o cursor):

```text

  select title from products
  where exists (select 1 fr▮ stock where stock.product_id = products.id and quantity = 0)

```

**Palavra que a pessoa ia digitar:** `from`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 from
```

**Veredito:** ✅ Bom. FROM é o único item.

---

### SQL-95
<a id="sql-95"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D5, linha 14 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text

  select title from products
  where exists (select 1 from stock where sto▮.product_id = products.id and quantity = 0)

```

**Palavra que a pessoa ia digitar:** `stock`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 stock [a]
```

**Veredito:** ✅ Bom. `stock` é o único item.

---

### SQL-96
<a id="sql-96"></a>

**Situação:** pedido explícito (Ctrl+Espaço), nada digitado ainda · **documento** D5, linha 14 · **lugar na gramática:** `sql:continue-where`

**Entrada** (a string onde está o cursor):

```text

  select title from products
  where exists (select 1 from stock where stock.product_id = products.id ▮ quantity = 0)

```

**Palavra que a pessoa ia digitar:** `and`: aparece em 3º lugar de 100

**Saída** (as 20 primeiras sugestões):

```text
 1 quantity [a]   11 like
 2 where          12 between
 3 and            13 not
 4 or             14 union
 5 group          15 offset
 6 order          16 add
 7 limit          17 all
 8 having         18 alter
 9 in             19 analyze
10 is             20 any
```

**Veredito:** ⚠️ Razoável, com ressalva. Pedido explícito: `quantity` (a palavra depois do cursor) primeiro, AND em 3º.

---

### SQL-97
<a id="sql-97"></a>

**Situação:** digitando (1 letra já digitada) · **documento** D5, linha 17 · **lugar na gramática:** `sql:table`

**Entrada** (a string onde está o cursor):

```text
update p▮ set updated_at = now() where sku = ${sku}
```

**Palavra que a pessoa ia digitar:** `products`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 products [a]
 2 product_id [a]
 3 purge [a]
 4 partition
 5 precision
 6 primary
 7 procedure
```

**Veredito:** ✅ Bom. `products` em primeiro.

---

### SQL-98
<a id="sql-98"></a>

**Situação:** digitando (2 letras já digitadas) · **documento** D5, linha 17 · **lugar na gramática:** `sql:expression`

**Entrada** (a string onde está o cursor):

```text
update products set updated_at = no▮() where sku = ${sku}
```

**Palavra que a pessoa ia digitar:** `now`: a palavra não existe em outro lugar do arquivo nem no vocabulário, então nenhuma sugestão poderia acertá-la

**Saída** (as 20 primeiras sugestões):

```text
 1 not
 2 nothing
```

**Veredito:** ⚠️ Razoável, com ressalva. `no(`: `now()` é uma função e falta no vocabulário; NOT/NOTHING são oferecidas.

---

### SQL-99
<a id="sql-99"></a>

**Situação:** digitando (3 letras já digitadas) · **documento** D5, linha 18 · **lugar na gramática:** `sql:statement`

**Entrada** (a string onde está o cursor):

```text
del▮ from stock where quantity = 0 and updated_at < ${cutoff}
```

**Palavra que a pessoa ia digitar:** `delete`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 delete
```

**Veredito:** ✅ Bom. DELETE é o único item.

---

### SQL-100
<a id="sql-100"></a>

**Situação:** erro de digitação: duas letras trocadas · **documento** D5, linha 18 · **lugar na gramática:** `sql:continue-from`

**Entrada** (a string onde está o cursor):

```text
delete from stock wehr▮ quantity = 0 and updated_at < ${cutoff}
```

**Palavra que a pessoa ia digitar:** `where`: aparece em 1º lugar

**Saída** (as 20 primeiras sugestões):

```text
 1 where [~]
```

**Veredito:** ✅ Bom. Erro de digitação corrigido: WHERE.

---
