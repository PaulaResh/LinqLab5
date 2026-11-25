using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LinqLab5.Extensions;

namespace LinqLab5.Models;

/// <summary>Таблица «Товар».</summary>
public class Product
{
    public string Article { get; }

    public int CategoryId { get; }

    public string Name { get; }

    public decimal PurchasePrice { get; }

    public decimal SalePrice { get; }

    public int DiscountPercent { get; }

    public Product(
        string article,
        int categoryId,
        string name,
        decimal purchasePrice,
        decimal salePrice,
        int discountPercent)
    {
        Article = article;
        CategoryId = categoryId;
        Name = name;
        PurchasePrice = purchasePrice;
        SalePrice = salePrice;
        DiscountPercent = discountPercent;
    }

    /// <summary>Цена продажи с учётом карты.</summary>
    public decimal PriceWithCard(bool hasCard)
    {
        return hasCard
            ? SalePrice * (100 - DiscountPercent) / 100
            : SalePrice;
    }

    public override string ToString()
    {
        return $"{Article} | кат.{CategoryId} | {Name} | " +
            $"закуп {PurchasePrice.ToMoney()} | " +
            $"продажа {SalePrice.ToMoney()} | " +
            $"скидка {DiscountPercent}%";
    }
}