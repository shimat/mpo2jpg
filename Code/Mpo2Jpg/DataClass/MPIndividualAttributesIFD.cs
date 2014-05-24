using System;
using System.Collections.Generic;
using System.Text;

namespace Mpo2Jpg
{
    public class MPIndividualAttributesIFD
    {
        public ushort Count { get; set; }
        public IFD MPFVersion { get; set; }
        public IFD MPIndividualNum { get; set; }
        public IFD PanOrientation { get; set; }
        public IFD PanOverlap_H { get; set; }
        public IFD PanOverlap_V { get; set; }
        public IFD BaseViewpointNum { get; set; }
        public IFD ConvergenceAngle { get; set; }
        public IFD BaselineLength { get; set; }
        public IFD VerticalDivergence { get; set; }
        public IFD AxisDistance_X { get; set; }
        public IFD AxisDistance_Y { get; set; }
        public IFD AxisDistance_Z { get; set; }
        public IFD YawAngle { get; set; }
        public IFD PitchAngle { get; set; }
        public IFD RollAngle { get; set; }
        public uint NextIFDOffset { get; set; }
    }
}
