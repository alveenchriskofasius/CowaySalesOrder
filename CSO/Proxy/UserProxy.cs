using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using CSO.VO;
using Helper;

namespace CSO.Proxy
{
    internal static class UserProxy
    {
        public static UserVO CurrentUser { get; set; } = new UserVO();

        public static Task<bool> Login(String username, String password)
        {
            return Task.Run(() =>
            {
                object[] parameters = {
                    "@Username", username,
                    "@Password", password};

                DataSet result = DBHelper.ExecuteProcedure("uspUserLogin", parameters);

                CurrentUser = new UserVO(); ;

                // has results
                if (result.Tables[0].Rows.Count == 1)
                {
                    CurrentUser = new UserVO(result.Tables[0].Rows[0]);
                    CurrentUser.Roles = RoleProxy.Data(CurrentUser.ID);
                }

                return CurrentUser.Roles != null;

            });
        }

        public static ObservableCollection<UserVO> Data()
        {
            ObservableCollection<UserVO> users = new ObservableCollection<UserVO>();

            try
            {

                DataSet result = DBHelper.ExecuteProcedure("uspUserGet");

                foreach (DataRow dataRow in result.Tables[0].Rows)
                {
                    users.Add(new UserVO(dataRow));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            // has results
            return (users);

        }
    }
}
