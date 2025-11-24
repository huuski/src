using ApplicationLayer.Interfaces.Repositories;
using DomainLayer.Entities;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repositories;

public class InMemoryExecutionFlowRepository : IExecutionFlowRepository
{
    private readonly Dictionary<Guid, ExecutionFlow> _executionFlows = new();
    private readonly object _lock = new();

    public InMemoryExecutionFlowRepository(SeedDataService? seedDataService = null)
    {
        bool executionFlowsLoaded = false;
        
        if (seedDataService != null)
        {
            var executionFlows = seedDataService.GetExecutionFlows();
            foreach (var executionFlow in executionFlows)
            {
                try
                {
                    _executionFlows[executionFlow.Id] = executionFlow;
                    executionFlowsLoaded = true;
                }
                catch
                {
                    continue;
                }
            }
        }
        
        // Fallback: Initialize default execution flows if SeedDataService is not available or no execution flows were loaded
        if (!executionFlowsLoaded)
        {
            InitializeDefaultExecutionFlows();
        }
    }

    private void InitializeSeedData(SeedDataService seedDataService)
    {
        var executionFlows = seedDataService.GetExecutionFlows();
        foreach (var executionFlow in executionFlows)
        {
            try
            {
                _executionFlows[executionFlow.Id] = executionFlow;
            }
            catch
            {
                continue;
            }
        }
    }

    private void InitializeDefaultExecutionFlows()
    {
        var idProperty = typeof(Entity).GetProperty("Id");

        // Fluxo 1: Limpeza de Pele
        var flow1Json = @"{
            ""steps"": [
                {
                    ""id"": ""step-1"",
                    ""title"": ""Avaliação Inicial da Pele"",
                    ""subtitle"": ""Avaliação da pele"",
                    ""description"": ""Avaliação da condição atual da pele, tipo de pele, sensibilidade e histórico de tratamentos"",
                    ""stepNumber"": 1,
                    ""questions"": [
                        {
                            ""id"": ""q-1-1"",
                            ""order"": 1,
                            ""title"": ""Qual o tipo de pele do cliente?"",
                            ""subtitle"": ""Selecione uma opção"",
                            ""description"": ""Informe o tipo de pele"",
                            ""type"": 3,
                            ""options"": [
                                {
                                    ""id"": ""opt-1-1"",
                                    ""title"": ""Oleosa"",
                                    ""value"": ""oleosa"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-1-2"",
                                    ""title"": ""Seca"",
                                    ""value"": ""seca"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-1-3"",
                                    ""title"": ""Mista"",
                                    ""value"": ""mista"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-1-4"",
                                    ""title"": ""Normal"",
                                    ""value"": ""normal"",
                                    ""enableExtraAnswer"": false
                                }
                            ],
                            ""required"": true
                        }
                    ]
                },
                {
                    ""id"": ""step-2"",
                    ""title"": ""Preparação e Higienização"",
                    ""subtitle"": ""Preparação"",
                    ""description"": ""Preparação da pele para o procedimento, incluindo higienização e aplicação de produtos preparatórios"",
                    ""stepNumber"": 2,
                    ""questions"": [
                        {
                            ""id"": ""q-2-1"",
                            ""order"": 1,
                            ""title"": ""A pele foi higienizada corretamente?"",
                            ""subtitle"": ""Confirme a higienização"",
                            ""description"": ""Confirme que a pele foi higienizada antes do procedimento"",
                            ""type"": 3,
                            ""options"": [
                                {
                                    ""id"": ""opt-2-1"",
                                    ""title"": ""Sim"",
                                    ""value"": ""sim"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-2-2"",
                                    ""title"": ""Não"",
                                    ""value"": ""nao"",
                                    ""enableExtraAnswer"": false
                                }
                            ],
                            ""required"": true
                        }
                    ]
                },
                {
                    ""id"": ""step-3"",
                    ""title"": ""Procedimento de Limpeza"",
                    ""subtitle"": ""Limpeza"",
                    ""description"": ""Realização da limpeza profunda, extração de cravos e espinhas, e aplicação de máscaras"",
                    ""stepNumber"": 3,
                    ""questions"": [
                        {
                            ""id"": ""q-3-1"",
                            ""order"": 1,
                            ""title"": ""Quantas áreas foram tratadas?"",
                            ""subtitle"": ""Informe o número"",
                            ""description"": ""Informe quantas áreas do rosto foram tratadas"",
                            ""type"": 2,
                            ""options"": [],
                            ""maxLength"": 10,
                            ""required"": true
                        }
                    ]
                },
                {
                    ""id"": ""step-4"",
                    ""title"": ""Finalização e Cuidados Pós-Tratamento"",
                    ""subtitle"": ""Finalização"",
                    ""description"": ""Aplicação de produtos finais, proteção solar e orientações sobre cuidados pós-tratamento"",
                    ""stepNumber"": 4,
                    ""questions"": [
                        {
                            ""id"": ""q-4-1"",
                            ""order"": 1,
                            ""title"": ""Protetor solar foi aplicado?"",
                            ""subtitle"": ""Confirme a aplicação"",
                            ""description"": ""Confirme que o protetor solar foi aplicado ao final do procedimento"",
                            ""type"": 3,
                            ""options"": [
                                {
                                    ""id"": ""opt-4-1"",
                                    ""title"": ""Sim"",
                                    ""value"": ""sim"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-4-2"",
                                    ""title"": ""Não"",
                                    ""value"": ""nao"",
                                    ""enableExtraAnswer"": false
                                }
                            ],
                            ""required"": true
                        },
                        {
                            ""id"": ""q-4-2"",
                            ""order"": 2,
                            ""title"": ""Controle de insumos utilizados"",
                            ""subtitle"": ""Selecione os insumos e informe a quantidade"",
                            ""description"": ""Registre os insumos utilizados durante o procedimento e suas quantidades"",
                            ""type"": 6,
                            ""options"": [
                                {
                                    ""id"": ""opt-stock-1"",
                                    ""title"": ""Luvas"",
                                    ""value"": ""d50e8400-e29b-41d4-a716-446655440001"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-stock-2"",
                                    ""title"": ""Algodão"",
                                    ""value"": ""d50e8400-e29b-41d4-a716-446655440002"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-stock-3"",
                                    ""title"": ""Gaze"",
                                    ""value"": ""d50e8400-e29b-41d4-a716-446655440003"",
                                    ""enableExtraAnswer"": false
                                }
                            ],
                            ""required"": false,
                            ""stockControl"": {
                                ""allowMultipleProducts"": true,
                                ""requireQuantity"": true,
                                ""quantityUnit"": ""unidade""
                            }
                        }
                    ]
                }
            ]
        }";

