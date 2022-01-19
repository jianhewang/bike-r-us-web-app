using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#region Additional Namespaces
using Backend_Purchasing.BLL;
using Microsoft.AspNetCore.Identity;
using WebApp.Data;
using AppSecurity.BLL;
using Backend_Purchasing.Models;
#endregion

namespace WebApp.Pages.Admin
{
    public class PurchasingModel : PageModel
    {

        #region Constructor and Dependencies

        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly SecurityService _Security;
        private readonly PurchasingServices _purchasingservices;

        public PurchasingModel(UserManager<ApplicationUser> userManager, SecurityService security, PurchasingServices services)
        {
            _UserManager = userManager;
            _Security = security;
            _purchasingservices = services;
        }
        #endregion

        #region Properties
        public ApplicationUser AppUser { get; set; }
        public string EmployeeName { get; set; }

        [BindProperty]
        public int EmployeeID { get; set; }

        public string ErrorMessage { get; set; }
        public List<string> ErrorDetails { get; set; } = new();
        [TempData]
        public string SuccessMessage { get; set; }

        public List<VendorList> AllVendors { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SelectedVendor { get; set; }

        [BindProperty]
        public int SelectedPartID { get; set; }

        public decimal? Subtotal { get; set; }

        public decimal? GST { get; set; }

        public decimal? Total { get; set; }

        public PurchaseOrderSummary PurchaseOrderSummary { get; set; }

        [BindProperty]
        public List<PurchaseOrderItem> OrderDetailList { get; set; }

        [BindProperty]
        public List<PurchaseOrderItem> VendorInventory { get; set; }

        [BindProperty]
        public List<OrderedPart> OrderedParts { get; set; } = new();
        #endregion

        public async Task OnGet()
        {
            AppUser = await _UserManager.FindByNameAsync(User.Identity.Name);
            EmployeeName = _Security.GetEmployeeName(AppUser.EmployeeId.Value);
            EmployeeID = AppUser.EmployeeId.Value;

            AllVendors = _purchasingservices.GetVendorList();

            if (SelectedVendor > 0)
            {
                PurchaseOrderSummary = _purchasingservices.GetOrderInfo(SelectedVendor);

                VendorInventory = _purchasingservices.GetVendorInventory(SelectedVendor);

                // check if there is a current active order
                if (PurchaseOrderSummary.PurchaseOrderNumber > 0)
                {
                    // retrieve current order details
                    OrderDetailList = PurchaseOrderSummary.PurchaseOrderDetails;
                }
                else
                {
                    // generate a suggested order from vendor inventory
                    OrderDetailList = VendorInventory.FindAll(x => (x.ReorderLevel - x.QtyOnHand - x.QtyOnOrder) > 0);
                    // remove suggested items from inventory list
                    VendorInventory.RemoveAll(x => (x.ReorderLevel - x.QtyOnHand - x.QtyOnOrder) > 0);
                }
            }
        }

        public IActionResult OnPostFind()
        {
            return RedirectToPage(new { SelectedVendor = SelectedVendor });
        }

        public void OnPostAddItem()
        {
            var found = VendorInventory.SingleOrDefault(x => x.ID == SelectedPartID);
            if (found != null)
            {
                VendorInventory.Remove(found);
                OrderDetailList.Add(found);
            }

            PopulateOrderInfo();
        }

        public void OnPostDeleteItem()
        {
            var found = OrderDetailList.SingleOrDefault(x => x.ID == SelectedPartID);
            if (found != null)
            {
                OrderDetailList.Remove(found);
                VendorInventory.Add(found);
            }

            PopulateOrderInfo();
        }

        public void OnPostRefreshItem()
        {
            PopulateOrderInfo();

            Subtotal = OrderedParts.Sum(x => x.PurchasePrice * x.QtyToOrder);
            GST = Subtotal * (decimal)0.05;
            Total = Subtotal * (decimal)1.05;
        }

        public IActionResult OnPostSave()
        {
            try
            {
                _purchasingservices.UpdatePurchaseOrder(SelectedVendor, EmployeeID, OrderedParts);
                SuccessMessage = "The order has been updated";

                return RedirectToPage();
            }
            catch (AggregateException ex)
            {
                ErrorMessage = "Unable to process your request:";
                foreach (var problem in ex.InnerExceptions)
                    ErrorDetails.Add(problem.Message);
                PopulateOrderInfo();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = GetInnerException(ex).Message;
                PopulateOrderInfo();
                return Page();
            }
        }

        public IActionResult OnPostNew()
        {
            try
            {
                _purchasingservices.CreatePurchaseOrder(SelectedVendor, EmployeeID, OrderedParts);
                SuccessMessage = "New order has been added";
                return RedirectToPage();
            }
            catch (AggregateException ex)
            {
                ErrorMessage = "Unable to process your request:";
                foreach (var problem in ex.InnerExceptions)
                    ErrorDetails.Add(problem.Message);
                PopulateOrderInfo();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = GetInnerException(ex).Message;
                PopulateOrderInfo();
                return Page();
            }
        }

        public IActionResult OnPostPlace()
        {
            try
            {
                _purchasingservices.PlacePurchaseOrder(SelectedVendor, EmployeeID, OrderedParts);
                SuccessMessage = "The current active order has been placed";
                return RedirectToPage();
            }
            catch (AggregateException ex)
            {
                ErrorMessage = "Unable to process your request:";
                foreach (var problem in ex.InnerExceptions)
                    ErrorDetails.Add(problem.Message);
                PopulateOrderInfo();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = GetInnerException(ex).Message;
                PopulateOrderInfo();
                return Page();
            }
        }

        public IActionResult OnPostDelete()
        {
            try
            {
                _purchasingservices.DeletePurchaseOrder(SelectedVendor, EmployeeID);
                SuccessMessage = "The current active order has been deleted";
                return RedirectToPage(new { SelectedVendor = "" });
            }
            catch (AggregateException ex)
            {
                ErrorMessage = "Unable to process your request:";
                foreach (var problem in ex.InnerExceptions)
                    ErrorDetails.Add(problem.Message);
                PopulateOrderInfo();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = GetInnerException(ex).Message;
                PopulateOrderInfo();
                return Page();
            }
        }

        public IActionResult OnPostClear()
        {
            return RedirectToPage(new { SelectedVendor = "" });
        }

        private void PopulateOrderInfo()
        {
            AllVendors = _purchasingservices.GetVendorList();
            PurchaseOrderSummary = _purchasingservices.GetOrderInfo(SelectedVendor);
        }

        private Exception GetInnerException(Exception ex)
        {
            while (ex.InnerException != null)
                ex = ex.InnerException;
            return ex;
        }
    }
}
