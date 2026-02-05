# Plataforma de Agendamentos

Sistema completo para gerenciamento de agendamentos de servicos, permitindo que prestadores oferecam seus servicos e clientes facam reservas de forma pratica e organizada.

## Status do Projeto

**API Backend 100% funcional** com:
- Autenticacao JWT + BCrypt
- CRUD completo para usuarios (cliente/prestador), servicos e agendamentos
- PostgreSQL + Entity Framework
- Swagger UI para documentacao
- Health checks e monitoramento
- Middleware de logging e tratamento de erros

## Tecnologias

**Backend (.NET 8):**
- ASP.NET Core - Framework web
- Entity Framework Core - ORM
- PostgreSQL - Banco de dados
- JWT Bearer - Autenticacao
- Serilog - Logging estruturado
- FluentValidation - Validacao de dados
- Swagger/OpenAPI - Documentacao
- BCrypt - Hash de senhas

## Papeis de Usuario

### Cliente (`cliente`)
Usuario final que agenda servicos.

**Funcionalidades:**
- Cadastro e login
- Atualizacao de perfil (telefone, endereco, CPF, data de nascimento)
- Listagem de servicos disponiveis
- Criacao de agendamentos
- Visualizacao de agendamentos proprios

**Campos especificos:**
- `TelefoneCliente`
- `DataNascimento`
- `EnderecoCliente`
- `CPF`
- `PreferenciasNotificacao`
- `TotalAgendamentosCliente`
- `UltimoAgendamento`

### Prestador (`prestador`)
Profissional ou empresa que oferece servicos.

**Funcionalidades:**
- Cadastro e login
- Atualizacao de perfil completo (slug, bio, logo, cores, etc)
- Gestao de servicos (CRUD)
- Gestao de horarios disponiveis (CRUD)
- Visualizacao de reservas recebidas
- Atualizacao de status de agendamento
- Perfil publico com slug

**Campos especificos:**
- `Slug` (URL personalizada)
- `DisplayName` (nome de exibicao)
- `Bio` (biografia/descricao)
- `LogoUrl` (logo do negocio)
- `CoverImageUrl` (imagem de capa)
- `PrimaryColor` (cor principal da marca)
- `CNPJ`
- `TelefonePrestador`
- `EnderecoPrestador`
- `Site`
- `AvaliacaoMedia`
- `TotalServicos`
- `TotalAgendamentosPrestador`
- `AceitaAgendamentoImediato`
- `HorasAntecedenciaMinima`
- `PerfilAtivo`
- `HorarioInicioSemana`
- `HorarioFimSemana`

## Fluxo Completo de Uso

### 1. Criar Usuario Cliente

**Endpoint:** `POST /api/auth/register`

**Request:**
```json
{
  "name": "Joao Silva",
  "email": "joao@email.com",
  "password": "123456",
  "userTypes": ["cliente"]
}
```

**Response:**
```json
{
  "success": true,
  "message": "Usuario registrado com sucesso",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": "guid-do-usuario",
      "name": "Joao Silva",
      "email": "joao@email.com",
      "userType": "cliente"
    }
  }
}
```

**Importante:** Guardar o `token` para usar em requisicoes futuras.

### 2. Atualizar Perfil do Cliente

**Endpoint:** `PUT /api/profile/cliente`

**Headers:**
```
Authorization: Bearer {token_do_cliente}
Content-Type: application/json
```

**Request:**
```json
{
  "name": "Joao Silva Santos",
  "telefoneCliente": "(11) 98765-4321",
  "dataNascimento": "1990-01-15T00:00:00Z",
  "enderecoCliente": "Rua Exemplo, 123 - Sao Paulo/SP",
  "cpf": "123.456.789-00",
  "preferenciasNotificacao": "email,sms"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Perfil de cliente atualizado com sucesso",
  "data": {
    "id": "guid-do-usuario",
    "name": "Joao Silva Santos",
    "email": "joao@email.com",
    "userType": "cliente",
    "dadosCliente": {
      "telefoneCliente": "(11) 98765-4321",
      "dataNascimento": "1990-01-15T00:00:00Z",
      "enderecoCliente": "Rua Exemplo, 123 - Sao Paulo/SP",
      "cpf": "123.456.789-00",
      "preferenciasNotificacao": "email,sms",
      "totalAgendamentosCliente": 0
    }
  }
}
```

