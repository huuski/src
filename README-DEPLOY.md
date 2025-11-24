# 📦 Guia de Deploy - API .NET

## 🎯 Resumo Rápido

Esta API está pronta para deploy em plataformas gratuitas. Foram criados os seguintes arquivos:

- ✅ `Dockerfile` - Para deploy via Docker
- ✅ `.dockerignore` - Otimização do build
- ✅ `railway.json` - Configuração Railway
- ✅ `fly.toml` - Configuração Fly.io
- ✅ `Program.cs` - Atualizado para suportar porta dinâmica

## 🚀 Opções Gratuitas Disponíveis

### 1. Railway (Recomendado para PoC)
- **URL**: https://railway.app
- **Crédito**: $5/mês grátis
- **Setup**: 5 minutos
- **Deploy**: Automático via GitHub

### 2. Render
- **URL**: https://render.com
- **Tier**: Gratuito disponível
- **Setup**: 10 minutos
- **Limitação**: Dorme após 15min de inatividade

### 3. Fly.io
- **URL**: https://fly.io
- **Recursos**: 3 VMs gratuitas
- **Setup**: 10 minutos
- **Performance**: Melhor opção

### 4. Azure App Service
- **URL**: https://portal.azure.com
- **Tier**: F1 gratuito
- **Setup**: 15 minutos
- **Integração**: GitHub Actions

## 📋 Arquivos Criados

```
/
├── Dockerfile              # Configuração Docker
├── .dockerignore           # Arquivos ignorados no build
├── railway.json            # Config Railway
├── fly.toml                # Config Fly.io
├── DEPLOY.md               # Guia completo detalhado
└── DEPLOY-QUICK-START.md   # Guia rápido passo a passo
```

## ⚡ Deploy Rápido (Railway)

1. Acesse https://railway.app
2. Login com GitHub
3. "New Project" > "Deploy from GitHub repo"
4. Selecione seu repositório
5. Pronto! Railway faz tudo automaticamente

## 🔍 Teste Após Deploy

```bash
# Teste endpoint de supplies
curl https://your-api-url.com/api/supply

# Teste endpoint de products
curl https://your-api-url.com/api/product
```

## 📚 Documentação Completa

- **Guia Detalhado**: Veja `DEPLOY.md`
- **Guia Rápido**: Veja `DEPLOY-QUICK-START.md`

## ⚠️ Importante

- Os dados são armazenados em memória (serão perdidos ao reiniciar)
- HTTPS é configurado automaticamente
- Porta é configurada via variável de ambiente `PORT`

