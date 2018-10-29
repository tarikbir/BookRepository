using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRepository
{
    public static class Recommender
    {
        private static List<Book> fullBookList;
        private static List<Vote> fullVoteList;
        private static List<User> fullUserList;

        public static void Recommend(User user)
        {
            fullBookList = SqlHandler.GetAllBooks().Content;
            fullVoteList = SqlHandler.GetAllVotes().Content;
            fullUserList = SqlHandler.GetAllDataUsers().Content;
            int[,] bigArray = new int[300000,300000];
            var bigData = from v in fullVoteList join b in fullBookList on v.Book.ISBN equals b.ISBN join u in fullUserList on v.User.UserID equals u.UserID select new { Book = b, User = u, Vote = v.Rating };

            foreach (var item in bigData)
            {
                
            }
        }
    }
}
