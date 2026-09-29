# 🏦 Bankly - Sistema de Simulação Bancária (Checkpoint 5)

## 👤 Integrante

Esse trabalho foi realizado individualmente.

- **Nome:** Luna de Carvalho Guimarães
- **RM:** 562290
- **Turma:** 2TDSPG

---

## 🎯 Domínio Escolhido

O domínio escolhido é o de **Simulação Bancária**, cujo objetivo é estruturar a operação de um banco digital gerenciando os dados cadastrais dos clientes, suas contas correntes ou poupança, cartões emitidos e histórico completo de movimentações financeiras.

> **Nota sobre o Domínio:** Diferente do tema abordado no CP1, este Checkpoint foca no sistema bancário **Bankly**. Essa mudança de escopo deve-se à alteração na formação do grupo (agora conduzido de forma individual), conforme alinhado previamente. Toda a estrutura de entidades, regras e mapeamentos de persistência foi construída do zero para este ecossistema.

---

## 🧱 Entidades Modeladas

1. **User:** Dados pessoais e credenciais de acesso do cliente (Nome, CPF, Email, Senha com Hash).
2. **Address:** Dados de endereço residencial vinculados obrigatoriamente a um usuário.
3. **AccountType:** Categoria da conta bancária cadastrada no sistema (ex.: Corrente, Poupança, Salário).
4. **Account:** Conta bancária ativa contendo agência, número único e saldo monetário.
5. **Card:** Cartões físicos ou virtuais associados a uma conta, com número, CVV, validade e flag de bloqueio/desbloqueio.
6. **Transaction:** Registro imutável de movimentação financeira (Depósito, Saque ou Transferência).

---

## 🔗 Relacionamentos (MER)

- **User (1:1) Address:** Um usuário possui um único endereço de cadastro.
- **User (1:N) Account:** Um usuário pode ser titular de múltiplas contas bancárias.
- **AccountType (1:N) Account:** Um tipo de conta categoriza diversas contas do sistema.
- **Account (1:N) Card:** Uma conta bancária pode conter múltiplos cartões vinculados.
- **Account (1:N) Transaction:** Uma conta mantém um histórico de várias transações financeiras.

---

## 💰 Regras de Negócio

- **Hash seguro de senhas:** Nenhuma credencial é persistida em texto puro. Toda senha recebida é criptografada utilizando o algoritmo **BCrypt** (`HashHelper`) antes da gravação.
- **Invariantes de saldo:**
    - `DEPOSITO`: Incrementa o saldo da conta com o valor informado.
    - `SAQUE` e `TRANSFERENCIA`: Realizam o débito apenas se houver saldo suficiente (`Balance >= Amount`). Caso contrário, é lançada uma `DomainException`.
    - Tipo de transação fora do enum (ex.: `99`) também lança `DomainException`, sem alterar o saldo.
- **Integridade cadastral:** CPF e e-mail são únicos no banco de dados. Tentativas de duplicidade disparam regras de negócio validadas pela aplicação.

---

## 🗄️ Banco de Dados

- **SGBD:** Oracle Database 19c
- **Provedor:** `Oracle.EntityFrameworkCore`
- **Ambiente:** Servidor Oracle FIAP

---

## 🏛️ Arquitetura da Solução

A solução segue os preceitos da **Clean Architecture**, dividida nos seguintes projetos:

- **`Bankly.Domain`:** Entidades (`Account`, `Transaction`, `User`, etc.), Enums, regras de negócio e validações de invariantes, sem dependência de frameworks externos de persistência.
- **`Bankly.Application`:** DTOs (Requests e Responses com Data Annotations, incluindo o envelope `PagedResponse<T>`), interfaces de repositório (`IGenericRepository<T>`, `ITransactionRepository`, etc.) e serviços de aplicação (`TransactionService`, que valida `page`/`pageSize`).
- **`Bankly.Infrastructure`:** Contexto do Entity Framework Core (`BanklyContext`), mapeamento Fluent API, migrations e implementações concretas dos repositórios (incluindo o `GetPaged` de `TransactionRepository`).
- **`Bankly.Api`:** Controllers REST, versionamento de API, rate limit, injeção de dependências, tratamento global de exceções, logs estruturados, Swagger por versão e Health Checks.
- **`Bankly.Domain.Tests`:** Projeto de testes de unidade para validação de regras de domínio (sem mock).
- **`Bankly.Application.Tests`:** Projeto de testes de unidade para a camada de aplicação.

