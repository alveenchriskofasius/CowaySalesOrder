using Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using CSO.VO;
namespace CSO.Proxy
{
    internal class CustomerProxy
    {
        public static void Save(ref CustomerVO customer)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                bool isNew = customer.ID == 0;

                try
                {
                    if (isNew)
                    {
                        object[] insert = {
                            "@Name", customer.Name,
                            "@CustomerID",customer.CustomerID,
                            "@PlaceDOB",customer.PlaceDOB,
                            "@DateDOB",customer.Date,
                            "@Gender",customer.Gender,
                            "@AreaID", customer.AreaID,
                            "@TypeID", customer.TypeID,
                            "@Address", customer.Address,
                            "@Email", customer.Email,
                            "@Phone",customer.Phone,
                            "@Note",customer.Note,
                            "@CreatedBy", UserProxy.CurrentUser.ID,
                            "@Active",customer.Active,
                            "@Postal", customer.Postal,
                            "@District", customer.DistrictName,
                            "@Urban", customer.UrbanName
                        };

                        DataSet result = DBHelper.ExecuteProcedure("uspCustomerAdd", insert);
                        if (result.Tables[0].Rows.Count == 1)
                        {
                            // retrieve the ID
                            customer.ID = Convert.ToInt32(result.Tables[0].Rows[0]["ID"]);
                        }
                    }
                    else
                    {
                        object[] update = {
                            "@ID", customer.ID,
                            "@Name", customer.Name,
                            "@CustomerID",customer.CustomerID,
                            "@PlaceDOB",customer.PlaceDOB,
                            "@DateDOB",customer.Date,
                            "@Gender",customer.Gender,
                            "@AreaID", customer.AreaID,
                            "@TypeID", customer.TypeID,
                            "@Address", customer.Address,
                            "@Email", customer.Email,
                            "@Phone",customer.Phone,
                            "@Note",customer.Note,
                            "@CreatedBy", UserProxy.CurrentUser.ID,
                            "@Active",customer.Active,
                            "@Postal", customer.Postal,
                            "@District", customer.DistrictName,
                            "@Urban", customer.UrbanName
                        };

                        DBHelper.ExecuteProcedure("uspcustomerUpdate", update);
                    }

                    scope.Complete();
                }
                catch (Exception e)
                {
                    //// reverse transaction
                    if (isNew)
                    {
                        // reset parent
                        customer.ID = 0;
                    }

                    throw e;
                }
            }
        }
        public static Task<List<CustomerVO>> Data(FilterVO filter)
        {
            return Task.Run(() =>
            {
                List<CustomerVO> customers = new List<CustomerVO>();

                object[] parameters = {
                        "@Name", filter.FullName,
                        "@ProvinceID", filter.ProvinceID,
                        "@CityID", filter.CityID,
                        "@AreaID", filter.AreaID,
                        "@TypeID", filter.CustomerID,
                        "@Active", filter.Active
                };

                DataSet result = DBHelper.ExecuteProcedure("uspCustomerListGet", parameters);

                foreach (DataRow dataRow in result.Tables[0].Rows)
                {
                    customers.Add(new CustomerVO(dataRow));
                }

                // has results
                return customers;
            });
        }
        public static CustomerVO Data(CustomerVO customer)
        {
            object[] parameters = {
                "@ID", customer.ID

            };
            DataSet result = DBHelper.ExecuteProcedure("uspCustomerGet", parameters);
            if (result.Tables[0].Rows.Count > 0)
            {
                customer.SetValue(result.Tables[0].Rows[0]);
            }

            // has results
            return customer;
        }
        public static Task<List<CustomerVO>> Data(bool withAll)
        {
            return Task.Run(() =>
            {
                List<CustomerVO> customers = new List<CustomerVO>();


                DataSet result = DBHelper.ExecuteProcedure("uspCustomerCombo");
                if (withAll)
                {
                    customers.Add(new CustomerVO { ID = 0, Name = "Semua Pelanggan" });
                }
                foreach (DataRow dataRow in result.Tables[0].Rows)
                {
                    customers.Add(new CustomerVO(dataRow));
                }

                // has results
                return customers;
            });
        }

        public static void Delete(CustomerVO customer)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                object[] parameters = {
                    "@ID", customer.ID
                };
                DBHelper.ExecuteProcedure("uspTraderDelete", parameters);

                scope.Complete();
            }
        }
    }
}
