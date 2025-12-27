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
            
            // 1. 如果檔案不存在，建立它
            if (!File.Exists(DatabaseName))
            {
                SQLiteConnection.CreateFile(DatabaseName);
            }

            // 2. 確保資料表存在
            CreateTable();

            // 3. 檢查是否需要插入測試資料 (如果資料庫是空的)
            if (IsTableEmpty())
            {
                InsertSampleData();
            }
        }
        // 新增一個輔助方法來檢查資料表是否為空
        private bool IsTableEmpty()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM MenuItem";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    long count = (long)command.ExecuteScalar();
                    return count == 0; // 如果數量為 0，回傳 true
                }
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
                // 2. 新增 Orders 表 (訂單主檔)
                // 記錄訂單日期 (OrderDate) 和總金額 (TotalAmount)
                string createOrderTableSql = @"
                CREATE TABLE IF NOT EXISTS Orders (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OrderDate DATETIME NOT NULL,
                    TotalAmount REAL NOT NULL
                );";

                // 3. 新增 OrderItems 表 (訂單明細)
                // 記錄每筆訂單裡面的飲料細節
                // 注意：我們這裡會「快照 (Snapshot)」存入 Name 和 Price，
                // 防止未來菜單改名或漲價後，舊的訂單紀錄也跟著變動。
                string createOrderItemTableSql = @"
                CREATE TABLE IF NOT EXISTS OrderItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OrderId INTEGER NOT NULL,
                    MenuItemName TEXT NOT NULL,
                    Price REAL NOT NULL,
                    Quantity INTEGER NOT NULL,
                    Subtotal REAL NOT NULL,
                    FOREIGN KEY(OrderId) REFERENCES Orders(Id)
                );";

                using (var command = new SQLiteCommand(createMenuItemTableSql, connection)) { command.ExecuteNonQuery(); }
                using (var command = new SQLiteCommand(createOrderTableSql, connection)) { command.ExecuteNonQuery(); }
                using (var command = new SQLiteCommand(createOrderItemTableSql, connection)) { command.ExecuteNonQuery(); }
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

                            command.ExecuteNonQuery();
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
        /// <summary>
        /// 取得所有訂單紀錄
        /// </summary>
        /// <returns>order</returns>
        public List<Order> GetOrderHistory()
        {
            var list =new List<Order>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT Id, OrderDate, TotalAmount FROM Orders ORDER BY Id DESC";
                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    //持續讀取資料直到沒有資料會回傳false結束迴圈
                    while (reader.Read())
                    {
                        list.Add(new Order
                        {
                            Id = reader.GetInt32(0),//轉為int
                            OrderDate = reader.GetDateTime(1),//轉為Datatime
                            TotalAmount = reader.GetDecimal(2)//轉為decimal
                        });
                    }
                }
                
            }
            return list;
        }
        /// <summary>
        /// 根據訂單ID 取得訂單的飲料明細
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public List<OrderItem> GetOrderDetails(int orderId)
        {
            var list = new List<OrderItem>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT MenuItemName, Price, Quantity, Subtotal FROM OrderItems WHERE OrderId = @id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", orderId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new OrderItem
                            {
                                Name = reader.GetString(0), // 對應 MenuItemName
                                Price = reader.GetDecimal(1),
                                Quantity = reader.GetInt32(2),

                                // Subtotal 可以直接讀取，或是由 Property 自動計算，這裡示範直接讀取
                            });
                        }
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 儲存點餐訊息
        /// </summary>
        /// <param name="total"></param>
        /// <param name="items"></param>
        public void SaveOrder(decimal total,List<OrderItem> items)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. 插入訂單主檔並取得自動生成的 ID
                        string insertOrderSql = "INSERT INTO Orders (OrderDate, TotalAmount) VALUES (DateTime('now','localtime'), @total); SELECT last_insert_rowid();";
                        long orderId;
                        using (var cmd = new SQLiteCommand(insertOrderSql, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@total", total);
                            orderId = (long)cmd.ExecuteScalar();
                        }

                        // 2. 插入該訂單的所有明細
                        string insertItemSql = "INSERT INTO OrderItems (OrderId, MenuItemName, Price, Quantity,Subtotal) VALUES (@orderId, @name, @price, @qty,@stal)";
                        foreach (var item in items)
                        {
                            using (var cmd = new SQLiteCommand(insertItemSql, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@orderId", orderId);
                                cmd.Parameters.AddWithValue("@name", item.Name);
                                cmd.Parameters.AddWithValue("@price", item.Price);
                                cmd.Parameters.AddWithValue("@qty", item.Quantity);
                                cmd.Parameters.AddWithValue("@stal", total);

                                cmd.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                    catch { transaction.Rollback(); throw; }
                }
            }
        }
        // 取得當日下班結帳資訊 (日結)
        public string GetDailyReport()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                // 查詢當天的訂單數與總金額
                string sql = "SELECT COUNT(*), SUM(TotalAmount) FROM Orders WHERE date(OrderDate) = date('now','localtime')";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int count = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            decimal sum = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1);

                            if (count == 0)
                            {
                                return "無資料";
                            }

                            return $"今日日期: {DateTime.Now:yyyy-MM-dd}\n總訂單數: {count} 筆\n總營業額: ${sum:N0}";
                        }
                        
                    }
                }
            }
            return "無資料";
        }

    }
}
