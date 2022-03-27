using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using CSO.VO;
using Helper;

namespace CSO.Proxy
{
    internal class RightProxy
    {
        public static Task<List<RightsVO>> Data(RoleVO role = null)
        {
            try
            {
                List<RightsVO> rights = new List<RightsVO>();
                return Task.Run(() =>
                {

                    using (TransactionScope scope = new TransactionScope())
                    {
                        object[] parameters = { "@RoleID", role.ID };
                        DataSet result = DBHelper.ExecuteProcedure("uspRightsGet", parameters);
                        if (result.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dataRow in result.Tables[0].Rows)
                            {
                                rights.Add(new RightsVO(dataRow));

                            }
                        }
                        scope.Complete();
                    }

                    return rights;
                });
            }
            catch (Exception e)
            {

                throw e;
            }

        }
    }
}
