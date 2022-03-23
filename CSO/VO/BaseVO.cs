using System;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using Prism.Mvvm;

namespace CSO.VO
{
    public abstract class BaseVO : BindableBase, IDataErrorInfo
    {

        #region IDataErrorInfo Members

        public string Error { get; set; }

        public string this[string columnName]
        {
            get
            {
                return GetError(columnName);
            }
        }
        #endregion

        #region Helper
        // Comparing objects content
        public bool Compare(BaseVO object2)
        {
            // Loop through each class properties
            foreach (var property in GetType().GetProperties())
            {
                object value1 = GetType().GetProperty(property.Name).GetValue(this);
                object value2 = GetType().GetProperty(property.Name).GetValue(object2);

                if (value1 != null && value2 != null)
                {
                    if (value1.ToString() != value2.ToString())
                    {
                        return false;
                    }
                }
                else if (value1 == null && value2 == null)
                {
                    if (property.Name == "Error")
                    {
                        return true;
                    }
                    continue;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        // Populate object properties from another object instance
        public void SetValue(BaseVO object2)
        {
            // Loop through each class properties
            PropertyInfo[] properties = GetType().GetProperties();

            foreach (PropertyInfo property in GetType().GetProperties())
            {
                if (property.CanRead && property.CanWrite)
                {
                    PropertyInfo property2 = object2.GetType().GetProperty(property.Name, property.PropertyType);

                    if (property2 != null)
                    {
                        property.SetValue(this, property2.GetValue(object2), null);
                    }
                }
            }
        }


        // Populate object properties from a DataRow object
        public void SetValue(DataRow row)
        {
            foreach (DataColumn c in row.Table.Columns)
            {
                System.Reflection.PropertyInfo property;

                try
                {
                    property = GetType().GetProperty(c.ColumnName);
                }
                catch (AmbiguousMatchException)
                {
                    property = GetType().GetProperty(c.ColumnName, c.DataType);
                }

                if (property != null)
                {
                    try
                    {
                        if (row[c] != DBNull.Value)
                        {
                            property.SetValue(this, row[c], null);
                        }
                        else
                        {
                            if (property.PropertyType == typeof(String))
                                property.SetValue(this, "", null);
                        }

                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(c.ColumnName + ": " + ex.Message);
                        if (property.PropertyType == typeof(int))
                        {
                            GetType().GetProperty(property.Name, typeof(int)).SetValue(this, Convert.ToInt32(row[c]), null);
                        }
                        else if (property.PropertyType == typeof(Int16))
                        {
                            GetType().GetProperty(property.Name, typeof(Int16)).SetValue(this, Convert.ToInt16(row[c]), null);
                        }
                        else if (property.PropertyType == typeof(decimal))
                        {
                            GetType().GetProperty(property.Name, typeof(decimal)).SetValue(this, Convert.ToDecimal(row[c]), null);
                        }
                        else if (property.PropertyType == typeof(DateTime))
                        {
                            GetType().GetProperty(property.Name, typeof(DateTime)).SetValue(this, Convert.ToDateTime("1/1/2000"), null);
                        }
                        else if (property.PropertyType == typeof(bool))
                        {
                            GetType().GetProperty(property.Name, typeof(bool)).SetValue(this, Convert.ToBoolean(false), null);
                        }
                    }

                }
            }
        }

        abstract public string GetError(string columnName);

        #endregion

    }
}
