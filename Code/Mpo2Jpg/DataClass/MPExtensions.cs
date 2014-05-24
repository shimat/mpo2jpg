using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mpo2Jpg
{
    public class MPExtensions
    {
        public MPHeader Header { get; set; }
        public MPIndexIFD Index { get; set; }
        public MPIndividualAttributesIFD IndAttr { get; set; }
    }
}
