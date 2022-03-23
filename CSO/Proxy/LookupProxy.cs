using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSO.VO;
using Helper;

namespace CSO.Proxy
{
    internal class LookupProxy
    {
        public static List<LookupVO> Data(string entity, string withAll = null)
        {
            List<LookupVO> Status = new List<LookupVO>();

            object[] parameters =
            {
                "@Entity", entity
            };

            DataSet result = DBHelper.ExecuteProcedure("uspLookupGet", parameters);

            if (withAll != null)
            {
                Status.Add(new LookupVO { ID = 0, Name = withAll });
            }

            foreach (DataRow dataRow in result.Tables[0].Rows)
            {
                Status.Add(new LookupVO(dataRow));
            }

            return (Status);
        }
    }
}
