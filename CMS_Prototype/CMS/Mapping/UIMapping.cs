using AutoMapper;
using CMS.UI;
using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace CMS.Mapping
{
    public class UIMapping : Profile
    {
        public UIMapping()
        {
            MapEnumsByStringValue();
            MapDALToDefinitions();
            MapDefinitionsToDAL();
        }

        private void MapDALToDefinitions()
        {
            CreateMap<DAL.Models.Event, EventDefinition>(MemberList.None)
                .ForMember(x => x.Type, m => m.UseValue(DefinitionType.Event))
                .ForMember(x => x.Entity, m => m.MapFrom(src => src))
                .ForMember(x => x.Actions, m => m.MapFrom(src => ToList(src.Actions)));


            CreateMap<DAL.Models.Action, ActionDefinition>(MemberList.None)
                .ForMember(x => x.Type, m => m.UseValue(DefinitionType.Action))
                .ForMember(x => x.Entity, m => m.MapFrom(src => src))
                .ForMember(x => x.Parameters, m => m.MapFrom(src => ToList(src.Parameters)));

            CreateMap<DAL.Models.Parameter, ParameterDefinition>(MemberList.None)
                .ForMember(x => x.Type, m => m.UseValue(DefinitionType.Parameter))
                .ForMember(x => x.Entity, m => m.MapFrom(src => src));

            CreateMap<DAL.Models.Section, SectionDefinition>(MemberList.None)
                .ForMember(x => x.Type, m => m.UseValue(DefinitionType.Section))
                .ForMember(x => x.Entity, m => m.MapFrom(src => src));

            CreateMap<DAL.Models.Style, StyleDefinition>(MemberList.None)
                .ForMember(x => x.Type, m => m.UseValue(DefinitionType.Style))
                .ForMember(x => x.Entity, m => m.MapFrom(src => src));

            CreateMap<DAL.Models.Template, TemplateDefinition>(MemberList.None)
                .ForMember(x => x.Type, m => m.UseValue(DefinitionType.Template))
                .ForMember(x => x.Entity, m => m.MapFrom(src => src));

            CreateMap<DAL.Models.Field, FieldDefinition>(MemberList.None)
                .ForMember(x => x.LinkedTemplateId, m => m.MapFrom(src => src.LinkedField != null ? (int?)src.LinkedField.TemplateId : null))
                .ForMember(x => x.Type, m => m.UseValue(DefinitionType.Field))
                .ForMember(x => x.Entity, m => m.MapFrom(src => src));

            CreateMap<DAL.Models.View, ViewDefinition>(MemberList.None)
                .ForMember(x => x.Type, m => m.UseValue(DefinitionType.View))
                .ForMember(x => x.Entity, m => m.MapFrom(src => src))
                .ForMember(x => x.Filters, m => m.MapFrom(src => ToList(src.Filters)))
                .ForMember(x => x.ChildViews, m => m.MapFrom(src => ToList(src.ChildViews)))
                .ForMember(x => x.Controls, m => m.MapFrom(src => ToList(src.Controls)));

            CreateMap<DAL.Models.Control, ControlDefinition>(MemberList.None)
                .ForMember(x => x.ControlFields, m => m.MapFrom(src => ToList(src.ControlFields)))
                .ForMember(x => x.Type, m => m.UseValue(DefinitionType.Control))
                .ForMember(x => x.Entity, m => m.MapFrom(src => src));

            CreateMap<DAL.Models.Filter, FilterDefinition>(MemberList.None)
                .ForMember(x => x.Type, m => m.UseValue(DefinitionType.Filter))
                .ForMember(x => x.FilterFields, m => m.MapFrom(src => ToList(src.FilterFields)))
                .ForMember(x => x.Entity, m => m.MapFrom(src => src))
                .ForMember(x => x.Fields, m => m.MapFrom(src => ToList(src.Fields)));

            CreateMap<DAL.Models.Dictionary, DictionaryDefinition>(MemberList.None)
                .ForMember(x => x.Type, m => m.UseValue(DefinitionType.Dictionary));

            CreateMap<DAL.Models.User, UserDefinition>(MemberList.None)
                .ForMember(x => x.Type, m => m.UseValue(DefinitionType.User))
                .ForMember(x => x.UserRoles, m => m.MapFrom(src => ToList(src.UserRoles)))
                .ForMember(x => x.Entity, m => m.MapFrom(src => src));

            CreateMap<DAL.Models.Role, RoleDefinition>(MemberList.None)
                .ForMember(x => x.Type, m => m.UseValue(DefinitionType.Role))
                .ForMember(x => x.Entity, m => m.MapFrom(src => src))
                .ForMember(x => x.Users, m => m.MapFrom(src => ToList(src.UserRoles.Select(ur => ur.User))))
                .ForMember(x => x.Permissions, m => m.MapFrom(src => ToList(src.Permissions)));

            CreateMap<DAL.Models.UserRole, UserRoleDefinition>(MemberList.None)
                .ForMember(x => x.Id, m => m.UseValue(0))
                .ForMember(x => x.Type, m => m.UseValue(DefinitionType.UserRole))
                .ForMember(x => x.Entity, m => m.UseValue<DAL.Models.Entity>(null));

            CreateMap<DAL.Models.Permission, PermissionDefinition>(MemberList.None)
                .ForMember(x => x.Type, m => m.UseValue(DefinitionType.Permission))
                .ForMember(x => x.ViewDisplayName, m => m.MapFrom(src => src.View.DisplayName))
                .ForMember(x => x.Entity, m => m.MapFrom(src => src));
        }

        private void MapDefinitionsToDAL()
        {
            CreateMap<EventDefinition, DAL.Models.Event>(MemberList.None)
                .ForMember(x => x.Id, m => m.MapFrom(src => SetNullIfZero(src.Id)));

            CreateMap<ActionDefinition, DAL.Models.Action>(MemberList.None)
                .ForMember(x => x.Id, m => m.MapFrom(src => SetNullIfZero(src.Id)));

            CreateMap<ParameterDefinition, DAL.Models.Parameter>(MemberList.None)
                .ForMember(x => x.FieldId, m => m.MapFrom(src => SetNullIfZero(src.FieldId)))
                .ForMember(x => x.Id, m => m.MapFrom(src => SetNullIfZero(src.Id)));

            CreateMap<SectionDefinition, DAL.Models.Section>(MemberList.None)
                .ForMember(x => x.Id, m => m.MapFrom(src => SetNullIfZero(src.Id)));

            CreateMap<ViewDefinition, DAL.Models.View>(MemberList.None)
                .ForMember(x => x.LinkedFieldId, m => m.MapFrom(src => SetNullIfZero(src.LinkedFieldId)))
                .ForMember(x => x.ParentViewId, m => m.MapFrom(src => SetNullIfZero(src.ParentViewId)))
                .ForMember(x => x.SectionId, m => m.MapFrom(src => SetNullIfZero(src.SectionId)))
                .ForMember(x => x.StyleId, m => m.MapFrom(src => SetNullIfZero(src.StyleId)))
                .ForMember(x => x.Id, m => m.MapFrom(src => SetNullIfZero(src.Id)));

            CreateMap<TemplateDefinition, DAL.Models.Template>(MemberList.None)
                .ForMember(x => x.Id, m => m.MapFrom(src => SetNullIfZero(src.Id)));

            CreateMap<FieldDefinition, DAL.Models.Field>(MemberList.None)
                .ForMember(x => x.Id, m => m.MapFrom(src => SetNullIfZero(src.Id)))
                .ForMember(x => x.LinkedFieldId, m => m.MapFrom(src => SetNullIfZero(src.LinkedFieldId)))
                .ForMember(x => x.DictionaryId, m => m.MapFrom(src => SetNullIfZero(src.DictionaryId)))
                .ForMember(x => x.Length, m => m.MapFrom(src => SetNullIfZero(src.Length)));

            CreateMap<ControlDefinition, DAL.Models.Control>(MemberList.None)
                .ForMember(x => x.FieldId, m => m.MapFrom(src => SetNullIfZero(src.FieldId)))
                .ForMember(x => x.MaxLength, m => m.MapFrom(src => SetNullIfZero(src.MaxLength)))
                .ForMember(x => x.StyleId, m => m.MapFrom(src => SetNullIfZero(src.StyleId)))
                .ForMember(x => x.Id, m => m.MapFrom(src => SetNullIfZero(src.Id)));

            CreateMap<FilterDefinition, DAL.Models.Filter>(MemberList.None)
                .ForMember(x => x.Order, m => m.MapFrom(src => SetNullIfZero(src.Order)))
                .ForMember(x => x.Width, m => m.MapFrom(src => SetNullIfZero(src.Width)))
                .ForMember(x => x.Id, m => m.MapFrom(src => SetNullIfZero(src.Id)));

            CreateMap<DictionaryDefinition, DAL.Models.Dictionary>(MemberList.None)
                .ForMember(x => x.Id, m => m.MapFrom(src => SetNullIfZero(src.Id)));

            CreateMap<UserDefinition, DAL.Models.User>(MemberList.None)
                .ForMember(x => x.Id, m => m.MapFrom(src => SetNullIfZero(src.Id)));

            CreateMap<RoleDefinition, DAL.Models.Role>(MemberList.None)
                .ForMember(x => x.UserRoles, m => m.MapFrom(src => GetUserRoles(src)))
                .ForMember(x => x.Id, m => m.MapFrom(src => SetNullIfZero(src.Id)));

            CreateMap<UserRoleDefinition, DAL.Models.UserRole>(MemberList.None);

            CreateMap<PermissionDefinition, DAL.Models.Permission>(MemberList.None)
                .ForMember(x => x.ViewId, m => m.MapFrom(src => SetNullIfZero(src.ViewId)))
                .ForMember(x => x.Id, m => m.MapFrom(src => SetNullIfZero(src.Id)));
        }

        private Collection<DAL.Models.UserRole> GetUserRoles(RoleDefinition src)
        {
            var userRoles = src.Users.Select(u => new DAL.Models.UserRole()
            {
                RoleId = src.Id,
                UserId = u.Id,
                UserCanChangeRole = u.UserRoles.FirstOrDefault(rm => rm.RoleId == src.Id).UserCanChangeRole
            })
            .Where(ur => ur.RoleId == src.Id)
            .ToList();

            return new Collection<DAL.Models.UserRole>(userRoles);
        }


        private void MapEnumsByStringValue()
        {
            CreateMap<DAL.Models.ActionType, UI.ActionType>()
                .ConvertUsing(value => EnumFromEnumByStringValue<UI.ActionType, DAL.Models.ActionType>(value));

            CreateMap<DAL.Models.ControlType, UI.ControlType>()
                .ConvertUsing(value => EnumFromEnumByStringValue<UI.ControlType, DAL.Models.ControlType>(value));

            CreateMap<DAL.Models.DictionaryType, UI.DictionaryType>()
                .ConvertUsing(value => EnumFromEnumByStringValue<UI.DictionaryType, DAL.Models.DictionaryType>(value));

            CreateMap<DAL.Models.EventType, UI.EventType>()
                .ConvertUsing(value => EnumFromEnumByStringValue<UI.EventType, DAL.Models.EventType>(value));

            CreateMap<DAL.Models.FieldType, UI.FieldType>()
                .ConvertUsing(value => EnumFromEnumByStringValue<UI.FieldType, DAL.Models.FieldType>(value));

            CreateMap<DAL.Models.FilterType, UI.FilterType>()
                .ConvertUsing(value => EnumFromEnumByStringValue<UI.FilterType, DAL.Models.FilterType>(value));

            CreateMap<DAL.Models.ParameterType, UI.ParameterType>()
                .ConvertUsing(value => EnumFromEnumByStringValue<UI.ParameterType, DAL.Models.ParameterType>(value));

            CreateMap<DAL.Models.TemplateType, UI.TemplateType>()
                .ConvertUsing(value => EnumFromEnumByStringValue<UI.TemplateType, DAL.Models.TemplateType>(value));

            CreateMap<DAL.Models.ViewType, UI.ViewType>()
                .ConvertUsing(value => EnumFromEnumByStringValue<UI.ViewType, DAL.Models.ViewType>(value));

            CreateMap<DAL.Models.OperationType, UI.OperationType>()
                .ConvertUsing(value => EnumFromEnumByStringValue<UI.OperationType, DAL.Models.OperationType>(value));

            CreateMap<DAL.Models.PermissionType, UI.PermissionType>()
                .ConvertUsing(value => EnumFromEnumByStringValue<UI.PermissionType, DAL.Models.PermissionType>(value));
        }

        private TDestEnum EnumFromEnumByStringValue<TDestEnum, TSourceEnum>(TSourceEnum sourceEnumValue) where TDestEnum : struct
        {
            var success = Enum.TryParse(Enum.GetName(typeof(TSourceEnum), sourceEnumValue), out TDestEnum result);

            if (success)
                return result;
            else
                throw new CustomValidationException($"Unable to map {typeof(TSourceEnum).ToString()}, value {Enum.GetName(typeof(TSourceEnum), sourceEnumValue)} to {typeof(TDestEnum).ToString()}");
        }

        private List<T> ToList<T>(IEnumerable<T> source)
        {
            if (source == null)
                return new List<T>();

            return source.ToList();
        }

        private int? SetNullIfZero(int? value)
        {
            return value == 0 ? null : value;
        }

    }
}
