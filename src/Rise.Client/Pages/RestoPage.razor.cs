using Microsoft.AspNetCore.Components;
using Rise.Client.Services;
using Rise.Shared.Common;
using Rise.Shared.Resto;
using System.Collections.Generic;

namespace Rise.Client.Pages
{
    public partial class RestoPage
    {

        [Inject] public required IRestoService restoService { get; set; }
        private List<string> Restos = new() { "Aan het laden..." };
        List<String> weekOrder = new List<string> { "Ma", "Di", "Wo", "Do", "Vr" };



        public class MenuEntry
        {
            public string Category { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public bool IsVegan { get; set; }
            public bool IsVeggie { get; set; }
        }

        public class MenuDay
        {
            public string Day { get; set; } = string.Empty;
            public List<MenuEntry> Entries { get; set; } = new();
        }

        private Dictionary<string, MenuDto> RestoMenusStructured = new();
        private string? GeselecteerdeResto;

        private DateTime MondayDate => DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);

        private void OnRestoChanged(ChangeEventArgs e)
        {
            GeselecteerdeResto = e.Value?.ToString();
        }
        protected override async Task OnInitializedAsync()
        {
            var request = new QueryRequest.SkipTake
            {
                Skip = 0,
                Take = 50,
                OrderBy = "Id",
            };

            Restos = (await restoService.GetIndexAsync(request)).Value.Restos.Select(r => r.Name).ToList();
            RestoMenusStructured = (await restoService.GetIndexAsync(request)).Value.Restos
            .GroupBy(r => r.Name)
            .ToDictionary(
                g => g.Key,
                g =>
                {
                    var menu = g.First().Menu;
                    var sortedItems = menu.Items
                        .OrderBy(kvp => weekOrder.IndexOf(kvp.Key))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    menu.Items = sortedItems;

                    return menu;
                }
            );


        }
    }
}