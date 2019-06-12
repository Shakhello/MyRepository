Vue.component('tabs-header', {
  props: ['block'],
  methods: {
    href: function (name) {
      return "#" + name;
    }
  },
  template:
    "<li class='nav-item'>" +
      "<a class='nav-link' :href='href(block.Props.Name)' role='tab' data-toggle='tab':class='{ active: block.Props.Active }'>{{block.Props.DisplayName}}</a>" +
    "</li>"
});

Vue.component('tabs-body', {
  props: ['block'],
  methods: {
    submit: function (events) {
      this.$emit("submit", events);
    },
    bindControl: function (data) {
      app.bindControl(this, data);
    }
  },
  template:
    "<div class='tab-pane' role='tabpanel' :id='block.Props.Name' :class='{ active: block.Props.Active }'>" +
      "<div class='row'>" +
        "<elm v-for='(elm, index) in block.Elements' :block='elm' :key='index' @bindControl='bindControl' @submit='submit'></elm>" +
      "</div>" +
    "</div>"
});

Vue.component('view-tab', {
  props: ['block'],
  methods: {
    bindControl: function (data) {
      app.bindControl(this, data);
    },
    submit: function (events) {

      var event = _.find(events, function (e) {
        return e.EventType === "Click";
      });

      var action = _.find(event.Actions, function (i) {
        return i.ActionType == "UpdateDocument";
      });

      var view = JSON.parse(JSON.stringify(this.block));
      action.ViewData = app.getViewCopy(view);

      app.executeEvent(events);
    }
  },
    template:
      "<div class='card'>" +

      "<div class='card-body'>" +
        "<ul class='nav nav-tabs' role='tablist'>" +
          "<tabs-header v-for='(tab, index) in block.ChildViews' :block='tab' :key='index' @bindControl='bindControl'></tabs-header>" +
        "</ul>" +

        "<div class='tab-content'>" +
          "<tabs-body v-for='(tab, index) in block.ChildViews' :block='tab' :key='index' @submit='submit'></tabs-body>" +
        "</div>" +

      "</div>" +

      "</div>"
});

Vue.component('breadcrumb', {
    props: ['block'],
    template:
        "<ol class='breadcrumb'>" +
            "<li class='breadcrumb-item' v-for='control in block.Controls'>" +
                "<template v-if='control.Active'>" +
                    "{{control.Props.DisplayName}}" +
                "</template>" +
                "<template v-else>" +
                    "<a :href='control.Value'>{{control.Props.DisplayName}}</a>" +
                "</template>" +
            "</li>" +
        "</ol>"
});
