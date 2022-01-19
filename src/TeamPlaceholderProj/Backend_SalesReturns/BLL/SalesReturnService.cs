using Backend_SalesReturns.DAL;
using Backend_SalesReturns.Entities;
using Backend_SalesReturns.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_SalesReturns.BLL
{
    public class SalesReturnService
    {
        #region Dependencies & Constructors
        private readonly SalesReturnsContext _context;
        internal SalesReturnService(SalesReturnsContext context)
        {
            _context = context;
        }
        #endregion

        #region Queries
        #region SALES
        // get categories for drop down
        public List<Models.Category> GetCategories()
        {
            var categories = (from category in _context.Categories
                              select new Models.Category
                              {
                                  CategoryID = category.CategoryID,
                                  Description = category.Description
                              });
            return categories.ToList();
        }

        // get parts for drop down
        public List<PartMenu> GetParts()
        {
            var parts = (from part in _context.Parts
                              select new PartMenu
                              {
                                  PartID = part.PartID,
                                  Description = part.Description,
                                  CategoryID = part.CategoryID,
                                  Discontinued = part.Discontinued
                              });
            return parts.ToList();
        }

        // find part via partID
        public Parts ToParts(int partID)
        {
            var temp = _context.Parts.Find(partID);
            if (temp is not null)
                return new Parts
                {
                    PartID = partID,
                    Description = temp.Description,
                    SellingPrice = temp.SellingPrice
                };
            else
                throw new KeyNotFoundException("Part does not exist - please pick a pick");
        }

        // find coupon through coupon code
        // return Coupon object if valid
        // throws exception with message if not
        public Models.Coupon VerifyCoupon(string couponCode)
        {
            var coupon = _context.Coupons.FirstOrDefault(x => x.CouponIDValue == couponCode);
            // ensure coupon exists
            if (coupon is not null)
            {
                bool dateOkay = DateTime.Today > coupon.StartDate && DateTime.Today < coupon.EndDate;
                // ensure it's used for sale
                // DB strings imply 1 - service & 2 - sales
                if (coupon.SalesOrService == 1 && dateOkay)
                    return new Models.Coupon
                    {
                        CouponID = coupon.CouponID,
                        CouponIDValue = coupon.CouponIDValue,
                        CouponDiscount = coupon.CouponDiscount
                    };
                else if (!dateOkay)
                {
                    throw new ArgumentException("ERROR - coupon cannot be used at this time");
                }
                /*else if (coupon.SalesOrService != 1)
                {
                    throw new ArgumentException("ERROR - wrong coupon type");
                }*/
                else
                {
                    throw new Exception("Unexpected error");
                }
            }
            else
            {
                throw new KeyNotFoundException("ERROR - coupon does not exist");
            }
            
        }

        // get sale to display
        public SalesSummary GetSalesSummary(int salesID)
        {
            var sale = _context.Sales.Find(salesID);
            if (sale is null) 
                throw new ArgumentException($"Sales by {salesID} not found.");
            List<SaleDetail> saleDetailsEntity = _context.SaleDetails
                            .Where(x => x.SaleID == salesID).ToList();
            List<Parts> saleDetailsModels = new();
            foreach (var item in saleDetailsEntity)
            {
                saleDetailsModels.Add(
                    new Parts
                    {
                        Description = _context.Parts.Find(item.PartID).Description,
                        Quantity = item.Quantity,
                        SellingPrice = item.SellingPrice
                    }
                    );
            }

            Models.Coupon couponToShow = null;
            if (sale.CouponID.HasValue)
            {
                Entities.Coupon tempCoupon = _context.Coupons.Find(sale.CouponID.Value);
                couponToShow = new Models.Coupon
                {
                    CouponID = tempCoupon.CouponID,
                    CouponIDValue = tempCoupon.CouponIDValue,
                    CouponDiscount = tempCoupon.CouponDiscount
                };
            }            

            return new SalesSummary
            {
                SaleID = salesID,
                PartsList = saleDetailsModels,
                EmployeeID = sale.EmployeeID,
                TaxAmount = sale.TaxAmount,
                SubTotal = sale.SubTotal,
                PaymentType = sale.PaymentType,
                Coupon = couponToShow
            };
        }
        

        #endregion
        #region RETURNS
        public ForRefundSalesSummary GetSale(int saleID)
        {
            var sales = _context.Sales.Find(saleID);
            if (sales is null)
                throw new ArgumentException("This sale ID does not exist in the system.");
            // all details related to saleID
            ForRefundSalesSummary salesSummary = new ForRefundSalesSummary
            {
                SaleID = saleID,
                CouponID = sales.CouponID,
                PartsList = (from item in _context.Parts
                             where item.SaleDetails.Where(x => x.SaleID == saleID).Any()
                             select new RefundParts
                             {
                                 PartID = item.PartID,
                                 Description = item.Description,
                                 Quantity = (from temp in item.SaleDetails
                                            where temp.SaleID == saleID
                                            select temp.Quantity).Sum(),
                                 SellingPrice = item.SellingPrice,
                                 Refundable = item.Refundable.ToLower() == "y",
                                 RefundQuantity = (from temp in _context.SaleRefundDetails
                                                   where temp.PartID == item.PartID
                                                    && temp.SaleRefund.SaleID == saleID
                                                   select temp.Quantity).Sum(),
                                 SaleDetailID = item.SaleDetails
                                                .Where(x => x.SaleID == saleID && x.PartID == item.PartID)
                                                .First().SaleDetailID
                                 /* RefundReason = string.Join("\n", (from temp in _context.SaleRefundDetails
                                                                   where temp.PartID == item.PartID
                                                                    && _context.SaleRefunds.Where(x => x.SaleID == saleID).Any()
                                                                   select temp.Reason)) */
                             }
            ).ToList()
            };

            return salesSummary;

            //throw new NotImplementedException();
        }
        public Models.Coupon GetCoupon(int couponID)
        {
            var coupon = _context.Coupons.Find(couponID);
            return new Models.Coupon
            {
                CouponID = couponID,
                CouponIDValue = coupon.CouponIDValue,
                CouponDiscount = coupon.CouponDiscount
            };
        }
        // process refund
        public int Refund(List<RefundParts> parts, int saleId, int employeeId)
        {
            // list of errors to throw
            List<Exception> refundErrors = new();

            if (!AnItemIsBeingRefunded(parts))
                refundErrors.Add(new Exception($"Add at least 1 item to refund to process.\n"));

            // business rules
            foreach (var part in parts)
            {
                if (part is not null)
                {
                    string name = _context.Parts.Find(part.PartID).Description;
                    // refundable?
                    if (!ItemRefundable(part) && part.ToRefund > 0)
                        refundErrors.Add(new Exception($"{name} is not refundable. \n"));
                    // eligible quantities to refund
                    if (!ToRefundLessThanBought(part))
                        refundErrors.Add(new Exception($"{name} is being returned more than bought. \n"));
                    // reason with return
                    if (ReturnWithoutReason(part))
                        refundErrors.Add(new Exception($"{name} is being refunded without a reason " +
                            $"or has a reason without being refunded. \n"));
                }
            }

            // throw errors if any
            if (refundErrors.Count > 0) 
                throw new AggregateException("The following issues have been found", refundErrors);

            // save if no errors
            // generate new  SaleRefund record for refund
            SaleRefund newRefund = new SaleRefund
            {
                SaleRefundDate = DateTime.Today,
                SaleID = saleId,
                EmployeeID = employeeId,
                TaxAmount = 0,
                SubTotal = 0
            };
            // generate a SaleRefundDetail for each part returned
            foreach (var part in parts)
            {
                if (part.ToRefund > 0)
                {
                    SaleRefundDetail newPart = new SaleRefundDetail
                    {
                        SaleRefund = newRefund,
                        PartID = part.PartID,
                        Quantity = part.ToRefund,
                        SellingPrice = _context.Parts.Find(part.PartID).SellingPrice,
                        Reason = part.RefundReason
                    };
                    _context.SaleRefundDetails.Add(newPart);
                    newRefund.TaxAmount += (newPart.Quantity * newPart.SellingPrice * 0.05m);
                    newRefund.SubTotal += (newPart.Quantity * newPart.SellingPrice);
                    _context.Parts.Find(part.PartID).QuantityOnHand += part.ToRefund;
                    _context.Entry(_context.Parts.Find(part.PartID)).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
            }


            // save
            _context.Add(newRefund);
            _context.SaveChanges();

            return newRefund.SaleRefundID;
        }
        public bool ItemRefundable(RefundParts part)
        {
            return _context.Parts.Find(part.PartID).Refundable.ToLower() == "y";
        }
        public bool ToRefundLessThanBought(RefundParts part)
        {
            return !((part.RefundQuantity + part.ToRefund) > part.Quantity);
        }
        public bool ReturnWithoutReason(RefundParts part)
        {
            return (part.ToRefund != 0 && string.IsNullOrEmpty(part.RefundReason))
                        || (part.ToRefund == 0 && !string.IsNullOrEmpty(part.RefundReason));
        }
        public bool AnItemIsBeingRefunded(List<RefundParts> parts)
        {
            bool valid = false;
            foreach(var part in parts)
            {
                if (part.ToRefund > 0)
                    valid = true;
            }
            return valid;
        }
        // for display after refund has been processed
        public ReturnSummary GetRefundSummary(int refundId)
        {
            //throws exception if refund not found
            var returnRaw = _context.SaleRefunds.Find(refundId);
            if (returnRaw is null) throw new Exception("This refund does not exist.");
            var refundDetails = _context.SaleRefundDetails.Where(x => x.SaleRefundID == refundId);
            var sale = _context.Sales.Where(x => x.SaleID == returnRaw.SaleID).First();
            var saleDetails = _context.SaleDetails.Where(x => x.SaleID == returnRaw.SaleID);
            // create object to display
            ReturnSummary returnDisplay = new ReturnSummary
            {
                SaleRefundID = returnRaw.SaleRefundID,
                SaleID = returnRaw.SaleID,
                PartsReturned = (from item in refundDetails
                                 select new RefundParts
                                {
                                     PartID = item.PartID,
                                    Description = item.Part.Description,
                                    Quantity = saleDetails
                                                .Where(x => x.PartID == item.PartID)
                                                .Select(x => x.Quantity).FirstOrDefault(),
                                    SellingPrice = item.SellingPrice,
                                    RefundQuantity = refundDetails
                                                        .Where(x => x.PartID == item.PartID)
                                                        .Select(x => x.Quantity).Sum(),
                                    Refundable = item.Part.Refundable.ToLower() == "y",
                                    ToRefund = item.Quantity,
                                    RefundReason = item.Reason
                                }).ToList(),
                TaxAmount = returnRaw.TaxAmount,
                SubTotal = returnRaw.SubTotal,
                OriginalCoupon = 
                sale.CouponID is null ? null :
                    new Models.Coupon
                    {
                        CouponID = sale.CouponID.Value,
                        CouponIDValue = _context.Coupons.Find(sale.CouponID.Value).CouponIDValue,
                        CouponDiscount = _context.Coupons.Find(sale.CouponID.Value).CouponDiscount
                    }
            };
            return returnDisplay;
        }
        #endregion
        #endregion

        #region Commands
        #region SALES
        // ensure sales is valid for checkout
        public bool CheckoutSales(SalesSummary sale)
        {
            // for all errors with sales
            List<Exception> saleIssues = new();
            // recalculate totals
            if (sale.SubTotal != CalculateSubtotal(sale.PartsList))
                saleIssues.Add(new Exception("Issue occurred when finding subtotals. "));
            if (sale.TaxAmount != CalculateTax(sale.PartsList))
                saleIssues.Add(new Exception("Issue occurred when finding taxes. "));
            if (sale.PaymentType is null)
                saleIssues.Add(new Exception("Please select a method to pay. "));
            
            // true if all validation met
            // throw aggregate exception otherwise
            return saleIssues.Count == 0 ?  true 
                : throw new AggregateException(saleIssues);            
        }

        // saves sales & items
        // updates inventory totals
        public int PayNow(SalesSummary sale)
        {
            // List<Exception> errors = new();
            // ensure inventory has stock for order
            // throws exception if not
            IsThereEnoughStock(sale.PartsList);

            // create Sale from sale
            var saleToSave = new Entities.Sale
            {
                SaleDate = DateTime.Today,
                EmployeeID = sale.EmployeeID,
                TaxAmount = sale.TaxAmount,
                SubTotal = sale.SubTotal,
                CouponID = sale.Coupon is null ? null 
                            : sale.Coupon.CouponID,
                PaymentType = sale.PaymentType
            };
            // create SaleDetail for each item
            List<SaleDetail> saleDetails = new();
            foreach(var item in sale.PartsList)
            {
                saleDetails.Add(
                    new SaleDetail
                    {
                        Sale = saleToSave,
                        PartID = item.PartID,
                        Quantity = item.Quantity,
                        SellingPrice = item.SellingPrice
                    }
                );
            }
            // save sales, sale items (each)
            // update inventory sales
            _context.Sales.Add(saleToSave);
            foreach(var item in saleDetails)
            {
                _context.SaleDetails.Add(item);
                _context.Parts.Find(item.PartID).QuantityOnHand -= item.Quantity;
            }
            // save DB
            _context.SaveChanges();
            return saleToSave.SaleID;
        }

        // business rules
        // ensure stock match sales
        public void IsThereEnoughStock(List<Parts> list)
        {
            int stock;
            string amountStock = "";
            foreach (var item in list)
            {
                stock = _context.Parts.Find(item.PartID).QuantityOnHand;
                if (item.Quantity > stock)
                    amountStock += $"{item.Description} has {stock} left.\n";
            }
            if (amountStock != "")
                throw new ArgumentOutOfRangeException(amountStock);
        }

        #endregion
        #endregion

        #region Little Helpers
        // calculate subtotal based on a <Parts> list
        public decimal CalculateSubtotal(List<Parts> list)
        {
            decimal subtotal = 0;
            foreach (var item in list)
            {
                subtotal += (item.Quantity * item.SellingPrice);
            }
            return subtotal;
        }

        // calculate taxes based on a <Parts> list
        public decimal CalculateTax(List<Parts> list)
        {
            decimal taxes = 0;
            try
            {
                foreach (var item in list)
                {
                    taxes +=  (0.05m * (item.SellingPrice * item.Quantity));
                }
                return taxes;
            }
            catch
            {
                throw new ArgumentException("An issue occurred with calculating the taxes. ");
            }
        }

        #endregion
    }
}
