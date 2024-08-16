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

## Melhorias

Ainda não sei como posso corrigir um bug que pode acontecer enquanto mais de 2 cópias estejam online.

Ao subir um lider (1), uma cópia (2), e uma cópia (3) observando a cópia (2).

Caso a cópia (2) desligue, a cópia (3) e o lider (1) irão iniciar o processamento como lider, o que é um comportamento incorreto.

**Solução teórica**: 

Adicionar o IP do lider original, e a cópia (3) só irá atuar caso o lider (1) e a cópia (2) estejam offline.