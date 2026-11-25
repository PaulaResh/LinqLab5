using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using LinqLab5.Extensions;
using LinqLab5.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace LinqLab5.Data;

/// <summary>Чтение и запись Excel через NPOI.</summary>
public static class ExcelStorage
{
    private const string MovesSheet = "Движение товаров";
    private const string StoresSheet = "Магазин";
    private const string ProductsSheet = "Товар";
    private const string CategoriesSheet = "Категория";

    public static Database Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "Файл не найден.", path);
        }

        using var fs = new FileStream(
            path, FileMode.Open, FileAccess.Read);
        IWorkbook book = WorkbookFactory.Create(fs);
        var db = new Database();

        ReadRows(book, CategoriesSheet, db.Categories,
            r => new Category(
                GetInt(r, 0), GetText(r, 1), GetText(r, 2)));

        ReadRows(book, StoresSheet, db.Stores,
            r => new Store(
                GetText(r, 0), GetText(r, 1), GetText(r, 2)));

        ReadRows(book, ProductsSheet, db.Products,
            r => new Product(
                GetText(r, 0), GetInt(r, 1), GetText(r, 2),
                GetDecimal(r, 3), GetDecimal(r, 4),
                GetPercent(r, 5)));

        ReadRows(book, MovesSheet, db.Movements,
            r => new Movement(
                GetInt(r, 0), GetDate(r, 1), GetText(r, 2),
                GetText(r, 3),
                GetText(r, 4).ToOperationType(),
                GetInt(r, 5), GetCard(r, 6)));

        db.IsLoaded = true;
        return db;
    }

    public static void Save(Database db, string path)
    {
        string ext = Path.GetExtension(path);
        ext = ext.ToLowerInvariant();
        if (ext is not (".xls" or ".xlsx"))
        {
            throw new ArgumentException(
                "Нужен файл .xls или .xlsx.");
        }

        IWorkbook book = ext == ".xlsx"
            ? new XSSFWorkbook()
            : new HSSFWorkbook();
        ICellStyle dateStyle = book.CreateCellStyle();
        dateStyle.DataFormat = book.CreateDataFormat()
            .GetFormat("dd.mm.yyyy");

        WriteRows(
            book, CategoriesSheet,
            new[] { "ID", "Наименование",
                "Возрастное ограничение" },
            db.Categories,
            (row, x) =>
            {
                row.CreateCell(0).SetCellValue(x.Id);
                row.CreateCell(1).SetCellValue(x.Name);
                row.CreateCell(2).SetCellValue(x.AgeLimit);
            });

        WriteRows(
            book, StoresSheet,
            new[] { "ID", "Район", "Адрес" },
            db.Stores,
            (row, x) =>
            {
                row.CreateCell(0).SetCellValue(x.Id);
                row.CreateCell(1).SetCellValue(x.District);
                row.CreateCell(2).SetCellValue(x.Address);
            });

        WriteRows(
            book, ProductsSheet,
            new[] { "Артикул", "ID категории",
                "Наименование товара",
                "Цена закупки при поступлении, руб",
                "Цена продажи без учёта скидки, руб",
                "Скидка при наличии карты клиента, %" },
            db.Products,
            (row, x) =>
            {
                row.CreateCell(0).SetCellValue(x.Article);
                row.CreateCell(1).SetCellValue(x.CategoryId);
                row.CreateCell(2).SetCellValue(x.Name);
                row.CreateCell(3).SetCellValue(
                    (double)x.PurchasePrice);
                row.CreateCell(4).SetCellValue(
                    (double)x.SalePrice);
                row.CreateCell(5).SetCellValue(
                    x.DiscountPercent + "%");
            });

        WriteRows(
            book, MovesSheet,
            new[] { "ID операции", "Дата", "ID магазина",
                "Артикул", "Тип операции",
                "Количество упаковок, шт",
                "Наличие карты клиента" },
            db.Movements,
            (row, x) =>
            {
                row.CreateCell(0).SetCellValue(x.OperationId);
                ICell dc = row.CreateCell(1);
                var dt = x.Date.ToDateTime(
                    TimeOnly.MinValue);
                dc.SetCellValue(dt);
                dc.CellStyle = dateStyle;
                row.CreateCell(2).SetCellValue(x.StoreId);
                row.CreateCell(3).SetCellValue(x.Article);
                row.CreateCell(4).SetCellValue(
                    x.Type.ToText());
                row.CreateCell(5).SetCellValue(
                    x.PackageCount);
                row.CreateCell(6).SetCellValue(
                    x.HasClientCard.ToCardText());
            });

        using var fs = new FileStream(
            path, FileMode.Create, FileAccess.Write);
        book.Write(fs);
    }

    /// <summary>Строка без единого значения (мусор).</summary>
    private static bool IsEmptyRow(IRow row)
    {
        for (int c = 0; c < row.LastCellNum; c++)
        {
            if (GetText(row, c).Length > 0)
            {
                return false;
            }
        }

        return true;
    }

    private static void ReadRows<T>(
        IWorkbook book,
        string name,
        List<T> list,
        Func<IRow, T> create)
    {
        ISheet? sheet = book.GetSheet(name);
        if (sheet is null)
        {
            throw new InvalidDataException(
                $"Нет листа \"{name}\".");
        }

        for (int i = 1; i <= sheet.LastRowNum; i++)
        {
            IRow? row = sheet.GetRow(i);
            if (row is null || IsEmptyRow(row))
            {
                continue;
            }

            try
            {
                list.Add(create(row));
            }
            catch (FormatException ex)
            {
                throw new InvalidDataException(
                    $"Лист \"{name}\", строка {i + 1}: " +
                    ex.Message);
            }
        }
    }

    private static void WriteRows<T>(
        IWorkbook book,
        string name,
        string[] headers,
        IEnumerable<T> items,
        Action<IRow, T> fill)
    {
        ISheet sheet = book.CreateSheet(name);
        IRow head = sheet.CreateRow(0);
        for (int c = 0; c < headers.Length; c++)
        {
            head.CreateCell(c).SetCellValue(headers[c]);
        }

        int r = 1;
        foreach (T item in items)
        {
            fill(sheet.CreateRow(r), item);
            r++;
        }
    }

    private static string GetText(IRow row, int col)
    {
        ICell? cell = row.GetCell(col);
        if (cell is null)
        {
            return string.Empty;
        }

        return cell.CellType switch
        {
            CellType.String => cell.StringCellValue,
            CellType.Numeric => cell.NumericCellValue
                .ToString(CultureInfo.InvariantCulture),
            _ => cell.ToString() ?? string.Empty,
        };
    }

    private static int GetInt(IRow row, int col)
    {
        ICell? cell = row.GetCell(col);
        if (cell is not null &&
            cell.CellType == CellType.Numeric)
        {
            return (int)cell.NumericCellValue;
        }

        if (int.TryParse(GetText(row, col), out int value))
        {
            return value;
        }

        throw new FormatException(
            $"Ожидалось целое: \"{GetText(row, col)}\"");
    }

    private static decimal GetDecimal(IRow row, int col)
    {
        ICell? cell = row.GetCell(col);
        if (cell is not null &&
            cell.CellType == CellType.Numeric)
        {
            return (decimal)cell.NumericCellValue;
        }

        string text = GetText(row, col).Replace(',', '.');
        if (decimal.TryParse(
            text,
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out decimal value))
        {
            return value;
        }

        throw new FormatException(
            $"Ожидалась цена: \"{text}\"");
    }

    private static int GetPercent(IRow row, int col)
    {
        ICell? cell = row.GetCell(col);
        if (cell is null)
        {
            throw new FormatException("Пустая скидка.");
        }

        if (cell.CellType == CellType.Numeric)
        {
            double v = cell.NumericCellValue;
            string fmt =
                cell.CellStyle.GetDataFormatString();
            if (fmt.Contains('%'))
            {
                v *= 100;
            }

            return (int)Math.Round(v);
        }

        string text = GetText(row, col).Trim();
        text = text.Replace("%", "").Replace(',', '.');
        if (decimal.TryParse(
            text,
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out decimal d))
        {
            return (int)Math.Round(d);
        }

        throw new FormatException($"Скидка: \"{text}\"");
    }

    private static DateOnly GetDate(IRow row, int col)
    {
        ICell? cell = row.GetCell(col);
        if (cell is not null &&
            cell.CellType == CellType.Numeric &&
            DateUtil.IsCellDateFormatted(cell))
        {
            DateTime dt = cell.DateCellValue
                ?? throw new FormatException("Нет даты.");
            return DateOnly.FromDateTime(dt);
        }

        string[] formats =
        {
            "dd.MM.yyyy", "d.M.yyyy", "M/d/yy",
        };
        if (DateOnly.TryParseExact(
            GetText(row, col),
            formats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateOnly date))
        {
            return date;
        }

        throw new FormatException("Неверная дата.");
    }

    private static bool? GetCard(IRow row, int col)
    {
        string text = GetText(row, col).Trim();
        text = text.ToLowerInvariant();
        return text switch
        {
            "да" => true,
            "нет" => false,
            "-" or "" => null,
            _ => throw new FormatException(
                $"Карта клиента: \"{text}\""),
        };
    }
}
