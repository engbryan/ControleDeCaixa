# Update 19/09/2024
A organização arquitetural do código da solução foi aprimorada em relação aos nomes de pastas, arquivos físicos, e desenho arquitetural.

Nesta etapa, algumas camadas foram separadas, separando fatidicamente o aplicativo POS da solução Cloud. Estes dois, agora, obedecem à um contrato.

## Diagrama de dependências
![image](https://github.com/user-attachments/assets/71799c07-24f6-4a15-be6e-0581d3585b57)

## Qualidade
![image](https://github.com/user-attachments/assets/3651108c-eaa1-4760-b20f-cd53a6aeea07)
(algumas issues não foram atendidas por questões de brevidade)

![image](https://github.com/user-attachments/assets/b0448dec-dd1f-4952-9495-8402abfe2f32)

## File System
![image](https://github.com/user-attachments/assets/e1cb09f2-315b-4bba-baa0-37dd39ac2b6e)

## Solução
![image](https://github.com/user-attachments/assets/0b85b069-430d-4b65-a01f-13665b4d7b96)


# Solução de Controle de Caixa

## Overview
Esta solução foi elaborada como parte de um processo seletivo.

### Visão de Contexto
![image](https://github.com/user-attachments/assets/5bc4e460-290e-4ad4-adfe-12a349acfb1e)

Houveram problemas técnicos simplórios durante o seu desenvolvimento que ocasionaram em tempo adicional de concepção:
- Uma secretKey de desenvolvimento do AWS foi enviada acidentalmente para o GitHub, ocasionando o bloqueio da conta AWS, impactando em 1 dia de espera para retomar o acesso.
- Um problema de thread secundária no Windows Forms impedia o uso dos formulários. Embora simplório e trivial, tomou 1 dia para resolução.
- Um problema arquitetural nas roles do Cognito que não compartilhava a identidade com o IAM. Embora seja possível, tomou 1 dia de pesquisa e tentativas.

## Requisitos
Auxiliar um comerciante em controlar seus débitos e créditos, podendo ser gerado um relatório de balanços totais do dia.

### Requisitos Funcionais
- 1 computador POS (caixa)
- 1 Assinatura AWS
- 1 Assinatura GitHub
- 1 engenheiro front-end (Vue ou React)
- 1 engenheiro .NET web com AWS
- 1 engenheiro .NET Windows Forms
- 1 engenheiro DevOps

### Requisitos Não Funcionais
- Especificação de requisitos
- Budget AWS de $50 mensais para desenvolvimento
- Definição do processo de desenvolvimento

## Entregáveis
- Desenvolvimento de solução de arquitetura
- Desenvolvimento Windows Forms
- Desenvolvimento web
- Desenvolvimento AWS
- Desenvolvimento CI/CD
- Desenvolvimento testes
- Desenvolvimento IaC

## Proposta
A solução destina-se a observar a resiliência, manutenibilidade, performance, segurança, observabilidade e auditabilidade, retribuindo à equipe de desenvolvimento com praticidade e ao product owner com baixo tempo de entregas. Desta forma, foca-se na utilização de recursos que tragam time-to-market reduzido, com custo executivo e operacional de baixo ROI.

Oferecem-se recursos que tragam flexibilidade e praticidade, possibilitando alta escalabilidade para futuras funcionalidades, sendo uma solução future-ready. Inicialmente foi idealizada uma solução efetiva, mas menos atrativa, focando apenas na simplicidade e monólitos, mas posteriormente foi atualizada para a versão atual.

A ideação anterior é:
O sistema de Controle de Lançamentos utiliza autenticação integrada do Windows para garantir segurança, extraindo automaticamente a identificação do usuário logado. Para usuários externos, uma chave única é gerada com base na identificação da máquina e login, armazenada no Credential Manager. A aplicação opera com um banco de dados local SQLite para armazenar temporariamente os lançamentos quando não há conexão com o servidor online. Uma vez restabelecida a conexão, os dados são sincronizados com um banco de dados SQL Server na nuvem, que possui failover e mirroring para garantir alta disponibilidade. A gestão de permissões de acesso é feita pela equipe de banco de dados via abertura de tickets, e os dados centralizados permitem a geração de relatórios consolidados diretamente no SQL Server Reporting Services (SSRS), que podem ser visualizados e baixados no próprio formulário.

Como é uma solução antiquada, foi aprimorada.

A atual é:
Um novo usuário de caixa é criado no sistema e recebe um e-mail para acessar o aplicativo. Ao fazer o login pela primeira vez, o usuário troca sua senha e começa a registrar as entradas de vendas e pagamentos no aplicativo. Esses registros ficam guardados no computador do caixa e, quando há conexão com a internet, são enviados automaticamente para a nuvem. O sistema verifica a conexão a cada 5 segundos para garantir que tudo está funcionando bem. Todos os dias, o sistema gera um resumo automático das vendas e dos pagamentos do dia e armazena esse balanço na nuvem. O gerente pode acessar esses relatórios pela internet, fazer login no site e visualizar os dados de forma segura.

## Premissas
Esta solução teve enfoque na escalabilidade, conforme solicitado nos requisitos, por ser um over-kill dada a simplicidade dos requisitos de negócio que ela visa atender, sendo um over-engineering. O tradeoff foi ponderar entre a simplicidade e a escalabilidade.

A seguinte razão foi encontrada:
Dada a simplicidade de requisito e o enfoque na escalabilidade, o projeto é um MVC.

Visão:
- **Curto prazo** (este MVC): overengineering e alta escalabilidade.
- **Médio prazo**: simplicidade e alta escalabilidade.
- **Longo prazo**: simplicidade e alta escalabilidade.

## Solução Técnica

### Visão de Contêiners
![image](https://github.com/user-attachments/assets/5e007612-9698-4161-810f-a69d73daacd3)

### Visão de Componentes
![image](https://github.com/user-attachments/assets/c1d8ba41-ce9e-42bb-9186-fe6b7443789f)

### 1. Usuário e autenticação
- Um novo usuário "caixa" é criado no Cognito.
- O usuário é colocado na role "cashier".
- O template de invite do grupo envia um e-mail ao usuário.
- O usuário acessa o link recebido por e-mail e baixa a aplicação cliente.

### 2. Aplicação Windows
- O usuário abre a aplicação e faz login com seu usuário.
- O usuário é forçado a trocar de senha. *(Este ponto foi omitido, sendo a senha temporária automaticamente definida como senha definitiva durante o primeiro login).*
- O usuário se autentica usando o SDK do Cognito dentro da aplicação.
- A aplicação verifica se o banco local SQLite existe. Se não existir, cria o banco na raiz da aplicação com a estrutura apropriada.
- Obtém os lançamentos do banco local e exibe na list view.
- O health check da aplicação publica a cada 5 segundos (configurável no appsettings) no tópico `healthcheck` do SNS e armazena em memória o status de conexão baseado no sucesso da resposta.
- O usuário insere novos lançamentos e eles são armazenados diretamente no banco local.
- Se o status de healthcheck for Ok, os lançamentos são publicados no tópico `commitentry-topic`.
- O sistema sincroniza os lançamentos não sincronizados para o tópico quando a conexão é reestabelecida (inclui quando a aplicação é aberta e possui conexão).
- Qualquer lançamento armazenado no banco é atualizado como sincronizado quando for enviado com sucesso para o tópico.

![image](https://github.com/user-attachments/assets/a13f915c-7710-4fef-9966-a8ed221ff54e)

![image](https://github.com/user-attachments/assets/f9ebc153-bd06-47e9-a814-87f23e845c26)

![image](https://github.com/user-attachments/assets/5d048576-6472-42b8-802b-2e8a156cc8ca)

### 3. Cloud AWS
- Mensagens publicadas no tópico `commitentry` são encaminhadas para uma fila `receivedentries`.
- As mensagens da fila são automaticamente processadas por uma Lambda `movemententry` e armazenadas no banco SQL Server do RDS no AWS. Se houver erro, o lançamento é encaminhado para a DLQ e pode ser reprocessado.
- Todo dia às 20h GMT-3, o CloudWatch dispara uma Lambda `scheduletrigger` que publica a data atual no tópico `dailyreports`.
- Esse tópico coloca a mensagem na fila `orderedreports`, que é processada por uma Lambda `orderedreports`.
- Essa Lambda obtém os lançamentos desta data, faz o somatório de débitos, somatório de créditos e subtrai o total de débitos do total de créditos, gerando o balanço do dia, que é então armazenado na tabela "dailyreports" do banco SQL Server no AWS.
- O SQL do RDS possui failover e mirroring para garantir alta disponibilidade

![image](https://github.com/user-attachments/assets/134418f6-15c1-4117-83b3-1b9b3b4682e6)

### 4. Frontend (Vue)
- O usuário manager acessa a URL do Frontend.
- A URL acessa o CDN CloudFront, que por sua vez acessa o S3 e serve a página.
- O Frontend exibe um formulário de login.
- O usuário entra com suas credenciais.
- Suas credenciais são processadas pelo SDK do Cognito.
- Caso seu usuário tenha uma senha temporária, ele será solicitado a trocar a senha. *(Este passo foi omitido e a senha será reenviada como senha definitiva durante o login).*
- O usuário autenticado solicita, via API Gateway, a Lambda `listreports` para obter a lista de relatórios.
- O API Gateway enforça a autorização desta URL `/reports` para apenas a role manager, utilizando um custom authorizer para o Cognito.
- 
![image](https://github.com/user-attachments/assets/dd63574a-6236-4c7b-9573-fdbaaa6c6809)

![image](https://github.com/user-attachments/assets/43166dd9-e8ff-4469-a036-0934a45dc1fc)

![image](https://github.com/user-attachments/assets/05a4fc51-b9b9-4246-acae-ccde31c71022)

## Decisões técnicas
#### .NET Core
Optamos pelo .NET Core pela sua robustez e familiaridade no desenvolvimento de APIs e serviços backend. Ele oferece boa performance, suporte multiplataforma e uma curva de aprendizado acessível. O uso de global usings e boas práticas de organização de código garantem simplicidade no desenvolvimento e manutenção.

#### Vue.js
Vue.js foi escolhido como framework para o front-end devido à sua simplicidade e facilidade de integração. Ele possui uma curva de aprendizado suave, ideal para equipes que querem adotar um framework leve e flexível, sem a complexidade adicional de alternativas mais pesadas. Sua arquitetura baseada em componentes facilita a reutilização e organização de código, garantindo um desenvolvimento ágil e eficiente para a interface de usuário do EntryControl.

#### AWS Lambda
O uso de Lambdas simplifica a operação e elimina a necessidade de gerenciar infraestrutura, utilizando uma abordagem serverless que escala automaticamente com a demanda. Isso também reduz custos, uma vez que os recursos são cobrados apenas quando utilizados. A integração nativa com SQS e SNS permite o processamento assíncrono e escalável das entradas financeiras.

#### Amazon SQS e SNS
A combinação de SQS e SNS foi escolhida para simplificar o fluxo de dados e garantir a integridade das transações. SQS garante a entrega confiável de mensagens em filas, enquanto SNS facilita a distribuição de eventos entre diferentes componentes do sistema, permitindo um fluxo de mensagens eficiente e escalável.

#### Amazon RDS (SQL Server)
O RDS foi escolhido pela sua simplicidade de gestão, segurança integrada e escalabilidade. A escolha do SQL Server oferece robustez e familiaridade para lidar com transações financeiras, além de recursos avançados de consulta e suporte a transações complexas.

#### AWS CloudFormation
O uso de CloudFormation para gerenciar a infraestrutura como código (IaC) facilita a criação, atualização e versionamento dos recursos na AWS. Isso garante que toda a infraestrutura esteja sempre sincronizada, permitindo automação e evitando a necessidade de configuração manual, o que torna o processo mais ágil e seguro.

#### CI/CD com GitHub Actions
GitHub Actions foi escolhido para automação do pipeline CI/CD. A integração com AWS permite um fluxo contínuo de deploys para o S3 e atualização de recursos via CloudFormation, garantindo um ciclo de desenvolvimento ágil e eliminando a necessidade de intervenção manual.

#### Amazon Cognito
O Cognito foi escolhido para autenticação e autorização, já que ele proporciona uma solução segura e escalável para gerenciamento de usuários. Ele é totalmente integrado com a AWS, simplificando a gestão de identidades sem a necessidade de criar um sistema de autenticação do zero.

#### Por que AWS?
A escolha da AWS foi motivada por vários fatores que priorizam simplicidade, escalabilidade e integração. A AWS oferece um vasto ecossistema de serviços gerenciados, como Lambda, SQS, SNS, RDS e Cognito, que se integram de maneira nativa e eficiente. Isso permite um desenvolvimento mais rápido e elimina a sobrecarga de gerenciar infraestrutura manualmente. Além disso, a AWS garante segurança, com ferramentas como IAM para controle de acesso e monitoramento integrado, simplificando o gerenciamento de permissões e garantindo que o sistema opere de maneira segura e escalável.

## Padrões de Projeto, Desenvolvimento e Arquitetura

O projeto segue uma arquitetura modular, com clara separação de responsabilidades:

- **Injeção de Dependência**: Utilizada para gerenciar a criação e o ciclo de vida dos serviços.
- **Serviços**: Cada serviço (como `SynchronizeService` e `HealthCheckService`) é responsável por uma tarefa específica, promovendo a reutilização e testabilidade do código.
- **Eventos**: Utilizados para permitir a comunicação assíncrona entre componentes (por exemplo, mudanças no estado de conectividade).

## Padrões de Projeto e Princípios de Desenvolvimento

#### 1. Padrão de Injeção de Dependência (Dependency Injection)
- **Descrição:** A injeção de dependência é um padrão de design usado para implementar a Inversão de Controle (IoC). Ela permite que as dependências de uma classe sejam fornecidas a partir do exterior, em vez de a própria classe criar suas dependências.
- **Exemplo no Projeto:** No projeto apresentado, a injeção de dependência é usada amplamente para passar instâncias de `IEntryRepository`, `IEntryApi`, `IHealthCheckService`, e `ILogger<T>`, entre outros, para os serviços que os consomem. Isso facilita o teste unitário, permitindo a substituição dessas dependências por mocks durante os testes.

#### 2. Padrão de Repositório (Repository Pattern)
- **Descrição:** O padrão de repositório abstrai a camada de acesso a dados, fornecendo uma interface que encapsula a lógica necessária para acessar os dados de diferentes fontes.
- **Exemplo no Projeto:** `IEntryRepository` e sua implementação abstraem a lógica de acesso ao banco de dados, permitindo que o serviço `EntryService` interaja com os dados sem conhecer os detalhes da persistência, seguindo o princípio de separação de responsabilidades.

#### 3. Padrão de Estratégia (Strategy Pattern)
- **Descrição:** Permite que uma classe ou método de comportamento específico seja alterado em tempo de execução, dependendo das condições ou do contexto.
- **Exemplo no Projeto:** Embora não implementado explicitamente, o uso de `IOptionsMonitor` para trocar URLs dinamicamente no `SynchronizeService` pode ser visto como uma aplicação simplificada do padrão de estratégia, onde a estratégia (URL do serviço) pode ser alterada sem modificar a lógica interna.

### Princípios SOLID

#### 1. Single Responsibility Principle (SRP)
- **Descrição:** Cada classe deve ter uma única responsabilidade, ou seja, uma única razão para mudar.
- **Exemplo no Projeto:** A classe `HealthCheckService` tem a responsabilidade única de verificar o estado de saúde do serviço externo. Ela não se preocupa com a sincronização de dados ou com a manipulação de transações, o que mantém a lógica focada e fácil de manter.

#### 2. Open/Closed Principle (OCP)
- **Descrição:** As classes devem ser abertas para extensão, mas fechadas para modificação.
- **Exemplo no Projeto:** A interface `IEntryApi` permite que diferentes implementações de APIs possam ser usadas sem alterar o código dos serviços que consomem essa API. Se uma nova API precisa ser integrada, uma nova implementação de `IEntryApi` pode ser criada, deixando o código do `SynchronizeService` inalterado.

#### 3. Liskov Substitution Principle (LSP)
- **Descrição:** Objetos de uma classe derivada devem poder substituir objetos de sua classe base sem alterar a integridade do programa.
- **Exemplo no Projeto:** A classe `MockTransacaoRepository`, que implementa a interface `IEntryRepository` para fins de teste, pode substituir a implementação real sem causar problemas no funcionamento do `EntryService`. Isso garante que qualquer implementação de `IEntryRepository` possa ser usada de maneira intercambiável.

#### 4. Interface Segregation Principle (ISP)
- **Descrição:** Uma classe não deve ser forçada a implementar interfaces que não utiliza.
- **Exemplo no Projeto:** A interface `IHealthCheckService` é específica para a verificação do estado do serviço online. Outras interfaces, como `ISynchronizeService`, não são obrigadas a implementar métodos de verificação de saúde, mantendo-as focadas em suas respectivas responsabilidades.

#### 5. Dependency Inversion Principle (DIP)
- **Descrição:** Dependa de abstrações, não de concretizações.
- **Exemplo no Projeto:** O `SynchronizeService` depende da abstração `IEntryRepository` em vez de uma classe concreta. Isso permite que o repositório seja facilmente substituído por outra implementação, como um repositório em memória para testes, sem alterar o código do serviço.

### Princípios DRY, YAGNI e KISS

#### 1. Don't Repeat Yourself (DRY)
- **Descrição:** Evite a duplicação de código, reutilizando componentes e abstrações.
- **Exemplo no Projeto:** O projeto adere ao princípio DRY ao utilizar serviços e repositórios compartilhados. Por exemplo, a lógica de acesso ao banco de dados é centralizada no repositório, e não duplicada em cada serviço que necessita dessa funcionalidade.

#### 2. You Aren't Gonna Need It (YAGNI)
- **Descrição:** Evite adicionar funcionalidades até que elas sejam realmente necessárias.
- **Exemplo no Projeto:** O projeto parece seguir o princípio YAGNI, com uma implementação direta e focada no necessário para cumprir os requisitos funcionais. Não há complexidade adicional desnecessária ou código escrito para funcionalidades futuras incertas.

#### 3. Keep It Simple, Stupid (KISS)
- **Descrição:** Simplicidade deve ser um objetivo chave no design, evitando complexidade desnecessária.
- **Exemplo no Projeto:** A simplicidade no design de serviços como `EntryService` e `SynchronizeService`, com responsabilidades claras e uma interface de fácil compreensão, exemplifica o princípio KISS. As decisões arquiteturais visam a clareza e a facilidade de manutenção.

### Padrão Triple A (Arrange, Act, Assert)

#### 1. Descrição:
O padrão Triple A é uma abordagem comum em testes unitários que organiza o teste em três seções distintas:
  - **Arrange:** Configura os objetos e as condições iniciais necessárias para o teste.
  - **Act:** Executa a ação ou comportamento que está sendo testado.
  - **Assert:** Verifica se o resultado da ação está de acordo com o esperado.

#### 2. Exemplo no Projeto:
Em todos os testes unitários do projeto, o padrão Triple A é seguido de maneira consistente. Por exemplo, no teste `Sincronizar_DeveEnviarLancamentosNaoSincronizados_E_MarcarComoSincronizado`:
  - **Arrange:** O código configura os mocks para `IEntryRepository` e `IEntryApi`, definindo o comportamento esperado e preparando o serviço de sincronização.
  - **Act:** O método `Synchronize()` é chamado no serviço de sincronização.
  - **Assert:** Asserções são feitas para garantir que as transações foram marcadas como sincronizadas e que o método de sincronização foi chamado corretamente.

## Escalabilidade
- A arquitetura suporta escalabilidade horizontal, com recursos automatizados para escalar Lambdas e bancos de dados conforme necessário. O uso de Auto Scaling no RDS e a distribuição de carga entre as filas SQS garante que o sistema possa atender a picos de demanda sem degradação significativa de performance.

## Observabilidade e monitoramento
- A aplicação é monitorada pelo AWS, utilizando serviços como CloudWatch Logs e X-Ray para tracing e detecção de anomalias. Métricas customizadas são configuradas para monitorar o fluxo de lançamentos, sincronizações e status da aplicação cliente, com alertas automáticos em caso de falhas ou degradação de serviço.

## Autenticação e segurança
- Cognito é configurado para MFA (autenticação multifator), garantindo maior segurança no acesso de usuários. As políticas de IAM também são revisadas periodicamente, e a rotação de chaves e práticas de segurança como proteção contra fraudes são implementadas.
- As URLs do API Gateway utilizadas no frontend estão configuradas com CORS (Cross-Origin Resource Sharing) ativado, permitindo apenas requisições provenientes da origem correspondente ao CDN do CloudFront. Essa configuração garante que apenas o domínio autorizado possa realizar chamadas à API, prevenindo ataques comuns de Cross-Site Request Forgery (CSRF) e Cross-Origin Resource Sharing (CORS) mal configurados. Com isso, protege-se contra requisições maliciosas vindas de origens não confiáveis, reforçando a segurança das interações entre o frontend e o backend.

## Segredos e credenciais
- As senhas, connection strings e outros segredos são armazenados no AWS Secret Manager, garantindo que não sejam expostos diretamente no código. O acesso aos segredos é controlado por permissões de IAM apropriadas, garantindo segurança no armazenamento e uso. *(Nesta entrega não está em uso.)*

## Resiliência e alta disponibilidade
- Em caso de falhas massivas, a arquitetura possui failover configurado para o RDS, garantindo a disponibilidade contínua do banco de dados. Além disso, backups diários são feitos automaticamente, permitindo recuperação rápida em caso de desastres.

## Processo de deploy e CI/CD
- A aplicação é enviada via pipeline através da etapa de CloudFormation para o AWS. Todos os recursos da Stack são persistidos no CloudFormation. São utilizados blue/green deployments para garantir que atualizações não causem interrupções no serviço.

## Testes
- Os testes são feitos via teste unitário. A aplicação Windows Forms possui seus testes criados, garantindo que novos blocos de código sejam testados.
- A aplicação cloud é testada também via testes unitários, com arquivos JSON simulando cenários para as Lambdas. *(Nesta entrega, os testes unitários do frontend não estão presentes.)*
- O frontend também deve ser testado com testes unitários. *(Nesta entrega, os testes unitários do frontend não estão presentes.)*

### Cenários de Testes
#### 1. Adicionar um Lançamento

**Cenário:** O usuário adiciona um novo lançamento do tipo "Débito".

- **Dado** que o usuário acessa a interface de adição de lançamentos,
- **Quando** o usuário insere as informações (Tipo: "Débito", Valor: 100, Descrição: "Compra de materiais") e confirma a adição,
- **Então** o lançamento deve ser salvo no banco de dados com o status `Synchronized = false`,
- **E** um evento `OnEntryAdded` deve ser disparado.

#### 2. Sincronização de Lançamentos Não Sincronizados

**Cenário:** Sincronizar lançamentos não sincronizados com o serviço externo.

- **Dado** que existem lançamentos com `Synchronized = false` no banco de dados,
- **Quando** o serviço externo está disponível,
- **Então** o sistema deve enviar os lançamentos não sincronizados ao serviço externo,
- **E** marcar os lançamentos como sincronizados (`Synchronized = true`),
- **E** disparar o evento `OnSyncCompleted`.

#### 3. Falha na Sincronização devido a Falta de Conexão

**Cenário:** O serviço externo não está disponível durante a tentativa de sincronização.

- **Dado** que o serviço externo está indisponível,
- **Quando** o sistema tenta sincronizar os lançamentos não sincronizados,
- **Então** o sistema deve capturar a exceção e logar o erro,
- **E** os lançamentos devem permanecer com o status `Synchronized = false`.

#### 4. Verificação de Saúde do Serviço Externo

**Cenário:** Verificação da saúde do serviço externo.

- **Dado** que a aplicação está rodando,
- **Quando** o `HealthCheckService` realiza a verificação do serviço externo,
- **E** o serviço externo está disponível,
- **Então** o sistema deve registrar que o serviço está saudável (`IsServiceHealthy = true`),
- **E** disparar um evento `OnHealthStatusChanged` se o status de saúde mudou.

#### 5. Sincronização Automática ao Restabelecer Conexão

**Cenário:** Sincronização automática de lançamentos após a conexão com o serviço externo ser restabelecida.

- **Dado** que o serviço externo estava indisponível e se torna disponível novamente,
- **Quando** o sistema detecta que o serviço externo está novamente saudável,
- **Então** todos os lançamentos não sincronizados devem ser sincronizados automaticamente,
- **E** o sistema deve marcar esses lançamentos como sincronizados.

#### 6. Visualização de Lançamentos Sincronizados e Não Sincronizados

**Cenário:** O usuário visualiza lançamentos sincronizados e não sincronizados na interface.

- **Dado** que o usuário acessa a aba de listagem de lançamentos,
- **Quando** o usuário seleciona a opção "Todos",
- **Então** a lista deve exibir todos os lançamentos do banco de dados.
- **Quando** o usuário seleciona a opção "Sincronizados",
- **Então** a lista deve exibir apenas os lançamentos com `Synchronized = true`.
- **Quando** o usuário seleciona a opção "Não Sincronizados",
- **Então** a lista deve exibir apenas os lançamentos com `Synchronized = false`.

#### 7. Atualização do Intervalo de Verificação de Saúde

**Cenário:** O intervalo de verificação de saúde do serviço externo é alterado.

- **Dado** que o intervalo de verificação de saúde é definido nas configurações,
- **Quando** o intervalo é alterado no arquivo `appsettings.json`,
- **Então** o sistema deve reconfigurar o `Timer` de verificação de saúde para o novo intervalo,
- **E** logar a reconfiguração do intervalo.
****

# Como executar
Há duas formas:

## 1- Iniciando uma nova implantação
### Github / AWS
- Gerar uma nova access key na AWS
- Configurar a AccessKey nas configuracoes do runner da pipeline (https://github.com/{SEU_USUARIO}/{SEU_REPOSITORIO}/settings/secrets/actions)
![image](https://github.com/user-attachments/assets/05569391-6250-46aa-b317-9d5e624048a0)
- Implantar o CloudFormation no AWS executando a pipeline
- Criar usuários no cognito definindo a senha inicial

### POS
- Compilar o POS
- Executar a aplicação
- Login com usuário e senha criado no cognito

### FrontEnd
- Visitar o front em https://{SEUAPIGATEWAY}.cloudfront.net/
- Login com usuário e senha criado no cognito


### 2- Usando a assinatura do teste
#### AWS
- Acessar https://248428763777.signin.aws.amazon.com/console
- Login IAM (ADMIN):
  usuario: "testuser"
  pass: "P@ssw0rd"
  
#### POS
- Compilar o POS na pasta Client
- Executar a aplicação
- Login:
  usuário: "user1"
  senha: "123456"

#### FrontEnd
- Visitar o front em https://dw5fxkf82pexh.cloudfront.net/
- Login:
  usuário: "user1"
  senha: "123456"

## Conclusão
A solução busca equilibrar simplicidade, escalabilidade e robustez, utilizando práticas modernas de desenvolvimento e operações na nuvem. A arquitetura é flexível o suficiente para lidar com os requisitos atuais e futuras expansões, oferecendo uma solução future-ready para o controle de caixa.

