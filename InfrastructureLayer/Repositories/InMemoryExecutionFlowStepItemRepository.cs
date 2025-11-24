using ApplicationLayer.Interfaces.Repositories;
using DomainLayer.Entities;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repositories;

public class InMemoryExecutionFlowStepItemRepository : IExecutionFlowStepItemRepository
{
    private readonly Dictionary<Guid, ExecutionFlowStepItem> _executionFlowStepItems = new();
    private readonly Dictionary<Guid, List<ExecutionFlowStepItem>> _executionFlowStepItemsByStepId = new();
    private readonly object _lock = new();

    public InMemoryExecutionFlowStepItemRepository(SeedDataService? seedDataService = null)
    {
        bool executionFlowStepItemsLoaded = false;
        
        if (seedDataService != null)
        {
            var executionFlowStepItems = seedDataService.GetExecutionFlowStepItems();
            foreach (var executionFlowStepItem in executionFlowStepItems)
            {
                try
                {
                    _executionFlowStepItems[executionFlowStepItem.Id] = executionFlowStepItem;

                    if (!_executionFlowStepItemsByStepId.TryGetValue(executionFlowStepItem.ExecutionFlowStepId, out var items))
                    {
                        items = new List<ExecutionFlowStepItem>();
                        _executionFlowStepItemsByStepId[executionFlowStepItem.ExecutionFlowStepId] = items;
                    }
                    items.Add(executionFlowStepItem);
                    executionFlowStepItemsLoaded = true;
                }
                catch
                {
                    continue;
                }
            }
        }
        
        // Fallback: Initialize default execution flow step items if SeedDataService is not available or no items were loaded
        if (!executionFlowStepItemsLoaded)
        {
            InitializeDefaultExecutionFlowStepItems();
        }
    }

    private void InitializeSeedData(SeedDataService seedDataService)
    {
        var executionFlowStepItems = seedDataService.GetExecutionFlowStepItems();
        foreach (var executionFlowStepItem in executionFlowStepItems)
        {
            try
            {
                _executionFlowStepItems[executionFlowStepItem.Id] = executionFlowStepItem;

                if (!_executionFlowStepItemsByStepId.TryGetValue(executionFlowStepItem.ExecutionFlowStepId, out var items))
                {
                    items = new List<ExecutionFlowStepItem>();
                    _executionFlowStepItemsByStepId[executionFlowStepItem.ExecutionFlowStepId] = items;
                }
                items.Add(executionFlowStepItem);
            }
            catch
            {
                continue;
            }
        }
    }

    private void InitializeDefaultExecutionFlowStepItems()
    {
        var stepId1 = new Guid("c50e8400-e29b-41d4-a716-446655440001");
        
        var item1 = new ExecutionFlowStepItem(
            stepId1,
            1,
            "Qual é o motivo principal da consulta?",
            DomainLayer.Enums.AnswerType.Text,
            true,
            null,
            null,
            500,
            null,
            null
        );
        var idProperty = typeof(Entity).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(item1, new Guid("d50e8400-e29b-41d4-a716-446655440001"));
        }
        _executionFlowStepItems[item1.Id] = item1;
        AddToIndex(item1);

        var item2 = new ExecutionFlowStepItem(
            stepId1,
            2,
            "Há quanto tempo os sintomas estão presentes?",
            DomainLayer.Enums.AnswerType.Text,
            true,
            null,
            "Informe o tempo de início dos sintomas",
            200,
            null,
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(item2, new Guid("d50e8400-e29b-41d4-a716-446655440002"));
        }
        _executionFlowStepItems[item2.Id] = item2;
        AddToIndex(item2);

        // Items for Limpeza de Pele ExecutionFlow Steps
        
        // Step 3 (Avaliação Inicial) - c50e8400-e29b-41d4-a716-446655440003
        var stepId3 = new Guid("c50e8400-e29b-41d4-a716-446655440003");
        
        var item3 = new ExecutionFlowStepItem(
            stepId3,
            1,
            "Qual o tipo de pele do cliente?",
            DomainLayer.Enums.AnswerType.Radio,
            true,
            null,
            null,
            null,
            null,
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(item3, new Guid("d50e8400-e29b-41d4-a716-446655440003"));
        }
        _executionFlowStepItems[item3.Id] = item3;
        AddToIndex(item3);

        var item4 = new ExecutionFlowStepItem(
            stepId3,
            2,
            "A pele apresenta sensibilidade?",
            DomainLayer.Enums.AnswerType.Radio,
            true,
            null,
            null,
            null,
            null,
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(item4, new Guid("d50e8400-e29b-41d4-a716-446655440004"));
        }
        _executionFlowStepItems[item4.Id] = item4;
        AddToIndex(item4);

        var item5 = new ExecutionFlowStepItem(
            stepId3,
            3,
            "Há histórico de alergias ou reações a produtos?",
            DomainLayer.Enums.AnswerType.Radio,
            true,
            null,
            "Descreva as alergias ou reações conhecidas",
            null,
            null,
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(item5, new Guid("d50e8400-e29b-41d4-a716-446655440005"));
        }
        _executionFlowStepItems[item5.Id] = item5;
        AddToIndex(item5);

