# REFATORAÇÃO - REMOVER LÓGICA DE NEGÓCIO DAS CONTROLLERS

## ?? PROBLEMAS IDENTIFICADOS

### Controllers com Lógica de Negócio:
1. ? **PrestadorController** - Cálculo de horários disponíveis
2. ? **AuthController** - Criação de perfis e validações
3. ? **ProfileController** - Validações de slug, criação de entidades
4. ? **ServicesController** - CRUD sem service layer

## ? SOLUÇÃO - CRIAR SERVICES

### 1. IAuthService / AuthService

**Responsabilidades:**
- Registro de usuário
- Login e autenticação
- Criação automática de perfis
- Validação de email duplicado
- Hash de senha

```csharp
public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<bool> EmailExistsAsync(string email);
}

public class AuthResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public UserDto? User { get; set; }
    public string? ErrorMessage { get; set; }
}
```

**O que move para o service:**
- Validação de tipo de usuário
- Criação de User + Perfil + Branding + Métricas
- Geração de slug
- Hash de senha (BCrypt)
- Geração de JWT

---

### 2. IProfileService / ProfileService

**Responsabilidades:**
- Obter perfil (cliente ou prestador)
- Atualizar perfil cliente
- Atualizar perfil prestador
- Validação de slug único
- Geração de slug automático
- Criação lazy de perfis

```csharp
public interface IProfileService
{
    Task<ProfileDto?> GetProfileAsync(Guid userId);
    Task<ProfileDto> UpdateClienteProfileAsync(Guid userId, UpdateClienteProfileRequest request);
    Task<ProfileDto> UpdatePrestadorProfileAsync(Guid userId, UpdatePrestadorProfileRequest request);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeUserId = null);
}
```

**O que move para o service:**
- Verificação de slug duplicado
- Criação lazy de ClientePerfil/PrestadorPerfil
- Geração de slug
- Validação de tipo de usuário
- Criação de Branding se necessário

---

### 3. IServiceManagementService / ServiceManagementService

**Responsabilidades:**
- CRUD de serviços
- Validação de ownership
- Validação de dados

```csharp
public interface IServiceManagementService
{
    Task<IEnumerable<ServiceDto>> GetServicesByProviderAsync(Guid providerId);
    Task<ServiceDto?> GetServiceByIdAsync(Guid serviceId, Guid providerId);
    Task<ServiceDto> CreateServiceAsync(Guid providerId, ServiceRequest request);
    Task<ServiceDto> UpdateServiceAsync(Guid serviceId, Guid providerId, ServiceRequest request);
    Task<bool> DeleteServiceAsync(Guid serviceId, Guid providerId);
}
```

**O que move para o service:**
- Validação de ownership
- Criação/atualização de serviços
- Validações de negócio

---

### 4. IPrestadorService / PrestadorService

**Responsabilidades:**
- Busca de prestador por slug
- Cálculo de horários disponíveis
- Listagem de serviços do prestador

```csharp
public interface IPrestadorService
{
    Task<PrestadorPublicDto?> GetBySlugAsync(string slug);
    Task<IEnumerable<string>> GetAvailableTimeSlotsAsync(string slug, DateTime date);
}
```

**O que move para o service:**
- Algoritmo de geração de slots de horário
- Verificação de agendamentos conflitantes
- Busca e montagem de dados públicos

---

## ?? PRIORIDADE DE REFATORAÇÃO

### Alta Prioridade (Fazer Primeiro):
1. ? **BookingService** - JÁ FEITO
2. ?? **AuthService** - Lógica crítica de registro/login
3. ?? **ProfileService** - Validações importantes

### Média Prioridade:
4. ?? **ServiceManagementService** - CRUD simples
5. ?? **PrestadorService** - Lógica de horários

### Baixa Prioridade:
6. ?? Outros controllers (se houver)

---

## ?? EXEMPLO DE REFATORAÇÃO

### ANTES ? - AuthController

```csharp
[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterRequest request)
{
    // 80+ linhas de lógica de negócio aqui
    var normalizedEmail = request.Email.Trim().ToLowerInvariant();
    
    if (await _context.Users.AnyAsync(u => u.Email == normalizedEmail))
        return BadRequest(...);
    
    var user = new User { ... };
    _context.Users.Add(user);
    
    if (userType == UserTypes.Cliente)
    {
        var clientePerfil = new ClientePerfil { ... };
        _context.ClientePerfis.Add(clientePerfil);
    }
    else if (userType == UserTypes.Prestador)
    {
        // Muita lógica...
    }
    
    await _context.SaveChangesAsync();
    var token = _jwtService.GenerateJwtToken(...);
    return Ok(...);
}
```

### DEPOIS ? - AuthController

```csharp
[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterRequest request)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    try
    {
        var result = await _authService.RegisterAsync(request);
        
        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Ok(new
        {
            success = true,
            message = "Usuario registrado com sucesso",
            data = new
            {
                token = result.Token,
                user = result.User
            }
        });
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(ex.Message);
    }
}
```

**Redução:** De 80+ linhas para ~20 linhas!

---

## ?? PASSOS PARA IMPLEMENTAR

### 1. Criar Interfaces e DTOs
```
Services/
??? IAuthService.cs
??? IProfileService.cs
??? IServiceManagementService.cs
??? IPrestadorService.cs

DTOs/
??? AuthDTOs.cs
??? ProfileDTOs.cs
??? ServiceDTOs.cs (já existe)
??? PrestadorDTOs.cs
```

### 2. Implementar Services
```
Services/
??? AuthService.cs
??? ProfileService.cs
??? ServiceManagementService.cs
??? PrestadorService.cs
```

### 3. Registrar no DI (Program.cs)
```csharp
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IServiceManagementService, ServiceManagementService>();
builder.Services.AddScoped<IPrestadorService, PrestadorService>();
```

### 4. Refatorar Controllers
- Injetar services via construtor
- Remover acesso direto ao DbContext
- Delegar lógica para services
- Manter apenas coordenação HTTP

### 5. Atualizar Testes
- Criar testes para services
- Atualizar testes de controllers
- Usar mocks/fakes para services

---

## ?? BENEFÍCIOS

### Antes (Atual):
- Controllers: 200+ linhas cada
- Lógica espalhada
- Difícil testar
- Acoplamento alto
- Violação de SRP

### Depois (Com Services):
- Controllers: 50-80 linhas cada
- Lógica centralizada
- Fácil testar
- Baixo acoplamento
- Segue SOLID

---

## ?? IMPORTANTE

1. **Não fazer tudo de uma vez** - Refatorar um controller por vez
2. **Testar após cada mudança** - Garantir que funciona
3. **Commit incremental** - Um service por commit
4. **Manter compatibilidade** - API externa não muda

---

## ?? OBJETIVO FINAL

```
Controllers (HTTP Layer)
    ?? Apenas roteamento
    ?? Validação de ModelState
    ?? Autenticação/Autorização
    ?? Retorno de responses

Services (Business Layer)
    ?? Regras de negócio
    ?? Validações complexas
    ?? Orquestração
    ?? Lógica de domínio

Repositories/DbContext (Data Layer)
    ?? Acesso a dados
```

**Controllers finos, services robustos!** ?
