# ? STATUS DE ALINHAMENTO - BACKEND PRESTADOR

## ?? CHECKLIST COMPLETO

### ? **1. Campo `descricao` (Bio)**

**Status:** ? **100% IMPLEMENTADO**

#### DTO:
```csharp
// UpdatePrestadorProfileRequest.cs
public string? Bio { get; set; }
```

#### Model:
```csharp
// PrestadorPerfil.cs
public string? Bio { get; set; }
```

#### Service:
```csharp
// ProfileService.UpdatePrestadorProfileAsync()
if (!string.IsNullOrEmpty(request.Bio))
    perfil.Bio = request.Bio.Trim();
```

#### Response:
```json
{
  "dadosPrestador": {
    "bio": "Descrição do prestador"
  }
}
```

? **Recebe no PUT**
? **Armazena no banco**
? **Retorna no response**

---

### ? **2. Campo `document` (CPF/CNPJ)**

**Status:** ? **100% IMPLEMENTADO**

#### DTO:
```csharp
// UpdatePrestadorProfileRequest.cs
public string? CPF { get; set; }
public string? CNPJ { get; set; }
```

#### Model:
```csharp
// PrestadorPerfil.cs
public string? CPF { get; set; }
public string? CNPJ { get; set; }
```

#### Service:
```csharp
// ProfileService.UpdatePrestadorProfileAsync()
if (!string.IsNullOrEmpty(request.CPF))
    perfil.CPF = request.CPF.Trim();

if (!string.IsNullOrEmpty(request.CNPJ))
    perfil.CNPJ = request.CNPJ.Trim();
```

#### Response:
```json
{
  "dadosPrestador": {
    "cpf": "123.456.789-00",
    "cnpj": "12.345.678/0001-90"
  }
}
```

? **Recebe CPF e CNPJ separados**
? **Armazena no banco**
? **Retorna no response**

**Nota:** Backend aceita ambos os campos. Frontend deve enviar apenas um por vez baseado no `documentType`.

---

### ? **3. Campo `fotoPerfilUrl` (Imagem)**

**Status:** ? **100% IMPLEMENTADO**

#### DTO:
```csharp
// UpdatePrestadorProfileRequest.cs
public string? FotoPerfilUrl { get; set; }
```

#### Model:
```csharp
// User.cs (base)
public string? FotoPerfilUrl { get; set; }
```

#### Service:
```csharp
// ProfileService.UpdatePrestadorProfileAsync()
if (!string.IsNullOrEmpty(request.FotoPerfilUrl))
    user.FotoPerfilUrl = request.FotoPerfilUrl.Trim();
```

#### Response:
```json
{
  "fotoPerfilUrl": "https://exemplo.com/avatar.jpg",
  "dadosPrestador": { ... }
}
```

? **Recebe URL da foto**
? **Armazena no User.FotoPerfilUrl**
? **Retorna no response**

**Implementação Atual:**
- Backend recebe URL da imagem (string)
- Frontend deve fazer upload para serviço externo (Cloudinary, S3, etc)
- Frontend envia URL no `fotoPerfilUrl`

**Alternativa (se precisar upload direto):**
- Criar endpoint `POST /api/profile/avatar`
- Receber `IFormFile`
- Salvar em `/wwwroot/uploads/avatars/`
- Retornar URL completa

---

### ? **4. Campo `servicos`**

**Status:** ? **100% IMPLEMENTADO**

#### Response de GET /api/profile:
```json
{
  "dadosPrestador": { ... },
  "services": [
    {
      "id": "uuid",
      "nome": "Corte de Cabelo",
      "description": "Corte masculino tradicional",
      "preco": 50.00,
      "durationMinutes": 30
    }
  ]
}
```

#### Service:
```csharp
// ProfileService.GetProfileAsync()
Services = user.Services.Select(s => new
{
    s.Id,
    s.Nome,
    s.Description,
    s.Preco,
    s.DurationMinutes
})
```

? **Retorna array de serviços**
? **Inclui id, nome, preco, description, durationMinutes**
? **Apenas serviços do prestador autenticado**

