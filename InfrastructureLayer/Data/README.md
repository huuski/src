# Seed Data

Este diretório contém arquivos JSON com dados iniciais para popular os repositórios em memória.

## Arquivo seed-data.json

O arquivo `seed-data.json` contém dados de exemplo para as seguintes entidades:

- **customers**: Clientes iniciais
- **services**: Serviços disponíveis
- **products**: Produtos disponíveis
- **paymentMethods**: Métodos de pagamento
- **notifications**: Notificações iniciais
- **reminders**: Lembretes iniciais
- **spotlightCards**: Cards de destaque

## Como funciona

Os repositórios em memória são inicializados automaticamente com os dados do arquivo JSON quando a aplicação inicia. Se o arquivo não for encontrado, os repositórios serão criados vazios (comportamento opcional).

## Formato dos dados

### Customer
```json
{
  "id": "guid",
  "name": "string",
  "document": "string",
  "birthDate": "ISO 8601 date",
  "email": "string",
  "street": "string",
  "city": "string",
  "state": "string",
  "zipCode": "string",
  "country": "string (opcional)",
  "complement": "string (opcional)",
  "phoneNumber": "string"
}
```

### Service
```json
{
  "name": "string",
  "description": "string",
  "category": 0-3 (enum: Medical=0, Aesthetical=1, Wellness=2, Other=3),
  "amount": "decimal",
  "duration": "HH:mm:ss",
  "image": "string (opcional)"
}
```

### Product
```json
{
  "name": "string",
  "description": "string",
  "category": 0-4 (enum: Cosmetic=0, Medication=1, Supplement=2, Equipment=3, Other=4),
  "amount": "decimal",
  "image": "string (opcional)"
}
```

### PaymentMethod
```json
{
  "name": "string",
  "type": 0-5 (enum: CreditCard=0, DebitCard=1, Cash=2, BankTransfer=3, Pix=4, Other=5),
  "inactive": "boolean"
}
```

### Notification
```json
{
  "title": "string",
  "description": "string",
  "icon": "string (opcional)",
  "readAt": "ISO 8601 date (opcional)"
}
```

### Reminder
```json
{
  "title": "string",
  "description": "string",
  "priority": 0-2 (enum: Low=0, Normal=1, High=2),
  "readAt": "ISO 8601 date (opcional)"
}
```

### SpotlightCard
```json
{
  "title": "string",
  "shortDescription": "string",
  "longDescription": "string",
  "image": "string (opcional)",
  "buttonTitle": "string (opcional)",
  "buttonLink": "string (opcional)",
  "initDate": "ISO 8601 date",
  "endDate": "ISO 8601 date",
  "inactive": "boolean"
}
```

