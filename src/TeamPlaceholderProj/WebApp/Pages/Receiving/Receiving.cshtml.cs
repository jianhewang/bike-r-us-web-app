using AppSecurity.BLL;
using Backend_Receiving.BLL;
using Backend_Receiving.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Data;

namespace WebApp.Pages.Receiving
{
    public class ReceivingModel : PageModel
    {
        private readonly ReceivingService _service;
        private readonly SecurityService _security;
        private readonly UserManager<ApplicationUser> _userManager;
        public ReceivingModel(ReceivingService service, SecurityService security, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _security = security;
            _userManager = userManager;
        }

        [TempData]
        public string FeedbackMessage { get; set; }
        public string WarningMessage { get; set; }
        public List<string> WarningDetails { get; set; } = new();

        public List<OutstandingOrder> OutstandingOrders { get; set; }
        [BindProperty(SupportsGet =true)]
        public int? PurchaseOrderId { get; set; }
        [BindProperty]
        public ReceivingDetails SelectedOrder { get; set; }

        public ApplicationUser AppUser { get; set; }
        public string EmployeeName { get; set; }
        [BindProperty]
        public int EmployeeId { get; set; }

        [BindProperty]
        public List<CurrentReceiving> CurrentReceivings { get; set; }
        [BindProperty]
        public List<UnOrderedItem> UnOrderedItems { get; set; } = new List<UnOrderedItem>();
        [BindProperty]
        public string ItemToRemove { get; set; }
        [BindProperty]
        public UnOrderedItem NewlyInsertItem { get; set; }
        [BindProperty]
        public string CloseReason { get; set; }



        public async Task OnGet()
        {
            AppUser = await _userManager.FindByNameAsync(User.Identity.Name);
            EmployeeName = _security.GetEmployeeName(AppUser.EmployeeId.Value);
            EmployeeId = AppUser.EmployeeId.Value;

            OutstandingOrders = _service.ListOutstandingOrders();
            if(PurchaseOrderId.HasValue)
            {
                SelectedOrder = _service.GetReceivingDetials(PurchaseOrderId.Value);
            }
                
        }



        public IActionResult OnPostReceive()
        {
            try
            {
                if (PurchaseOrderId.HasValue)
                    _service.ProcessReceiving(PurchaseOrderId.Value, EmployeeId, CurrentReceivings, UnOrderedItems);

                FeedbackMessage = $"Purchase Order {PurchaseOrderId.HasValue} receiving status updated!";
                return RedirectToPage(new { PurchaseOrderId = "" });
            }
            catch (AggregateException ex)
            {
                WarningMessage = "Unable to process your request:";
                foreach (var problem in ex.InnerExceptions)
                    WarningDetails.Add(problem.Message);
                PopulateForm();
                return Page();
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null)
                    inner = inner.InnerException;
                WarningMessage = inner.Message;
                PopulateForm();
                return Page();
            }
        }

        public IActionResult OnPostClose()
        {
            try
            {
                if (PurchaseOrderId.HasValue)
                    _service.ProcessForceClose(PurchaseOrderId.Value, CloseReason);

                FeedbackMessage = $"Purchase Order {PurchaseOrderId.HasValue} has been force close!";
                return RedirectToPage(new { PurchaseOrderId = "" });
            }
            catch (AggregateException ex)
            {
                WarningMessage = "Unable to process your request:";
                foreach (var problem in ex.InnerExceptions)
                    WarningDetails.Add(problem.Message);
                PopulateForm();
                return Page();
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null)
                    inner = inner.InnerException;
                WarningMessage = inner.Message;
                PopulateForm();
                return Page();
            }
        }

        public void OnPostAddItem()
        {
            PopulateForm();
            if (UnOrderedItems.Any(item => item.VendorPartId == NewlyInsertItem.VendorPartId))
                WarningMessage = "You cannot add duplicate item";
            else
                UnOrderedItems.Add(NewlyInsertItem);
        }

        public void OnPostClearItem()
        {
            PopulateForm();
            UnOrderedItems = new List<UnOrderedItem>();
        }

        public void OnPostRemoveItem()
        {
            PopulateForm();
            var item = UnOrderedItems.FirstOrDefault(item => item.VendorPartId == ItemToRemove);
            if (item != null)
                UnOrderedItems.Remove(item);
            else
                WarningMessage = $"Hmm. For some reason, {ItemToRemove} is already gone.";
        }

        private Exception GetInnerException(Exception ex)
        {
            while (ex.InnerException != null)
                ex = ex.InnerException;
            return ex;
        }

        private void PopulateForm()
        {
            OutstandingOrders = _service.ListOutstandingOrders();
            if (PurchaseOrderId.HasValue)
            {
                SelectedOrder = _service.GetReceivingDetials(PurchaseOrderId.Value);
            }
        }
    }
}
