Vue.component('filter-input', {
  props: ['control'],
  methods: {
    filterInput: function () {
      this.$emit('filterInput')
    }
  },
  template: "<input type='text' class='form-control' v-model='control.Value' v-on:keyup.enter='filterInput' />"
});

Vue.component('filter-switch3', {
  props: ['control'],
  data: function () {
    return {
      options: [
        { Key: null, Value: 'Все' },
        { Key: true, Value: 'Да' },
        { Key: false, Value: 'Нет' }]
    }
  },
  methods: {
    changeSelectVal: function (val) {
      this.control.Value = val;
    },
    getClass: function (key) {
      return [{ 'btn-primary': this.control.Value === key, 'btn-outline-primary': this.control.Value !== key }];
    }
  },
  template:
    "<div class='btn-group'>" +
      "<button class='btn' v-for='opt in options' @click='changeSelectVal(opt.Key)' :class='getClass(opt.Key)'>{{opt.Value}}</button>" +
    "</div>"
});

Vue.component('filter-control', {
  props: ['control'],
  data: function () {
    return {
      columnClass: 'col-' + this.control.Props.Width
    }
  },
  methods: {
    search: function (events, action) {
      this.$emit("search", events, action);
    },
    filterInput: function () {
      this.$emit("filterInput");
    },
    withLabel: function (control) {
      return control.Type === "Filter";
    }
  },
  computed: {
    currentControl: function () {
      return app.selectComponent(this.control);
    }
  },
  template:
    "<div class='form-group' :class='[columnClass]'>" +

    "<label v-if='withLabel(control)'>{{control.Props.DisplayName}}</label>" +
    "<label v-else-if='!withLabel(control)'>&nbsp;</label>" +

    "<div>" +
    "<component :is='currentControl' :control='control' @search='search' @filterInput='filterInput'></component>" +
    "</div>" +

    "</div>"
});

Vue.component('view-filter', {
  props: ['block'],
  methods: {
    filterInput: function () {
      var searchEvent = app.getSearchEvent(this.block);
      var action = app.getSearchAction(searchEvent);
      this.search([searchEvent], action);
    },
    search: function (events, action) {
      this.$emit("search", events, action);
      var view = { Filters: this.block.Filters };
      action.ViewData = app.getViewCopy(view);

      app.executeEvent(events);
    },
    currentControl: function (control) {
      return app.selectComponent(control);
    },
    withLabel: function () {
      return false;
    }
  },
  computed: {
    controls: function () {
      _.each(this.block.Controls, function (i) {
        i.WithOutLabel = true;
      });
      return this.block.Controls;
    }
  },
  template:
    "<div>" +

    "<div class='row'>" +
      "<filter-control v-for='filter in block.Filters' :control='filter' @filterInput='filterInput' @search='search' :key='filter.Props.Id'></filter-control>" +
    "</div>" +

    "<div class='row'>" +
      "<elm v-for='(elm, index) in controls' :block='elm' :key='index' @search='search' class='ml-2'></elm>" +
    "</div>" +

    "</div>"
});