---

## 🚀 Como Executar o Projeto

### Pré-requisitos

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) instalado.
- Acesso à rede corporativa/VPN da FIAP para comunicação com o Oracle Database.

### Passos de Execução

1. Clone o repositório em sua máquina:
```bash
   git clone <URL_DO_REPOSITORIO>
   cd Bankly
```

2. Configure a connection string no arquivo `appsettings.Development.json` do projeto `Bankly.Api` (use o seu RM e a sua senha; **não commite credenciais reais**):

```json
   {
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=oracle.fiap.com.br)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl)));User Id=SEU_RM;Password=SUA_SENHA;"
  }
}
```

Alternativa sem escrever a senha em arquivo do repositório (User Secrets):

```bash
   dotnet user-secrets init --project src/Bankly.Api
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<connection string>" --project src/Bankly.Api
```

3. Restaure as dependências e execute a aplicação:

```bash
   dotnet restore
   dotnet run --project src/Bankly.Api
```

### URLs de Acesso

| Recurso | HTTP (perfil `http`) | HTTPS (perfil `https`) |
| --- | --- | --- |
| Swagger UI | `http://localhost:5136/swagger` | `https://localhost:7186/swagger` |
| Health Check | `http://localhost:5136/health` | `https://localhost:7186/health` |
| **Listagem de transações v1 (deprecada)** | `http://localhost:5136/api/transaction?api-version=1.0` | `https://localhost:7186/api/transaction?api-version=1.0` |
| **Listagem de transações v2 (paginada)** | `http://localhost:5136/api/transaction?api-version=2.0` | `https://localhost:7186/api/transaction?api-version=2.0` |
| Listagem sem versão (cai na v2) | `http://localhost:5136/api/transaction` | `https://localhost:7186/api/transaction` |

> No HTTPS local, o navegador pode exibir aviso de certificado. Para confiar nele, rode `dotnet dev-certs https --trust`. As evidências em `/docs` foram geradas pela URL HTTP.

---

## 🔀 Versionamento de API (CP5)

O recurso escolhido para versionar foi **Transaction** (`/api/transaction`), o que mais cresce no domínio. Os pacotes usados são `Asp.Versioning.Mvc` e `Asp.Versioning.Mvc.ApiExplorer`.

### Configuração

- `DefaultApiVersion = 2.0`
- `AssumeDefaultVersionWhenUnspecified = true` (requisição sem versão cai na **2.0**)
- `ReportApiVersions = true` (a resposta envia os headers `api-supported-versions` e `api-deprecated-versions`)
- Leitores combinados (`ApiVersionReader.Combine`): query string `api-version` e header `X-Api-Version`.

### As duas versões

| Versão | Situação | `GET /api/transaction` devolve |
| --- | --- | --- |
| **1.0** | **Deprecada** (`[ApiVersion("1.0", Deprecated = true)]`) | Lista simples (contrato antigo do CP3), **sem paginação** |
| **2.0** | Atual (padrão) | Envelope paginado (`PagedResponse`) |

As duas versões chamam o **mesmo** `ITransactionService`. Nenhuma regra de negócio foi duplicada por versão. O `POST /api/transaction` responde nas duas versões (com ou sem versão informada).

### Como informar a versão

```http
GET /api/transaction?api-version=1.0
```

```http
GET /api/transaction
X-Api-Version: 1.0
```

```http
GET /api/transaction
```

O terceiro caso (sem versão) responde com a **v2** (envelope paginado). Não foi implementado o leitor por segmento de URL (item recomendado, não obrigatório).

### Swagger por versão

Em Development, o Swagger tem um documento por versão (`GroupNameFormat = 'v'VVVV`): **v1.0** e **v2.0**. A descrição do documento da v1 informa que a versão está **deprecada**. A UI permite alternar entre os dois grupos.

### Demais controllers

`User`, `Account`, `Card`, `Address` e `AccountType` **não foram versionados**. Para que continuassem aparecendo no Swagger e respondendo depois de ligar o ApiExplorer, receberam `[ApiVersionNeutral]`. Eles seguem chamáveis exatamente como no CP3/CP4.

---

## 📄 Paginação (CP5, somente na listagem v2)

`GET /api/transaction?page=1&pageSize=20`

| Parâmetro | Padrão | Regra |
| --- | --- | --- |
| `page` | `1` | inteiro ≥ 1 |
| `pageSize` | `20` | inteiro de **1 a 100** (teto: **100**) |

