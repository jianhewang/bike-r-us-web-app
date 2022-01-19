using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using Backend_Purchasing.Models;
using Backend_Purchasing.DAL;
using Backend_Purchasing.Entities;
#endregion

namespace Backend_Purchasing.BLL
{
    public class PurchasingServices
    {
        #region Constructor and DI variable setup
        private readonly PurchasingContext _context;

        internal PurchasingServices(PurchasingContext context)
        {
            _context = context;
        }
        #endregion

        #region Query
        /// <summary>
        /// Retrieve all vendors from Vendors Table
        /// </summary>
        /// <returns>A list of vendors by VendorList model</returns>
        public List<VendorList> GetVendorList()
        {
            var result = _context.Vendors
                        .OrderBy(x => x.VendorName)
                        .Select(x => new VendorList
                        {
                            VendorID = x.VendorId,
                            VendorName = x.VendorName
                        });
            return result.ToList();
        }

        /// <summary>
        /// Get order summary information as well as the order details
        /// </summary>
        /// <param name="vendorId">To select a specific vendor</param>
        /// <returns>Return an object by PurchaseOrderSummary model including order summary and details</returns>
        public PurchaseOrderSummary GetOrderInfo(int vendorId)
        {
            var exist = _context.PurchaseOrders
                            .Where(x => x.VendorId == vendorId && x.OrderDate == null)
                            .Select(x => x.PurchaseOrderId).FirstOrDefault();

            if (exist != 0)
            {
                // retrieve order summary with item details
                var orderInfo = _context.PurchaseOrders
                                .Where(x => x.VendorId == vendorId && x.OrderDate == null)
                                .Select(x => new PurchaseOrderSummary
                                {
                                    Phone = x.Vendor.Phone,
                                    City = x.Vendor.City,
                                    PurchaseOrderNumber = x.PurchaseOrderNumber,
                                    SubTotal = x.SubTotal,
                                    TaxAmount = x.TaxAmount,
                                    PurchaseOrderDetails = x.PurchaseOrderDetails
                                                            .Select(o => new PurchaseOrderItem
                                                            {
                                                                PurchaseOrderDetailID = o.PurchaseOrderDetailId,
                                                                ID = o.PartId,
                                                                Description = o.Part.Description,
                                                                QtyOnHand = o.Part.QuantityOnHand,
                                                                ReorderLevel = o.Part.ReorderLevel,
                                                                QtyOnOrder = o.Part.QuantityOnOrder,
                                                                QtyToOrder = o.Quantity,
                                                                PurchasePrice = o.PurchasePrice
                                                            }).ToList()
                                }).FirstOrDefault();
                return orderInfo;
            }
            else
            {
                // create a new order object
                var vendor = _context.Vendors
                                .Where(x => x.VendorId == vendorId)
                                .Select(x => new
                                {
                                    Phone = x.Phone,
                                    City = x.City,
                                }).FirstOrDefault();

                PurchaseOrderSummary newOrder = new PurchaseOrderSummary(vendor.Phone, vendor.City, 0, 0, 0, null);

                return newOrder;
            }
        }

        /// <summary>
        /// Get parts inventory from a vendor on unselected items
        /// </summary>
        /// <param name="vendorId">To select a specific vendor</param>
        /// <returns>Return a list of PurchaseOrderItem model including unselected parts details</returns>
        public List<PurchaseOrderItem> GetVendorInventory(int vendorId)
        {
            // select all part IDs that not listed in order from the Parts table for a selected vendor
            var parts = (_context.Parts
                            .Where(x => x.VendorId == vendorId)
                            .OrderBy(x => x.Description)
                            .Select(x => x.PartId))
                            .Except(_context.PurchaseOrderDetails
                                        .Where(x => x.PurchaseOrder.VendorId == vendorId && x.PurchaseOrder.OrderDate == null)
                                        .Select(x => x.PartId)).ToList();
            // create a list to store all part info objects
            List<PurchaseOrderItem> inventory = new List<PurchaseOrderItem>();

            // retrieve the part info with the list of IDs
            foreach (var id in parts)
            {
                var part = _context.Parts
                            .Where(x => x.PartId == id)
                            .Select(x => new PurchaseOrderItem
                            {
                                ID = x.PartId,
                                Description = x.Description,
                                QtyOnHand = x.QuantityOnHand,
                                ReorderLevel = x.ReorderLevel,
                                QtyOnOrder = x.QuantityOnOrder,
                                PurchasePrice = x.PurchasePrice
                            }).SingleOrDefault();
                inventory.Add(part);
            }

            return inventory;
        }
        #endregion

        #region Command
        #region Private Helper Methods
        private void CheckValidVendor(List<Exception> errors, int vendorID, List<OrderedPart> orderedParts)
        {
            var validVendor = _context.Vendors.Find(vendorID);
            if (validVendor == null)
            {
                errors.Add(new Exception("Vendor does not exist"));
            }
        }

        private void CheckValidEmployee(List<Exception> errors, int employeeID, List<OrderedPart> orderedParts)
        {
            var validEmployee = _context.Employees.Find(employeeID);
            if (validEmployee == null)
            {
                errors.Add(new Exception("Employee does not exist"));
            }
        }

