﻿using System;
using System.IO;

namespace ideal_giggle
{
    class Program
    {
        static void Main(string[] args)
        {
            DataManager dm = new DataManager("DbData", "Users", "Posts", "Comments", "Votes");

            var succ = dm.CheckIfDataExists();

            if (!succ)
                return;


            // All files are present and the data loading may begin
            
        }
    }
}
