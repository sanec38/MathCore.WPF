using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(GreateThenOrEqual))]
    [ValueConversion(typeof(double), typeof(bool?))]
    public class GreateThenOrEqual : DoubleToBoolConverter
    {
        public double Value { get; set; } = double.NegativeInfinity;

        public GreateThenOrEqual() { }

        public GreateThenOrEqual(double value) => Value = value;

        protected override bool? Convert(double v) => v.IsNaN() ? null : (bool?)(v >= Value);
    }
}