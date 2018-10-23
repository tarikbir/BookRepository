using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace BookRepository
{
    public static class SqlHandler
    {
        private static string connectionString = "server=localhost;user=root;port=3306;database=bookrepository;password=;charset=utf8;SslMode=none";

        #region Functional Methods
        public static bool IsConnected()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static Book GetBookFromReader(MySqlDataReader reader)
        {
            Int32.TryParse(reader.GetFieldValue<string>(3), out int year);
            Book Book = new Book() {
                ISBN = reader.GetFieldValue<string>(0),
                BookTitle = reader.GetFieldValue<string>(1),
                BookAuthor = reader.GetFieldValue<string>(2),
                Publisher = reader.GetFieldValue<string>(4),
                ImageURI_S = reader.GetFieldValue<string>(5),
                ImageURI_M = reader.GetFieldValue<string>(6),
                ImageURI_L = reader.GetFieldValue<string>(7),
                YearOfPublication = year
            };
            return Book;
        }
        #endregion

        #region SQL Responses
        public static Response.BookListResponse GetPopularList()
        {
            Response.BookListResponse bookListResponse = new Response.BookListResponse();
            string query = "SELECT * FROM `bx-books` AS B INNER JOIN (SELECT `ISBN`, COUNT(`ISBN`) AS NumberRating FROM `bx-book-ratings` GROUP BY `ISBN`) AS F ON B.`ISBN` = F.`ISBN` ORDER BY F.NumberRating DESC LIMIT 10";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommand = new MySqlCommand(query, conn);

                try
                {
                    conn.Open();
                    
                    MySqlDataReader reader = mySqlCommand.ExecuteReader();

                    while(reader.Read())
                    {
                        try
                        {
                            bookListResponse.Books.Add(GetBookFromReader(reader));
                        }
                        catch (Exception e)
                        {
                            bookListResponse.ErrorText += e.Message + "\n";
                            continue;
                        }
                    }
                    bookListResponse.Success = true;
                }
                catch (Exception e)
                {
                    bookListResponse.ErrorText = e.Message;
                }
            }
            return bookListResponse;
        }

        public static Response.BookListResponse GetHighRatedList()
        {
            Response.BookListResponse bookListResponse = new Response.BookListResponse();
            string query = "SELECT * FROM `bx-books` AS B INNER JOIN (SELECT DISTINCT `ISBN`, Weight FROM `bx-book-ratings` ORDER BY Weight DESC LIMIT 10) AS F ON B.`ISBN` = F.`ISBN`";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommand = new MySqlCommand(query, conn);

                try
                {
                    conn.Open();

                    MySqlDataReader reader = mySqlCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        try
                        {
                            bookListResponse.Books.Add(GetBookFromReader(reader));
                        }
                        catch (Exception e)
                        {
                            bookListResponse.ErrorText += e.Message + "\n";
                            continue;
                        }
                    }
                    bookListResponse.Success = true;
                }
                catch (Exception e)
                {
                    bookListResponse.ErrorText = e.Message;
                }
            }
            return bookListResponse;
        }

        public static Response.LoginResponse UserEntry(string username, string password)
        {
            string queryLogin = "SELECT * FROM `bx-users` WHERE username = @username AND password = @password";
            Response.LoginResponse loginEntry = new Response.LoginResponse();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandLogin = new MySqlCommand(queryLogin, conn);
                mySqlCommandLogin.Parameters.AddWithValue("@username", username);
                mySqlCommandLogin.Parameters.AddWithValue("@password", password);
                try
                {
                    conn.Open();
                    MySqlDataReader reader = mySqlCommandLogin.ExecuteReader();

                    if (reader.Read())
                    {
                        //Full texts UserID Username Password Location Age
                        loginEntry.User = new User()
                        {
                            UserID = reader.GetFieldValue<UInt32>(0),
                            Username = reader.GetFieldValue<string>(1),
                            Location = reader.GetFieldValue<string>(3) ?? String.Empty,
                            Age = reader.IsDBNull(4)? 0 : reader.GetUInt32(4)
                        };
                    }
                    else
                    {
                        throw new Exception("No matching username and password combination.");
                    }
                    loginEntry.Success = true;
                }
                catch(Exception e)
                {
                    loginEntry.ErrorText = e.Message;
                }
                return loginEntry;
            }
        }

        public static Response.GetBookResponse GetBook(string ISBN)
        {
            Response.GetBookResponse bookResponse = new Response.GetBookResponse();
            string query = "SELECT * FROM `bx-books` WHERE ISBN = @ISBN";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommand = new MySqlCommand(query, conn);
                mySqlCommand.Parameters.AddWithValue("@ISBN", ISBN);
                try
                {
                    conn.Open();
                    MySqlDataReader reader = mySqlCommand.ExecuteReader();
                    Book Book;

                    if (reader.Read())
                    {
                        Book = GetBookFromReader(reader);
                    }
                    else
                    {
                        throw new Exception("Could not find a book with ISBN no: '" + ISBN + "'.");
                    }

                    bookResponse.Book = Book;
                    bookResponse.Success = true;
                }
                catch (Exception e)
                {
                    bookResponse.ErrorText = e.Message;
                }
            }

            return bookResponse;
        }

        public static Response.GenericResponse<int> GetVote(string userID, string bookISBN)
        {
            Response.GenericResponse<int> vote = new Response.GenericResponse<int>();
            string query = "SELECT `Book-Rating` FROM `bx-book-ratings` WHERE ISBN = @ISBN AND `User-ID` = @UserID";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommand = new MySqlCommand(query, conn);
                mySqlCommand.Parameters.AddWithValue("@ISBN", bookISBN);
                mySqlCommand.Parameters.AddWithValue("@UserID", userID);
                try
                {
                    conn.Open();
                    MySqlDataReader reader = mySqlCommand.ExecuteReader();

                    if (reader.Read())
                    {
                        vote.Content = reader.GetFieldValue<int>(0);
                    }
                    else
                    {
                        throw new Exception("No vote has been cast by user '" + userID + "' for the book '" + bookISBN + "'.");
                    }
                }
                catch (Exception e)
                {
                    vote.ErrorText = e.Message;
                }
            }
            return vote;
        }

        public static Response.BaseResponse Register(string username, int age, string country, string county, string city, string password, bool isAdmin)
        {
            string Concat = string.Join(",", country, county, city);
            string queryUser = "INSERT INTO `bx-users`(`Username`, `Password`, `Location`, `Age`, `IsAdmin`) VALUES (@Username,@Password,@Concat,@Age,@IsAdmin)";
            Response.BaseResponse register = new Response.BaseResponse();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandUser = new MySqlCommand(queryUser, conn);
                mySqlCommandUser.Parameters.AddWithValue("@Age", age);
                mySqlCommandUser.Parameters.AddWithValue("@Concat", Concat);
                mySqlCommandUser.Parameters.AddWithValue("@Username", username);
                mySqlCommandUser.Parameters.AddWithValue("@Password", password);
                mySqlCommandUser.Parameters.AddWithValue("@IsAdmin", isAdmin);
                try
                {
                    conn.Open();
                    int rowsAffected = mySqlCommandUser.ExecuteNonQuery();
                    if (rowsAffected != 1)
                    {
                        throw new Exception("Error inserting into users table.");
                    }
                    register.Success = true;
                }
                catch (Exception e)
                {
                    register.ErrorText = e.Message;
                }
                return register;
            }
        }
        #endregion
    }
}


