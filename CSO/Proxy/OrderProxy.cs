using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using CSO.VO;
using Helper;

namespace CSO.Proxy
{
    internal class OrderProxy
    {
        public static Task<OrderVO> Save(OrderVO order)
        {
            return Task.Run(() =>
            {
                bool isNew = order.ID == 0;
                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {

                        if (isNew)
                        {
                            object[] insert = {
                           "@Date",order.Date,
                           "@PersonInChargeID",order.PersonInChargeID,
                           "@CustomerID",order.CustomerID,
                           "@PaymentTypeID",order.PaymentTypeID,
                           "@PromotionID",order.PromotionID,
                           "@StatusID",order.StatusID,
                           "@OrderProductID",order.OrderProductID,
                           "@CreatedBy",UserProxy.CurrentUser.ID,
                           "@Quantity",order.Quantity,
                           "@GrandTotal",order.GrandTotal,
                           "@Discount",order.Discount,
                           "@InstallAddress",order.InstallAddress,
                           "@IsDiscount",order.IsDiscounted,
                           "@IsCustomerAddress",order.IsCustomerAddress,
                           "@ServicePackageID",order.ServicePackageID
                        };

                            DataSet result = DBHelper.ExecuteProcedure("uspOrderAdd", insert);
                            if (result.Tables[0].Rows.Count == 1)
                            {
                                // retrieve the ID
                                order.ID = Convert.ToInt32(result.Tables[0].Rows[0]["ID"]);
                                order.No = result.Tables[0].Rows[0].Field<String>("No");
                            }
                        }
                        else
                        {
                            object[] update = {
                           "@ID", order.ID,
                           "@Date",order.Date,
                           "@PersonInChargeID",order.PersonInChargeID,
                           "@CustomerID",order.CustomerID,
                           "@PaymentTypeID",order.PaymentTypeID,
                           "@PromotionID",order.PromotionID,
                           "@OrderProductID",order.OrderProductID,
                           "@UpdatedBy",UserProxy.CurrentUser.ID,
                           "@Quantity",order.Quantity,
                           "@GrandTotal",order.GrandTotal,
                           "@Discount",order.Discount,
                           "@InstallAddress",order.InstallAddress,
                           "@IsDiscount",order.IsDiscounted,
                           "@IsCustomerAddress",order.IsCustomerAddress,
                           "@ServicePackageID",order.ServicePackageID
                        };

                            DBHelper.ExecuteProcedure("uspOrderUpdate", update);
                        }
                        if (order.ID > 0)
                        {
                            SaveOrderProduct(order.ID, order.Products);
                        }
                        scope.Complete();
                    }
                }

                catch (Exception)
                {
                    // reverse transaction
                    if (isNew)
                    {
                        // reset parent
                        order.ID = 0;
                        order.No = "";

                        // reset children
                        foreach (OrderProductVO product in order.Products)
                        {
                            product.ID = 0;
                            product.OrderID = 0;
                        }
                    }
                    throw;
                }
                return order;
            });
        }
        private static void SaveOrderProduct(int orderID, ObservableCollection<OrderProductVO> products)
        {
            foreach (OrderProductVO product in products)
            {
                if (product.IsEdited == true)
                {
                    object[] parameters = {
                        "@ID",product.ID,
                        "@ProductID", product.ProductID,
                        "@Quantity", product.Quantity,
                        "@Amount",product.TotalAmount
                    };

                    product.OrderID = orderID;

                    DataSet data = DBHelper.ExecuteProcedure("uspOrderProductUpdate", parameters);

                    if (data.Tables.Count == 1)
                    {
                        // retrieve the ID
                        product.ID = Convert.ToInt32(data.Tables[0].Rows[0]["ID"]);
                    }
                }
            }
        }
        public static Task<List<OrderVO>> Data(FilterVO filter)
        {
            return Task.Run(() =>
            {
                List<OrderVO> customers = new List<OrderVO>();

                object[] parameters = {
                        "@No", filter.FullName,
                        "@DateFrom", filter.DateFrom,
                        "@DateTo", filter.DateTo,
                        "@StatusID", filter.StatusID,
                        "@CustomerID", filter.CustomerID,
                        "@Active", filter.Active
                };

                DataSet result = DBHelper.ExecuteProcedure("uspOrderListGet");

                foreach (DataRow dataRow in result.Tables[0].Rows)
                {
                    customers.Add(new OrderVO(dataRow));
                }

                // has results
                return customers;
            });
        }
    }
}
