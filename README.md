# 📅 Plataforma de Agendamentos

Uma plataforma completa para agendamento de serviços, desenvolvida com ASP.NET Core e Vue.js.

## 🚀 Funcionalidades

### Para Prestadores de Serviço:
- ✅ Cadastro e autenticação JWT
- ✅ Criação de perfil público com slug personalizado
- ✅ Gerenciamento de serviços (CRUD)
- ✅ Configuração de horários disponíveis
- ✅ Visualização e gerenciamento de agendamentos
- ✅ Confirmação/cancelamento de agendamentos

### Para Clientes:
- ✅ Cadastro e autenticação
- ✅ Acesso a perfis públicos via slug
- ✅ Agendamento de serviços
- ✅ Visualização de agendamentos realizados

### Recursos Técnicos:
- ✅ API RESTful com ASP.NET Core 8
- ✅ Autenticação JWT
- ✅ Entity Framework Core com PostgreSQL
- ✅ Swagger/OpenAPI para documentação
- ✅ CORS configurado
- ✅ Validação de dados
- ✅ Estrutura modular e escalável

## 🛠️ Tecnologias Utilizadas

- **Backend**: ASP.NET Core 8
- **Banco de Dados**: PostgreSQL + Entity Framework Core
- **Autenticação**: JWT Bearer Token
- **Documentação**: Swagger/OpenAPI
- **Criptografia**: BCrypt.Net

## 📋 Pré-requisitos

- .NET 8 SDK
- PostgreSQL
- Visual Studio 2022 ou VS Code

## ⚙️ Configuração do Ambiente

### 1. Clone o repositório e instale as dependências

```bash
git clone [seu-repositorio]
cd Plataforma_Agendamentos
dotnet restore
```

### 2. Configure o banco de dados

1. Instale o PostgreSQL
2. Crie um banco de dados: `plataforma_agendamentos_dev`
3. Atualize a connection string em `appsettings.Development.json`

### 3. Execute as migrações

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Execute o projeto

```bash
dotnet run
```

A API estará disponível em: `https://localhost:5001` ou `http://localhost:5000`
A documentação Swagger estará em: `https://localhost:5001`

## 📚 Endpoints da API

### Autenticação
- `POST /api/auth/register` - Cadastro de usuário
- `POST /api/auth/login` - Login

### Perfil
- `GET /api/profile` - Buscar perfil do usuário logado
- `PUT /api/profile` - Atualizar perfil

### Serviços (Prestadores)
- `GET /api/services` - Listar serviços do prestador
- `POST /api/services` - Criar serviço
- `GET /api/services/{id}` - Buscar serviço específico
- `PUT /api/services/{id}` - Atualizar serviço
- `DELETE /api/services/{id}` - Excluir serviço

### Horários (Prestadores)
- `GET /api/schedules` - Listar horários disponíveis
- `POST /api/schedules` - Criar horário
- `PUT /api/schedules/{id}` - Atualizar horário
- `DELETE /api/schedules/{id}` - Excluir horário

### Agendamentos
- `GET /api/bookings` - Listar agendamentos
- `POST /api/bookings` - Criar agendamento (clientes)
- `GET /api/bookings/{id}` - Buscar agendamento específico
- `PUT /api/bookings/{id}/status` - Atualizar status (prestadores)

### Público
- `GET /api/prestador/{slug}` - Buscar prestador por slug (público)
- `GET /api/prestador/{slug}/available-times?date=YYYY-MM-DD` - Horários disponíveis

## 🔐 Autenticação

A API utiliza JWT Bearer Token. Para acessar endpoints protegidos:

1. Faça login em `/api/auth/login`
2. Use o token retornado no header: `Authorization: Bearer {token}`

## 📝 Exemplos de Uso

### Cadastro de Prestador

```json
POST /api/auth/register
{
  "name": "João Silva",
  "email": "joao@email.com",
  "password": "minhasenha123",
  "userType": "prestador"
}
```

### Atualizar Perfil Público

```json
PUT /api/profile
{
  "slug": "joaocarwash",
  "displayName": "João Car Wash",
  "logoUrl": "https://exemplo.com/logo.jpg",
  "coverImageUrl": "https://exemplo.com/capa.jpg",
  "primaryColor": "#1e90ff",
  "bio": "Lavagem ecológica e rápida!"
}
```

### Criar Serviço

```json
POST /api/services
{
  "title": "Lavagem Completa",
  "description": "Lavagem externa e interna",
  "price": 45.00,
  "durationMinutes": 60
}
```

### Agendar Serviço (Cliente)

```json
POST /api/bookings
{
  "serviceId": "guid-do-servico",
  "date": "2024-01-15T14:00:00"
}
```

## 🔮 Próximos Passos

- [ ] Frontend Vue.js
- [ ] Notificações por email
- [ ] Sistema de avaliações
- [ ] Pagamentos online
- [ ] Aplicativo móvel
- [ ] Dashboard com métricas

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch: `git checkout -b feature/nova-funcionalidade`
3. Commit: `git commit -m 'Adiciona nova funcionalidade'`
4. Push: `git push origin feature/nova-funcionalidade`
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo LICENSE para mais detalhes.
