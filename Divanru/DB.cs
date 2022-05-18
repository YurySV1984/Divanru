using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Divanru
{
    class DB
    {
        private static readonly string dbConnectionString = ConfigurationManager.ConnectionStrings["DBconnectionString"].ConnectionString;
        private readonly MySqlConnection connection = new MySqlConnection(dbConnectionString);
        
        public event EventHandler<EventArgs> OnError;
        public event EventHandler<CopyCatToDBArgs> OnCopyCategoryToDB;

        private const string productUrl = "https://www.divan.ru/ekaterinburg/product/";

        public void OpenConnecton()
        {
            if (connection.State == ConnectionState.Closed)
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ee)
                {
                    OnError?.Invoke(this, new EventArgs(ee.Message));
                }
            }
        }

        public void CloseConnecton()
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        public MySqlConnection GetConnection()
        {
            return connection;
        }

        /// <summary>
        /// копирует продует в БД
        /// </summary>, проверяя перед этим его наличие там
        public void CopyProductToDB(Furniture furniture)
        {
            if (furniture.Model == null) return;

            var db = new DB();
            var table = new DataTable();
            var adapter = new MySqlDataAdapter();
            var command = new MySqlCommand($"SELECT * FROM `furniture` WHERE `Model`=@Model", db.GetConnection());
            command.Parameters.Add("@Model", MySqlDbType.VarChar).Value = furniture.Model;

            try
            {
                adapter.SelectCommand = command;
                adapter.Fill(table);
            }
            catch (Exception ee)
            {
                OnError?.Invoke(this, new EventArgs(ee.Message));
            }

            if (table.Rows.Count > 0)
            {        
                OnError?.Invoke(this, new EventArgs($"{furniture.Model} is aleady in the Database"));
                return;
            }

            command = new MySqlCommand($"INSERT INTO `furniture` (`Categories0`, `Categories1`, `Categories2`, `Model`, `Description`, `Price`, `OldPrice`, `Link`, `size0`, `size1`, `size2`, `characteristics0`, `characteristics1`, `characteristics2`, `characteristics3`, `characteristics4`, `characteristics5`, `characteristics6`, `characteristics7`, `characteristics8`, `characteristics9`, `characteristics10`, `characteristics11`, `characteristics12`, `characteristics13`, `ImageUrl`, `Image`) VALUES (@Categories0, @Categories1, @Categories2, @Model, @Description, @Price, @OldPrice, @Link, @size0, @size1, @size2, @characteristics0, @characteristics1, @characteristics2, @characteristics3, @characteristics4, @characteristics5, @characteristics6, @characteristics7, @characteristics8, @characteristics9, @characteristics10, @characteristics11, @characteristics12, @characteristics13, @ImageUrl, @Image);", db.GetConnection());

            command.Parameters.Add("@Categories0", MySqlDbType.VarChar).Value = furniture.Categories?.Length > 0 ? furniture.Categories[0] : "";
            command.Parameters.Add("@Categories1", MySqlDbType.VarChar).Value = furniture.Categories?.Length > 1 ? furniture.Categories[1] : "";
            command.Parameters.Add("@Categories2", MySqlDbType.VarChar).Value = furniture.Categories?.Length > 2 ? furniture.Categories[2] : "";
            command.Parameters.Add("@Model", MySqlDbType.VarChar).Value = furniture.Model;
            command.Parameters.Add("@Description", MySqlDbType.VarChar).Value = furniture.Description ?? "";
            command.Parameters.Add("@Price", MySqlDbType.VarChar).Value = furniture.Price ?? "";
            command.Parameters.Add("@OldPrice", MySqlDbType.VarChar).Value = furniture.OldPrice ?? "";
            command.Parameters.Add("@Link", MySqlDbType.VarChar).Value = furniture.Link ?? "";
            command.Parameters.Add("@size0", MySqlDbType.VarChar).Value = furniture.Size?.Length > 0 ? furniture.Size[0] : "";
            command.Parameters.Add("@size1", MySqlDbType.VarChar).Value = furniture.Size?.Length > 1 ? furniture.Size[1] : "";
            command.Parameters.Add("@size2", MySqlDbType.VarChar).Value = furniture.Size?.Length > 2 ? furniture.Size[2] : "";
            command.Parameters.Add("@characteristics0", MySqlDbType.VarChar).Value = furniture.Characteristics?.Length > 0 ? furniture.Characteristics[0] : "";
            command.Parameters.Add("@characteristics1", MySqlDbType.VarChar).Value = furniture.Characteristics?.Length > 1 ? furniture.Characteristics[1] : "";
            command.Parameters.Add("@characteristics2", MySqlDbType.VarChar).Value = furniture.Characteristics?.Length > 2 ? furniture.Characteristics[2] : "";
            command.Parameters.Add("@characteristics3", MySqlDbType.VarChar).Value = furniture.Characteristics?.Length > 3 ? furniture.Characteristics[3] : "";
            command.Parameters.Add("@characteristics4", MySqlDbType.VarChar).Value = furniture.Characteristics?.Length > 4 ? furniture.Characteristics[4] : "";
            command.Parameters.Add("@characteristics5", MySqlDbType.VarChar).Value = furniture.Characteristics?.Length > 5 ? furniture.Characteristics[5] : "";
            command.Parameters.Add("@characteristics6", MySqlDbType.VarChar).Value = furniture.Characteristics?.Length > 6 ? furniture.Characteristics[6] : "";
            command.Parameters.Add("@characteristics7", MySqlDbType.VarChar).Value = furniture.Characteristics?.Length > 7 ? furniture.Characteristics[7] : "";
            command.Parameters.Add("@characteristics8", MySqlDbType.VarChar).Value = furniture.Characteristics?.Length > 8 ? furniture.Characteristics[8] : "";
            command.Parameters.Add("@characteristics9", MySqlDbType.VarChar).Value = furniture.Characteristics?.Length > 9 ? furniture.Characteristics[9] : "";
            command.Parameters.Add("@characteristics10", MySqlDbType.VarChar).Value = furniture.Characteristics?.Length > 10 ? furniture.Characteristics[10] : "";
            command.Parameters.Add("@characteristics11", MySqlDbType.VarChar).Value = furniture.Characteristics?.Length > 11 ? furniture.Characteristics[11] : "";
            command.Parameters.Add("@characteristics12", MySqlDbType.VarChar).Value = furniture.Characteristics?.Length > 12 ? furniture.Characteristics[12] : "";
            command.Parameters.Add("@characteristics13", MySqlDbType.VarChar).Value = furniture.Characteristics?.Length > 13 ? furniture.Characteristics[13] : "";
            command.Parameters.Add("@ImageUrl", MySqlDbType.VarChar).Value = furniture.ImageUrl ?? "";
            command.Parameters.Add("@Image", MySqlDbType.MediumBlob).Value = furniture.Image ?? new byte[1];

            db.OpenConnecton();

            try
            {
                if (command.ExecuteNonQuery() == 1)
                    OnError?.Invoke(this, new EventArgs($"{furniture.Model} has been added to the Database"));
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, new EventArgs($"Error adding {furniture.Model}, {e.Message}"));
            }

            db.CloseConnecton();
        }

        public SFurniture[] SearchInDb(string key)
        {
            var db = new DB();
            var table = new DataTable();
            var adapter = new MySqlDataAdapter();

            var command = new MySqlCommand($"SELECT `id`, `Model` FROM `furniture` WHERE `Model` LIKE @keyword OR `Categories0` LIKE @keyword OR `Categories1` LIKE @keyword OR `Categories2` LIKE @keyword OR `Description` LIKE @keyword OR `Price` LIKE @keyword", db.GetConnection());

            var s1 = key.Split(' ');
            var s2 = string.Empty;
            foreach (string s in s1)
                s2 = s2 + "%" + s;
            command.Parameters.Add("@keyword", MySqlDbType.VarChar).Value = "%" + s2 + "%";
            try
            {
                adapter.SelectCommand = command;
                adapter.Fill(table);
            }
            catch (Exception ee)
            {
                OnError?.Invoke(this, new EventArgs(ee.Message));
            }

            if (table.Rows.Count == 0)
            {
                OnError?.Invoke(this, new EventArgs($"{key} not found"));
                return null;
            }

            var sfurTable = new SFurniture[table.Rows.Count];
                     
            for (int i = 0; i < table.Rows.Count; i++)
            {
                sfurTable[i] = new SFurniture
                {
                    Id = (uint)table.Rows[i].ItemArray[0],
                    Model = (string)table.Rows[i].ItemArray[1]
                };
            }
            return sfurTable;
        }

        public void OpenProductFromDB(uint id, Furniture furniture)
        {
            var db = new DB();
            var table = new DataTable();
            var adapter = new MySqlDataAdapter();

            var command = new MySqlCommand($"SELECT `Categories0`, `Categories1`, `Categories2`, `Model`, `Description`, `Price`, `OldPrice`, `Link`, `size0`, `size1`, `size2`, `characteristics0`, `characteristics1`, `characteristics2`, `characteristics3`, `characteristics4`, `characteristics5`, `characteristics6`, `characteristics7`, `characteristics8`, `characteristics9`, `characteristics10`, `characteristics11`, `characteristics12`, `characteristics13`, `ImageUrl`, `Image` FROM `furniture` WHERE `id` = @id", db.GetConnection());
            command.Parameters.Add("@id", MySqlDbType.UInt32).Value = id;
            try
            {
                adapter.SelectCommand = command;
                adapter.Fill(table);
            }
            catch (Exception ee)
            {
                OnError?.Invoke(this, new EventArgs(ee.Message));
            }

            furniture.Categories = new string[3] { (string)table.Rows[0].ItemArray[0], (string)table.Rows[0].ItemArray[1], (string)table.Rows[0].ItemArray[2] };
            furniture.Model = (string)table.Rows[0].ItemArray[3];
            furniture.Description = (string)table.Rows[0].ItemArray[4];
            furniture.Price = (string)table.Rows[0].ItemArray[5];
            furniture.OldPrice = (string)table.Rows[0].ItemArray[6];
            furniture.Link = (string)table.Rows[0].ItemArray[7];
            furniture.Size = new string[3] { (string)table.Rows[0].ItemArray[8], (string)table.Rows[0].ItemArray[9], (string)table.Rows[0].ItemArray[10] };
            furniture.Characteristics = new string[14] { (string)table.Rows[0].ItemArray[11], (string)table.Rows[0].ItemArray[12], (string)table.Rows[0].ItemArray[13], (string)table.Rows[0].ItemArray[14], (string)table.Rows[0].ItemArray[15], (string)table.Rows[0].ItemArray[16], (string)table.Rows[0].ItemArray[17], (string)table.Rows[0].ItemArray[18], (string)table.Rows[0].ItemArray[19], (string)table.Rows[0].ItemArray[20], (string)table.Rows[0].ItemArray[21], (string)table.Rows[0].ItemArray[22], (string)table.Rows[0].ItemArray[23], (string)table.Rows[0].ItemArray[24] };
            furniture.ImageUrl = (string)table.Rows[0].ItemArray[25];
            furniture.Image = (byte[])table.Rows[0].ItemArray[26];
        }

        public void DeleteProductFromDB(uint id, string model)
        {
            var db = new DB();
            var command = new MySqlCommand($"DELETE FROM `furniture` WHERE `furniture`.`id` = @id", db.GetConnection());
            command.Parameters.Add("@id", MySqlDbType.UInt32).Value = id;

            db.OpenConnecton();
            try
            {
                if (command.ExecuteNonQuery() == 1)
                {
                    OnError?.Invoke(this, new EventArgs($"{model} has been deleted from the Database"));
                }
            }
            catch (Exception ee)
            {
                OnError?.Invoke(this, new EventArgs(ee.Message));
            }
            db.CloseConnecton();
        }

        public async Task CopyCategoryToDb (Products products)
        {
            Furniture furniture = new Furniture ();
            for (int i = 0; i < products.Count; i++)    
            {
                await products.GetOneProduct(productUrl + products[i].Link, furniture);   
                CopyProductToDB(furniture); 
                OnCopyCategoryToDB?.Invoke(this, new CopyCatToDBArgs(products.Count, i + 1, furniture)); 
            } 
        }  
    }  
}
