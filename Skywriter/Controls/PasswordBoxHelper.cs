using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Skywriter.Controls
{
    public class PasswordBoxHelper : DependencyObject
    {
        public static bool GetIsMonitoring(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMonitoringProperty);
        }

        public static void SetIsMonitoring(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMonitoringProperty, value);
        }

        public static readonly DependencyProperty IsMonitoringProperty = DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(PasswordBoxHelper), new UIPropertyMetadata(false, OnIsMonitoringChanged));

        public static bool GetDisplayWatermark(DependencyObject obj)
        {
            return (bool)obj.GetValue(DisplayWatermark);
        }

        public static void SetDisplayWatermark(DependencyObject obj, bool value)
        {
            obj.SetValue(DisplayWatermark, value);
        }

        public static readonly DependencyProperty DisplayWatermark = DependencyProperty.RegisterAttached("DisplayWatermark", typeof(bool), typeof(PasswordBoxHelper), new UIPropertyMetadata(false));

        private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pb = d as PasswordBox;

            if (pb == null)
            {
                return;
            }

            SetDisplayWatermark(pb, true);
            
            if ((bool)e.NewValue)
            {
                pb.PasswordChanged += WatermarkVisible;
                pb.GotFocus += WatermarkVisible;
                pb.LostFocus += WatermarkVisible;
            }
            else
            {
                pb.PasswordChanged -= WatermarkVisible;
                pb.GotFocus -= WatermarkVisible;
                pb.LostFocus -= WatermarkVisible;
            }
        }

        static void WatermarkVisible(object sender, RoutedEventArgs e)
        {
            var pb = sender as PasswordBox;

            if (pb.IsFocused)
            {
                SetDisplayWatermark(pb, false);
            }
            else
            {
                if (pb.Password.Length > 0)
                {
                    SetDisplayWatermark(pb, false);
                }
                else
                {
                    SetDisplayWatermark(pb, true);
                }
            }
        }
    }
}
