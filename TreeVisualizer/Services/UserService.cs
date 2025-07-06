using TreeVisualizer.Models;
using MultipleChoice.Utils;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TreeVisualizer.Repositories;
using ZstdSharp.Unsafe;

namespace TreeVisualizer.Services
{
    public class UserService
    {
        UserRepository _userRepo;
        public UserService()
        {
            _userRepo = new UserRepository();
        }
        public bool Create(User user)
        {
            var passUtil = new SecurityUtil();
            user.Password = passUtil.HashPassword(user.Password);
            return _userRepo.CreateUser(user);
        }

        public User GetById(int id)
        {
            return _userRepo.GetById(id);

        }
        public User GetByUsername(string username)
        {

            return _userRepo.GetByUsername(username);
        }
        public User GetByEmail(string email)
        {

            return _userRepo.GetByEmail(email);
        }
        public List<User> GetAll()
        {

            return _userRepo.GetAll();
        }

        public void Update(User user)
        {
            if (_userRepo.GetById(user.Id) == null)
            {
                throw new Exception("User not found");
            }
            _userRepo.Update(user);
        }

        public void Delete(int id)
        {
            _userRepo.Delete(id);
        }
    }
}
