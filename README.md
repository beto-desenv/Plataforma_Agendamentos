# 🗓️ Plataforma de Agendamentos

## 📋 Sobre o Projeto

Sistema completo para gerenciamento de agendamentos de serviços, permitindo que prestadores ofereçam seus serviços e clientes façam reservas de forma prática e organizada.

## 🚀 Status do Projeto

✅ **API Backend funcionando completamente**
- Autenticação JWT implementada
- CRUD completo para usuários, serviços e agendamentos
- Health checks configurados
- Swagger UI disponível
- PostgreSQL integrado
- Middleware de logging e tratamento de erros

## 🛠️ Tecnologias Utilizadas

### **Backend (.NET 8)**
- **ASP.NET Core** - Framework web
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados
- **JWT Bearer** - Autenticação
- **Serilog** - Logging estruturado
- **FluentValidation** - Validação de dados
- **Swagger/OpenAPI** - Documentação da API
- **BCrypt** - Hash de senhas

### **Arquitetura**
- Clean Architecture
- Controllers com padrões REST
- DTOs para transferência de dados
- Services para lógica de negócio
- Middleware customizado
- Validadores centralizados

## 📊 Funcionalidades Implementadas

### **🔐 Autenticação e Autorização**
- [x] Registro de usuários (cliente/prestador)
- [x] Login com JWT
- [x] Middleware de autenticação
- [x] Validação de tokens
- [x] Perfil do usuário

### **👥 Gestão de Usuários**
- [x] Cadastro de clientes e prestadores
- [x] Perfis públicos para prestadores
- [x] Sistema de roles (cliente/prestador)
- [x] Validação de dados completa

### **💼 Serviços**
- [x] CRUD completo de serviços
- [x] Associação com prestadores
- [x] Preços e durações
- [x] Descrições detalhadas

### **📅 Agendamentos**
- [x] Criação de horários disponíveis
- [x] Sistema de reservas
- [x] Status de agendamentos
- [x] Controle por prestador

### **🏥 Monitoramento**
- [x] Health checks da aplicação
- [x] Health checks do banco de dados
- [x] Logging estruturado
- [x] Middleware de rastreamento

## 👥 Papéis (roles) e suas responsabilidades

### **Cliente (`cliente`)**
**O que é:** usuário final que agenda serviços.  
**Para que serve:** permitir que pessoas encontrem serviços e criem reservas.  
**Funcionalidades associadas:**
- Cadastro e login com JWT.
- Acesso ao próprio perfil.
- Listagem de serviços disponíveis.
- Criação de agendamentos.
- Visualização de seus próprios agendamentos.  
**APIs principais:** `/api/auth`, `/api/services`, `/api/schedules`, `/api/bookings`, `/api/auth/profile`.

### **Prestador (`prestador`)**
**O que é:** profissional ou empresa que oferece serviços.  
**Para que serve:** publicar serviços, disponibilizar horários e administrar reservas.  
**Funcionalidades associadas:**
- Cadastro e login com JWT.
- Gestão de serviços (CRUD).
- Gestão de horários disponíveis (CRUD).
- Visualização de reservas recebidas.
- Atualização de status de agendamento (confirmar/cancelar).
- Perfil público com slug e serviços publicados.  
**APIs principais:** `/api/auth`, `/api/services`, `/api/schedules`, `/api/bookings`, `/api/prestador/{slug}`.

## 🧩 Funcionalidades da API (o que são e para que servem)

### **Autenticação e perfis**
- **Registro**: cria usuários com tipo (`cliente` ou `prestador`) e retorna JWT.  
  **Serve para** criar contas e autenticar chamadas futuras.
- **Login**: valida credenciais e retorna JWT.  
  **Serve para** autenticar usuários existentes.
- **Perfil**: retorna dados do usuário autenticado.  
  **Serve para** exibir informações básicas no front-end.

### **Serviços**
- **CRUD de serviços**: prestadores podem criar, editar, listar e remover serviços.  
  **Serve para** expor ofertas com preço/duração/descrição.

### **Horários (Schedules)**
- **CRUD de horários**: prestadores definem dias da semana e janelas de atendimento.  
  **Serve para** limitar reservas a períodos disponíveis.

### **Agendamentos (Bookings)**
- **Criação de reserva**: clientes reservam serviços em horários válidos.  
  **Serve para** garantir disponibilidade e evitar conflitos.
- **Atualização de status**: prestadores confirmam ou cancelam agendamentos.  
  **Serve para** controlar o ciclo do atendimento.

