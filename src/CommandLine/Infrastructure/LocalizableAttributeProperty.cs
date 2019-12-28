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
                    throw new ArgumentException($"{_type.FullName} is not visible.", nameof(ResourceType));
                PropertyInfo propertyInfo = _type.GetProperty(_value, BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Static);
                if (propertyInfo == null)
                {
                    PropertyInfo nonVisibleProperty = _type.GetProperty(_value, BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Static);
                    if (nonVisibleProperty != null)
                        throw new ArgumentException($"{_type.FullName}.{_value} is not visible.", _propertyName);

                    PropertyInfo instanceProperty = _type.GetProperty(_value, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance);
                    if (instanceProperty != null)
                        throw new ArgumentException($"{_type.FullName}.{_value} is not static.", _propertyName);

                    throw new ArgumentException($"{_type.FullName}.{_value} does not exist.", _propertyName);
                }

                if (!propertyInfo.CanRead)
                    throw new ArgumentException($"{_type.FullName}.{_value} can not be read.", _propertyName);
                if(propertyInfo.PropertyType != typeof(string))
                    throw new ArgumentException($"{_type.FullName}.{_value} is not a string.", _propertyName);

                _localizationPropertyInfo = propertyInfo;
            }
            return (string)_localizationPropertyInfo.GetValue(null, null);
        }
    }

}
