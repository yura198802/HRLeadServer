using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.Crm.Settings;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelCrm.Profile;
using Monica.Core.DbModel.ModelDto.Roles;
using Monica.Core.DbModel.ModelDto.Users;

namespace Frgo.Dohod.DbModel.DataAdapter.Settings
{
    /// <summary>
    /// Адаптер для работы с ролями пользователей
    /// </summary>
    public class RolesAdapter : IRolesAdapter
    {
        private ReportDbContext _crmDbContext;
        public RolesAdapter(ReportDbContext crmDbContext)
        {
            _crmDbContext = crmDbContext;
        }
        public async Task<IEnumerable<UserRoleDto>> GetRolesByLevelOrgAsync(int idOrg)
        {
            var result = new List<UserRoleDto>();
            try
            {
                var roles = _crmDbContext.UserRole.Where(s => s.LevelOrgId == idOrg);
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
                if ((await _crmDbContext.UserRole.FirstOrDefaultAsync(r => r.LevelOrgId == args.IdLevelorg & r.Name == args.CaptionRole)) != null)
                {
                    result.AddError("", "Такая роль уже существует!");
                    return result;
                }
                var role = new UserRole();
                role.LevelOrgId = args.IdLevelorg;
                role.Name = args.CaptionRole;
                role.Sysname = args.CaptionRole;
                await _crmDbContext.UserRole.AddAsync(role);
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
                _crmDbContext.UserLinkRole.RemoveRange(_crmDbContext.UserLinkRole.Where(l => l.UserRoleId == idRole));
                _crmDbContext.UserRole.Remove(await _crmDbContext.UserRole.FirstOrDefaultAsync(ur => ur.Id == idRole));
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
                var orgId = (await _crmDbContext.UserRole.FirstOrDefaultAsync(x => x.Id == sysIdRole)).LevelOrgId;
                if ((await _crmDbContext.UserRole.FirstOrDefaultAsync(ur => ur.Name == newName & ur.LevelOrgId == orgId)) != null)
                {

                    throw new Exception("такая роль уже существует!");
                }
                var role = await _crmDbContext.UserRole.FirstOrDefaultAsync(ur => ur.Id == sysIdRole);
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
