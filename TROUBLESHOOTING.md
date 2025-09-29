# ?? Plataforma de Agendamentos - Guia de Execução

## ? Problemas Identificados

Os erros que você está vendo são comuns e relacionados a:

1. **IIS Express conflitos** - Portas ocupadas ou configurações conflitantes
2. **JavaScript Debugger** - Problemas com source maps do navegador
3. **Códigos de saída 0xffffffff** - Encerramento anormal de processos

## ? Soluções Implementadas

### 1. **Program.cs Melhorado**
- ? URLs explícitas configuradas
- ? Logs detalhados no console
- ? Tratamento robusto de erros
- ? Inicialização mais clara

### 2. **launchSettings.json Simplificado**
- ? Removido IIS Express (problemático)
- ? Perfis limpos e diretos
- ? Sem configurações desnecessárias

### 3. **Script de Inicialização**
- ? `start-app.bat` para execução direta
- ? Bypass completo do Visual Studio

## ?? Como Executar Agora

### **Opção 1: Via Script (Recomendado)**
```bash
# Execute o arquivo start-app.bat
# Duplo clique ou pelo terminal:
start-app.bat
```

### **Opção 2: Terminal/CMD**
```bash
cd Plataforma_Agendamentos
dotnet run
```

### **Opção 3: Visual Studio**
- Selecione o perfil "Plataforma_Agendamentos"
- Pressione F5 ou Ctrl+F5

## ?? URLs da Aplicação

- **Swagger**: https://localhost:5001/swagger
- **API**: https://localhost:5001/api
- **HTTP**: http://localhost:5000

## ?? Verificação de Problemas

### **Se ainda houver problemas:**

1. **Verifique portas ocupadas:**
```bash
netstat -ano | findstr :5000
netstat -ano | findstr :5001
```

2. **Mate processos se necessário:**
```bash
taskkill /PID [NUMERO_DO_PID] /F
```

3. **Execute como Administrador**

4. **Desabilite antivírus temporariamente**

## ?? Logs Esperados

Quando funcionar, você verá:
```
?? Iniciando Plataforma de Agendamentos...
?? Configurando serviços...
??? Configurando banco de dados: Host=localhost...
?? Configurando autenticação JWT...
??? Construindo aplicação...
?? Modo DESENVOLVIMENTO ativo
?? Inicializando banco de dados...
? Banco de dados criado com sucesso!

?? ===============================================
?? PLATAFORMA DE AGENDAMENTOS INICIADA!
?? ===============================================

?? URLs disponíveis:
   ?? HTTPS: https://localhost:5001
   ?? HTTP:  http://localhost:5000
   ?? Swagger: https://localhost:5001/swagger
```

## ?? Ainda com Problemas?

1. **Reinicie o computador**
2. **Atualize o .NET SDK**
3. **Execute o `start-app.bat`**
4. **Use HTTP em vez de HTTPS**: http://localhost:5000/swagger