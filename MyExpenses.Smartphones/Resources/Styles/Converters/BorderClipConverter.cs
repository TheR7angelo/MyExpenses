using System.Globalization;

namespace MyExpenses.Smartphones.Resources.Styles.Converters
{
    internal class BorderClipConverter : IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length <= 1
                || values[0] is not double width
                || values[1] is not double height) return null; // Return an appropriate default value or null for MAUI
            if (width < 1.0 || height < 1.0)
            {
                return null; // Replace Geometry.Empty with null or an empty path suitable for MAUI
            }

            CornerRadius cornerRadius = default;
            Thickness borderThickness = default;
            if (values.Length > 2 && values[2] is CornerRadius radius)
            {
                cornerRadius = NormalizeCornerRadius(radius);
                if (values.Length > 3 && values[3] is Thickness thickness)
                {
                    borderThickness = thickness;
                }
            }

            var path = GetRoundRectangle(new Rect(0, 0, width, height), borderThickness, cornerRadius);
            return path;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static CornerRadius NormalizeCornerRadius(CornerRadius cornerRadius)
        {
            var topLeft = cornerRadius.TopLeft < double.Epsilon ? 0.0 : cornerRadius.TopLeft;
            var topRight = cornerRadius.TopRight < double.Epsilon ? 0.0 : cornerRadius.TopRight;
            var bottomLeft = cornerRadius.BottomLeft < double.Epsilon ? 0.0 : cornerRadius.BottomLeft;
            var bottomRight = cornerRadius.BottomRight < double.Epsilon ? 0.0 : cornerRadius.BottomRight;

            return new CornerRadius(topLeft, topRight, bottomLeft, bottomRight);
        }

        private static PathF GetRoundRectangle(Rect baseRect, Thickness borderThickness, CornerRadius cornerRadius)
        {
            var path = new PathF();

            // Taking the border thickness into account
            var leftHalf = (float)(borderThickness.Left * 0.5);
            var topHalf = (float)(borderThickness.Top * 0.5);
            var rightHalf = (float)(borderThickness.Right * 0.5);
            var bottomHalf = (float)(borderThickness.Bottom * 0.5);

            // Start constructing the path for the rounded rectangle
            path.MoveTo((float)(baseRect.Left + cornerRadius.TopLeft - leftHalf), (float)baseRect.Top + topHalf);
            path.LineTo((float)(baseRect.Right - cornerRadius.TopRight + rightHalf), (float)baseRect.Top + topHalf);
            path.QuadTo((float)baseRect.Right - rightHalf, (float)baseRect.Top + topHalf, (float)baseRect.Right - rightHalf, (float)(baseRect.Top + cornerRadius.TopRight - topHalf));
            path.LineTo((float)(baseRect.Right - rightHalf), (float)(baseRect.Bottom - cornerRadius.BottomRight + bottomHalf));
            path.QuadTo((float)baseRect.Right - rightHalf, (float)baseRect.Bottom - bottomHalf, (float)(baseRect.Right - cornerRadius.BottomRight + rightHalf), (float)baseRect.Bottom - bottomHalf);
            path.LineTo((float)(baseRect.Left + cornerRadius.BottomLeft - leftHalf), (float)baseRect.Bottom - bottomHalf);
            path.QuadTo((float)baseRect.Left + leftHalf, (float)baseRect.Bottom - bottomHalf, (float)baseRect.Left + leftHalf, (float)(baseRect.Bottom - cornerRadius.BottomLeft + bottomHalf));
            path.LineTo((float)(baseRect.Left + leftHalf), (float)(baseRect.Top + cornerRadius.TopLeft - topHalf));
            path.QuadTo((float)baseRect.Left + leftHalf, (float)baseRect.Top + topHalf, (float)(baseRect.Left + cornerRadius.TopLeft - leftHalf), (float)baseRect.Top + topHalf);
            path.Close();

            return path;
        }
    }
}