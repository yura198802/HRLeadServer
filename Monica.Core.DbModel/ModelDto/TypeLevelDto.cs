using Monica.Core.DbModel.ModelDto.Core;

namespace Monica.Core.DbModel.ModelDto
{
    /// <summary>
    /// модель данных для передачи типов уровней орг
    /// </summary>
    public class TypeLevelDto : BaseModelDto
    {
        public TypeLevelDto()
        {
            IsDeleted = false;
        }
        public string Caption { get; set; }
    }

}
