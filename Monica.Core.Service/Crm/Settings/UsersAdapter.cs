using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.Crm.Settings;
using Monica.Core.Abstraction.Registration;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelCrm.Profile;
using Monica.Core.DbModel.ModelDto;
using Monica.Core.ModelParametrs.ModelsArgs;

//using MonicaUser = Monica.Core.DbModel.ModelCrm.Profile.User;

namespace Monica.Core.Service.Crm.Settings
{
    /// <summary>
    /// Адаптер для работы с ролями пользователей
    /// </summary>
    public class UsersAdapter : IUsersAdapter
    {
        private IRegistrationUserAdapter _registration;
        private ReportDbContext _crmDbContext;
        public UsersAdapter(ReportDbContext crmDbContext)
        {
            _crmDbContext = crmDbContext;
        }
        public async Task<IEnumerable<User>> GetUsersByRoleAsync(int idRole)
        {
            var result = new List<User>();
            try
            {
                var links = await _crmDbContext.UserLinkRole.Where(r => r.UserRoleId == idRole).ToListAsync();
                if (links.Count() == 0)
                    throw new Exception($"В данной роли отсутствуют пользователи.");//{levelOrg.Caption}
                links.ForEach(r => result.Add(_crmDbContext.User.FirstOrDefault(u => u.Id == r.UserId)));
                return result;
            }
            catch (Exception e)
            {
                return result;
            }
        }
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var result = new List<User>();
            try
            {
                var users = await _crmDbContext.User.ToListAsync();
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
                await _crmDbContext.User.AddAsync(user);
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
                    var user = await _crmDbContext.User.FirstOrDefaultAsync(u =>u.Account.ToLower() == account.ToLower());
                    _crmDbContext.UserLinkRole.RemoveRange(_crmDbContext.UserLinkRole.Where((ur) => ur.UserId == user.Id));
                    users.Add(user);
                }
                await _crmDbContext.SaveChangesAsync();
                _crmDbContext.User.RemoveRange(users);
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
                var editable = await _crmDbContext.User.FirstOrDefaultAsync(c => c.Account.ToLower() == args.Account.ToLower());
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
                _crmDbContext.User.Update(editable);
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
                    var add = await _crmDbContext.UserLinkRole.FirstOrDefaultAsync(l => l.UserRoleId == idRole & l.UserId == id);
                    if (add != null)
                        continue;//throw new Exception("К роли уже добавлен данный пользователь.");
                    link.Add(new UserLinkRole() { UserId = id, UserRoleId = idRole });
                }
                await _crmDbContext.UserLinkRole.AddRangeAsync(link);
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
                var removeable = await _crmDbContext.UserLinkRole.FirstOrDefaultAsync(l => l.UserId == idUser & l.UserRoleId == idRole);
                if (removeable == null)
                    throw new Exception("Не обнаружена связь между пользователем и ролью.");
                _crmDbContext.UserLinkRole.Remove(removeable);
                await _crmDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.AddError("", e.Message);
            }
            return result;
        }
        public async Task<IEnumerable<User>> GetFreeAsync()
        {
            var result = new List<User>();
            try
            {
                var links = await _crmDbContext.UserLinkRole.ToListAsync();
                var users = await _crmDbContext.User.ToListAsync();
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
