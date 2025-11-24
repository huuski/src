using ApplicationLayer.Interfaces.Repositories;
using DomainLayer.Entities;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repositories;

public class InMemoryExecutionFlowStepRepository : IExecutionFlowStepRepository
{
    private readonly Dictionary<Guid, ExecutionFlowStep> _executionFlowSteps = new();
    private readonly Dictionary<Guid, List<ExecutionFlowStep>> _executionFlowStepsByExecutionFlowId = new();
    private readonly object _lock = new();

    public InMemoryExecutionFlowStepRepository(SeedDataService? seedDataService = null)
    {
        bool executionFlowStepsLoaded = false;
        
        if (seedDataService != null)
        {
            var executionFlowSteps = seedDataService.GetExecutionFlowSteps();
            foreach (var executionFlowStep in executionFlowSteps)
            {
                try
                {
                    _executionFlowSteps[executionFlowStep.Id] = executionFlowStep;

                    if (!_executionFlowStepsByExecutionFlowId.TryGetValue(executionFlowStep.ExecutionFlowId, out var steps))
                    {
                        steps = new List<ExecutionFlowStep>();
                        _executionFlowStepsByExecutionFlowId[executionFlowStep.ExecutionFlowId] = steps;
                    }
                    steps.Add(executionFlowStep);
                    executionFlowStepsLoaded = true;
                }
                catch
                {
                    continue;
                }
            }
        }
        
        // Fallback: Initialize default execution flow steps if SeedDataService is not available or no steps were loaded
        if (!executionFlowStepsLoaded)
        {
            InitializeDefaultExecutionFlowSteps();
        }
    }

    private void InitializeSeedData(SeedDataService seedDataService)
    {
        var executionFlowSteps = seedDataService.GetExecutionFlowSteps();
        foreach (var executionFlowStep in executionFlowSteps)
        {
            try
            {
                _executionFlowSteps[executionFlowStep.Id] = executionFlowStep;

                if (!_executionFlowStepsByExecutionFlowId.TryGetValue(executionFlowStep.ExecutionFlowId, out var steps))
                {
                    steps = new List<ExecutionFlowStep>();
                    _executionFlowStepsByExecutionFlowId[executionFlowStep.ExecutionFlowId] = steps;
                }
                steps.Add(executionFlowStep);
            }
            catch
            {
                continue;
            }
        }
    }

    private void InitializeDefaultExecutionFlowSteps()
    {
        var executionFlowId1 = new Guid("b50e8400-e29b-41d4-a716-446655440001");
        
        var step1 = new ExecutionFlowStep(
            executionFlowId1,
            1,
            "Anamnese Inicial",
            "Coleta de informações sobre histórico médico, queixas principais e sintomas atuais do paciente",
            1,
            "Passo 1: Anamnese Inicial",
            null
        );
        var idProperty = typeof(Entity).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(step1, new Guid("c50e8400-e29b-41d4-a716-446655440001"));
        }
        _executionFlowSteps[step1.Id] = step1;
        AddToIndex(step1);

        var step2 = new ExecutionFlowStep(
            executionFlowId1,
            2,
            "Histórico Clínico",
            "Avaliação de histórico de doenças, cirurgias, alergias e medicações em uso",
            2,
            "Passo 2: Histórico Clínico",
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(step2, new Guid("c50e8400-e29b-41d4-a716-446655440002"));
        }
        _executionFlowSteps[step2.Id] = step2;
        AddToIndex(step2);

        // Steps for Limpeza de Pele ExecutionFlow (b50e8400-e29b-41d4-a716-446655440002)
        var executionFlowId2 = new Guid("b50e8400-e29b-41d4-a716-446655440002");
        
        // Step 1: Avaliação Inicial da Pele
        var step3 = new ExecutionFlowStep(
            executionFlowId2,
            1,
            "Avaliação Inicial da Pele",
            "Avaliação da condição atual da pele, tipo de pele, sensibilidade e histórico de tratamentos",
            1,
            "Passo 1: Avaliação Inicial",
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(step3, new Guid("c50e8400-e29b-41d4-a716-446655440003"));
        }
        _executionFlowSteps[step3.Id] = step3;
        AddToIndex(step3);

        // Step 2: Preparação e Higienização
        var step4 = new ExecutionFlowStep(
            executionFlowId2,
            2,
            "Preparação e Higienização",
            "Preparação da pele para o procedimento, incluindo higienização e aplicação de produtos preparatórios",
            2,
            "Passo 2: Preparação",
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(step4, new Guid("c50e8400-e29b-41d4-a716-446655440004"));
        }
        _executionFlowSteps[step4.Id] = step4;
        AddToIndex(step4);

