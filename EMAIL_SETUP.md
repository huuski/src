# Configuração de Email

## Opções Gratuitas para Envio de Email

### 1. Gmail SMTP (Recomendado para testes)
- **Limite**: 500 emails/dia
- **Gratuito**: Sim
- **Configuração**:
  1. Ative a verificação em duas etapas na sua conta Google
  2. Gere uma "App Password" em: https://myaccount.google.com/apppasswords
  3. Use essa senha no `appsettings.json`

**Configuração no appsettings.json:**
```json
{
  "Email": {
    "Smtp": {
      "Host": "smtp.gmail.com",
      "Port": "587",
      "Username": "seu-email@gmail.com",
      "Password": "sua-app-password-gerada",
      "EnableSsl": "true"
    },
    "From": {
      "Address": "seu-email@gmail.com",
      "Name": "Sistema"
    }
  }
}
```

### 2. Outlook/Hotmail SMTP
- **Limite**: 300 emails/dia
- **Gratuito**: Sim
- **Configuração**:
```json
{
  "Email": {
    "Smtp": {
      "Host": "smtp-mail.outlook.com",
      "Port": "587",
      "Username": "seu-email@outlook.com",
      "Password": "sua-senha",
      "EnableSsl": "true"
    }
  }
}
```

### 3. SendGrid (Recomendado para produção)
- **Plano Gratuito**: 100 emails/dia
- **API Key**: Necessária
- **Site**: https://sendgrid.com
- **Nota**: Requer implementação adicional usando a biblioteca SendGrid

### 4. Brevo (Sendinblue)
- **Plano Gratuito**: 300 emails/dia
- **API Key**: Necessária
- **Site**: https://brevo.com

### 5. Mailgun
- **Plano Gratuito**: 5.000 emails/mês (primeiros 3 meses), depois 1.000/mês
- **API Key**: Necessária
- **Site**: https://mailgun.com

## Como Configurar Gmail

1. Acesse: https://myaccount.google.com/security
2. Ative "Verificação em duas etapas"
3. Acesse: https://myaccount.google.com/apppasswords
4. Gere uma "App Password" para "Email"
5. Use essa senha no `appsettings.json` (não use sua senha normal)

## Segurança

⚠️ **IMPORTANTE**: Nunca commite o arquivo `appsettings.json` com credenciais reais!
- Use `appsettings.Development.json` para desenvolvimento local
- Use variáveis de ambiente ou Azure Key Vault em produção
- Adicione `appsettings.json` ao `.gitignore` se contiver credenciais

