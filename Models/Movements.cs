using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LinqLab5.Extensions;

namespace LinqLab5.Models;

/// <summary>Таблица «Движение товаров».</summary>
public class Movement
{
    public int OperationId { get; }

    public DateOnly Date { get; }

    public string StoreId { get; }

    public string Article { get; }

    public OperationType Type { get; }

    public int PackageCount { get; }

    public bool? HasClientCard { get; }

    public Movement(
        int operationId,
        DateOnly date,
        string storeId,
        string article,
        OperationType type,
        int packageCount,
        bool? hasClientCard)
    {
        OperationId = operationId;
        Date = date;
        StoreId = storeId;
        Article = article;
        Type = type;
        PackageCount = packageCount;
        HasClientCard = hasClientCard;
    }

    public override string ToString()
    {
        return $"{OperationId} | {Date.ToRu()} | " +
            $"{StoreId} | {Article} | {Type.ToText()} | " +
            $"{PackageCount} | {HasClientCard.ToCardText()}";
    }
}