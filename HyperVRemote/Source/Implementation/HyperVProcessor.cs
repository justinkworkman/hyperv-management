using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using HyperVRemote.Source.Interface;

namespace HyperVRemote.Source.Implementation
{
    public class HyperVProcessor : IHyperVProcessor
    {
        private readonly ManagementObject _rawCpu;
        public HyperVProcessor(ManagementObject rawCpu)
        {
            _rawCpu = rawCpu;
        }
        public int CPUUsage { get; set; }
        public Guid SystemName { get; set; }
    }
}
