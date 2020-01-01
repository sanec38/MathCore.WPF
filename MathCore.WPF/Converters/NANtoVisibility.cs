using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(NaNtoVisibility))]
    [ValueConversion(typeof(double), typeof(Visibility))]
    public class NaNtoVisibility : ValueConverter
    {
        public bool Collapse { get; set; }

        public bool Inverted { get; set; }

        /// <inheritdoc />
        protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => 
            v is null 
                ? (object?) null 
                : Inverted
                    ? !double.IsNaN((double)v)
                        ? (Collapse ? Visibility.Collapsed : Visibility.Hidden)
                        : Visibility.Visible
                    : double.IsNaN((double)v)
                        ? (Collapse ? Visibility.Collapsed : Visibility.Hidden)
                        : Visibility.Visible;
    }
}