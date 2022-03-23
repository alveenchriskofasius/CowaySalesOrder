using System.Collections.Generic;
using System.Data;

namespace CSO.VO
{
    public class UserVO : BaseVO
    {
        #region Property setters/getters
        private int _id;
        public int ID
        {
            get { return _id; }
            set
            {
                SetProperty(ref _id, value);
            }
        }

        private int _role;
        public int RoleID
        {
            get { return _role; }
            set
            {
                SetProperty(ref _role, value);
            }
        }


        private string _fullName;
        public string FullName
        {
            get { return _fullName; }
            set
            {
                SetProperty(ref _fullName, value);
            }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                SetProperty(ref _username, value);
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value);
            }
        }

        private string _gender;
        public string Gender
        {
            get { return _gender; }
            set
            {
                SetProperty(ref _gender, value);
            }
        }


        private List<RoleVO> _roles;
        public List<RoleVO> Roles
        {
            get { return _roles; }
            set
            {
                SetProperty(ref _roles, value);
            }
        }
        public UserVO()
        {
        }

        public UserVO(UserVO user)
        {
            SetValue(user);
        }

        public UserVO(DataRow dataRow)
        {
            SetValue(dataRow);
        }

        public bool Role(string role)
        {
            if (_roles != null)
            {
                foreach (RoleVO userRole in _roles)
                {
                    if (userRole.Name.ToLower() == role.ToLower())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
        #region BaseVO Members
        public override string GetError(string columnName)
        {
            string result = null;

            return result;
        }

        #endregion

    }

}
