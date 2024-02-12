using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CommandLine.Infrastructure
{
    internal class LocalizableAttributeProperty
    {
        private string _propertyName;
        private string _value;

#if NET8_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
            DynamicallyAccessedMemberTypes.PublicMethods)]
#endif
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

#if NET8_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods |
            DynamicallyAccessedMemberTypes.PublicProperties)]
#endif
        public Type ResourceType
        {
            set
            {
                _localizationPropertyInfo = null;
                _type = value;
            }
        }

#if NET8_0_OR_GREATER
        [UnconditionalSuppressMessage("Reflection", "IL2072")]
#endif
        private string GetLocalizedValue()
        {
            if (string.IsNullOrEmpty(_value) || _type == null)
                return _value;
            if (_localizationPropertyInfo != null)
            {
                return _localizationPropertyInfo.GetValue(null, null).Cast<string>();
            }

            // Static class IsAbstract
            if (!_type.IsVisible)
                throw new ArgumentException(
                    $"Invalid resource type '{_type.FullName}'! {_type.Name} is not visible for the parser! Change resources 'Access Modifier' to 'Public'",
                    _propertyName);
            PropertyInfo propertyInfo = _type.GetProperty(_value,
                BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Static);

            bool IsStringable(
#if NET8_0_OR_GREATER
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
#endif
                Type type) =>
                type != typeof(string) && !type.CanCast<string>();

            if (propertyInfo == null || !propertyInfo.CanRead || IsStringable(propertyInfo.PropertyType))
            {
                throw new ArgumentException($"Invalid resource property name! Localized value: {_value}",
                    _propertyName);
            }

            _localizationPropertyInfo = propertyInfo;

            return _localizationPropertyInfo.GetValue(null, null).Cast<string>();
        }
    }
}
