# GUIA DE REFATORAÇÃO COMPLETA - SEPARAÇÃO DE PERFIS

## ? CONCLUÍDO

1. ? **Models criados:**
   - `ClientePerfil.cs` - Perfil de cliente
   - `PrestadorPerfil.cs` - Perfil de prestador  
   - `PrestadorBranding.cs` - Branding visual
   - `PrestadorMetricas.cs` - Métricas e estatísticas

2. ? **User.cs refatorado:**
   - Removidos campos específicos
   - Mantido apenas identidade base
   - Navigation properties para perfis

3. ? **AppDbContext.cs atualizado:**
   - Configurações para novas entidades
   - Relacionamentos 1:1 definidos
   - Índices configurados

4. ? **AuthController.cs parcialmente atualizado:**
   - Criação automática de perfis no registro
   - Include de perfis no login

## ?? PRÓXIMOS PASSOS (MANUAL)

### 1. Atualizar Controllers Restantes

#### PrestadorController.cs
Substituir:
```csharp
// ANTES
.FirstOrDefaultAsync(u => u.Slug == slug && u.UserType == UserTypes.Prestador);

// DEPOIS
.Include(u => u.PrestadorPerfil)
    .ThenInclude(p => p.Branding)
.FirstOrDefaultAsync(u => u.PrestadorPerfil.Slug == slug && u.UserType == UserTypes.Prestador);

// E no retorno:
var perfil = prestador.PrestadorPerfil;
return Ok(new
{
    perfil.Slug,
    perfil.DisplayName,
    perfil.Bio,
    Branding = perfil.Branding,
    ...
});
```

#### ProfileController.cs
O arquivo completo já foi criado em `ProfileControllerNew.cs`. 
Apenas renomeie ou copie o conteúdo para `ProfileController.cs`.

### 2. Gerar e Aplicar Migration

```bash
cd Plataforma_Agendamentos

# Gerar migration
dotnet ef migrations add RefactorUserToSeparateProfiles

# Revisar migration gerada
# IMPORTANTE: Adicionar código de migração de dados!

# Aplicar migration
dotnet ef database update
```

### 3. Migration de Dados (CRÍTICO!)

