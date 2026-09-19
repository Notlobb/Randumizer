using System;
using System.Collections.Generic;

public class BankAddressAllocator
{
    private class BankRange
    {
        public int Current;
        public int End;
    }

    private readonly Dictionary<int, BankRange> banks = new Dictionary<int, BankRange>();

    public void AddBank(int bank, int start, int end = 0xc000)
    {
        if (start >= end)
            throw new ArgumentException("Invalid bank range");

        if (!banks.TryAdd(bank, new BankRange { Current = start, End = end }))
            throw new ArgumentException($"Bank {bank} already exists");
    }

    public ushort GetAddress(int bank)
    {
        if (!banks.TryGetValue(bank, out var range))
            throw new ArgumentException($"Bank {bank} not configured");

        // banks with base $c000 will have one-past-end bigger than ushort::max
        if (range.Current == 0x10000)
            throw new InvalidOperationException($"Bank {bank} is full");

        // overflows caught by SetAddress, so this value will always be valid
        return (ushort)range.Current;
    }

    public void SetAddress(int bank, int address)
    {
        if (!banks.TryGetValue(bank, out var range))
            throw new ArgumentException($"Bank {bank} not configured");

        if (address > range.End)
            throw new InvalidOperationException($"Address ${address:X} exceeds bank {bank} range");

        range.Current = address;
    }
}
