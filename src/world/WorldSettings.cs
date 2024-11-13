using selfdrivingcar.src.visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace selfdrivingcar.src.world
{
    internal class WorldSettings
    {
        public int RoadWidth { get; set; } = 70;
        public int RoadRoundness { get; set; } = 10;
        public int RoadStrokeThickness { get; set; } = 13;
        public int RoadPathStrokeThickness { get; set; } = 4;
        //Buildings 
        public int BuildingWidth { get; set;} = 150;
        public int BuildingMinLength { get; set; } = 150;
        public int BuildingSpacing { get; set; } = 50;
        public int BuidingStrokeThickness { get; set; } = 2;
        public SolidColorBrush BuildingFillColor { get; set; } = BrushesUtils.BlueTransparent(80);
        public SolidColorBrush BuildingStrokeColor { get; set; } = BrushesUtils.BlueTransparent(0);
        //Trees
        public int TryCountTrees { get; set; } = 50;
        public float TreeSize { get; set; } = 160;
    }
}
