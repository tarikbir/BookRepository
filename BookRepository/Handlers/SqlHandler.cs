﻿using System;
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

                    bookResponse.Book = Book;
                    bookResponse.Success = true;
                }
                catch (MySqlException e)
                {
                    bookResponse.ErrorText = e.Message;
                }
                catch (Exception e)
                {
                    bookResponse.ErrorText = e.Message;
                }
            }

            return bookResponse;
        }

        public static Response.RegisterResponse Register(string username,int age, string country, string county, string city,string password)
        {
            string Concat = string.Join(",", country, county, city);
            string queryUser = "INSERT INTO `bx-users`(`Age`, `Location`) VALUES (@Age,@Concat)";
            string queryRegister = "INSERT INTO `bx-registry` VALUES (@Username,@UserID,@Password)";
            Response.RegisterResponse register = new Response.RegisterResponse();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommandUser = new MySqlCommand(queryUser, conn);
                MySqlCommand mySqlCommandRegister = new MySqlCommand(queryRegister, conn);
                mySqlCommandUser.Parameters.AddWithValue("@Age", age);
                mySqlCommandUser.Parameters.AddWithValue("@Concat", Concat);
                try
                {
                    conn.Open();
                    int rowsAffected = mySqlCommandUser.ExecuteNonQuery();
                    if(rowsAffected!=1)
                    {
                        throw new Exception("Error inserting into users table.");
                    }
                    Response.LastIDResponse lastID = GetLastID();
                    if(!lastID.Success)
                    {
                        throw new Exception(lastID.ErrorText);
                    }
                    mySqlCommandRegister.Parameters.AddWithValue("@Username",username);
                    mySqlCommandRegister.Parameters.AddWithValue("@UserID",lastID.LastID);
                    mySqlCommandRegister.Parameters.AddWithValue("@Password",password);
                    rowsAffected = mySqlCommandRegister.ExecuteNonQuery();
                    if (rowsAffected != 1)
                    {
                        throw new Exception("Error inserting into registry table.");
                    }
                    register.Success = true;
    

                }
                catch (MySqlException e)
                {
                    register.ErrorText = e.Message;
                }
                catch (Exception e)
                {
                    register.ErrorText = e.Message;
                }
                return register;
            }
        }
        public static Response.LastIDResponse GetLastID()
        {
            string query = "SELECT MAX(`User-ID`) FROM `bx-users`";
            string lastID;
            Response.LastIDResponse lastIDresponse = new Response.LastIDResponse();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand mySqlCommand = new MySqlCommand(query, conn);
                try
                {
                    conn.Open();
                    MySqlDataReader reader = mySqlCommand.ExecuteReader();

                    if (reader.Read())
                    {
                        lastID = reader.GetString(0);
                    }
                    else
                    {
                        throw new Exception("Unknown error has occured.");
                    }
                    lastIDresponse.LastID = lastID;
                    lastIDresponse.Success = true;
                }
                catch (MySqlException e)
                {
                    lastIDresponse.ErrorText = e.Message;
                }
                catch (Exception e)
                {
                    lastIDresponse.ErrorText = e.Message;
                }
            }
            return lastIDresponse;
        }
    }
}


