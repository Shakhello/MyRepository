Vue.component('view-form', {
  props: ['block'],
  template:
    "<div class='form-horizontal'>" +
    "<control v-for='control in block.Controls' :control='control'></control>" +
    "</div>"
});

Vue.component('view-block', {
  props: ['block'],
  methods: {
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
      "<div class='card-header'>" +
        "<template v-if='block.Props.DisplayName'>" +
          "<h5>{{block.Props.DisplayName}}</h5>" +
        "</template>" +
        "</div>" +
        "<div class='card-body'>" +
        "<div class='row'>" +
          "<elm v-for='elm in block.Elements' :block='elm' @submit='submit'></elm>" +
        "</div>" +
      "</div>" +
    "</div>"
});

Vue.component('elm', {
  props: ['block'],
  data: function () {
    return {
      columnClass: 'col-' + app.getWidth(this.block)
    }
  },
  methods: {
    submit: function (events) {
      this.$emit("submit", events);
    },
    search: function (events, action) {
      this.$emit("search", events, action);
    },
    isBlock: function (type) {
      return ["Block", "Tab"].indexOf(type) >= 0;
    },
    bindControl: function (data) {
      app.bindControl(this, data);
    }
  },
  template:
    "<div :class='[columnClass]'>" +
    "<template v-if=\"block.Type == 'View'\">" +

      "<view-block v-if='isBlock(block.ViewType)' :block='block'></view-block>" +

      "<view-table v-else-if=\"block.ViewType === 'TableContainer'\" :block='block'></view-table>" +

      "<view-tab v-else-if=\"block.ViewType === 'TabContainer'\" :block='block'></view-tab>" +

      "<view-card v-else-if=\"block.ViewType === 'card'\" :block='block'></view-card>" +

      "<view-form v-else-if=\"block.ViewType === 'form'\" :block='block'></view-form>" +

      "<registry v-else-if=\"block.ViewType === 'registry'\" :block='block'></registry>" +

      "<view-roles v-else-if=\"block.ViewType === 'SectionsSettings'\" :block='block'></view-roles>" +

    "</template>" +

    "<template v-else-if=\"block.Type == 'Control'\">" +

      "<control-header v-if=\"block.ControlType ==='control-header'\" :control='block'></control-header>" +

      "<control-progress v-else-if=\"block.ControlType ==='control-progress'\" :control='block'></control-progress>" +

      "<control-text v-else-if=\"block.ControlType ==='control-text'\" :control='block'></control-text>" +

      "<control-badge v-else-if=\"block.ControlType ==='control-badge'\" :control='block'></control-badge>" +

      "<control-info-box v-else-if=\"block.ControlType ==='control-info-box'\" :control='block'></control-info-box>" +

      "<control-file v-else-if=\"block.ControlType ==='File'\" :control='block'></control-file>" +

      "<control v-else :control='block' @submit='submit' @bindControl='bindControl' @search='search'></control>" +

    "</template>" +

    "</div>"
});

Vue.component('page', {
  props: ['block'],
  template:
    "<div>" +
      "<template v-for='elm in block.Views'>" +
        "<template v-if=\"elm.ViewType === 'Block'\">" +
          "<view-block :block='elm' ></view-block>" +
        "</template>" +
        "<template v-else>" +
          "<elm :block='elm'></elm>" +
        "</template>" +
      "</template>" +
    "</div>"
});
