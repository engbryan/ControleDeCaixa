## Desenvolvimento e Conceito de MVP

A aplicação foi desenvolvida do zero, seguindo o conceito de Minimum Viable Product (MVP), com foco na entrega rápida e eficiente, devido ao curto prazo disponível para a execução do projeto. Durante o desenvolvimento, não foram utilizadas assinaturas cloud pré-existentes, nem foram reaproveitados trechos de código de projetos anteriores. O documento de especificação do teste não solicitou uma arquitetura específica, como cloud, nem mencionou a necessidade de acessos fora da rede corporativa. Além disso, o caso de uso dos usuários não foi claramente definido (se o acesso seria realizado via smartphones, computadores, rede interna ou externa), deixando a critério do arquiteto a utilização do tempo limitado para atender à demanda.

## Premissas da Arquitetura

Na concepção da arquitetura, foram adotadas apenas duas premissas:
- Os usuários usam apenas computadores para POS;
- Os usuários estão integrados ao Active Directory (AD) da empresa;

Essas decisões foram tomadas para simplificar o gerenciamento de usuários e autenticação, sem a necessidade de implementar soluções complexas de arquitetura e segurança no MVP.

## Propósito da Solução

O propósito desta solução foi demonstrar uma visão de escalabilidade e resiliência, utilizando os recursos disponíveis no momento de sua concepção. A arquitetura foi projetada para ser facilmente adaptável a uma infraestrutura serverless ou para fazer uso de mensageria, conforme necessário. Embora a solução atual seja básica, ela foi pensada para permitir melhorias e evoluções de forma simples e eficiente.

## Melhorias Potenciais

### Escalabilidade

A solução atual utiliza eventos, que podem ser facilmente substituídos por notificações seguindo o padrão Mediator. Isso permitiria uma melhor organização do código e a separação de responsabilidades.

### Resiliência

A arquitetura modular e a separação de responsabilidades possibilitam a expansão da solução para mensageria ou microserviços, caso seja necessário aumentar a robustez e a capacidade de resposta do sistema.

### Segurança

A aplicação faz uso de injeção de dependências de forma modular, o que facilita a adição de mecanismos de autenticação e segurança mais avançados, conforme necessário.

### Idioma

Inicialmente foi utilizado o português para nomes de negócio, e inglês para os demais termos técnicos e nomenclarutas em geral, porém em dado momento a utilização do português se tornou mais frequente, sendo uma má prática.

## Versões

A versão 6.0 do .Net foi escolhida para o POS com objetivo oferecer maior compatibilidade com as instalações nos ambientes cliente. Uma versão mais nova teria um nicho de prontidão mais reduzido, obrigando uma parcela maior clientes a fazer atualizações.

## Autenticação e Segurança

Nesta versão, a autenticação no formulário foi omitida, ficando a cargo dos mecanismos padrões do sistema operacional (Windows). Recomenda-se que o usuário utilize o bloqueio de tela do Windows para evitar acessos não autorizados, da mesma forma que se protege o acesso ao Teams, E-mail e outras ferramentas corporativas durante pausas no uso. Também é comum que POS não possuam bloqueios de tela e permaneçam acessíveis em tempo integral propositalmente.

## Uso de Mediator Pattern

O desenvolvimento atual inclui eventos que podem ser substituídos pelas notificações do padrão Mediator, utilizando a biblioteca Mediatr. A separação de responsabilidades na aplicação também permite a implementação dos handlers do Mediator, o que facilitaria a manutenção e a expansão futura da solução. Devido ao foco no MVP, esses recursos foram considerados como melhorias futuras e não foram implementados nesta versão inicial.

## Estrutura

Para este teste foi pensado em levantar um container docker com SQLServer publicado no NGrok para funcionar como o SQLServer central e demonstrar o funcionamento de dois ControleLancamentos simultaneamente, simulando dois usuários. O serviço de sincronia precisaria ser reescrito para poder se comunicar com este SQLServer. O SSRS precisaria ser ativado em um novo container. O relatório precisaria ser criado. Por escassez de tempo, estas etapas não estão sendo atendidas neste MVP.

# Solução

## Controle de Lançamentos

É oferecido um link via email que permita aos usuários baixar o formulário `configurado`. Este link baixa o Controle de Lançamentos que extrai a identificação do usuário logado do Windows e se utiliza da autenticação integrada do sistema operacional 'Integrated Security=true' que é exclusiva para o usuário, permitindo a execução segura e a realização das operações desejadas, atendendo a múltiplas instâncias de uso simultâneas.

