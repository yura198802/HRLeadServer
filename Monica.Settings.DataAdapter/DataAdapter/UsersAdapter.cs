using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.Registration;
using Monica.Core.ModelParametrs.ModelsArgs;
using Monica.Settings.DataAdapter.Interfaces;
using Monica.Settings.DataAdapter.Models.Crm;
using Monica.Settings.DataAdapter.Models.Crm.Core;
using Monica.Settings.DataAdapter.Models.Crm.Profile;
using Monica.Settings.DataAdapter.Models.Dto;

//using MonicaUser = Monica.Core.DbModel.ModelCrm.Profile.User;

namespace Monica.Settings.DataAdapter.DataAdapter
{
    /// <summary>
    /// Адаптер для работы с ролями пользователей
    /// </summary>
    public class UsersAdapter : IUsersAdapter
    {
        private IRegistrationUserAdapter _registration;
        private SettingsDbContext _crmDbContext;
        public UsersAdapter(SettingsDbContext crmDbContext)
        {
            _crmDbContext = crmDbContext;
        }
        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(int idRole)
        {
            var result = new List<UserDto>();
            try
            {
                var links = await _crmDbContext.userlinkrole.Where(r => r.UserRoleId == idRole).ToListAsync();
                if (links.Count() == 0)
                    throw new Exception($"В данной роли отсутствуют пользователи.");//{levelOrg.Caption}
                links.ForEach(r => result.Add(_crmDbContext.user.FirstOrDefault(u => u.Id == r.UserId)));
                return result;
            }
            catch (Exception e)
            {
                return result;
            }
        }
        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var result = new List<UserDto>();
            try
            {
                var users = await _crmDbContext.user.ToListAsync();
                foreach (var u in users)
                {
                    result.Add(u);
                }
                return result;
            }
            catch (Exception e)
            {
                return result;
            }
        }
        public async Task<ResultCrmDb> RegisterUserAsync(RegistrationUserArgs args)
        {
            var result = new ResultCrmDb();
            var errors = 0;
            List<string> textErrors = new List<string>();
            try
            {
                if (string.IsNullOrWhiteSpace(args.Account))
                {
                    textErrors.Add("аккаунт пользователя не указан");
                    errors++;
                }
                if (string.IsNullOrWhiteSpace(args.Surname))
                {
                    textErrors.Add("Фамилия пользователя не указана");
                    errors++;
                }
                if (errors > 0)
                    throw new Exception();
                var countUser = await _crmDbContext.User.CountAsync(c => c.Account.ToLower() == args.Account.ToLower());
                if (countUser > 0)
                {
                    textErrors.Add("пользователь с таким логином уже существует");
                    errors++;
                }
                var user = new User();
                user.Account = args.Account; 
                user.Email = args.Email;
                user.Phone = args.Phone;
                user.Name = args.Name;
                user.Surname = args.Surname;
                user.Middlename = args.Middlename;
                await _crmDbContext.user.AddAsync(user);
                await _crmDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var err = "";
                
                if (errors > 0)
                    err = string.Join(",\n", textErrors.ToArray());
                else
                    err = e.Message;
                var patternError = $"Ошибка регистрации пользователя:\n {err}.";
                result.AddError("",$"{patternError}" );

            }
            return result;
        }
        public async Task<ResultCrmDb> RemoveUsersAsync(IEnumerable<string> accounts)
        {
            var result = new ResultCrmDb();
            try
            {
                List<User> users = new List<User>();
                foreach(var account in accounts)
                {
                    var user = await _crmDbContext.user.FirstOrDefaultAsync(u =>u.Account.ToLower() == account.ToLower());
                    _crmDbContext.userlinkrole.RemoveRange(_crmDbContext.userlinkrole.Where((ur) => ur.UserId == user.Id));
                    users.Add(user);
                }
                await _crmDbContext.SaveChangesAsync();
                _crmDbContext.user.RemoveRange(users);
                await _crmDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.AddError("", e.Message);
            }
            return result;
        }
        public async Task<ResultCrmDb> EditUserAsync(RegistrationUserArgs args)
        {
            var result = new ResultCrmDb();
            var errors = 0;
            List<string> textErrors = new List<string>();
            try
            {
                if (string.IsNullOrWhiteSpace(args.Account))
                {
                    textErrors.Add("аккаунт пользователя не указан");
                    errors++;
                }
                if (string.IsNullOrWhiteSpace(args.Name))
                {
                    textErrors.Add("имя пользователя не указано");
                    errors++;
                }
                var editable = await _crmDbContext.user.FirstOrDefaultAsync(c => c.Account.ToLower() == args.Account.ToLower());
                if (editable == null)
                {
                    textErrors.Add("пользователь с таким логином отсутствует");
                    errors++;
                }
                if (errors > 0)
                    throw new Exception();
                editable.Email = args.Email;
                editable.Phone = args.Phone;
                editable.Name = args.Name;
                editable.Surname = args.Surname;
                editable.Middlename = args.Middlename;
                if(args.Password == args.ConfirmPassword && !string.IsNullOrWhiteSpace(args.Password))
                {
                        //запрос к is4 на смену пароля
                }
                _crmDbContext.user.Update(editable);
                await _crmDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var err = "";

                if (errors > 0)
                    err = string.Join(",\n", textErrors.ToArray());
                else
                    err = e.Message;
                var patternError = $"Ошибка регистрации пользователя:\n {err}.";
                result.AddError("", $"{patternError}");

            }
            return result;
        }
        public async Task<ResultCrmDb> AddUserToRoleAsync(int idRole, IEnumerable<int> idsUsers)
        {
            var result = new ResultCrmDb();
            try
            {
                var link = new List<UserLinkRole>();
                foreach (var id in idsUsers)
                {
                    var add = await _crmDbContext.userlinkrole.FirstOrDefaultAsync(l => l.UserRoleId == idRole & l.UserId == id);
                    if (add != null)
                        continue;//throw new Exception("К роли уже добавлен данный пользователь.");
                    link.Add(new UserLinkRole() { UserId = id, UserRoleId = idRole });
                }
                await _crmDbContext.userlinkrole.AddRangeAsync(link);
                await _crmDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.AddError("", e.Message);
            }
            return result;
        }
        public async Task<ResultCrmDb> RemoveUserFromRoleAsync(int idUser, int idRole)
        {
            var result = new ResultCrmDb();
            try
            {
                var removeable = await _crmDbContext.userlinkrole.FirstOrDefaultAsync(l => l.UserId == idUser & l.UserRoleId == idRole);
                if (removeable == null)
                    throw new Exception("Не обнаружена связь между пользователем и ролью.");
                _crmDbContext.userlinkrole.Remove(removeable);
                await _crmDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.AddError("", e.Message);
            }
            return result;
        }
        public async Task<IEnumerable<UserDto>> GetFreeAsync()
        {
            var result = new List<UserDto>();
            try
            {
                var links = await _crmDbContext.userlinkrole.ToListAsync();
                var users = await _crmDbContext.user.ToListAsync();
                foreach (var u in users)
                {
                    if (links.Exists(x => x.UserId == u.Id))
                    {
                        continue;
                    }
                    result.Add(u);
                }
                return result;
            }
            catch (Exception e)
            {
                return result;
            }
        }
    }
}
