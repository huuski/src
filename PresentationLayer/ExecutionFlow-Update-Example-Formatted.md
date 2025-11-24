# Exemplo de Atualização de ExecutionFlow

## Endpoint
```
PUT /api/executionflow/{id}
```

## Exemplo de Request Body

### JSON Formatado (para melhor visualização)

```json
{
  "title": "Questionário completo de exemplo - Atualizado",
  "flow": {
    "steps": [
      {
        "id": "step-1",
        "title": "Etapa 1 - Informações básicas",
        "subtitle": "Dados pessoais e de contato",
        "description": "Coleta de informações básicas do cliente",
        "stepNumber": 1,
        "questions": [
          {
            "id": "q-1-1",
            "order": 1,
            "title": "Nome completo",
            "subtitle": "Digite seu nome completo",
            "description": "Informe seu nome completo como consta no documento",
            "type": 1,
            "options": [],
            "maxLength": 255,
            "required": true
          },
          {
            "id": "q-1-2",
            "order": 2,
            "title": "Email",
            "subtitle": "Seu endereço de email",
            "description": "Email para contato e comunicação",
            "type": 1,
            "options": [],
            "maxLength": 100,
            "required": true
          }
        ]
      },
      {
        "id": "step-2",
        "title": "Etapa 2 - Seleção única",
        "subtitle": "Escolha uma opção",
        "description": "Perguntas com seleção única (radio buttons)",
        "stepNumber": 2,
        "questions": [
          {
            "id": "q-2-1",
            "order": 1,
            "title": "Qual é o seu gênero?",
            "subtitle": "Selecione uma opção",
            "description": "Informe seu gênero",
            "type": 3,
            "options": [
              {
                "id": "opt-2-1-1",
                "title": "Masculino",
                "value": "masculino",
                "enableExtraAnswer": false
              },
              {
                "id": "opt-2-1-2",
                "title": "Feminino",
                "value": "feminino",
                "enableExtraAnswer": false
              },
              {
                "id": "opt-2-1-3",
                "title": "Outro",
                "value": "outro",
                "enableExtraAnswer": true,
                "extraAnswerMaxLength": 100
              }
            ],
            "required": true
          }
        ]
      }
    ]
  }
}
```

### ⚠️ IMPORTANTE: O campo `flow` deve ser uma STRING JSON

No request HTTP, o campo `flow` deve ser uma **string JSON serializada**, não um objeto:

```json
{
  "title": "Questionário completo de exemplo",
  "description": "Descrição do fluxo",
  "flow": "{\"steps\":[{\"id\":\"step-1\",\"title\":\"Etapa 1\",\"stepNumber\":1,\"questions\":[]}]}"
}
```

## Exemplo usando cURL

```bash
curl -X PUT "http://localhost:5026/api/executionflow/b50e8400-e29b-41d4-a716-446655440001" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Fluxo de Execução - Consulta Médica",
    "description": "Fluxo completo para consulta médica",
    "flow": "{\"steps\":[{\"id\":\"step-1\",\"title\":\"Anamnese Inicial\",\"subtitle\":\"Dados do paciente\",\"description\":\"Coleta de informações básicas\",\"stepNumber\":1,\"questions\":[{\"id\":\"q-1-1\",\"order\":1,\"title\":\"Qual é o motivo principal da consulta?\",\"subtitle\":\"Descreva o motivo\",\"description\":\"Informe o motivo principal\",\"type\":1,\"options\":[],\"maxLength\":500,\"required\":true}]}]}"
  }'
```

## Exemplo usando JavaScript/Fetch

```javascript
const executionFlowId = 'b50e8400-e29b-41d4-a716-446655440001';
const flowJson = {
  steps: [
    {
      id: "step-1",
      title: "Etapa 1 - Informações básicas",
      subtitle: "Dados pessoais e de contato",
      description: "Coleta de informações básicas do cliente",
      stepNumber: 1,
      questions: [
        {
          id: "q-1-1",
          order: 1,
          title: "Nome completo",
          subtitle: "Digite seu nome completo",
          description: "Informe seu nome completo como consta no documento",
          type: 1,
          options: [],
          maxLength: 255,
          required: true
        }
      ]
    }
  ]
};

const response = await fetch(`http://localhost:5026/api/executionflow/${executionFlowId}`, {
  method: 'PUT',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({
    title: "Questionário completo de exemplo",
    flow: JSON.stringify(flowJson) // ⚠️ IMPORTANTE: Serializar o objeto para string JSON
  })
});

const result = await response.json();
console.log(result);
```

## Tipos de Perguntas (AnswerType)

- `1` = Text
- `2` = Number (não usado no exemplo, mas disponível)
- `3` = Radio
- `4` = Multiselect
- `5` = Photo

## Response

### Sucesso (200 OK)
```json
{
  "id": "b50e8400-e29b-41d4-a716-446655440001",
  "title": "Questionário completo de exemplo",
  "flow": "{\"steps\":[...]}",
  "createdAt": "2024-01-15T10:00:00Z",
  "updatedAt": "2024-01-20T14:30:00Z"
}
```

### Erro - JSON Inválido (400 Bad Request)
```json
{
  "message": "Invalid JSON format in Flow: ..."
}
```

### Erro - Não Encontrado (404 Not Found)
```json
{
  "message": "ExecutionFlow with id {id} not found"
}
```

