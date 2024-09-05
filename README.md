# Solução de Controle de Caixa

## Overview
Esta solução foi elaborada como parte de um processo seletivo.

### Visão de Contexto
![image](https://github.com/user-attachments/assets/076f5963-9e95-4cd2-a1f6-19e7be142c92)

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
![image](https://github.com/user-attachments/assets/ec588268-1dcb-484e-8322-4ea04790a1a1)

### Visão de Componentes
![image](https://github.com/user-attachments/assets/3ad7ee52-652c-40c4-9bf6-c35d9268cbc6)

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

![image](https://github.com/user-attachments/assets/41d91adc-3d6e-49cd-8e22-363027baf056)

![image](https://github.com/user-attachments/assets/ccf1994e-702b-4611-86ff-37be8acc7d29)

![image](https://github.com/user-attachments/assets/e82a7d33-4127-49dd-8b77-6755b3e51c19)

### 3. Cloud AWS
- Mensagens publicadas no tópico `commitentry` são encaminhadas para uma fila `receivedentries`.
- As mensagens da fila são automaticamente processadas por uma Lambda `movemententry` e armazenadas no banco SQL Server do RDS no AWS. Se houver erro, o lançamento é encaminhado para a DLQ e pode ser reprocessado.
- Todo dia às 20h GMT-3, o CloudWatch dispara uma Lambda `scheduletrigger` que publica a data atual no tópico `dailyreports`.
- Esse tópico coloca a mensagem na fila `orderedreports`, que é processada por uma Lambda `orderedreports`.
- Essa Lambda obtém os lançamentos desta data, faz o somatório de débitos, somatório de créditos e subtrai o total de débitos do total de créditos, gerando o balanço do dia, que é então armazenado na tabela "dailyreports" do banco SQL Server no AWS.
- O SQL do RDS possui failover e mirroring para garantir alta disponibilidade

![image](https://github.com/user-attachments/assets/c22e1153-b8b1-454d-a11b-3ceaddc3e218)

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
![image](https://github.com/user-attachments/assets/d3f385a0-5fd8-47ac-ba7b-dfb0cbd848a4)

![image](https://github.com/user-attachments/assets/c1a0a63c-7110-4600-bea1-48d12582efac)

![image](https://github.com/user-attachments/assets/e92969da-37b8-494c-9e3a-8b0889e759c2)

### 5. Escalabilidade
- A arquitetura suporta escalabilidade horizontal, com recursos automatizados para escalar Lambdas e bancos de dados conforme necessário. O uso de Auto Scaling no RDS e a distribuição de carga entre as filas SQS garante que o sistema possa atender a picos de demanda sem degradação significativa de performance.

### 6. Observabilidade e monitoramento
- A aplicação é monitorada pelo AWS, utilizando serviços como CloudWatch Logs e X-Ray para tracing e detecção de anomalias. Métricas customizadas são configuradas para monitorar o fluxo de lançamentos, sincronizações e status da aplicação cliente, com alertas automáticos em caso de falhas ou degradação de serviço.

### 7. Autenticação e segurança
- Cognito é configurado para MFA (autenticação multifator), garantindo maior segurança no acesso de usuários. As políticas de IAM também são revisadas periodicamente, e a rotação de chaves e práticas de segurança como proteção contra fraudes são implementadas.
- As URLs do API Gateway utilizadas no frontend estão configuradas com CORS (Cross-Origin Resource Sharing) ativado, permitindo apenas requisições provenientes da origem correspondente ao CDN do CloudFront. Essa configuração garante que apenas o domínio autorizado possa realizar chamadas à API, prevenindo ataques comuns de Cross-Site Request Forgery (CSRF) e Cross-Origin Resource Sharing (CORS) mal configurados. Com isso, protege-se contra requisições maliciosas vindas de origens não confiáveis, reforçando a segurança das interações entre o frontend e o backend.

### 8. Segredos e credenciais
- As senhas, connection strings e outros segredos são armazenados no AWS Secret Manager, garantindo que não sejam expostos diretamente no código. O acesso aos segredos é controlado por permissões de IAM apropriadas, garantindo segurança no armazenamento e uso. *(Nesta entrega não está em uso.)*

### 9. Resiliência e alta disponibilidade
- Em caso de falhas massivas, a arquitetura possui failover configurado para o RDS, garantindo a disponibilidade contínua do banco de dados. Além disso, backups diários são feitos automaticamente, permitindo recuperação rápida em caso de desastres.

### 10. Processo de deploy e CI/CD
- A aplicação é enviada via pipeline através da etapa de CloudFormation para o AWS. Todos os recursos da Stack são persistidos no CloudFormation. São utilizados blue/green deployments para garantir que atualizações não causem interrupções no serviço.

### 11. Testes
- Os testes são feitos via teste unitário. A aplicação Windows Forms possui seus testes criados, garantindo que novos blocos de código sejam testados.
- A aplicação cloud é testada também via testes unitários, com arquivos JSON simulando cenários para as Lambdas. *(Nesta entrega, os testes unitários do frontend não estão presentes.)*
- O frontend também deve ser testado com testes unitários. *(Nesta entrega, os testes unitários do frontend não estão presentes.)*

# Como executar
Há duas formas:

## 1- Iniciando uma nova implantação
### Github / AWS
- Gerar uma nova access key na AWS
- Configurar a AccessKey nas configuracoes do runner da pipeline (https://github.com/{SEU_USUARIO}/{SEU_REPOSITORIO}/settings/secrets/actions)
![image](https://github.com/user-attachments/assets/17d07fc9-5830-4754-9360-5a252bdc905c)
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