        // Step 3: Procedimento de Limpeza
        var step5 = new ExecutionFlowStep(
            executionFlowId2,
            3,
            "Procedimento de Limpeza",
            "Realização da limpeza profunda, extração de cravos e espinhas, e aplicação de máscaras",
            3,
            "Passo 3: Limpeza",
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(step5, new Guid("c50e8400-e29b-41d4-a716-446655440005"));
        }
        _executionFlowSteps[step5.Id] = step5;
        AddToIndex(step5);

        // Step 4: Finalização e Cuidados Pós-Tratamento
        var step6 = new ExecutionFlowStep(
            executionFlowId2,
            4,
            "Finalização e Cuidados Pós-Tratamento",
            "Aplicação de produtos finais, proteção solar e orientações sobre cuidados pós-tratamento",
            4,
            "Passo 4: Finalização",
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(step6, new Guid("c50e8400-e29b-41d4-a716-446655440006"));
        }
        _executionFlowSteps[step6.Id] = step6;
        AddToIndex(step6);
    }

    private void AddToIndex(ExecutionFlowStep step)
    {
        if (!_executionFlowStepsByExecutionFlowId.TryGetValue(step.ExecutionFlowId, out var steps))
        {
            steps = new List<ExecutionFlowStep>();
            _executionFlowStepsByExecutionFlowId[step.ExecutionFlowId] = steps;
        }
        steps.Add(step);
    }

    public Task<ExecutionFlowStep?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _executionFlowSteps.TryGetValue(id, out var executionFlowStep);
            return Task.FromResult<ExecutionFlowStep?>(executionFlowStep?.IsDeleted == false ? executionFlowStep : null);
        }
    }

    public Task<IEnumerable<ExecutionFlowStep>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<ExecutionFlowStep>>(
                _executionFlowSteps.Values.Where(efs => !efs.IsDeleted).ToList()
            );
        }
    }

    public Task<IEnumerable<ExecutionFlowStep>> GetByExecutionFlowIdAsync(Guid executionFlowId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_executionFlowStepsByExecutionFlowId.TryGetValue(executionFlowId, out var steps))
                return Task.FromResult<IEnumerable<ExecutionFlowStep>>(Enumerable.Empty<ExecutionFlowStep>());

            return Task.FromResult<IEnumerable<ExecutionFlowStep>>(
                steps.Where(s => !s.IsDeleted).ToList()
            );
        }
    }

    public Task<ExecutionFlowStep> CreateAsync(ExecutionFlowStep executionFlowStep, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_executionFlowSteps.ContainsKey(executionFlowStep.Id))
                throw new InvalidOperationException($"ExecutionFlowStep with id {executionFlowStep.Id} already exists");

            _executionFlowSteps[executionFlowStep.Id] = executionFlowStep;

            if (!_executionFlowStepsByExecutionFlowId.TryGetValue(executionFlowStep.ExecutionFlowId, out var steps))
            {
                steps = new List<ExecutionFlowStep>();
                _executionFlowStepsByExecutionFlowId[executionFlowStep.ExecutionFlowId] = steps;
            }
            steps.Add(executionFlowStep);

            return Task.FromResult(executionFlowStep);
        }
    }

    public Task<ExecutionFlowStep> UpdateAsync(ExecutionFlowStep executionFlowStep, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_executionFlowSteps.ContainsKey(executionFlowStep.Id))
                throw new InvalidOperationException($"ExecutionFlowStep with id {executionFlowStep.Id} not found");

            _executionFlowSteps[executionFlowStep.Id] = executionFlowStep;
            return Task.FromResult(executionFlowStep);
        }
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_executionFlowSteps.TryGetValue(id, out var executionFlowStep))
                return Task.FromResult(false);

            executionFlowStep.MarkAsDeleted();
            return Task.FromResult(true);
        }
    }

    public void Reset(SeedDataService? seedDataService = null)
    {
        lock (_lock)
        {
            _executionFlowSteps.Clear();
            _executionFlowStepsByExecutionFlowId.Clear();
            
            bool executionFlowStepsLoaded = false;
            if (seedDataService != null)
            {
                var executionFlowSteps = seedDataService.GetExecutionFlowSteps();
                foreach (var executionFlowStep in executionFlowSteps)
                {
                    try
                    {
                        _executionFlowSteps[executionFlowStep.Id] = executionFlowStep;
                        AddToIndex(executionFlowStep);
                        executionFlowStepsLoaded = true;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            
            if (!executionFlowStepsLoaded)
            {
                InitializeDefaultExecutionFlowSteps();
            }
        }
    }
}

