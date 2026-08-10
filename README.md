# 🏦 Bankly - Sistema de Simulação Bancária (Checkpoint 3)

## 👤 Integrante
Esse trabalho foi feito individualmente.

- **Nome:** Luna de Carvalho Guimarães
- **RM:** 562290
- **Turma:** 2TDSPG

## 🎯 Domínio Escolhido

O domínio escolhido é o de **Simulação Bancária**, onde a aplicação possui como objetivo estruturar um banco digital, gerenciando os dados dos usuários, suas respectivas contas, além dos seus cartões vinculados e dos seus históricos de transações financeiras.

> **Nota sobre o Domínio:** Diferente do tema abordado no CP1, este Checkpoint foca no sistema bancário **Bankly**. Essa mudança de escopo deve-se à alteração na formação do grupo (agora conduzido de forma individual), conforme autorizado previamente via Microsoft Teams. Ressalto que toda a estrutura de entidades e mapeamentos de persistência foi reconstruída do zero para este novo cenário.

---

# 🧱 Entidades Modeladas

As entidades modeladas no sistema foram:

1. **User:** Onde ficam guardados os dados pessoais do cliente (Nome, CPF, Email, Senha).
2. **Address:** Registra os dados da localização do usuário.
3. **AccountType:** Define a categoria da conta bancária (Conta Corrente, Poupança, etc.).
4. **Account:** Armazena os dados da conta bancária (Agência, Número e Saldo).
5. **Card:** Contém as informações dos cartões físicos ou virtuais vinculados à conta.
6. **Transaction:** Mantém o histórico das movimentações financeiras (Saques, Depósitos e Transferências).

---

# 🔗 Resumo dos Relacionamentos

- **User (1:1) Address:** Um usuário possui apenas um endereço de cadastro.
- **User (1:N) Account:** Um usuário pode possuir várias contas bancárias.
- **AccountType (1:N) Account:** Um tipo de conta pode ser utilizado por diversas contas.
- **Account (1:N) Card:** Uma conta pode possuir vários cartões (crédito, débito e virtual).
- **Account (1:N) Transaction:** Uma conta mantém um histórico de diversas transações financeiras.

---

# 💰 Regras de Negócio

- **Senha do usuário:** nunca é armazenada em texto puro. Toda senha recebida na criação ou atualização do usuário é transformada em hash com **BCrypt** antes de ser persistida no banco.
- **Saldo da conta:** é atualizado automaticamente a cada transação registrada.
  - `DEPOSITO` soma o valor ao saldo da conta.
  - `SAQUE` e `TRANSFERENCIA` subtraem o valor do saldo, desde que haja saldo suficiente.
  - Caso o saldo seja insuficiente para um saque ou transferência, a operação é rejeitada com uma exceção de domínio, retornada como `400 Bad Request` via `ProblemDetails`.
- **CPF e Email únicos:** o sistema impede o cadastro de um usuário com CPF já existente, lançando uma exceção de domínio tratada pelo `GlobalExceptionHandler`.

---

# 🗄️ Banco de Dados

- **SGBD:** Oracle Database
- **Ferramenta:** Oracle SQL Developer
- **Ambiente:** Oracle FIAP

---

# 🌐 API REST (Evolução do Checkpoint 3)

A persistência desenvolvida no CP2 foi exposta através de uma **API REST**, utilizando uma arquitetura baseada em separação de responsabilidades.

### Estrutura do Projeto

- **Bankly.Api**
  - Controllers
  - Configuração do Swagger
  - Tratamento global de exceções

- **Bankly.Application**
  - DTOs de Request e Response
  - Interfaces dos Repositórios

- **Bankly.Domain**
  - Entidades
  - Exceções de domínio
  - Regras de negócio
  - Hash de senha (BCrypt)

- **Bankly.Infrastructure**
  - DbContext
  - Migrations
  - Implementação dos repositórios utilizando Entity Framework Core

Os Controllers **não acessam diretamente o DbContext**, mantendo toda a persistência encapsulada nos repositórios.

---

# 📘 Swagger

