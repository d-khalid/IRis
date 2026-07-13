using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace IRis.Views.Main.Canvas.CircuitObjects.Components;

public partial class MultiplexerView : UserControl
{
    public static readonly IMultiValueConverter CanvasOffset = new SelectCanvasOffsetConverter();

    public MultiplexerView()
    {
        InitializeComponent();
    }

    private sealed class SelectCanvasOffsetConverter : IMultiValueConverter
    {
        public object? Convert(
            IList<object?> values,
            Type targetType,
            object? parameter,
            CultureInfo culture
        )
        {
            if (values.Count < 2 || values.Any(v => v is not double))
                return 0.0;

            return (double)values[0]! - (double)values[1]! - 5;
        }
    }
}
