﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRepository
{
    internal static class CommonLibrary
    {
        public static int MinBookToCountVote { get; set; }

        public static User LoggedInUser { get; private set; }

        internal static void LogInUser (User user)
        {
            if (LoggedInUser == null)
                LoggedInUser = user;
        }

        internal static double GetWeightRate(double v, double R)
        {
            /**<summary>Gets the weight rate of books (quick).</summary>
             * <param name="v">Number of votes for the book.</param>
             * <param name="R">Average rating for the book.</param>
             * <remarks>Weighted rating (WR) = (v ÷ (v+m)) × R + (m ÷ (v+m)) × C where:
             * R = average for the book (mean) = (Rating)
             * v = number of votes for the book = (votes)
             * m = minimum votes required to be listed
             * C = the mean vote across the whole report</remarks>
             **/
            double C = SqlHandler.GetTotalAverageVotes().Content;
            return GetWeightRate(v, R, C, MinBookToCountVote);
        }

        internal static double GetWeightRate(double v, double R, double C, double m)
        {
            /**<summary>Gets the weight rate of books.</summary>
             * <param name="v">Number of votes for the book.</param>
             * <param name="R">Average rating for the book.</param>
             * <param name="C">Mean vote across the whole report.</param>
             * <param name="m">Minimum votes required to be listed.</param>
             * <remarks>Weighted rating (WR) = (v ÷ (v+m)) × R + (m ÷ (v+m)) × C where:
             * R = average for the book (mean) = (Rating)
             * v = number of votes for the book = (votes)
             * m = minimum votes required to be listed
             * C = the mean vote across the whole report</remarks>
             **/
            return (v / (v + m)) * R + (m / (v + m)) * C;
        }
    }
}
