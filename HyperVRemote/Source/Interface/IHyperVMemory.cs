using System;

namespace HyperVRemote.Source.Interface
{
    public interface IHyperVMemory
    {
        int AmountUsed { get; set; }
        Guid SystemName { get; set; }
    }
}