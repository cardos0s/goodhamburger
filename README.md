# 🍔 Good Hamburger

Sistema de pedidos para uma lanchonete, com API REST em .NET 10 e frontend em Blazor WebAssembly.
Implementação do desafio técnico para desenvolvedor C#, com foco em Clean Architecture, Domain-Driven Design e testabilidade.

---

## 📐 Arquitetura

O projeto segue Clean Architecture em quatro camadas. A regra de dependência aponta sempre para dentro: `Domain` não conhece ninguém; `API` conhece todos.

```
GoodHamburger/
├── src/
│   ├── GoodHamburger.Domain/          # Entidades ricas, regras de negócio, exceções tipadas
│   ├── GoodHamburger.Application/     # Use cases, DTOs, validadores, interfaces de repositório
│   ├── GoodHamburger.Infrastructure/  # EF Core, repositórios, configurações de mapeamento
│   ├── GoodHamburger.API/             # Controllers, middleware global, DI, Swagger
│   └── GoodHamburger.Web/             # Blazor WebAssembly — consome a API
└── tests/
└── GoodHamburger.Tests/           # xUnit + AwesomeAssertions (24 testes unitários)

---
``` 
## 🚀 Como executar

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Rider, Visual Studio 2022+ ou VS Code

### Passos

```bash
# 1. Clonar
git clone https://github.com/cardos0s/goodhamburger.git
cd goodhamburger

# 2. Restaurar e buildar
dotnet build

# 3. Aplicar migrations (cria goodhamburger.db com o cardápio populado)
dotnet ef database update --project src/GoodHamburger.Infrastructure --startup-project src/GoodHamburger.API

# 4. Em um terminal, subir a API
dotnet run --project src/GoodHamburger.API
# → http://localhost:5173
# → Swagger UI em http://localhost:5173/swagger

# 5. Em OUTRO terminal, subir o frontend Blazor
dotnet run --project src/GoodHamburger.Web
# → http://localhost:5035

# 6. Rodar os testes
dotnet test
# → 24 testes, todos passando
```

---

## 🎯 Decisões de arquitetura

### Domain: entidade rica, não anêmica

`Pedido` encapsula suas invariantes. A lista interna de itens é `private readonly List<ItemPedido> _itens`, exposta apenas como `IReadOnlyList`. A única forma de adicionar item é via `AdicionarItem(Produto)`, que valida a regra "apenas um item por categoria". Isso impede que código externo burle a regra de negócio.

### Propriedades calculadas, não persistidas

`Subtotal`, `Desconto` e `Total` são expressões calculadas em tempo de leitura. Não viram colunas no banco (`Ignore` no mapeamento). Elimina risco de inconsistência entre dados-fonte (itens) e dados derivados.

### Cálculo de desconto como função pura

`CalculadoraDesconto` é uma `static class`. Aceita um `Pedido`, retorna um `decimal`. Zero estado, zero dependência externa. Testável sem mock, explicitamente separada do resto do domínio.

Se no futuro a regra precisar depender de estado externo (descontos por cliente vindos do banco, com validade), a refatoração natural é transformar em `ICalculadoraDesconto` injetada via DI. Hoje, YAGNI.

### Arredondamento comercial explícito

`Math.Round(valor, 2, MidpointRounding.AwayFromZero)`. O default do .NET é *banker's rounding*, que arredonda `0.125` para `0.12`. Em contextos fiscais brasileiros, o esperado é `0.13`. A escolha é documentada e coberta por teste — mudá-la quebra o teste intencionalmente.

### EF Core com backing field para coleções

O mapeamento do `Pedido` usa `.UsePropertyAccessMode(PropertyAccessMode.Field).HasField("_itens")`. Isso permite ao EF popular diretamente a lista privada ao hidratar do banco, preservando o encapsulamento da entidade rica. Sem esse truque, seria necessário expor um setter público e vazar a coleção interna.

### Foreign keys com políticas explícitas

- `Pedido → ItemPedido`: `Cascade`. Item não existe sem pedido.
- `ItemPedido → Produto`: `Restrict`. Produto pode sair do cardápio ativo, mas não pode ser deletado se houver pedidos históricos — protege a integridade do histórico.

### Enum persistido como inteiro

`HasConversion<int>()` em `CategoriaProduto`. Refatorar o nome do enum no código nunca quebra o banco em produção. Os valores são fixados explicitamente (`Sanduiche = 1`) para garantir estabilidade.

### Middleware global de exceção

Em vez de try/catch em cada controller, um middleware captura exceções e traduz por tipo:

| Exceção | Status HTTP |
|---|---|
| `ValidationException` (FluentValidation) | 400 com lista de erros por campo |
| `RecursoNaoEncontradoException` | 404 |
| `DomainException` (ex: `PedidoInvalidoException`) | 400 |
| `Exception` genérica | 500 com log, sem expor detalhes ao cliente |

Controllers ficam focados no happy path.

### DTOs como `record`

`record` dá imutabilidade, igualdade estrutural e `ToString` útil. Transferência de dados é naturalmente imutável — usar `class` com setters seria desperdício semântico.

### AwesomeAssertions em vez de FluentAssertions

FluentAssertions 8+ passou a exigir licença paga para uso comercial em 2024. AwesomeAssertions é o fork MIT mantido pela comunidade com API idêntica. Troca de 2 linhas que evita exposição legal.

### Validação em duas camadas

- **FluentValidation** valida formato do payload (IDs positivos, lista não-vazia)
- **Domain** valida regras de negócio (não pode dois sanduíches)

Concerns diferentes protegidos em lugares diferentes.

### Blazor desacoplado do Application

