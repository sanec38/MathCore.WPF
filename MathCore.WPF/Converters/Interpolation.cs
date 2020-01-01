﻿using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    [MarkupExtensionReturnType(typeof(Interpolation))]
    public class Interpolation : DoubleValueConverter
    {
        private Polynom? _Polynom;
        private Point[]? _Points;

        public Point[]? Points
        {
            get => _Points;
            set
            {
                if(ReferenceEquals(_Points, value)) return;
                _Polynom = new MathCore.Interpolation.Lagrange(value.Select(p => p.X).ToArray(), value.Select(p => p.Y).ToArray()).Polynom;
                _Points = value;
            }
        }

        protected override double Convert(double v, double? p = null) => _Polynom!.Value(v);
    }
}
