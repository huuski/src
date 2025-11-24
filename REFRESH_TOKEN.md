# Refresh Token Implementation

## Visão Geral

A funcionalidade de RefreshToken foi implementada para melhorar a segurança e a experiência do usuário. O sistema agora utiliza tokens de acesso de curta duração (AccessToken) e tokens de atualização de longa duração (RefreshToken) para autenticação.

## Como Funciona

### Fluxo de Autenticação

1. **Login**: O usuário faz login e recebe:
   - `AccessToken`: Token JWT de curta duração (padrão: 60 minutos)
   - `RefreshToken`: Token JWT de longa duração (padrão: 7 dias)
   - `ExpiresAt`: Data de expiração do AccessToken

2. **Uso do AccessToken**: O cliente usa o AccessToken para fazer requisições autenticadas à API.

3. **Renovação**: Quando o AccessToken expira, o cliente usa o RefreshToken para obter um novo par de tokens.

4. **Token Rotation**: A cada renovação, o RefreshToken antigo é revogado e um novo é gerado (segurança adicional).

### Endpoints Disponíveis

#### POST `/api/auth/login`
Autentica o usuário e retorna tokens.

**Request:**
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-01-15T10:00:00Z",
  "user": {
    "id": "guid",
    "name": "User Name",
    "email": "user@example.com",
    "avatar": null
  }
}
```

#### POST `/api/auth/refresh`
Renova os tokens usando um RefreshToken válido.

**Request:**
```json
{
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-01-15T11:00:00Z"
}
```

**Nota**: O RefreshToken antigo é automaticamente revogado após a renovação (token rotation).

#### POST `/api/auth/logout`
Revoga um RefreshToken específico (logout de um dispositivo).

**Request:**
```json
{
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Response:**
```json
{
  "message": "Token revoked successfully"
}
```

#### POST `/api/auth/logout-all`
Revoga todos os RefreshTokens do usuário autenticado (logout de todos os dispositivos).

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Response:**
```json
{
  "message": "All tokens revoked successfully"
}
```

## Configuração

As configurações de JWT estão no `appsettings.json`:

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

## Segurança

### Token Rotation
- A cada renovação, o RefreshToken antigo é revogado
- Isso previne reutilização de tokens comprometidos
- Um token revogado não pode ser usado novamente

### Validação de Tokens
- Os RefreshTokens são validados contra:
  - Assinatura JWT
  - Expiração
  - Status de revogação
  - Existência no repositório

### Armazenamento
- Os RefreshTokens são armazenados em memória (para PoC)
- Em produção, devem ser armazenados em banco de dados
- Tokens expirados podem ser limpos periodicamente

## Exemplo de Uso no Cliente

```javascript
// 1. Login
const loginResponse = await fetch('/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ email, password })
});
const { accessToken, refreshToken, expiresAt } = await loginResponse.json();

// Armazenar tokens (localStorage, sessionStorage, etc.)
localStorage.setItem('accessToken', accessToken);
localStorage.setItem('refreshToken', refreshToken);

// 2. Fazer requisições autenticadas
const apiCall = async () => {
  let token = localStorage.getItem('accessToken');
  
  let response = await fetch('/api/protected-endpoint', {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  
  // 3. Se token expirou, renovar
  if (response.status === 401) {
    const refreshResponse = await fetch('/api/auth/refresh', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ 
        refreshToken: localStorage.getItem('refreshToken') 
      })
    });
    
    if (refreshResponse.ok) {
      const { accessToken: newAccessToken, refreshToken: newRefreshToken } = 
        await refreshResponse.json();
      
      localStorage.setItem('accessToken', newAccessToken);
      localStorage.setItem('refreshToken', newRefreshToken);
      
      // Tentar novamente com o novo token
      response = await fetch('/api/protected-endpoint', {
        headers: { 'Authorization': `Bearer ${newAccessToken}` }
      });
    } else {
      // RefreshToken inválido, fazer login novamente
      // Redirecionar para tela de login
    }
  }
  
  return response;
};

// 4. Logout
const logout = async () => {
  const refreshToken = localStorage.getItem('refreshToken');
  await fetch('/api/auth/logout', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken })
  });
  
  localStorage.removeItem('accessToken');
  localStorage.removeItem('refreshToken');
};
```

## Estrutura de Dados

### RefreshToken Entity
```csharp
public class RefreshToken : Entity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }
}
```

## Melhorias Futuras

- [ ] Limpeza automática de tokens expirados
- [ ] Limite de tokens ativos por usuário
- [ ] Rastreamento de dispositivos/IPs
- [ ] Notificações de login suspeito
- [ ] Migração para banco de dados persistente