O frontend duplica os DTOs em `GoodHamburger.Web.Models` em vez de referenciar `GoodHamburger.Application`. O contrato entre frontend e backend é JSON (HTTP), não binário. Referenciar projetos do backend criaria acoplamento que impediria evoluir as camadas independentemente.

---

## 🧪 Testes

24 testes unitários cobrindo:

- **Invariantes da entidade `Pedido`** — não permite duas categorias iguais, remoção de item inexistente lança, reuso de validação no `SubstituirItens`
- **Todas as 3 regras de desconto** — 20% combo completo, 15% sanduíche+refri, 10% sanduíche+batata
- **Casos sem desconto** — pedido vazio, só sanduíche, só batata, só refri, batata+refri sem sanduíche
- **Arredondamento comercial** — teste cujo valor só passa com `AwayFromZero`

Testes organizados em **classes aninhadas** (padrão Context-Specification), que deixa o output do test runner legível como frase:
DescontoTests+ComboCompleto.Aplica_20_porcento          ✓
DescontoTests+SanduicheERefrigerante.Aplica_15_porcento ✓
PedidoTests+AoAdicionarItem.Segundo_sanduiche_eh_rejeitado ✓

---

## 📦 Stack técnica

### Backend
- **.NET 10**
- **ASP.NET Core** — Web API + OpenAPI
- **Entity Framework Core 10** com provider **SQLite**
- **FluentValidation** — validação de DTOs
- **Swashbuckle** — UI do Swagger
- **xUnit** + **AwesomeAssertions** — testes

### Frontend
- **Blazor WebAssembly** (.NET 10)
- **Poppins** via Google Fonts
- CSS custom com design system próprio (paleta dark + accent amarelo)
- Toast service para feedback visual
- Splash screen animada

---

## 🎨 Frontend

Design inspirado em um mockup dark com accent amarelo, layout responsivo com bottom tab bar no mobile.

Páginas:
- **Home**: splash screen animada → hero + cards de destaques
- **Cardápio**: grid de produtos com filtro por categoria, busca e botão `+` que leva ao modal de pedido já pré-selecionado
- **Pedidos**: tabela estilo dashboard + 4 métricas calculadas (pedidos hoje, faturamento, ticket médio, em andamento) + modal para criação e detalhamento

Itens "Clientes / Relatórios / Configurações" estão no menu marcados como **Em breve** — indicam extensões naturais, porém fora do escopo do desafio.

Feedback de UX:
- Toast service (`Sucesso / Aviso / Erro`) informa todas as ações
- Quando o usuário muda o item de uma categoria no modal, aparece aviso "X foi substituído por Y"
- Erros da API (ex: "produto não encontrado") viram toast vermelho automaticamente

---

## 🚧 O que ficou fora

Decisões conscientes para respeitar o prazo do desafio:

- **Autenticação** — não faz parte do enunciado. Em produção, usaria OAuth2/OIDC com Identity Server ou Keycloak.
- **Testes de integração com `WebApplicationFactory`** — os unitários cobrem as regras de negócio críticas. Integração seria o próximo passo natural.
- **Carrinho de compras persistente cross-page** — a UX atual é "modal de criação de pedido" com pré-seleção vinda do cardápio. Um carrinho real exigiria state service global e ícone fixo na sidebar.
- **Status de pedido** (Em andamento / Pronto / Entregue) — o backend não modela workflow de pedido. Os badges no frontend são representação visual.
- **Paginação em `GET /api/pedidos`** — com o volume esperado de um desafio, não compensa. Para produção, acrescentaria `?page=&pageSize=`.
- **Logging estruturado** (Serilog + Seq/ELK) — em dev, `ILogger` padrão da Microsoft é suficiente.

---

## 🗂️ Endpoints da API

| Verbo | Rota | Descrição |
|---|---|---|
| GET | `/api/cardapio` | Lista produtos disponíveis |
| GET | `/api/pedidos` | Lista todos os pedidos |
| GET | `/api/pedidos/{id}` | Consulta pedido por ID |
| POST | `/api/pedidos` | Cria pedido |
| PUT | `/api/pedidos/{id}` | Atualiza itens do pedido |
| DELETE | `/api/pedidos/{id}` | Remove pedido |

Documentação interativa completa: `http://localhost:5173/swagger`.

### Exemplo: criar um pedido

```http
POST /api/pedidos
Content-Type: application/json

{
  "produtoIds": [1, 4, 5]
}
```

Resposta:

```json
{
  "id": 1,
  "criadoEm": "2026-04-23T14:30:00Z",
  "itens": [
    { "produtoId": 1, "nomeProduto": "X Burger", "preco": 5.00 },
    { "produtoId": 4, "nomeProduto": "Batata frita", "preco": 2.00 },
    { "produtoId": 5, "nomeProduto": "Refrigerante", "preco": 2.50 }
  ],
  "subtotal": 9.50,
  "desconto": 1.90,
  "total": 7.60
}
```

---

## 📝 Cardápio fixo

Conforme o enunciado:

| Produto | Preço | Categoria |
|---|---|---|
| X Burger | R$ 5,00 | Sanduíche |
| X Egg | R$ 4,50 | Sanduíche |
| X Bacon | R$ 7,00 | Sanduíche |
| Batata frita | R$ 2,00 | Batata |
| Refrigerante | R$ 2,50 | Refrigerante |

### Regras de desconto

| Combinação | Desconto |
|---|---|
| Sanduíche + Batata + Refrigerante | 20% |
| Sanduíche + Refrigerante | 15% |
| Sanduíche + Batata | 10% |
| Qualquer outra | 0% |

**Restrição**: cada pedido pode conter no máximo **um sanduíche, uma batata e um refrigerante**. Duplicatas retornam HTTP 400 com mensagem explicativa.

---