Alternativamente, a identificação da máquina juntamente com o login do usuário também pode ser obtida para usuários externos e gerar uma chave única possibilitando o acesso deste tipo de usuário. Estas chaves serão armazenadas no Credential Manager do Windows e também se beneficiam da autenticação integrada segura para a operação.

A aplicação é equipada com um banco de dados local SQLite para armazenar os lançamentos enquanto a conexão com o serviço online estiver indisponível. Também há um banco de dados SQLServer na nuvem com failover e mirroring, onde todos os lançamentos seriam sincronizados utilizando uma ConnectionString. Isso eliminaria a necessidade de desenvolver serviços adicionais, como módulos de autenticação, logging, API gateways, caches, etc., na versão inicial, favorecendo a simplicidade, performance e eficiência ao utilizar estes mecanismos já oferecidos pelo SQL Server.

A concessão de permissões é gerida pelo time de banco de dados, devendo ser abertos tickets para que usuários tenham seus acessos concedidos ou removidos. O ticket deve ser criado quando a useraccount do usuário no AD já existe, o que pode ser feito após o time de infraestrutura adicionar a conta no AD. Esta abertura de ticket precisa estar listada no handbook dos gestores sobre ingresso e saída de colaboradores com esta matriz de acesso.

## Consolidação e Relatórios

Todos os dados provenientes dos usuários seriam centralizados nesse banco de dados central (na nuvem ou on-premises), permitindo a geração de relatórios consolidados diários diretamente no SQL Server Reporting Services (SSRS). Esses relatórios poderiam ser exibidos no próprio formulário, em uma nova aba, com a opção de download ou impressão, simplificando o acesso às informações e a tomada de decisões.

Alternativamente, pode ser criada uma aplicação web em novo microserviço que forneça esses relatórios apenas aos usuários com perfil de gestão. Esta aplicação seria oferecida com uso de CDN, API Gateway, Autenticação, em uma arquitetura de Microserviços em uma plataforma Cloud.


# Documentação da Arquitetura do Aplicativo de Controle de Transações

## 1. Visão Geral do Projeto

O projeto é uma aplicação Windows Forms para gerenciamento de transações financeiras de um comerciante, que permite o lançamento de débitos e créditos. A aplicação é estruturada utilizando os princípios de injeção de dependências e é integrada a um serviço online para sincronização de dados.

## 2. Componentes Principais

### 2.1 MainForm
O `MainForm` é a interface gráfica principal da aplicação. Ele exibe os formulários para cadastro de transações e a lista de transações já registradas. Além disso, o formulário gerencia a interação com o usuário, exibe toasts e uma barra de status indicando a conectividade com o serviço online.

### 2.2 SynchronizeService
O `SynchronizeService` é responsável por sincronizar as transações locais com o serviço online. Ele verifica a conectividade através do `HealthCheckService` e, caso a conexão esteja disponível, envia as transações pendentes para o serviço online.

### 2.3 HealthCheckService
O `HealthCheckService` monitora a conectividade com o serviço online. Ele dispara eventos quando o status da conexão muda, o que permite ao `SynchronizeService` reagir automaticamente à reconexão.

### 2.4 EntryService
O `EntryService` lida com a lógica de negócio relacionada às transações, como a adição de novas transações e a obtenção de transações (sincronizadas e não sincronizadas). Ele interage com o repositório de transações para persistir e recuperar os dados.

### 2.5 IEntryApi
`IEntryApi` é uma interface que define os endpoints do serviço online com o qual a aplicação se comunica. Esta interface é implementada usando a biblioteca Refit, que simplifica as chamadas HTTP.

## 3. Fluxo de Dados e Interação

1. **Cadastro de Transações**: O usuário insere uma transação através do `MainForm`. O `EntryService` valida e armazena a transação no banco de dados local.
2. **Sincronização**: Periodicamente, o `SynchronizeService` verifica se há transações não sincronizadas e se o serviço online está disponível. Caso positivo, ele envia as transações e as marca como sincronizadas.
3. **Monitoramento de Conectividade**: O `HealthCheckService` verifica a conectividade em intervalos regulares. Se a conexão for perdida ou restabelecida, ele aciona eventos para notificar os serviços dependentes.

