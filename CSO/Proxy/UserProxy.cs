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

                CurrentUser = new UserVO();

                // has results
                if (result.Tables[0].Rows.Count == 1)
                {
                    CurrentUser = new UserVO(result.Tables[0].Rows[0]);
                    CurrentUser.Roles = RoleProxy.Data(CurrentUser.ID);
                }

                return CurrentUser.Roles != null;

            });
        }
        public static void Save(ref UserVO user, List<RoleVO> roles = null)
        {
            bool isNew = user.ID == 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {

                    if (isNew)
                    {
                        object[] parameters = {
                            "@Username", user.Username,
                            "@Password", user.Password,
                            "@FullName", user.FullName,
                            "@Gender", user.Gender,
                            "@Active", user.Active
                        };

                        DataSet result = DBHelper.ExecuteProcedure("uspUserAdd", parameters);

                        if (result.Tables[0].Rows.Count == 1)
                        {
                            // retrieve the ID
                            user.ID = Convert.ToInt32(result.Tables[0].Rows[0]["ID"]);
                        }
                    }
                    else
                    {
                        object[] parameters = {
                            "@ID", user.ID,
                            "@Username", user.Username,
                            "@Password", user.Password,
                            "@FullName", user.FullName,
                            "@Gender", user.Gender,
                            "@Active", user.Active,
                        };
                        DBHelper.ExecuteProcedure("uspUserUpdate", parameters);
                    }

                    // save user role
                    if (roles != null)
                    {
                        //roles = ListboxRole.ItemsSource as List<RoleVO>;
                        RoleProxy.SaveUserRole(roles, user.ID);
                    }

                    scope.Complete();
                }

            }
            catch (Exception ex)
            {
                // rollback
                if (isNew)
                {
                    user.ID = 0;
                }

                throw ex;
            }

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
                foreach (UserVO user in users)
                {
                    user.Roles = RoleProxy.Data(user.ID).Where(x => x.Selected == true).ToList();

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            // has results
            return (users);

        }
        public static UserVO Data(int id)
        {
            UserVO user = null;
            try
            {
                object[] parameters = { "@ID", id };
                DataSet result = DBHelper.ExecuteProcedure("uspUserGet", parameters);
                if (result.Tables[0].Rows.Count > 0)
                {
                    user = new UserVO(result.Tables[0].Rows[0]);
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

            // has results
            return user;

        }
        public static void Delete(UserVO user)
        {
            if (user.ID != 0)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    object[] parameters = {
                        "@ID", user.ID
                    };

                    DBHelper.ExecuteProcedure("uspUserDelete", parameters);

                    scope.Complete();
                }
            }
        }
    }
}