A documentação da API está disponível em ambiente de desenvolvimento:

```
http://localhost:5136/swagger
```

O Swagger possui:

- Título e descrição personalizados
- Versão da API
- Comentários XML em todas as actions
- Documentação dos códigos HTTP
  - 200 OK
  - 400 Bad Request
  - 404 Not Found

---

# 🗂️ Repositório Genérico

Foi implementado um repositório genérico composto por:

- `IGenericRepository<T>`
- `GenericRepository<T>`

Disponibilizando os métodos:

- GetAll()
- GetById()
- Add()
- Update()
- Delete()

O serviço foi registrado na Injeção de Dependência através de:

```csharp
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
```

Os repositórios de:

- Account
- AccountType
- Address
- Card
- Transaction

herdam do repositório genérico.

Já o repositório de **User** implementa uma interface específica (`IUserRepository`), pois realiza o cadastro do usuário juntamente com seu endereço em uma única operação transacional (se qualquer etapa falhar, nada é gravado).

---

# 🛑 Tratamento Global de Exceções

Foi implementado um **GlobalExceptionHandler**, utilizando `IExceptionHandler`, retornando respostas no padrão **ProblemDetails (RFC 7807)**.

| Exceção | HTTP Status |
|----------|-------------|
| DomainException | 400 Bad Request |
| KeyNotFoundException | 404 Not Found |
| Demais exceções | 500 Internal Server Error |

Para erros internos, a API retorna apenas uma mensagem genérica, sem expor detalhes internos da aplicação.

Exemplo real de resposta ao tentar sacar um valor maior que o saldo disponível:

```json
{
  "type": "https://httpstatuses.com/400",
  "title": "Erro de validação",
  "status": 400,
  "detail": "Saldo insuficiente para realizar a operação."
}
```

---

# 🚀 Como Executar o Projeto

## 1. Clonar o Repositório

```bash
git clone <url-do-repositorio>
```

---

## 2. Configurar o User Secrets

Por segurança, a Connection String **não está presente no appsettings.json**.

Dentro da pasta **Bankly.Api**, execute:

```bash
cd src/Bankly.Api

dotnet user-secrets init

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=oracle.fiap.com.br)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl)));User Id=SEU_RM;Password=SUA_SENHA;"
```

Substitua:

- `SEU_RM`
- `SUA_SENHA`

pelas suas credenciais do Oracle.

---

## 3. Aplicar as Migrations

Na raiz do projeto execute:

```bash
dotnet ef database update --project src/Bankly.Infrastructure --startup-project src/Bankly.Api
```

---

## 4. Executar a API

```bash
dotnet run --project src/Bankly.Api
```

---

## 5. Testar a API

Abra o navegador e acesse:

```
http://localhost:5136/swagger
```

Os endpoints disponíveis são:

- Users
- Addresses
- AccountTypes
- Accounts
- Cards
- Transactions

Todos podem ser testados diretamente pela interface do Swagger.

### Fluxo sugerido de teste

1. `POST /api/AccountType` — cria um tipo de conta.
2. `POST /api/User` — cria um usuário com endereço.
3. `POST /api/Account` — cria uma conta vinculada ao usuário e tipo de conta criados.
4. `POST /api/Transaction` com `type: "DEPOSITO"` — deposita um valor e confere o saldo em `GET /api/Account/{id}`.
5. `POST /api/Transaction` com `type: "SAQUE"` maior que o saldo disponível — confirma o retorno `400 Bad Request` com `ProblemDetails`.

---

# ✅ Tecnologias Utilizadas

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- Oracle Database
- Oracle SQL Developer
- Swagger / OpenAPI
- Dependency Injection
- Clean Architecture
- Repository Pattern
- User Secrets
- ProblemDetails (RFC 7807)
- BCrypt.Net-Next (hash de senha)

---

# 📌 Observações

Este projeto foi desenvolvido como parte do **Checkpoint 3** da disciplina de **Advanced Business Development with .NET**, seguindo os requisitos propostos para a persistência dos dados, API REST, documentação via Swagger, tratamento global de exceções e boas práticas de arquitetura.