### 3. Criar Usuario Prestador

**Endpoint:** `POST /api/auth/register`

**Request:**
```json
{
  "name": "Maria Santos",
  "email": "maria@email.com",
  "password": "123456",
  "userTypes": ["prestador"]
}
```

**Response:**
```json
{
  "success": true,
  "message": "Usuario registrado com sucesso",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": "guid-do-prestador",
      "name": "Maria Santos",
      "email": "maria@email.com",
      "userType": "prestador"
    }
  }
}
```

### 4. Atualizar Perfil do Prestador

**Endpoint:** `PUT /api/profile/prestador`

**Headers:**
```
Authorization: Bearer {token_do_prestador}
Content-Type: application/json
```

**Request:**
```json
{
  "name": "Maria Santos",
  "displayName": "Salao da Maria",
  "slug": "salao-da-maria",
  "bio": "Salao especializado em cortes modernos, coloracao e tratamentos capilares. Atendemos ha mais de 10 anos em Sao Paulo.",
  "telefonePrestador": "(11) 91234-5678",
  "enderecoPrestador": "Av. Principal, 456 - Sao Paulo/SP",
  "cnpj": "12.345.678/0001-90",
  "site": "https://salaodamaria.com.br",
  "primaryColor": "#FF5733",
  "logoUrl": "https://exemplo.com/logo.png",
  "coverImageUrl": "https://exemplo.com/capa.jpg",
  "aceitaAgendamentoImediato": true,
  "horasAntecedenciaMinima": 2,
  "perfilAtivo": true,
  "horarioInicioSemana": "08:00",
  "horarioFimSemana": "18:00"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Perfil de prestador atualizado com sucesso",
  "data": {
    "id": "guid-do-prestador",
    "name": "Maria Santos",
    "email": "maria@email.com",
    "userType": "prestador",
    "dadosPrestador": {
      "slug": "salao-da-maria",
      "displayName": "Salao da Maria",
      "bio": "Salao especializado em cortes modernos...",
      "telefonePrestador": "(11) 91234-5678",
      "enderecoPrestador": "Av. Principal, 456 - Sao Paulo/SP",
      "site": "https://salaodamaria.com.br",
      "primaryColor": "#FF5733",
      "logoUrl": "https://exemplo.com/logo.png",
      "coverImageUrl": "https://exemplo.com/capa.jpg",
      "cnpj": "12.345.678/0001-90",
      "aceitaAgendamentoImediato": true,
      "horasAntecedenciaMinima": 2,
      "perfilAtivo": true,
      "horarioInicioSemana": "08:00",
      "horarioFimSemana": "18:00",
      "publicUrl": "/prestador/salao-da-maria",
      "perfilCompleto": true,
      "avaliacaoMedia": 0,
      "totalServicos": 0,
      "totalAgendamentosPrestador": 0
    }
  }
}
```

### 5. Criar Servico (Prestador)

**Endpoint:** `POST /api/services`

**Headers:**
```
Authorization: Bearer {token_do_prestador}
Content-Type: application/json
```

**Request:**
```json
{
  "title": "Corte de Cabelo Masculino",
  "description": "Corte moderno com finalizacao",
  "price": 45.00,
  "durationMinutes": 60
}
```

### 6. Criar Horario Disponivel (Prestador)

**Endpoint:** `POST /api/schedules`

**Headers:**
```
Authorization: Bearer {token_do_prestador}
Content-Type: application/json
```

**Request:**
```json
{
  "dayOfWeek": 1,
  "startTime": "08:00:00",
  "endTime": "18:00:00"
}
```

### 7. Fazer Agendamento (Cliente)

**Endpoint:** `POST /api/bookings`

**Headers:**
```
Authorization: Bearer {token_do_cliente}
Content-Type: application/json
```

**Request:**
```json
{
  "serviceId": "guid-do-servico",
  "date": "2024-12-20T10:00:00Z",
  "notes": "Prefiro corte mais curto nas laterais"
}
```