### Resposta 200 (envelope)

```json
{
  "page": 1,
  "pageSize": 20,
  "totalItems": 137,
  "totalPages": 7,
  "items": [],
  "hasPrevious": false,
  "hasNext": true
}
```

- `totalPages` = teto de `totalItems / pageSize`.
- `page` além do total devolve **200** com `items` vazio (não é erro).
- `page < 1` ou `pageSize` fora de 1–100 devolve **400** em `application/problem+json`, com a regra violada no campo `detail`. A validação está no `TransactionService` (Application), que lança `DomainException`, tratada pelo `GlobalExceptionHandler`. O serviço valida o `page` primeiro e para no primeiro erro.

### Onde cada camada entra

- **Controller:** lê `page` e `pageSize`.
- **Application:** `TransactionService.GetPaged` valida o intervalo, pede a página ao repositório e monta o envelope (`PagedResponse<T>`, que fica em Application).
- **Infrastructure:** `TransactionRepository.GetPaged` executa `Count` + `OrderBy` + `Skip` + `Take` no `IQueryable` e só então materializa com `ToList()`. A ordenação é por `CreatedAt` com `Id` como desempate, para a página 1 e a página 2 nunca se sobreporem.

A **v1 não pagina**: preserva o contrato antigo (lista completa).

---

## 🚦 Rate Limit (CP5)

Middleware nativo do ASP.NET Core (`Microsoft.AspNetCore.RateLimiting`), sem pacote de terceiros.

| Item | Valor |
| --- | --- |
| Política | `escrita` (fixed window) |
| Endpoint limitado | `POST /api/transaction` |
| Limite | **10 requisições** |
| Janela | **1 minuto** |
| Fila | 0 (excedente é rejeitado na hora) |
| Partição | Global (o contador é compartilhado por todos os clientes) |

### O que acontece no estouro

A 11ª requisição dentro da janela recebe:

- **Status 429** (Too Many Requests);
- header **`Retry-After`** com os segundos até poder tentar de novo;
- corpo JSON (Problem Details) com `status: 429` e a mensagem do limite.

### Ordem do pipeline

`UseExceptionHandler()` → `UseHttpsRedirection()` → `UseRateLimiter()` → `UseAuthorization()` → `MapControllers()`.

### `/health` fora do teto

`GET /health` usa `DisableRateLimiting()`. Depois de estourar o `POST`, o `/health` continua respondendo **200**.

> Observação: o limitador roda antes do controller, então toda requisição ao `POST` conta na janela, inclusive as que depois resultam em 400 ou 404.

---

## 🩺 Health Checks (`GET /health`)

A aplicação expõe a rota única **`GET /health`** configurada com formatador JSON customizado (`HealthCheckResponseWriter`), retornando o status geral, duração e o diagnóstico de cada dependência registrada:

- **`self`:** Validação do processo da API em execução (`HealthCheckResult.Healthy`).
- **`oracle-db`:** Verificação de conectividade com a base Oracle via `AddDbContextCheck<BanklyContext>`.

### Códigos HTTP de Resposta

- **200 OK:** Para status `Healthy` ou `Degraded` (serviço operacional).
- **503 Service Unavailable:** Para status `Unhealthy` (quando o banco ou o processo caem).
- **Tratamento por ambiente:** Detalhes de exceção técnica (`error`) só são serializados no JSON quando em ambiente `Development`.

---

## 📊 Observabilidade e Logs Estruturados

A observabilidade foi implementada utilizando `ILogger<T>` nativo com correlação baseada no identificador único de requisição (`HttpContext.TraceIdentifier`):

- **Fluxo de Escrita (`TransactionController`):**
    - Log de início da operação com propriedades nomeadas: `{TraceId}`, `{AccountId}`, `{Amount}`, `{Type}`.
    - Log de sucesso ao persistir: `{TraceId}`, `{TransactionId}`.
- **Tratamento de Exceções (`GlobalExceptionHandler`):**
    - Captura qualquer erro não tratado e registra em nível `Error` contendo `{TraceId}`, `{Path}` e `{Message}`.
    - Em ambiente de desenvolvimento, o `traceId` é injetado diretamente nas extensões do **`ProblemDetails`** (RFC 7807).

---

## 🧪 Testes Automatizados (xUnit)

