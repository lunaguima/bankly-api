# 🏦 Bankly - Sistema de Simulação Bancária (Checkpoint 4)

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
- **`Bankly.Application`:** DTOs (Requests e Responses com Data Annotations), interfaces de repositório (`IGenericRepository<T>`, `IAccountRepository`, etc.) e serviços de aplicação (`TransactionService`).
- **`Bankly.Infrastructure`:** Contexto do Entity Framework Core (`BanklyContext`), mapeamento Fluent API, migrations e implementações concretas dos repositórios.
- **`Bankly.Api`:** Controllers REST, injeção de dependências, tratamento global de exceções, observabilidade com logs estruturados e configuração de Health Checks.
- **`Bankly.Domain.Tests`:** Projeto de testes de unidade para validação de regras de domínio (sem mock).
- **`Bankly.Application.Tests`:** Projeto de testes de unidade para a camada de aplicação utilizando mocks (Moq).

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

2. Configure a connection string no arquivo `appsettings.Development.json` do projeto `Bankly.Api`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=oracle.fiap.com.br)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl)));User Id=SEU_RM;Password=SUA_SENHA;"
     }
   }
   ```

3. Restaure as dependências e execute a aplicação:

   ```bash
   dotnet restore
   dotnet run --project Bankly.Api
   ```

### URLs de Acesso

- **Swagger UI:** `https://localhost:7041/swagger` (ou `http://localhost:5136/swagger`)
- **Health Check:** `https://localhost:7041/health` (ou `http://localhost:5136/health`)

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

A cobertura de testes automatizados contempla a base e o meio da pirâmide de testes:

1. **`Bankly.Domain.Tests`** (Sem Mock):
  - Testa regras reais de domínio na entidade `Account` (crédito, débito, saldo insuficiente) e `Transaction` (valor zerado ou negativo).
  - Utiliza padrão **AAA** (Arrange, Act, Assert), métodos com **`[Fact]`** e cenários parametrizados com **`[Theory]`** + **`[InlineData]`**.
2. **`Bankly.Application.Tests`** (Com Mock):
  - Testa o serviço de aplicação `TransactionService` isolando as dependências de banco com **Moq** (`ITransactionRepository` e `IAccountRepository`).
  - Valida que falha por dependência ausente lança `KeyNotFoundException` e **não** chama os métodos de persistência (`Times.Never`).
  - Valida o caminho feliz persistindo a transação e atualizando a conta uma única vez (`Times.Once`).

### Executando os Testes

A partir da raiz da solução, execute o comando:

```bash
dotnet test
```

---

## ⚠️ Tabela de Mapeamento de Exceções

| **Exceção** | **Status HTTP** | **Descrição / Causa** |
| --- | --- | --- |
| `DomainException` | `400 Bad Request` | Violação de regra de negócio (saldo inicial negativo, saque sem saldo suficiente, valor inválido). |
| `KeyNotFoundException` | `404 Not Found` | Recurso solicitado não foi encontrado (conta inexistente na transação). |
| `Exception` (demais erros) | `500 Internal Server Error` | Erros inesperados não tratados, mascarados no payload para proteção em produção. |

---

## 📁 Evidências de Testes e Operação (`/docs`)

As capturas de tela comprovando o funcionamento da entrega encontram-se na pasta `/docs`:

- `health-healthy.png` — Resposta 200 OK de `/health` com API e Oracle operacionais.
- `health-unhealthy.png` — Resposta 503 Service Unavailable de `/health` simulando queda do banco.
- `log-transacao-sucesso.png` — Console da API registrando logs estruturados com `traceId` no `POST /api/transaction`.
- `log-exception-handler.png` — Console registrando log de erro no `GlobalExceptionHandler` com `traceId`.
- `problemdetails-erro.png` — Resposta ProblemDetails contendo a extensão de correlação `traceId`.
- `testes-xunit.png` — Execução do `dotnet test` com todos os testes passando em verde.
- `swagger-endpoints-*.png` — Interface do Swagger documentando os endpoints da API.