---

## ?? RESUMO COMPLETO

| Campo | Status | DTO | Model | Service | Response |
|-------|--------|-----|-------|---------|----------|
| **Bio (descricao)** | ? 100% | ? | ? | ? | ? |
| **CPF** | ? 100% | ? | ? | ? | ? |
| **CNPJ** | ? 100% | ? | ? | ? | ? |
| **FotoPerfilUrl** | ? 100% | ? | ? | ? | ? |
| **Services** | ? 100% | N/A | ? | ? | ? |

---

## ?? ENDPOINTS DISPONÍVEIS

### **PUT /api/profile/prestador**

**Request:**
```json
{
  "name": "João Silva",
  "displayName": "Salão do João",
  "slug": "salao-do-joao",
  "tituloProfissional": "Barbeiro",
  "bio": "Especialista em cortes masculinos com 10 anos de experiência",
  "cpf": "123.456.789-00",
  "cnpj": null,
  "anosExperiencia": 10,
  "telefone": "(11) 98765-4321",
  "endereco": "Rua das Flores, 123",
  "cidade": "São Paulo",
  "estado": "SP",
  "cep": "01234-567",
  "site": "https://salaodojoao.com.br",
  "raioAtendimento": 10,
  "aceitaAgendamentoImediato": true,
  "horasAntecedenciaMinima": 2,
  "fotoPerfilUrl": "https://exemplo.com/avatar.jpg",
  "logoUrl": "https://exemplo.com/logo.png",
  "coverImageUrl": "https://exemplo.com/cover.jpg",
  "primaryColor": "#FF5733"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Perfil de prestador atualizado com sucesso",
  "data": {
    "id": "uuid",
    "name": "João Silva",
    "email": "joao@exemplo.com",
    "userType": "prestador",
    "fotoPerfilUrl": "https://exemplo.com/avatar.jpg",
    "dadosPrestador": {
      "slug": "salao-do-joao",
      "displayName": "Salão do João",
      "tituloProfissional": "Barbeiro",
      "bio": "Especialista em cortes masculinos com 10 anos de experiência",
      "cpf": "123.456.789-00",
      "cnpj": null,
      "anosExperiencia": 10,
      "telefone": "(11) 98765-4321",
      "endereco": "Rua das Flores, 123",
      "cidade": "São Paulo",
      "estado": "SP",
      "cep": "01234-567",
      "site": "https://salaodojoao.com.br",
      "raioAtendimento": 10,
      "aceitaAgendamentoImediato": true,
      "horasAntecedenciaMinima": 2,
      "horarioInicioSemana": null,
      "horarioFimSemana": null,
      "publicUrl": "/prestador/salao-do-joao",
      "perfilCompleto": false,
      "branding": {
        "logoUrl": "https://exemplo.com/logo.png",
        "coverImageUrl": "https://exemplo.com/cover.jpg",
        "primaryColor": "#FF5733"
      },
      "metricas": {
        "avaliacaoMedia": 0,
        "totalServicos": 0,
        "totalAgendamentos": 0
      }
    }
  }
}
```

---

### **GET /api/profile**

**Response:**
```json
{
  "success": true,
  "message": "Perfil obtido com sucesso",
  "data": {
    "id": "uuid",
    "name": "João Silva",
    "email": "joao@exemplo.com",
    "userType": "prestador",
    "fotoPerfilUrl": "https://exemplo.com/avatar.jpg",
    "createdAt": "2024-01-01T00:00:00Z",
    "dadosPrestador": {
      "slug": "salao-do-joao",
      "displayName": "Salão do João",
      "tituloProfissional": "Barbeiro",
      "bio": "Especialista em cortes masculinos",
      "cpf": "123.456.789-00",
      "cnpj": null,
      // ... outros campos
      "branding": { ... },
      "metricas": { ... }
    },
    "services": [
      {
        "id": "uuid",
        "nome": "Corte Masculino",
        "description": "Corte tradicional",
        "preco": 50.00,
        "durationMinutes": 30
      }
    ],
    "schedules": [
      {
        "id": "uuid",
        "dayOfWeek": 1,
        "startTime": "09:00:00",
        "endTime": "18:00:00"
      }
    ]
  }
}
```

