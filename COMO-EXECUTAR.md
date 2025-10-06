# ?? COMO EXECUTAR A APLICAÇÃO - ATUALIZADO

## ?? **EXECUÇÃO RÁPIDA E TESTE AUTOMÁTICO**

### **?? RECOMENDADO - Inicia e Testa Automaticamente:**

#### **No PowerShell:**
```powershell
# Script completo que inicia aplicação e testa todas as rotas
.\start-and-test.ps1
```

#### **No CMD:**
```cmd
start-and-test.bat
```

**Este script:**
- ? **Inicia a aplicação** automaticamente
- ? **Aguarda 15 segundos** para inicialização
- ? **Testa todos os endpoints** automaticamente
- ? **Abre URLs funcionais** no navegador
- ? **Mostra diagnóstico** se algo falhar

---

## ?? **OUTRAS OPÇÕES**

### **Scripts Individuais:**

#### **Para apenas iniciar:**
```powershell
# PowerShell
.\start-swagger.bat

# Ou direto
cd Plataforma_Agendamentos
dotnet run --urls="https://localhost:5001;http://localhost:5000"
```

#### **Para testar (com aplicação já rodando):**
```powershell
.\test-api-routes.bat
```

---

## ?? **RESULTADO ESPERADO**

Após executar `.\start-and-test.ps1`:

```
? HOME: https://localhost:5001/ - OK
? HEALTH: https://localhost:5001/api/health - OK  
? PING: https://localhost:5001/api/health/ping - OK
? SWAGGER JSON: https://localhost:5001/swagger/v1/swagger.json - OK
? SWAGGER UI: https://localhost:5001/swagger - OK

?? RESUMO DOS TESTES:
================================
? Home funcionando
? Health funcionando
? Ping funcionando
? Swagger JSON funcionando
? Swagger UI funcionando

?? Abrindo URLs funcionais no navegador...
? Aplicação funcionando! URLs abertas no navegador.
```

---

## ?? **URLs que devem abrir automaticamente:**

- **?? Home**: `https://localhost:5001/` (JSON da API)
- **?? Swagger**: `https://localhost:5001/swagger` (Interface Swagger)
- **?? Health**: `https://localhost:5001/api/health` (Status da aplicação)

---

## ?? **SE TODOS OS TESTES FALHAREM**

O script mostra diagnóstico automático:

```
? APLICAÇÃO NÃO ESTÁ RESPONDENDO!

?? Possíveis causas:
  1. Aplicação falhou ao iniciar
  2. Porta ocupada por outro processo  
  3. Certificado SSL inválido
  4. Firewall bloqueando conexões
```

### **Soluções:**
1. **Execute como Administrador**
2. **Verifique se PostgreSQL está rodando**
3. **Tente HTTP**: `http://localhost:5000/swagger`
4. **Execute**: `.\admin-fix.bat` para limpar processos

---

## ?? **SCRIPTS DISPONÍVEIS**

| Script | Descrição | Recomendação |
|--------|-----------|--------------|
| `start-and-test.ps1` | **?? Inicia + Testa + Abre URLs** | **PRINCIPAL** |
| `start-and-test.bat` | Mesma função (CMD) | **PRINCIPAL** |
| `start-swagger.bat` | Apenas inicia | Se só quer iniciar |
| `test-api-routes.bat` | Apenas testa | Se app já está rodando |
| `admin-fix.bat` | Resolver problemas | Se houver erros |

---

## ?? **DICAS IMPORTANTES**

### **Certificado SSL:**
- Se aparecer aviso de segurança, clique **"Avançado"** ? **"Continuar para localhost"**
- Ou use HTTP: `http://localhost:5000/swagger`

### **Para desenvolvimento:**
- Use `.\start-and-test.ps1` uma vez para testar
- Depois use `F5` no Visual Studio para desenvolvimento normal

### **Para parar:**
- Pressione qualquer tecla no script para manter rodando
- Ou `Ctrl+C` para parar

---

## ?? **EXECUÇÃO RECOMENDADA AGORA**

```powershell
# Execute este comando:
.\start-and-test.ps1
```

**Este script resolve tudo automaticamente:**
1. ? Inicia a aplicação
2. ? Testa se está funcionando  
3. ? Abre Swagger no navegador
4. ? Mostra diagnóstico se falhar

**É a forma mais rápida e confiável de ver a aplicação funcionando!** ??