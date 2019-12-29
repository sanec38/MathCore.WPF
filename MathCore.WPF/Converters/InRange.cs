﻿using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(InRange))]
    [ValueConversion(typeof(double), typeof(bool?))]
    public class InRange : DoubleToBoolConverter
    {
        private Interval _Interval = new Interval(double.NegativeInfinity, double.PositiveInfinity);

        public double Min { get => _Interval.Min; set => _Interval = _Interval.SetMin(value); }
        public bool MinInclude { get => _Interval.MinInclude; set => _Interval = _Interval.IncludeMin(value); }
        public double Max { get => _Interval.Max; set => _Interval = _Interval.SetMax(value); }
        public bool MaxInclude { get => _Interval.MaxInclude; set => _Interval = _Interval.IncludeMax(value); }

        public InRange() { }

        public InRange(double MinMax) : this(new Interval(-MinMax, MinMax)) { }
        public InRange(double min, double max) : this(new Interval(Math.Min(min, max), Math.Max(min, max))) { }

        public InRange(Interval interval) => _Interval = interval;

        /// <inheritdoc />
        protected override bool? Convert(double v) => v.IsNaN() ? null : (bool?)_Interval.Check(v);
    }
}