# 🔐 Autenticação e Autorização

## Visão Geral

Todas as APIs estão protegidas por autenticação JWT, **exceto** os endpoints de autenticação e alguns endpoints de teste/demo.

## ✅ Endpoints Públicos (Não Requerem Autenticação)

### Autenticação
- `POST /api/auth/login` - Login de usuário
- `POST /api/auth/resetpassword` - Reset de senha
- `POST /api/auth/refresh` - Refresh token

### Teste/Demo
- `POST /api/app/resetdemo` - Reset de dados demo
- `POST /api/testemail/send-test-email` - Teste de envio de email
- `POST /api/testemail/send-test-email-with-pdf` - Teste de envio de email com PDF

## 🔒 Endpoints Protegidos (Requerem Autenticação)

**TODOS** os outros endpoints requerem um token JWT válido no header da requisição.

Exemplos:
- `GET /api/product` - Listar produtos
- `GET /api/supply` - Listar supplies
- `GET /api/appointment` - Listar atendimentos
- `POST /api/negotiation` - Criar negociação
- E todos os outros endpoints...

## 📝 Como Usar a Autenticação

### 1. Fazer Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "marco.uski@huuski.com",
  "password": "Senha123!"
}
```

**Resposta:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh-token-here...",
  "expiresAt": "2024-01-01T12:00:00Z",
  "user": {
    "id": "850e8400-e29b-41d4-a716-446655440001",
    "name": "Marco Uski",
    "email": "marco.uski@huuski.com"
  }
}
```

### 2. Usar o Token em Requisições

Adicione o token no header `Authorization`:

```http
GET /api/product
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 3. Exemplo Completo com cURL

```bash
# 1. Fazer login
TOKEN=$(curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"marco.uski@huuski.com","password":"Senha123!"}' \
  | jq -r '.accessToken')

# 2. Usar o token em uma requisição
curl -X GET http://localhost:5000/api/product \
  -H "Authorization: Bearer $TOKEN"
```

### 4. Exemplo com JavaScript/Fetch

```javascript
// 1. Login
const loginResponse = await fetch('http://localhost:5000/api/auth/login', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({
    email: 'marco.uski@huuski.com',
    password: 'Senha123!'
  })
});

const { accessToken } = await loginResponse.json();

// 2. Usar o token em requisições
const productsResponse = await fetch('http://localhost:5000/api/product', {
  headers: {
    'Authorization': `Bearer ${accessToken}`
  }
});

const products = await productsResponse.json();
```

### 5. Exemplo com Postman/Insomnia

1. Faça uma requisição POST para `/api/auth/login`
2. Copie o `accessToken` da resposta
3. Nas requisições seguintes, adicione no header:
   - **Key**: `Authorization`
   - **Value**: `Bearer {seu-token-aqui}`

## 🔄 Refresh Token

Quando o access token expirar, use o refresh token para obter um novo:

```http
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "seu-refresh-token-aqui"
}
```

## ⚠️ Respostas de Erro

### 401 Unauthorized
Token ausente, inválido ou expirado:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.2",
  "title": "Unauthorized",
  "status": 401
}
```

**Solução**: Faça login novamente ou use o refresh token.

### 403 Forbidden
Token válido mas sem permissão (não aplicável no momento, mas pode ser usado no futuro).

## 🔧 Configuração

A configuração JWT está em `appsettings.json`:

```json
{
  "Jwt": {
    "SecretKey": "your-secret-key-here-change-in-production-minimum-32-characters",
    "Issuer": "your-app",
    "Audience": "your-app-users",
    "AccessTokenExpirationMinutes": "60",
    "RefreshTokenExpirationDays": "7"
  }
}
```

**⚠️ IMPORTANTE**: Altere o `SecretKey` em produção para um valor seguro e aleatório!

## 📋 Usuários de Teste

### Usuário 1
- **Email**: `marco.uski@huuski.com`
- **Senha**: `Senha123!`

### Usuário 2
- **Email**: `fernandinho.palmeirense@huuski.com`
- **Senha**: `Senha456!`

## 🧪 Testando a Autenticação

### Teste 1: Requisição sem token (deve falhar)
```bash
curl http://localhost:5000/api/product
# Resposta: 401 Unauthorized
```

### Teste 2: Requisição com token válido (deve funcionar)
```bash
# Obter token
TOKEN=$(curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"marco.uski@huuski.com","password":"Senha123!"}' \
  | jq -r '.accessToken')

# Usar token
curl -H "Authorization: Bearer $TOKEN" http://localhost:5000/api/product
# Resposta: Lista de produtos
```

### Teste 3: Endpoint público (deve funcionar sem token)
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"marco.uski@huuski.com","password":"Senha123!"}'
# Resposta: Token e informações do usuário
```

