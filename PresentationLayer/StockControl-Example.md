# Tipo de Pergunta: Controle de Estoque (StockControl)

## Visão Geral

O tipo `StockControl` (valor `6` no enum `AnswerType`) permite criar perguntas que controlam o estoque de produtos durante a execução de um fluxo. Este tipo de pergunta permite selecionar produtos e informar a quantidade utilizada.

## Estrutura no JSON do Flow

### Exemplo de Pergunta com Controle de Estoque

```json
{
  "id": "q-stock-1",
  "order": 1,
  "title": "Controle de produtos utilizados",
  "subtitle": "Selecione os produtos e informe a quantidade",
  "description": "Registre os produtos utilizados durante o procedimento e suas quantidades",
  "type": 6,
  "options": [
    {
      "id": "opt-stock-1",
      "title": "Protetor solar FPS 80",
      "value": "c50e8400-e29b-41d4-a716-446655440001",
      "enableExtraAnswer": false
    },
    {
      "id": "opt-stock-2",
      "title": "Vitamina C Sérum",
      "value": "c50e8400-e29b-41d4-a716-446655440002",
      "enableExtraAnswer": false
    },
    {
      "id": "opt-stock-3",
      "title": "Hidratante com ceramidas",
      "value": "c50e8400-e29b-41d4-a716-446655440003",
      "enableExtraAnswer": false
    }
  ],
  "required": true,
  "stockControl": {
    "allowMultipleProducts": true,
    "requireQuantity": true,
    "quantityUnit": "unidade"
  }
}
```

## Campos Específicos do StockControl

### Campo `stockControl` (opcional)

O campo `stockControl` pode ser adicionado à pergunta para configurar comportamentos específicos:

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|-------------|-----------|
| `allowMultipleProducts` | boolean | Não | Permite selecionar múltiplos produtos (padrão: `true`) |
| `requireQuantity` | boolean | Não | Exige que seja informada a quantidade (padrão: `true`) |
| `quantityUnit` | string | Não | Unidade de medida da quantidade (ex: "unidade", "ml", "g") |

## Estrutura de Resposta Esperada

Quando uma pergunta do tipo `StockControl` é respondida, a resposta deve seguir esta estrutura:

```json
{
  "questionId": "q-stock-1",
  "answer": [
    {
      "productId": "c50e8400-e29b-41d4-a716-446655440001",
      "productName": "Protetor solar FPS 80",
      "quantity": 2,
      "unit": "unidade"
    },
    {
      "productId": "c50e8400-e29b-41d4-a716-446655440002",
      "productName": "Vitamina C Sérum",
      "quantity": 1,
      "unit": "ml"
    }
  ]
}
```

## Validações

- Se `requireQuantity` for `true`, cada produto selecionado deve ter uma quantidade informada
- A quantidade deve ser um número positivo
- Os `productId` devem corresponder a produtos válidos no sistema

## Exemplo Completo em um Step

```json
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
      "title": "Quais produtos foram utilizados?",
      "subtitle": "Selecione e informe a quantidade",
      "description": "Selecione os produtos utilizados e informe a quantidade de cada um",
      "type": 6,
      "options": [
        {
          "id": "opt-1-1",
          "title": "Protetor solar FPS 80",
          "value": "c50e8400-e29b-41d4-a716-446655440001",
          "enableExtraAnswer": false
        },
        {
          "id": "opt-1-2",
          "title": "Vitamina C Sérum",
          "value": "c50e8400-e29b-41d4-a716-446655440002",
          "enableExtraAnswer": false
        },
        {
          "id": "opt-1-3",
          "title": "Hidratante com ceramidas",
          "value": "c50e8400-e29b-41d4-a716-446655440003",
          "enableExtraAnswer": false
        }
      ],
      "required": true,
      "stockControl": {
        "allowMultipleProducts": true,
        "requireQuantity": true,
        "quantityUnit": "unidade"
      }
    }
  ]
}
```

## Integração com Produtos

Os produtos disponíveis para seleção devem ser obtidos da API `/api/product`. O campo `value` nas `options` deve conter o `Id` do produto (GUID).

## Notas de Implementação

- O tipo `StockControl` não requer validação especial de `options` obrigatórias (diferente de Radio e Multiselect)
- As `options` devem conter os produtos disponíveis para seleção
- O campo `value` em cada `option` deve ser o GUID do produto
- O campo `title` em cada `option` deve ser o nome do produto

