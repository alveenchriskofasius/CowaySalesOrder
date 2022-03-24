using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSO.VO
{
    public abstract class BaseHeaderVO : BaseVO
    {
        private bool _selected = false;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                SetProperty(ref _selected, value);
            }
        }

        private int _id = 0;
        public int ID
        {
            get { return _id; }
            set
            {
                SetProperty(ref _id, value);
            }
        }

        private string _no = "";
        public string No
        {
            get { return _no; }
            set
            {
                SetProperty(ref _no, value);
            }
        }

        private DateTime _date = DateTime.Now;
        public DateTime Date
        {
            get { return _date; }
            set
            {
                SetProperty(ref _date, value);
            }
        }

        private int _typeID = 0;
        public int TypeID
        {
            get { return _typeID; }
            set
            {
                SetProperty(ref _typeID, value);
            }
        }

        private string _typeName = "";
        public string TypeName
        {
            get { return _typeName; }
            set
            {
                SetProperty(ref _typeName, value);
            }
        }


        private int _statusID = 1;
        public int StatusID
        {
            get { return _statusID; }
            set
            {
                SetProperty(ref _statusID, value);
            }
        }

        private string _status = "";
        public string Status
        {
            get { return _status; }
            set
            {
                SetProperty(ref _status, value);
            }
        }

        private int _createdBy = 0;
        public int CreatedBy
        {
            get { return _createdBy; }
            set
            {
                SetProperty(ref _createdBy, value);
            }
        }

        private string _createdByName = "";
        public string CreatedByName
        {
            get { return _createdByName; }
            set
            {
                SetProperty(ref _createdByName, value);
            }
        }

        private DateTime _createdOn = DateTime.Now;
        public DateTime CreatedOn
        {
            get { return _createdOn; }
            set
            {
                SetProperty(ref _createdOn, value);
            }
        }

        private int _updatedBy = 0;
        public int UpdatedBy
        {
            get { return _updatedBy; }
            set
            {
                SetProperty(ref _updatedBy, value);
            }
        }

        private string _updatedByName = "";
        public string UpdatedByName
        {
            get { return _updatedByName; }
            set
            {
                SetProperty(ref _updatedByName, value);
            }
        }

        private string _updatedOn = "";
        public string UpdatedOn
        {
            get { return _updatedOn; }
            set
            {
                SetProperty(ref _updatedOn, value);
            }
        }

        private string _verifiedOn = "";
        public string VerifiedOn
        {
            get { return _verifiedOn; }
            set
            {
                SetProperty(ref _verifiedOn, value);
            }
        }

        private int _verifiedBy = 0;
        public int VerifiedBy
        {
            get { return _verifiedBy; }
            set
            {
                SetProperty(ref _verifiedBy, value);
            }
        }

        private string _verifiedByName = "";
        public string VerifiedByName
        {
            get { return _verifiedByName; }
            set
            {
                SetProperty(ref _verifiedByName, value);
            }
        }

        private string _approvedOn = "";
        public string ApprovedOn
        {
            get { return _approvedOn; }
            set
            {
                SetProperty(ref _approvedOn, value);
            }
        }

        private int _approvedBy = 0;
        public int ApprovedBy
        {
            get { return _approvedBy; }
            set
            {
                SetProperty(ref _approvedBy, value);
            }
        }

        private string _approvedByName = "";
        public string ApprovedByName
        {
            get { return _approvedByName; }
            set
            {
                SetProperty(ref _approvedByName, value);
            }
        }
        private string _name = "";
        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value);
            }
        }
    }
}
