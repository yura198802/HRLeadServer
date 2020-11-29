using Microsoft.EntityFrameworkCore;
using Monica.Core.DataBaseUtils;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelCrm.EngineReport;
using Monica.Settings.DataAdapter.Models.Crm.Profile;

namespace Monica.Settings.DataAdapter.Models.Crm
{
    public class SettingsDbContext : BaseDbContext
    {
        /// <summary>
        /// Строуктурный уровень администраторов
        /// </summary>
        public DbSet<t_levelorg> t_levelorg { get; set; }



        public DbSet<userrole> userrole { get; set; }
        public DbSet<User> user { get; set; }

        public DbSet<TypeUser> typeuser { get; set; }
        public DbSet<AccessForm> accessForm { get; set; }
        public DbSet<TypeForm> typeForm { get; set; }
        public DbSet<FormModel> formModel { get; set; }
        public DbSet<ButtonForm> buttonForm { get; set; }
        public DbSet<Field> field { get; set; }

        public DbSet<UserLinkRole> userlinkrole { get;set;}
        
        public SettingsDbContext(IDataBaseMain dataBaseMain) : base(dataBaseMain)
        {
        }
    }
}
