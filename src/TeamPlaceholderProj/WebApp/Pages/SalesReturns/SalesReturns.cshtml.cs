using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppSecurity.BLL;
using Backend_SalesReturns.BLL;
using Backend_SalesReturns.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Data;

namespace WebApp.Pages.Sales
{
    public class SalesReturnsModel : PageModel
    {
        #region CONSTRUCTOR & DEPENDENCIES
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly SecurityService _Security;
        private readonly SalesReturnService _service;

        public SalesReturnsModel(UserManager<ApplicationUser> userManager, SecurityService security, SalesReturnService service)
        {
            _UserManager = userManager;
            _Security = security;
            _service = service;
        }
        #endregion

        #region GET
        #region PROPERTIES
        public ApplicationUser AppUser { get; set; }
        public string EmployeeName { get; set; }
        public ForRefundSalesSummary RefundSalesSummary { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? SalesID { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? ReturnID { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? RefundID { get; set; }
        [BindProperty]
        public int? CategoriesID { get; set; }
        public ReturnSummary ReturnDisplay { get; set; }
        #endregion
        public async Task OnGet(int? SalesID, int? ReturnID, int? RefundID)
        {
            // get logged in user info
            AppUser = await _UserManager.FindByNameAsync(User.Identity.Name);
            EmployeeName = _Security.GetEmployeeName(AppUser.EmployeeId.Value);

            EmployeeID = AppUser.EmployeeId.Value;
            PopulateDropdowns();
            
            // get the sales/refund information to display
            try
            {
                // after refund created
                if (RefundID.HasValue)
                {
                    ReturnDisplay = _service.GetRefundSummary(RefundID.Value);
                    RefundSalesSummary = _service.GetSale(ReturnDisplay.SaleID);
                    ReturnID = null;
                }
                else if (SalesID.HasValue && ReturnID.HasValue)
                {
                    // sales summary for asfter sales post
                    CurrentSaleSummary = _service.GetSalesSummary(SalesID.Value);
                    RefundSalesSummary = _service.GetSale(ReturnID.Value);
                    if (RefundSalesSummary.CouponID is not null)
                        Coupon = _service.GetCoupon(RefundSalesSummary.CouponID.Value);
                } 
                else if (SalesID.HasValue)
                {
                    CurrentSaleSummary = _service.GetSalesSummary(SalesID.Value);
                }
                else if (ReturnID.HasValue)
                {
                    RefundSalesSummary = _service.GetSale(ReturnID.Value);
                    if (RefundSalesSummary.CouponID is not null)
                        Coupon = _service.GetCoupon(RefundSalesSummary.CouponID.Value);
                }
            }
            catch (Exception ex)
            {
                FindSaleMessage = ex.Message;
            }
            
        }
        #endregion

        #region Properties
        // to disable forms
        public bool SalesPosted = false;
        public bool RefundsPosted = false;
        
        // populate dropdowns
        public List<Category> Categories { get; set; }
        public List<PartMenu> Parts { get; set; }
        [BindProperty]
        public int EmployeeID { get; set; }
        // error messages
        public string ItemFeedback { get; set; }
        public string CouponMessage { get; set; }
        public string CheckoutMessage { get; set;  }
        public string FindSaleMessage { get; set; }
        public string RefundMessage { get; set; }
        #region SALES properties
        [BindProperty]
        public SalesSummary CurrentSaleSummary { get; set; }
        [BindProperty]
        public List<Parts> PartsList { get; set; } = new List<Parts>();
        [BindProperty]
        public Parts ToPurchasePart { get; set; }
        [BindProperty]
        public int SalesFocusPart { get; set; }
        [BindProperty]
        public string CouponCode { get; set; }
        [BindProperty]
        public Coupon Coupon { get; set; }
        public bool CanPay { get; set; }
        #endregion
        #region REFUND
        [BindProperty]
        public List<RefundParts> RefundPartsList { get; set; } = new List<RefundParts>();
        #endregion
        #endregion

        #region REFUND POSTS
        public IActionResult OnPostRefund(int ReturnID, int EmployeeID)
        {
            PopulateDropdowns();
            try
            {
                RefundSalesSummary = _service.GetSale(ReturnID);
                RefundID = _service.Refund(RefundPartsList, ReturnID, EmployeeID);
                RefundsPosted = true;
                RefundMessage = "Refund completed.";
                return RedirectToPage( new { RefundID = this.RefundID, SalesID = 0 } );
            }
            catch (Exception ex)
            {
                RefundSalesSummary = _service.GetSale(ReturnID);
                RefundMessage = ex.Message;
                return Page();
            }
        }
        #endregion
        #region SALES POSTS
        // after choosing category
        // repopulate lists (so the right part list is populated)
        public void OnPostGetCategory()
        {
            PopulateDropdowns();
            RefundID = null;
            ItemFeedback = "";
        }
        // add item to cart for purchase
        public void OnPostAddItem()
        {
            PopulateDropdowns();
            RefundID = null;
            if (PartsList.Any(x => x.PartID == ToPurchasePart.PartID))
            {
                try
                {
                    var temp = PartsList.FirstOrDefault(x => x.PartID == ToPurchasePart.PartID);
                    temp.Quantity += ToPurchasePart.Quantity;
                    ItemFeedback = $"SUCCESS - Item is already in cart; " +
                        $"{ToPurchasePart.Quantity} added to {temp.Description}.";
                }
                catch (Exception e)
                {
                    ItemFeedback = e.Message;
                }
            }
            else if (ToPurchasePart is not null)
            {
                try
                {
                    var temp = _service.ToParts(ToPurchasePart.PartID);
                    temp.Quantity = ToPurchasePart.Quantity;
                    ToPurchasePart = temp;
                    PartsList.Add(ToPurchasePart);
                    ItemFeedback = "SUCCESS - Item added";
                }
                catch (Exception e)
                {
                    ItemFeedback = e.Message;
                }
            }
            if (PartsList.Count > 0) CalculateTotals();
            CategoriesID = null;
        }
        // remove item from cart
        
        public void OnPostRemoveItem()
        {
            PopulateDropdowns();
            RefundID = null;
            var temp = PartsList.FirstOrDefault(x => x.PartID == SalesFocusPart);
            if (temp is null)
                ItemFeedback = "ERROR - the item is not in cart";
            else
            {
                PartsList.Remove(temp);
                ItemFeedback = "SUCCESS - item removed from cart";
            }

            CalculateTotals();
        }
        // update new quantity for item
        public void OnPostSalesUpdateQuantity()
        {
            PopulateDropdowns();
            //var temp = PartsList.FirstOrDefault(x => x.PartID == SalesFocusPart);
            //if (temp is null)
            //    ItemFeedback = "ERROR - the item is not in cart";
            //else
            RefundID = null;
            ItemFeedback = "SUCCESS - item quantity updated";
            CalculateTotals();

        }

        public void OnPostGetCoupon()
        {
            PopulateDropdowns();
            RefundID = null;
            try
            {
                Coupon = _service.VerifyCoupon(CouponCode);
                CouponMessage = $"SUCCESS - {Coupon.CouponDiscount}% off";
            }
            catch (Exception e)
            {
                CouponMessage = e.Message;
            }
            CalculateTotals();
        }

        public IActionResult OnPostCheckOut()
        {
            PopulateDropdowns();
            CalculateTotals();
            // validate and update sales
            // show paynow button
            RefundID = null;
            try
            { 
                CurrentSaleSummary.PartsList = PartsList;
                if (CouponCode is not null)
                {
                    Coupon = _service.VerifyCoupon(CouponCode);
                    CurrentSaleSummary.Coupon = Coupon;
                }
                CanPay = _service.CheckoutSales(CurrentSaleSummary);
                return Page();
            }
            // print exeption
            catch (Exception ex)
            {
                CheckoutMessage = ex.Message;
                return Page();
            }
        }

        // add to database
        public IActionResult OnPostPayNow(int EmployeeID)
        {
            RefundID = null;
            try
            {
                PopulateDropdowns();
                // ensure sales object is updated
                CalculateTotals();
                if (CouponCode is not null)
                    CurrentSaleSummary.Coupon = _service.VerifyCoupon (CouponCode);
                CurrentSaleSummary.PartsList = PartsList;
                CurrentSaleSummary.EmployeeID = EmployeeID;

                int newSaleID = _service.PayNow(CurrentSaleSummary);

                return RedirectToPage( new { SalesID = newSaleID });
            }
            catch (Exception ex)
            {
                CheckoutMessage = ex.Message;
                return Page();
            }                
        }


        #endregion
        // clear for clear buttons
        // on both sales and refund
        public IActionResult OnPostClear()
        {
            return RedirectToPage(new { SalesID = (int?)null, RefundID = (int?)null, ReturnID = (int?)null });
        }

        #region HELPERS
        public void PopulateDropdowns()
        {
            PopulateCategories();
            PopulateParts();
            EmployeeName = _Security.GetEmployeeName(EmployeeID);
        }
        private void PopulateCategories()
        {
            Categories = _service.GetCategories();
        }
        private void PopulateParts()
        {            
            Parts = _service.GetParts();
        }
        private void CalculateTotals()
        {
            CurrentSaleSummary.SubTotal = _service.CalculateSubtotal(PartsList);
            CurrentSaleSummary.TaxAmount = _service.CalculateTax(PartsList);
        }
        #endregion

    }
}
