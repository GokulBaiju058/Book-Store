using BookStore_API.Business.Abstractions;
using BookStore_API.Business.Strategies.BookSearchStrategy;
using BookStore_API.Data.Enum;

namespace BookStore_API.Business.Factories
{
    public class BookSearchStrategyFactory
    {
        private readonly IDictionary<SearchTypeEnum, IBookSearchStrategy> _strategies;

        public BookSearchStrategyFactory(StandardSearchStrategy standardStrategy,
                                        InventorySearchStrategy inventoryStrategy)
        {
            _strategies = new Dictionary<SearchTypeEnum, IBookSearchStrategy>
            {
                { SearchTypeEnum.Standard, standardStrategy },
                { SearchTypeEnum.Inventory, inventoryStrategy }
            };
        }

        public IBookSearchStrategy GetStrategy(SearchTypeEnum searchType)
        {
            if (_strategies.TryGetValue(searchType, out var strategy))
            {
                return strategy;
            }
            return _strategies[SearchTypeEnum.Standard];
        }
    }
}
