using System;
using System.Diagnostics.Contracts;
using System.Windows.Markup;

namespace Fusee.Examples.PcRendering.WPF {

    public class EnumBindingSourceExtension : MarkupExtension
    {
        private Type _enumType;
        public Type EnumType
        {
            get
            {
                return _enumType;
            }
            set
            {
                Contract.Assert(null != value);

                if (value != _enumType)
                {
                    var enumType = Nullable.GetUnderlyingType(value) ?? value;
                    Contract.Assert(enumType.IsEnum);

                    _enumType = value;
                }
            }
        }

        public EnumBindingSourceExtension()
        {
            // N/A
        }

        public EnumBindingSourceExtension(Type enumType)
        {
            Contract.Requires(null != enumType);

            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Contract.Assert(null != _enumType);

            var actualEnumType = Nullable.GetUnderlyingType(_enumType) ?? _enumType;
            var enumValues = Enum.GetValues(actualEnumType);

            if (actualEnumType == _enumType)
            {
                return enumValues;
            }

            var tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);

            return tempArray;
        }
    }
}