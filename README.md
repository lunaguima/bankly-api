# 🏦 Bankly - Sistema de Simulação Bancária (Checkpoint 2)

## 👤 Integrante
Esse trabalho foi feito individualmente.
* **Nome:** Luna de Carvalho Guimarães
* **RM:** 562290
* **Turma:** 2TDSPG

## 🎯 Domínio Escolhido
O domínio escolhido é o de **Simulação Bancária**, onde a aplicação possui como objetivo estruturar um banco digital, gerenciando os dados dos usuários, suas respectivas contas, além dos seus cartões vinculados e dos seus históricos de transações financeiras.

**Nota sobre o Domínio:** Diferente do tema abordado no CP1, este Checkpoint foca no sistema bancário Bankly. Essa mudança de escopo deve-se à alteração na formação do grupo (agora conduzido de forma individual), conforme autorizado previamente via Microsoft Teams. Ressalto que toda a estrutura de entidades e mapeamentos de persistência foi reconstruída do zero para este novo cenário.


## 🧱 Entidades Modeladas
As entidades modeladas no sistema foram:
1. **User:** Onde ficam guardados os dados pessoais do cliente (Nome, CPF, Email).
2. **Address:** Registra os dados da localização do usuário.
3. **AccountType:** É a entidade que define a categoria da conta bancária (ex: Conta Corrente, Poupança, etc.).
4. **Account:** Os dados da conta bancária em si (Agência, Número, Saldo).
5. **Card:** Contém as informações dos cartões físicos ou virtuais vinculados à conta.
6. **Transaction:** Mantém os registros das movimentações financeiras (Saques, depósitos, transferências).

## 🔗 Resumo dos Relacionamentos
* **User (1:1) Address:** Um relacionamento obrigatório onde um usuário possui apenas um endereço de cadastro.
* **User (1:N) Account:** O usuário pode ser titular de várias contas no banco.
* **AccountType (1:N) Account:** Um tipo de conta serve de modelo para múltiplas contas cadastradas.
* **Account (1:N) Card:** Uma mesma conta bancária pode ter vários cartões (ex: crédito, débito, virtual) atrelados a ela.
* **Account (1:N) Transaction:** A conta concentra um histórico de N transações realizadas ao longo do tempo.

## 🗄️ SGBD utilizado
Oracle Database (Acessado via SQL Developer).

## 🚀 Como Executar o Projeto
Para garantir a reprodução do ambiente utilizando o Oracle, siga os passos abaixo:

1. Clonar o repositório na sua máquina.
2. Na parte do projeto `Bankly.Api`, abra o arquivo `appsettings.Development.json`.
3. Na chave `DefaultConnection`, substitua os valores `MEU_RM` e `MINHA_SENHA` pelas suas credenciais válidas.
4. Depois abra o terminal ou console do Gerenciador de Pacotes apontando para a pasta do projeto **`Bankly.Infrastructure`**.
5. Execute o comando abaixo para aplicar as *migrations* e materializar as tabelas no seu banco de dados Oracle:
   `dotnet ef database update`
6. Após a atualização, execute o projeto Bankly.Api. Em seguida, acesse o link http://localhost:5136/swagger no seu navegador para abrir a interface interativa do programa Swagger e testar os endpoints.