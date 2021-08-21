using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using HyperVRemote.Source.Interface;

namespace HyperVRemote.Source.Implementation
{
    public class HyperVMemory : IHyperVMemory
    {
        private readonly ManagementObject _rawRam;
        public HyperVMemory(ManagementObject rawRam)
        {
            _rawRam = rawRam;
        }
        public Guid SystemName { get; set; }
        public int AmountUsed { get; set; }
    }
}
