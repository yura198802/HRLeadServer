﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using Monica.Settings.DataAdapter.Models.Crm.Core;
using Monica.Settings.DataAdapter.Models.Dto;

namespace Monica.Settings.DataAdapter.Models.Crm
{
    public class t_levelorg : BaseModel
    {
        /// <summary>
        /// Тип уровня организации ( f|a )
        /// </summary>
        public string TypeLevel { get; set; }
        /// <summary>
        /// Ссылка на администратора
        /// </summary>
        public int AdmId { get; set; }
        /// <summary>
        /// Ссылка на вышестоящую организацию
        /// </summary>
        public int? Parent { get; set; }
        /// <summary>
        /// Наименование орг
        /// </summary>
        public string Caption { get; set; }
        /// <summary>
        /// ИНН орг(12 символов)
        /// </summary>
        public string Inn { get; set; }
        /// <summary>
        /// КПП орг ( 9 символов)
        /// </summary>
        public string Kpp { get; set; }
        /// <summary>
        /// ОКТМО орг (11 символов)
        /// </summary>
        public string Oktmo { get; set; }
        /// <summary>
        /// Сортировка структуры орг на экране
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Дата создания записи
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// Помечен к удалению
        /// </summary>
        public bool? IsDeleted { get; set; }
        public static implicit operator LevelOrgDto(t_levelorg levelorg)
        {
            return new LevelOrgDto()
            {
                Id = levelorg.Sysid,
                TypeLevel = levelorg.TypeLevel,
                Parent = levelorg.Parent == null ? 0 : (int)levelorg.Parent, 
                Caption = levelorg.Caption,
                Inn = levelorg.Inn,
                Kpp = levelorg.Kpp,
                Oktmo = levelorg.Oktmo,
                IsDeleted = levelorg.IsDeleted == null ? false : (bool)levelorg.IsDeleted
            };
        }
    }
}
