using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private static T GetSafeField<T>(MySqlDataReader reader, int index)
        {
            if (!reader.IsDBNull(index))
                return reader.GetFieldValue<T>(index);
            return default(T);
        }

        private static Book GetBookFromReader(MySqlDataReader reader)
        {
            int year = 1920;
            Int32.TryParse(GetSafeField<string>(reader, 3), out year);
            Book Book = new Book()
            {
                ISBN = GetSafeField<string>(reader, 0),
                BookTitle = GetSafeField<string>(reader, 1).Trim(),
                BookAuthor = GetSafeField<string>(reader, 2),
                Publisher = GetSafeField<string>(reader, 4),
                ImageURI_S = GetSafeField<string>(reader, 5),
                ImageURI_M = GetSafeField<string>(reader, 6),
                ImageURI_L = GetSafeField<string>(reader, 7),
                YearOfPublication = year
            };
            return Book;
        }

        private static User GetUserFromReader(MySqlDataReader reader)
        {
            User user = new User()
            {
                UserID = GetSafeField<UInt32>(reader, 0),
                Username = GetSafeField<string>(reader, 1),
                Location = GetSafeField<string>(reader, 3),
                Age = GetSafeField<UInt32>(reader, 4),
                IsAdmin = GetSafeField<bool>(reader, 5)
            };
            return user;
        }
        #endregion

        #region SQL Responses
        public static Response.GenericResponse<List<Book>> GetPopularList()
        {
            Response.GenericResponse<List<Book>> response = new Response.GenericResponse<List<Book>>();
            response.Content = new List<Book>();
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
                            response.Content.Add(GetBookFromReader(reader));
                        }
                        catch (Exception e)
                        {
                            response.ErrorText += e.Message + "\n";
                            continue;
                        }
                    }
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.ErrorText = e.Message;
                }
            }
            return response;
        }

        public static Response.GenericResponse<List<Book>> GetHighRatedList()
        {
            Response.GenericResponse<List<Book>> response = new Response.GenericResponse<List<Book>>();
            response.Content = new List<Book>();
            string query = "SELECT * FROM `bx-books` ORDER BY RatingWeight DESC LIMIT 10";

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
                            response.Content.Add(GetBookFromReader(reader));
                        }
                        catch (Exception e)
                        {
                            response.ErrorText += e.Message + "\n";
                            continue;
                        }
                    }
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.ErrorText = e.Message;
                }
            }
            return response;
        }

        public static Response.GenericResponse<List<Book>> GetNewsList()
        {
            Response.GenericResponse<List<Book>> response = new Response.GenericResponse<List<Book>>();
            response.Content = new List<Book>();
            string query = "SELECT * FROM `bx-books` ORDER BY AddedDate DESC LIMIT 10";

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
                            response.Content.Add(GetBookFromReader(reader));
                        }
                        catch (Exception e)
                        {
                            response.ErrorText += e.Message + "\n";
                            continue;
                        }
                    }
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.ErrorText = e.Message;
                }
            }
            return response;
        }

        public static Response.GenericResponse<User> UserEntry(string username, string password)
        {
            string queryLogin = "SELECT * FROM `bx-users` WHERE username = @username AND password = @password";
            Response.GenericResponse<User> response = new Response.GenericResponse<User>();
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
                        response.Content = GetUserFromReader(reader);
                    }
                    else
                    {
                        throw new Exception("No matching username and password combination.");
                    }
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.ErrorText = e.Message;
                }
                return response;
            }
        }

        public static Response.GenericResponse<Book> GetBook(string ISBN)
        {
            Response.GenericResponse<Book> response = new Response.GenericResponse<Book>();
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
                    response.Content = Book;
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.ErrorText = e.Message;
                }
            }

            return response;
        }

        public static Response.GenericResponse<int> GetVote(string userID, string bookISBN)
        {
            Response.GenericResponse<int> response = new Response.GenericResponse<int>();
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
                        response.Content = GetSafeField<int>(reader, 0);
                    }
                    else
                    {
                        throw new Exception("No vote has been cast by user '" + userID + "' for the book '" + bookISBN + "'.");
                    }
                }
                catch (Exception e)
                {
                    response.ErrorText = e.Message;
                }
            }
            return response;
        }

        internal static Response.GenericResponse<double> GetTotalAverageVotes()
        {
            Response.GenericResponse<double> response = new Response.GenericResponse<double>();
            string queryGetC = "SELECT AVG(`Book-Rating`) FROM `bx-book-ratings`";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    MySqlCommand commandGetC = new MySqlCommand(queryGetC, conn);
                    MySqlDataReader reader = commandGetC.ExecuteReader();
                    double C = 0.0;
                    if (reader.Read())
                    {
                        C = GetSafeField<double>(reader, 0);
                    }
                    else
                    {
                        throw new Exception("Error getting mean value.");
                    }
                    response.Content = C;
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.ErrorText = e.Message;
                }
                return response;
            }
        }

        internal static Response.BaseResponse UpdateAllWeights(BackgroundWorker bgw)
        {
            Response.BaseResponse response = new Response.BaseResponse();
            string queryUpdate = "UPDATE `bx-books` SET `RatingWeight` = @Weight WHERE ISBN = @ISBN";
            string queryGetSum = "SELECT SUM(`Book-Rating`), COUNT(`Book-Rating`) FROM `bx-book-ratings` WHERE ISBN = @ISBN";

            var responseC = GetTotalAverageVotes();
            double C = 0.0;
            if (responseC.Success)
            {
                C = responseC.Content;
            }
            else
            {
                response.ErrorText += responseC.ErrorText + ".\n";
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    var allBooksResponse = GetAllBooks();
                    conn.Open();
                    if (allBooksResponse.Success)
                    {
                        int bookCount = allBooksResponse.Content.Count, index = 0;
                        foreach (Book item in allBooksResponse.Content)
                        {
                            MySqlCommand commandUpdate = new MySqlCommand(queryUpdate, conn);
                            commandUpdate.Parameters.AddWithValue("@ISBN", item.ISBN);
                            MySqlCommand commandGetSum = new MySqlCommand(queryGetSum, conn);
                            commandGetSum.Parameters.AddWithValue("@ISBN", item.ISBN);

                            int count = 0;
                            double sum = 0.0;
                            try
                            {
                                using (MySqlDataReader reader = commandGetSum.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        sum = GetSafeField<double>(reader, 0);
                                        count = (int)GetSafeField<Int64>(reader, 1);
                                    }
                                    else
                                    {
                                        throw new Exception("Can't read count and sum values for book " + item.ISBN + ".");
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                response.ErrorText += e.Message + " Error occured on update at " + item.ISBN + ".\n";
                                continue;
                            }

                            double weight;
                            if (count == 0)
                            {
                                weight = 0;
                            }
                            else
                            {
                                double avg = sum / count;
                                weight = CommonLibrary.GetWeightRate(count, avg, C, CommonLibrary.MinBookToCountVote);
                            }
                            commandUpdate.Parameters.AddWithValue("@Weight", weight);
                            int rowsAffected = commandUpdate.ExecuteNonQuery();
                            if (rowsAffected != 1)
                            {
                                throw new Exception("Error weight update. Rows affected: " + rowsAffected);
                            }
                            bgw.ReportProgress((100 * index++) / bookCount);
                        }
                    }
                    else
                    {
                        throw new Exception(allBooksResponse.ErrorText);
                    }
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.ErrorText = e.Message;
                }
            }
            return response;
        }

        public static Response.BaseResponse CastVote(string userID, string bookISBN, int vote)
        {
            Response.BaseResponse response = new Response.BaseResponse();
            string queryGetVotesForBook = "SELECT SUM(`Book-Rating`), COUNT(`Book-Rating`) FROM `bx-book-ratings` WHERE ISBN = @ISBN";
            string queryAddVote = "INSERT INTO `bx-book-ratings`(`User-ID`, `ISBN`, `Book-Rating`) VALUES (@UserID,@ISBN,@Rating)";
            string queryUpdateVote = "UPDATE `bx-book-ratings` SET `Book-Rating` = @Rating, `Weight`= @Weight WHERE ISBN = @ISBN AND `User-ID` = @UserID";

            var responsePrevVote = GetVote(userID, bookISBN);
            int prevVote;
            if (responsePrevVote.Success)
            {
                prevVote = responsePrevVote.Content;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlDataReader reader = null;
                    int voteFromTable;

                    if (reader.Read())
                    {
                        //This is an already voted book.
                        voteFromTable = (int)GetSafeField<Int64>(reader, 0);
                    }
                    else
                    {
                        //This is a newly voted book.
                        MySqlCommand commandAddVote = new MySqlCommand(queryAddVote, conn);
                        commandAddVote.Parameters.AddWithValue("@UserID", userID);
                        commandAddVote.Parameters.AddWithValue("@ISBN", bookISBN);
                        commandAddVote.Parameters.AddWithValue("@Rating", vote);
                        commandAddVote.Parameters.AddWithValue("@Weight", userID);

                    }
                }
                catch (Exception e)
                {
                    response.ErrorText = e.Message;
                }
            }
            return response;
        }

        public static Response.BaseResponse AddUser(User user, string password)
        {
            string queryAddUser = "INSERT INTO `bx-users`(`Username`, `Password`, `Location`, `Age`) VALUES (@Username,@Password,@Location,@Age)";
            Response.BaseResponse response = new Response.BaseResponse();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandAddUser = new MySqlCommand(queryAddUser, conn);
                mySqlCommandAddUser.Parameters.AddWithValue("@Username", user.Username);
                mySqlCommandAddUser.Parameters.AddWithValue("@Password", password);
                mySqlCommandAddUser.Parameters.AddWithValue("@Location", user.Location);
                mySqlCommandAddUser.Parameters.AddWithValue("@Age", user.Age);

                try
                {
                    conn.Open();
                    int rowsAffected = mySqlCommandAddUser.ExecuteNonQuery();
                    if (rowsAffected != 1)
                    {
                        throw new Exception("Error inserting into users table. Rows affected: " + rowsAffected);
                    }
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.ErrorText = e.Message;
                }
                return response;
            }
        }

        public static Response.BaseResponse RemoveUser(User user)
        {
            string queryRemoveUser = ("DELETE FROM `bx-users` WHERE Username=@username");
            Response.BaseResponse response = new Response.BaseResponse();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandRemoveUser = new MySqlCommand(queryRemoveUser, conn);
                mySqlCommandRemoveUser.Parameters.AddWithValue("@username", user.Username);

                try
                {
                    conn.Open();
                    int rowsAffected = mySqlCommandRemoveUser.ExecuteNonQuery();
                    if (rowsAffected != 1)
                    {
                        throw new Exception("Error removing from users table. Rows affected: " + rowsAffected);
                    }
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.ErrorText = e.Message;
                }
                return response;
            }
        }

        public static Response.GenericResponse<List<User>> GetAllUsers()
        {
            string queryGetBooks = "SELECT * FROM `bx-users` WHERE Username IS NOT NULL";
            Response.GenericResponse<List<User>> response = new Response.GenericResponse<List<User>> { Content = new List<User>() };

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandGetAllUsers = new MySqlCommand(queryGetBooks, conn);

                try
                {
                    conn.Open();

                    MySqlDataReader reader = mySqlCommandGetAllUsers.ExecuteReader();

                    while (reader.Read())
                    {
                        try
                        {
                            response.Content.Add(GetUserFromReader(reader));
                        }
                        catch (Exception e)
                        {
                            response.ErrorText += e.Message + "\n";
                            continue;
                        }
                    }
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.ErrorText = e.Message;
                }
                return response;
            }
        }

        public static Response.BaseResponse AddBook(Book book)
        {
            Response.BaseResponse response = new Response.BaseResponse();
            string queryAddNewBook = "INSERT INTO `bx-books`(ISBN,BookTitle,BookAuthor,YearOfPublication,Publisher,ImageURI_S,ImageURI_M,ImageURI_L,AddedDate) VALUES (@ISBN,@BookTitle,@BookAuthor,@YearOfPublication,@Publisher,@ImageURI_S,@ImageURI_M,@ImageURI_L,@AddedDate)";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandAddBook = new MySqlCommand(queryAddNewBook, conn);
                mySqlCommandAddBook.Parameters.AddWithValue("@ISBN ", book.ISBN);
                mySqlCommandAddBook.Parameters.AddWithValue("@BookTitle ", book.BookTitle);
                mySqlCommandAddBook.Parameters.AddWithValue("@BookAuthor ", book.BookAuthor);
                mySqlCommandAddBook.Parameters.AddWithValue("@YearOfPublication", book.YearOfPublication);
                mySqlCommandAddBook.Parameters.AddWithValue("@Publisher", book.Publisher);
                mySqlCommandAddBook.Parameters.AddWithValue("@ImageURI_S", book.ImageURI_S);
                mySqlCommandAddBook.Parameters.AddWithValue("@ImageURI_M", book.ImageURI_M);
                mySqlCommandAddBook.Parameters.AddWithValue("@ImageURI_L", book.ImageURI_L);
                var now = DateTime.Now;
                mySqlCommandAddBook.Parameters.AddWithValue("@AddedDate", now.ToString("yyyy-MM-dd HH:mm:ss"));

                try
                {
                    conn.Open();
                    int rowsAffected = mySqlCommandAddBook.ExecuteNonQuery();
                    if (rowsAffected != 1)
                    {
                        throw new Exception("Error inserting into books table. Rows affected: " + rowsAffected);
                    }
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.ErrorText = e.Message;
                }
            }
            return response;
        }

        public static Response.BaseResponse RemoveBook(Book book)
        {
            string queryRemoveBook = ("DELETE FROM `bx-books` WHERE ISBN = @ISBN");
            Response.BaseResponse response = new Response.BaseResponse();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandRemoveBook = new MySqlCommand(queryRemoveBook, conn);
                mySqlCommandRemoveBook.Parameters.AddWithValue("@ISBN", book.ISBN);
                try
                {
                    conn.Open();
                    int rowsAffected = mySqlCommandRemoveBook.ExecuteNonQuery();
                    if (rowsAffected != 1)
                    {
                        throw new Exception("Error removing from books table. Rows affected: " + rowsAffected);
                    }
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.ErrorText = e.Message;
                }
            }
            return response;
        }

        public static Response.GenericResponse<List<Book>> GetAllBooks()
        {
            string queryGetBooks = ("SELECT * FROM `bx-books`");
            Response.GenericResponse<List<Book>> response = new Response.GenericResponse<List<Book>> { Content = new List<Book>() };

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandGetAllBooks = new MySqlCommand(queryGetBooks, conn);
                try
                {
                    conn.Open();
                    MySqlDataReader reader = mySqlCommandGetAllBooks.ExecuteReader();

                    while (reader.Read())
                    {
                        try
                        {
                            response.Content.Add(GetBookFromReader(reader));
                        }
                        catch (Exception e)
                        {
                            response.ErrorText += e.Message + "\n";
                            continue;
                        }
                    }
                    if (response.Content.Count <= 0) throw new Exception("No books returned.");
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.ErrorText = e.Message;
                }
                return response;
            }
        }

        public static Response.BaseResponse Register(string username, int age, string password, string country, string county, string city, bool IsAdmin)
        {
            string Concat = string.Join(",", country, county, city);
            string queryUser = "INSERT INTO `bx-users`(`Username`, `Password`, `Location`, `Age`, `IsAdmin`) VALUES (@Username,@Password,@Location,@Age,@IsAdmin)";
            Response.BaseResponse response = new Response.BaseResponse();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandUser = new MySqlCommand(queryUser, conn);
                mySqlCommandUser.Parameters.AddWithValue("@Age", age);
                mySqlCommandUser.Parameters.AddWithValue("@Concat", Concat);
                mySqlCommandUser.Parameters.AddWithValue("@Username", username);
                mySqlCommandUser.Parameters.AddWithValue("@Password", password);
                mySqlCommandUser.Parameters.AddWithValue("@IsAdmin", IsAdmin);
                try
                {
                    conn.Open();
                    int rowsAffected = mySqlCommandUser.ExecuteNonQuery();
                    if (rowsAffected != 1)
                    {
                        throw new Exception("Error inserting into users table.");
                    }
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.ErrorText = e.Message;
                }
                return response;
            }
        }
        #endregion
    }
}