### 8. Ver Perfil Publico do Prestador (Sem autenticacao)

**Endpoint:** `GET /api/prestador/{slug}`

**Exemplo:** `GET /api/prestador/salao-da-maria`

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "guid-do-prestador",
    "name": "Maria Santos",
    "displayName": "Salao da Maria",
    "slug": "salao-da-maria",
    "bio": "Salao especializado...",
    "logoUrl": "https://exemplo.com/logo.png",
    "coverImageUrl": "https://exemplo.com/capa.jpg",
    "primaryColor": "#FF5733"
  }
}
```

## Endpoints da API

### Autenticacao (`/api/auth`)
```
POST /api/auth/register     # Registro de usuario (cliente ou prestador)
POST /api/auth/login        # Login
GET  /api/auth/profile      # Perfil do usuario autenticado
```

### Perfil (`/api/profile`)
```
GET  /api/profile           # Ver perfil completo (cliente ou prestador)
PUT  /api/profile/cliente   # Atualizar perfil de cliente
PUT  /api/profile/prestador # Atualizar perfil de prestador
```

### Servicos (`/api/services`)
```
GET    /api/services        # Listar servicos
POST   /api/services        # Criar servico (prestador)
GET    /api/services/{id}   # Obter servico
PUT    /api/services/{id}   # Atualizar servico (prestador)
DELETE /api/services/{id}   # Deletar servico (prestador)
```

### Horarios (`/api/schedules`)
```
GET  /api/schedules         # Listar horarios disponiveis
POST /api/schedules         # Criar horario (prestador)
```

### Agendamentos (`/api/bookings`)
```
GET  /api/bookings          # Listar agendamentos
POST /api/bookings          # Criar agendamento (cliente)
```

### Perfil Publico (`/api/prestador`)
```
GET /api/prestador/{slug}           # Ver perfil publico do prestador
GET /api/prestador/{slug}/services  # Ver servicos do prestador
```

### Monitoramento
```
GET /health                 # Health check simples
GET /api/health            # Health check detalhado
GET /api/health/ping       # Ping rapido
GET /api/health/info       # Informacoes do sistema
```

## Como Executar

### Pre-requisitos
- .NET 8 SDK
- PostgreSQL

### Setup do Banco
```sql
CREATE USER plataforma_user WITH PASSWORD '180312';
CREATE DATABASE plataforma_agendamentos_dev;
GRANT ALL PRIVILEGES ON DATABASE plataforma_agendamentos_dev TO plataforma_user;
```

### Executar
```bash
cd Plataforma_Agendamentos
dotnet restore
dotnet build
dotnet run
```

### Acessar
- **Home**: `https://localhost:5001/`
- **Swagger**: `https://localhost:5001/swagger`
- **Health**: `https://localhost:5001/api/health`

## Implementacao no Frontend

### 1. Configuracao do Axios

```javascript
// src/services/api.js
import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:5001/api',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  }
});

// Interceptor para adicionar token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

export default api;
```

### 2. Servico de Autenticacao

```javascript
// src/services/authService.js
import api from './api';

export const authService = {
  // Registro de cliente
  async registerCliente(data) {
    const response = await api.post('/auth/register', {
      name: data.name,
      email: data.email,
      password: data.password,
      userTypes: ['cliente']
    });
    
    if (response.data.success) {
      localStorage.setItem('token', response.data.data.token);
      localStorage.setItem('user', JSON.stringify(response.data.data.user));
    }
    
    return response.data;
  },

  // Registro de prestador
  async registerPrestador(data) {
    const response = await api.post('/auth/register', {
      name: data.name,
      email: data.email,
      password: data.password,
      userTypes: ['prestador']
    });
    
    if (response.data.success) {
      localStorage.setItem('token', response.data.data.token);
      localStorage.setItem('user', JSON.stringify(response.data.data.user));
    }
    
    return response.data;
  },

  // Login
  async login(email, password) {
    const response = await api.post('/auth/login', { email, password });
    
    if (response.data.success) {
      localStorage.setItem('token', response.data.data.token);
      localStorage.setItem('user', JSON.stringify(response.data.data.user));
    }
    
    return response.data;
  },

  // Logout
  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  },

  // Verificar se esta autenticado
  isAuthenticated() {
    return !!localStorage.getItem('token');
  },

  // Obter usuario atual
  getCurrentUser() {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  },

  // Verificar tipo de usuario
  isCliente() {
    const user = this.getCurrentUser();
    return user?.userType === 'cliente';
  },

  isPrestador() {
    const user = this.getCurrentUser();
    return user?.userType === 'prestador';
  }
};
```

