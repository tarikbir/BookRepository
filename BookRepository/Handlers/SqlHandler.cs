using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BookRepository.Response;
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
        internal static BaseResponse UpdateAllWeights(BackgroundWorker bgw)
        {
            BaseResponse response = new BaseResponse();
            string queryUpdate = "UPDATE `bx-books` SET `RatingWeight` = @Weight WHERE ISBN = @ISBN";

            var responseC = GetTotalAverageVotes();

            double C = 0.0;
            if (responseC.Success)
                C = responseC.Content;
            else
                response.ErrorText += responseC.ErrorText + ".\n";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                var allBooksResponse = GetAllBooks();
                try
                {
                    conn.Open();
                    if (allBooksResponse.Success)
                    {
                        int bookCount = allBooksResponse.Content.Count, index = 0;
                        foreach (Book item in allBooksResponse.Content)
                        {
                            MySqlCommand commandUpdate = new MySqlCommand(queryUpdate, conn);
                            commandUpdate.Parameters.AddWithValue("@ISBN", item.ISBN);

                            var countSumResponse = GetCountSumRating(item.ISBN);
                            
                            double weight;
                            if (countSumResponse.Content?.ElementAt(0) != 0)
                            {
                                double avg = (double)countSumResponse.Content.ElementAt(0) / (double)countSumResponse.Content.ElementAt(1);
                                weight = CommonLibrary.GetWeightRate((double)countSumResponse.Content.ElementAt(1), avg, C, CommonLibrary.MinBookToCountVote);
                            }
                            else
                            {
                                weight = 0;
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

        private static GenericResponse<List<int>> GetCountSumRating(string ISBN)
        {
            GenericResponse<List<int>> response = new GenericResponse<List<int>>();
            string queryGetSum = "SELECT SUM(`Book-Rating`), COUNT(`Book-Rating`) FROM `bx-book-ratings` WHERE ISBN = @ISBN";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand commandGetSum = new MySqlCommand(queryGetSum, conn);
                commandGetSum.Parameters.AddWithValue("@ISBN", ISBN);

                int count = 0;
                int sum = 0;

                try
                {
                    conn.Open();
                    using (MySqlDataReader reader = commandGetSum.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            sum = (int) GetSafeField<Decimal>(reader, 0);
                            count = (int) GetSafeField<Int64>(reader, 1);
                        }
                        else
                        {
                            throw new Exception("Can't read count and sum values for book.");
                        }
                    }
                    response.Content = new List<int>() { sum, count };
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.ErrorText += e.Message + " Error occured on update at " + ISBN + ".\n";
                }
            }
            return response;
        }

        private static BaseResponse UpdateWeight(string ISBN, double weight)
        {
            BaseResponse response = new BaseResponse();
            string queryUpdateWeight = "UPDATE `bx-books` SET `RatingWeight` = @Weight WHERE ISBN = @ISBN";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandUpdateWeight = new MySqlCommand(queryUpdateWeight, conn);
                mySqlCommandUpdateWeight.Parameters.AddWithValue("@ISBN", ISBN);
                mySqlCommandUpdateWeight.Parameters.AddWithValue("@Weight", weight);

                try
                {
                    conn.Open();

                    int rowsAffected = mySqlCommandUpdateWeight.ExecuteNonQuery();
                    if (rowsAffected != 1)
                    {
                        throw new Exception("Error updating weight. Rows affected: " + rowsAffected);
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

        public static GenericResponse<List<Book>> GetRecommendList()
        {
            GenericResponse<List<Book>> response = new GenericResponse<List<Book>>() { Content = new List<Book>() };
            /*string query = "SELECT * FROM `bx-books` A NATURAL JOIN (SELECT `Book-Rating`,`ISBN` FROM `bx-book-ratings` WHERE `User-ID` = @UserID) B";

            
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommand = new MySqlCommand(query, conn);
                mySqlCommand.Parameters.AddWithValue("@UserID", CommonLibrary.LoggedInUser.UserID);

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
            }*/
            return response;
        }

        public static GenericResponse<List<Book>> GetPopularList()
        {
            GenericResponse<List<Book>> response = new GenericResponse<List<Book>>() { Content = new List<Book>() };
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

        public static GenericResponse<List<Book>> GetHighRatedList()
        {
            GenericResponse<List<Book>> response = new GenericResponse<List<Book>>() { Content = new List<Book>() };
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

        public static GenericResponse<List<Book>> GetNewsList()
        {
            GenericResponse<List<Book>> response = new GenericResponse<List<Book>>() { Content = new List<Book>() };

            string query = "SELECT * FROM `bx-books` ORDER BY AddedDate DESC LIMIT 5";

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

        public static GenericResponse<List<User>> GetAllUsers()
        {
            string queryGetBooks = "SELECT * FROM `bx-users` WHERE Username IS NOT NULL";
            GenericResponse<List<User>> response = new GenericResponse<List<User>> { Content = new List<User>() };

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

        public static GenericResponse<List<User>> GetAllDataUsers()
        {
            string queryGetBooks = "SELECT * FROM `bx-users`";
            GenericResponse<List<User>> response = new GenericResponse<List<User>> { Content = new List<User>() };

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

        public static GenericResponse<List<Book>> GetAllBooks()
        {
            string queryGetBooks = "SELECT * FROM `bx-books`";
            GenericResponse<List<Book>> response = new GenericResponse<List<Book>> { Content = new List<Book>() };

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

        public static GenericResponse<List<Vote>> GetAllVotes()
        {
            string queryGetVotes = "SELECT * FROM `bx-book-ratings`";
            GenericResponse<List<Vote>> response = new GenericResponse<List<Vote>> { Content = new List<Vote>() };

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandGetAllVotes = new MySqlCommand(queryGetVotes, conn);
                try
                {
                    conn.Open();
                    MySqlDataReader reader = mySqlCommandGetAllVotes.ExecuteReader();

                    while (reader.Read())
                    {
                        Vote vote = new Vote();
                        try
                        {
                            vote.User.UserID = GetSafeField<UInt32>(reader, 0);
                            vote.Book.ISBN = GetSafeField<string>(reader, 1);
                            vote.Rating = (int) GetSafeField<SByte>(reader, 2);

                            response.Content.Add(vote);
                        }
                        catch (Exception e)
                        {
                            response.ErrorText += e.Message + "\n";
                            continue;
                        }
                    }
                    if (response.Content.Count <= 0) throw new Exception("No votes returned.");
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.ErrorText = e.Message;
                }
                return response;
            }
        }

        public static GenericResponse<List<Vote>> GetAllUserVotes(User user)
        {
            string queryGetVotes = "SELECT `ISBN`, `Book-Rating` FROM `bx-book-ratings` WHERE `User-ID` = @UserID";
            GenericResponse<List<Vote>> response = new GenericResponse<List<Vote>> { Content = new List<Vote>() };

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandGetAllVotes = new MySqlCommand(queryGetVotes, conn);
                mySqlCommandGetAllVotes.Parameters.AddWithValue("@UserID", user.UserID);

                try
                {
                    conn.Open();
                    MySqlDataReader reader = mySqlCommandGetAllVotes.ExecuteReader();

                    while (reader.Read())
                    {
                        Vote vote = new Vote();
                        try
                        {
                            vote.Book.ISBN = GetSafeField<string>(reader, 0);
                            vote.Rating = GetSafeField<int>(reader, 1);
                            response.Content.Add(vote);
                        }
                        catch (Exception e)
                        {
                            response.ErrorText += e.Message + "\n";
                            continue;
                        }
                    }
                    if (response.Content.Count <= 0) throw new Exception("No votes returned.");
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.ErrorText = e.Message;
                }
                return response;
            }
        }

        public static GenericResponse<List<Vote>> GetAllBookVotes(Book  book)
        {
            string queryGetVotes = "SELECT `User-ID`, `Book-Rating` FROM `bx-book-ratings` WHERE `ISBN` = @ISBN";
            GenericResponse<List<Vote>> response = new GenericResponse<List<Vote>> { Content = new List<Vote>() };

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandGetAllVotes = new MySqlCommand(queryGetVotes, conn);
                mySqlCommandGetAllVotes.Parameters.AddWithValue("@ISBN", book.ISBN);

                try
                {
                    conn.Open();
                    MySqlDataReader reader = mySqlCommandGetAllVotes.ExecuteReader();

                    while (reader.Read())
                    {
                        Vote vote = new Vote();
                        try
                        {
                            vote.User.UserID = GetSafeField<UInt32>(reader, 0);
                            vote.Rating = GetSafeField<int>(reader, 1);
                            response.Content.Add(vote);
                        }
                        catch (Exception e)
                        {
                            response.ErrorText += e.Message + "\n";
                            continue;
                        }
                    }
                    if (response.Content.Count <= 0) throw new Exception("No votes returned.");
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.ErrorText = e.Message;
                }
                return response;
            }
        }

        public static GenericResponse<User> GetUser(string username, string password)
        {
            string queryLogin = "SELECT * FROM `bx-users` WHERE username = @username AND password = @password";
            GenericResponse<User> response = new GenericResponse<User>();
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

        public static GenericResponse<Book> GetBook(string ISBN)
        {
            GenericResponse<Book> response = new GenericResponse<Book>();
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

        public static GenericResponse<int> GetVote(string userID, string bookISBN)
        {
            GenericResponse<int> response = new GenericResponse<int>();
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
                        response.Content = GetSafeField<SByte>(reader, 0);
                    }
                    else
                    {
                        response.Content = -1;
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

        internal static GenericResponse<double> GetTotalAverageVotes()
        {
            GenericResponse<double> response = new GenericResponse<double>();
            string queryGetC = "SELECT AVG(`Book-Rating`) FROM `bx-book-ratings`";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand commandGetC = new MySqlCommand(queryGetC, conn);
                    MySqlDataReader reader = commandGetC.ExecuteReader();
                    double C = 0.0;
                    if (reader.Read())
                    {
                        C = (double) GetSafeField<Decimal>(reader, 0);
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

        public static BaseResponse AddVote(string userID, string bookISBN, int vote)
        {
            BaseResponse response = new BaseResponse();
            string queryAddVote = "INSERT INTO `bx-book-ratings`(`User-ID`, `ISBN`, `Book-Rating`) VALUES (@UserID,@ISBN,@Rating)";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandAddVote = new MySqlCommand(queryAddVote, conn);
                mySqlCommandAddVote.Parameters.AddWithValue("@UserID", userID);
                mySqlCommandAddVote.Parameters.AddWithValue("@ISBN", bookISBN);
                mySqlCommandAddVote.Parameters.AddWithValue("@Rating", vote);

                try
                {
                    var countSumResponse = GetCountSumRating(bookISBN);

                    double weight;
                    if (countSumResponse.Content?.ElementAt(0) != 0)
                    {
                        double avg = (double)countSumResponse.Content.ElementAt(0) / (double)countSumResponse.Content.ElementAt(1);
                        weight = CommonLibrary.GetWeightRate((double)countSumResponse.Content.ElementAt(1), avg);
                    }
                    else
                    {
                        weight = 0;
                    }
                    UpdateWeight(bookISBN, weight);

                    conn.Open();

                    int rowsAffected = mySqlCommandAddVote.ExecuteNonQuery();
                    if (rowsAffected != 1)
                    {
                        throw new Exception("Error inserting the vote. Rows affected: " + rowsAffected);
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

        public static BaseResponse UpdateVote(string userID, string bookISBN, int vote)
        {
            BaseResponse response = new BaseResponse();
            string queryUpdateVote = "UPDATE `bx-book-ratings` SET `Book-Rating` = @Rating WHERE ISBN = @ISBN AND `User-ID` = @UserID";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandUpdateVote = new MySqlCommand(queryUpdateVote, conn);
                mySqlCommandUpdateVote.Parameters.AddWithValue("@UserID", userID);
                mySqlCommandUpdateVote.Parameters.AddWithValue("@ISBN", bookISBN);
                mySqlCommandUpdateVote.Parameters.AddWithValue("@Rating", vote);

                try
                {
                    var countSumResponse = GetCountSumRating(bookISBN);

                    double weight;
                    if (countSumResponse.Content?.ElementAt(0) != 0)
                    {
                        double avg = (double)countSumResponse.Content.ElementAt(0) / (double)countSumResponse.Content.ElementAt(1);
                        weight = CommonLibrary.GetWeightRate((double)countSumResponse.Content.ElementAt(1), avg);
                    }
                    else
                    {
                        weight = 0;
                    }
                    UpdateWeight(bookISBN, weight);

                    conn.Open();

                    int rowsAffected = mySqlCommandUpdateVote.ExecuteNonQuery();
                    if (rowsAffected != 1)
                    {
                        throw new Exception("Error updating the vote. Rows affected: " + rowsAffected);
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

        public static GenericResponse<UInt32> AddUser(User user, string password)
        {
            string queryAddUser = "INSERT INTO `bx-users`(`Username`, `Password`, `Location`, `Age`) VALUES (@Username,@Password,@Location,@Age)";
            GenericResponse<UInt32> response = new GenericResponse<UInt32>();
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

                    string queryGetID = "SELECT MAX(`User-ID`) FROM `bx-users`";
                    MySqlCommand mySqlCommandGetID = new MySqlCommand(queryGetID, conn);

                    UInt32 getID = 0;

                    using (MySqlDataReader reader = mySqlCommandGetID.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            getID = GetSafeField<UInt32>(reader, 0);
                        }
                    }
                    response.Content = getID;
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.ErrorText = e.Message;
                }
                return response;
            }
        }

        public static BaseResponse RemoveUser(User user)
        {
            string queryRemoveUser = "DELETE FROM `bx-users` WHERE Username=@username";
            BaseResponse response = new BaseResponse();
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

        public static BaseResponse AddBook(Book book)
        {
            BaseResponse response = new BaseResponse();
            string queryAddNewBook = "INSERT INTO `bx-books`(`ISBN`,`Book-Title`,`Book-Author`,`Year-Of-Publication`,`Publisher`,`Image-URL-S`,`Image-URL-M`,`Image-URL-L`,`AddedDate`) VALUES (@ISBN,@BookTitle,@BookAuthor,@YearOfPublication,@Publisher,@ImageURI_S,@ImageURI_M,@ImageURI_L,@AddedDate)";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandAddBook = new MySqlCommand(queryAddNewBook, conn);
                mySqlCommandAddBook.Parameters.AddWithValue("@ISBN", book.ISBN);
                mySqlCommandAddBook.Parameters.AddWithValue("@BookTitle", book.BookTitle);
                mySqlCommandAddBook.Parameters.AddWithValue("@BookAuthor", book.BookAuthor);
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

        public static BaseResponse RemoveBook(Book book)
        {
            string queryRemoveBook = "DELETE FROM `bx-books` WHERE ISBN = @ISBN";
            BaseResponse response = new BaseResponse();
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
        #endregion
    }
}


