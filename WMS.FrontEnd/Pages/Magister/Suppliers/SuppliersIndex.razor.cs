using Blazored.Modal.Services;
using Blazored.Modal;
using ClosedXML.Excel;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net;
using WMS.FrontEnd.Pages.Location.Wineries;
using WMS.FrontEnd.Repositories;
using WMS.FrontEnd.Shared;
using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Models.Magister;

namespace WMS.FrontEnd.Pages.Magister.Suppliers
{
    public partial class SuppliersIndex
    {
        private int currentPage = 1;
        private int totalPages;

        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [Parameter, SupplyParameterFromQuery] public string Page { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

        public List<Supplier>? MyList { get; set; }

        [CascadingParameter]
        IModalService Modal { get; set; } = default!;
        public long? BranchId { get; set; }
        public string? NameBranch { get; set; }
        public string? DescriptionBranch { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task SelectedPageAsync(int page)
        {
            if (!string.IsNullOrWhiteSpace(Page))
            {
                page = Convert.ToInt32(Page);
            }

            currentPage = page;
            await LoadAsync(page);
        }

        private async Task LoadAsync(int page = 1)
        {
            var ok = await LoadListAsync(page);
            if (ok)
            {
                await LoadPagesAsync();
            }
        }

        private async Task<bool> LoadListAsync(int page)
        {
            var url = $"api/Supplier/getasync?page={page}";
            if (!string.IsNullOrEmpty(Filter))
            {
                url += $"&filter={Filter}";
            }

            var responseHttp = await Repository.GetAsync<List<Supplier>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return false;
            }
            MyList = responseHttp.Response;
            return true;
        }

        private async Task LoadPagesAsync()
        {
            var url = $"api/supplier/totalPages";
            string FilterUrl = string.Empty;
            if (!String.IsNullOrEmpty(Filter))
            {
                FilterUrl += $"?filter={Filter}";
            }
            if (BranchId != null && BranchId != 0)
            {
                if (!string.IsNullOrEmpty(FilterUrl))
                {
                    FilterUrl += $"&filter1={BranchId}";
                }
                else
                {
                    FilterUrl += $"?filter1={BranchId}";
                }
            }
            if (!string.IsNullOrEmpty(FilterUrl))
            {
                url += FilterUrl;
            }
            var responseHttp = await Repository.GetAsync<int>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            totalPages = responseHttp.Response;
        }

        private async Task CleanFilterAsync()
        {
            Filter = string.Empty;
            await ApplyFilterAsync();
        }

        private async Task ApplyFilterAsync()
        {
            int page = 1;
            await LoadAsync(page);
            await SelectedPageAsync(page);
        }

        private async Task DeleteAsycn(Supplier model)
        {
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = $"¿Estas seguro de eliminar : {model.FirstName}?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true,
            });
            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            var responseHttp = await Repository.DeleteAsync<Supplier>($"api/supplier/deleteasync/{model.Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/supliers");
                }
                else
                {
                    var mensajeError = await responseHttp.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", mensajeError, SweetAlertIcon.Error);
                }
                return;
            }

            await LoadAsync();
            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer = 3000
            });
            await toast.FireAsync(icon: SweetAlertIcon.Success, message: "Registro eliminado con éxito.");
        }

        private async Task Export()
        {
            var url = $"api/suplier/downloadasync";
            string FilterUrl = string.Empty;
            if (!String.IsNullOrEmpty(Filter))
            {
                FilterUrl += $"?filter={Filter}";
            }
            if (BranchId != 0)
            {
                if (!string.IsNullOrEmpty(FilterUrl))
                {
                    FilterUrl += $"&filter1={BranchId}";
                }
                else
                {
                    FilterUrl += $"?filter1={BranchId}";
                }
            }
            if (!string.IsNullOrEmpty(FilterUrl))
            {
                url += FilterUrl;
            }

            var responseHttp = await Repository.GetAsync<List<Supplier>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            var ListDownload = responseHttp.Response;

            using (var book = new XLWorkbook())
            {
                IXLWorksheet sheet = book.Worksheets.Add("FormatoProveedores");
                sheet.Cell(1, 1).Value = "Nombre de la compañia";
                sheet.Cell(1, 2).Value = "Nombres";
                sheet.Cell(1, 3).Value = "Apellidos";
                sheet.Cell(1, 4).Value = "Dirección";
                sheet.Cell(1, 5).Value = "Teléfono";
                sheet.Cell(1, 6).Value = "Correo";

                if (ListDownload == null || ListDownload.Count == 0)
                {
                    sheet.Cell(2, 1).Value = 0;
                    sheet.Cell(2, 2).Value = "person1";
                    sheet.Cell(2, 3).Value = "lastname";
                    sheet.Cell(2, 4).Value = "carrera 20 sur";
                    sheet.Cell(2, 5).Value = 1;
                    sheet.Cell(2, 6).Value = 0;
                }
                else
                {
                    int i = 2;
                    foreach (var item in ListDownload)
                    {
                        sheet.Cell(i, 1).Value = item.CompanyName;
                        sheet.Cell(i, 2).Value = item.FirstName;
                        sheet.Cell(i, 3).Value = item.LastName;
                        sheet.Cell(i, 4).Value = item.Address;
                        sheet.Cell(i, 5).Value = item.Phone;
                        sheet.Cell(i, 6).Value = item.Email;
                        i++;
                    }
                }

                using (var memory = new MemoryStream())
                {
                    book.SaveAs(memory);
                    await JSRuntime.InvokeAsync<object>(
                            "DownloadExcel",
                            $"{DateTime.Now.ToString("yyyyMMdd")}_Proveedores.xlsx",
                            Convert.ToBase64String(memory.ToArray())
                    );
                }
            }
        }

        private async Task ShowModal(Supplier model)
        {
            IModalReference modalReference;
            var parameters = new ModalParameters();
            parameters.Add("Id", model.Id);
            ModalOptions mo = new ModalOptions
            {
                HideCloseButton = false,
                HideHeader = false,
                DisableBackgroundCancel = false,

            };
            modalReference = Modal.Show<SuppliersClock>(model.CompanyName, parameters, mo);


            var result = await modalReference.Result;
            //if (result.Confirmed)
            //{
            //    await LoadAsync();
            //}
        }

        //private async Task SearchBranch()
        //{
        //    IModalReference modalReference;
        //    var parameters = new ModalParameters();
        //    parameters.Add("Label", "Sucursal");
        //    parameters.Add("Url", "api/branches/genericsearch");

        //    var modalOptions = new ModalOptions();
        //    modalOptions.HideCloseButton = false;
        //    modalOptions.DisableBackgroundCancel = false;
        //    modalReference = Modal.Show<SearchForm>(string.Empty, parameters, modalOptions);

        //    var result = await modalReference.Result;
        //    if (result.Confirmed)
        //    {
        //        var ItemSelect = (GenericSearchDTO)result.Data!;
        //        NameBranch = ItemSelect.Name;
        //        DescriptionBranch = " - " + ItemSelect.Description;
        //        BranchId = ItemSelect.Id;
        //    }
        //    return;
        //}
    }
}