### 3. Servico de Perfil

```javascript
// src/services/profileService.js
import api from './api';

export const profileService = {
  // Obter perfil
  async getProfile() {
    const response = await api.get('/profile');
    return response.data;
  },

  // Atualizar perfil de cliente
  async updateClienteProfile(data) {
    const response = await api.put('/profile/cliente', data);
    return response.data;
  },

  // Atualizar perfil de prestador
  async updatePrestadorProfile(data) {
    const response = await api.put('/profile/prestador', data);
    return response.data;
  }
};
```

### 4. Componente de Registro (Vue.js)

```vue
<!-- src/views/Register.vue -->
<template>
  <div class="register-page">
    <h1>Criar Conta</h1>
    
    <!-- Escolher tipo de usuario -->
    <div class="user-type-selector">
      <button 
        @click="userType = 'cliente'" 
        :class="{ active: userType === 'cliente' }"
      >
        Sou Cliente
      </button>
      <button 
        @click="userType = 'prestador'" 
        :class="{ active: userType === 'prestador' }"
      >
        Sou Prestador
      </button>
    </div>

    <!-- Formulario -->
    <form @submit.prevent="handleRegister">
      <input 
        v-model="form.name" 
        type="text" 
        placeholder="Nome completo" 
        required 
      />
      <input 
        v-model="form.email" 
        type="email" 
        placeholder="E-mail" 
        required 
      />
      <input 
        v-model="form.password" 
        type="password" 
        placeholder="Senha" 
        required 
      />
      
      <button type="submit">Criar Conta</button>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { authService } from '@/services/authService';

const router = useRouter();
const userType = ref('cliente');
const form = ref({
  name: '',
  email: '',
  password: ''
});

const handleRegister = async () => {
  try {
    if (userType.value === 'cliente') {
      await authService.registerCliente(form.value);
      router.push('/perfil/cliente');
    } else {
      await authService.registerPrestador(form.value);
      router.push('/perfil/prestador');
    }
  } catch (error) {
    console.error('Erro ao registrar:', error);
    alert('Erro ao criar conta');
  }
};
</script>
```

### 5. Componente de Perfil do Cliente

```vue
<!-- src/views/ClienteProfile.vue -->
<template>
  <div class="cliente-profile">
    <h1>Meu Perfil</h1>
    
    <form @submit.prevent="handleUpdate">
      <input v-model="form.name" type="text" placeholder="Nome" />
      <input v-model="form.telefoneCliente" type="tel" placeholder="Telefone" />
      <input v-model="form.dataNascimento" type="date" placeholder="Data de Nascimento" />
      <input v-model="form.enderecoCliente" type="text" placeholder="Endereco" />
      <input v-model="form.cpf" type="text" placeholder="CPF" />
      
      <button type="submit">Atualizar Perfil</button>
    </form>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { profileService } from '@/services/profileService';

const form = ref({
  name: '',
  telefoneCliente: '',
  dataNascimento: '',
  enderecoCliente: '',
  cpf: ''
});

onMounted(async () => {
  const profile = await profileService.getProfile();
  if (profile.data.dadosCliente) {
    form.value = { ...profile.data, ...profile.data.dadosCliente };
  }
});

const handleUpdate = async () => {
  try {
    await profileService.updateClienteProfile(form.value);
    alert('Perfil atualizado com sucesso!');
  } catch (error) {
    console.error('Erro ao atualizar:', error);
    alert('Erro ao atualizar perfil');
  }
};
</script>
```

### 6. Componente de Perfil do Prestador

