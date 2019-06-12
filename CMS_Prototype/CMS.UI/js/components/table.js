Vue.component('control-sort', {
  props: ['control'],
  methods: {
    executeEvent: function () {
      app.executeControl(this);
      return false;
    }
  },
  computed: {
    faClass: function () {
      return "fa " + (this.control.Props.SortDirection == "desc" ? " fa-sort-amount-down" : "fa-sort-amount-up");
    }
  },
  template:
    "<a href='' @click.prevent='executeEvent'>{{control.Props.DisplayName}}<i :class='faClass' v-if='control.Props.IsCurrentSortControl'></i></a>"
});

Vue.component('view-table-header', {
  props: ['block', 'filters'],
  methods: {
    search: function (events, action) {
      this.$emit("search", events, action);
    }
  },
  template:
    "<thead>" +
      "<th v-for='control in block.Controls'>" +
        "<control-sort :control='control' @search='search'></control-sort>" +
      "</th>" +
    "</thead>"
});

Vue.component('view-table-col', {
  props: ['col'],
  methods: {
    currentControl: function (control) {
      return app.selectComponent(control);
    },
    style: function (control) {
      return app.getStyle(control);
    }
  },
  template:
    "<td>" +
      "<template v-for='control in col.Controls'>" +
        "<component :is='currentControl(control)' :control='control' :styles='style(control)'></component>" +
      "</template>" +
    "</td>"
});


Vue.component('view-table-row', {
  props: ['row'],
  template:
    "<tr>" +
      "<view-table-col v-for='(col, index) in row.ChildViews' :col='col' :key='index'></view-table-col>" +
    "</tr>"
});

Vue.component('view-table-body', {
  props: ['block'],
  template:
    "<tbody>" +
      "<view-table-row v-for='(row, index) in block.ChildViews' :row='row' :key='index'></view-table-row>" +
    "</tbody>"
});

Vue.component('control-pagging-button', {
  props: ['control'],
  methods: {
    executeEvent: function () {
      app.executeControl(this);
    }
  },
  template: "<li class='page-item' :class='{ active: control.Props.IsCurrent }'><a @click='executeEvent' class='page-link'>{{control.Value}}</a></li>"
});

Vue.component('view-table-footer', {
  props: ['block', 'filters'],
  methods: {
    search: function (events, action) {
      this.$emit("search", events, action);
    }
  },
  template:
    "<ul class='pagination'>" +
      "<control-pagging-button v-for='(control, index) in block.Controls' :control='control' :key='index' @search='search'></control-pagging-button>" +
    "</ul>"
});

Vue.component('view-table', {
  props: ['block'],
  methods: {
    search: function (events, action) {
      var view = app.getFilters(this.block);
      action.ViewData = app.getViewCopy(view);

      app.executeEvent(events);
    }
  },
  computed: {
    footer: function () {
      return _.find(this.block.ChildViews, function (i) { return i.ViewType == "TableFooter" });
    },
    filters: function () {
      return app.getFilters(this.block);
    }
  },
  template:
    "<div class='card'>" +

    "<template v-if='block.Props.DisplayName'>" +
      "<div class='card-header'><h5>{{block.Props.DisplayName}}</h5></div>" +
    "</template>" +

    "<div class='card-body'>" +
      "<view-filter :block='block'></view-filter>" +
      "<table class='table table-bordered'>" +
        "<template v-for='view in block.ChildViews'>" +
          "<view-table-header v-if=\"view.ViewType == 'TableHeader'\" :block='view' :filters='filters' @search='search'></view-table-header>" +
          "<view-table-body v-if=\"view.ViewType == 'TableBody'\" :block='view'></view-table-body>" +
        "</template>" +
      "</table>" +
      "<view-table-footer :block='footer' :filters='filters' @search='search'></view-table-footer>" +
    "</div>" +

    "</div>"
});
