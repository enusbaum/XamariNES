using System;
using System.Collections.Generic;
using System.Text;

namespace XamariNES.Cartridge.Mappers
{
    public interface IA12Connection
    {
        void PulseA12();

        bool isIRQ { get; set; }
    }
}
