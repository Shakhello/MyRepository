Vue.component('control-daterange', {
  props: ['control'],
  mounted: function () {
    var vm = this;

    $(vm.$el).children("input").dateRangePicker({
      format: 'DD.MM.YYYY',
      separator: ' - ',
      autoClose: true,
      language: 'ru',
      showTopbar: false,
      monthSelect: true,
      yearSelect: true,
      showShortcuts: true,
      customShortcuts:
        [
          {
            name: 'Неделя',
            dates: function () {
              var start = moment().startOf('week').toDate();
              var end = new Date(Date.now());
              return [start, end];
            }
          },
          {
            name: 'Месяц',
            dates: function () {
              var start = moment().startOf('month').toDate();
              var end = new Date(Date.now());
              return [start, end];
            }
          },
          {
            name: 'Год',
            dates: function () {
              var start = moment().startOf('year').toDate();
              var end = new Date(Date.now());
              return [start, end];
            }
          }
        ]
    }).bind('datepicker-change', function (event, obj) {

      vm.control.Value = [new Date(obj.date1), new Date(obj.date2)];

      }).on('change', function (event) {
        var val = event.target.value;
        if (val) {
          var dates = val.split("-");
          var date1 = moment(dates[0], "DD.MM.YYYY").toDate();
          var date2 = moment(dates[1], "DD.MM.YYYY").toDate();
          vm.control.Value = [date1, date2];
        } else {
          vm.control.Value = null;
        }
    });
  },
  methods: {
    clear: function () {
      $(this.$el).children("input").data('dateRangePicker').clear("");
      this.control.Value = null;
    }
  },
  template: "<span><input type='text' class='form-control' /><i v-if='control.Value' @click.stop='clear' class='fa fa-times form-control-clear'></i></span>"
});
