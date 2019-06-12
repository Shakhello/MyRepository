Vue.component('control-file-item', {
  props: ['file'],
  methods: {
    getEvent: function () {
      var vm = this;
      var event = {
        EventType: 0,
        Actions: [
          {
            ActionType: 10,
            Props: {
              FileId: vm.file.Id
            }
          }
        ]
      };

      return event;
    },
    download: function () {
      app.openURL('ticket' + "/" + 'DownloadFile?jsonEvent=' + JSON.stringify(this.getEvent()));
    },
    remove: function () {
      if (!confirm("Вы действительно хотите удалить файл?")) {
        return;
      }

      var vm = this;
      var event = vm.getEvent();
      event.Actions[0].ActionType = 11;
      if (vm.file.Id <= 0) {
        vm.$emit("deleteFile", vm.file);
      } else {
        app.executeEvent([event], {}, function () {
          vm.$emit("deleteFile", vm.file);
        });
      }
    }
  },
  template:
    "<li class='list-group-item'>" +
      "<div class='row'>" +
        "<div class='col-4'>" +
          "<span>{{file.Name}}</span>" +
        "</div>" +
        "<div class='col-6'>" +
          "<input v-if='file.Id <= 0' v-model='file.Comment' type='text' class='form-control'/>" +
          "<span v-if='file.Id > 0'>{{file.Comment}}</span>"+
        "</div>" +
        "<div class='col-2 text-right'>" +
          "<button class='btn btn-success' @click='download'><i class='fa fa-download'></i></button>" +
          "<button class='btn btn-danger ml-4' @click='remove'><i class='fa fa-trash'></i></button>" +
        "</div>" +
      "</div>" +
    "</li>"
});

Vue.component('control-file', {
  props: ['control'],
  data: function () {
    return {
      progress: 0,
      uploading: false
    }
  },
  computed: {
    fileInputName: function () {
      var vm = this;
      return 'fileInput_' + vm.control.DocId + '_' + vm.control.Props.FieldId;
    },
    fileUploadVisible: function () {
      return !this.uploading && _.some(this.control.Props.Files, function (f) {
        return f.Id <= 0;
      });
    },
    newFiles: function () {
      return _.filter(this.control.Props.Files, function (f) {
        return f.Id <= 0;
      });
    }
  },
  methods: {
    pickFiles: function () {
      $(this.$refs[this.fileInputName]).click();
    },
    addFiles: function (event) {
      var vm = this;

      if (event.target.files.length > 0) {
        // Удалить все незагруженные файлы из списка
        vm.control.Props.Files = _.filter(vm.control.Props.Files, function (f) {
          return f.Id > 0;
        });

        _.each(event.target.files, function (f) {
          vm.control.Props.Files.push({ Id: 0, Name: f.name, Comment: "", ContentType: f.type, file: f });
        });
      }
    },
    uploadFiles: function () {
      var vm = this;

      var data = new FormData();

      var event = vm.control.Events[0];

      _.each(vm.newFiles, function (file) {
        data.append(file.Name, file.file);
      });

      event.Actions[0].Value = vm.newFiles;

      data.append("data", JSON.stringify(event));

      vm.uploading = true;
      app.postForm('UploadFiles', data, function (eventResult) {
        // Удалить все незагруженные файлы из списка
        vm.control.Props.Files = _.filter(vm.control.Props.Files, function (f) {
          return f.Id > 0;
        });

        var files = eventResult[0].ActionResults[0].Data;

        _.each(files, function (file) {
          vm.control.Props.Files.push(file);
        });
        vm.uploading = false;
      }, function (progress) {
        vm.progress = progress;
      });
    },
    deleteFile: function (file) {
      this.control.Props.Files.splice(this.control.Props.Files.indexOf(file), 1);
    }
  },
  template:
    "<div class='form-group'>" +
      "<label>{{control.DisplayName}}</label>"+
      "<input style='display:none;' type='file' @change='addFiles' class='form-control-file' :ref='fileInputName'>" +
      "<div class='row'>" +
        "<div class='col-2'>" +
          "<button @click='pickFiles' class='btn btn-primary mb-1 mr-2'><i class='fa fa-plus'></i>Добавить файлы</button>" +
        "</div>" +
        "<div class='col-10'>" +
          "<button v-if='fileUploadVisible' @click='uploadFiles' class='btn btn-warning'><i class='fa fa-upload'></i>Загрузить файлы</button>" +
          "<control-progress v-if='uploading' :value='progress'></control-progress>" +
        "</div>" +
      "</div>" +
      "<ul class='list-group'>" +
        "<control-file-item v-for='file in control.Props.Files' :file='file' :key='file.Id' @deleteFile='deleteFile'></control-file-item>" +
      "</ul>" +
    "</div>"
});
