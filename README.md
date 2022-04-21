﻿# MeAgendaAi

[![dotnet package](https://github.com/Maarkis/MeAgendaAi/actions/workflows/workflows-me-agenda-ai.yml/badge.svg)](https://github.com/Maarkis/MeAgendaAi/actions/workflows/workflows-me-agenda-ai.yml)

## Características do projeto

- Versão .NET: [6.0](https://dotnet.microsoft.com/download/dote/6.0)
- Banco de dados: [PostgreSQL](https://www.postgresql.org/).
- Ferramenta de manipulação de dados: [Entity Framework](https://entityframework.net/).
- Ferramenta de testes: [xUnit](https://docs.nunit.org/).
- Ferramenta de Mock: [Moq](https://documentation.help/Moq/).
- Ferramenta de Assert: [FluentAssertions](https://fluentassertions.com/).
- Ferramenta de Data Generator: [Bogus](https://www.nuget.org/packages/Bogus/).
- Ferramenta de Mapeamento: [AutoMapper](https://automapper.org/).
- Ferramenta de Cache: [Redis](https://redis.io/).
- Ferramenta de visualização do banco de dados: [PgAdmin](https://www.pgadmin.org/).
- Ferramenta de visualização do cache: [Redis Commander](https://github.com/joeferner/redis-commander).

## SetUp - Ferramentas necessárias para o desenvolvimento

- [Visual Studio](https://visualstudio.microsoft.com/pt-br/downloads/)
- [NET SDK 6+](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop)

## Criando imagem docker da aplicação

```bash
docker build -t me-agenda-ai:latest .
```

Argumento _-t_ especifica o nome e a tag da imagem que será criada. Nesse caso, a imagem será chamada com uma tag atribuída.

- Ex. _meagendaai:dev_

## Criando container com imagem da aplicação MeAgendaAi, PostgreSQL e PgAdmin

O Projeto dispõe de uma estrutura de **docker-compose** para execução da aplicação, uma instância de container com um banco de dados em _PostgreSQL_ e uma ferramenta web chamada _PgAdmin_ para manipulação do banco de dados.

```bash
docker-compose up -d
```

**Obs.:** O _-d_ indica que a execução do container será em background (em segundo plano).

## PgAdmin - Configuração de acesso ao banco de dados

Para conectar na instância local do banco de dados, o _PgAdmin_ deve ser configurado para acesso remoto. Você deve acessar a URL [localhost:8081](http://localhost:8081) e digitar o usuário e senha para acesso ao banco de dados. Caso não seja possível acessar o _PgAdmin_, verifique se o mesmo está rodando.

O e-mail padrão é _admin@admin.com_ e a senha _123_ (**configuração definida no docker-compose.yml**).

Ao acessar o _PgAdmin_ pela primeira vez o item **Servers** estará vazio, pois ainda não terá a configuração para se conectar-se ao banco de dados. Para configurar a conexão, siga as instruções abaixo:

1. Click com o botão direito do mouse em **Servers** e acesse o menu _Create => Server_.
2. Na guia **General** dê um nome para o servidor (_**Recomendado: MeAgendaAi**_).
3. Na guia **Connection**, no campo **Host/address**, digite _**postgresdb**_, preencha o campo **Port** com _**5432**_ e **Maintenance database** com _**postgres**_, caso ainda não esteja configurado.
4. Ainda na guia **Connection**, no campo **Username**, digite _**admin**_ e no campo **Password**, digite _**123**_.
5. Salve a configuração e neste momento, você já pode acessar o banco de dados.

## Criando migration

Antes de criar qualquer migrations, precisar instalar o tool do entity framework (ef). Pode ser instalado como uma ferramenta global ou local.

**Ex. local**

```bash
dotnet tool install  dotnet-ef
```

**Ex. global**

```bash
dotnet tool install --global dotnet-ef
```

Para criar uma nova migration

```bash
dotnet ef migrations add _<NOME_MIGRATION>_ -s ./src/MeAgendaAi.Application/ -p ./src/MeAgendaAi.Infra.Data/
```

- Argumento **-s** é referente a **--startup-project**, projeto onde tem referência da connection string e injeção da configuração do banco.
- Argumento **-p** é referente a **--project**, projeto onde tem referência contexto do banco.

## Aplicando migration

```bash
dotnet ef database update -s ./src/MeAgendaAi.Application/
```

## Redis Commander - Acessando

Redis-Commander é um aplicativo web node.js usado para visualizar, editar e gerenciar um banco de dados Redis.

Para manipular e visualizar o banco de dados Redis, utilizaremos o _Redis Commander_ em container Docker (Criado anteriormente) e acessaremos via o endereço [http://localhost:8091/](http://localhost:8091/).
