// <copyright file="JsonDataHelper.cs" company="MaaAssistantArknights">
// Part of the MaaWpfGui project, maintained by the MaaAssistantArknights team (Maa Team)
// Copyright (C) 2021-2025 MaaAssistantArknights Contributors
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License v3.0 only as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY
// </copyright>

#nullable enable

using System;
using System.IO;
using Newtonsoft.Json;
using Serilog;

namespace MaaWpfGui.Helper
{
    public static class JsonDataHelper
    {
        private static readonly string _dataDir = "data";
        private static readonly object _lock = new();
        private static readonly ILogger _logger = Log.ForContext("SourceContext", "JsonDataHelper");

        /// <summary>
        /// 从 data/{key}.json 读取对象，如果不存在则返回 defaultValue
        /// </summary>
        /// <typeparam name="T">要获取的数据类型</typeparam>
        /// <param name="key">文件名</param>
        /// <param name="defaultValue">数据类型</param>
        /// <param name="dataDir">目标文件夹</param>
        /// <returns>反序列化数据</returns>
        public static T? Get<T>(string key, T? defaultValue = default, string? dataDir = null)
        {
            var filePath = Path.Combine(dataDir ?? _dataDir, $"{key}.json");

            if (!File.Exists(filePath))
            {
                return defaultValue;
            }

            try
            {
                var json = File.ReadAllText(filePath);
                if (typeof(T) == typeof(string))
                {
                    return (T)(object)json;
                }

                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to load data file for key {key}");
                return defaultValue;
            }
        }

        /// <summary>
        /// 将对象写入 data/{key}.json
        /// </summary>
        /// <typeparam name="T">要写入的数据类型</typeparam>
        /// <param name="key">文件名</param>
        /// <param name="value">数据</param>
        /// <param name="dataDir">目标文件夹</param>
        /// <returns>是否设置成功</returns>
        public static bool Set<T>(string key, T value, string? dataDir = null)
        {
            var filePath = Path.Combine(dataDir ?? _dataDir, $"{key}.json");

            lock (_lock)
            {
                try
                {
                    Directory.CreateDirectory(_dataDir);
                    var json = JsonConvert.SerializeObject(value, Formatting.Indented);
                    File.WriteAllText(filePath, json);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Failed to save data file for key {key}");
                    return false;
                }
            }
        }

        /// <summary>
        /// 删除 data/{key}.json
        /// </summary>
        /// <param name="key">文件名</param>
        /// <returns>是否成功删除</returns>
        public static bool Delete(string key)
        {
            var filePath = Path.Combine(_dataDir, $"{key}.json");

            try
            {
                if (!File.Exists(filePath))
                {
                    return false;
                }

                File.Delete(filePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to delete data file for key {key}");
                return false;
            }
        }

        /// <summary>
        /// 判断 data/{key}.json 是否存在
        /// </summary>
        /// <param name="key">文件名</param>
        /// <returns>是否存在</returns>
        public static bool Exists(string key)
        {
            var filePath = Path.Combine(_dataDir, $"{key}.json");
            return File.Exists(filePath);
        }
    }
}