### **Perfil público**
- **Perfil por slug**: expõe dados públicos do prestador e serviços.  
  **Serve para** páginas de divulgação compartilháveis.

### **Monitoramento**
- **Health checks**: disponibilidade da API e banco.  
  **Serve para** observabilidade e diagnóstico.

## 🗄️ Necessidades e alinhamentos de Banco de Dados

### **Entidades principais**
- **User**: dados do usuário e papel (`cliente`/`prestador`), com `Slug` único para prestadores.
- **Service**: serviços publicados por prestadores.
- **Schedule**: janelas de atendimento por prestador (dia da semana e horário).
- **Booking**: reservas feitas por clientes, associadas a serviço e data/hora.

### **Regras e integridade**
- **Unicidade**: `Email` e `Slug` devem ser únicos.
- **Relacionamentos**:
  - `User` (prestador) → `Service` (1:N)
  - `User` (prestador) → `Schedule` (1:N)
  - `Booking` → `User` (cliente) (N:1)
  - `Booking` → `Service` (N:1)
- **Validações**:
  - `Booking` não pode ocorrer no passado.
  - `Booking` não pode conflitar com outra reserva ativa.
  - `Schedule` deve respeitar `StartTime < EndTime`.

### **Migrações e seed**
- Recomenda-se manter migrações do EF Core para versionar o schema.
- Opcional: seed de dados para prestadores/serviços de exemplo em ambiente dev.

## 🧑‍💻 Alinhamento com Front-end (necessidades e fluxo)

### **Autenticação**
- Guardar JWT no client (ex.: storage seguro) e enviar em `Authorization: Bearer {token}`.
- Diferenciar fluxo/menus por `UserType` (`cliente` x `prestador`).

### **Cliente**
- Tela de listagem de serviços (`GET /api/services`).
- Criação de agendamento (`POST /api/bookings`) com data futura válida.
- Lista de agendamentos próprios (`GET /api/bookings`).

### **Prestador**
- CRUD de serviços (`/api/services`).
- CRUD de horários (`/api/schedules`).
- Lista de reservas recebidas (`GET /api/bookings`).
- Atualização de status (`PUT /api/bookings/{id}/status`).

### **Perfil público**
- Página pública baseada em slug (`GET /api/prestador/{slug}`).
- Lista de serviços públicos do prestador (`GET /api/prestador/{slug}/services`).

### **Monitoramento**
- Páginas ou widgets de status consumindo `/api/health` e `/api/health/info` (ambiente interno).

## 🎯 Endpoints da API

### **Autenticação (`/api/auth`)**
```
POST /api/auth/register     # Registro de usuário
POST /api/auth/login        # Login
GET  /api/auth/profile      # Perfil atual
```

### **Serviços (`/api/services`)**
```
GET    /api/services        # Listar serviços
POST   /api/services        # Criar serviço
GET    /api/services/{id}   # Obter serviço
PUT    /api/services/{id}   # Atualizar serviço
DELETE /api/services/{id}   # Deletar serviço
```

### **Agendamentos (`/api/schedules` e `/api/bookings`)**
```
GET  /api/schedules         # Horários disponíveis
POST /api/schedules         # Criar horário
GET  /api/bookings          # Listar reservas
POST /api/bookings          # Fazer reserva
```

### **Perfil Público (`/api/prestador/{slug}`)**
```
GET /api/prestador/{slug}           # Perfil público
GET /api/prestador/{slug}/services  # Serviços do prestador
```

### **Monitoramento**
```
GET /health                 # Status da aplicação
GET /api/health            # Health check detalhado
GET /api/health/ping       # Ping rápido
GET /api/health/info       # Informações detalhadas do sistema
```

## 🚀 Como Executar

### **Pré-requisitos**
- .NET 8 SDK
- PostgreSQL
- Git

### **1. Clonagem e Setup**
```bash
git clone https://github.com/beto-desenv/Plataforma_Agendamentos.git
cd Plataforma_Agendamentos
```

### **2. Configuração do Banco**
```sql
-- Criar usuário e banco no PostgreSQL
CREATE USER plataforma_user WITH PASSWORD '180312';
CREATE DATABASE plataforma_agendamentos_dev;
GRANT ALL PRIVILEGES ON DATABASE plataforma_agendamentos_dev TO plataforma_user;
```

### **3. Execução (Método Mais Simples)**

#### **Windows:**
```cmd
# Execute o script principal
start-swagger.bat
```