## 4. Dependências Externas

- **Entity Framework Core**: Utilizado para interação com o banco de dados local.
- **Refit**: Utilizado para simplificar as chamadas HTTP para o serviço online.
- **MaterialSkin**: Utilizado para criar uma interface gráfica moderna no Windows Forms.
- **Microsoft.Extensions.DependencyInjection**: Utilizado para configurar a injeção de dependências.

## 5. Cenários de Testes

### 5.1. Adicionar um Lançamento

**Cenário:** O usuário adiciona um novo lançamento do tipo "Débito".

- **Dado** que o usuário acessa a interface de adição de lançamentos,
- **Quando** o usuário insere as informações (Tipo: "Débito", Valor: 100, Descrição: "Compra de materiais") e confirma a adição,
- **Então** o lançamento deve ser salvo no banco de dados com o status `Synchronized = false`,
- **E** um evento `OnEntryAdded` deve ser disparado.

### 5.2. Sincronização de Lançamentos Não Sincronizados

**Cenário:** Sincronizar lançamentos não sincronizados com o serviço externo.

- **Dado** que existem lançamentos com `Synchronized = false` no banco de dados,
- **Quando** o serviço externo está disponível,
- **Então** o sistema deve enviar os lançamentos não sincronizados ao serviço externo,
- **E** marcar os lançamentos como sincronizados (`Synchronized = true`),
- **E** disparar o evento `OnSyncCompleted`.

### 5.3. Falha na Sincronização devido a Falta de Conexão

**Cenário:** O serviço externo não está disponível durante a tentativa de sincronização.

- **Dado** que o serviço externo está indisponível,
- **Quando** o sistema tenta sincronizar os lançamentos não sincronizados,
- **Então** o sistema deve capturar a exceção e logar o erro,
- **E** os lançamentos devem permanecer com o status `Synchronized = false`.

### 5.4. Verificação de Saúde do Serviço Externo

**Cenário:** Verificação da saúde do serviço externo.

- **Dado** que a aplicação está rodando,
- **Quando** o `HealthCheckService` realiza a verificação do serviço externo,
- **E** o serviço externo está disponível,
- **Então** o sistema deve registrar que o serviço está saudável (`IsServiceHealthy = true`),
- **E** disparar um evento `OnHealthStatusChanged` se o status de saúde mudou.

### 5.5. Sincronização Automática ao Restabelecer Conexão

**Cenário:** Sincronização automática de lançamentos após a conexão com o serviço externo ser restabelecida.

- **Dado** que o serviço externo estava indisponível e se torna disponível novamente,
- **Quando** o sistema detecta que o serviço externo está novamente saudável,
- **Então** todos os lançamentos não sincronizados devem ser sincronizados automaticamente,
- **E** o sistema deve marcar esses lançamentos como sincronizados.

### 5.6. Visualização de Lançamentos Sincronizados e Não Sincronizados

**Cenário:** O usuário visualiza lançamentos sincronizados e não sincronizados na interface.

- **Dado** que o usuário acessa a aba de listagem de lançamentos,
- **Quando** o usuário seleciona a opção "Todos",
- **Então** a lista deve exibir todos os lançamentos do banco de dados.
- **Quando** o usuário seleciona a opção "Sincronizados",
- **Então** a lista deve exibir apenas os lançamentos com `Synchronized = true`.
- **Quando** o usuário seleciona a opção "Não Sincronizados",
- **Então** a lista deve exibir apenas os lançamentos com `Synchronized = false`.

### 5.7. Atualização do Intervalo de Verificação de Saúde

**Cenário:** O intervalo de verificação de saúde do serviço externo é alterado.

- **Dado** que o intervalo de verificação de saúde é definido nas configurações,
- **Quando** o intervalo é alterado no arquivo `appsettings.json`,
- **Então** o sistema deve reconfigurar o `Timer` de verificação de saúde para o novo intervalo,
- **E** logar a reconfiguração do intervalo.


## 6. Padrões de Projeto, Desenvolvimento e Arquitetura

O projeto segue uma arquitetura modular, com clara separação de responsabilidades:

- **Injeção de Dependência**: Utilizada para gerenciar a criação e o ciclo de vida dos serviços.
- **Serviços**: Cada serviço (como `SynchronizeService` e `HealthCheckService`) é responsável por uma tarefa específica, promovendo a reutilização e testabilidade do código.
- **Eventos**: Utilizados para permitir a comunicação assíncrona entre componentes (por exemplo, mudanças no estado de conectividade).