        private static void CheckValidOrderList(List<Exception> errors, List<OrderedPart> orderedParts)
        {
            if (!orderedParts.Any())
            {
                errors.Add(new Exception("Order cannot be empty. Please select and add parts to create an order"));
            }
        }

        private void CheckValidCurrentOrderExist(List<Exception> errors, int vendorID)
        {
            var order = _context.PurchaseOrders
                        .Where(x => x.VendorId == vendorID && x.OrderDate == null)
                        .Select(x => x).SingleOrDefault();

            if (order == null)
            {
                errors.Add(new Exception($"There is no Current Active Order for the selected vendor '{_context.Vendors.Find(vendorID).VendorName}'"));
            }
        }

        private void CheckValidCurrentOrderNumber(List<Exception> errors, int vendorID)
        {
            var order = _context.PurchaseOrders
                        .Where(x => x.VendorId == vendorID && x.OrderDate == null)
                        .Select(x => x).SingleOrDefault();

            if (order != null)
            {
                errors.Add(new Exception($"There is already one Current Active Order for the selected vendor '{_context.Vendors.Find(vendorID).VendorName}'"));
            }
        }

        public void CheckValidQtyAndPrice(List<Exception> errors, List<OrderedPart> orderedParts)
        {
            foreach (var part in orderedParts)
            {
                if (part.QtyToOrder <= 0)
                {
                    errors.Add(new Exception($"Quantity To Order value must be greater than 0 for part: ({part.PartID}) {_context.Parts.Find(part.PartID).Description} "));
                }

                if (part.PurchasePrice <= 0)
                {
                    errors.Add(new Exception($"Purchase Price value must be greater than 0 for part: ({part.PartID}) {_context.Parts.Find(part.PartID).Description} "));
                }
            }
        }

        #endregion
        public void CreatePurchaseOrder(int vendorID, int employeeID, List<OrderedPart> orderedParts)
        {
            // Create an empty list of problems with the data
            List<Exception> errors = new();

            // must have a vendor to create new order
            CheckValidVendor(errors, vendorID, orderedParts);

            // must have a employee to create new order
            CheckValidEmployee(errors, employeeID, orderedParts);

            // must have parts info to create new order
            CheckValidOrderList(errors, orderedParts);

            // cannot create more than one current active order at a time
            CheckValidCurrentOrderNumber(errors, vendorID);

            // quantity to order and purchase price must be greater than zero
            CheckValidQtyAndPrice(errors, orderedParts);

            // throw exceptions if there are any errors
            if (errors.Any())
            {
                throw new AggregateException("Unable to process your request: ", errors);
            }

            var newOrder = new PurchaseOrder
            {
                PurchaseOrderNumber = (_context.PurchaseOrders.Max(x => x.PurchaseOrderNumber) + 1),
                OrderDate = null,
                TaxAmount = orderedParts.Sum(x => x.QtyToOrder * x.PurchasePrice) * (decimal)0.05,
                SubTotal = orderedParts.Sum(x => x.QtyToOrder * x.PurchasePrice),
                Closed = false,
                Notes = null,
                EmployeeId = employeeID,
                VendorId = vendorID
            };

            foreach (var part in orderedParts)
            {
                var details = new PurchaseOrderDetail
                {
                    PurchaseOrder = newOrder,
                    PartId = part.PartID,
                    Quantity = part.QtyToOrder,
                    PurchasePrice = part.PurchasePrice
                };

                _context.PurchaseOrderDetails.Add(details);
            }

            _context.PurchaseOrders.Add(newOrder);

            _context.SaveChanges();
        }

        public void UpdatePurchaseOrder(int vendorID, int employeeID, List<OrderedPart> orderedParts)
            {
            // Create an empty list of problems with the data
            List<Exception> errors = new();

            // must have a vendor to update current order
            CheckValidVendor(errors, vendorID, orderedParts);

            // must have a employee to update current order
            CheckValidEmployee(errors, employeeID, orderedParts);

            // must have parts info to update current order
            CheckValidOrderList(errors, orderedParts);

            // quantity to order and purchase price must be greater than zero
            CheckValidQtyAndPrice(errors, orderedParts);

            // a current order must exist in the table for the selected vendor
            CheckValidCurrentOrderExist(errors, vendorID);

            // throw exceptions if there are any errors
            if (errors.Any())
            {
                throw new AggregateException("Unable to process your request: ", errors);
            }

            // update order detail changes
            var orderID = _context.PurchaseOrders
                            .Where(x => x.OrderDate == null && x.VendorId == vendorID)
                            .Select(x => x.PurchaseOrderId).SingleOrDefault();

            if (orderID != 0)
            {
                decimal subTotal = 0;

                foreach (var part in orderedParts)
                {
                    if (part.PurchaseOrderDetailID != 0)
                    {
                        // update existed records
                        var record = _context.PurchaseOrderDetails.Find(part.PurchaseOrderDetailID);
                        record.Quantity = part.QtyToOrder;
                        record.PurchasePrice = part.PurchasePrice;
                    }

                    if (part.PurchaseOrderDetailID == 0)
                    {
                        // insert the new record
                        _context.PurchaseOrderDetails.Add(new PurchaseOrderDetail
                        {
                            PurchaseOrderId = orderID,
                            PartId = part.PartID,
                            Quantity = part.QtyToOrder,
                            PurchasePrice = part.PurchasePrice
                        });

                    }

                    subTotal += part.QtyToOrder * part.PurchasePrice;
                }

                var order = _context.PurchaseOrders.Find(orderID);
                order.SubTotal = subTotal;
                order.TaxAmount = subTotal * (decimal)0.05;

                _context.SaveChanges();
            }

        }

