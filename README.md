# Projeto DevOps - SPRINT 3

Este repositório contém os scripts e configurações necessárias para realizar o deploy de uma aplicação .NET no Azure Web App, com integração a um banco de dados SQL. Ele inclui também scripts de testes para garantir o correto funcionamento do sistema.

## Estrutura do Projeto

- `deploy-scripts/`: Scripts para criar e configurar a infraestrutura no Azure (Web App, banco de dados SQL, etc).
- `app.zip`: Código-fonte da aplicação, pronto para deploy no Azure.
- `tests/`: Scripts de teste para validar a API e o banco de dados.

## Pré-requisitos

- **Azure CLI**
- **Node.js**
- **npm**

## Passos para realizar o deploy

1. **Login no Azure**  
Primeiro, você precisa se autenticar na sua conta do Azure. Para isso, use o comando:
    ```bash
    az login
    ```

2. **Descompactar a Pasta do Projeto Baixado do GitHub**  
Baixe o projeto do GitHub (caso ainda não tenha feito) e extraia o arquivo `.zip`. Supondo que o arquivo `app.zip` contenha o código da aplicação, execute:
    ```bash
    powershell Compress-Archive -Path * -DestinationPath ../app.zip -Force
    ```

3. **Acessar o Diretório do Projeto**  
Depois de descompactar o projeto, entre no diretório da pasta do projeto:
    ```bash
    cd path/do/projeto
    ```

4. **Criar o Grupo de Recursos no Azure**  
Agora, crie um grupo de recursos no Azure onde todos os recursos serão alocados:
    ```bash
    az group create -n mottu-rg -l brazilsouth
    ```

5. **Criar o Plano de App Service**  
Crie o plano de App Service onde a aplicação será hospedada:
    ```bash
    az appservice plan create -g mottu-rg -n mottu-plan --sku B1 --is-linux
    ```

6. **Criar o Web App**  
Crie o Web App para hospedar a aplicação:
    ```bash
    az webapp create -g mottu-rg -p mottu-plan -n mottu-fleet-web --runtime "NODE|20-lts"
    ```

7. **Configurar a Versão do Node.js no Web App**  
Configure a versão do Node.js para a aplicação:
    ```bash
    az webapp config appsettings set -g mottu-rg -n mottu-fleet-web --settings WEBSITE_NODE_DEFAULT_VERSION=20
    ```

8. **Criar o Servidor SQL**  
Agora, crie o servidor de banco de dados SQL:
    ```bash
    az sql server create -l brazilsouth -g mottu-rg -n mottufleet-sqlsrv -u sqladmin -p Str0ng!Senha!123
    ```

9. **Configurar a Regra de Firewall para o SQL Server**  
Crie a regra de firewall para permitir o acesso do Azure ao servidor SQL:
    ```bash
    az sql server firewall-rule create -g mottu-rg -s mottufleet-sqlsrv -n AllowAzureServices --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0
    ```

10. **Criar o Banco de Dados SQL**  
Crie o banco de dados no servidor SQL:
    ```bash
    az sql db create -g mottu-rg -s mottufleet-sqlsrv -n mottu-fleet-db --service-objective GP_Gen5_2 --zone-redundant false --max-size 2GB
    ```

11. **Obter o Nome Completo do Servidor SQL**  
Recupere o FQDN do servidor SQL para configurar a string de conexão:
    ```bash
    for /f %A in ('az sql server show -n mottufleet-sqlsrv -g mottu-rg --query fullyQualifiedDomainName -o tsv') do set SQL_FQDN=%A
    ```

12. **Configurar a String de Conexão do Banco de Dados no Web App**  
Agora, configure a string de conexão no Web App:
    ```bash
    set CONNECTION_STRING=Server=tcp:%SQL_FQDN%,1433;Initial Catalog=mottu-fleet-db;Persist Security Info=False;User ID=sqladmin;Password=Str0ng!Senha!123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
    az webapp config appsettings set -g mottu-rg -n mottu-fleet-web --settings SQL_CONNECTION="%CONNECTION_STRING%" NODE_ENV=production
    ```

13. **Realizar o Deploy da Aplicação**  
Realize o deploy do código no Azure:
    ```bash
    az webapp deploy -g mottu-rg -n mottu-fleet-web --src-path ../app.zip
    ```

14. **Configuração do Build Durante o Deploy**  
Configure a opção de build durante o deploy:
    ```bash
    az webapp config appsettings set -g mottu-rg -n mottu-fleet-web --settings SCM_DO_BUILD_DURING_DEPLOYMENT=true
    ```

---

Agora você pode copiar e colar tudo diretamente no seu **README.md**. Ele está organizado em um único texto, com todos os passos e scripts prontos para seguir.
