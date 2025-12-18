using System;
using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;

namespace BeveragePOS.Models
{
    public class DataService
    {
        //資料庫檔案名稱
        private const string DatabaseName = "pos.db";
        // SQLit 線字串
        private readonly string connectionString=$"Data Source={DatabaseName};Version=3;";
        /// <summary>
        /// 檢查資料庫檔案是否存在，如果不存在則建立
        /// </summary>
        public void InitializeDatabase()
        {
            bool needsCreation = !File.Exists(DatabaseName);

            if (needsCreation)
            {
                // 如果不存在，則建立新的資料庫檔案
                SQLiteConnection.CreateFile(DatabaseName);
            }

            // 無論如何都執行資料表檢查和建立
            CreateTable();

            // 如果是新建立的資料庫，插入測試資料
            if (needsCreation)
            {
                InsertSampleData();
            }
        }
        /// <summary>
        /// 建立 POS 系統所需的所有資料表結構 (MenuItem, Order, OrderItem)。
        /// </summary>
        private void CreateTable()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                // 1. 建立 MenuItem (菜單項目) 資料表
                string createMenuItemTableSql= @"
                CREATE TABLE IF NOT EXISTS MenuItem (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Price REAL NOT NULL, -- SQLite 用 REAL 代表小數
                        Category TEXT,
                        IsAvailable INTEGER NOT NULL -- SQLite 用 INTEGER (0=False, 1=True)
                    );";
                using (var command = new SQLiteCommand(createMenuItemTableSql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// 插入幾筆測試用的飲料資料
        /// </summary>
        private void InsertSampleData()
        {
            using(var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                //使用事務(Transaction) 提高插入效率並確保資料完整性
                using(var trasnaction = connection.BeginTransaction())
                {
                    string instetSql = @"INSERT INTO MenuItem (Name, Price, Category, IsAvailable) 
                                         VALUES (@name, @price, @category, @isAvailable)";
                    using (var command=new SQLiteCommand(instetSql, connection, trasnaction))
                    {
                        //範例資料清單
                        var items = new List<MenuItem>
                        {
                            new MenuItem { Name = "經典紅茶", Price = 30.00m, Category = "純茶", IsAvailable = true },
                            new MenuItem { Name = "珍珠奶茶", Price = 55.00m, Category = "奶茶", IsAvailable = true },
                            new MenuItem { Name = "檸檬綠茶", Price = 45.00m, Category = "特調", IsAvailable = true },
                            new MenuItem { Name = "冬瓜茶", Price = 35.00m, Category = "古早味", IsAvailable = true }
                        };
                        foreach (var item in items)
                        {
                            // 使用參數化查詢 (Parameterized Query) 防止 SQL 注入
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@name",item.Name);
                            command.Parameters.AddWithValue("@price",item.Price);
                            command.Parameters.AddWithValue("@category",item.Category);
                            command.Parameters.AddWithValue("@isAvailable", item.IsAvailable);

                        }

                    }
                    trasnaction.Commit();//確認執行
                }
            }
        }
        /// <summary>
        /// 從 menuItem 資料表讀取所有菜單項目
        /// </summary>
        /// <returns></returns>
        public List<MenuItem>GetMenuItems()
        {
            var menuItems= new List<MenuItem>();
            string selectSql = "SELECT Id, Name, Price, Category, IsAvailable FROM MenuItem WHERE IsAvailable = 1;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(selectSql, connection))
                {
                    // 執行查詢，返回一個 DataReader 物件
                    using (var reader = command.ExecuteReader())
                    {
                        //讀取逐筆資料
                        while (reader.Read())
                        {
                            var item = new MenuItem
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                // SQLite 的 REAL 轉換為 C# 的 decimal
                                Price = reader.GetDecimal(2),
                                Category = reader.IsDBNull(3) ? "其他" : reader.GetString(3),
                                // SQLite 的 INTEGER (1=True) 轉換為 C# 的 bool
                                IsAvailable = reader.GetInt32(4) == 1
                            };
                            menuItems.Add(item);
                        }
                    }
                }

            }
                return menuItems;
        }
        // (未來：GetMenuItems(), AddOrder() 等方法將會加在這裡)
    }
}
