# DailyFlow

O DailyFlow é um serviço que gerencia o controle de lançamentos e a consolidação de transações, incluindo créditos e débitos, e fornece um relatório consolidado diário. O serviço é composto por 3 APIs, cada uma responsável por uma parte do processo.

## Pré-requisitos

Certifique-se de que as seguintes ferramentas estão instaladas:
- **Docker**: Para a contêinerização.
- **Docker Compose**: Para gerenciar os containers
- **.NET**: O serviço foi desenvolvido com .NET.
- **AWS CLI**: Para gerenciar serviços da AWS, como o SQS localmente.

![alt text](fluxo.png)

## AUTH API

A **Auth API** lida com a autenticação do usuário. Ela recebe um nome de usuário e verifica se está registrado na base de dados. Vale ressaltar que a autenticação por senha ainda não está implementada, mas é recomendada para futuras melhorias, assim como a implementação de um mecanismo de refresh token.


## Releases API

A **Releases API** é responsável pelo processamento dos lançamentos (créditos e débitos). Cada lançamento exige uma descrição e um valor:
- Valores positivos são considerados créditos.
- Valores negativos são considerados débitos.


A API utiliza autenticação JWT para verificar a empresa e o usuário que está realizando o lançamento. Após receber os dados, o lançamento é enviado para uma fila SQS para processamento assíncrono. 
São executadas duas instâncias do leitor da fila para demonstrar a escalabilidade, garantindo que cada mensagem seja processada por apenas uma instância.

## Consolidated API

A **Consolidated API** agrega todos os lançamentos do dia, calculando o total de créditos, débitos e o saldo geral do dia.

Para otimizar o desempenho, a API armazena o primeiro resultado em um cache Redis, que é válido por 10 minutos. A duração do cache é facilmente configurável.

OS dados vão para um base MONGODB

### Para rodar a API e o leitor da fila:

```bash
$ docker compose -f docker-compose.yml up -d --force-recreate --build

$ aws --endpoint-url=http://localhost:4566 sqs create-queue --queue-name testQueue
```

# Postman

O aquivo <b>RELEASES.postman_collection</b> quando carregado no POSTMAN, disponibiliza 3 endpoints para teste
 - Lançamentos
 - Consolidados
 - Autenticação