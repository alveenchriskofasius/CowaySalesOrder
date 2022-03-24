using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using CSO.VO;
using Helper;
namespace CSO.Proxy
{
    internal static class RoleProxy
    {
        private static RoleVO Role { get; set; }

        public static List<RoleVO> Data(int userID)
        {
            List<RoleVO> roles = new List<RoleVO>();
            object[] parameters =
            {
                "@UserID", userID,
            };
            DataSet result = DBHelper.ExecuteProcedure("uspUserRoleGet", parameters);

            foreach (DataRow dataRow in result.Tables[0].Rows)
            {
                roles.Add(new RoleVO(dataRow));
            }
            // has results
            return roles;
        }
        public static void SaveUserRole(List<RoleVO> roles, int userID)
        {
            ListDictionary param = new ListDictionary();
            string roleIDs = "";

            foreach (RoleVO role in roles)
            {
                if (role.Selected)
                    roleIDs += role.ID.ToString() + ",";
            }

            param.Clear();
            param.Add("@RoleIDs", roleIDs);
            param.Add("@UserID", userID);
            DBHelper.Execute("uspUserRoleAdd", param);
        }
    }
}
