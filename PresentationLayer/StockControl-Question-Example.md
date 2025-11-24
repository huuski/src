# Exemplo de Pergunta de Controle de Estoque

## Tipo de Pergunta: StockControl (6)

O tipo de pergunta "Controle de Estoque" permite registrar movimentações de produtos durante a execução de um serviço.

## Estrutura JSON

```json
{
  "id": "q-1-1",
  "order": 1,
  "title": "Registrar uso de produto",
  "subtitle": "Controle de estoque",
  "description": "Registre os produtos utilizados durante o procedimento",
  "type": 6,
  "required": true,
  "stockControl": {
    "operation": "output",
    "products": [
      {
        "productId": "c50e8400-e29b-41d4-a716-446655440001",
        "quantity": 1,
        "unit": "unidade"
      },
      {
        "productId": "c50e8400-e29b-41d4-a716-446655440002",
        "quantity": 2,
        "unit": "ml"
      }
    ]
  }
}
```

## Campos da Pergunta

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|-------------|-----------|
| `id` | string | ✅ Sim | Identificador único da pergunta |
| `order` | number | ✅ Sim | Ordem da pergunta na etapa |
| `title` | string | ✅ Sim | Título da pergunta |
| `subtitle` | string | ❌ Não | Subtítulo da pergunta |
| `description` | string | ❌ Não | Descrição detalhada |
| `type` | number | ✅ Sim | Tipo da pergunta (6 = StockControl) |
| `required` | boolean | ✅ Sim | Se a pergunta é obrigatória |
| `stockControl` | object | ✅ Sim | Configuração do controle de estoque |

## Campos do stockControl

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|-------------|-----------|
| `operation` | string | ✅ Sim | Tipo de operação: "input" (entrada) ou "output" (saída) |
| `products` | array | ✅ Sim | Lista de produtos a serem controlados |

## Campos do produto em stockControl.products

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|-------------|-----------|
| `productId` | string (Guid) | ✅ Sim | ID do produto |
| `quantity` | number | ✅ Sim | Quantidade utilizada/recebida |
| `unit` | string | ✅ Sim | Unidade de medida (ex: "unidade", "ml", "g", "kg") |

## Tipos de Operação

- `"input"`: Entrada de produtos no estoque (compra, recebimento)
- `"output"`: Saída de produtos do estoque (uso, consumo)

## Exemplo Completo em um Step

```json
{
  "steps": [
    {
      "id": "step-1",
      "title": "Aplicação de Produtos",
      "subtitle": "Produtos utilizados",
      "description": "Registre os produtos utilizados durante o procedimento",
      "stepNumber": 1,
      "questions": [
        {
          "id": "q-1-1",
          "order": 1,
          "title": "Registrar uso de produtos",
          "subtitle": "Controle de estoque",
          "description": "Registre os produtos utilizados durante o procedimento",
          "type": 6,
          "required": true,
          "stockControl": {
            "operation": "output",
            "products": [
              {
                "productId": "c50e8400-e29b-41d4-a716-446655440001",
                "quantity": 1,
                "unit": "unidade"
              },
              {
                "productId": "c50e8400-e29b-41d4-a716-446655440002",
                "quantity": 5,
                "unit": "ml"
              }
            ]
          }
        }
      ]
    }
  ]
}
```

## Exemplo de Resposta/Valor

Quando o usuário responder a esta pergunta, o valor retornado seria:

```json
{
  "questionId": "q-1-1",
  "answer": {
    "operation": "output",
    "products": [
      {
        "productId": "c50e8400-e29b-41d4-a716-446655440001",
        "productName": "Protetor solar FPS 80",
        "quantity": 1,
        "unit": "unidade",
        "timestamp": "2024-01-20T14:30:00Z"
      },
      {
        "productId": "c50e8400-e29b-41d4-a716-446655440002",
        "productName": "Vitamina C Sérum",
        "quantity": 5,
        "unit": "ml",
        "timestamp": "2024-01-20T14:30:00Z"
      }
    ]
  }
}
```

## Casos de Uso

1. **Durante um procedimento estético**: Registrar produtos utilizados (sérum, creme, etc.)
2. **Após uma compra**: Registrar entrada de produtos no estoque
3. **Controle de consumo**: Acompanhar uso de produtos por serviço
4. **Inventário**: Atualizar estoque em tempo real durante atendimentos

## Validações Recomendadas

- Verificar se o `productId` existe
- Validar se a quantidade é maior que zero
- Verificar se há estoque suficiente (para operações de saída)
- Validar a unidade de medida

