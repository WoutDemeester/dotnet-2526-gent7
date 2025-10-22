using Rise.Domain.Infrastructure;
using Rise.Shared.Resto;

namespace Rise.Domain.Extensions
{
    public static class RestoExtensions
    {
        public static RestoDto MapToRestoDto(this Resto r)
        {
            return new RestoDto
            {
                Id = r.Id,
                Name = r.Name,
                Coordinates = r.Coordinates,
                Menu = new MenuDto
                {
                    Id = r.Menu.Id,
                    Items = r.Menu.Items.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Select(item => new MenuItemDto
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Description = item.Description,
                            IsVeganAndHalal = item.IsVeganAndHalal,
                            FoodCategory = item.Category.ToString()
                        }).ToList()
                    )
                }
            };
        }
    }
}
