using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LinqLab5.Models;

namespace LinqLab5.Extensions;

/// <summary>Расширяющие методы (лекция 5).</summary>
public static class AppExtensions
{
    public static string ToText(this OperationType type)
    {
        return type switch
        {
            OperationType.Receipt => "Поступление",
            OperationType.Sale => "Продажа",
            OperationType.Return => "Возврат",
            _ => "Неизвестно",
        };
    }

    public static OperationType ToOperationType(
        this string text)
    {
        return text.Trim().ToLowerInvariant() switch
        {
            "поступление" => OperationType.Receipt,
            "продажа" => OperationType.Sale,
            "возврат" => OperationType.Return,
            _ => throw new FormatException(
                $"Неизвестный тип операции: {text}"),
        };
    }

    public static string ToMoney(this decimal value)
    {
        return value.ToString("N2") + " руб.";
    }

    public static string ToRu(this DateOnly date)
    {
        return date.ToString("dd.MM.yyyy");
    }

    public static string ToCardText(this bool? card)
    {
        return card switch
        {
            true => "Да",
            false => "Нет",
            null => "-",
        };
    }
}
