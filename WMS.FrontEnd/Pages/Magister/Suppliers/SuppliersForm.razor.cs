using Blazored.Modal.Services;
using Blazored.Modal;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using WMS.FrontEnd.Repositories;
using WMS.FrontEnd.Shared;
using WMS.Share.DTOs;
using WMS.Share.Models.Magister;

namespace WMS.FrontEnd.Pages.Magister.Suppliers
{
    public partial class SuppliersForm
    {
        private EditContext editContext = null!;
        private bool loading;

        [EditorRequired, Parameter]
        public Supplier Model { get; set; } = null!;

        [EditorRequired, Parameter]
        public EventCallback OnValidSubmit { get; set; }

        [EditorRequired, Parameter]
        public EventCallback ReturnAction { get; set; }

        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        [CascadingParameter]
        IModalService Modal { get; set; } = default!;

        public bool FormPostedSuccessfully { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        //protected override void OnInitialized()
        //{
        //    editContext = new(Model);
        //}

        protected override async Task OnParametersSetAsync()
        {
            editContext = new(Model);
            if (Model.CompanyName != null)
            {
                Name = Model.CompanyName;
                Description = " - " + Model.Address;
            }
        }

        private async Task OnDataAnnotationsValidatedAsync()
        {
            await OnValidSubmit.InvokeAsync();
        }

        private async Task SearchDocument()
        {
            IModalReference modalReference;
            var parameters = new ModalParameters();
            parameters.Add("Label", "Sucursal");
            parameters.Add("Url", "api/branches/genericsearch");

            var modalOptions = new ModalOptions();
            modalOptions.HideCloseButton = false;
            modalOptions.DisableBackgroundCancel = false;
            modalReference = Modal.Show<SearchForm>(string.Empty, parameters, modalOptions);



            var result = await modalReference.Result;
            if (result.Confirmed)
            {
                var ItemSelect = (GenericSearchDTO)result.Data!;
                Name = ItemSelect.Name;
                Description = " - " + ItemSelect.Description;
                Model.DocumentTypeUserId = ItemSelect.Id;
            }
            return;
        }


        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            var formWasEdited = editContext.IsModified();
            if (!formWasEdited || FormPostedSuccessfully)
            {
                return;
            }

            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = "¿Deseas abandonar la página y perder los cambios?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true,
            });
            var confirm = !string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }
            context.PreventNavigation();
        }

        private async Task HandleFileChange(InputFileChangeEventArgs e, int index)
        {
            var file = e.File;
            switch (index)
            {
                case 1:
                    Model.Attachment1Name = file.Name;
                    Model.Attachment1 = await GetFileBase64(file); break;
                case 2:
                    Model.Attachment2Name = file.Name;
                    Model.Attachment2 = await GetFileBase64(file); break;
                case 3:
                    Model.Attachment3Name = file.Name;
                    Model.Attachment3 = await GetFileBase64(file); break;
                case 4:
                    Model.Attachment4Name = file.Name;
                    Model.Attachment4 = await GetFileBase64(file); break;
                case 5:
                    Model.Attachment5Name = file.Name;
                    Model.Attachment5 = await GetFileBase64(file); break;
            }
        }

        private async Task<string> GetFileBase64(IBrowserFile file)
        {
            var stream = file.OpenReadStream();
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            string base64 = Convert.ToBase64String(memoryStream.ToArray());
            return base64;
        }
    }
}
