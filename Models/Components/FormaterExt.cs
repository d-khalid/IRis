using Avalonia.Media;
using IRis.Models.Core;
using System.Globalization;

namespace IRis.Models.Components
{
    public static class FormaterExt
    {
        public static FormattedText CreateFormattedText(this string label)
        {
            return new FormattedText(
                label,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                ComponentDefaults.LabelTypeface,
                ComponentDefaults.LabelSize,
                ComponentDefaults.LabelBrush
            );
        }
    }
}
