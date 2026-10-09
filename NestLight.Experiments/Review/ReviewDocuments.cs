using System.Collections.Generic;

namespace NestLight.Experiments
{
    /// <summary>
    /// Files written by hand, the way a developer writes them: host code with strings of one embedded language in them. They are not
    /// made by the generator of the experiments, so the review does not check the engine against its own assumptions.
    /// </summary>
    internal static class ReviewDocuments
    {
        public static readonly Dictionary<string, string[]> ByLanguage = new Dictionary<string, string[]>
        {
            { "sql", Sql() }, { "css", Css() }, { "html", Html() }, { "graphql", GraphQl() },
            { "json", Json() }, { "yaml", Yaml() }, { "glsl", Glsl() }, { "wgsl", Wgsl() },
        };

        public static readonly string[] Languages = { "sql", "css", "html", "graphql", "json", "yaml", "glsl", "wgsl" };

        private static string[] Sql()
        {
            return new[]
            {
"""
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
""",
"""
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
""",
"""
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
""",
"""
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
""",
"""
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
""",
            };
        }

        private static string[] Css()
        {
            return new[]
            {
"""
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
""",
"""
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
""",
"""
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
""",
"""
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
""",
"""
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
""",
            };
        }

        private static string[] Html()
        {
            return new[]
            {
"""
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
""",
"""
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
""",
"""
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
""",
"""
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
""",
"""
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
""",
            };
        }

        private static string[] GraphQl()
        {
            return new[]
            {
"""
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
""",
"""
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
""",
"""
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
""",
"""
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
""",
            };
        }

        private static string[] Json()
        {
            return new[]
            {
"""
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
""",
"""
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
""",
"""
const config = json`
{
  "server": { "host": "localhost", "port": 8080, "secure": false },
  "database": { "host": "db.internal", "port": 5432, "poolSize": 10, "ssl": true },
  "features": { "betaSearch": true, "legacyExport": false, "maintenanceBanner": null },
  "logging": { "level": "info", "destination": "stdout" }
}
`;

const empty = json`{ "items": [], "total": 0, "hasMore": false }`;
""",
            };
        }

        private static string[] Yaml()
        {
            return new[]
            {
"""
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
""",
"""
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
""",
"""
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
""",
            };
        }

        private static string[] Glsl()
        {
            return new[]
            {
"""
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
""",
"""
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
""",
"""
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
""",
            };
        }

        private static string[] Wgsl()
        {
            return new[]
            {
"""
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
""",
"""
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
""",
            };
        }
    }
}
