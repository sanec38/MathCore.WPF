using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MathCore.WPF
{
    // Defines SearchPredicate delegate alias as <element, search pattern, result>
    /// <summary>
    /// Provides a search/filter for items bind to an ItemsControl.
    /// To use this control, simply place an ItemsControl object as the content
    /// </summary>
    [TemplatePart(Name = "PART_FilterBox")]
    public class Filter : HeaderedContentControl
    {
        static Filter() { DefaultStyleKeyProperty.OverrideMetadata(typeof(Filter), new FrameworkPropertyMetadata(typeof(Filter))); }

        public static readonly Func<object, string, bool> ContentTextSearch = (element, pattern) =>
        {
            if(string.IsNullOrEmpty(pattern)) return true;
            var container = (ContentControl)element;
            return (container.Content?.ToString() ?? element.ToString()).ToLower().Contains(pattern.ToLower());
        };

        public static readonly DependencyProperty FilterBoxStyleProperty = DependencyProperty.Register("FilterBoxStyle", typeof(Style), typeof(Filter), new FrameworkPropertyMetadata(null, (s, e) => { }));
        public Style FilterBoxStyle { get => (Style)GetValue(FilterBoxStyleProperty); set => SetValue(FilterBoxStyleProperty, value); }

        public static readonly DependencyProperty PatternProperty = DependencyProperty.Register("Pattern", typeof(string), typeof(Filter), new FrameworkPropertyMetadata(string.Empty, (s, e) => ((Filter)s).View.Refresh()));
        public string Pattern { get => (string)GetValue(PatternProperty); set => SetValue(PatternProperty, value); }

        public static readonly DependencyProperty SearchStrategyProperty = DependencyProperty.Register("SearchStrategy", typeof(Func<object, string, bool>), typeof(Filter), new FrameworkPropertyMetadata(ContentTextSearch, OnSearchStrategyChanged));

        public Func<object, string, bool> SearchStrategy { get => (Func<object, string, bool>)GetValue(SearchStrategyProperty); set => SetValue(SearchStrategyProperty, value); }

        private static void OnSearchStrategyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var f = (Filter)d;
            if(f.View == null) return;
            f.View.Filter = i => f.SearchStrategy(i, f.Pattern);
        }

        private static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(Filter), new FrameworkPropertyMetadata(null, OnItemsSourceChanged));

        private IEnumerable ItemsSource { get => (IEnumerable)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var @old = e.OldValue as IEnumerable;
            if(@old != null)
                CollectionViewSource.GetDefaultView(@old).Filter = null;

            var @new = e.NewValue as IEnumerable;
            if(@new == null) return;
            var f = (Filter)d;
            CollectionViewSource.GetDefaultView(@new).Filter = i => f.SearchStrategy(i, f.Pattern);
        }

        private TextBox _FilterBox;

        public Filter() { FilterBoxStyle = (Style)TryFindResource(new ComponentResourceKey(typeof(Filter), "FilterBoxStyle")); }

        private ICollectionView View => CollectionViewSource.GetDefaultView(ItemsSource);

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            var control = newContent as ItemsControl;
            if(control == null)
                throw new ArgumentException("Content or Content Template must be an ItemsControl");

            SetBinding(ItemsSourceProperty, new Binding("ItemsSource") { Mode = BindingMode.OneWay, Source = control });

            base.OnContentChanged(oldContent, newContent);
        }

        public override void OnApplyTemplate()
        {
            _FilterBox = Template.FindName("PART_FilterBox", this) as TextBox;

            if(_FilterBox == null)
                throw new ArgumentException("Filter ControlTemplate must have at least one TextBox element named PART_FilterBox");

            SetBinding(PatternProperty, new Binding("Text") { Mode = BindingMode.TwoWay, Source = _FilterBox });

            base.OnApplyTemplate();
        }
    }
}