A migration vai criar as novas tabelas, mas os dados existentes em `Users`
precisam ser migrados. Adicione isso na migration:

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // 1. Criar novas tabelas
    migrationBuilder.CreateTable(
        name: "ClientePerfis",
        ...
    );
    
    // 2. Migrar dados de clientes
    migrationBuilder.Sql(@"
        INSERT INTO ""ClientePerfis"" (""Id"", ""UserId"", ""Telefone"", ""CPF"", ""DataNascimento"", ""Endereco"", ""PreferenciasNotificacao"", ""TotalAgendamentos"", ""UltimoAgendamento"", ""CreatedAt"", ""UpdatedAt"")
        SELECT 
            gen_random_uuid(),
            ""Id"",
            ""TelefoneCliente"",
            ""CPF"",
            ""DataNascimento"",
            ""EnderecoCliente"",
            ""PreferenciasNotificacao"",
            ""TotalAgendamentosCliente"",
            ""UltimoAgendamento"",
            ""CreatedAt"",
            ""UpdatedAt""
        FROM ""Users""
        WHERE ""UserType"" = 'cliente'
    ");
    
    // 3. Migrar dados de prestadores
    migrationBuilder.Sql(@"
        INSERT INTO ""PrestadorPerfis"" (""Id"", ""UserId"", ""DisplayName"", ""Slug"", ""Bio"", ""CNPJ"", ""Telefone"", ""Endereco"", ""Site"", ""AceitaAgendamentoImediato"", ""HorasAntecedenciaMinima"", ""CreatedAt"", ""UpdatedAt"")
        SELECT 
            gen_random_uuid(),
            ""Id"",
            COALESCE(""DisplayName"", ""Name""),
            ""Slug"",
            ""Bio"",
            ""CNPJ"",
            ""TelefonePrestador"",
            ""EnderecoPrestador"",
            ""Site"",
            ""AceitaAgendamentoImediato"",
            ""HorasAntecedenciaMinima"",
            ""CreatedAt"",
            ""UpdatedAt""
        FROM ""Users""
        WHERE ""UserType"" = 'prestador'
    ");
    
    // 4. Criar branding para prestadores
    migrationBuilder.Sql(@"
        INSERT INTO ""PrestadorBrandings"" (""Id"", ""PrestadorPerfilId"", ""LogoUrl"", ""CoverImageUrl"", ""PrimaryColor"")
        SELECT 
            gen_random_uuid(),
            pp.""Id"",
            u.""LogoUrl"",
            u.""CoverImageUrl"",
            u.""PrimaryColor""
        FROM ""PrestadorPerfis"" pp
        JOIN ""Users"" u ON pp.""UserId"" = u.""Id""
    ");
    
    // 5. Criar métricas para prestadores
    migrationBuilder.Sql(@"
        INSERT INTO ""PrestadorMetricas"" (""Id"", ""PrestadorPerfilId"", ""AvaliacaoMedia"", ""TotalAvaliacoes"", ""TotalServicos"", ""TotalAgendamentos"", ""UltimaAtualizacao"")
        SELECT 
            gen_random_uuid(),
            pp.""Id"",
            u.""AvaliacaoMedia"",
            u.""TotalAvaliacoes"",
            u.""TotalServicos"",
            u.""TotalAgendamentosPrestador"",
            NOW()
        FROM ""PrestadorPerfis"" pp
        JOIN ""Users"" u ON pp.""UserId"" = u.""Id""
    ");
    
    // 6. Remover colunas antigas da tabela Users
    migrationBuilder.DropColumn(name: "TelefoneCliente", table: "Users");
    migrationBuilder.DropColumn(name: "CPF", table: "Users");
    // ... etc para todas as colunas migradas
}
```

### 4. Atualizar Testes

Todos os testes que usam `User` diretamente precisam ser atualizados:

```csharp
// ANTES
var user = new User
{
    Slug = "teste",
    DisplayName = "Teste",
    Bio = "Bio teste"
};

// DEPOIS
var user = new User
{
    UserType = UserTypes.Prestador,
    PrestadorPerfil = new PrestadorPerfil
    {
        Slug = "teste",
        DisplayName = "Teste",
        Bio = "Bio teste"
    }
};
```

### 5. Atualizar DTOs

Alguns DTOs podem precisar de ajustes para refletir a nova estrutura.

## ?? BENEFÍCIOS DA REFATORAÇÃO

### ? Código Mais Limpo
- Separação clara de responsabilidades
- Cada entidade com propósito único
- Zero campos NULL desnecessários

### ? Manutenção Mais Fácil
- Mudanças isoladas por perfil
- Regras de negócio claras
- Fácil adicionar novos perfis

### ? Performance Melhor
- Queries mais eficientes
- Carrega só o necessário
- Índices otimizados

### ? Evolução Simples
- Adicionar Admin? Criar `AdminPerfil`
- Adicionar Moderador? Criar `ModeradorPerfil`
- Sem quebrar código existente

## ?? ATENÇÃO

1. **BACKUP DO BANCO** antes de aplicar migration
2. **TESTE EM DESENVOLVIMENTO** primeiro
3. **REVISE A MIGRATION** antes de aplicar
4. **MIGRAÇÃO DE DADOS** é crítica - não pule

## ?? SUPORTE

Se encontrar problemas durante a migração:
1. Reverta: `dotnet ef database update <migration_anterior>`
2. Corrija o problema
3. Gere nova migration
4. Aplique novamente

---

**Esta refatoração é grande mas vale a pena!** ??
O código ficará muito mais profissional e escalável.
