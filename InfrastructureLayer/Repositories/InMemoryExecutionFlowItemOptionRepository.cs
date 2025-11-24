using ApplicationLayer.Interfaces.Repositories;
using DomainLayer.Entities;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repositories;

public class InMemoryExecutionFlowItemOptionRepository : IExecutionFlowItemOptionRepository
{
    private readonly Dictionary<Guid, ExecutionFlowItemOption> _executionFlowItemOptions = new();
    private readonly Dictionary<Guid, List<ExecutionFlowItemOption>> _executionFlowItemOptionsByItemId = new();
    private readonly object _lock = new();

    public InMemoryExecutionFlowItemOptionRepository(SeedDataService? seedDataService = null)
    {
        bool executionFlowItemOptionsLoaded = false;
        
        if (seedDataService != null)
        {
            var executionFlowItemOptions = seedDataService.GetExecutionFlowItemOptions();
            foreach (var executionFlowItemOption in executionFlowItemOptions)
            {
                try
                {
                    _executionFlowItemOptions[executionFlowItemOption.Id] = executionFlowItemOption;

                    if (!_executionFlowItemOptionsByItemId.TryGetValue(executionFlowItemOption.ExecutionFlowStepItemId, out var options))
                    {
                        options = new List<ExecutionFlowItemOption>();
                        _executionFlowItemOptionsByItemId[executionFlowItemOption.ExecutionFlowStepItemId] = options;
                    }
                    options.Add(executionFlowItemOption);
                    executionFlowItemOptionsLoaded = true;
                }
                catch
                {
                    continue;
                }
            }
        }
        
        // Fallback: Initialize default execution flow item options if SeedDataService is not available or no options were loaded
        if (!executionFlowItemOptionsLoaded)
        {
            InitializeDefaultExecutionFlowItemOptions();
        }
    }

    private void InitializeSeedData(SeedDataService seedDataService)
    {
        var executionFlowItemOptions = seedDataService.GetExecutionFlowItemOptions();
        foreach (var executionFlowItemOption in executionFlowItemOptions)
        {
            try
            {
                _executionFlowItemOptions[executionFlowItemOption.Id] = executionFlowItemOption;

                if (!_executionFlowItemOptionsByItemId.TryGetValue(executionFlowItemOption.ExecutionFlowStepItemId, out var options))
                {
                    options = new List<ExecutionFlowItemOption>();
                    _executionFlowItemOptionsByItemId[executionFlowItemOption.ExecutionFlowStepItemId] = options;
                }
                options.Add(executionFlowItemOption);
            }
            catch
            {
                continue;
            }
        }
    }

