﻿using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace TimedScrapAPI
{
    public class ScrapEntity : TableEntity
    {
        public ScrapEntity(string rowKey)
        {
            RowKey = rowKey;
            PartitionKey = rowKey;
        }
        public ScrapEntity()
        {
        }
        public DateTime ScrapExacutedAt { get; set; }
        public bool Success { get; set; }
    }
}
