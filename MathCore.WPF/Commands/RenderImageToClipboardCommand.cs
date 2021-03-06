﻿using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MathCore.Annotations;
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Commands
{
    public class RenderImageToClipboardCommand : LambdaCommand<FrameworkElement>
    {
        public static double PictureFactor { get; set; } = 1;

        /// <inheritdoc />
        public RenderImageToClipboardCommand()
        {
            _ExecuteAction = RenderElement;
            _CanExecute = e => e != null;
        }

        private static void RenderElement([NotNull] FrameworkElement? e)
        {
            var height = e.ActualHeight;
            var width = e.ActualWidth;
            e.Arrange(new Rect(0, 0, width, height));
            //(e as ParameterViewer)?.DrawAll();
            e.UpdateLayout();

            var bitmap_height = height * PictureFactor;
            var bitmap_width = width * PictureFactor;

            var bitmap = new RenderTargetBitmap((int)bitmap_width, (int)bitmap_height, 96 * bitmap_width / width, 96 * bitmap_height / height, PixelFormats.Default);
            bitmap.Render(e);
            Clipboard.SetImage(bitmap);
        }
    }
}