    private void InitializeDefaultExecutionFlowItemOptions()
    {
        var itemId1 = new Guid("d50e8400-e29b-41d4-a716-446655440004");
        
        var option1 = new ExecutionFlowItemOption(itemId1, "Sim", "sim", 0);
        var idProperty = typeof(Entity).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option1, new Guid("e50e8400-e29b-41d4-a716-446655440001"));
        }
        _executionFlowItemOptions[option1.Id] = option1;
        AddToIndex(option1);

        var option2 = new ExecutionFlowItemOption(itemId1, "Não", "nao", 1);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option2, new Guid("e50e8400-e29b-41d4-a716-446655440002"));
        }
        _executionFlowItemOptions[option2.Id] = option2;
        AddToIndex(option2);

        // Options for Limpeza de Pele ExecutionFlow Items
        
        // Item 3: Tipo de pele (d50e8400-e29b-41d4-a716-446655440003)
        var itemId3 = new Guid("d50e8400-e29b-41d4-a716-446655440003");
        var option3 = new ExecutionFlowItemOption(itemId3, "Oleosa", "oleosa", 0);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option3, new Guid("e50e8400-e29b-41d4-a716-446655440003"));
        }
        _executionFlowItemOptions[option3.Id] = option3;
        AddToIndex(option3);

        var option4 = new ExecutionFlowItemOption(itemId3, "Seca", "seca", 1);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option4, new Guid("e50e8400-e29b-41d4-a716-446655440004"));
        }
        _executionFlowItemOptions[option4.Id] = option4;
        AddToIndex(option4);

        var option5 = new ExecutionFlowItemOption(itemId3, "Mista", "mista", 2);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option5, new Guid("e50e8400-e29b-41d4-a716-446655440005"));
        }
        _executionFlowItemOptions[option5.Id] = option5;
        AddToIndex(option5);

        var option6 = new ExecutionFlowItemOption(itemId3, "Normal", "normal", 3);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option6, new Guid("e50e8400-e29b-41d4-a716-446655440006"));
        }
        _executionFlowItemOptions[option6.Id] = option6;
        AddToIndex(option6);

        // Item 4: Sensibilidade (d50e8400-e29b-41d4-a716-446655440004)
        var itemId4 = new Guid("d50e8400-e29b-41d4-a716-446655440004");
        var option7 = new ExecutionFlowItemOption(itemId4, "Sim", "sim", 0);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option7, new Guid("e50e8400-e29b-41d4-a716-446655440007"));
        }
        _executionFlowItemOptions[option7.Id] = option7;
        AddToIndex(option7);

        var option8 = new ExecutionFlowItemOption(itemId4, "Não", "nao", 1);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option8, new Guid("e50e8400-e29b-41d4-a716-446655440008"));
        }
        _executionFlowItemOptions[option8.Id] = option8;
        AddToIndex(option8);

        // Item 5: Alergias (d50e8400-e29b-41d4-a716-446655440005)
        var itemId5 = new Guid("d50e8400-e29b-41d4-a716-446655440005");
        var option9 = new ExecutionFlowItemOption(itemId5, "Sim", "sim", 0);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option9, new Guid("e50e8400-e29b-41d4-a716-446655440009"));
        }
        _executionFlowItemOptions[option9.Id] = option9;
        AddToIndex(option9);

        var option10 = new ExecutionFlowItemOption(itemId5, "Não", "nao", 1);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option10, new Guid("e50e8400-e29b-41d4-a716-446655440010"));
        }
        _executionFlowItemOptions[option10.Id] = option10;
        AddToIndex(option10);

        // Item 8: Técnica de higienização (d50e8400-e29b-41d4-a716-446655440008)
        var itemId8 = new Guid("d50e8400-e29b-41d4-a716-446655440008");
        var option11 = new ExecutionFlowItemOption(itemId8, "Vapor de ozônio", "vapor_ozonio", 0);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option11, new Guid("e50e8400-e29b-41d4-a716-446655440011"));
        }
        _executionFlowItemOptions[option11.Id] = option11;
        AddToIndex(option11);

        var option12 = new ExecutionFlowItemOption(itemId8, "Esfoliação química", "esfoliacao_quimica", 1);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option12, new Guid("e50e8400-e29b-41d4-a716-446655440012"));
        }
        _executionFlowItemOptions[option12.Id] = option12;
        AddToIndex(option12);

        var option13 = new ExecutionFlowItemOption(itemId8, "Esfoliação mecânica", "esfoliacao_mecanica", 2);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option13, new Guid("e50e8400-e29b-41d4-a716-446655440013"));
        }
        _executionFlowItemOptions[option13.Id] = option13;
        AddToIndex(option13);

        var option14 = new ExecutionFlowItemOption(itemId8, "Limpeza manual", "limpeza_manual", 3);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option14, new Guid("e50e8400-e29b-41d4-a716-446655440014"));
        }
        _executionFlowItemOptions[option14.Id] = option14;
        AddToIndex(option14);

        // Item 10: Áreas tratadas (d50e8400-e29b-41d4-a716-446655440010)
        var itemId10 = new Guid("d50e8400-e29b-41d4-a716-446655440010");
        var option15 = new ExecutionFlowItemOption(itemId10, "Testa", "testa", 0);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option15, new Guid("e50e8400-e29b-41d4-a716-446655440015"));
        }
        _executionFlowItemOptions[option15.Id] = option15;
        AddToIndex(option15);

        var option16 = new ExecutionFlowItemOption(itemId10, "Nariz", "nariz", 1);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option16, new Guid("e50e8400-e29b-41d4-a716-446655440016"));
        }
        _executionFlowItemOptions[option16.Id] = option16;
        AddToIndex(option16);

        var option17 = new ExecutionFlowItemOption(itemId10, "Bochechas", "bochechas", 2);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option17, new Guid("e50e8400-e29b-41d4-a716-446655440017"));
        }
        _executionFlowItemOptions[option17.Id] = option17;
        AddToIndex(option17);

        var option18 = new ExecutionFlowItemOption(itemId10, "Queixo", "queixo", 3);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option18, new Guid("e50e8400-e29b-41d4-a716-446655440018"));
        }
        _executionFlowItemOptions[option18.Id] = option18;
        AddToIndex(option18);

        var option19 = new ExecutionFlowItemOption(itemId10, "Pescoço", "pescoco", 4);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option19, new Guid("e50e8400-e29b-41d4-a716-446655440019"));
        }
        _executionFlowItemOptions[option19.Id] = option19;
        AddToIndex(option19);

        // Item 15: Protetor solar (d50e8400-e29b-41d4-a716-446655440015)
        var itemId15 = new Guid("d50e8400-e29b-41d4-a716-446655440015");
        var option20 = new ExecutionFlowItemOption(itemId15, "Sim", "sim", 0);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option20, new Guid("e50e8400-e29b-41d4-a716-446655440020"));
        }
        _executionFlowItemOptions[option20.Id] = option20;
        AddToIndex(option20);

        var option21 = new ExecutionFlowItemOption(itemId15, "Não", "nao", 1);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option21, new Guid("e50e8400-e29b-41d4-a716-446655440021"));
        }
        _executionFlowItemOptions[option21.Id] = option21;
        AddToIndex(option21);

        // Item 17: Orientações (d50e8400-e29b-41d4-a716-446655440017)
        var itemId17 = new Guid("d50e8400-e29b-41d4-a716-446655440017");
        var option22 = new ExecutionFlowItemOption(itemId17, "Evitar exposição solar", "evitar_exposicao_solar", 0);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option22, new Guid("e50e8400-e29b-41d4-a716-446655440022"));
        }
        _executionFlowItemOptions[option22.Id] = option22;
        AddToIndex(option22);

        var option23 = new ExecutionFlowItemOption(itemId17, "Usar protetor solar diariamente", "usar_protetor_solar", 1);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option23, new Guid("e50e8400-e29b-41d4-a716-446655440023"));
        }
        _executionFlowItemOptions[option23.Id] = option23;
        AddToIndex(option23);

        var option24 = new ExecutionFlowItemOption(itemId17, "Evitar produtos com ácidos", "evitar_acidos", 2);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option24, new Guid("e50e8400-e29b-41d4-a716-446655440024"));
        }
        _executionFlowItemOptions[option24.Id] = option24;
        AddToIndex(option24);

        var option25 = new ExecutionFlowItemOption(itemId17, "Hidratar a pele", "hidratar_pele", 3);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option25, new Guid("e50e8400-e29b-41d4-a716-446655440025"));
        }
        _executionFlowItemOptions[option25.Id] = option25;
        AddToIndex(option25);

        var option26 = new ExecutionFlowItemOption(itemId17, "Retornar em 30 dias", "retornar_30_dias", 4);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(option26, new Guid("e50e8400-e29b-41d4-a716-446655440026"));
        }
        _executionFlowItemOptions[option26.Id] = option26;
        AddToIndex(option26);
    }

    private void AddToIndex(ExecutionFlowItemOption option)
    {
        if (!_executionFlowItemOptionsByItemId.TryGetValue(option.ExecutionFlowStepItemId, out var options))
        {
            options = new List<ExecutionFlowItemOption>();
            _executionFlowItemOptionsByItemId[option.ExecutionFlowStepItemId] = options;
        }
        options.Add(option);
    }

    public Task<ExecutionFlowItemOption?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _executionFlowItemOptions.TryGetValue(id, out var executionFlowItemOption);
            return Task.FromResult<ExecutionFlowItemOption?>(executionFlowItemOption?.IsDeleted == false ? executionFlowItemOption : null);
        }
    }

    public Task<IEnumerable<ExecutionFlowItemOption>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<ExecutionFlowItemOption>>(
                _executionFlowItemOptions.Values.Where(efio => !efio.IsDeleted).ToList()
            );
        }
    }

    public Task<IEnumerable<ExecutionFlowItemOption>> GetByExecutionFlowStepItemIdAsync(Guid executionFlowStepItemId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_executionFlowItemOptionsByItemId.TryGetValue(executionFlowStepItemId, out var options))
                return Task.FromResult<IEnumerable<ExecutionFlowItemOption>>(Enumerable.Empty<ExecutionFlowItemOption>());

            return Task.FromResult<IEnumerable<ExecutionFlowItemOption>>(
                options.Where(o => !o.IsDeleted).OrderBy(o => o.Order).ToList()
            );
        }
    }

    public Task<ExecutionFlowItemOption> CreateAsync(ExecutionFlowItemOption executionFlowItemOption, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_executionFlowItemOptions.ContainsKey(executionFlowItemOption.Id))
                throw new InvalidOperationException($"ExecutionFlowItemOption with id {executionFlowItemOption.Id} already exists");

            _executionFlowItemOptions[executionFlowItemOption.Id] = executionFlowItemOption;

            if (!_executionFlowItemOptionsByItemId.TryGetValue(executionFlowItemOption.ExecutionFlowStepItemId, out var options))
            {
                options = new List<ExecutionFlowItemOption>();
                _executionFlowItemOptionsByItemId[executionFlowItemOption.ExecutionFlowStepItemId] = options;
            }
            options.Add(executionFlowItemOption);

            return Task.FromResult(executionFlowItemOption);
        }
    }

    public Task<ExecutionFlowItemOption> UpdateAsync(ExecutionFlowItemOption executionFlowItemOption, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_executionFlowItemOptions.ContainsKey(executionFlowItemOption.Id))
                throw new InvalidOperationException($"ExecutionFlowItemOption with id {executionFlowItemOption.Id} not found");

            _executionFlowItemOptions[executionFlowItemOption.Id] = executionFlowItemOption;
            return Task.FromResult(executionFlowItemOption);
        }
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_executionFlowItemOptions.TryGetValue(id, out var executionFlowItemOption))
                return Task.FromResult(false);

            executionFlowItemOption.MarkAsDeleted();
            return Task.FromResult(true);
        }
    }

    public void Reset(SeedDataService? seedDataService = null)
    {
        lock (_lock)
        {
            _executionFlowItemOptions.Clear();
            _executionFlowItemOptionsByItemId.Clear();
            
            bool executionFlowItemOptionsLoaded = false;
            if (seedDataService != null)
            {
                var executionFlowItemOptions = seedDataService.GetExecutionFlowItemOptions();
                foreach (var executionFlowItemOption in executionFlowItemOptions)
                {
                    try
                    {
                        _executionFlowItemOptions[executionFlowItemOption.Id] = executionFlowItemOption;
                        AddToIndex(executionFlowItemOption);
                        executionFlowItemOptionsLoaded = true;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            
            if (!executionFlowItemOptionsLoaded)
            {
                InitializeDefaultExecutionFlowItemOptions();
            }
        }
    }
}

