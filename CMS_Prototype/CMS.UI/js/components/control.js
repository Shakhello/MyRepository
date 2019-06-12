Vue.component('control-multiselect', {
  props: ['value', 'options', 'control'],
  template: "<select class='select2 form-control custom-select' style='width: 100%; height:32px;'><slot></slot></select>",
  mounted: function () {
    var vm = this;

    var options = _.map(this.control.Props.Options, function (item) {
      return { id: item.Key, text: item.Value };
    });

    var selectData = {
      multiple: true,
      data: options
    };

    $(this.$el)
      .select2(selectData)
      .val(vm.control.Value)
      .trigger('change')
      .on('change', function () {
        var values = _.map(this.selectedOptions, function (opt) {
          return opt.value;
        });
        vm.control.Value = values;
      });
  },
  watch: {
    value: function (value) {
      $(this.$el).val(value);
    }
  },
  destroyed: function () {
    $(this.$el).off().select2('destroy')
  }
});

Vue.component('control-autocomplete', {
  props: ['value', 'options', 'control'],
  template: "<select class='select2 form-control custom-select' style='width: 100%; height:32px;'><slot></slot></select>",
  data: function () {
    return {
      action: this.control.Events ? (this.control.Events.length > 0 ? this.control.Events[0].Actions[0] : null) : null
    }
  },
  mounted: function () {
    var vm = this;
    
    $(this.$el)
      .select2({
        data: [],
        minimumInputLength: 3,
        language: {
          inputTooShort: function () {
            return "Введите не менее 3-х символов";
          }
        },
        ajax: {
          dataType: 'json',
          delay: 250,
          data: function (params) {
            return {
              txt: params.term
            };
          },
          transport: function (params, success) {
            vm.control.Events[0].Actions[0].Value = params.data.txt;
            app.executeEvent(vm.control.Events, {}, function (result) {
              var data = result.Data || [];
              var options = _.map(data, function (item) {
                return { id: item.Key, text: item.Value };
              });
              success(options);
            });
          },
          processResults: function (items) {
            return {
              results: items,
              totals: items.length
            };
          }
        }
      })
      .val(this.value)
      .trigger('change')
      .on('change', function () {
        vm.control.Value = this.value;
      });
  },
  watch: {
    value: function (value) {
      var vm = this;
      if (value.length > 2) {
        vm.action.Value = value;
        
      }
    },
    options: function (options) {
      $(this.$el).empty().select2({ data: options })
    }
  },
  destroyed: function () {
    $(this.$el).off().select2('destroy')
  }
});

Vue.component('control-button', {
  props: ['control', 'styles'],
  mounted: function () {
    var vm = this;
    if (this.control.Events.length > 0) {
      var createActions = _.filter(this.control.Events[0].Actions, function (action) { return action.ActionType == "CreateDocument"; });
      _.each(createActions, function (action) {
        var paramsCopyFrom = _.filter(action.Parameters, function (p) {
          return p.ParameterType == "CopyValueFrom" && p.Props.ControlId > 0;
        });
        _.each(paramsCopyFrom, function (p) {
          vm.$emit('bindControl', {
            controlId: p.Props.ControlId, callback: function (value) {
              p.DefaultValue = value;
            }
          });
        });
      });
    }
  },
  methods: {
    executeEvent: function () {
      app.executeControl(this);
    }
  },
  computed: {
    controlClass: function () {
      if (!this.control.Props.Style)
        return 'btn-primary';
      return 'btn-' + (this.control.Props.Style.length > 0 ? this.control.Props.Style[0] : 'primary')
    }
  },
  template: "<button type='button' class='btn btn-primary' :style='styles' @click='executeEvent'>{{control.Props.DisplayName}}</button>"
});

Vue.component('control-date', {
  props: ['control'],
  mounted: function () {
    var vm = this;
    if (this.control.Value) {
      var d = new Date(this.control.Value);
      this.$el.value = (d.getDate() < 10 ? "0" + d.getDate() : d.getDate()) + "." + (d.getMonth() < 9 ? "0" + (d.getMonth() + 1) : (d.getMonth() + 1)) + "." + d.getFullYear();
    }
    $(vm.$el).datepicker({
      language: "ru",
      autoclose: true
    }).on('changeDate', function (event) {
      var date = new Date(Date.UTC(event.date.getFullYear(), event.date.getMonth(), event.date.getDate()));
      vm.control.Value = date;
    });
  },
  template: "<input type='text' class='form-control' />"
});

Vue.component('control-progress', {
  props: ['value'],
  template:
    "<div class='progress'>" +
      "<div class='progress-bar bg-warning progress-bar-animated' role='progressbar' :style=\"{ width: value + '%' }\" :aria-valuenow='value' aria-valuemin='0' aria-valuemax='100'></div>" +
    "</div>"
});

Vue.component('control-switch', {
  props: ['control'],
  template:
    "<div>" +
      "<label class='switch switch-pill switch-primary'>" +
        "<input class='switch-input' type='checkbox' v-model='control.Value' />" +
        "<span class='switch-slider'></span>" +
      "</label>" +
    "</div>"
});

Vue.component('control-input', {
  props: ['control'],
  template: "<input type='text' class='form-control' v-model='control.Value' />"
});

Vue.component('control-textarea', {
  props: ['control'],
  template: "<textarea type='text' class='form-control' v-model='control.Value' ></textarea>"
});

Vue.component('control-select', {
  props: ['control'],
  data: function () {
    return {
      selected: (this.control.Value && this.control.Value.length > 0) ? this.control.Value[0] : null
    }
  },
  watch: {
    selected: function (value) {
      if (value)
        this.control.Value = [value];
      else
        this.control.Value = null;
    }
  },
  template:
    "<select class='form-control' v-model='selected'>" +
      "<option v-for='option in control.Props.Options' :value='option.Key'>" +
        "{{ option.Value }}" +
      "</option>" +
    "</select>"
});

Vue.component('control-badge', {
  props: ['control'],
  template: "<span class='badge badge-success'>{{control.Props.DisplayName}}</span>"
});

Vue.component('control-header', {
  props: ['control'],
  template: "<h5>{{control.Props.DisplayName}}<br><b>{{control.Value}}</b></h5>"
});

Vue.component('control-label', {
  props: ['control', 'styles'],
  template: "<p :style='styles'>{{control.Value}}</p>"
});

Vue.component('control', {
  props: ['control'],
  data: function () {
    return {
      withlabel: !(this.control.WithOutLabel),
      columnClass: 'col-' + this.control.Props.Width
    }
  },
  computed: {
    currentControl: function () {
      return app.selectComponent(this.control);
    },
    isButton: function () {
      return this.control.ControlType.indexOf('Button') >= 0;
    },
    style: function () {
      return app.getStyle(this.control);
    }
  },
  methods: {
    submit: function (events) {
      this.$emit("submit", events);
    },
    search: function (events, action) {
      this.$emit("search", events, action);
    },
    bindControl: function (data) {
      app.bindControl(this, data);
    }
  },
  template:
    "<div class='form-group'>" +
      "<template v-if='withlabel'>" +
        "<label v-if='!isButton'>{{control.Props.DisplayName}}</label>" +
        "<label v-if='isButton'>&nbsp;</label>" +
      "</template>" +
      "<component :is='currentControl' :control='control' @submit='submit' @search='search' @bindControl='bindControl' :styles='style'></component>" +
    "</div>"
});