        var ef1 = new ExecutionFlow(
            "Fluxo de Execução - Limpeza de Pele",
            flow1Json
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(ef1, new Guid("b50e8400-e29b-41d4-a716-446655440001"));
        }
        _executionFlows[ef1.Id] = ef1;

        // Fluxo 2: Toxina Botulínica (Botox)
        var flow2Json = @"{
            ""steps"": [
                {
                    ""id"": ""step-1"",
                    ""title"": ""Avaliação e Anamnese"",
                    ""subtitle"": ""Avaliação inicial"",
                    ""description"": ""Avaliação da área a ser tratada, histórico de tratamentos anteriores e contraindicações"",
                    ""stepNumber"": 1,
                    ""questions"": [
                        {
                            ""id"": ""q-1-1"",
                            ""order"": 1,
                            ""title"": ""O cliente já realizou aplicação de toxina botulínica anteriormente?"",
                            ""subtitle"": ""Histórico de tratamentos"",
                            ""description"": ""Informe se o cliente já realizou este procedimento antes"",
                            ""type"": 3,
                            ""options"": [
                                {
                                    ""id"": ""opt-1-1"",
                                    ""title"": ""Sim"",
                                    ""value"": ""sim"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-1-2"",
                                    ""title"": ""Não"",
                                    ""value"": ""nao"",
                                    ""enableExtraAnswer"": false
                                }
                            ],
                            ""required"": true
                        }
                    ]
                },
                {
                    ""id"": ""step-2"",
                    ""title"": ""Marcação dos Pontos de Aplicação"",
                    ""subtitle"": ""Marcação"",
                    ""description"": ""Marcação dos pontos exatos onde a toxina será aplicada"",
                    ""stepNumber"": 2,
                    ""questions"": [
                        {
                            ""id"": ""q-2-1"",
                            ""order"": 1,
                            ""title"": ""Quantos pontos foram marcados para aplicação?"",
                            ""subtitle"": ""Número de pontos"",
                            ""description"": ""Informe o número total de pontos marcados"",
                            ""type"": 2,
                            ""options"": [],
                            ""maxLength"": 10,
                            ""required"": true
                        }
                    ]
                },
                {
                    ""id"": ""step-3"",
                    ""title"": ""Aplicação da Toxina Botulínica"",
                    ""subtitle"": ""Aplicação"",
                    ""description"": ""Aplicação da toxina botulínica nos pontos marcados"",
                    ""stepNumber"": 3,
                    ""questions"": [
                        {
                            ""id"": ""q-3-1"",
                            ""order"": 1,
                            ""title"": ""Quantas unidades foram aplicadas?"",
                            ""subtitle"": ""Unidades aplicadas"",
                            ""description"": ""Informe o total de unidades de toxina botulínica aplicadas"",
                            ""type"": 2,
                            ""options"": [],
                            ""maxLength"": 10,
                            ""required"": true
                        }
                    ]
                },
                {
                    ""id"": ""step-4"",
                    ""title"": ""Orientações Pós-Procedimento"",
                    ""subtitle"": ""Orientações"",
                    ""description"": ""Orientações sobre cuidados pós-procedimento e retorno"",
                    ""stepNumber"": 4,
                    ""questions"": [
                        {
                            ""id"": ""q-4-1"",
                            ""order"": 1,
                            ""title"": ""O cliente foi orientado sobre os cuidados pós-procedimento?"",
                            ""subtitle"": ""Confirmação de orientações"",
                            ""description"": ""Confirme que todas as orientações foram repassadas"",
                            ""type"": 3,
                            ""options"": [
                                {
                                    ""id"": ""opt-4-1"",
                                    ""title"": ""Sim"",
                                    ""value"": ""sim"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-4-2"",
                                    ""title"": ""Não"",
                                    ""value"": ""nao"",
                                    ""enableExtraAnswer"": false
                                }
                            ],
                            ""required"": true
                        }
                    ]
                }
            ]
        }";

        var ef2 = new ExecutionFlow(
            "Fluxo de Execução - Toxina Botulínica (Botox)",
            flow2Json
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(ef2, new Guid("b50e8400-e29b-41d4-a716-446655440002"));
        }
        _executionFlows[ef2.Id] = ef2;

        // Fluxo 3: Preenchimento Facial
        var flow3Json = @"{
            ""steps"": [
                {
                    ""id"": ""step-1"",
                    ""title"": ""Avaliação e Planejamento"",
                    ""subtitle"": ""Avaliação inicial"",
                    ""description"": ""Avaliação da área a ser preenchida, análise de assimetrias e planejamento do procedimento"",
                    ""stepNumber"": 1,
                    ""questions"": [
                        {
                            ""id"": ""q-1-1"",
                            ""order"": 1,
                            ""title"": ""Qual área será preenchida?"",
                            ""subtitle"": ""Selecione a área"",
                            ""description"": ""Informe a área do rosto que será preenchida"",
                            ""type"": 4,
                            ""options"": [
                                {
                                    ""id"": ""opt-1-1"",
                                    ""title"": ""Lábios"",
                                    ""value"": ""labios"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-1-2"",
                                    ""title"": ""Bochechas"",
                                    ""value"": ""bochechas"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-1-3"",
                                    ""title"": ""Bigode chinês"",
                                    ""value"": ""bigode_chines"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-1-4"",
                                    ""title"": ""Mandíbula"",
                                    ""value"": ""mandibula"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-1-5"",
                                    ""title"": ""Outra área"",
                                    ""value"": ""outra"",
                                    ""enableExtraAnswer"": true,
                                    ""extraAnswerMaxLength"": 200
                                }
                            ],
                            ""required"": true
                        }
                    ]
                },
                {
                    ""id"": ""step-2"",
                    ""title"": ""Preparação e Antissepsia"",
                    ""subtitle"": ""Preparação"",
                    ""description"": ""Preparação da área, antissepsia e marcação dos pontos de aplicação"",
                    ""stepNumber"": 2,
                    ""questions"": [
                        {
                            ""id"": ""q-2-1"",
                            ""order"": 1,
                            ""title"": ""A área foi antissepsada corretamente?"",
                            ""subtitle"": ""Confirme a antissepsia"",
                            ""description"": ""Confirme que a antissepsia foi realizada antes do procedimento"",
                            ""type"": 3,
                            ""options"": [
                                {
                                    ""id"": ""opt-2-1"",
                                    ""title"": ""Sim"",
                                    ""value"": ""sim"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-2-2"",
                                    ""title"": ""Não"",
                                    ""value"": ""nao"",
                                    ""enableExtraAnswer"": false
                                }
                            ],
                            ""required"": true
                        }
                    ]
                },
                {
                    ""id"": ""step-3"",
                    ""title"": ""Aplicação do Preenchimento"",
                    ""subtitle"": ""Aplicação"",
                    ""description"": ""Aplicação do ácido hialurônico na área planejada"",
                    ""stepNumber"": 3,
                    ""questions"": [
                        {
                            ""id"": ""q-3-1"",
                            ""order"": 1,
                            ""title"": ""Quantos ml de ácido hialurônico foram aplicados?"",
                            ""subtitle"": ""Volume aplicado"",
                            ""description"": ""Informe o volume total aplicado em ml"",
                            ""type"": 2,
                            ""options"": [],
                            ""maxLength"": 10,
                            ""required"": true
                        }
                    ]
                },
                {
                    ""id"": ""step-4"",
                    ""title"": ""Moldagem e Finalização"",
                    ""subtitle"": ""Finalização"",
                    ""description"": ""Moldagem do produto aplicado e verificação do resultado"",
                    ""stepNumber"": 4,
                    ""questions"": [
                        {
                            ""id"": ""q-4-1"",
                            ""order"": 1,
                            ""title"": ""O resultado está simétrico e natural?"",
                            ""subtitle"": ""Avaliação do resultado"",
                            ""description"": ""Avalie se o resultado está conforme o esperado"",
                            ""type"": 3,
                            ""options"": [
                                {
                                    ""id"": ""opt-4-1"",
                                    ""title"": ""Sim"",
                                    ""value"": ""sim"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-4-2"",
                                    ""title"": ""Não"",
                                    ""value"": ""nao"",
                                    ""enableExtraAnswer"": true,
                                    ""extraAnswerMaxLength"": 300
                                }
                            ],
                            ""required"": true
                        }
                    ]
                }
            ]
        }";

        var ef3 = new ExecutionFlow(
            "Fluxo de Execução - Preenchimento Facial",
            flow3Json
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(ef3, new Guid("b50e8400-e29b-41d4-a716-446655440003"));
        }
        _executionFlows[ef3.Id] = ef3;

        // Fluxo 4: Peeling Químico
        var flow4Json = @"{
            ""steps"": [
                {
                    ""id"": ""step-1"",
                    ""title"": ""Avaliação da Pele e Indicação"",
                    ""subtitle"": ""Avaliação inicial"",
                    ""description"": ""Avaliação do tipo de pele, condições atuais e indicação do tipo de peeling adequado"",
                    ""stepNumber"": 1,
                    ""questions"": [
                        {
                            ""id"": ""q-1-1"",
                            ""order"": 1,
                            ""title"": ""Qual o tipo de peeling será aplicado?"",
                            ""subtitle"": ""Selecione o tipo"",
                            ""description"": ""Informe o tipo de peeling químico que será aplicado"",
                            ""type"": 3,
                            ""options"": [
                                {
                                    ""id"": ""opt-1-1"",
                                    ""title"": ""Superficial"",
                                    ""value"": ""superficial"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-1-2"",
                                    ""title"": ""Médio"",
                                    ""value"": ""medio"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-1-3"",
                                    ""title"": ""Profundo"",
                                    ""value"": ""profundo"",
                                    ""enableExtraAnswer"": false
                                }
                            ],
                            ""required"": true
                        }
                    ]
                },
                {
                    ""id"": ""step-2"",
                    ""title"": ""Preparação da Pele"",
                    ""subtitle"": ""Preparação"",
                    ""description"": ""Limpeza profunda e preparação da pele para receber o peeling"",
                    ""stepNumber"": 2,
                    ""questions"": [
                        {
                            ""id"": ""q-2-1"",
                            ""order"": 1,
                            ""title"": ""A pele foi preparada adequadamente?"",
                            ""subtitle"": ""Confirme a preparação"",
                            ""description"": ""Confirme que a pele foi limpa e preparada antes da aplicação"",
                            ""type"": 3,
                            ""options"": [
                                {
                                    ""id"": ""opt-2-1"",
                                    ""title"": ""Sim"",
                                    ""value"": ""sim"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-2-2"",
                                    ""title"": ""Não"",
                                    ""value"": ""nao"",
                                    ""enableExtraAnswer"": false
                                }
                            ],
                            ""required"": true
                        }
                    ]
                },
                {
                    ""id"": ""step-3"",
                    ""title"": ""Aplicação do Peeling"",
                    ""subtitle"": ""Aplicação"",
                    ""description"": ""Aplicação do produto de peeling químico na pele"",
                    ""stepNumber"": 3,
                    ""questions"": [
                        {
                            ""id"": ""q-3-1"",
                            ""order"": 1,
                            ""title"": ""Qual foi o tempo de contato do produto?"",
                            ""subtitle"": ""Tempo em minutos"",
                            ""description"": ""Informe o tempo de contato do produto com a pele em minutos"",
                            ""type"": 2,
                            ""options"": [],
                            ""maxLength"": 10,
                            ""required"": true
                        }
                    ]
                },
                {
                    ""id"": ""step-4"",
                    ""title"": ""Neutralização e Cuidados Pós-Tratamento"",
                    ""subtitle"": ""Finalização"",
                    ""description"": ""Neutralização do produto, aplicação de calmantes e orientações pós-tratamento"",
                    ""stepNumber"": 4,
                    ""questions"": [
                        {
                            ""id"": ""q-4-1"",
                            ""order"": 1,
                            ""title"": ""O produto foi neutralizado corretamente?"",
                            ""subtitle"": ""Confirme a neutralização"",
                            ""description"": ""Confirme que o processo de neutralização foi realizado"",
                            ""type"": 3,
                            ""options"": [
                                {
                                    ""id"": ""opt-4-1"",
                                    ""title"": ""Sim"",
                                    ""value"": ""sim"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-4-2"",
                                    ""title"": ""Não"",
                                    ""value"": ""nao"",
                                    ""enableExtraAnswer"": false
                                }
                            ],
                            ""required"": true
                        }
                    ]
                }
            ]
        }";

        var ef4 = new ExecutionFlow(
            "Fluxo de Execução - Peeling Químico",
            flow4Json
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(ef4, new Guid("b50e8400-e29b-41d4-a716-446655440004"));
        }
        _executionFlows[ef4.Id] = ef4;

        // Fluxo 5: Depilação a Laser
        var flow5Json = @"{
            ""steps"": [
                {
                    ""id"": ""step-1"",
                    ""title"": ""Avaliação e Consulta Inicial"",
                    ""subtitle"": ""Avaliação inicial"",
                    ""description"": ""Avaliação da área a ser tratada, tipo de pelo, cor da pele e histórico de tratamentos"",
                    ""stepNumber"": 1,
                    ""questions"": [
                        {
                            ""id"": ""q-1-1"",
                            ""order"": 1,
                            ""title"": ""Qual área será tratada?"",
                            ""subtitle"": ""Selecione a área"",
                            ""description"": ""Informe a área do corpo que será tratada"",
                            ""type"": 4,
                            ""options"": [
                                {
                                    ""id"": ""opt-1-1"",
                                    ""title"": ""Rosto"",
                                    ""value"": ""rosto"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-1-2"",
                                    ""title"": ""Axilas"",
                                    ""value"": ""axilas"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-1-3"",
                                    ""title"": ""Pernas"",
                                    ""value"": ""pernas"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-1-4"",
                                    ""title"": ""Virilha"",
                                    ""value"": ""virilha"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-1-5"",
                                    ""title"": ""Outra área"",
                                    ""value"": ""outra"",
                                    ""enableExtraAnswer"": true,
                                    ""extraAnswerMaxLength"": 200
                                }
                            ],
                            ""required"": true
                        }
                    ]
                },
                {
                    ""id"": ""step-2"",
                    ""title"": ""Preparação e Ajuste do Equipamento"",
                    ""subtitle"": ""Preparação"",
                    ""description"": ""Preparação da área, raspagem dos pelos e ajuste dos parâmetros do laser"",
                    ""stepNumber"": 2,
                    ""questions"": [
                        {
                            ""id"": ""q-2-1"",
                            ""order"": 1,
                            ""title"": ""Os pelos foram raspados adequadamente?"",
                            ""subtitle"": ""Confirme a raspagem"",
                            ""description"": ""Confirme que os pelos foram raspados antes do procedimento"",
                            ""type"": 3,
                            ""options"": [
                                {
                                    ""id"": ""opt-2-1"",
                                    ""title"": ""Sim"",
                                    ""value"": ""sim"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-2-2"",
                                    ""title"": ""Não"",
                                    ""value"": ""nao"",
                                    ""enableExtraAnswer"": false
                                }
                            ],
                            ""required"": true
                        }
                    ]
                },
                {
                    ""id"": ""step-3"",
                    ""title"": ""Aplicação do Laser"",
                    ""subtitle"": ""Aplicação"",
                    ""description"": ""Aplicação do laser na área tratada"",
                    ""stepNumber"": 3,
                    ""questions"": [
                        {
                            ""id"": ""q-3-1"",
                            ""order"": 1,
                            ""title"": ""Qual foi a energia utilizada (J/cm²)?"",
                            ""subtitle"": ""Energia do laser"",
                            ""description"": ""Informe a energia utilizada durante o procedimento"",
                            ""type"": 2,
                            ""options"": [],
                            ""maxLength"": 10,
                            ""required"": true
                        }
                    ]
                },
                {
                    ""id"": ""step-4"",
                    ""title"": ""Finalização e Orientações"",
                    ""subtitle"": ""Finalização"",
                    ""description"": ""Aplicação de produtos calmantes e orientações sobre cuidados pós-tratamento"",
                    ""stepNumber"": 4,
                    ""questions"": [
                        {
                            ""id"": ""q-4-1"",
                            ""order"": 1,
                            ""title"": ""O cliente foi orientado sobre cuidados pós-tratamento?"",
                            ""subtitle"": ""Confirmação de orientações"",
                            ""description"": ""Confirme que todas as orientações foram repassadas"",
                            ""type"": 3,
                            ""options"": [
                                {
                                    ""id"": ""opt-4-1"",
                                    ""title"": ""Sim"",
                                    ""value"": ""sim"",
                                    ""enableExtraAnswer"": false
                                },
                                {
                                    ""id"": ""opt-4-2"",
                                    ""title"": ""Não"",
                                    ""value"": ""nao"",
                                    ""enableExtraAnswer"": false
                                }
                            ],
                            ""required"": true
                        }
                    ]
                }
            ]
        }";

        var ef5 = new ExecutionFlow(
            "Fluxo de Execução - Depilação a Laser",
            flow5Json
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(ef5, new Guid("b50e8400-e29b-41d4-a716-446655440005"));
        }
        _executionFlows[ef5.Id] = ef5;
    }

    public Task<ExecutionFlow?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _executionFlows.TryGetValue(id, out var executionFlow);
            return Task.FromResult<ExecutionFlow?>(executionFlow?.IsDeleted == false ? executionFlow : null);
        }
    }

    public Task<IEnumerable<ExecutionFlow>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<ExecutionFlow>>(
                _executionFlows.Values.Where(ef => !ef.IsDeleted).ToList()
            );
        }
    }

    public Task<ExecutionFlow> CreateAsync(ExecutionFlow executionFlow, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_executionFlows.ContainsKey(executionFlow.Id))
                throw new InvalidOperationException($"ExecutionFlow with id {executionFlow.Id} already exists");

            _executionFlows[executionFlow.Id] = executionFlow;
            return Task.FromResult(executionFlow);
        }
    }

    public Task<ExecutionFlow> UpdateAsync(ExecutionFlow executionFlow, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_executionFlows.ContainsKey(executionFlow.Id))
                throw new InvalidOperationException($"ExecutionFlow with id {executionFlow.Id} not found");

            _executionFlows[executionFlow.Id] = executionFlow;
            return Task.FromResult(executionFlow);
        }
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_executionFlows.TryGetValue(id, out var executionFlow))
                return Task.FromResult(false);

            executionFlow.MarkAsDeleted();
            return Task.FromResult(true);
        }
    }

    public void Reset(SeedDataService? seedDataService = null)
    {
        lock (_lock)
        {
            _executionFlows.Clear();
            
            bool executionFlowsLoaded = false;
            if (seedDataService != null)
            {
                var executionFlows = seedDataService.GetExecutionFlows();
                foreach (var executionFlow in executionFlows)
                {
                    try
                    {
                        _executionFlows[executionFlow.Id] = executionFlow;
                        executionFlowsLoaded = true;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            
            if (!executionFlowsLoaded)
            {
                InitializeDefaultExecutionFlows();
            }
        }
    }
}