        var item6 = new ExecutionFlowStepItem(
            stepId3,
            4,
            "Observações sobre a condição atual da pele",
            DomainLayer.Enums.AnswerType.Text,
            false,
            null,
            null,
            500,
            null,
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(item6, new Guid("d50e8400-e29b-41d4-a716-446655440006"));
        }
        _executionFlowStepItems[item6.Id] = item6;
        AddToIndex(item6);

        // Step 4 (Preparação) - c50e8400-e29b-41d4-a716-446655440004
        var stepId4 = new Guid("c50e8400-e29b-41d4-a716-446655440004");
        
        var item7 = new ExecutionFlowStepItem(
            stepId4,
            1,
            "Produto de limpeza utilizado",
            DomainLayer.Enums.AnswerType.Text,
            true,
            null,
            null,
            100,
            null,
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(item7, new Guid("d50e8400-e29b-41d4-a716-446655440007"));
        }
        _executionFlowStepItems[item7.Id] = item7;
        AddToIndex(item7);

        var item8 = new ExecutionFlowStepItem(
            stepId4,
            2,
            "Técnica de higienização aplicada",
            DomainLayer.Enums.AnswerType.Multiselect,
            true,
            null,
            null,
            null,
            null,
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(item8, new Guid("d50e8400-e29b-41d4-a716-446655440008"));
        }
        _executionFlowStepItems[item8.Id] = item8;
        AddToIndex(item8);

        var item9 = new ExecutionFlowStepItem(
            stepId4,
            3,
            "Tempo de preparação (minutos)",
            DomainLayer.Enums.AnswerType.Number,
            true,
            null,
            null,
            null,
            null,
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(item9, new Guid("d50e8400-e29b-41d4-a716-446655440009"));
        }
        _executionFlowStepItems[item9.Id] = item9;
        AddToIndex(item9);

        // Step 5 (Procedimento) - c50e8400-e29b-41d4-a716-446655440005
        var stepId5 = new Guid("c50e8400-e29b-41d4-a716-446655440005");
        
        var item10 = new ExecutionFlowStepItem(
            stepId5,
            1,
            "Áreas tratadas",
            DomainLayer.Enums.AnswerType.Multiselect,
            true,
            null,
            null,
            null,
            null,
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(item10, new Guid("d50e8400-e29b-41d4-a716-446655440010"));
        }
        _executionFlowStepItems[item10.Id] = item10;
        AddToIndex(item10);

        var item11 = new ExecutionFlowStepItem(
            stepId5,
            2,
            "Quantidade de cravos extraídos",
            DomainLayer.Enums.AnswerType.Number,
            false,
            null,
            null,
            null,
            null,
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(item11, new Guid("d50e8400-e29b-41d4-a716-446655440011"));
        }
        _executionFlowStepItems[item11.Id] = item11;
        AddToIndex(item11);

        var item12 = new ExecutionFlowStepItem(
            stepId5,
            3,
            "Quantidade de espinhas extraídas",
            DomainLayer.Enums.AnswerType.Number,
            false,
            null,
            null,
            null,
            null,
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(item12, new Guid("d50e8400-e29b-41d4-a716-446655440012"));
        }
        _executionFlowStepItems[item12.Id] = item12;
        AddToIndex(item12);

        var item13 = new ExecutionFlowStepItem(
            stepId5,
            4,
            "Máscara aplicada",
            DomainLayer.Enums.AnswerType.Text,
            false,
            null,
            null,
            100,
            null,
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(item13, new Guid("d50e8400-e29b-41d4-a716-446655440013"));
        }
        _executionFlowStepItems[item13.Id] = item13;
        AddToIndex(item13);

        var item14 = new ExecutionFlowStepItem(
            stepId5,
            5,
            "Observações sobre o procedimento",
            DomainLayer.Enums.AnswerType.Text,
            false,
            null,
            "Informe detalhes relevantes sobre o procedimento realizado",
            500,
            null,
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(item14, new Guid("d50e8400-e29b-41d4-a716-446655440014"));
        }
        _executionFlowStepItems[item14.Id] = item14;
        AddToIndex(item14);

        // Step 6 (Finalização) - c50e8400-e29b-41d4-a716-446655440006
        var stepId6 = new Guid("c50e8400-e29b-41d4-a716-446655440006");
        
        var item15 = new ExecutionFlowStepItem(
            stepId6,
            1,
            "Protetor solar aplicado?",
            DomainLayer.Enums.AnswerType.Radio,
            true,
            null,
            null,
            null,
            null,
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(item15, new Guid("d50e8400-e29b-41d4-a716-446655440015"));
        }
        _executionFlowStepItems[item15.Id] = item15;
        AddToIndex(item15);

