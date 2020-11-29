using Microsoft.EntityFrameworkCore;
using Monica.Core.DataBaseUtils;
using Monica.Core.DbModel.ModelCrm.Profile;

namespace Monica.Core.DbModel.ModelCrm.Core
{
    public class BaseDbContext : DbContext
    {
        private IDataBaseMain _dataBaseMain;

        public BaseDbContext(IDataBaseMain dataBaseMain)
        {
            _dataBaseMain = dataBaseMain;
        }
        public BaseDbContext()
        {

        }
        /// <summary>
        /// Игроки, менеджеры
        /// </summary>
        public DbSet<User> User { get; set; }
        /// <summary>
        /// Пользовательские роли
        /// </summary>
        public DbSet<UserRole> UserRole { get; set; }
        /// <summary>
        /// Сызяь между пользователем и ролью
        /// </summary>
        public DbSet<UserLinkRole> UserLinkRole { get; set; }
        /// <summary>
        /// Тип пользователя
        /// </summary>
        public DbSet<TypeUser> TypeUser { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_dataBaseMain == null)
                optionsBuilder.UseMySql("Server=84.201.188.15;Port=3306;Database=hackatonfinal_gpb;;User Id=develop;Password=$zXcvbnm512$;TreatTinyAsBoolean=true;charset=utf8;");
            else optionsBuilder.UseMySql(_dataBaseMain.ConntectionString);
        }
    }
}
