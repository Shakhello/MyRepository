namespace CMS.DAL.Migrations
{
    using CMS.DAL.Common;
    using CMS.DAL.Models;
    using CMS.DAL.Resources;
    using Common.Config;
    using Common.Exceptions;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Data.Entity;


    internal sealed class Configuration : DbMigrationsConfiguration<CMS.DAL.Services.CMSContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "CMS.DAL.Services.CMSContext";
        }

        protected override void Seed(CMS.DAL.Services.CMSContext context)
        {
            PopulateRolesAndUsers(context);
        }

        private void PopulateRolesAndUsers(Services.CMSContext context)
        {
            Role superAdmins = context.Roles.FirstOrDefault(r => r.Name == Constants.ROLE_GLOBAL_ADMINS) ??
                context.Roles.Add(new Role() { Name = Constants.ROLE_GLOBAL_ADMINS, DisplayName = RC.GLOBAL_ADMINS_DISP_NAME });

            var userLogins = Config.KEY<string>("GlobalAdmins").Split(';');

            if (userLogins == null || userLogins.Count() == 0)
                throw new CustomValidationException("GlobalAdmins parameter must be defined in web.config.");

            foreach (var login in userLogins)
            {
                var user = context.Users.FirstOrDefault(u => u.Login.ToUpper() == login.ToUpper()) ?? 
                    context.Users.Add(new User() { Login = login.ToUpper() });

                var userAdminRole = context.UserRoles
                    .FirstOrDefault(ur => ur.UserId == user.Id && ur.RoleId == superAdmins.Id);

                if (userAdminRole == null)
                {
                    userAdminRole = new UserRole()
                    {
                        User = user,
                        Role = superAdmins,
                        UserCanChangeRole = true
                    };

                    context.UserRoles.Add(userAdminRole);
                }
            }
        }


    }
}