        var item16 = new ExecutionFlowStepItem(
            stepId6,
            2,
            "FPS do protetor solar",
            DomainLayer.Enums.AnswerType.Number,
            false,
            null,
            null,
            null,
            null,
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(item16, new Guid("d50e8400-e29b-41d4-a716-446655440016"));
        }
        _executionFlowStepItems[item16.Id] = item16;
        AddToIndex(item16);

        var item17 = new ExecutionFlowStepItem(
            stepId6,
            3,
            "Orientações fornecidas ao cliente",
            DomainLayer.Enums.AnswerType.Multiselect,
            true,
            null,
            null,
            null,
            null,
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(item17, new Guid("d50e8400-e29b-41d4-a716-446655440017"));
        }
        _executionFlowStepItems[item17.Id] = item17;
        AddToIndex(item17);

        var item18 = new ExecutionFlowStepItem(
            stepId6,
            4,
            "Observações finais",
            DomainLayer.Enums.AnswerType.Text,
            false,
            null,
            null,
            300,
            null,
            null
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(item18, new Guid("d50e8400-e29b-41d4-a716-446655440018"));
        }
        _executionFlowStepItems[item18.Id] = item18;
        AddToIndex(item18);
    }

    private void AddToIndex(ExecutionFlowStepItem item)
    {
        if (!_executionFlowStepItemsByStepId.TryGetValue(item.ExecutionFlowStepId, out var items))
        {
            items = new List<ExecutionFlowStepItem>();
            _executionFlowStepItemsByStepId[item.ExecutionFlowStepId] = items;
        }
        items.Add(item);
    }

    public Task<ExecutionFlowStepItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _executionFlowStepItems.TryGetValue(id, out var executionFlowStepItem);
            return Task.FromResult<ExecutionFlowStepItem?>(executionFlowStepItem?.IsDeleted == false ? executionFlowStepItem : null);
        }
    }

    public Task<IEnumerable<ExecutionFlowStepItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<ExecutionFlowStepItem>>(
                _executionFlowStepItems.Values.Where(efsi => !efsi.IsDeleted).ToList()
            );
        }
    }

    public Task<IEnumerable<ExecutionFlowStepItem>> GetByExecutionFlowStepIdAsync(Guid executionFlowStepId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_executionFlowStepItemsByStepId.TryGetValue(executionFlowStepId, out var items))
                return Task.FromResult<IEnumerable<ExecutionFlowStepItem>>(Enumerable.Empty<ExecutionFlowStepItem>());

            return Task.FromResult<IEnumerable<ExecutionFlowStepItem>>(
                items.Where(i => !i.IsDeleted).ToList()
            );
        }
    }

    public Task<ExecutionFlowStepItem> CreateAsync(ExecutionFlowStepItem executionFlowStepItem, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_executionFlowStepItems.ContainsKey(executionFlowStepItem.Id))
                throw new InvalidOperationException($"ExecutionFlowStepItem with id {executionFlowStepItem.Id} already exists");

            _executionFlowStepItems[executionFlowStepItem.Id] = executionFlowStepItem;

            if (!_executionFlowStepItemsByStepId.TryGetValue(executionFlowStepItem.ExecutionFlowStepId, out var items))
            {
                items = new List<ExecutionFlowStepItem>();
                _executionFlowStepItemsByStepId[executionFlowStepItem.ExecutionFlowStepId] = items;
            }
            items.Add(executionFlowStepItem);

            return Task.FromResult(executionFlowStepItem);
        }
    }

    public Task<ExecutionFlowStepItem> UpdateAsync(ExecutionFlowStepItem executionFlowStepItem, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_executionFlowStepItems.ContainsKey(executionFlowStepItem.Id))
                throw new InvalidOperationException($"ExecutionFlowStepItem with id {executionFlowStepItem.Id} not found");

            _executionFlowStepItems[executionFlowStepItem.Id] = executionFlowStepItem;
            return Task.FromResult(executionFlowStepItem);
        }
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_executionFlowStepItems.TryGetValue(id, out var executionFlowStepItem))
                return Task.FromResult(false);

            executionFlowStepItem.MarkAsDeleted();
            return Task.FromResult(true);
        }
    }

    public void Reset(SeedDataService? seedDataService = null)
    {
        lock (_lock)
        {
            _executionFlowStepItems.Clear();
            _executionFlowStepItemsByStepId.Clear();
            
            bool executionFlowStepItemsLoaded = false;
            if (seedDataService != null)
            {
                var executionFlowStepItems = seedDataService.GetExecutionFlowStepItems();
                foreach (var executionFlowStepItem in executionFlowStepItems)
                {
                    try
                    {
                        _executionFlowStepItems[executionFlowStepItem.Id] = executionFlowStepItem;
                        AddToIndex(executionFlowStepItem);
                        executionFlowStepItemsLoaded = true;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            
            if (!executionFlowStepItemsLoaded)
            {
                InitializeDefaultExecutionFlowStepItems();
            }
        }
    }
}

