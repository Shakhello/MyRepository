using CMS.Services;
using CMS.UI;
using Common;
using Common.Exceptions;
using Unity.Models;
using Unity.Resources;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Unity.Controllers
{
    /// <summary>
    /// Контроллер для работы с метаданными
    /// </summary>
    public class EditorController : BaseController
    {
        #region Templates

        /// <summary>
        /// Получает шаблон по ид с полями
        /// </summary>
        /// <returns>Шаблон с полями</returns>
        [HttpGet]
        [Route("api/editor/GetTemplate")]
        public Response<TemplateDefinition> GetTemplate(int id)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).GetTemplate(id);
            });
        }
    
        /// <summary>
        /// Получает все шаблоны
        /// </summary>
        /// <returns>Все шаблоны</returns>
        [HttpGet]
        [Route("api/editor/GetTemplates")]
        public Response<TemplateDefinition> GetTemplates()
        {
            return GetResponse<TemplateDefinition>(() =>
            {
                return new EditorService(User).GetTemplates();
            });
        }

        /// <summary>
        /// Создает новый шаблон
        /// </summary>
        /// <param name="template">Новый шаблон</param>
        /// <returns>Новый шаблон в случае успеха</returns>
        [HttpPost]
        [Route("api/editor/CreateTemplate")]
        public Response<TemplateDefinition> CreateTemplate([FromBody] TemplateDefinition template)
        {
            return GetResponse(() =>
            {
                ValidatePostModel(template);

                return new EditorService(User).CreateTemplate(template);
            });
        }

        /// <summary>
        /// Удаляет существующий шаблон по ид
        /// </summary>
        /// <param name="id">Ид шаблона</param>
        /// <returns>Успех удаления</returns>
        [HttpGet]
        [Route("api/editor/DeleteTemplate")]
        public Response DeleteTemplate(int id)
        {
            return GetResponse(() =>
            {
                new EditorService(User).DeleteTemplate(id);
            });
        }

        /// <summary>
        /// Обновляет шаблон
        /// </summary>
        /// <param name="template">Измененный шаблон</param>
        /// <returns>Обновленный шаблон в случае успеха</returns>
        [HttpPost]
        [Route("api/editor/UpdateTemplate")]
        public Response<TemplateDefinition> UpdateTemplate([FromBody] TemplateDefinition template)
        {
            return GetResponse(() =>
            {
                ValidatePostModel(template);

                return new EditorService(User).UpdateTemplate(template);
            });
        }

        #endregion

        #region Fields

        /// <summary>
        /// Создает поле
        /// </summary>
        /// <param name="field">Новое поле</param>
        /// <returns>Созданное поле в случае успеха</returns>
        [HttpPost]
        [Route("api/editor/CreateField")]
        public Response<FieldDefinition> CreateField([FromBody] FieldDefinition field)
        {
            return GetResponse(() =>
            {
                ValidatePostModel(field);

                return new EditorService(User).CreateField(field);
            });
        }

        /// <summary>
        /// Создает поля для связи двух шаблонов
        /// </summary>
        /// <param name="fields">Модель содержащая два поля</param>
        /// <returns>Созданное поле со ссылкой на второе созданное поле</returns>
        [HttpPost]
        [Route("api/editor/CreateReferenceField")]
        public Response<FieldDefinition> CreateReferenceField([FromBody] ReferenceFields fields)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).CreateReferenceField(fields.FieldA, fields.FieldB);
            });
        }

        /// <summary>
        /// Обновляет поле
        /// </summary>
        /// <param name="field">Измененное поле</param>
        /// <returns>Обновленное поле в случае успеха</returns>
        [HttpPost]
        [Route("api/editor/UpdateField")]
        public Response<FieldDefinition> UpdateField([FromBody] FieldDefinition field)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).UpdateField(field);
            });
        }

        /// <summary>
        /// Удаляет существующее поле по ид
        /// </summary>
        /// <param name="id">Ид поля</param>
        /// <returns>Успех удаления</returns>
        [HttpGet]
        [Route("api/editor/DeleteField/")]
        public Response DeleteField(int id)
        {
            return GetResponse(() =>
            {
                new EditorService(User).DeleteField(id);
            });
        }

        #endregion

        #region Dictionaries

        /// <summary>
        /// Получает все справочники
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/editor/GetDictionaries")]
        public Response<DictionaryDefinition> GetDictionaries()
        {
            return GetResponse<DictionaryDefinition>(() =>
            {
                return new EditorService(User).GetDictionaries();
            });
        }

        /// <summary>
        /// Получает все справочники
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/editor/GetDictionary")]
        public Response<DictionaryDefinition> GetDictionary(int id)
        {
            return GetResponse<DictionaryDefinition>(() =>
            {
                return new EditorService(User).GetDictionary(id);
            });
        }

        /// <summary>
        /// Создает справочник в БД
        /// </summary>
        /// <param name="control">Новый справочник</param>
        /// <returns>Созданный справочник</returns>
        [HttpPost]
        [Route("api/editor/CreateDictionary")]
        public Response<DictionaryDefinition> CreateDictionary([FromBody] DictionaryDefinition dict)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).CreateDictionary(dict);
            });
        }

        /// <summary>
        /// Удаляет справочник
        /// </summary>
        /// <param name="id">Ид справочника</param>
        /// <returns>Успешность выполнения</returns>
        [HttpGet]
        [Route("api/editor/DeleteDictionary")]
        public Response DeleteDictionary(int id)
        {
            return GetResponse(() =>
            {
                new EditorService(User).DeleteDictionary(id);
            });
        }

        /// <summary>
        /// Получает все значения определенного справочника
        /// </summary>
        /// <param name="dictId">Ид справочника</param>
        /// <returns>Значения справочника</returns>
        [HttpGet]
        [Route("api/ticket/GetDictionaryRecords")]
        public Response<DictionaryRecord> GetDictionaryRecords(int dictId)
        {
            return GetResponse<DictionaryRecord>(() =>
            {
                return new TicketService(User).GetDictionaryRecords(dictId);
            });
        }

        /// <summary>
        /// Добавляет запись в справочник
        /// </summary>
        /// <param name="record">Запись справочника</param>
        /// <returns>Успешность выполнения</returns>
        [HttpPost]
        [Route("api/ticket/AddDictionaryRecord")]
        public Response AddDictionaryRecord(DictionaryRecord record)
        {
            return GetResponse(() => new TicketService(User).AddDictionaryRecord(record));
        }

        /// <summary>
        /// Обновляет запись в справочнике
        /// </summary>
        /// <param name="record">Запись справочника</param>
        /// <returns>Успешность выполнения</returns>
        [HttpPost]
        [Route("api/ticket/UpdateDictionaryRecord")]
        public Response UpdateDictionaryRecord(DictionaryRecord record)
        {
            return GetResponse(() => new TicketService(User).UpdateDictionaryRecord(record));
        }

        /// <summary>
        /// Удаляет запись справочника
        /// </summary>
        /// <param name="dictId">Ид справочника</param>
        /// <param name="key">Ключ справочника</param>
        /// <returns>Успешность выполнения</returns>
        [HttpGet]
        [Route("api/ticket/DeleteDictionaryRecord")]
        public Response DeleteDictionaryRecord(int dictId, string key)
        {
            return GetResponse(() =>
            {
                new TicketService(User).DeleteDictionaryRecord(dictId, key);
            });
        }

        #endregion

        #region Views

        /// <summary>
        /// Получает представление по ид
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/editor/GetView")]
        public Response<ViewDefinition> GetView(int id)
        {
            return GetResponse(() =>
            {
                var viewDef = new EditorService(User).GetView(id);
                return viewDef;
            });
        }

        /// <summary>
        /// Получает все представления
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/editor/GetViews")]
        public Response<ViewDefinition> GetViews()
        {
            return GetResponse<ViewDefinition>(() =>
            {
                return new EditorService(User).GetViews();
            });
        }

        /// <summary>
        /// Находит представление по части названия
        /// </summary>
        /// <param name="name">Часть названия</param>
        /// <returns>Список представлений</returns>
        [HttpGet]
        [Route("api/editor/FindViewsByName")]
        public Response<ViewDefinition> FindViewsByName(string name)
        {
            return GetResponse<ViewDefinition>(() =>
            {
                return new EditorService(User).FindViewsByName(name);
            });
        }

        /// <summary>
        /// Создает представление в БД
        /// </summary>
        /// <param name="view"></param>
        /// <returns>Созданное представление</returns>
        [HttpPost]
        [Route("api/editor/CreateView")]
        public Response<ViewDefinition> CreateView([FromBody] ViewDefinition view)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).CreateView(view);
            });
        }

        /// <summary>
        /// Обновляет представление
        /// </summary>
        /// <param name="view">Измененное представление</param>
        /// <returns>Обновленное представление</returns>
        [HttpPost]
        [Route("api/editor/UpdateView")]
        public Response<ViewDefinition> UpdateView([FromBody] ViewDefinition view)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).UpdateView(view);
            });
        }

        /// <summary>
        /// Удаляет существующее представление по ид
        /// </summary>
        /// <param name="id">Ид представления</param>
        /// <returns>Успех удаления</returns>
        [HttpGet]
        [Route("api/editor/DeleteView")]
        public Response DeleteView(int id)
        {
            return GetResponse(() =>
            {
                new EditorService(User).DeleteView(id);
            });
        }

        #endregion

        #region Sections

        /// <summary>
        /// Получает секции доступные текущему пользователю
        /// </summary>
        /// <returns>Секции доступные текущему пользователю</returns>
        [Route("api/editor/GetSections/")]
        public Response<SectionDefinition> GetSections()
        {
            return GetResponse<SectionDefinition>(() =>
            {
                return new EditorService(User).GetSections();
            });
        }

        /// <summary>
        /// Создает секцию
        /// </summary>
        /// <param name="section">Новая секция</param>
        /// <returns>Созданная секция</returns>
        [Route("api/editor/CreateSection/")]
        public Response<SectionDefinition> CreateSection([FromBody] SectionDefinition section)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).CreateSection(section);
            });
        }

        /// <summary>
        /// Удаляет существующую секцию по ид
        /// </summary>
        /// <param name="id">Ид секции</param>
        /// <returns>Успех удаления</returns>
        [HttpGet]
        [Route("api/editor/DeleteSection/")]
        public Response DeleteSection(int id)
        {
            return GetResponse(() =>
            {
                new EditorService(User).DeleteSection(id);
            });
        }

        #endregion

        #region Controls

        /// <summary>
        /// Получает контрол по его ид
        /// </summary>
        /// <param name="id">Ид контрола</param>
        /// <returns>Контрол</returns>
        [HttpGet]
        [Route("api/editor/GetControl")]
        public Response<ControlDefinition> GetControl(int id)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).GetControl(id);
            });
        }

        /// <summary>
        /// Получает контрол по ид представления
        /// </summary>
        /// <param name="viewId">Ид представления</param>
        /// <returns>Контрол</returns>
        [HttpGet]
        [Route("api/editor/GetControlsByViewId")]
        public Response<ControlDefinition> GetControlsByViewId(int id)
        {
            return GetResponse<ControlDefinition>(() =>
            {
                return new EditorService(User).GetControlsByViewId(id);
            });
        }

        /// <summary>
        /// Создает контрол в БД
        /// </summary>
        /// <param name="control">Новый контрол</param>
        /// <returns>Созданный контрол</returns>
        [HttpPost]
        [Route("api/editor/CreateControl")]
        public Response<ControlDefinition> CreateControl([FromBody] ControlDefinition control)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).CreateControl(control);
            });
        }

        /// <summary>
        /// Обновляет контрол
        /// </summary>
        /// <param name="control">Измененный контрол</param>
        /// <returns>Обновленный контрол</returns>
        [HttpPost]
        [Route("api/editor/UpdateControl")]
        public Response<ControlDefinition> UpdateControl([FromBody] ControlDefinition control)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).UpdateControl(control);
            });
        }

        /// <summary>
        /// Удаляет существующий контрол по ид
        /// </summary>
        /// <param name="id">Ид контрола</param>
        /// <returns>Успех удаления</returns>
        [HttpGet]
        [Route("api/editor/DeleteControl")]
        public Response DeleteControl(int id)
        {
            return GetResponse(() =>
            {
                new EditorService(User).DeleteControl(id);
            });
        }

        /// <summary>
        /// Создает ControlField в БД
        /// </summary>
        /// <param name="control">Новый ControlField</param>
        /// <returns>Созданный ControlField</returns>
        [HttpPost]
        [Route("api/editor/CreateControlField")]
        public Response<ControlFieldDefinition> CreateControlField([FromBody] ControlFieldDefinition controlField)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).CreateControlField(controlField);
            });
        }

        /// <summary>
        /// Удаляет ControlField
        /// </summary>
        /// <param name="control">ControlField</param>
        /// <returns>Успешность удаления</returns>
        [HttpPost]
        [Route("api/editor/DeleteControlField")]
        public Response DeleteControlField([FromBody] ControlFieldDefinition controlField)
        {
            return GetResponse(() =>
            {
                new EditorService(User).DeleteControlField(controlField);
            });
        }

        #endregion

        #region Filters

        /// <summary>
        /// Получает фильтр по его ид
        /// </summary>
        /// <param name="id">Ид фильтра</param>
        /// <returns>Фильтр</returns>
        [HttpGet]
        [Route("api/editor/GetFilter")]
        public Response<FilterDefinition> GetFilter(int id)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).GetFilter(id);
            });
        }

        /// <summary>
        /// Получает фильтр по ид представления
        /// </summary>
        /// <param name="viewId">Ид представления</param>
        /// <returns>Фильтр</returns>
        [HttpGet]
        [Route("api/editor/GetFiltersByViewId")]
        public Response<FilterDefinition> GetFiltersByViewId(int viewId)
        {
            return GetResponse<FilterDefinition>(() =>
            {
                return new EditorService(User).GetFiltersByViewId(viewId);
            });
        }

        /// <summary>
        /// Создает фильтр в БД
        /// </summary>
        /// <param name="filter">Новый фильтр</param>
        /// <returns>Созданный фильтр</returns>
        [HttpPost]
        [Route("api/editor/CreateFilter")]
        public Response<FilterDefinition> CreateFilter([FromBody] FilterDefinition filter)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).CreateFilter(filter);
            });
        }

        /// <summary>
        /// Обновляет фильтр
        /// </summary>
        /// <param name="filter">Измененный фильтр</param>
        /// <returns>Обновленный фильтр</returns>
        [HttpPost]
        [Route("api/editor/UpdateFilter")]
        public Response<FilterDefinition> UpdateFilter([FromBody] FilterDefinition filter)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).UpdateFilter(filter);
            });
        }

        /// <summary>
        /// Удаляет существующий фильтр по ид
        /// </summary>
        /// <param name="id">Ид фильтр</param>
        /// <returns>Успех удаления</returns>
        [HttpGet]
        [Route("api/editor/DeleteFilter")]
        public Response DeleteFilter(int id)
        {
            return GetResponse(() =>
            {
                new EditorService(User).DeleteFilter(id);
            });
        }


        /// <summary>
        /// Создает FilterField в БД
        /// </summary>
        /// <param name="control">Новый FilterField</param>
        /// <returns>Созданный FilterField</returns>
        [HttpPost]
        [Route("api/editor/CreateFilterField")]
        public Response<FilterFieldDefinition> CreateFilterField([FromBody] FilterFieldDefinition filterField)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).CreateFilterField(filterField);
            });
        }

        /// <summary>
        /// Удаляет FilterField
        /// </summary>
        /// <param name="control">FilterField</param>
        /// <returns>Успешность удаления</returns>
        [HttpPost]
        [Route("api/editor/DeleteFilterField")]
        public Response DeleteFilterField([FromBody] FilterFieldDefinition filterField)
        {
            return GetResponse(() =>
            {
                new EditorService(User).DeleteFilterField(filterField);
            });
        }

        #endregion

        #region Styles

        /// <summary>
        /// Получает все стили
        /// </summary>
        /// <returns>Все стили</returns>
        [HttpGet]
        [Route("api/editor/GetStyles")]
        public Response<StyleDefinition> GetStyles()
        {
            return GetResponse<StyleDefinition>(() =>
            {
                return new EditorService(User).GetStyles();
            });
        }

        /// <summary>
        /// Получает стиль по ид
        /// </summary>
        /// <returns>Все стили</returns>
        [HttpGet]
        [Route("api/editor/GetStyle")]
        public Response<StyleDefinition> GetStyle(int id)
        {
            return GetResponse<StyleDefinition>(() =>
            {
                return new EditorService(User).GetStyle(id);
            });
        }

        /// <summary>
        /// Создает стиль
        /// </summary>
        /// <param name="field">Новый стиль</param>
        /// <returns>Созданный стиль в случае успеха</returns>
        [HttpPost]
        [Route("api/editor/CreateStyle")]
        public Response<StyleDefinition> CreateStyle([FromBody] StyleDefinition style)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).CreateStyle(style);
            });
        }

        /// <summary>
        /// Обновляет стиль
        /// </summary>
        /// <param name="field">Стиль</param>
        /// <returns>Обновленный стиль в случае успеха</returns>
        [HttpPost]
        [Route("api/editor/UpdateStyle")]
        public Response<StyleDefinition> UpdateStyle([FromBody] StyleDefinition style)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).UpdateStyle(style);
            });
        }

        /// <summary>
        /// Удаляет стиль по ид
        /// </summary>
        /// <param name="id">Ид стиля</param>
        /// <returns>Успешность выполнения</returns>
        [HttpGet]
        [Route("api/editor/DeleteStyle")]
        public Response DeleteStyle(int id)
        {
            return GetResponse(() =>
            {
                new EditorService(User).DeleteStyle(id);
            });
        }

        #endregion

        #region Events

        /// <summary>
        /// Получает событие по его ид
        /// </summary>
        /// <param name="id">Ид события</param>
        /// <returns>Событие</returns>
        [HttpGet]
        [Route("api/editor/GetEvent")]
        public Response<EventDefinition> GetEvent(int id)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).GetEvent(id);
            });
        }

        /// <summary>
        /// Создает событие в БД
        /// </summary>
        /// <param name="evt">Событие</param>
        /// <returns>Созданное событие</returns>
        [HttpPost]
        [Route("api/editor/CreateEvent")]
        public Response<EventDefinition> CreateEvent([FromBody] EventDefinition evt)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).CreateEvent(evt);
            });
        }

        /// <summary>
        /// Удаляет событие
        /// </summary>
        /// <param name="id">Ид события</param>
        /// <returns>Успешность удаления</returns>
        [HttpGet]
        [Route("api/editor/DeleteEvent")]
        public Response DeleteEvent(int id)
        {
            return GetResponse(() =>
            {
                new EditorService(User).DeleteEvent(id);
            });
        }

        #endregion

        #region Actions

        /// <summary>
        /// Создает экшен
        /// </summary>
        /// <param name="action">Экшен</param>
        /// <returns>Созданный экшен</returns>
        [HttpPost]
        [Route("api/editor/CreateAction")]
        public Response<ActionDefinition> CreateAction([FromBody] ActionDefinition action)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).CreateAction(action);
            });
        }

        /// <summary>
        /// Обновляет экшен
        /// </summary>
        /// <param name="action">Экшен</param>
        /// <returns>Обновленный экшен</returns>
        [HttpPost]
        [Route("api/editor/UpdateAction")]
        public Response<ActionDefinition> UpdateAction([FromBody] ActionDefinition action)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).UpdateAction(action);
            });
        }

        /// <summary>
        /// Удаляет экшен
        /// </summary>
        /// <param name="id">Ид экшена</param>
        /// <returns>Успешность удаления</returns>
        [HttpGet]
        [Route("api/editor/DeleteAction")]
        public Response DeleteAction(int id)
        {
            return GetResponse(() =>
            {
                new EditorService(User).DeleteAction(id);
            });
        }

        #endregion

        #region Parameters

        /// <summary>
        /// Создает параметр экшена
        /// </summary>
        /// <param name="parameter">Параметр</param>
        /// <returns>Созданный параметр</returns>
        [HttpPost]
        [Route("api/editor/CreateParameters")]
        public Response<ParameterDefinition> CreateParameters([FromBody] List<ParameterDefinition> parameters)
        {
            return GetResponse<ParameterDefinition>(() =>
            {
                return new EditorService(User).CreateParameters(parameters);
            });
        }

        /// <summary>
        /// Удаляет параметр
        /// </summary>
        /// <param name="id">Ид параметра</param>
        /// <returns>Успешность удаления</returns>
        [HttpGet]
        [Route("api/editor/DeleteParameters")]
        public Response DeleteParameters([FromUri] List<int> ids)
        {
            return GetResponse(() =>
            {
                new EditorService(User).DeleteParameters(ids);
            });
        }

        #endregion

        #region Users, Roles Permission

        /// <summary>
        /// Находит пользователя по части логина
        /// </summary>
        /// <param name="login">Часть логина пользователя</param>
        /// <returns>Пользователь</returns>
        [HttpGet]
        [Route("api/editor/FindUsersByLogin")]
        public Response<UserDefinition> FindUsersByLogin(string login)
        {
            return GetResponse<UserDefinition>(() =>
            {
                return new EditorService(User).FindUsersByLogin(login);
            });
        }

        /// <summary>
        /// Создает роль
        /// </summary>
        /// <param name="role">Новая роль</param>
        /// <returns>Созданная роль</returns>
        [HttpPost]
        [Route("api/editor/CreateRoleWithSectionId")]
        public Response<RoleDefinition> CreateRoleWithSectionId([FromBody] RoleWithSectionId role)
        {
            return GetResponse(() =>
            {
                return new EditorService(User).CreateRoleWithSectionId(role.Role, role.SectionId);
            });
        }

        /// <summary>
        /// Добавляет пользователя к роли
        /// </summary>
        /// <param name="userRole">Объект типа UserRoleDefinition</param>
        /// <returns>Созданный объект UserRoleDefinition</returns>
        [HttpPost]
        [Route("api/editor/AddUserToRole")]
        public Response<UserRoleDefinition> AddUserToRole(UserRoleDefinition userRole)
        {
            return GetResponse<UserRoleDefinition>(() =>
            {
                return new EditorService(User).AddUserToRole(userRole);
            });
        }

        /// <summary>
        /// Обновляет связь пользовтель-роль
        /// </summary>
        /// <param name="userRole">Объект типа UserRole</param>
        /// <returns>Успешность выполнения</returns>
        [HttpPost]
        [Route("api/editor/UpdateUserRole")]
        public Response UpdateUserRole(UserRoleDefinition userRole)
        {
            return GetResponse(() =>
            {
                new EditorService(User).UpdateUserRole(userRole);
            });
        }

        /// <summary>
        /// Удаляет пользователя из роли
        /// </summary>
        /// <param name="userRole">Объект типа UserRoleDefinition</param>
        /// <returns>Успешность выполнения</returns>
        [HttpPost]
        [Route("api/editor/RemoveUserFromRole")]
        public Response RemoveUserFromRole(UserRoleDefinition userRole)
        {
            return GetResponse(() =>
            {
                new EditorService(User).RemoveUserFromRole(userRole);
            });
        }

        /// <summary>
        /// Добавляет разрешение к роли
        /// </summary>
        /// <param name="permission">Новое разрешение</param>
        /// <returns>Созданное разрешение</returns>
        [HttpPost]
        [Route("api/editor/AddPermissionToRole")]
        public Response<PermissionDefinition> AddPermissionToRole(PermissionDefinition permission)
        {
            return GetResponse<PermissionDefinition>(() =>
            {
                return new EditorService(User).AddPermissionToRole(permission);
            });
        }

        /// <summary>
        /// Удаляет разрешение из роли
        /// </summary>
        /// <param name="userRole">Существующее разрешение</param>
        /// <returns>Успешность выполнения</returns>
        [HttpPost]
        [Route("api/editor/RemovePermissionFromRole")]
        public Response RemovePermissionFromRole(PermissionDefinition permission)
        {
            return GetResponse(() =>
            {
                new EditorService(User).RemovePermissionFromRole(permission);
            });
        }

        /// <summary>
        /// Обновляет разрешение
        /// </summary>
        /// <param name="permission">Разрешение</param>
        /// <returns>Успешность выполнения</returns>
        [HttpPost]
        [Route("api/editor/UpdatePermission")]
        public Response UpdatePermission(PermissionDefinition permission)
        {
            return GetResponse(() =>
            {
                new EditorService(User).UpdatePermission(permission);
            });
        }

        #endregion


    }
}