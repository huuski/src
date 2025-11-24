# Exemplo de Atualização de Service (PUT)

## Endpoint
```
PUT /api/service/{id}
```

## Estrutura do Request Body

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|-------------|-----------|
| `name` | string | ✅ Sim | Nome do serviço |
| `description` | string | ✅ Sim | Descrição do serviço |
| `category` | int (enum) | ✅ Sim | Categoria do serviço (0=Medical, 1=Aesthetical, 2=Wellness, 3=Other) |
| `amount` | decimal | ✅ Sim | Valor do serviço |
| `image` | string? | ❌ Não | URL da imagem do serviço |
| `duration` | string | ✅ Sim | Duração no formato "HH:mm:ss" ou ISO 8601 duration |
| `executionFlowId` | Guid? | ❌ Não | ID do fluxo de execução vinculado |

## ServiceCategory Enum

- `0` = Medical
- `1` = Aesthetical
- `2` = Wellness
- `3` = Other

## Formato de Duration

O campo `duration` deve estar no formato **TimeSpan**:
- Formato .NET: `"HH:mm:ss"` (exemplo: `"00:30:00"` para 30 minutos)
- Formato ISO 8601: `"PT30M"` para 30 minutos, `"PT1H"` para 1 hora, `"PT1H30M"` para 1h30min

## Exemplos

### Exemplo 1: Atualizar Consulta Médica Completo

**Request:**
```http
PUT http://localhost:5026/api/service/a50e8400-e29b-41d4-a716-446655440001
Content-Type: application/json

{
  "name": "Consulta Médica Geral - Atualizado",
  "description": "Consulta médica geral com especialista, incluindo avaliação completa e orientações personalizadas",
  "category": 0,
  "amount": 280.00,
  "image": "https://example.com/images/consulta-medica-atualizada.jpg",
  "duration": "00:30:00",
  "executionFlowId": "b50e8400-e29b-41d4-a716-446655440001"
}
```

**Response (200 OK):**
```json
{
  "id": "a50e8400-e29b-41d4-a716-446655440001",
  "name": "Consulta Médica Geral - Atualizado",
  "description": "Consulta médica geral com especialista, incluindo avaliação completa e orientações personalizadas",
  "category": 0,
  "amount": 280.00,
  "image": "https://example.com/images/consulta-medica-atualizada.jpg",
  "duration": "00:30:00",
  "executionFlowId": "b50e8400-e29b-41d4-a716-446655440001",
  "createdAt": "2024-01-15T10:00:00Z",
  "updatedAt": "2024-01-20T14:30:00Z"
}
```

### Exemplo 2: Atualizar Limpeza de Pele

**Request:**
```http
PUT http://localhost:5026/api/service/a50e8400-e29b-41d4-a716-446655440002
Content-Type: application/json

{
  "name": "Limpeza de Pele Profunda",
  "description": "Limpeza de pele profunda com extração de cravos e espinhas, máscara hidratante e protetor solar",
  "category": 1,
  "amount": 200.00,
  "image": "https://example.com/images/limpeza-pele-profunda.jpg",
  "duration": "01:30:00",
  "executionFlowId": "b50e8400-e29b-41d4-a716-446655440002"
}
```

### Exemplo 3: Atualização sem ExecutionFlowId

**Request:**
```http
PUT http://localhost:5026/api/service/a50e8400-e29b-41d4-a716-446655440003
Content-Type: application/json

{
  "name": "Massagem Relaxante Premium",
  "description": "Massagem relaxante premium de 90 minutos com óleos essenciais",
  "category": 2,
  "amount": 180.00,
  "image": "https://example.com/images/massagem-relaxante.jpg",
  "duration": "01:30:00",
  "executionFlowId": null
}
```

### Exemplo 4: Atualização Mínima (sem campos opcionais)

**Request:**
```http
PUT http://localhost:5026/api/service/a50e8400-e29b-41d4-a716-446655440001
Content-Type: application/json

{
  "name": "Consulta Médica",
  "description": "Consulta médica geral",
  "category": 0,
  "amount": 250.00,
  "duration": "00:30:00"
}
```

## Exemplo usando cURL

```bash
curl -X PUT "http://localhost:5026/api/service/a50e8400-e29b-41d4-a716-446655440001" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Consulta Médica Geral - Atualizado",
    "description": "Consulta médica geral com especialista, incluindo avaliação completa e orientações personalizadas",
    "category": 0,
    "amount": 280.00,
    "image": "https://example.com/images/consulta-medica-atualizada.jpg",
    "duration": "00:30:00",
    "executionFlowId": "b50e8400-e29b-41d4-a716-446655440001"
  }'
```

## Exemplo usando JavaScript/Fetch

```javascript
const response = await fetch('http://localhost:5026/api/service/a50e8400-e29b-41d4-a716-446655440001', {
  method: 'PUT',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({
    name: "Consulta Médica Geral - Atualizado",
    description: "Consulta médica geral com especialista, incluindo avaliação completa e orientações personalizadas",
    category: 0, // Medical
    amount: 280.00,
    image: "https://example.com/images/consulta-medica-atualizada.jpg",
    duration: "00:30:00", // 30 minutos
    executionFlowId: "b50e8400-e29b-41d4-a716-446655440001"
  })
});

const result = await response.json();
console.log(result);
```

## Códigos de Resposta

- `200 OK`: Serviço atualizado com sucesso
- `400 Bad Request`: Dados inválidos ou faltando campos obrigatórios
- `404 Not Found`: Serviço não encontrado
- `500 Internal Server Error`: Erro interno do servidor

## Validações

- `name`: Não pode ser vazio ou apenas espaços em branco
- `description`: Não pode ser vazio ou apenas espaços em branco
- `amount`: Deve ser maior ou igual a zero
- `duration`: Deve ser maior que zero (formato válido de TimeSpan)
- `category`: Deve ser um valor válido do enum (0-3)

## IDs de Serviços de Exemplo

- Consulta Médica: `a50e8400-e29b-41d4-a716-446655440001`
- Limpeza de Pele: `a50e8400-e29b-41d4-a716-446655440002`
- Massagem Relaxante: `a50e8400-e29b-41d4-a716-446655440003`

## IDs de ExecutionFlow de Exemplo

- Consulta Médica Flow: `b50e8400-e29b-41d4-a716-446655440001`
- Limpeza de Pele Flow: `b50e8400-e29b-41d4-a716-446655440002`

