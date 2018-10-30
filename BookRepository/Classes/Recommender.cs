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
            var voteQuery = from v in fullVoteList group v by v.Book.ISBN into grp where grp.Count() > 50 select grp;
            var userQuery = from u in fullUserList where (u.Location.Contains(user.Location.Split(',').ElementAt(2)) && user.Age >= u.Age-3 && user.Age <= u.Age+3) select u;
            
            foreach (var item in userQuery)
            {
                if (user.UserID != item.UserID)
                {
                    var userVoteQuery = from c in voteQuery select (from innerC in c where item.UserID == innerC.User.UserID select innerC);
                    foreach (var item2 in userVoteQuery)
                    {
                        var b = item2;
                    }
                }
            }
            /*
            foreach (IGrouping<string,Vote> item in voteQuery)
            {
                var b = item.ElementAt(0).Book.ISBN;
            }*/
        }
    }
}