1. **`Bankly.Domain.Tests`** (sem mock, 11 testes):
    - Testa regras reais de domínio na entidade `Account` (saldo inicial válido e negativo, saque com saldo suficiente, saque com saldo insuficiente, tipo de transação inválido) e `Transaction` (valor válido, zerado ou negativo).
    - Utiliza padrão **AAA** (Arrange, Act, Assert), métodos com **`[Fact]`** e cenários parametrizados com **`[Theory]`** + **`[InlineData]`**.
2. **`Bankly.Application.Tests`** (14 testes):
    - **`TransactionServiceTests`** (CP4): testa o `TransactionService.Create` isolando as dependências de banco com **Moq** (`ITransactionRepository` e `IAccountRepository`): conta inexistente lança `KeyNotFoundException` e não persiste nada (`Times.Never`); caminho feliz atualiza o saldo e persiste uma vez (`Times.Once`).
    - **`TransactionServicePagingTests`** (CP5): cobre a paginação sem subir API nem banco, usando um repositório falso escrito à mão:
        - `[Theory]` + `[InlineData]` para `page` / `pageSize` inválidos (`page` 0 ou negativo; `pageSize` 0, negativo ou acima de 100), que devem lançar `DomainException` sem consultar o repositório;
        - `[Theory]` para o intervalo válido e `[Fact]` para o cálculo de `totalPages`, `hasPrevious` e `hasNext`.

### Executando os Testes

A partir da raiz da solução, execute o comando (com a API parada):

```bash
dotnet test
```

Resultado atual: **25 testes (11 Domain + 14 Application), 0 falhas**.

---

## ⚠️ Tabela de Mapeamento de Exceções

| **Exceção** | **Status HTTP** | **Descrição / Causa** |
| --- | --- | --- |
| `DomainException` | `400 Bad Request` | Violação de regra de negócio (saldo inicial negativo, saque sem saldo suficiente, valor inválido, tipo de transação inválido, `page`/`pageSize` fora da faixa). |
| `KeyNotFoundException` | `404 Not Found` | Recurso solicitado não foi encontrado (conta inexistente na transação). |
| `Exception` (demais erros) | `500 Internal Server Error` | Erros inesperados não tratados, mascarados no payload para proteção em produção. |

O **429** não vem de exceção: é devolvido pelo middleware de rate limit (`OnRejected`), antes de chegar ao controller.

---

## 📁 Evidências de Testes e Operação (`/docs`)

**CP4:**

- `health-healthy.png` — Resposta 200 OK de `/health` com API e Oracle operacionais.
- `health-unhealthy.png` — Resposta 503 Service Unavailable de `/health` simulando queda do banco.
- `log-transacao-sucesso.png` — Console da API registrando logs estruturados com `traceId` no `POST /api/transaction`.
- `log-exception-handler.png` — Console registrando log de erro no `GlobalExceptionHandler` com `traceId`.
- `problemdetails-erro.png` — Resposta ProblemDetails contendo a extensão de correlação `traceId`.
- `swagger-endpoints-*.png` — Interface do Swagger documentando os endpoints da API.

**CP5:**

- `get-v1-lista.json` — `GET /api/transaction?api-version=1.0` (lista simples).
- `get-v2-envelope.json` — `GET /api/transaction?api-version=2.0` (envelope paginado).
- `headers-versao.png` — Headers `api-supported-versions` e `api-deprecated-versions` na resposta da v1.
- `swagger-v1-deprecada.png` — Swagger na V1.0, com o aviso de versão deprecada na descrição do documento.
- `swagger-v2-dropdown.png` — Swagger na V2.0, com o seletor aberto mostrando os dois grupos (V2.0 e V1.0).
- `paginacao-400.png` — 400 para `page=0` (`pageSize=20`), com a regra do `page` no campo `detail`.
- `paginacao-400-pagesize.png` — 400 para `page=1&pageSize=9999`, com a regra do `pageSize` (entre 1 e 100) no campo `detail`.
- `paginacao-pagina-1.png` — Página 1 (`page=1&pageSize=2`), com `totalItems` e `totalPages` coerentes.
- `paginacao-pagina-2.png` — Página 2 (`page=2&pageSize=2`), itens diferentes dos da página 1 (sem sobreposição).
- `rate-limit-429.png` — 429 no `POST /api/transaction` com `Retry-After` e corpo JSON.
- `health-apos-429.png` — `GET /health` respondendo 200 depois do estouro (prova de que o probe não divide o teto).
- `testes-cp5-xunit.png` — Execução do `dotnet test` com os 25 testes passando em verde.