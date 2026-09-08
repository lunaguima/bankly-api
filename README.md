# 🏦 Bankly - Sistema de Simulação Bancária (Checkpoint 4)

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
  - Health checks (`/health`)

- **Bankly.Application**
  - DTOs de Request e Response
  - Interfaces dos Repositórios (pasta `Repositories/`)
  - `TransactionService` (regra de aplicação da criação de transação)

- **Bankly.Domain**
  - Entidades
  - Exceções de domínio
  - Regras de negócio
  - Hash de senha (BCrypt)

- **Bankly.Infrastructure**
  - DbContext
  - Migrations
  - Implementação dos repositórios utilizando Entity Framework Core

- **Bankly.Domain.Tests**
  - Testes unitários das regras de negócio do Domain, sem mock

- **Bankly.Application.Tests**
  - Testes unitários do `TransactionService`, com mock dos repositórios (Moq)

Os Controllers **não acessam diretamente o DbContext**, mantendo toda a persistência encapsulada nos repositórios.

---

# 📘 Swagger

A documentação da API está disponível em ambiente de desenvolvimento: