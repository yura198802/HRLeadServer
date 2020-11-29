using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Monica.Settings.DataAdapter.Extensions;
using Monica.Settings.DataAdapter.Interfaces;
using Monica.Settings.DataAdapter.Models.Crm;
using Monica.Settings.DataAdapter.Models.Crm.Core;
using Monica.Settings.DataAdapter.Models.Dto;

namespace Monica.Settings.DataAdapter.DataAdapter
{
    /// <summary>
    /// Адаптер для работы с ролями пользователей
    /// </summary>
    public class RolesAdapter : IRolesAdapter
    {
        private SettingsDbContext _crmDbContext;
        public RolesAdapter(SettingsDbContext crmDbContext)
        {
            _crmDbContext = crmDbContext;
        }
        public async Task<IEnumerable<UserRoleDto>> GetRolesByLevelOrgAsync(int idOrg)
        {
            var result = new List<UserRoleDto>();
            try
            {
                var roles = _crmDbContext.userrole.Where(s => s.LevelOrgId == idOrg);
                if (roles.Count() == 0)
                    throw new Exception($"В  отсутствуют роли.");//{levelOrg.Caption}
                foreach (var role in roles)
                {
                    result.Add(role.Map(new UserRoleDto()));
                }
                return result;
            }
            catch (Exception e)
            {
                return result;
            }
        }
        public async Task<ResultCrmDb> AddRoleForLevelOrgAsync(RoleCreateArgs args)
        {
            var result = new ResultCrmDb();
            try
            {
                if ((await _crmDbContext.userrole.FirstOrDefaultAsync(r => r.LevelOrgId == args.IdLevelorg & r.Name == args.CaptionRole)) != null)
                {
                    result.AddError("", "Такая роль уже существует!");
                    return result;
                }
                var role = new userrole();
                role.LevelOrgId = args.IdLevelorg;
                role.Name = args.CaptionRole;
                role.Sysname = args.CaptionRole;
                await _crmDbContext.userrole.AddAsync(role);
                await _crmDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.AddError("", e.Message);
            }
            return result;
        }
        public async Task<ResultCrmDb> RemoveUserRoleAsync(int idRole)
        {
            var result = new ResultCrmDb();
            try
            {
                _crmDbContext.userlinkrole.RemoveRange(_crmDbContext.userlinkrole.Where(l => l.UserRoleId == idRole));
                _crmDbContext.userrole.Remove(await _crmDbContext.userrole.FirstOrDefaultAsync(ur => ur.Id == idRole));
                await _crmDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.AddError("", e.Message);
            }
            return result;
        }
        public async Task<ResultCrmDb> EditUserRoleAsync(int sysIdRole, string newName)
        {
            var result = new ResultCrmDb();
            try
            {
                var orgId = (await _crmDbContext.userrole.FirstOrDefaultAsync(x => x.Id == sysIdRole)).LevelOrgId;
                if ((await _crmDbContext.userrole.FirstOrDefaultAsync(ur => ur.Name == newName & ur.LevelOrgId == orgId)) != null)
                {

                    throw new Exception("такая роль уже существует!");
                }
                var role = await _crmDbContext.userrole.FirstOrDefaultAsync(ur => ur.Id == sysIdRole);
                role.Name = newName;
                await _crmDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.AddError("", e.Message);
            }
            return result;
        }
      
    }
}
