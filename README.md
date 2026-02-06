# Plataforma de Agendamentos

Sistema completo de gerenciamento de agendamentos para prestadores de servicos e clientes.

## Status do Projeto

**Backend 100% funcional** com autenticacao JWT, CRUD completo, PostgreSQL e documentacao Swagger.

## Tecnologias

- **Backend**: .NET 8, ASP.NET Core, Entity Framework Core
- **Banco de Dados**: PostgreSQL
- **Autenticacao**: JWT + BCrypt
- **Documentacao**: Swagger/OpenAPI
- **Validacao**: FluentValidation
- **Logging**: Serilog

## Funcionalidades

### Autenticacao
- Registro de usuarios (cliente/prestador)
- Login com JWT
- Perfis diferenciados

### Gestao de Usuarios
- **Cliente**: Telefone, CPF, endereco, data de nascimento
- **Prestador**: Slug, bio, logo, CNPJ, avaliacao, servicos

### Servicos
- CRUD completo de servicos
- Associacao com prestadores
- Precos e duracoes

### Agendamentos
- Sistema de horarios disponiveis
- Reservas com validacao de conflitos
- Status de agendamentos

## Como Executar

### Pre-requisitos
- .NET 8 SDK
- PostgreSQL

### 1. Configurar Banco de Dados
```sql
CREATE USER plataforma_user WITH PASSWORD '180312';
CREATE DATABASE plataforma_agendamentos_dev;
GRANT ALL PRIVILEGES ON DATABASE plataforma_agendamentos_dev TO plataforma_user;
```

### 2. Iniciar Backend

#### Visual Studio:
1. Abra `Plataforma_Agendamentos.sln`
2. Pressione **F5**
3. Swagger abrira em `https://localhost:5001/swagger`

#### Linha de Comando:
```bash
cd Plataforma_Agendamentos
dotnet run
```

### 3. Acessar
- **Raiz**: `https://localhost:5001/` (redireciona para Swagger)
- **Swagger**: `https://localhost:5001/swagger`
- **Health Check**: `https://localhost:5001/api/health`
- **API Info**: `https://localhost:5001/api/health/info` (lista todos os endpoints)
## Estrutura de Usuarios

### Cliente
**Campos especificos:**
- TelefoneCliente, DataNascimento, EnderecoCliente
- CPF, PreferenciasNotificacao
- TotalAgendamentosCliente

**Endpoints:**
- `PUT /api/profile/cliente` - Atualizar perfil

### Prestador
**Campos especificos:**
- Slug, DisplayName, Bio, LogoUrl, CoverImageUrl
- CNPJ, Site, PrimaryColor
- AvaliacaoMedia, TotalServicos
- HorasAntecedenciaMinima, PerfilAtivo

**Endpoints:**
- `PUT /api/profile/prestador` - Atualizar perfil
- `GET /api/prestador/{slug}` - Perfil publico

## Endpoints Principais

### Autenticacao
```
POST /api/auth/register  - Cadastro
POST /api/auth/login     - Login
```

### Health Checks
```
GET  /api/health         - Status detalhado da API
GET  /api/health/ping    - Verificacao rapida
GET  /api/health/info    - Informacoes do sistema
```

### Perfil
```
GET  /api/profile           - Ver perfil
PUT  /api/profile/cliente   - Atualizar cliente
PUT  /api/profile/prestador - Atualizar prestador
```

### Servicos
```
GET    /api/services     - Listar
POST   /api/services     - Criar
PUT    /api/services/{id} - Atualizar
DELETE /api/services/{id} - Deletar
```

### Agendamentos
```
GET  /api/schedules  - Horarios disponiveis
POST /api/schedules  - Criar horario
GET  /api/bookings   - Listar reservas
POST /api/bookings   - Criar reserva
```

## Configuracao para Frontend

### Axios
```javascript
import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:5001/api',
  headers: {
    'Content-Type': 'application/json'
  }
});

// Interceptor para token
api.interceptors.request.use(config => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default api;
```

### CORS
Backend configurado para aceitar:
- `http://localhost:3000` (React/Next.js)
- `http://localhost:5173` (Vite)

## Exemplos de Uso

### Registro de Cliente
```json
POST /api/auth/register
{
  "name": "Joao Silva",
  "email": "joao@email.com",
  "password": "123456",
  "userTypes": ["cliente"]
}
```

### Atualizar Perfil de Prestador
```json
PUT /api/profile/prestador
{
  "displayName": "Salao da Maria",
  "slug": "salao-da-maria",
  "bio": "Salao especializado em cortes modernos",
  "cnpj": "12.345.678/0001-90",
  "primaryColor": "#FF5733"
}
```

## Seguranca

- Hash de senhas com BCrypt
- Autenticacao JWT com expiracao
- Validacao de entrada com FluentValidation
- Headers de seguranca HTTP
- CORS configurado
- Logs de auditoria

## Estrutura do Projeto

```
Plataforma_Agendamentos/
├── Controllers/          # Endpoints da API
├── Models/              # Modelos de dados
├── DTOs/                # Data Transfer Objects
├── Services/            # Logica de negocio
├── Data/                # Entity Framework Context
├── Migrations/          # Migracoes do banco
├── Middleware/          # Middleware customizado
├── Validators/          # Validadores FluentValidation
└── Constants/           # Constantes do sistema
```

## Testes

```bash
cd Plataforma_Agendamentos.Tests
dotnet test
```

## Contato

**Desenvolvedor**: Beto Vieira Carlos  
**Email**: beto.vieiracarlos@gmail.com  
**GitHub**: [beto-desenv](https://github.com/beto-desenv)

## Licenca

Este projeto esta sob licenca MIT.

---

**Backend completo e pronto para integracao!** 🚀
