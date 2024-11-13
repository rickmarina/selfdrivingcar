using System.Windows.Media;

namespace selfdrivingcar.src.visual
{
    internal class BrushesUtils
    {
        public static SolidColorBrush BrushRoad => new SolidColorBrush(Color.FromRgb(187, 187, 187));
        public static SolidColorBrush RandomBrush => new SolidColorBrush(Color.FromRgb((byte)Random.Shared.Next(0,255), (byte)Random.Shared.Next(0, 255), (byte)Random.Shared.Next(0, 255)));
        public static SolidColorBrush White => Brushes.White;
        public static SolidColorBrush Red => Brushes.Red;
        public static SolidColorBrush TreeColor => Brushes.DarkOliveGreen;
        public static SolidColorBrush BlackTransparent(byte alfa) => new SolidColorBrush(Color.FromArgb(alfa, 0, 0, 0));
        public static SolidColorBrush BlueTransparent(byte alfa) => new SolidColorBrush(Color.FromArgb(alfa, 0, 0, 255));
    }
}
