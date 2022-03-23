using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Transactions;
using CSO.VO;
using Helper;

namespace CSO.Proxy
{
    internal static class AreaProxy
    {
        public static ObservableCollection<AreaVO> Data(int ID, Level level = Level.Any, bool withAll = false)
        {
            ObservableCollection<AreaVO> areas = new ObservableCollection<AreaVO>();
            object[] parameters =
            {
                "@ID", ID,
                "@Level", level,
            };

            if (withAll)
            {
                string label = "Semua " + (level == Level.City ? "Kota" : level == Level.Area ? "Area" : "Propinsi");
                areas.Add(new AreaVO { ID = 0, Name = label });
            }

            DataSet result = DBHelper.ExecuteProcedure("uspAreaGet", parameters);
            foreach (DataRow dataRow in result.Tables[0].Rows)
            {
                areas.Add(new AreaVO(dataRow));
            }
            // has results
            return (areas);
        }



        public static ObservableCollection<ZIPCodeVO> FetchPostalCodes()
        {
            ObservableCollection<ZIPCodeVO> zips = new ObservableCollection<ZIPCodeVO>();
            ListDictionary parameters = new ListDictionary();
            parameters.Clear();

            DataSet result = DBHelper.Execute("uspZIPCodeListGet", parameters);
            foreach (DataRow dataRow in result.Tables[0].Rows)
            {
                zips.Add(new ZIPCodeVO(dataRow));
            }
            // has results
            return (zips);
        }
    }
}
