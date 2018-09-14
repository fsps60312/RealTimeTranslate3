using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RealTimeTranslate3
{
    public static class MyLib
    {
        public static UIElement Set(this UIElement uIElement,int row,int column)
        {
            Grid.SetRow(uIElement, row);
            Grid.SetColumn(uIElement, column);
            return uIElement;
        }
        public static FrameworkElement Set(this FrameworkElement frameworkElement,int row,int column,double marginThickness=0)
        {
            (frameworkElement as UIElement).Set(row, column);
            if (marginThickness != 0) frameworkElement.Margin = new Thickness(marginThickness);
            return frameworkElement;
        }
        public static ContentControl SetContent(this ContentControl contentControl,UIElement content)
        {
            contentControl.Content = content;
            return contentControl;
        }
        public static UIElement SetSpan(this UIElement uIElement,int rowSpan,int columnSpan)
        {
            Grid.SetRowSpan(uIElement, rowSpan);
            Grid.SetColumnSpan(uIElement, columnSpan);
            return uIElement;
        }
    }
}
