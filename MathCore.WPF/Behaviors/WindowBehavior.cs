﻿using System;
using System.Windows;
using MathCore.Annotations;
using Microsoft.Xaml.Behaviors;
// ReSharper disable MemberCanBeProtected.Global

namespace MathCore.WPF.Behaviors
{
    public abstract class WindowBehavior : Behavior<UIElement>
    {
        private WeakReference<Window>? _WindowReference;

        [CanBeNull]
        public Window? AssociatedWindow => _WindowReference is null 
            ? null
            : _WindowReference.TryGetTarget(out var window)
                ? window 
                : null;

        protected override void OnAttached()
        {
            var window = AssociatedObject as Window ?? AssociatedObject.FindVisualParent<Window>();
            _WindowReference = window is null ? null : new WeakReference<Window>(window);
        }
    }
}