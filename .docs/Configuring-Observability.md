# Configurando a Observabilidade

Este guia explica os componentes de observabilidade da stack de desenvolvimento do RavenLedger, seu propósito, e como configurar o Grafana para visualizar logs e informações de APM (rastreamento distribuído).

---

## Componentes

### Prometheus

**O que é:** banco de dados de séries temporais para métricas.  
**Como funciona:** faz *scrape* ativo do endpoint `/metrics` da API a cada 15 segundos e armazena os dados localmente.  
**Propósito:** registrar contadores, histogramas e gauges da aplicação (requisições HTTP, latência, erros).  
**Acesso:** http://localhost:9090

### Grafana

**O que é:** plataforma de dashboards e visualização.  
**Como funciona:** conecta-se a múltiplas fontes de dados (Prometheus, Loki, Jaeger) via datasources configurados. Não coleta dados por conta própria.  
**Propósito:** ponto central de observabilidade — métricas, logs e traces em painéis unificados.  
**Acesso:** http://localhost:3000 · credenciais: `admin / admin`

### Jaeger

**O que é:** sistema de rastreamento distribuído compatível com OpenTelemetry.  
**Como funciona:** recebe spans via OTLP gRPC (porta 4317) e os correlaciona em traces completos, permitindo visualizar o caminho de uma requisição através dos serviços.  
**Propósito:** APM — identificar gargalos de latência, falhas em chamadas entre serviços e fluxo de execução.  
**Acesso:** http://localhost:16686

### Zipkin

**O que é:** sistema alternativo de rastreamento distribuído.  
**Como funciona:** recebe spans via HTTP no protocolo Zipkin (porta 9411).  
**Propósito:** redundância de coleta de traces; útil para comparar com o Jaeger ou integrar com sistemas que já usam o protocolo Zipkin.  
**Acesso:** http://localhost:9411

### Loki

**O que é:** sistema de agregação de logs, otimizado para armazenar e consultar logs indexados por labels.  
**Como funciona:** recebe streams de logs enviados pelo Promtail e os armazena de forma comprimida. Não processa o conteúdo do log — apenas indexa as labels (ex.: nome do container).  
**Propósito:** centralizar os logs JSON emitidos no stdout dos containers para consulta no Grafana.  
**Acesso:** http://localhost:3100 (API interna — use o Grafana para consultar)

### Promtail

**O que é:** agente coletor de logs que alimenta o Loki.  
**Como funciona:** conecta-se ao socket do Docker (`/var/run/docker.sock`), descobre containers ativos automaticamente via service discovery e encaminha suas saídas stdout/stderr ao Loki com labels de contexto (nome do container, stream).  
**Propósito:** ponte entre os logs do Docker e o Loki, sem necessidade de modificar a aplicação.  
**Acesso:** sem interface — serviço interno.

### Nginx

**O que é:** proxy reverso.  
**Como funciona:** recebe requisições na porta `8080` do host e as repassa à API na rede interna Docker.  
**Propósito:** ponto de entrada único para a API, com possibilidade futura de TLS, rate limiting e roteamento.  
**Acesso:** http://localhost:8080

---

## Diagrama de fluxo

```
           HTTP
Cliente ──────────► Nginx :8080 ──► API :8080
                                      │
                         ┌────────────┼────────────┐
                         ▼            ▼             ▼
                    Jaeger         Zipkin       stdout (JSON)
                    :4317           :9411            │
                  (OTLP gRPC)                    Promtail
                      │                              │
                   Grafana ◄─── Prometheus ◄─ /metrics
                   :3000          :9090
                      │
                     Loki
                    :3100
```

---

## Passo a passo: Logs no Grafana

O Grafana já vem com o datasource **Loki** provisionado automaticamente. Para consultar os logs:

1. Acesse o Grafana em http://localhost:3000 e faça login com `admin / admin`.
2. No menu lateral, clique em **Explore** (ícone de bússola).
3. No seletor de datasource (canto superior esquerdo), escolha **Loki**.
4. No campo **Label filters**, selecione `container` = `<nome-do-container>`.
   - O container da API normalmente se chama `docker-api-1`.
5. Clique em **Run query** (ou pressione `Shift+Enter`).

Os logs aparecerão em ordem cronológica inversa. O conteúdo é JSON (formato CLEF do Serilog); o Grafana exibe o campo `message` por padrão.

### Filtros úteis no LogQL

```logql
# Todos os logs do container da API
{container="docker-api-1"}

# Apenas logs de nível Warning ou superior
{container="docker-api-1"} | json | level >= "Warning"

# Busca por texto livre
{container="docker-api-1"} |= "exception"
```

---

## Passo a passo: APM (Traces) no Grafana

O datasource **Jaeger** também está provisionado automaticamente. Para explorar os traces:

1. Acesse o Grafana em http://localhost:3000.
2. No menu lateral, clique em **Explore**.
3. No seletor de datasource, escolha **Jaeger**.
4. No campo **Service**, selecione `RavenLedger.Ingestion.Api`.
5. Clique em **Run query** para listar os traces recentes.
6. Clique em qualquer trace para expandir os spans e ver:
   - Duração de cada etapa da requisição
   - Tags e logs associados ao span
   - Erros sinalizados em vermelho

### Correlacionando logs com traces

Quando um trace ID aparecer nos logs JSON (campo `TraceId`), você pode colá-lo diretamente no campo **Trace ID** do datasource Jaeger no Explore para saltar do log ao trace correspondente.

---

## Referências

- [Documentação do Loki](https://grafana.com/docs/loki/latest/)
- [Documentação do Jaeger](https://www.jaegertracing.io/docs/)
- [LogQL — linguagem de query do Loki](https://grafana.com/docs/loki/latest/query/)
- [Explore no Grafana](https://grafana.com/docs/grafana/latest/explore/)
