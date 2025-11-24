# 🚀 Deploy Rápido - Guia Passo a Passo

## Opção 1: Railway (Mais Rápido) ⭐

### Passo a Passo:

1. **Criar conta no Railway**
   - Acesse: https://railway.app
   - Faça login com GitHub

2. **Criar novo projeto**
   - Clique em "New Project"
   - Selecione "Deploy from GitHub repo"
   - Escolha seu repositório

3. **Configurar deploy**
   - Railway detecta automaticamente o projeto .NET
   - Clique em "Add Service" > "GitHub Repo"
   - Selecione o repositório
   - Railway fará o build e deploy automaticamente

4. **Configurar variáveis (se necessário)**
   - Vá em "Variables"
   - Adicione `PORT` se necessário (Railway geralmente configura automaticamente)

5. **Acessar sua API**
   - Após o deploy, Railway fornecerá uma URL pública
   - Exemplo: `https://your-api.up.railway.app`

**Tempo estimado**: 5-10 minutos

---

## Opção 2: Render (Muito Simples)

### Passo a Passo:

1. **Criar conta no Render**
   - Acesse: https://render.com
   - Faça login com GitHub

2. **Criar novo Web Service**
   - Clique em "New +" > "Web Service"
   - Conecte seu repositório GitHub

3. **Configurar o serviço**
   - **Name**: Seu nome de API
   - **Environment**: `Docker` ou `Native`
   - **Region**: Escolha mais próxima (ex: São Paulo)
   - **Branch**: `main` ou `master`
   - **Root Directory**: Deixe vazio (raiz do projeto)
   - **Build Command**: `dotnet publish -c Release -o ./publish`
   - **Start Command**: `dotnet ./publish/PresentationLayer.dll`

4. **Configurar variáveis de ambiente**
   - `ASPNETCORE_ENVIRONMENT`: `Production`
   - `PORT`: Render configura automaticamente

5. **Deploy**
   - Clique em "Create Web Service"
   - Render fará o build e deploy

**Tempo estimado**: 10-15 minutos

---

## Opção 3: Fly.io (Melhor Performance)

### Passo a Passo:

1. **Instalar Fly CLI**
   ```bash
   curl -L https://fly.io/install.sh | sh
   ```

2. **Fazer login**
   ```bash
   fly auth login
   ```

3. **Criar app**
   ```bash
   cd /Users/huuski/projects/src
   fly launch
   ```
   - Escolha um nome para o app
   - Escolha a região (ex: `gru` para São Paulo)
   - Confirme as configurações

4. **Deploy**
   ```bash
   fly deploy
   ```

5. **Acessar sua API**
   ```bash
   fly open
   ```

**Tempo estimado**: 10-15 minutos

---

## ✅ Verificação Pós-Deploy

Após o deploy, teste sua API:

```bash
# Teste de health (se tiver endpoint)
curl https://your-api-url.com/api/supply

# Ou teste qualquer endpoint
curl https://your-api-url.com/api/product
```

---

## 🔧 Troubleshooting

### Erro: "Port already in use"
- Verifique se a variável `PORT` está configurada
- A aplicação deve usar `0.0.0.0` como host

### Erro: "Build failed"
- Verifique os logs da plataforma
- Certifique-se de que o Dockerfile está correto
- Verifique se todas as dependências estão no `.csproj`

### API não responde
- Verifique os logs da plataforma
- Certifique-se de que a porta está configurada corretamente
- Verifique se o CORS está configurado (se necessário)

---

## 📝 Próximos Passos

1. **Configurar CORS** (se precisar acessar de um frontend):
   ```csharp
   builder.Services.AddCors(options =>
   {
       options.AddDefaultPolicy(policy =>
       {
           policy.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader();
       });
   });
   ```

2. **Adicionar endpoint de health check**:
   ```csharp
   app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
   ```

3. **Configurar domínio customizado** (opcional):
   - Na plataforma escolhida, configure um domínio personalizado

---

## 💡 Dicas

- **Railway**: Melhor para começar rápido
- **Render**: Bom para PoC simples
- **Fly.io**: Melhor para produção e performance
- **Azure**: Se você já usa ecossistema Microsoft

---

## 🆘 Precisa de Ajuda?

- Railway Docs: https://docs.railway.app
- Render Docs: https://render.com/docs
- Fly.io Docs: https://fly.io/docs

