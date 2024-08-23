using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Runtime.InteropServices;

namespace Dutch_Lady_App
{
    public class ConverterStringCount : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ushort count_NG = (ushort)value;
            return count_NG.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string count_NG = (string)value;
            if (count_NG == "") { return 0; }
            return System.Convert.ToInt16(count_NG);
        }
    }

    public class ConverterStringOutValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float Out_Value= (float)value;
            if (Out_Value == 0) { return ""; }
            return Out_Value.ToString("F");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string Out_Value = (string)value;
            if (Out_Value == "") { return 0.00; }
            return System.Convert.ToDouble(Out_Value);
        }
    }

    public class ConverterStringOutIntValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float Out_Value = (float)value;
            if (Out_Value == 0) { return ""; }
            return Out_Value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string Out_Value = (string)value;
            if (Out_Value == "") { return 0; }
            return System.Convert.ToDouble(Out_Value);
        }
    }

    public class ConverterColor: IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            bool ok_color = (bool)value;
            if (ok_color) return Brushes.LimeGreen;
            return Brushes.Red;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush ok_color = (SolidColorBrush)value;
            if (ok_color == Brushes.Red) return false;
            return true;
        }
    }

    public class ConverterString_OK_NG : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            bool ok_color = (bool)value;
            if (ok_color) return "  OK  ";
            return "  NG  ";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string ok_color = (string)value;
            if (ok_color == "  NG  ") return false;
            return true;
        }
    }
    public struct ImageInfo
    {
        public IntPtr data; // IntPtr type because we'll free it later in C++ source 
        public int size;
        public int elementSize;
    };
    public struct Deform
    {
        public int wrinkle_area;
        public float circularity;
    }
    public partial class ImageClassifier
    {
        const string dllImport = ".\\x64\\Release\\MyDll.dll";
        [DllImport(dllImport, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr CreateClassifierModel(ref string modelPath);
        [DllImport(dllImport, CallingConvention = CallingConvention.Cdecl)]
        private static extern void DisposeClassifierModel(IntPtr Model);

        private readonly IntPtr ImgClassifierPointer;
        public ImageClassifier(string modelPath)
        {
            ImgClassifierPointer = CreateClassifierModel(ref modelPath);
        }
        ~ImageClassifier()
        {
            DisposeClassifierModel(ImgClassifierPointer);
        }
        public void Dispose()
        {
            DisposeClassifierModel(ImgClassifierPointer);
        }
    }
}