## Padrões de Projeto e Princípios de Desenvolvimento

### 1. Padrão de Injeção de Dependência (Dependency Injection)
- **Descrição:** A injeção de dependência é um padrão de design usado para implementar a Inversão de Controle (IoC). Ela permite que as dependências de uma classe sejam fornecidas a partir do exterior, em vez de a própria classe criar suas dependências.
- **Exemplo no Projeto:** No projeto apresentado, a injeção de dependência é usada amplamente para passar instâncias de `IEntryRepository`, `IEntryApi`, `IHealthCheckService`, e `ILogger<T>`, entre outros, para os serviços que os consomem. Isso facilita o teste unitário, permitindo a substituição dessas dependências por mocks durante os testes.

### 2. Padrão de Repositório (Repository Pattern)
- **Descrição:** O padrão de repositório abstrai a camada de acesso a dados, fornecendo uma interface que encapsula a lógica necessária para acessar os dados de diferentes fontes.
- **Exemplo no Projeto:** `IEntryRepository` e sua implementação abstraem a lógica de acesso ao banco de dados, permitindo que o serviço `EntryService` interaja com os dados sem conhecer os detalhes da persistência, seguindo o princípio de separação de responsabilidades.

### 3. Padrão de Estratégia (Strategy Pattern)
- **Descrição:** Permite que uma classe ou método de comportamento específico seja alterado em tempo de execução, dependendo das condições ou do contexto.
- **Exemplo no Projeto:** Embora não implementado explicitamente, o uso de `IOptionsMonitor` para trocar URLs dinamicamente no `SynchronizeService` pode ser visto como uma aplicação simplificada do padrão de estratégia, onde a estratégia (URL do serviço) pode ser alterada sem modificar a lógica interna.

## Princípios SOLID

### 1. Single Responsibility Principle (SRP)
- **Descrição:** Cada classe deve ter uma única responsabilidade, ou seja, uma única razão para mudar.
- **Exemplo no Projeto:** A classe `HealthCheckService` tem a responsabilidade única de verificar o estado de saúde do serviço externo. Ela não se preocupa com a sincronização de dados ou com a manipulação de transações, o que mantém a lógica focada e fácil de manter.

### 2. Open/Closed Principle (OCP)
- **Descrição:** As classes devem ser abertas para extensão, mas fechadas para modificação.
- **Exemplo no Projeto:** A interface `IEntryApi` permite que diferentes implementações de APIs possam ser usadas sem alterar o código dos serviços que consomem essa API. Se uma nova API precisa ser integrada, uma nova implementação de `IEntryApi` pode ser criada, deixando o código do `SynchronizeService` inalterado.

### 3. Liskov Substitution Principle (LSP)
- **Descrição:** Objetos de uma classe derivada devem poder substituir objetos de sua classe base sem alterar a integridade do programa.
- **Exemplo no Projeto:** A classe `MockTransacaoRepository`, que implementa a interface `IEntryRepository` para fins de teste, pode substituir a implementação real sem causar problemas no funcionamento do `EntryService`. Isso garante que qualquer implementação de `IEntryRepository` possa ser usada de maneira intercambiável.

### 4. Interface Segregation Principle (ISP)
- **Descrição:** Uma classe não deve ser forçada a implementar interfaces que não utiliza.
- **Exemplo no Projeto:** A interface `IHealthCheckService` é específica para a verificação do estado do serviço online. Outras interfaces, como `ISynchronizeService`, não são obrigadas a implementar métodos de verificação de saúde, mantendo-as focadas em suas respectivas responsabilidades.

### 5. Dependency Inversion Principle (DIP)
- **Descrição:** Dependa de abstrações, não de concretizações.
- **Exemplo no Projeto:** O `SynchronizeService` depende da abstração `IEntryRepository` em vez de uma classe concreta. Isso permite que o repositório seja facilmente substituído por outra implementação, como um repositório em memória para testes, sem alterar o código do serviço.

## Princípios DRY, YAGNI e KISS

### 1. Don't Repeat Yourself (DRY)
- **Descrição:** Evite a duplicação de código, reutilizando componentes e abstrações.
- **Exemplo no Projeto:** O projeto adere ao princípio DRY ao utilizar serviços e repositórios compartilhados. Por exemplo, a lógica de acesso ao banco de dados é centralizada no repositório, e não duplicada em cada serviço que necessita dessa funcionalidade.