```vue
<!-- src/views/PrestadorProfile.vue -->
<template>
  <div class="prestador-profile">
    <h1>Meu Perfil Profissional</h1>
    
    <form @submit.prevent="handleUpdate">
      <input v-model="form.name" type="text" placeholder="Nome" />
      <input v-model="form.displayName" type="text" placeholder="Nome do Negocio" />
      <input v-model="form.slug" type="text" placeholder="URL Personalizada" />
      <textarea v-model="form.bio" placeholder="Biografia"></textarea>
      <input v-model="form.telefonePrestador" type="tel" placeholder="Telefone" />
      <input v-model="form.enderecoPrestador" type="text" placeholder="Endereco" />
      <input v-model="form.cnpj" type="text" placeholder="CNPJ" />
      <input v-model="form.site" type="url" placeholder="Site" />
      <input v-model="form.primaryColor" type="color" placeholder="Cor Principal" />
      
      <button type="submit">Atualizar Perfil</button>
    </form>
    
    <div class="preview">
      <h2>Seu Perfil Publico:</h2>
      <a :href="`/prestador/${form.slug}`" target="_blank">
        Ver Perfil Publico
      </a>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { profileService } from '@/services/profileService';

const form = ref({
  name: '',
  displayName: '',
  slug: '',
  bio: '',
  telefonePrestador: '',
  enderecoPrestador: '',
  cnpj: '',
  site: '',
  primaryColor: '#007bff'
});

onMounted(async () => {
  const profile = await profileService.getProfile();
  if (profile.data.dadosPrestador) {
    form.value = { ...profile.data, ...profile.data.dadosPrestador };
  }
});

const handleUpdate = async () => {
  try {
    await profileService.updatePrestadorProfile(form.value);
    alert('Perfil atualizado com sucesso!');
  } catch (error) {
    console.error('Erro ao atualizar:', error);
    alert('Erro ao atualizar perfil');
  }
};
</script>
```

## Estrutura Recomendada do Frontend

```
frontend/
├── src/
│   ├── services/
│   │   ├── api.js                  # Configuracao do Axios
│   │   ├── authService.js          # Autenticacao
│   │   ├── profileService.js       # Perfis
│   │   ├── serviceService.js       # Servicos
│   │   └── bookingService.js       # Agendamentos
│   ├── views/
│   │   ├── Register.vue            # Registro
│   │   ├── Login.vue               # Login
│   │   ├── ClienteProfile.vue      # Perfil do Cliente
│   │   ├── PrestadorProfile.vue    # Perfil do Prestador
│   │   ├── Services.vue            # Listagem de Servicos
│   │   ├── Bookings.vue            # Agendamentos
│   │   └── PrestadorPublic.vue     # Perfil Publico
│   ├── components/
│   │   ├── Navbar.vue              # Barra de navegacao
│   │   ├── ServiceCard.vue         # Card de servico
│   │   └── Calendar.vue            # Calendario de agendamentos
│   ├── router/
│   │   └── index.js                # Rotas
│   ├── store/
│   │   └── auth.js                 # Estado de autenticacao (Pinia/Vuex)
│   └── App.vue
└── package.json
```

## Proximos Passos no Frontend

1. **Implementar autenticacao completa**
   - Telas de registro/login
   - Guardas de rota (verificar autenticacao)
   - Diferenciar menus por tipo de usuario

2. **Dashboard do Cliente**
   - Busca de prestadores
   - Listagem de servicos
   - Calendario para agendamento
   - Historico de agendamentos

3. **Dashboard do Prestador**
   - Gestao de servicos (CRUD)
   - Gestao de horarios (CRUD)
   - Visualizacao de agendamentos recebidos
   - Atualizacao de status dos agendamentos

4. **Perfil Publico do Prestador**
   - Pagina com slug personalizada
   - Listagem de servicos
   - Botao de agendamento direto

5. **Funcionalidades Avancadas**
   - Notificacoes em tempo real
   - Sistema de avaliacoes
   - Upload de imagens (logo/capa)
   - Pagamento integrado

## Contato

**Desenvolvedor:** Beto Vieira Carlos  
**Email:** beto.vieiracarlos@gmail.com  
**GitHub:** [beto-desenv](https://github.com/beto-desenv)

---

**Backend completo e pronto para integracao com frontend!**