        public void PlacePurchaseOrder(int vendorID, int employeeID, List<OrderedPart> orderedParts)
        {
            // Create an empty list of problems with the data
            List<Exception> errors = new();

            // must have a vendor to update current order
            CheckValidVendor(errors, vendorID, orderedParts);

            // must have a employee to update current order
            CheckValidEmployee(errors, employeeID, orderedParts);

            // must have parts info to update current order
            CheckValidOrderList(errors, orderedParts);

            // quantity to order and purchase price must be greater than zero
            CheckValidQtyAndPrice(errors, orderedParts);

            // throw exceptions if there are any errors
            if (errors.Any())
            {
                throw new AggregateException("Unable to process your request: ", errors);
            }
                
            
            var orderID = _context.PurchaseOrders
                            .Where(x => x.OrderDate == null && x.VendorId == vendorID)
                            .Select(x => x.PurchaseOrderId).SingleOrDefault();

            // check whether the order exists in the table or not
            if (orderID == 0)
            {
                // insert new records if the order is from a suggeted one
                var newOrder = new PurchaseOrder
                {
                    PurchaseOrderNumber = (_context.PurchaseOrders.Max(x => x.PurchaseOrderNumber) + 1),
                    OrderDate = DateTime.Now,
                    TaxAmount = orderedParts.Sum(x => x.QtyToOrder * x.PurchasePrice) * (decimal)0.05,
                    SubTotal = orderedParts.Sum(x => x.QtyToOrder * x.PurchasePrice),
                    Closed = false,
                    Notes = null,
                    EmployeeId = employeeID,
                    VendorId = vendorID
                };
                foreach (var part in orderedParts)
                {
                    var details = new PurchaseOrderDetail
                    {
                        PurchaseOrder = newOrder,
                        PartId = part.PartID,
                        Quantity = part.QtyToOrder,
                        PurchasePrice = part.PurchasePrice
                    };

                    _context.PurchaseOrderDetails.Add(details);
                }

                _context.PurchaseOrders.Add(newOrder);
            }
            else
            {
                // update (and insert) records if the order is from a current active one
                decimal subTotal = 0;

                foreach (var part in orderedParts)
                {
                    if (part.PurchaseOrderDetailID != 0)
                    {
                        // update existed records
                        var record = _context.PurchaseOrderDetails.Find(part.PurchaseOrderDetailID);
                        record.Quantity = part.QtyToOrder;
                        record.PurchasePrice = part.PurchasePrice;
                    }

                    if (part.PurchaseOrderDetailID == 0)
                    {
                        // insert the new record
                        _context.PurchaseOrderDetails.Add(new PurchaseOrderDetail
                        {
                            PurchaseOrderId = orderID,
                            PartId = part.PartID,
                            Quantity = part.QtyToOrder,
                            PurchasePrice = part.PurchasePrice
                        });

                    }

                    subTotal += part.QtyToOrder * part.PurchasePrice;
                }

                var order = _context.PurchaseOrders.Find(orderID);
                order.SubTotal = subTotal;
                order.TaxAmount = subTotal * (decimal)0.05;

                order.OrderDate = DateTime.Now;
            }

            // update Parts table on column QtyOnOrder
            foreach(var part in orderedParts)
            {
                var partRecord = _context.Parts.Find(part.PartID);

                partRecord.QuantityOnOrder = part.QtyToOrder + partRecord.QuantityOnOrder;
            }

            // commit the change at the end for the entire transaction
            _context.SaveChanges();
        }

        public void DeletePurchaseOrder(int vendorID, int employeeID)
        {
            // rules: 
            // Can only delete current active order that not yet been placed 
            //      delete all records in order and order detail table
            var order = _context.PurchaseOrders
                            .Where(x => x.OrderDate == null && x.VendorId == vendorID && x.EmployeeId == employeeID)
                            .Select(x => x).SingleOrDefault();
            List<Exception> errors = new List<Exception>();

            if (order == null)
            {
                errors.Add(new Exception("Order does not exist"));
                throw new AggregateException(errors);
            }
            else
            {
                var orderDetailsToDelete = _context.PurchaseOrderDetails
                                            .Where(x => x.PurchaseOrderId == order.PurchaseOrderId)
                                            .Select(x => x);

                _context.PurchaseOrderDetails.RemoveRange(orderDetailsToDelete);
                _context.PurchaseOrders.Remove(order);

                _context.SaveChanges();
            }
        }
        #endregion
    }
}
