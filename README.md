# FCG Microservices

Sistema de microsserviços para gerenciamento de jogos, usuários, autenticação e pagamentos.

## 🏗️ Arquitetura

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   Auth API  │     │  User API   │     │  Games API  │     │ Payment API │
│   :5000     │     │   :5001     │     │   :5181     │     │   :5002     │
└──────┬──────┘     └──────┬──────┘     └──────┬──────┘     └──────┬──────┘
       │                   │                   │                   │
       └───────────────────┴───────────────────┴───────────────────┘
                                    │
                           ┌────────┴────────┐
                           │    RabbitMQ     │
                           │   :5672/:15672  │
                           └─────────────────┘
```

## 📋 Pré-requisitos

1. **Windows 10/11** com WSL2 habilitado
2. **Docker Desktop** instalado e configurado para usar WSL2
3. **Ubuntu** no WSL (Microsoft Store)

### Instalar Docker Desktop
1. Baixe em: https://www.docker.com/products/docker-desktop
2. Durante a instalação, marque **"Use WSL 2 instead of Hyper-V"**
3. Após instalar, vá em **Settings > Resources > WSL Integration**
4. Ative a integração com sua distribuição Ubuntu

### Verificar instalação
```bash
# No terminal Ubuntu (WSL)
docker --version
docker compose version
```

## 🚀 Como Rodar

### 1. Clonar o repositório
```bash
# No Ubuntu WSL
cd ~
git clone <URL_DO_REPOSITORIO> microservices
cd microservices
```

### 2. Subir os containers
```bash
docker compose up -d --build
```

> ⏳ O primeiro build pode demorar alguns minutos.

### 3. Verificar se está rodando
```bash
docker compose ps
```

Todos os containers devem estar com status `Up`:
- `fcg-rabbitmq`
- `fcg-auth`
- `fcg-user`
- `fcg-games`
- `fcg-payment`

### 4. Testar os serviços
```bash
# Health checks
curl http://localhost:5000/health/live  # Auth
curl http://localhost:5001/health/live  # User
curl http://localhost:5181/api/games/health  # Games
curl http://localhost:5002/health/live  # Payment
```

## 🌐 URLs de Acesso

| Serviço | URL | Descrição |
|---------|-----|-----------|
| Auth API | http://localhost:5000/swagger | Autenticação e login |
| User API | http://localhost:5001/swagger | Gerenciamento de usuários |
| Games API | http://localhost:5181/swagger | Catálogo de jogos |
| Payment API | http://localhost:5002/swagger | Pagamentos |
| RabbitMQ | http://localhost:15672 | Painel de filas (guest/guest) |

## 🧪 Fluxo de Teste

### 1. Criar conta
```bash
curl -X POST http://localhost:5000/auth/create-account \
  -H "Content-Type: application/json" \
  -d '{"name": "Teste", "email": "teste@email.com", "password": "Senha123!", "role": 1}'
```

### 2. Fazer login
```bash
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "teste@email.com", "password": "Senha123!"}'
```

### 3. Listar jogos
```bash
curl http://localhost:5181/api/games
```

### 4. Comprar jogo (comunicação assíncrona via RabbitMQ)
```bash
curl -X POST http://localhost:5002/payments/purchase-game \
  -H "Content-Type: application/json" \
  -d '{"userId": "SEU_USER_ID", "gameId": "ID_DO_JOGO"}'
```

## 🔧 Comandos Úteis

```bash
# Ver logs de todos os serviços
docker compose logs -f

# Ver logs de um serviço específico
docker compose logs -f payment

# Reiniciar um serviço
docker compose restart auth

# Parar todos os serviços
docker compose down

# Parar e remover volumes (limpa dados)
docker compose down -v

# Rebuild completo
docker compose up -d --build --force-recreate
```

## ⚠️ Solução de Problemas

### Erro de timeout no banco de dados
Os bancos PostgreSQL estão hospedados no Neon (cloud) e podem "adormecer" após inatividade. Se receber erro de timeout:
1. Aguarde alguns segundos
2. Tente novamente - o banco irá "acordar"

### Containers não sobem
```bash
# Limpar tudo e recomeçar
docker compose down -v
docker system prune -a -f
docker compose up -d --build
```

### Erro de porta em uso
```bash
# Verificar o que está usando a porta (ex: 5000)
netstat -tulpn | grep 5000

# Ou no Windows PowerShell
netstat -ano | findstr :5000
```

### Sincronizar relógio do WSL (se Prometheus/métricas derem erro de tempo)
```bash
sudo hwclock -s
# ou
sudo ntpdate time.windows.com
```

## 📁 Estrutura do Projeto

```
.
├── docker-compose.yml          # Orquestração dos containers
├── .dockerignore               # Arquivos ignorados no build
├── Fcg.Auth/                   # Microsserviço de Autenticação
├── Fcg.User/                   # Microsserviço de Usuários
├── Fcg.Games/                  # Microsserviço de Jogos
├── Fcg.Payment/                # Microsserviço de Pagamentos
└── Fcg.Shared/                 # Bibliotecas compartilhadas
    └── Fcg.Observability/      # OpenTelemetry e Health Checks
```

## 🛠️ Tecnologias

- **.NET 8** - Framework
- **RabbitMQ** - Mensageria assíncrona
- **PostgreSQL (Neon)** - Banco de dados
- **Docker** - Containerização
- **MassTransit** - Abstração para RabbitMQ
- **OpenTelemetry** - Observabilidade/APM

## 📝 Licença

Este projeto é parte do Tech Challenge FIAP.