### 2. You Aren't Gonna Need It (YAGNI)
- **Descrição:** Evite adicionar funcionalidades até que elas sejam realmente necessárias.
- **Exemplo no Projeto:** O projeto parece seguir o princípio YAGNI, com uma implementação direta e focada no necessário para cumprir os requisitos funcionais. Não há complexidade adicional desnecessária ou código escrito para funcionalidades futuras incertas.

### 3. Keep It Simple, Stupid (KISS)
- **Descrição:** Simplicidade deve ser um objetivo chave no design, evitando complexidade desnecessária.
- **Exemplo no Projeto:** A simplicidade no design de serviços como `EntryService` e `SynchronizeService`, com responsabilidades claras e uma interface de fácil compreensão, exemplifica o princípio KISS. As decisões arquiteturais visam a clareza e a facilidade de manutenção.

## Padrão Triple A (Arrange, Act, Assert)

### 1. Descrição:
O padrão Triple A é uma abordagem comum em testes unitários que organiza o teste em três seções distintas:
  - **Arrange:** Configura os objetos e as condições iniciais necessárias para o teste.
  - **Act:** Executa a ação ou comportamento que está sendo testado.
  - **Assert:** Verifica se o resultado da ação está de acordo com o esperado.

### 2. Exemplo no Projeto:
Em todos os testes unitários do projeto, o padrão Triple A é seguido de maneira consistente. Por exemplo, no teste `Synchronize_DeveEnviarLancamentosNaoSincronizados_E_MarcarComoSincronizado`:
  - **Arrange:** O código configura os mocks para `IEntryRepository` e `IEntryApi`, definindo o comportamento esperado e preparando o serviço de sincronização.
  - **Act:** O método `Synchronize()` é chamado no serviço de sincronização.
  - **Assert:** Asserções são feitas para garantir que as transações foram marcadas como sincronizadas e que o método de sincronização foi chamado corretamente.


  # EntryControl

Este repositório contém o código-fonte para o projeto **EntryControl**. Ele inclui a aplicação principal e um conjunto de testes unitários desenvolvidos com xUnit. O projeto é construído usando .NET 6.0.

## Pré-requisitos

Antes de começar, certifique-se de ter o seguinte instalado em seu sistema:

- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/)
- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Git](https://git-scm.com/)

## Clonando o Repositório

1. Abra seu terminal ou prompt de comando.

2. Clone o repositório para o seu ambiente local usando o comando:

   ```bash
   git clone https://github.com/engbryan/controlelancamentos.git
   ```

3. Navegue até o diretório do projeto:

   ```bash
   cd EntryControl
   ```

## Estrutura do Projeto

O projeto está estruturado da seguinte forma:

```
EntryControl/
│
├── EntryControl/               # Aplicação principal
│   ├── Controllers/                  # Controladores da API
│   ├── Models/                       # Modelos de dados
│   ├── Repositories/                 # Repositórios de dados
│   ├── Services/                     # Serviços de negócios
│   ├── Providers/                    # Provedores de negócios
│   ├── Program.cs                    # Arquivo principal do aplicativo
│   └── ...                           # Outros arquivos da aplicação
│
└── EntryControl.Tests/         # Projeto de testes unitários
    ├── Services/                     # Testes de serviços
    ├── Repositories/                 # Testes de repositórios
    └── EntryControl.Tests.csproj  # Arquivo de projeto de testes
```

## Executando o Projeto

1. Abra o projeto em sua IDE favorita (Visual Studio ou Visual Studio Code).

2. Restaure as dependências do projeto:

   ```bash
   dotnet restore
   ```

3. Compile o projeto:

   ```bash
   dotnet build
   ```

4. Execute a aplicação:

   ```bash
   dotnet run --project EntryControl/EntryControl.csproj
   ```

   A aplicação deve iniciar e estar disponível em `http://localhost:5000` (ou outra porta configurada).

## Executando os Testes

1. Navegue até o diretório do projeto de testes:

   ```bash
   cd EntryControl.Tests
   ```

2. Execute os testes usando o comando:

   ```bash
   dotnet test
   ```

   Todos os testes devem ser executados e você verá o resultado no terminal.

