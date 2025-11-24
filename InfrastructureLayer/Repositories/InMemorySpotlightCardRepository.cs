using ApplicationLayer.Interfaces.Repositories;
using DomainLayer.Entities;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repositories;

public class InMemorySpotlightCardRepository : ISpotlightCardRepository
{
    private readonly Dictionary<Guid, SpotlightCard> _spotlightCards = new();
    private readonly object _lock = new();

    public InMemorySpotlightCardRepository(SeedDataService? seedDataService = null)
    {
        bool cardsLoaded = false;
        
        if (seedDataService != null)
        {
            var spotlightCards = seedDataService.GetSpotlightCards();
            foreach (var spotlightCard in spotlightCards)
            {
                try
                {
                    _spotlightCards[spotlightCard.Id] = spotlightCard;
                    cardsLoaded = true;
                }
                catch
                {
                    continue;
                }
            }
        }
        
        // Fallback: Initialize default spotlight cards if SeedDataService is not available or no cards were loaded
        if (!cardsLoaded)
        {
            InitializeDefaultCards();
        }
    }

    private void InitializeDefaultCards()
    {
        // Card 1: Promoção Especial de Verão
        var card1 = new SpotlightCard(
            "Promoção Especial de Verão",
            "Desconto imperdível de 30% em todos os tratamentos estéticos",
            "Aproveite nossa promoção especial de verão! Desconto de 30% em todos os tratamentos estéticos, incluindo limpeza de pele, peelings e tratamentos faciais. Agende sua consulta agora e cuide da sua pele com quem entende. Válido até 31 de dezembro de 2025.",
            new DateTime(2025, 1, 1),
            new DateTime(2025, 12, 31),
            "https://images.unsplash.com/photo-1612817288484-6f916006741a?w=800&h=600&fit=crop&auto=format",
            "Ver mais detalhes",
            "https://www.youtube.com/watch?v=urkuHAqwgFc&t=293s"
        );
        _spotlightCards[card1.Id] = card1;

        // Card 2: Nova Linha de Produtos Premium
        var card2 = new SpotlightCard(
            "Nova Linha de Produtos Premium",
            "Descubra nossa linha exclusiva de cuidados com a pele",
            "Apresentamos nossa nova linha de produtos para cuidados com a pele. Formulados com ingredientes naturais selecionados e tecnologia avançada, nossos produtos foram desenvolvidos para proporcionar resultados visíveis e duradouros. Conheça toda a linha e transforme sua rotina de cuidados.",
            new DateTime(2025, 1, 15),
            new DateTime(2025, 12, 31),
            "https://images.unsplash.com/photo-1620916566398-39f1143ab7be?w=800&h=600&fit=crop&auto=format",
            "Ver mais detalhes",
            "https://www.youtube.com/watch?v=urkuHAqwgFc&t=293s"
        );
        _spotlightCards[card2.Id] = card2;
    }

    public Task<SpotlightCard?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _spotlightCards.TryGetValue(id, out var spotlightCard);
            return Task.FromResult<SpotlightCard?>(spotlightCard?.IsDeleted == false ? spotlightCard : null);
        }
    }

    public Task<IEnumerable<SpotlightCard>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<SpotlightCard>>(
                _spotlightCards.Values.Where(sc => !sc.IsDeleted).ToList()
            );
        }
    }

    public Task<SpotlightCard> CreateAsync(SpotlightCard spotlightCard, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_spotlightCards.ContainsKey(spotlightCard.Id))
                throw new InvalidOperationException($"SpotlightCard with id {spotlightCard.Id} already exists");

            _spotlightCards[spotlightCard.Id] = spotlightCard;
            return Task.FromResult(spotlightCard);
        }
    }

    public Task<SpotlightCard> UpdateAsync(SpotlightCard spotlightCard, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_spotlightCards.ContainsKey(spotlightCard.Id))
                throw new InvalidOperationException($"SpotlightCard with id {spotlightCard.Id} not found");

            _spotlightCards[spotlightCard.Id] = spotlightCard;
            return Task.FromResult(spotlightCard);
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var spotlightCard = await GetByIdAsync(id, cancellationToken);
        if (spotlightCard == null)
            return false;

        lock (_lock)
        {
            spotlightCard.MarkAsDeleted();
            _spotlightCards[spotlightCard.Id] = spotlightCard;
            return true;
        }
    }

    public void Reset(SeedDataService? seedDataService = null)
    {
        lock (_lock)
        {
            _spotlightCards.Clear();
            
            bool cardsLoaded = false;
            if (seedDataService != null)
            {
                var cards = seedDataService.GetSpotlightCards();
                foreach (var card in cards)
                {
                    try
                    {
                        _spotlightCards[card.Id] = card;
                        cardsLoaded = true;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            
            if (!cardsLoaded)
            {
                InitializeDefaultCards();
            }
        }
    }
}

