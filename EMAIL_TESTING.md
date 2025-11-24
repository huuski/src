# Como Testar o Envio de Email

## Opções de Teste

### 1. Usando o Endpoint de Teste (Recomendado)

Criei um controller `TestEmailController` com endpoints específicos para testes:

#### Teste Simples (sem anexo)
```bash
POST http://localhost:5026/api/TestEmail/send-test-email
Content-Type: application/json

{
  "to": "seu-email@gmail.com",
  "subject": "Teste de Email",
  "body": "<h1>Email de Teste</h1>"
}
```

#### Teste com PDF
```bash
POST http://localhost:5026/api/TestEmail/send-test-email-with-pdf
Content-Type: application/json

{
  "to": "seu-email@gmail.com"
}
```

### 2. Usando o Endpoint de Negociação

Teste o envio real de PDF de uma negociação:

```bash
POST http://localhost:5026/api/Negotiation/{id}/send-pdf-email
Content-Type: application/json

{
  "email": "seu-email@gmail.com"
}
```

### 3. Usando Ferramentas de Teste Local (Recomendado para Desenvolvimento)

#### Mailtrap (Gratuito)
- **Site**: https://mailtrap.io
- **Limite Gratuito**: 500 emails/mês
- **Vantagem**: Captura todos os emails sem enviar para destinatários reais
- **Configuração**: Use as credenciais SMTP do Mailtrap no `appsettings.json`

**Configuração Mailtrap:**
```json
{
  "Email": {
    "Smtp": {
      "Host": "smtp.mailtrap.io",
      "Port": "2525",
      "Username": "seu-username-mailtrap",
      "Password": "sua-password-mailtrap",
      "EnableSsl": "true"
    }
  }
}
```

#### MailHog (Local)
- **Site**: https://github.com/mailhog/MailHog
- **Gratuito**: Sim (open source)
- **Vantagem**: Roda localmente, não precisa de internet
- **Instalação**: 
  ```bash
  # macOS
  brew install mailhog
  
  # Ou via Docker
  docker run -d -p 1025:1025 -p 8025:8025 mailhog/mailhog
  ```

**Configuração MailHog:**
```json
{
  "Email": {
    "Smtp": {
      "Host": "localhost",
      "Port": "1025",
      "Username": "",
      "Password": "",
      "EnableSsl": "false"
    }
  }
}
```

Acesse a interface web em: http://localhost:8025

### 4. Usando Postman ou Insomnia

1. Importe o arquivo `Email-Test-Examples.http`
2. Configure a variável `@email` com seu email
3. Execute as requisições

### 5. Usando curl

```bash
# Teste simples
curl -X POST http://localhost:5026/api/TestEmail/send-test-email \
  -H "Content-Type: application/json" \
  -d '{
    "to": "seu-email@gmail.com",
    "subject": "Teste",
    "body": "<h1>Teste</h1>"
  }'

# Teste com PDF
curl -X POST http://localhost:5026/api/TestEmail/send-test-email-with-pdf \
  -H "Content-Type: application/json" \
  -d '{
    "to": "seu-email@gmail.com"
  }'
```

## Verificando se Funcionou

### Se usar Gmail/Outlook real:
- Verifique a caixa de entrada
- Verifique a pasta de spam
- Aguarde alguns segundos (pode haver delay)

### Se usar Mailtrap:
- Acesse: https://mailtrap.io/inboxes
- Veja os emails capturados na interface

### Se usar MailHog:
- Acesse: http://localhost:8025
- Veja os emails na interface web

## Troubleshooting

### Email não está sendo enviado
1. Verifique os logs da aplicação
2. Confirme que as credenciais estão corretas no `appsettings.json`
3. Para Gmail: use App Password, não a senha normal
4. Verifique se o firewall não está bloqueando a porta SMTP

### Erro de autenticação
- Gmail: Certifique-se de usar App Password
- Verifique se "Less secure app access" está desabilitado (não é mais necessário com App Password)

### Email vai para spam
- Configure SPF, DKIM e DMARC (para produção)
- Use um serviço profissional como SendGrid em produção

## Próximos Passos

Para produção, considere:
- SendGrid (100 emails/dia grátis)
- Brevo (300 emails/dia grátis)
- Mailgun (1.000 emails/mês grátis após trial)

