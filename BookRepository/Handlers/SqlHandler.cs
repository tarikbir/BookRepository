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

        public static Response.GetBookResponse GetBook(string ISBN)
        {
            Response.GetBookResponse BookResponse = new Response.GetBookResponse();
            string query = "SELECT * FROM `bx-books` WHERE ISBN = @ISBN";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommand = new MySqlCommand(query, conn);
                mySqlCommand.Parameters.AddWithValue("@ISBN", ISBN);
                try
                {
                    conn.Open();
                    MySqlDataReader reader = mySqlCommand.ExecuteReader();
                    Book Book = new Book();

                    if (reader.Read())
                    {
                        Book.ISBN = reader.GetFieldValue<string>(0);
                        Book.BookTitle = reader.GetFieldValue<string>(1);
                        Book.BookAuthor = reader.GetFieldValue<string>(2);
                        Int32.TryParse(reader.GetFieldValue<string>(3), out int year);
                        Book.Publisher = reader.GetFieldValue<string>(4);
                        Book.ImageURI_S = reader.GetFieldValue<string>(5);
                        Book.ImageURI_M = reader.GetFieldValue<string>(6);
                        Book.ImageURI_L = reader.GetFieldValue<string>(7);
                        Book.YearOfPublication = year;
                    }
                    else
                    {
                        throw new Exception("Could not find a book with ISBN no: '" + ISBN + "'.");
                    }

                    BookResponse.Book = Book;
                    BookResponse.Success = true;
                }
                catch (MySqlException e)
                {
                    BookResponse.ErrorText = e.Message;
                }
                catch (Exception e)
                {
                    BookResponse.ErrorText = e.Message;
                }
            }

            return BookResponse;
        }
    }
}
