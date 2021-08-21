using System;
namespace HyperVRemote.Source.Interface
{
    public interface IHyperVProcessor
    {
        int CPUUsage { get; set; }
        Guid SystemName { get; set; }
    }
}