using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommandLine.Infrastructure
{
    internal class LocalizableAttributeProperty
    {
        private string _propertyName;
        private string _value;
        private Type _type;
        private PropertyInfo _localizationPropertyInfo;

        public LocalizableAttributeProperty(string propertyName)
        {
            _propertyName = propertyName;
        }

        public string Value
        {
            get { return GetLocalizedValue(); }
            set
            {
                _localizationPropertyInfo = null;
                _value = value;
            }
        }

        public Type ResourceType
        {
            set
            {
                _localizationPropertyInfo = null;
                _type = value;
            }
        }

        private string GetLocalizedValue()
        {
            if (String.IsNullOrEmpty(_value) || _type == null)
                return _value;
            if (_localizationPropertyInfo == null)
            {
                // Static class IsAbstract 
                if (!_type.IsVisible)
                    throw new ArgumentException("Invalid resource type", _propertyName);
                PropertyInfo propertyInfo = _type.GetProperty(_value, BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Static);
                if (propertyInfo == null || !propertyInfo.CanRead || propertyInfo.PropertyType != typeof(string))
                    throw new ArgumentException("Invalid resource property name", _propertyName);
                _localizationPropertyInfo = propertyInfo;
            }
            return (string)_localizationPropertyInfo.GetValue(null, null);
        }
    }

}
