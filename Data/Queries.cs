using LinqLab5.Extensions;
using LinqLab5.Models;

namespace LinqLab5.Data;

/// <summary>Четыре запроса LINQ (пункт 5).</summary>
public static class Queries
{
    /// <summary>
    /// 5.1. Одна таблица, перечень: товары с ценой
    /// продажи не ниже заданной, по убыванию цены.
    /// </summary>
    public static IEnumerable<string> ExpensiveProducts(
        Database db, decimal minPrice)
    {
        var query =
            from p in db.Products
            where p.SalePrice >= minPrice
            orderby p.SalePrice descending
            select new { p.Article, p.Name, p.SalePrice };

        return query.Select(x =>
            $"{x.Article} | {x.Name} | " +
            x.SalePrice.ToMoney());
    }

    /// <summary>
    /// 5.2. Две таблицы, одно значение: сколько
    /// упаковок поступило в магазины района.
    /// </summary>
    public static int ReceivedPackages(
        Database db, string district)
    {
        var query =
            from m in db.Movements
            join s in db.Stores on m.StoreId equals s.Id
            where m.Type == OperationType.Receipt &&
                s.District == district
            select m.PackageCount;

        return query.Sum();
    }

    /// <summary>
    /// 5.3а. Три таблицы, перечень: журнал продаж
    /// за период (дата, район, товар, сумма).
    /// </summary>
    public static IEnumerable<string> SalesJournal(
        Database db, DateOnly start, DateOnly end)
    {
        var query =
            from m in db.Movements
            join s in db.Stores on m.StoreId equals s.Id
            join p in db.Products
                on m.Article equals p.Article
            where m.Type == OperationType.Sale &&
                m.Date >= start && m.Date <= end
            orderby m.Date, s.District
            select new
            {
                m.Date,
                s.District,
                p.Name,
                m.PackageCount,
                Sum = m.PackageCount *
                    p.PriceWithCard(m.HasClientCard == true),
            };

        return query.Select(x =>
            $"{x.Date.ToRu()} | {x.District} | {x.Name} | " +
            $"{x.PackageCount} уп. | {x.Sum.ToMoney()}");
    }

    /// <summary>
    /// 5.3б. Три таблицы, одно значение: общая
    /// стоимость продаж категории в районе за период.
    /// </summary>
    public static decimal SoldSum(
        Database db,
        string district,
        int categoryId,
        DateOnly start,
        DateOnly end,
        bool applyDiscount)
    {
        var query =
            from m in db.Movements
            join s in db.Stores on m.StoreId equals s.Id
            join p in db.Products
                on m.Article equals p.Article
            where m.Type == OperationType.Sale &&
                s.District == district &&
                p.CategoryId == categoryId &&
                m.Date >= start && m.Date <= end
            select applyDiscount
                ? m.PackageCount *
                    p.PriceWithCard(m.HasClientCard == true)
                : m.PackageCount * p.SalePrice;

        return query.Sum();
    }
}