using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using Common.Models;

namespace Common.Services
{
    public class MockMenuService : IMenuService
    {
        private string[] food = new[] { "ribs", "brisket", "chicken", "turkey", "pulled pork" };

        public Task DeleteMenuItemAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<MenuItem> GetMenuItemAsync(long id) =>
            Task.FromResult(GetMenuItemFake().Generate());

        public Task<List<MenuItem>> GetMenuItemsAsync() =>
            Task.FromResult(GetMenuItemFake().Generate(new Faker().Random.Number(5, 10)));

        public Task SaveMenuItemAsync(MenuItem item, bool newOne)
        {
            throw new NotImplementedException();
        }

        private Faker<MenuItem> GetMenuItemFake() =>
            new Faker<MenuItem>()
            .RuleFor(item => item.Name, fake => fake.PickRandom(food))
            .RuleFor(item => item.Price, fake => (float)fake.Finance.Amount())
            .RuleFor(item => item.Id, fake => fake.Random.Long());
    }
}
