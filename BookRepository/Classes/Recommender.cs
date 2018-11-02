using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRepository
{
    public class Recommender
    {
        private List<Vote> fullVoteList;
        private List<User> fullUserList;

        public List<string> Recommend(User user)
        {
            if (user.Age == null || user.Age == 0) return null;
            string[] location = user.Location.Split(',');
            string country = location[2];
            string state = location[1];
            string city = location[0];
            fullVoteList = SqlHandler.GetAllVotes().Content;
            fullUserList = SqlHandler.GetAllDataUsers().Content;
            var popularBookVotesQuery = from v in fullVoteList group v by v.Book.ISBN into grp where grp.Count() > 50 select grp;
            var popularBookVotes = popularBookVotesQuery.SelectMany(group => group).Where((x) => x.Rating > 5);
            var userBookVotesQuery = from v in fullVoteList where user.UserID == v.User.UserID select v;
            var userBookVotes = new List<Vote>(userBookVotesQuery);
            var similiarUsers = from u in fullUserList where (u.Location.Contains(state) && u.Location.Contains(country) && user.Age >= u.Age-3 && user.Age <= u.Age+3) select u;

            List<KeyValuePair<string,double>> neighbours = new List<KeyValuePair<string, double>>();
            foreach (var item in similiarUsers)
            {
                if (user.UserID != item.UserID)
                {
                    var neighbourVotes = from v in popularBookVotes where v.User.UserID == item.UserID select v;
                    if (neighbourVotes.Count() <= 0) continue;
                    var n = NeighbouringMethod(userBookVotes, new List<Vote>(neighbourVotes));
                    neighbours.Add(new KeyValuePair<string, double>(item.UserID.ToString(), n));
                }
            }
            neighbours.Sort((x, y) => x.Value.CompareTo(y.Value));
            List<string> booksToRecommend = new List<string>();
            var userBooks = from u in userBookVotes select u.Book.ISBN;
            if (neighbours.Count() <= 0) return null;
            foreach (var item in neighbours)
            {
                var neighbourVotes = from v in popularBookVotes where item.Key == v.User.UserID.ToString() select v;
                foreach (var vote in neighbourVotes)
                {
                    if (vote.Rating >= 7 && !booksToRecommend.Contains(vote.Book.ISBN) && !userBooks.Contains(vote.Book.ISBN))
                    {
                        booksToRecommend.Add(vote.Book.ISBN);
                    }
                }
            }

            return booksToRecommend;
        }

        private double NeighbouringMethod(List<Vote> userVotes, List<Vote> neighbourVotes)
        {
            double result = Double.MaxValue;
            var sharedLikes = from u in userVotes from n in neighbourVotes where u.Book.ISBN == n.Book.ISBN select n;
            if (sharedLikes.Count() <= 0) return result;
            result = 0.0;
            foreach (var item in sharedLikes)
            {
                var userVoteQuery = from u in userVotes where u.Book.ISBN == item.Book.ISBN select u;
                Vote userVote = userVoteQuery.FirstOrDefault();
                result += VotePow(userVote, item);
            }
            result = Math.Sqrt(result) / sharedLikes.Count();
            return result;
        }

        private double VotePow(Vote user, Vote neighbour)
        {
            if (user == null || neighbour == null) return 0.0;
            int u = user.Rating, n = neighbour.Rating;
            double result = Math.Pow((n - u), 2);
            return result;
        }
    }
}
