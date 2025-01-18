using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using WMS.FrontEnd.Pages.Location.Wineries;
using WMS.FrontEnd.Repositories;
using WMS.Share.Models.Magister;

namespace WMS.FrontEnd.Pages.Magister.Suppliers
{
    [Authorize]
    public partial class SuppliersCreate
    {
        private Supplier Model = new();
        public SuppliersForm? form;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

        protected override void OnInitialized()
        {
            if (Model == null)
            {
                Model = new Supplier();
            }
        }
        private async Task CreateAsync()
        {
            var httpResponse = await Repository.PostAsync("/api/Supplier", Model);
            if (httpResponse.Error)
            {
                var message = await httpResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            NavigationManager.NavigateTo("/suppliers");
        }

        private void Return()
        {
            form!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo($"/suppliers");
        }
    }
}
