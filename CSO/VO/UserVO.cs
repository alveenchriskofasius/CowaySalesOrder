using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using CSO.Proxy;
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
        private bool _active = true;
        public bool Active
        {
            get { return _active; }
            set
            {
                SetProperty(ref _active, value);
            }
        }
        private bool _canSave = true;
        public bool CanSave
        {
            get { return _canSave; }
            set
            {
                SetProperty(ref _canSave, value);
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
        private ObservableCollection<UserVO> _users = new ObservableCollection<UserVO>();
        public ObservableCollection<UserVO> Users
        {
            get
            {
                return _users;
            }
            set
            {
                SetProperty(ref _users, value);
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
        private List<RightsVO> _rights;
        public List<RightsVO> Rights
        {
            get { return _rights; }
            set
            {
                SetProperty(ref _rights, value);
            }
        }
        public UserVO()
        {
            Gender = "L";
        }
        public bool CheckUsername()
        {
            Users = UserProxy.Data();
            foreach (UserVO user in Users)
            {
                if (this.Username == user.Username && this.ID == 0)
                {
                    return false;
                }
            }
            return true;
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
                    if (userRole.Name.ToLower() == role.ToLower() && userRole.Selected)
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
            switch (columnName)
            {
                case "FullName":
                    if (string.IsNullOrEmpty(FullName))
                    {
                        result = "Nama lengkap wajib diisi";
                    }
                    break;
                case "Username":
                    CanSave = CheckUsername();
                    if (string.IsNullOrEmpty(Username))
                        result = "Username wajib diisi";
                    else if (!CanSave)
                    {
                        result = "Username sudah ada";
                    }
                    break;
                case "Gender":
                    if (string.IsNullOrEmpty(Gender))
                    {
                        result = "Jenis Kelamin wajib diisi";
                    }
                    break;
                case "Password":
                    if (string.IsNullOrEmpty(Password))
                    {
                        result = "Password wajib diisi";
                    }
                    break;
            }
            return result;
        }

        #endregion

    }

}
