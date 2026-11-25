using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqLab5.Models;

/// <summary>Таблица «Магазин», ID вида «Р1».</summary>
public class Store
{
    public string Id { get; }

    public string District { get; }

    public string Address { get; }

    public Store(string id, string district, string address)
    {
        Id = id;
        District = district;
        Address = address;
    }

    public override string ToString()
    {
        return $"{Id} | {District} | {Address}";
    }
}