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
            Book Book = new Book()
            {
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
        public static Response.GenericResponse<List<Book>> GetPopularList()
        {
            Response.GenericResponse<List<Book>> bookListResponse = new Response.GenericResponse<List<Book>>();
            string query = "SELECT * FROM `bx-books` AS B INNER JOIN (SELECT `ISBN`, COUNT(`ISBN`) AS NumberRating FROM `bx-book-ratings` GROUP BY `ISBN`) AS F ON B.`ISBN` = F.`ISBN` ORDER BY F.NumberRating DESC LIMIT 10";

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
                            bookListResponse.Content.Add(GetBookFromReader(reader));
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

        public static Response.GenericResponse<List<Book>> GetHighRatedList()
        {
            Response.GenericResponse<List<Book>> bookListResponse = new Response.GenericResponse<List<Book>>();
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
                            bookListResponse.Content.Add(GetBookFromReader(reader));
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

        public static Response.GenericResponse<User> UserEntry(string username, string password)
        {
            string queryLogin = "SELECT * FROM `bx-users` WHERE username = @username AND password = @password";
            Response.GenericResponse<User> loginEntry = new Response.GenericResponse<User>();
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
                        loginEntry.Content = new User()
                        {
                            UserID = reader.GetFieldValue<UInt32>(0),
                            Username = reader.GetFieldValue<string>(1),
                            Location = reader.GetFieldValue<string>(3) ?? String.Empty,
                            Age = reader.IsDBNull(4) ? 0 : reader.GetUInt32(4),
                            IsAdmin = reader.GetFieldValue<bool>(5)
                        };
                    }
                    else
                    {
                        throw new Exception("No matching username and password combination.");
                    }
                    loginEntry.Success = true;
                }
                catch (Exception e)
                {
                    loginEntry.ErrorText = e.Message;
                }
                return loginEntry;
            }
        }

        public static Response.GenericResponse<Book> GetBook(string ISBN)
        {
            Response.GenericResponse<Book> bookResponse = new Response.GenericResponse<Book>();
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
                    bookResponse.Content = Book;
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

        public static Response.BaseResponse AddUser(string username, int age, string country, string county, string city, string password)
        {
            string concat2 = string.Join(",", country, county, city);
            string queryAddUser = "INSERT INTO `bx-users`(`Username`, `Password`, `Location`, `Age`) VALUES (@Username,@Password,@Concat2,@Age)";
            Response.BaseResponse adduser = new Response.BaseResponse();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandAddUser = new MySqlCommand(queryAddUser, conn);
                mySqlCommandAddUser.Parameters.AddWithValue("@Username", username);
                mySqlCommandAddUser.Parameters.AddWithValue("@Password", password);
                mySqlCommandAddUser.Parameters.AddWithValue("@Concat2", concat2);
                mySqlCommandAddUser.Parameters.AddWithValue("@Age", age);
                try
                {
                    conn.Open();
                    int AffectedRows = mySqlCommandAddUser.ExecuteNonQuery();
                    if (AffectedRows != 1)
                    {
                        throw new Exception("The Error has occured while inserting into users table");
                    }
                    adduser.Success = true;
                }
                catch (Exception e)
                {
                    adduser.ErrorText = e.Message;
                }
                return adduser;
            }
        }

        public static Response.BaseResponse RemoveUser(string username)
        {

            string queryRemoveUser = ("DELETE FROM `bx-users` WHERE Username=@username");
            Response.BaseResponse removeuser = new Response.BaseResponse();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandRemoveUser = new MySqlCommand(queryRemoveUser, conn);
                try
                {
                    conn.Open();
                    mySqlCommandRemoveUser.ExecuteNonQuery();
                    removeuser.Success = true;
                }
                catch (Exception e)
                {
                    removeuser.ErrorText = e.Message;
                }
                return removeuser;

            }
        }

        public static Response.BaseResponse GetAllUsers()
        {
            string queryGetBooks = "SELECT * FROM `bx-users`";
            Response.BaseResponse getallusers = new Response.BaseResponse();
            Response.GenericResponse<List<User>> user; 
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandgetallusers = new MySqlCommand(queryGetBooks, conn);

                try
                {
                    conn.Open();


                    getallusers.Success = true;
                }
                catch(Exception e)
                {
                    getallusers.ErrorText = e.Message;
                }
                return getallusers;
            }
        }

        public static Response.BaseResponse AddBook(string ISBN,string BookTitle,string BookAuthor,int YearOfPublication,string Publisher)
        {
            Response.BaseResponse response = new Response.BaseResponse();
            string queryAddNewBook = "INSERT INTO `bx-books`(ISBN , Title , Author , Year , Publisher) VALUES (@ISBN,@Title,@Author,@Year,@Publisher)";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandAddBook = new MySqlCommand(queryAddNewBook,conn);
                mySqlCommandAddBook.Parameters.AddWithValue("@ISBN ", ISBN);
                mySqlCommandAddBook.Parameters.AddWithValue("@Title ", BookTitle);
                mySqlCommandAddBook.Parameters.AddWithValue("@Author ", BookAuthor);
                mySqlCommandAddBook.Parameters.AddWithValue("@Year" , YearOfPublication);
                mySqlCommandAddBook.Parameters.AddWithValue("@Publisher", Publisher);
            }

                return response;
        }

        public static Response.BaseResponse RemoveBook(string ISBN)
        {
            string queryRemoveBook = ("DELETE FROM `bx-books` WHERE ISBN = @ISBN");
            Response.BaseResponse removeBook = new Response.BaseResponse();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandRemoveBook = new MySqlCommand(queryRemoveBook,conn);
                try
                {
                    conn.Open();
                    mySqlCommandRemoveBook.ExecuteNonQuery();
                    removeBook.Success = true;
                }
                catch(Exception e)
                {
                    removeBook.ErrorText = e.Message;
                }
                return removeBook;
            }
        }

        public static Response.BaseResponse GetAllBooks()
        {
            string queryGetBooks = ("SELECT * FROM `bx-books`");
            Response.BaseResponse getallbooks = new Response.BaseResponse();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandgetallbooks = new MySqlCommand();
                try
                {
                    conn.Open();
                    getallbooks.Success = true;
                }
                catch (Exception e)
                {
                    getallbooks.ErrorText = e.Message;
                }
                return getallbooks;
            }
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


