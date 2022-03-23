using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSO.VO
{
    public class CustomerVO : BaseHeaderVO
    {
        #region Property setters/getters

        private string _address = "";
        public string Address
        {
            get { return _address; }
            set
            {
                SetProperty(ref _address, value);
            }
        }

        private string _postal = "";
        public string Postal
        {
            get { return _postal; }
            set
            {
                SetProperty(ref _postal, value);
            }
        }

        private int _provinceID = 0;
        public int ProvinceID
        {
            get { return _provinceID; }
            set
            {
                SetProperty(ref _provinceID, value);
            }
        }

        private string _provinceName = "";
        public string ProvinceName
        {
            get { return _provinceName; }
            set
            {
                SetProperty(ref _provinceName, value);
            }
        }

        private int _cityID = 0;
        public int CityID
        {
            get { return _cityID; }
            set
            {
                SetProperty(ref _cityID, value);
            }
        }

        private string _cityName = "";
        public string CityName
        {
            get { return _cityName; }
            set
            {
                SetProperty(ref _cityName, value);
            }
        }

        private int _areaID = 0;
        public int AreaID
        {
            get { return _areaID; }
            set
            {
                SetProperty(ref _areaID, value);
            }
        }

        private string _areaName = "";
        public string AreaName
        {
            get { return _areaName; }
            set
            {
                SetProperty(ref _areaName, value);
            }
        }

        private string _districtName = "";
        public string DistrictName
        {
            get { return _districtName; }
            set
            {
                SetProperty(ref _districtName, value);
            }
        }

        private string _urbanName = "";
        public string UrbanName
        {
            get { return _urbanName; }
            set
            {
                SetProperty(ref _urbanName, value);
            }
        }

        private string _email = "";
        public string Email
        {
            get { return _email; }
            set
            {
                SetProperty(ref _email, value);
            }
        }

        private string _phone = "";
        public string Phone
        {
            get { return _phone; }
            set
            {
                SetProperty(ref _phone, value);
            }
        }

        private string _note = "";
        public string Note
        {
            get { return _note; }
            set
            {
                SetProperty(ref _note, value);
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

        private string _placeDOB = "";
        public string PlaceDOB
        {
            get { return _placeDOB; }
            set
            {
                SetProperty(ref _placeDOB, value);
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
        private string _customerID = "";
        public string CustomerID
        {
            get { return _customerID; }
            set
            {
                SetProperty(ref _customerID, value);
            }
        }
        public CustomerVO()
        {
            TypeID = 2;
            Gender = "L";
        }
        public CustomerVO(CustomerVO customer)
        {
            SetValue(customer);
        }

        public CustomerVO(DataRow dataRow)
        {
            SetValue(dataRow);
        }
        #endregion
        public override string GetError(string columnName)
        {
            string result = null;
            switch (columnName)
            {
                case "Name":
                    if (string.IsNullOrEmpty(Name))
                    {
                        result = "Customer Name required";
                    }

                    break;

                case "CityID":
                    if (CityID == 0)
                    {
                        result = "City required";
                    }

                    break;
                case "AreaID":
                    if (AreaID == 0)
                    {
                        result = "Area required";
                    }

                    break;

                case "TypeID":
                    if (TypeID == 0)
                    {
                        result = "Customer type required  ";
                    }

                    break;

                case "Address":
                    if (string.IsNullOrEmpty(Address))
                    {
                        result = "Address required";
                    }

                    break;

                case "Phone":
                    if (string.IsNullOrEmpty(Phone))
                    {
                        result = "Phone number required";
                    }
                    else if (Phone.Length > 12)
                    {
                        result = "Maximum of Phone number is 12 digit";
                    }
                    else if (Phone.Length < 10)
                    {
                        result = "Minimum of PHone number is 10 digit";
                    }
                    break;
                case "CustomerID":
                    if (string.IsNullOrEmpty(CustomerID))
                    {
                        result = "CustomerID required";
                    }
                    else if (CustomerID.Length < 16 || CustomerID.Length > 16)
                    {
                        result = "CustomerID must be 16 digit";
                    }
                    break;
                case "PlaceDOB":
                    if (string.IsNullOrEmpty(PlaceDOB))
                    {
                        result = "Place Date of Birth required";
                    }
                    break;
                case "Date":
                    DateTime dt_now = DateTime.Now;

                    DateTime dt_18 = Date.AddYears(18);
                    if (dt_18.Date >= dt_now.Date)
                    {
                        result = "Age minimum is 18 years";
                    }
                    break;
                case "Gender":
                    if (string.IsNullOrEmpty(Gender))
                    {
                        result = "Gender required";
                    }
                    break;
                case "Email":
                    if (!Regex.IsMatch(Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
                        result = "Email is not valid";
                    if (string.IsNullOrEmpty(Email))
                        result = "Email required";
                    break;
            }
            return result;
        }
    }
}
