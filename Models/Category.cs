using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqLab5.Models;

/// <summary>Таблица «Категория».</summary>
public class Category
{
    public int Id { get; }

    public string Name { get; }

    public string AgeLimit { get; }

    public Category(int id, string name, string ageLimit)
    {
        Id = id;
        Name = name;
        AgeLimit = ageLimit;
    }

    public override string ToString()
    {
        return $"{Id} | {Name} | {AgeLimit}";
    }
}
