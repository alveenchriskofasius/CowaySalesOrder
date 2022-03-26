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
                           "@No",order.No,
                           "@Date",order.Date,
                           "@PersonInChargeID",order.PersonInChargeID,
                           "@CustomerID",order.CustomerID,
                           "@PaymentTypeID",order.PaymentTypeID,
                           "@PromotionID",order.PromotionID,
                           "@StatusID",order.StatusID,
                           "@CreatedBy",UserProxy.CurrentUser.ID,
                           "@Quantity",order.Quantity,
                           "@GrandTotal",order.GrandTotal,
                           "@Discount",order.Discount,
                           "@InstallAddress",order.InstallAddress,
                           "@IsDiscount",order.IsDiscounted,
                           "@IsCustomerAddress",order.IsCustomerAddress,
                           "@ServicePackageID",order.ServicePackageID,
                           "@ImageUploaded",order.URLImage
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
                           "@UpdatedBy",UserProxy.CurrentUser.ID,
                           "@Quantity",order.Quantity,
                           "@GrandTotal",order.GrandTotal,
                           "@Discount",order.Discount,
                           "@InstallAddress",order.InstallAddress,
                           "@IsDiscount",order.IsDiscounted,
                           "@IsCustomerAddress",order.IsCustomerAddress,
                           "@ServicePackageID",order.ServicePackageID,
                           "@ImageUploaded",order.URLImage
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
            foreach (OrderProductVO orderProduct in products)
            {
                if (orderProduct.IsEdited == true)
                {
                    object[] parameters = {
                        "@ID",orderProduct.ID,
                        "@ProductID", orderProduct.ProductID,
                        "@Quantity", orderProduct.Quantity,
                        "@Amount",orderProduct.TotalAmount,
                        "@OrderID",orderID,
                        "@DiscountAmount",orderProduct.DiscountAmount,
                        "@TotalAmount",orderProduct.TotalAmount
                    };

                    DataSet data = DBHelper.ExecuteProcedure("uspOrderProductUpdate", parameters);

                    if (data.Tables.Count == 1)
                    {
                        // retrieve the ID
                        orderProduct.ID = Convert.ToInt32(data.Tables[0].Rows[0]["ID"]);
                    }
                }
            }
        }
        public static Task<List<OrderVO>> Data(FilterVO filter)
        {
            return Task.Run(() =>
            {
                List<OrderVO> orders = new List<OrderVO>();

                object[] parameters = {
                        "@No", filter.No,
                        "@DateFrom", filter.DateFrom,
                        "@DateTo", filter.DateTo,
                        "@StatusID", filter.StatusList,
                        "@CustomerID", filter.CustomerID
                };

                DataSet result = DBHelper.ExecuteProcedure("uspOrderListGet", parameters);

                foreach (DataRow dataRow in result.Tables[0].Rows)
                {
                    orders.Add(new OrderVO(dataRow));
                }

                // has results
                return orders;
            });
        }
        public static Task<List<OrderVO>> DataDashboard(FilterVO filter)
        {
            return Task.Run(() =>
            {
                List<OrderVO> orders = new List<OrderVO>();

                object[] parameters = {
                        "@No", filter.No,
                        "@DateFrom", filter.DateFrom,
                        "@DateTo", filter.DateTo,
                        "@StatusID", filter.StatusList,
                        "@CustomerID", filter.CustomerID
                };

                DataSet result = DBHelper.ExecuteProcedure("uspOrderDashBoardListGet", parameters);

                foreach (DataRow dataRow in result.Tables[0].Rows)
                {
                    orders.Add(new OrderVO(dataRow));
                }

                // has results
                return orders;
            });
        }
        public static OrderVO Data(int orderID)
        {
            OrderVO order = null;

            object[] data = {
                "@ID", orderID
            };

            DataSet result = DBHelper.ExecuteProcedure("uspOrderGet", data);
            if (result.Tables.Count == 1)
            {
                order = new OrderVO(result.Tables[0].Rows[0]);
                order.Products = DataProduct(order.ID);
            }


            // has results
            return order;
        }
        public static OrderDetailVO DataDetail(int orderID)
        {
            OrderDetailVO orderDetail = null;

            object[] data = {
                "@ID", orderID
            };

            DataSet result = DBHelper.ExecuteProcedure("uspOrderDetailGet", data);
            if (result.Tables.Count == 1)
            {
                orderDetail = new OrderDetailVO(result.Tables[0].Rows[0]);
            }

            // has results
            return orderDetail;
        }
        public static void UpdateStatus(List<OrderVO> orders, int statusID, int assignID = 0)
        {
            foreach (OrderVO order in orders)
            {
                if (order.Selected)
                {
                    object[] data = {
                        "@ID", order.ID,
                        "@StatusID", statusID,
                        "@AssignID", assignID,
                        "@StatusUpdatedBy", UserProxy.CurrentUser.ID, };
                    DBHelper.ExecuteProcedure("uspOrderStatusUpdate", data);
                }
            }
        }
        public static ObservableCollection<OrderProductVO> DataProduct(int orderID)
        {
            ObservableCollection<OrderProductVO> orderProducts = new ObservableCollection<OrderProductVO>();
            object[] parameters =
            {
                "@OrderID", orderID
            };

            DataSet result = DBHelper.ExecuteProcedure("uspOrderProductListGet", parameters);
            foreach (DataRow dataRow in result.Tables[0].Rows)
            {
                orderProducts.Add(new OrderProductVO(dataRow));
            }
            // has results
            return orderProducts;
        }
        public static void Delete(OrderProductVO orderProduct)
        {
            if (orderProduct.ID != 0)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    object[] parameters = {
                        "@OrderID", orderProduct.ID
                    };

                    DataSet result = DBHelper.ExecuteProcedure("uspOrderProductDelete", parameters);

                    scope.Complete();
                }
            }
        }
        public static void Delete(OrderVO order)
        {
            if (order.ID != 0)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    object[] parameters = {
                        "@ID", order.ID
                    };

                    DBHelper.ExecuteProcedure("uspOrderDelete", parameters);

                    scope.Complete();
                }
            }
        }
    }
}
