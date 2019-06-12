Vue.component('view-roles', {
  props: ['block'],
  data: function () {
    return {
      currentRole: {},
      findedUsers: [],
      login: "",
      viewName: "",
      findedViews: [],
      newRole: {
        DisplayName: "",
        Id: 0,
        Name: "",
        Type: "Role"
      },
      isCreatingRole:false
    }
  },
  watch: {
    block: function () {
      this.currentRole = {};
    }
  },
  methods: {
    addRole: function () {
      this.isCreatingRole = true;
    },
    selectRole: function (role) {
      this.currentRole = role;
    },
    findPermissionByViewName: function () {
      var $this = this;
      app.editorGet("FindViewsByName", { name: this.viewName }, function (result) {
        $this.findedViews = result;
      });
    },
    addPermissionToRole: function (view) {
      var $this = this;
      var permission = {
        RoleId: $this.currentRole.Id,
        ViewId: view.Id
      }
      app.editorPost("AddPermissionToRole", permission, function (result) {
        $this.currentRole.Permissions.push(result[0]);
      });
    },
    updatePermission: function (permission) {
      app.editorPost("UpdatePermission", permission);
    },
    removePermissionFromRole: function (permission) {
      var $this = this;
      app.editorPost("RemovePermissionFromRole", permission, function () {
        $this.currentRole.Permissions.splice($this.currentRole.Permissions.indexOf(permission), 1);
      });
    },
    // Users
    findUsers: function () {
      var $this = this;
      app.editorGet("FindUsersByLogin", { login: this.login }, function (result) {
        $this.findedUsers = result;
      });
    },
    addUserToRole: function (user) {
      var $this = this;
      var userRole = {
        RoleId: this.currentRole.Id,
        UserId: user.Id
      };
      app.editorPost("AddUserToRole", userRole, function (result) {
        user.UserRoles = result;
        $this.currentRole.Users.push(user);
      });
    },
    updateUserRole: function (userRole) {
      app.editorPost("UpdateUserRole", userRole);
    },
    deleteUserFromRole: function (user) {
      var $this = this;
      app.editorPost("RemoveUserFromRole", user.UserRoles[0], function () {
        $this.currentRole.Users.splice($this.currentRole.Users.indexOf(user), 1);
      });
    },
    createRole: function () {
      var $this = this;
      if ($this.isCreatingRole) {
        var data = {
          Role: this.newRole,
          SectionId: $this.block.Props.SectionId
        };
        app.editorPost("CreateRoleWithSectionId", data, function (result) {
          $this.isCreatingRole = false;
          $this.block.Props.Roles.push(result[0]);
        });
      }
    },
    cancel: function () {
      if (this.isCreatingRole) {
        this.isCreatingRole = false;
      }
    },
    roleClass: function (role) {
      return role.Id == this.currentRole.Id ? 'table-primary' : '';
    }
  },
  template:
    "<div class='card'>" +

      "<div class='card-header'>" +
        "<template v-if='block.Props.DisplayName'>" +
          "<h5>{{block.Props.DisplayName}}</h5>" +
        "</template>" +
      "</div>" +

    "<div class='card-body'>" +

    "<div v-if='isCreatingRole' class='form-group'>" +
      "<label>Название</label><input v-model='newRole.DisplayName' class='form-control'/>" +
      "<label>Системное имя (англ.)</label><input v-model='newRole.Name' class='form-control'/>" +
    "</div>" +

    "<div v-else class='row'>" +

      // Роли
      "<div class='col-4'>" +
        "<table class='table table-bordered table-hover'>" +
          "<thead><tr><th>Роли</th></tr></thead>" +
          "<tbody>" +
            "<tr v-for='role in block.Props.Roles' :class='roleClass(role)'>" +
              "<td @click=selectRole(role)>{{role.DisplayName}}</td>" +
            "</tr>" +
          "</tbody>" +
        "</table>" +
      "</div>" +

      "<template v-if='currentRole.Id > 0'>" +

      "<div class='col-5'>" +

      // Поиск страниц

      "<div v-if='currentRole.CanBeEditedByCurrentUser' class='form-group'><input v-model='viewName' @keyup.enter='findPermissionByViewName' class='form-control' caption='Поиск страниц'/></div>" +

        "<table class='table table-bordered table-hover'>" +
          "<tbody>" +
          "<tr v-for='view in findedViews'>" +
            "<td @click=addPermissionToRole(view)>{{view.DisplayName}}</td>" +
            "<td style='width:10%'><button @click=addPermissionToRole(view) class='btn btn-success'><i class='fa fa-plus'></i></button></td>" +
          "</tr>" +
          "</tbody>" +
        "</table>" +

      // Страницы
      "<table class='table table-bordered table-hover'>" +

        "<thead><tr><th>Страницы</th><th colspan='2'><i class='fa fa-ban'></i> / <i class='fa fa-eye'></i> / <i class='fa fa-pencil-alt'></i></th></tr></thead>" +
        "<tbody>" +
          "<tr v-for='permission in currentRole.Permissions'>" +
            "<td>{{permission.ViewDisplayName}}</td>" +
            "<td>" +
              "<select :disabled='!currentRole.CanBeEditedByCurrentUser' v-model='permission.PermissionType' @change='updatePermission(permission)' type='radio' class='form-control'>" +
                "<option value=''>Нет доступа</option>" +
                "<option value='Read'>Чтение</option>" +
                "<option value='Write'>Запись</option>" +
              "</select>" +
            "</td>" +
            "<td><button @click=removePermissionFromRole(permission) v-if='currentRole.CanBeEditedByCurrentUser' class='btn btn-danger'><i class='fa fa-trash'></i></button></td>" +
          "</tr>" +
        "</tbody>" +
      "</table>" +

      "</div>" +

      // Поиск сотрудников
      "<div class='col-3'>" +

        "<div v-if='currentRole.CanBeEditedByCurrentUser' class='form-group'><input v-model='login' @keyup.enter='findUsers' class='form-control' caption='Поиск сотрудников'/></div>"+

        "<table class='table table-bordered table-hover'>" +
          "<tbody>" +
            "<tr v-for='user in findedUsers'>" +
              "<td>{{user.Login}}</td>" +
              "<td style='width:10%'><button @click=addUserToRole(user) class='btn btn-success'><i class='fa fa-plus'></i></button></td>" +
            "</tr>" +
          "</tbody>" +
        "</table>" +

        "<table class='table table-bordered table-hover'>" +
          "<thead><tr><th colspan='3'>Пользователи с доступом</th></tr></thead>" +
          "<tbody>" +
            "<tr v-for='user in currentRole.Users'>" +
              "<td>{{user.Login}}</td>" +
              "<td>" +
                "<label class='switch switch-pill switch-primary'>" +
                  "<input :disabled='!currentRole.CanBeEditedByCurrentUser' class='switch-input' type='checkbox' v-model='user.UserRoles[0].UserCanChangeRole' @change='updateUserRole(user.UserRoles[0])' />" +
                  "<span class='switch-slider'></span>" +
                "</label>" +
              "</td>"+
              "<td><button v-if='currentRole.CanBeEditedByCurrentUser' class='btn btn-danger'><i @click=deleteUserFromRole(user) class='fa fa-trash'></i></button></td>" +
            "</tr>" +
          "</tbody>" +
        "</table>" +

      "</div>" +

      "</template>" +

      "</div>" +

    "</div>" +

    "<div class='card-footer'>" +
      "<div class='form-group'>" +
        "<button v-if='newRole.Name && newRole.DisplayName' @click='createRole' class='btn btn-success'>Сохранить</button>" +
        "<button v-if='!isCreatingRole' @click='addRole' class='btn btn-primary ml-4'>Создать роль</button>" +
        "<button v-if='isCreatingRole' @click='cancel' class='btn btn-primary ml-4'>Отмена</button>" +
      "</div>" +
    "</div>" +

    "</div>"
});