---

### **GET /api/prestador/{slug}** (Público - sem autenticação)

**Response:**
```json
{
  "slug": "salao-do-joao",
  "displayName": "Salão do João",
  "tituloProfissional": "Barbeiro",
  "bio": "Especialista em cortes masculinos",
  "site": "https://salaodojoao.com.br",
  "telefone": "(11) 98765-4321",
  "cidade": "São Paulo",
  "estado": "SP",
  "logoUrl": "https://exemplo.com/logo.png",
  "coverImageUrl": "https://exemplo.com/cover.jpg",
  "primaryColor": "#FF5733",
  "services": [
    {
      "id": "uuid",
      "nome": "Corte Masculino",
      "description": "Corte tradicional",
      "preco": 50.00,
      "durationMinutes": 30
    }
  ],
  "schedules": [
    {
      "id": "uuid",
      "dayOfWeek": 1,
      "startTime": "09:00:00",
      "endTime": "18:00:00"
    }
  ]
}
```

---

## ?? GERENCIAMENTO DE SERVIÇOS

### **POST /api/services** (Criar serviço)

**Request:**
```json
{
  "title": "Corte Masculino",
  "description": "Corte tradicional com máquina e tesoura",
  "price": 50.00,
  "durationMinutes": 30
}
```

**Response:**
```json
{
  "id": "uuid",
  "nome": "Corte Masculino",
  "description": "Corte tradicional com máquina e tesoura",
  "preco": 50.00,
  "durationMinutes": 30
}
```

---

## ?? DIFERENÇAS ENTRE FOTO DE PERFIL E LOGO

| Campo | Localização | Propósito | Uso |
|-------|-------------|-----------|-----|
| **fotoPerfilUrl** | `User.FotoPerfilUrl` | Avatar pessoal | Foto do usuário (rosto) |
| **logoUrl** | `PrestadorBranding.LogoUrl` | Logo da empresa | Logo do negócio (marca) |

**Recomendação:**
- **fotoPerfilUrl**: Foto do prestador (pessoa física)
- **logoUrl**: Logo do estabelecimento (pessoa jurídica)

---

## ?? FLUXO DE UPLOAD DE IMAGEM (RECOMENDADO)

### **Opção 1: Upload para Serviço Externo (ATUAL)**

```
Frontend ? Cloudinary/S3 ? Recebe URL ? Backend (fotoPerfilUrl)
```

**Vantagens:**
- ? CDN integrado
- ? Otimização de imagens
- ? Não sobrecarrega backend
- ? Escalável

### **Opção 2: Upload Direto para Backend (ALTERNATIVA)**

Se precisar, podemos criar:

```csharp
// ProfileController.cs
[HttpPost("avatar")]
public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file)
{
    // Validar arquivo
    // Salvar em /wwwroot/uploads/avatars/
    // Retornar URL completa
}
```

**Request:**
```http
POST /api/profile/avatar
Content-Type: multipart/form-data

file: [binary data]
```

**Response:**
```json
{
  "success": true,
  "data": {
    "url": "https://seudominio.com/uploads/avatars/uuid.jpg"
  }
}
```

---

## ? CONCLUSÃO

**TODOS OS CAMPOS ESTÃO IMPLEMENTADOS E FUNCIONANDO! ??**

### Status Geral: ? **100% ALINHADO**

- ? **Bio (descrição)** - Funciona
- ? **CPF/CNPJ** - Funciona
- ? **FotoPerfilUrl** - Funciona
- ? **Services** - Funciona

### O que falta (opcional):
- ?? Endpoint de upload direto de imagem (se necessário)
- ?? Validação de formato CPF/CNPJ (se necessário)
- ?? Compressão automática de imagens (se necessário)

**Backend está pronto para produção!** ??
