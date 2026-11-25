using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LinqLab5.Models;

namespace LinqLab5.Data;

/// <summary>БД и CRUD через LINQ + проверки.</summary>
public class Database
{
    public List<Category> Categories { get; } = new();

    public List<Store> Stores { get; } = new();

    public List<Product> Products { get; } = new();

    public List<Movement> Movements { get; } = new();

    public bool IsLoaded { get; set; }

    /// <summary>Событие об изменении данных.</summary>
    public event Action<string>? Changed;

    public bool HasCategory(int id) =>
        Categories.Any(x => x.Id == id);

    public bool HasStore(string id) =>
        Stores.Any(x => x.Id == id);

    public bool HasArticle(string article) =>
        Products.Any(x => x.Article == article);

    public List<string> DistrictList() =>
        Stores.Select(x => x.District)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

    public void AddCategory(Category item)
    {
        if (HasCategory(item.Id))
        {
            throw new InvalidOperationException(
                $"Категория {item.Id} уже существует.");
        }

        Categories.Add(item);
        Changed?.Invoke($"Добавлена категория {item.Id}.");
    }

    public void AddStore(Store item)
    {
        if (HasStore(item.Id))
        {
            throw new InvalidOperationException(
                $"Магазин {item.Id} уже существует.");
        }

        Stores.Add(item);
        Changed?.Invoke($"Добавлен магазин {item.Id}.");
    }

    public void AddProduct(Product item)
    {
        if (HasArticle(item.Article))
        {
            throw new InvalidOperationException(
                $"Товар {item.Article} уже существует.");
        }

        if (!HasCategory(item.CategoryId))
        {
            throw new InvalidOperationException(
                $"Категория {item.CategoryId} не найдена.");
        }

        if (item.PurchasePrice < 0 || item.SalePrice < 0 ||
            item.DiscountPercent is < 0 or > 100)
        {
            throw new ArgumentException(
                "Неверные цены или скидка.");
        }

        Products.Add(item);
        Changed?.Invoke($"Добавлен товар {item.Article}.");
    }

    public void AddMovement(Movement item)
    {
        if (Movements.Any(
                x => x.OperationId == item.OperationId))
        {
            throw new InvalidOperationException(
                $"Операция {item.OperationId} уже есть.");
        }

        if (!HasStore(item.StoreId))
        {
            throw new InvalidOperationException(
                $"Магазин {item.StoreId} не найден.");
        }

        if (!HasArticle(item.Article))
        {
            throw new InvalidOperationException(
                $"Товар {item.Article} не найден.");
        }

        if (item.PackageCount <= 0)
        {
            throw new ArgumentException(
                "Количество должно быть больше нуля.");
        }

        if (item.Type == OperationType.Receipt &&
            item.HasClientCard is not null)
        {
            throw new ArgumentException(
                "Для поступления карта не указана (-).");
        }

        if (item.Type != OperationType.Receipt &&
            item.HasClientCard is null)
        {
            throw new ArgumentException(
                "Для продажи/возврата нужна карта Да/Нет.");
        }

        Movements.Add(item);
        Changed?.Invoke($"Добавлена операция {item.OperationId}.");
    }

    public void DeleteCategory(int id)
    {
        Category? item = Categories.FirstOrDefault(
            x => x.Id == id);
        if (item is null)
        {
            throw new KeyNotFoundException(
                $"Категория {id} не найдена.");
        }

        int used = Products.Count(x => x.CategoryId == id);
        if (used > 0)
        {
            throw new InvalidOperationException(
                $"Есть товары этой категории: {used}.");
        }

        Categories.Remove(item);
        Changed?.Invoke($"Удалена категория {id}.");
    }

    public void DeleteStore(string id)
    {
        Store? item = Stores.FirstOrDefault(x => x.Id == id);
        if (item is null)
        {
            throw new KeyNotFoundException(
                $"Магазин {id} не найден.");
        }

        int used = Movements.Count(x => x.StoreId == id);
        if (used > 0)
        {
            throw new InvalidOperationException(
                $"Есть операции этого магазина: {used}.");
        }

        Stores.Remove(item);
        Changed?.Invoke($"Удалён магазин {id}.");
    }

    public void DeleteProduct(string article)
    {
        Product? item = Products.FirstOrDefault(
            x => x.Article == article);
        if (item is null)
        {
            throw new KeyNotFoundException(
                $"Товар {article} не найден.");
        }

        int used = Movements.Count(
            x => x.Article == article);
        if (used > 0)
        {
            throw new InvalidOperationException(
                $"Есть операции этого товара: {used}.");
        }

        Products.Remove(item);
        Changed?.Invoke($"Удалён товар {article}.");
    }

    public void DeleteMovement(int id)
    {
        Movement? item = Movements.FirstOrDefault(
            x => x.OperationId == id);
        if (item is null)
        {
            throw new KeyNotFoundException(
                $"Операция {id} не найдена.");
        }

        Movements.Remove(item);
        Changed?.Invoke($"Удалена операция {id}.");
    }
}
