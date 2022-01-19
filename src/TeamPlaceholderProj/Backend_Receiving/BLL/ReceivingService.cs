using Backend_Receiving.DAL;
using Backend_Receiving.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Receiving.BLL
{
    public class ReceivingService
    {
        private readonly Ebike_DMIT2018Context _content;
        internal ReceivingService(Ebike_DMIT2018Context context)
        {
            _content = context;
        }
        
        public List<OutstandingOrder> ListOutstandingOrders()
        {
            var result = _content.PurchaseOrders
                            .Where(po => po.Closed == false)
                            .Where(po => po.OrderDate.HasValue)
                            .Select(order => new OutstandingOrder
                            {
                                Id = order.PurchaseOrderId,
                                PoNumber = order.PurchaseOrderNumber,
                                Date = order.OrderDate.Value,
                                Vendor = order.Vendor.VendorName,
                                Phone = order.Vendor.Phone
                            });

            return result.ToList();
        }

        public ReceivingDetails GetReceivingDetials(int purchaseOrderId)
        {
            var pO = _content.PurchaseOrders.Find(purchaseOrderId);

            ReceivingDetails result = _content.PurchaseOrders
                                                .Where(po => po.PurchaseOrderId == purchaseOrderId)
                                                .Select(po => new ReceivingDetails
                                                {
                                                    PurchaseOrderNumber = po.PurchaseOrderNumber,
                                                    Vendor = po.Vendor.VendorName,
                                                    Phone = po.Vendor.Phone,
                                                    ReceivingItems = _content.PurchaseOrderDetails
                                                                                .Where(x => x.PurchaseOrderId == purchaseOrderId)
                                                                                .OrderBy(x => x.Part.PartId)
                                                                                .Select(x => new ReceivingItem
                                                                                {
                                                                                    PurchaseOrderDetailId = x.PurchaseOrderDetailId,
                                                                                    PartId = x.Part.PartId,
                                                                                    PartName = x.Part.Description,
                                                                                    OrdQty = x.Quantity,
                                                                                    RecQty = x.ReceiveOrderDetails.Sum(o => (int?)o.QuantityReceived) ?? 0

                                                                                }).ToList()
                                                }).FirstOrDefault();
            return result;
        }

        public void ProcessForceClose(int purchaseOrderId, string closeReason)
        {
            List<Exception> brokenRules = new List<Exception>();
            if (string.IsNullOrEmpty(closeReason))
                brokenRules.Add(new AggregateException("You need to enter the reason for the force close"));

            if (brokenRules.Any())
                throw new AggregateException("Unable to process your request:", brokenRules);
            var found = _content.PurchaseOrders.Find(purchaseOrderId);
            if (found is null)
                throw new Exception($"Purchase Order: {purchaseOrderId} is missing");
            found.Closed = true;
            _content.SaveChanges();
        }

        public void ProcessReceiving(int purchaseOrderId, int employeeId, List<CurrentReceiving> currentReceivings, List<UnOrderedItem> unOrderedItems)
        {
            List<Exception> brokenRules = new List<Exception>();

            ReceivingDetails currentPurchaseOrderStatus = GetReceivingDetials(purchaseOrderId);

            if(currentReceivings.Any(item => item.RecQty < 0))
                brokenRules.Add(new AggregateException("The receiving quantity should be an positive number!"));

            if(currentReceivings.Any(item => item.Return < 0))
                brokenRules.Add(new AggregateException("The return quantity should be an positive number!"));

            if(currentReceivings.Any(item => (item.Return > 0 && string.IsNullOrEmpty(item.Reason.Trim()))))
                brokenRules.Add(new AggregateException("Any return must comes with the reason!"));

            for(var index = 0; index < currentReceivings.Count(); index++)
            {
                var currentStatusItem = currentPurchaseOrderStatus.ReceivingItems;
                if(currentReceivings[index].RecQty > (currentStatusItem[index].OrdQty - currentStatusItem[index].RecQty))
                    brokenRules.Add(new AggregateException($"The receiving quantiy for part: {currentStatusItem[index].PartId} has exceed the outstanding quantaty!"));
            }

            if(unOrderedItems.Any(item => item.Qty <= 0))
                brokenRules.Add(new AggregateException("The quantity of entered unordered item should be an positive number!"));

            if (unOrderedItems.Any(item => string.IsNullOrEmpty(item.Description.Trim())))
                brokenRules.Add(new AggregateException("The entered unordered item should have a description!"));

            if (unOrderedItems.Any(item => string.IsNullOrEmpty(item.VendorPartId.Trim())))
                brokenRules.Add(new AggregateException("The entered unordered item should have a vendor part ID!"));

            if (brokenRules.Any())
                throw new AggregateException("Unable to process your request:", brokenRules);

            Entities.ReceiveOrder newData = new()
            {
                PurchaseOrderId = purchaseOrderId,
                EmployeeId = employeeId
            };
            _content.ReceiveOrders.Add(newData);
            _content.SaveChanges();
            
            foreach(var receiveItem in currentReceivings)
            {
                if(receiveItem.RecQty > 0)
                {
                    _content.ReceiveOrderDetails.Add(new Entities.ReceiveOrderDetail
                    {
                        ReceiveOrderId = newData.ReceiveOrderId,
                        PurchaseOrderDetailId = receiveItem.PurchaseOrderDetailId,
                        QuantityReceived = receiveItem.RecQty
                    });
                }
                if(receiveItem.Return > 0)
                {
                    _content.ReturnedOrderDetails.Add(new Entities.ReturnedOrderDetail
                    {
                        ReceiveOrderId = newData.ReceiveOrderId,
                        PurchaseOrderDetailId = receiveItem.PurchaseOrderDetailId,
                        Quantity = receiveItem.Return,
                        Reason = receiveItem.Reason
                    });
                }
            }
            _content.SaveChanges();

            foreach(var returnItem in unOrderedItems)
            {
                _content.UnorderedPurchaseItemCarts.Add(new Entities.UnorderedPurchaseItemCart
                {
                    Description = returnItem.Description,
                    VendorPartNumber = returnItem.VendorPartId,
                    Quantity =returnItem.Qty
                });
            }
            _content.SaveChanges();

            ReceivingDetails newPurchaseOrderStatus = GetReceivingDetials(purchaseOrderId);
            if(!newPurchaseOrderStatus.ReceivingItems.Any(item => item.OrdQty > item.RecQty))
            {
                var found = _content.PurchaseOrders.Find(purchaseOrderId);
                if (found is null)
                    throw new Exception($"Purchase Order: {purchaseOrderId} is missing");
                found.Closed = true;
                _content.SaveChanges();
            }
                

        }
    }
}
