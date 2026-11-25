using LinqLab5.Data;
using LinqLab5.Extensions;
using LinqLab5.Models;
using LinqLab5.UI;

namespace LinqLab5;

/// <summary>Меню с выбором номера задания.</summary>
public static class Program
{
    private static Database _db = new();
    private static int _changes;

    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Attach(_db);
        while (true)
        {
            ShowMenu();
            int task = ConsoleHelper.ReadTask(6);
            if (task == -1)
            {
                if (ConfirmExit())
                {
                    break;
                }

                continue;
            }

            try
            {
                RunTask(task);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    private static void ShowMenu()
    {
        Console.WriteLine();
        Console.WriteLine("=== Детские товары: меню ===");
        Console.WriteLine("1. Чтение БД из Excel");
        Console.WriteLine("2. Просмотр БД");
        Console.WriteLine("3. Удаление по ключу");
        Console.WriteLine("4. Добавление записи");
        Console.WriteLine("5. Запросы LINQ (4 шт.)");
        Console.WriteLine("6. Сохранение БД в Excel");
        Console.WriteLine("Выход: только exit (q/выход)");
    }

    private static bool ConfirmExit()
    {
        if (_changes > 0)
        {
            Console.WriteLine(
                $"Несохранённых изменений: {_changes}.");
            if (ConsoleHelper.ReadBool(
                "Сохранить перед выходом? (Да/Нет): "))
            {
                Save();
            }
        }

        Console.WriteLine("До свидания!");
        return true;
    }

    private static void RunTask(int task)
    {
        switch (task)
        {
            case 1:
                Load();
                break;
            case 2:
                View();
                break;
            case 3:
                Delete();
                break;
            case 4:
                Add();
                break;
            case 5:
                RunQueries();
                break;
            case 6:
                Save();
                break;
        }
    }

    private static void Attach(Database db)
    {
        db.Changed += OnChanged;
        db.Changed += _ => _changes++;
    }

    private static void OnChanged(string message)
    {
        Console.WriteLine($"[БД] {message}");
    }

    private static void RequireData()
    {
        if (!_db.IsLoaded)
        {
            throw new InvalidOperationException(
                "Сначала загрузите БД (задание 1).");
        }
    }

    private static void Load()
    {
        string path = ConsoleHelper.ReadLine(
            "Файл (Enter = LR5-var1.xls): ");
        if (path.Length == 0)
        {
            path = "LR5-var1.xls";
        }

        _db = ExcelStorage.Load(path);
        Attach(_db);
        _changes = 0;
        Console.WriteLine(
            $"Категорий: {_db.Categories.Count}, " +
            $"магазинов: {_db.Stores.Count}, " +
            $"товаров: {_db.Products.Count}, " +
            $"движений: {_db.Movements.Count}.");
    }

    private static void View()
    {
        RequireData();
        Console.WriteLine(
            "1. Категория  2. Магазин  3. Товар");
        Console.WriteLine(
            "4. Движение товаров  5. Все");
        int c = ConsoleHelper.ReadSub(5);
        if (c is 1 or 5)
        {
            Console.WriteLine("-- Категория --");
            ConsoleHelper.PrintLines(
                _db.Categories.Select(x => x.ToString()));
        }

        if (c is 2 or 5)
        {
            Console.WriteLine("-- Магазин --");
            ConsoleHelper.PrintLines(
                _db.Stores.Select(x => x.ToString()));
        }

        if (c is 3 or 5)
        {
            Console.WriteLine("-- Товар --");
            ConsoleHelper.PrintLines(
                _db.Products.Select(x => x.ToString()));
        }

        if (c is 4 or 5)
        {
            Console.WriteLine("-- Движение товаров --");
            ConsoleHelper.PrintLines(
                _db.Movements.Select(x => x.ToString()));
        }
    }

    private static void Delete()
    {
        RequireData();
        Console.WriteLine(
            "1. Категория  2. Магазин  3. Товар  " +
            "4. Движение");
        int c = ConsoleHelper.ReadSub(4);
        switch (c)
        {
            case 1:
                _db.DeleteCategory(ReadCategoryKey());
                break;
            case 2:
                _db.DeleteStore(ReadStoreKey());
                break;
            case 3:
                _db.DeleteProduct(ReadArticleKey());
                break;
            case 4:
                _db.DeleteMovement(ConsoleHelper.ReadInt(
                    "ID операции: ", 1, int.MaxValue));
                break;
        }
    }

    private static void Add()
    {
        RequireData();
        Console.WriteLine(
            "1. Категория  2. Магазин  3. Товар  " +
            "4. Движение");
        int c = ConsoleHelper.ReadSub(4);
        switch (c)
        {
            case 1:
                AddCategory();
                break;
            case 2:
                AddStore();
                break;
            case 3:
                AddProduct();
                break;
            case 4:
                AddMovement();
                break;
        }
    }

    private static void AddCategory()
    {
        int id = ConsoleHelper.ReadInt(
            "ID: ", 1, int.MaxValue);
        string name = ConsoleHelper.ReadText(
            "Наименование: ");
        string age = ConsoleHelper.ReadAge(
            "Возрастное ограничение: ");
        _db.AddCategory(new Category(id, name, age));
    }

    private static void AddStore()
    {
        string id = ConsoleHelper.ReadStoreId("ID: ");
        string district = ConsoleHelper.ReadText(
            "Район: ");
        string address = ConsoleHelper.ReadText(
            "Адрес: ");
        _db.AddStore(new Store(id, district, address));
    }

    private static void AddProduct()
    {
        string article = ConsoleHelper.ReadArticle(
            "Артикул: ");
        int catId = ReadCategoryKey();
        string name = ConsoleHelper.ReadText(
            "Наименование товара: ");
        decimal buy = ConsoleHelper.ReadDecimal(
            "Цена закупки: ", 0, 1_000_000_000);
        decimal sell = ConsoleHelper.ReadDecimal(
            "Цена продажи: ", 0, 1_000_000_000);
        int disc = ConsoleHelper.ReadPercent(
            "Скидка по карте, %: ");
        _db.AddProduct(new Product(
            article, catId, name, buy, sell, disc));
    }

    private static void AddMovement()
    {
        int id = ConsoleHelper.ReadInt(
            "ID операции: ", 1, int.MaxValue);
        DateOnly date = ConsoleHelper.ReadDate(
            "Дата (дд.мм.гггг): ");
        string storeId = ReadStoreKey();
        string article = ReadArticleKey();
        Console.WriteLine(
            "1. Поступление  2. Продажа  3. Возврат");
        int t = ConsoleHelper.ReadInt(
            "Тип операции: ", 1, 3);
        var type = (OperationType)(t - 1);
        int count = ConsoleHelper.ReadInt(
            "Количество упаковок: ", 1, int.MaxValue);
        bool? card = null;
        if (type != OperationType.Receipt)
        {
            card = ConsoleHelper.ReadBool(
                "Карта клиента (Да/Нет): ");
        }
        else
        {
            Console.WriteLine("Карта клиента: -");
        }

        _db.AddMovement(new Movement(
            id, date, storeId, article, type, count, card));
    }

    private static void RunQueries()
    {
        RequireData();
        Console.WriteLine(
            "1. Товары дороже заданной цены");
        Console.WriteLine(
            "2. Поступления в район (упаковок)");
        Console.WriteLine(
            "3. Журнал продаж за период");
        Console.WriteLine(
            "4. Стоимость продаж: район+категория");
        int c = ConsoleHelper.ReadSub(4);
        switch (c)
        {
            case 1:
                Query1();
                break;
            case 2:
                Query2();
                break;
            case 3:
                Query3();
                break;
            case 4:
                Query4();
                break;
        }
    }

    private static void Query1()
    {
        decimal min = ConsoleHelper.ReadDecimal(
            "Минимальная цена продажи: ", 0,
            1_000_000_000);
        List<string> rows = Queries
            .ExpensiveProducts(_db, min)
            .ToList();
        ConsoleHelper.PrintLines(rows);
    }

    private static void Query2()
    {
        string district = ReadDistrictKey();
        int total = Queries.ReceivedPackages(
            _db, district);
        Console.WriteLine(
            $"Поступило в район \"{district}\": " +
            $"{total} уп.");
    }

    private static void Query3()
    {
        var (from, to) = ConsoleHelper.ReadPeriod();
        List<string> rows = Queries
            .SalesJournal(_db, from, to)
            .ToList();
        ConsoleHelper.PrintLines(rows);
    }

    private static void Query4()
    {
        string district = ReadDistrictKey();
        int categoryId = ReadCategoryKey();
        var (from, to) = ConsoleHelper.ReadPeriod();
        bool disc = ConsoleHelper.ReadBool(
            "Учитывать скидку по карте? (Да/Нет): ");
        decimal sum = Queries.SoldSum(
            _db, district, categoryId, from, to, disc);
        Console.WriteLine(
            $"Общая стоимость: {sum.ToMoney()}");
    }

    private static int ReadCategoryKey()
    {
        ConsoleHelper.PrintLines(
            _db.Categories.Select(x => x.ToString()), 0);
        while (true)
        {
            int id = ConsoleHelper.ReadInt(
                "ID категории: ", 1, int.MaxValue);
            if (_db.HasCategory(id))
            {
                return id;
            }

            Console.WriteLine("Нет такой категории.");
        }
    }

    private static string ReadStoreKey()
    {
        while (true)
        {
            string id = ConsoleHelper.ReadStoreId(
                "ID магазина: ");
            if (_db.HasStore(id))
            {
                return id;
            }

            Console.WriteLine("Нет такого магазина.");
        }
    }

    private static string ReadArticleKey()
    {
        while (true)
        {
            string a = ConsoleHelper.ReadArticle(
                "Артикул: ");
            if (_db.HasArticle(a))
            {
                return a;
            }

            Console.WriteLine("Нет такого товара.");
        }
    }

    private static string ReadDistrictKey()
    {
        List<string> list = _db.DistrictList();
        Console.WriteLine(
            "Районы: " + string.Join(", ", list));
        while (true)
        {
            string d = ConsoleHelper.ReadText("Район: ");
            if (list.Contains(d))
            {
                return d;
            }

            Console.WriteLine("Нет такого района.");
        }
    }

    private static void Save()
    {
        RequireData();
        string path = ConsoleHelper.ReadLine(
            "Файл (Enter = result.xls): ");
        if (path.Length == 0)
        {
            path = "result.xls";
        }

        ExcelStorage.Save(_db, path);
        _changes = 0;
        Console.WriteLine("Сохранено.");
    }
}