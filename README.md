## Leader

Leader é um teste realizado para implementar comunicação entre aplicações utilizando gRPC.

## Objetivo

O programa tem como objetivo ser lider e copia de si próprio.
Quando um lider está presente, todas as instâncias atuam como copias; Quando o lider se desconecta, a outra instância atua como lider.

## Utilizar

O arquivo `appsettings.json` determina se a aplicação irá atuar como lider ou cópia, verifique o campo `Leader`.

Assim como o campo `LeaderIp`, que será usado como parâmetro para verificar se o lider está ativo.

O lider não verifica em momento nenhum a atividade de suas cópias.

## Lider

O lider executa um processamento falso, apenas um cálculo matematico.

```bash
Iniciando como líder...
Leader
```

## Cópia

Quando uma cópia entrar em operação como lider, irá executar exatamente o mesmo processamento.

* Lider online:
```bash
Iniciando como backup...
Heartbeat solicitado às 16/08/2024 11:22:54: Leader
```

* Lider offline:
```bash
Heartbeat solicitado às 16/08/2024 11:23:52: Backup1
```