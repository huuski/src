# Guia de Deploy Gratuito - API .NET

Este guia apresenta as melhores opções gratuitas para publicar esta API para PoC e testes.

## 🚀 Opções Recomendadas

### 1. **Railway** ⭐ (Mais Fácil)
- ✅ **Gratuito**: $5 de crédito mensal (suficiente para PoC)
- ✅ **Deploy automático** via GitHub
- ✅ **Suporte nativo** para .NET
- ✅ **HTTPS automático**
- ✅ **Logs em tempo real**

**Como fazer:**
1. Acesse [railway.app](https://railway.app)
2. Conecte seu repositório GitHub
3. Railway detecta automaticamente o projeto .NET
4. Deploy automático a cada push

**Configuração necessária:**
- Criar `railway.json` (opcional, para configurações customizadas)
- Porta será configurada automaticamente via variável `PORT`

---

### 2. **Render** ⭐ (Muito Simples)
- ✅ **Gratuito**: Tier gratuito disponível
- ✅ **Deploy automático** via GitHub
- ✅ **HTTPS automático**
- ⚠️ **Limitação**: Aplicação "dorme" após 15min de inatividade

**Como fazer:**
1. Acesse [render.com](https://render.com)
2. Conecte seu repositório GitHub
3. Selecione "Web Service"
4. Configure:
   - **Build Command**: `dotnet publish -c Release -o ./publish`
   - **Start Command**: `dotnet ./publish/PresentationLayer.dll`
   - **Environment**: `Docker` ou `Native`

---

### 3. **Fly.io** ⭐ (Boa Performance)
- ✅ **Gratuito**: 3 VMs compartilhadas gratuitas
- ✅ **Deploy rápido** via CLI
- ✅ **HTTPS automático**
- ✅ **Escalável**

**Como fazer:**
1. Instale o Fly CLI: `curl -L https://fly.io/install.sh | sh`
2. Faça login: `fly auth login`
3. Crie o app: `fly launch` (na pasta do projeto)
4. Deploy: `fly deploy`

**Configuração necessária:**
- Criar `fly.toml` (será gerado automaticamente)

---

### 4. **Azure App Service** (Microsoft)
- ✅ **Gratuito**: Tier F1 gratuito (com limitações)
- ✅ **Integração** com GitHub Actions
- ✅ **HTTPS automático**
- ⚠️ **Limitação**: CPU compartilhada, pode ser lento

**Como fazer:**
1. Acesse [portal.azure.com](https://portal.azure.com)
2. Crie um "App Service"
3. Escolha o tier "Free (F1)"
4. Configure deploy via GitHub

---

### 5. **GitHub Codespaces** (Para Desenvolvimento)
- ✅ **Gratuito**: 60 horas/mês para contas pessoais
- ✅ **Ambiente completo** de desenvolvimento
- ✅ **Ideal para testes** e desenvolvimento

**Como fazer:**
1. No GitHub, clique em "Code" > "Codespaces"
2. Crie um novo Codespace
3. Execute: `dotnet run --project PresentationLayer`

---

## 📋 Preparação do Projeto

### 1. Configurar Porta Dinâmica

Atualize `Program.cs` para usar a porta da variável de ambiente:

```csharp
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://0.0.0.0:{port}");
```

### 2. Criar Dockerfile (Opcional mas Recomendado)

Veja o arquivo `Dockerfile` na raiz do projeto.

### 3. Configurar Variáveis de Ambiente

Crie um arquivo `.env.example` com as variáveis necessárias (se houver).

---

## 🎯 Recomendação para PoC

**Para PoC rápido**: Use **Railway** ou **Render**
- Setup mais simples
- Deploy automático
- HTTPS incluído
- Ideal para demonstrações

**Para testes contínuos**: Use **Fly.io**
- Melhor performance
- Mais recursos gratuitos
- Escalável

---

## 📝 Checklist de Deploy

- [ ] Criar conta na plataforma escolhida
- [ ] Conectar repositório GitHub
- [ ] Configurar variáveis de ambiente (se necessário)
- [ ] Ajustar porta para variável de ambiente
- [ ] Fazer deploy
- [ ] Testar endpoints da API
- [ ] Compartilhar URL da API

---

## 🔗 URLs Úteis

- **Railway**: https://railway.app
- **Render**: https://render.com
- **Fly.io**: https://fly.io
- **Azure**: https://portal.azure.com

---

## ⚠️ Observações Importantes

1. **Dados em Memória**: Esta API usa repositórios em memória, então os dados serão perdidos ao reiniciar o serviço.

2. **HTTPS**: Todas as plataformas acima fornecem HTTPS automático.

3. **Rate Limiting**: Algumas plataformas gratuitas têm limites de requisições. Verifique os termos de cada serviço.

4. **Logs**: Use os logs da plataforma para debug durante o deploy.

5. **CORS**: Se precisar acessar de um frontend, configure CORS no `Program.cs`.

