using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace BookRepository
{
    public static class SqlHandler
    {
        private static string connectionString = "server=localhost;user=root;port=3306;database=bookrepository;password=;charset=utf8;SslMode=none";

        public static Book GetBook(string ISBN)
        {
            Book book = new Book();
            string query = "SELECT * FROM `bx-books` WHERE ISBN = @ISBN";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommand = new MySqlCommand(query, conn);
                mySqlCommand.Parameters.AddWithValue("@ISBN", ISBN);
                try
                {
                    conn.Open();

                    MySqlDataReader reader = mySqlCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        book.ISBN = reader.GetFieldValue<string>(0);
                        book.BookTitle = reader.GetFieldValue<string>(1);
                        book.BookAuthor = reader.GetFieldValue<string>(2);
                        Int32.TryParse(reader.GetFieldValue<string>(3), out int year);
                        book.Publisher = reader.GetFieldValue<string>(4);
                        book.ImageURI_S = reader.GetFieldValue<string>(5);
                        book.ImageURI_M = reader.GetFieldValue<string>(6);
                        book.ImageURI_L = reader.GetFieldValue<string>(7);
                        book.YearOfPublication = year;
                    }
                }
                catch (MySqlException e)
                {

                }
                catch (Exception e)
                {

                }
            }

            return book;
        }
    }
}
