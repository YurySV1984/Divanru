using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Divanru
{
    class DB
    {
        MySqlConnection connection = new MySqlConnection("server=localhost;port=3307;username=root;password=root;database=divanparser");
        public void OpenConnecton()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ee)
                {
                    Log.WriteLog(ee.Message);
                }
            }
        }

        public void CloseConnecton()
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public MySqlConnection getConnection()
        {
            return connection;
        }

        /// <summary>
        /// копирует продует в БД
        /// </summary>, проверяя перед этим его наличие там
        public static void CopyProductToDB()
        {
            if (Furniture.Model == null) return;
            var db = new DB();
            var table = new DataTable();
            var adapter = new MySqlDataAdapter();
            var command = new MySqlCommand($"SELECT * FROM `furniture` WHERE `Model`=@Model", db.getConnection());
            command.Parameters.Add("@Model", MySqlDbType.VarChar).Value = Furniture.Model;

            try
            {
                adapter.SelectCommand = command;
                adapter.Fill(table);
            }
            catch (Exception ee)
            {
                //Logs.Items.Add(ee.Message);
                Log.WriteLog(ee.Message);
            }

            if (table.Rows.Count > 0)
            {
                //Logs.Items.Add($"{Furniture.Model} is aleady in the Database");
                Log.WriteLog($"{Furniture.Model} is aleady in the Database");
                return;
            }

            command = new MySqlCommand($"INSERT INTO `furniture` (`Categories0`, `Categories1`, `Categories2`, `Model`, `Description`, `Price`, `OldPrice`, `Link`, `size0`, `size1`, `size2`, `characteristics0`, `characteristics1`, `characteristics2`, `characteristics3`, `characteristics4`, `characteristics5`, `characteristics6`, `characteristics7`, `characteristics8`, `characteristics9`, `characteristics10`, `characteristics11`, `characteristics12`, `characteristics13`, `ImageUrl`, `Image`) VALUES (@Categories0, @Categories1, @Categories2, @Model, @Description, @Price, @OldPrice, @Link, @size0, @size1, @size2, @characteristics0, @characteristics1, @characteristics2, @characteristics3, @characteristics4, @characteristics5, @characteristics6, @characteristics7, @characteristics8, @characteristics9, @characteristics10, @characteristics11, @characteristics12, @characteristics13, @ImageUrl, @Image);", db.getConnection());

            command.Parameters.Add("@Categories0", MySqlDbType.VarChar).Value = Furniture.Categories.Length > 0 ? Furniture.Categories[0] : "";
            command.Parameters.Add("@Categories1", MySqlDbType.VarChar).Value = Furniture.Categories.Length > 1 ? Furniture.Categories[1] : "";
            command.Parameters.Add("@Categories2", MySqlDbType.VarChar).Value = Furniture.Categories.Length > 2 ? Furniture.Categories[2] : "";
            command.Parameters.Add("@Model", MySqlDbType.VarChar).Value = Furniture.Model;
            command.Parameters.Add("@Description", MySqlDbType.VarChar).Value = Furniture.Description ?? "";
            command.Parameters.Add("@Price", MySqlDbType.VarChar).Value = Furniture.Price ?? "";
            command.Parameters.Add("@OldPrice", MySqlDbType.VarChar).Value = Furniture.OldPrice ?? "";
            command.Parameters.Add("@Link", MySqlDbType.VarChar).Value = Furniture.Link ?? "";
            command.Parameters.Add("@size0", MySqlDbType.VarChar).Value = Furniture.Size.Length > 0 ? Furniture.Size[0] : "";
            command.Parameters.Add("@size1", MySqlDbType.VarChar).Value = Furniture.Size.Length > 1 ? Furniture.Size[1] : "";
            command.Parameters.Add("@size2", MySqlDbType.VarChar).Value = Furniture.Size.Length > 2 ? Furniture.Size[2] : "";
            command.Parameters.Add("@characteristics0", MySqlDbType.VarChar).Value = Furniture.Characteristics.Length > 0 ? Furniture.Characteristics[0] : "";
            command.Parameters.Add("@characteristics1", MySqlDbType.VarChar).Value = Furniture.Characteristics.Length > 1 ? Furniture.Characteristics[1] : "";
            command.Parameters.Add("@characteristics2", MySqlDbType.VarChar).Value = Furniture.Characteristics.Length > 2 ? Furniture.Characteristics[2] : "";
            command.Parameters.Add("@characteristics3", MySqlDbType.VarChar).Value = Furniture.Characteristics.Length > 3 ? Furniture.Characteristics[3] : "";
            command.Parameters.Add("@characteristics4", MySqlDbType.VarChar).Value = Furniture.Characteristics.Length > 4 ? Furniture.Characteristics[4] : "";
            command.Parameters.Add("@characteristics5", MySqlDbType.VarChar).Value = Furniture.Characteristics.Length > 5 ? Furniture.Characteristics[5] : "";
            command.Parameters.Add("@characteristics6", MySqlDbType.VarChar).Value = Furniture.Characteristics.Length > 6 ? Furniture.Characteristics[6] : "";
            command.Parameters.Add("@characteristics7", MySqlDbType.VarChar).Value = Furniture.Characteristics.Length > 7 ? Furniture.Characteristics[7] : "";
            command.Parameters.Add("@characteristics8", MySqlDbType.VarChar).Value = Furniture.Characteristics.Length > 8 ? Furniture.Characteristics[8] : "";
            command.Parameters.Add("@characteristics9", MySqlDbType.VarChar).Value = Furniture.Characteristics.Length > 9 ? Furniture.Characteristics[9] : "";
            command.Parameters.Add("@characteristics10", MySqlDbType.VarChar).Value = Furniture.Characteristics.Length > 10 ? Furniture.Characteristics[10] : "";
            command.Parameters.Add("@characteristics11", MySqlDbType.VarChar).Value = Furniture.Characteristics.Length > 11 ? Furniture.Characteristics[11] : "";
            command.Parameters.Add("@characteristics12", MySqlDbType.VarChar).Value = Furniture.Characteristics.Length > 12 ? Furniture.Characteristics[12] : "";
            command.Parameters.Add("@characteristics13", MySqlDbType.VarChar).Value = Furniture.Characteristics.Length > 13 ? Furniture.Characteristics[13] : "";
            command.Parameters.Add("@ImageUrl", MySqlDbType.VarChar).Value = Furniture.ImageUrl ?? "";
            command.Parameters.Add("@Image", MySqlDbType.Blob).Value = Furniture.Image ?? new byte[1];

            db.OpenConnecton();

            try
            {
                if (command.ExecuteNonQuery() == 1)
                {
                    //Logs.Items.Add($"{Furniture.Model} added to the Database");
                    Log.WriteLog($"{Furniture.Model} added to the Database");
                }
            }
            catch (Exception e)
            {
                //Logs.Items.Add($"Error adding {Furniture.Model}, {e.Message}");
                Log.WriteLog($"Error adding {Furniture.Model}, {e.Message}");
            }

            db.CloseConnecton();
        }

        public static SFurniture[] SearchInDb(string key)
        {
            var db = new DB();
            var table = new DataTable();
            var adapter = new MySqlDataAdapter();

            var command = new MySqlCommand($"SELECT `id`, `Model` FROM `furniture` WHERE `Model` LIKE @keyword OR `Categories0` LIKE @keyword OR `Categories1` LIKE @keyword OR `Categories2` LIKE @keyword OR `Description` LIKE @keyword OR `Price` LIKE @keyword", db.getConnection());

            var s1 = key.Split(' ');
            var s2 = string.Empty;
            foreach (string s in s1)
            {
                s2 = s2 + "%" + s;
            }
            command.Parameters.Add("@keyword", MySqlDbType.VarChar).Value = "%" + s2 + "%";
            try
            {
                adapter.SelectCommand = command;
                adapter.Fill(table);
            }
            catch (Exception ee)
            {
                //Logs.Items.Add(ee.Message);
                Log.WriteLog(ee.Message);
            }

            if (table.Rows.Count == 0)
            {
                //Logs.Items.Add($"{tbKeyword.Text} not found");
                Log.WriteLog($"{key} not found");
                return null;
            }

            var sfurTable = new SFurniture[table.Rows.Count];
            
            for (int i = 0; i < table.Rows.Count; i++)
            {
                sfurTable[i] = new SFurniture();
                sfurTable[i].id = (uint)table.Rows[i].ItemArray[0];
                sfurTable[i].model = (string)table.Rows[i].ItemArray[1];
                //listBox3.Items.Add(sfurTable[i].model);
            }
            return sfurTable;
        }

        public static void OpenProductFromDB(uint id)
        {
            var db = new DB();
            var table = new DataTable();
            var adapter = new MySqlDataAdapter();

            var command = new MySqlCommand($"SELECT `Categories0`, `Categories1`, `Categories2`, `Model`, `Description`, `Price`, `OldPrice`, `Link`, `size0`, `size1`, `size2`, `characteristics0`, `characteristics1`, `characteristics2`, `characteristics3`, `characteristics4`, `characteristics5`, `characteristics6`, `characteristics7`, `characteristics8`, `characteristics9`, `characteristics10`, `characteristics11`, `characteristics12`, `characteristics13`, `ImageUrl`, `Image` FROM `furniture` WHERE `id` = @id", db.getConnection());
            command.Parameters.Add("@id", MySqlDbType.UInt32).Value = id;
            try
            {
                adapter.SelectCommand = command;
                adapter.Fill(table);
            }
            catch (Exception ee)
            {
                //Logs.Items.Add(ee.Message);
                Log.WriteLog(ee.Message);
            }

            //Furniture[] furTable = new Furniture[table.Rows.Count];
            //for (int i = 0; i < table.Rows.Count; i++)
            //{
            //    furTable[i] = new Furniture();
            //    furTable[i].categories = new string[3] { (string)table.Rows[i].ItemArray[0], (string)table.Rows[i].ItemArray[1], (string)table.Rows[i].ItemArray[2] };
            //    furTable[i].model = (string)table.Rows[i].ItemArray[3];
            //    furTable[i].description = (string)table.Rows[i].ItemArray[4];
            //    furTable[i].price = (string)table.Rows[i].ItemArray[5];
            //    furTable[i].oldPrice = (string)table.Rows[i].ItemArray[6];
            //    furTable[i].link = (string)table.Rows[i].ItemArray[7];
            //    furTable[i].size = new string[3] { (string)table.Rows[i].ItemArray[8], (string)table.Rows[i].ItemArray[9], (string)table.Rows[i].ItemArray[10] };
            //    furTable[i].characteristics = new string[14] { (string)table.Rows[i].ItemArray[11], (string)table.Rows[i].ItemArray[12], (string)table.Rows[i].ItemArray[13], (string)table.Rows[i].ItemArray[14], (string)table.Rows[i].ItemArray[15], (string)table.Rows[i].ItemArray[16], (string)table.Rows[i].ItemArray[17], (string)table.Rows[i].ItemArray[18], (string)table.Rows[i].ItemArray[19], (string)table.Rows[i].ItemArray[20], (string)table.Rows[i].ItemArray[21], (string)table.Rows[i].ItemArray[22], (string)table.Rows[i].ItemArray[23], (string)table.Rows[i].ItemArray[24] };
            //    furTable[i].imageUrl = (string)table.Rows[i].ItemArray[25];
            //    furTable[i].image = (byte[])table.Rows[i].ItemArray[26];
            //}
            Furniture.Categories = new string[3] { (string)table.Rows[0].ItemArray[0], (string)table.Rows[0].ItemArray[1], (string)table.Rows[0].ItemArray[2] };
            Furniture.Model = (string)table.Rows[0].ItemArray[3];
            Furniture.Description = (string)table.Rows[0].ItemArray[4];
            Furniture.Price = (string)table.Rows[0].ItemArray[5];
            Furniture.OldPrice = (string)table.Rows[0].ItemArray[6];
            Furniture.Link = (string)table.Rows[0].ItemArray[7];
            Furniture.Size = new string[3] { (string)table.Rows[0].ItemArray[8], (string)table.Rows[0].ItemArray[9], (string)table.Rows[0].ItemArray[10] };
            Furniture.Characteristics = new string[14] { (string)table.Rows[0].ItemArray[11], (string)table.Rows[0].ItemArray[12], (string)table.Rows[0].ItemArray[13], (string)table.Rows[0].ItemArray[14], (string)table.Rows[0].ItemArray[15], (string)table.Rows[0].ItemArray[16], (string)table.Rows[0].ItemArray[17], (string)table.Rows[0].ItemArray[18], (string)table.Rows[0].ItemArray[19], (string)table.Rows[0].ItemArray[20], (string)table.Rows[0].ItemArray[21], (string)table.Rows[0].ItemArray[22], (string)table.Rows[0].ItemArray[23], (string)table.Rows[0].ItemArray[24] };
            Furniture.ImageUrl = (string)table.Rows[0].ItemArray[25];
            Furniture.Image = (byte[])table.Rows[0].ItemArray[26];

            //Furniture.Categories = furTable[0].categories;
            //Furniture.Model = furTable[0].model;
            //Furniture.Description = furTable[0].description;
            //Furniture.Price = furTable[0].price;
            //Furniture.OldPrice = furTable[0].oldPrice;
            //Furniture.Link = furTable[0].link;
            //Furniture.Size = furTable[0].size;
            //Furniture.Characteristics = furTable[0].characteristics;
            //Furniture.ImageUrl = furTable[0].imageUrl;
            //Furniture.Image = furTable[0].image;

            //return furTable;
        }

        public static void DeleteProductFromDB(uint id, string model)
        {
            var db = new DB();
            var command = new MySqlCommand($"DELETE FROM `furniture` WHERE `furniture`.`id` = @id", db.getConnection());
            command.Parameters.Add("@id", MySqlDbType.UInt32).Value = id;

            db.OpenConnecton();
            try
            {
                if (command.ExecuteNonQuery() == 1)
                {
                    //Logs.Items.Add($"{model} deleted from the Database");
                    Log.WriteLog($"{model} deleted from the Database");
                }
                else Log.WriteLog($"Error deleting {model}");
            }
            catch (Exception ee)
            {
                Log.WriteLog(ee.Message);
            }
            db.CloseConnecton();
        }
    }
}
