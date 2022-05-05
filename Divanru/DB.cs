using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Divanru
{
    class DB
    {
        MySqlConnection connection = new MySqlConnection("server=localhost;port=3307;username=root;password=root;database=divanparser");
        public event EventHandler<ErrEventArgs> OnError;


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
                    OnError?.Invoke(this, new ErrEventArgs(ee.Message));
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
        public void CopyProductToDB()
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
                OnError?.Invoke(this, new ErrEventArgs(ee.Message));
            }

            if (table.Rows.Count > 0)
            {        
                OnError?.Invoke(this, new ErrEventArgs($"{Furniture.Model} is aleady in the Database"));
                return;
            }

            command = new MySqlCommand($"INSERT INTO `furniture` (`Categories0`, `Categories1`, `Categories2`, `Model`, `Description`, `Price`, `OldPrice`, `Link`, `size0`, `size1`, `size2`, `characteristics0`, `characteristics1`, `characteristics2`, `characteristics3`, `characteristics4`, `characteristics5`, `characteristics6`, `characteristics7`, `characteristics8`, `characteristics9`, `characteristics10`, `characteristics11`, `characteristics12`, `characteristics13`, `ImageUrl`, `Image`) VALUES (@Categories0, @Categories1, @Categories2, @Model, @Description, @Price, @OldPrice, @Link, @size0, @size1, @size2, @characteristics0, @characteristics1, @characteristics2, @characteristics3, @characteristics4, @characteristics5, @characteristics6, @characteristics7, @characteristics8, @characteristics9, @characteristics10, @characteristics11, @characteristics12, @characteristics13, @ImageUrl, @Image);", db.getConnection());

            command.Parameters.Add("@Categories0", MySqlDbType.VarChar).Value = Furniture.Categories?.Length > 0 ? Furniture.Categories[0] : "";
            command.Parameters.Add("@Categories1", MySqlDbType.VarChar).Value = Furniture.Categories?.Length > 1 ? Furniture.Categories[1] : "";
            command.Parameters.Add("@Categories2", MySqlDbType.VarChar).Value = Furniture.Categories?.Length > 2 ? Furniture.Categories[2] : "";
            command.Parameters.Add("@Model", MySqlDbType.VarChar).Value = Furniture.Model;
            command.Parameters.Add("@Description", MySqlDbType.VarChar).Value = Furniture.Description ?? "";
            command.Parameters.Add("@Price", MySqlDbType.VarChar).Value = Furniture.Price ?? "";
            command.Parameters.Add("@OldPrice", MySqlDbType.VarChar).Value = Furniture.OldPrice ?? "";
            command.Parameters.Add("@Link", MySqlDbType.VarChar).Value = Furniture.Link ?? "";
            command.Parameters.Add("@size0", MySqlDbType.VarChar).Value = Furniture.Size?.Length > 0 ? Furniture.Size[0] : "";
            command.Parameters.Add("@size1", MySqlDbType.VarChar).Value = Furniture.Size?.Length > 1 ? Furniture.Size[1] : "";
            command.Parameters.Add("@size2", MySqlDbType.VarChar).Value = Furniture.Size?.Length > 2 ? Furniture.Size[2] : "";
            command.Parameters.Add("@characteristics0", MySqlDbType.VarChar).Value = Furniture.Characteristics?.Length > 0 ? Furniture.Characteristics[0] : "";
            command.Parameters.Add("@characteristics1", MySqlDbType.VarChar).Value = Furniture.Characteristics?.Length > 1 ? Furniture.Characteristics[1] : "";
            command.Parameters.Add("@characteristics2", MySqlDbType.VarChar).Value = Furniture.Characteristics?.Length > 2 ? Furniture.Characteristics[2] : "";
            command.Parameters.Add("@characteristics3", MySqlDbType.VarChar).Value = Furniture.Characteristics?.Length > 3 ? Furniture.Characteristics[3] : "";
            command.Parameters.Add("@characteristics4", MySqlDbType.VarChar).Value = Furniture.Characteristics?.Length > 4 ? Furniture.Characteristics[4] : "";
            command.Parameters.Add("@characteristics5", MySqlDbType.VarChar).Value = Furniture.Characteristics?.Length > 5 ? Furniture.Characteristics[5] : "";
            command.Parameters.Add("@characteristics6", MySqlDbType.VarChar).Value = Furniture.Characteristics?.Length > 6 ? Furniture.Characteristics[6] : "";
            command.Parameters.Add("@characteristics7", MySqlDbType.VarChar).Value = Furniture.Characteristics?.Length > 7 ? Furniture.Characteristics[7] : "";
            command.Parameters.Add("@characteristics8", MySqlDbType.VarChar).Value = Furniture.Characteristics?.Length > 8 ? Furniture.Characteristics[8] : "";
            command.Parameters.Add("@characteristics9", MySqlDbType.VarChar).Value = Furniture.Characteristics?.Length > 9 ? Furniture.Characteristics[9] : "";
            command.Parameters.Add("@characteristics10", MySqlDbType.VarChar).Value = Furniture.Characteristics?.Length > 10 ? Furniture.Characteristics[10] : "";
            command.Parameters.Add("@characteristics11", MySqlDbType.VarChar).Value = Furniture.Characteristics?.Length > 11 ? Furniture.Characteristics[11] : "";
            command.Parameters.Add("@characteristics12", MySqlDbType.VarChar).Value = Furniture.Characteristics?.Length > 12 ? Furniture.Characteristics[12] : "";
            command.Parameters.Add("@characteristics13", MySqlDbType.VarChar).Value = Furniture.Characteristics?.Length > 13 ? Furniture.Characteristics[13] : "";
            command.Parameters.Add("@ImageUrl", MySqlDbType.VarChar).Value = Furniture.ImageUrl ?? "";
            command.Parameters.Add("@Image", MySqlDbType.Blob).Value = Furniture.Image ?? new byte[1];

            db.OpenConnecton();

            try
            {
                if (command.ExecuteNonQuery() == 1)
                {
                    OnError?.Invoke(this, new ErrEventArgs($"{Furniture.Model} added to the Database"));
                }
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, new ErrEventArgs($"Error adding {Furniture.Model}, {e.Message}"));
            }

            db.CloseConnecton();
        }

        public SFurniture[] SearchInDb(string key)
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
                OnError?.Invoke(this, new ErrEventArgs(ee.Message));
            }

            if (table.Rows.Count == 0)
            {
                OnError?.Invoke(this, new ErrEventArgs($"{key} not found"));
                return null;
            }

            var sfurTable = new SFurniture[table.Rows.Count];
            
            for (int i = 0; i < table.Rows.Count; i++)
            {
                sfurTable[i] = new SFurniture();
                sfurTable[i].id = (uint)table.Rows[i].ItemArray[0];
                sfurTable[i].model = (string)table.Rows[i].ItemArray[1];
            }
            return sfurTable;
        }

        public void OpenProductFromDB(uint id)
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
                OnError?.Invoke(this, new ErrEventArgs(ee.Message));
            }

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
        }

        public void DeleteProductFromDB(uint id, string model)
        {
            var db = new DB();
            var command = new MySqlCommand($"DELETE FROM `furniture` WHERE `furniture`.`id` = @id", db.getConnection());
            command.Parameters.Add("@id", MySqlDbType.UInt32).Value = id;

            db.OpenConnecton();
            try
            {
                if (command.ExecuteNonQuery() == 1)
                {
                    OnError?.Invoke(this, new ErrEventArgs($"{model} deleted from the Database"));
                }
                //else Log.WriteLog($"Error deleting {model}");
            }
            catch (Exception ee)
            {
                OnError?.Invoke(this, new ErrEventArgs(ee.Message));
            }
            db.CloseConnecton();
        }

        
    }

    
}
