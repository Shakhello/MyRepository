Vue.component('sidebar-label', {
  props: ['control'],
  template:
    "<li class='nav-title'>{{control.Props.DisplayName}}</li>"
});

Vue.component('sidebar-item', {
  props: ['control'],
  methods: {
    executeEvent: function () {
      app.executeControl(this);
      return false;
    }
  },
  data: function () {
    return {
      fontClass: 'fa fa-' + (this.control.Props ? this.control.Props.Icon : ''),
      href: "#"
    }
  },
  template:
    "<li class='nav-item'>" +
    "<a class='nav-link' href='' @click.prevent='executeEvent' >" +
    "<i :class='[fontClass]'></i> {{control.Props.DisplayName}}" +
    "</a>" +
    "</li>"
});

Vue.component('sidebar-group', {
  props: ['control'],
  data: function () {
    return {
      fontClass: 'fa fa-' + (this.control.Props ? this.control.Props.Icon : ''),
      isOpen: false,
      defaultIcon: 'fa-user',
      currentIcon: 'fa-user'
    }
  },
  methods: {
    toggleOpen: function () {
      this.isOpen = !this.isOpen;
      return false;
    },
    checkSettings: function(){
      if(this.control.SettingsButton) {
        this.currentIcon = 'fa-cog';
      }
    },
    clearIcon: function () {
      this.currentIcon = this.defaultIcon;
    },
    goToSettings: function () {
      if (this.control.SettingsButton) {
        app.executeEvent(this.control.SettingsButton.Events);
      }
    }
  },
  template:
    "<li class='nav-item nav-dropdown' :class='{open:isOpen}' @mouseover='checkSettings' @mouseleave='clearIcon'>" +
      "<a class='nav-link nav-dropdown-toggle' href='' @click.prevent=toggleOpen>" +
        "<i class='fa' :class='[currentIcon]' @click.stop.prevent='goToSettings'></i> {{control.Props.DisplayName}}" +
      "</a>" +
      "<ul class='nav-dropdown-items'>" +
        "<sidebar-item v-for='control in control.ViewLinks' :control='control' :key='control.Props.Name'></sidebar-item>" +
      "</ul>" +
    "</li>"
});

Vue.component('sidebar', {
  props: ['block'],
  template:
    "<div class='sidebar'>" +
      "<nav class='sidebar-nav'>" +
        "<ul class='nav'>" +
          "<template v-for='control in block.Controls'>" +
            "<sidebar-label v-if=\"control.ControlType==='sidebar-label'\" :control='control'></sidebar-label>" +
            "<sidebar-item v-else-if=\"control.ControlType==='sidebar-item'\" :control='control'></sidebar-item>" +
            "<sidebar-group v-else-if=\"control.Type==='Section'\" :control='control'></sidebar-group>" +
          "</template>" +
        "</ul>" +
      "</nav>" +
    "</div>"
});
