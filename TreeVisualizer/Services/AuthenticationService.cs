using MultipleChoice.Utils;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeVisualizer.Models;
using TreeVisualizer.Repositories;

namespace TreeVisualizer.Services
{
    internal class AuthenticationService : BaseRepository
    {
        AuthenticationRepository _authRepo;
        public AuthenticationService()
        {
            _authRepo = new AuthenticationRepository();
        }
        public int Login(string email, string password)
        {
            var passUtil = new SecurityUtil();
            return _authRepo.Login(email, password);
        }
    }
}