#### **PowerShell:**
```powershell
# Ou use o PowerShell
.\test-swagger-final.ps1
```

#### **Manual:**
```bash
cd Plataforma_Agendamentos
dotnet restore
dotnet build
dotnet run --urls="https://localhost:5001;http://localhost:5000"
```

### **4. Acesso**
Após executar, acesse:
- **🏠 Home**: `https://localhost:5001/`
- **📖 Swagger**: `https://localhost:5001/swagger`
- **🏥 Health**: `https://localhost:5001/api/health`

## 🔧 Configuração para Frontend

Use as URLs corretas no seu frontend:

```javascript
// axiosConfig.js
const api = axios.create({
  baseURL: 'https://localhost:5001/api',  // API Backend
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  }
});
```

### **Endpoints Principais:**
- **Health Check**: `GET /api/health`
- **Registro**: `POST /api/auth/register`
- **Login**: `POST /api/auth/login`
- **Perfil**: `GET /api/auth/profile`

Veja `api-config.json` para configuração completa.

## 📁 Estrutura do Projeto

```
Plataforma_Agendamentos/
├── Controllers/           # Controllers da API
│   ├── AuthController.cs     # Autenticação
│   ├── ServicesController.cs # Serviços
│   ├── SchedulesController.cs # Horários
│   ├── BookingsController.cs # Reservas
│   ├── ProfileController.cs  # Perfis
│   ├── PrestadorController.cs # Perfil público
│   └── HealthController.cs   # Monitoramento
├── Models/               # Modelos de dados
├── DTOs/                # Data Transfer Objects
├── Services/            # Serviços de negócio
├── Middleware/          # Middleware customizado
├── Validators/          # Validadores FluentValidation
├── Data/                # Context do Entity Framework
├── Migrations/          # Migrações do banco
├── Properties/          # Configurações do projeto
└── Program.cs           # Entry point da aplicação
```

## 🧪 Testando a API

### **1. Via Swagger UI**
Acesse `https://localhost:5001/swagger` para interface interativa.

### **2. Via cURL**
```bash
# Health check
curl -k https://localhost:5001/api/health

# Registro
curl -k -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "name": "João Silva",
    "email": "joao@email.com",
    "password": "123456",
    "userTypes": ["cliente"]
  }'
```

### **3. Via Postman**
Importe o arquivo `postman_collection.json` (se disponível).

## 🔒 Segurança Implementada

- ✅ Hash de senhas com BCrypt
- ✅ Autenticação JWT
- ✅ Validação de entrada com FluentValidation
- ✅ Headers de segurança HTTP
- ✅ CORS configurado
- ✅ Rate limiting (via middleware)
- ✅ Logs de auditoria

## 📝 Logs e Monitoramento

### **Logs Estruturados**
```bash
[23:16:26 INF] Health check iniciado - RequestId: abc123
[23:16:26 INF] Operação: LOGIN_SUCCESS | Usuário: user@email.com
```

### **Health Checks**
- Aplicação rodando
- Conexão com PostgreSQL
- Uso de memória
- Tempo de resposta

## 🎯 Próximos Passos

### **Backend (Melhorias)**
- [ ] Implementação de cache (Redis)
- [ ] Rate limiting mais avançado
- [ ] Backup automático do banco
- [ ] Métricas com Prometheus
- [ ] Deploy com Docker

### **Frontend (A implementar)**
- [ ] Interface em Vue.js/React
- [ ] Dashboard para prestadores
- [ ] Calendário de agendamentos
- [ ] Notificações em tempo real
- [ ] PWA para mobile

## 🐛 Solução de Problemas

### **Aplicação não inicia?**
```cmd
# Execute o diagnóstico
.\COMO-EXECUTAR.md
```

### **PostgreSQL não conecta?**
1. Verifique se o PostgreSQL está rodando
2. Confirme usuário e senha
3. Teste conexão: `psql -h localhost -U plataforma_user`

### **Swagger não carrega?**
1. Verifique se aplicação iniciou completamente
2. Tente HTTP: `http://localhost:5000/swagger`
3. Aceite certificado SSL no navegador

## 📞 Contato

**Desenvolvedor:** Beto Vieira Carlos
**Email:** beto.vieiracarlos@gmail.com
**GitHub:** [beto-desenv](https://github.com/beto-desenv)

## 📄 Licença

Este projeto está sob licença MIT. Veja LICENSE para mais detalhes.

---

**🎉 Projeto funcionando completamente! Ready para produção!** 🚀
