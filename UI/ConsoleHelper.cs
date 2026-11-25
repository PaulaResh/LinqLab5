using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;
using System.Text.RegularExpressions;

namespace LinqLab5.UI;

/// <summary>Ввод с проверкой и вывод в консоль.</summary>
public static class ConsoleHelper
{
    public static string ReadLine(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    public static string ReadText(string prompt)
    {
        while (true)
        {
            string text = ReadLine(prompt);
            if (text.Length > 0)
            {
                return text;
            }

            Console.WriteLine("Ошибка: пустая строка.");
        }
    }

    public static int ReadInt(
        string prompt, int min, int max)
    {
        while (true)
        {
            string text = ReadLine(prompt);
            if (int.TryParse(text, out int v) &&
                v >= min && v <= max)
            {
                return v;
            }

            Console.WriteLine(
                $"Ошибка: нужно целое {min}..{max}.");
        }
    }

    public static decimal ReadDecimal(
        string prompt, decimal min, decimal max)
    {
        while (true)
        {
            string text = ReadLine(prompt)
                .Replace(',', '.');
            if (decimal.TryParse(
                text,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out decimal v) && v >= min && v <= max)
            {
                return v;
            }

            Console.WriteLine(
                $"Ошибка: число {min}..{max}.");
        }
    }

    public static DateOnly ReadDate(string prompt)
    {
        string[] formats = { "dd.MM.yyyy", "d.M.yyyy" };
        while (true)
        {
            string text = ReadLine(prompt);
            if (DateOnly.TryParseExact(
                text,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateOnly d))
            {
                return d;
            }

            Console.WriteLine(
                "Ошибка: формат дд.мм.гггг.");
        }
    }

    public static bool ReadBool(string prompt)
    {
        while (true)
        {
            string t = ReadLine(prompt).ToLowerInvariant();
            if (t is "да" or "д")
            {
                return true;
            }

            if (t is "нет" or "н")
            {
                return false;
            }

            Console.WriteLine("Ошибка: введите Да/Нет.");
        }
    }

    public static string ReadPattern(
        string prompt, string pattern, string hint)
    {
        while (true)
        {
            string text = ReadLine(prompt);
            if (Regex.IsMatch(text, pattern))
            {
                return text;
            }

            Console.WriteLine($"Ошибка: {hint}");
        }
    }

    /// <summary>ID магазина: Р1..Р999, лат. P меняем.</summary>
    public static string ReadStoreId(string prompt)
    {
        while (true)
        {
            string t = ReadLine(prompt).ToUpperInvariant();
            t = t.Replace('P', 'Р');
            if (Regex.IsMatch(t, "^Р\\d{1,3}$"))
            {
                return t;
            }

            Console.WriteLine(
                "Ошибка: ID вида Р1, Р15.");
        }
    }

    public static string ReadArticle(string prompt)
    {
        return ReadPattern(
            prompt, "^\\d{1,12}$", "артикул: только цифры");
    }

    public static string ReadAge(string prompt)
    {
        return ReadPattern(
            prompt, "^\\d{1,2}\\+$", "возраст: 0+, 6+, 12+");
    }

    public static int ReadPercent(string prompt)
    {
        return ReadInt(prompt, 0, 100);
    }

    public static (DateOnly From, DateOnly To) ReadPeriod()
    {
        while (true)
        {
            DateOnly a = ReadDate("С даты (дд.мм.гггг): ");
            DateOnly b = ReadDate("По дату (дд.мм.гггг): ");
            if (b >= a)
            {
                return (a, b);
            }

            Console.WriteLine(
                "Ошибка: конец раньше начала.");
        }
    }

    /// <summary>Главное меню: 1..count или exit.</summary>
    public static int ReadTask(int count)
    {
        while (true)
        {
            string t = ReadLine("Номер задания: ");
            t = t.ToLowerInvariant();
            if (t is "exit" or "q" or "выход")
            {
                return -1;
            }

            if (int.TryParse(t, out int n) &&
                n >= 1 && n <= count)
            {
                return n;
            }

            Console.WriteLine(
                $"Ошибка: введите 1..{count} или exit.");
        }
    }

    /// <summary>Подменю: 1..count, 0 = назад.</summary>
    public static int ReadSub(int count)
    {
        while (true)
        {
            string t = ReadLine("Номер (0 = назад): ");
            t = t.ToLowerInvariant();
            if (t is "0" or "back" or "назад")
            {
                return 0;
            }

            if (int.TryParse(t, out int n) &&
                n >= 1 && n <= count)
            {
                return n;
            }

            Console.WriteLine(
                $"Ошибка: введите 1..{count} или 0.");
        }
    }

    public static void PrintLines(
        IEnumerable<string> lines, int page = 20)
    {
        int n = 0;
        foreach (string line in lines)
        {
            Console.WriteLine(line);
            n++;
            if (page > 0 && n % page == 0)
            {
                Console.Write("-- Enter: далее --");
                Console.ReadLine();
            }
        }

        Console.WriteLine($"Всего записей: {n}");
    }
}
