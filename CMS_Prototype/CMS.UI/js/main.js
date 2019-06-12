$(function () {

  var App = function () {
    var $this = this;

    var domen = "http://pc0128526/unity";
    //var domen = "http://localhost:58995";

    $this.baseUrl = domen + "/api/";

    $this.page = null;

    $this.sidebar = null;

    function updateBlock(block) {
      // Объеденить блоки и контролы в один список
      block.Filters = _.filter(block.Filters, function (f) {
        return f.FilterType != "Hidden";
      });

      block.Elements = _.sortBy(_.union(block.Views, block.Controls, block.Filters, block.ChildViews), function (i) {
        return i.Props.Order;
      });

      // Заходим в каждый элемент и если это блок, то повторяем
      _.each(block.Views, function (block) {
        updateBlock(block);
      });

      // Заходим в каждый элемент и если это блок, то повторяем
      _.each(block.ChildViews, function (block) {
        updateBlock(block);
      });
    }

    function initApp(sidebarJson) {

      $this.sidebar = new Vue({
        el: '#sidebar',
        data: {
          data: sidebarJson
        }
      });

      $this.page = new Vue({
        el: '#page',
        data: {
          data: {
            Views: []
          }
        }
      });
    }

    function loadFromServer(controller, method, methodType, data, callback) {
      var data = (methodType === "GET" ? data : JSON.stringify(data))
      $.ajax({
        url: $this.baseUrl + controller + "/" + method,
        type: methodType,
        data: data,
        dataType: "json",
        cache: false,
        crossDomain: true,
        xhrFields: {
          withCredentials: true
        },
        headers: {
          "Content-Type": "application/json; charset=utf-8"
        },
        success: function (response) {
          if (response.Status == 3) {
            alert(response.Messages[0]);
          } else {
            if (callback) {
              callback(response.Objects);
            }
          }
        },
        error: function (xhr) {
          alert(xhr.responseText);
        }
      });
    };

    function getSections(callback) {
      loadFromServer("ticket", "GetSections", "GET", {}, function (data) {
        if (data.length == 0) {
          alert("Не найдено ни одной секции. Возможно у вас слетели права.");
          return;
        }
        callback(data);
      });
    }

    function loadSidebar(callback) {
      getSections(function (data) {
        callback({ Controls: data });
      });
    };

    function loadPage(data) {

      updateBlock(data);

      $this.page.data.Views = [data];
    };

    function goTo(data) {
      loadPage(data);
      window.scrollTo(0, 0);
    };

    function isGotoActionType(action) {
      var actionType = action.ActionType;
      return ["OpenSection", "Goto", "Search", "GetSectionSettings"].indexOf(actionType) >= 0;
    }

    function isSaveActionType(action) {
      var actionType = action.ActionType;
      return ["CreateDocument", "UpdateDocument", "DeleteDocument"].indexOf(actionType) >= 0;
    }

    function isCreateActionType(action) {
      var actionType = action.ActionType;
      return ["CreateDocument"].indexOf(actionType) >= 0;
    }

    function isDeleteActionType(action) {
      var actionType = action.ActionType;
      return ["DeleteDocument"].indexOf(actionType) >= 0;
    }

    function isSearchActionType(action) {
      var actionType = action.ActionType;
      return ["Search"].indexOf(actionType) >= 0;
    }

    function isCallbackAction(actionType) {
      return ["SearchTicketsWithAutoComplete", "DeleteFile"].indexOf(actionType) >= 0;
    }

    function isGotoEvent(event) {
      return _.some(event.Actions, isGotoActionType);
    }

    function isSaveEvent(event) {
      return _.some(event.Actions, isSaveActionType);
    }

    function isDeleteEvent(event) {
      return _.some(event.Actions, isDeleteActionType);
    }

    function getEvents() {
      var events = localStorage.getItem('unity.events');

      if (!(events)) {
        events = [];
      } else {
        try {
          events = JSON.parse(events);
        }
        catch (e) {
          console.log(e);
          events = [];
        }
      }

      if (!_.isArray(events)) {
        events = [];
      }

      // Оставить последние 10
      if (events.length > 10) {
        events = events.slice(events.length - 10);
      }

      return events;
    }

    function saveEvent(event) {
      var events = getEvents();

      events.push(event);

      localStorage.setItem('unity.events', JSON.stringify(events));
    }

    function goBack(options) {
      var events = getEvents();

      if (events.length > 1) {

        events.pop();

        var event = events.pop();

        if (event) {
          localStorage.setItem('unity.events', JSON.stringify(events));

          $this.executeEvent([event], options);
        }
      }
    }

    goToLast = function () {
      var events = getEvents();

      if (events.length > 0) {

        var event = events[events.length - 1];

        if (event) {
          localStorage.setItem('unity.events', JSON.stringify(events));

          $this.executeEvent([event], { saveEvent: false });
        }
      }
    }

    $this.getFilters = function (view) {
      return {
        Filters: JSON.parse(JSON.stringify(view.Filters))
      };
    };

    $this.getViewCopy = function (view) {
      var result = _.mapObject(view, function (val, key) {
        if (key == "Events") {
          return null;
        } else if (_.isArray(val)) {
          if (val.length == 0) {
            return null;
          };

          return _.each(val, $this.getViewCopy);
        } else if (_.isObject(val)) {
          if (_.isEmpty(val)) {
            return null;
          };

          return $this.getViewCopy(val);
        };
        return val;
      });
      return result;
    };

    $this.getSearchEvent = function (block) {
      var searchControl = _.find(block.Controls, function (c) {
        return _.some(c.Events, function (e) {
          return _.some(e.Actions, isSearchActionType);
        });
      });

      var searchEvent = _.find(searchControl.Events, function (e) {
        return _.some(e.Actions, isSearchActionType);
      });

      return searchEvent;
    }

    $this.getSearchAction = function (searchEvent) {
      var searchAction = _.find(searchEvent.Actions, isSearchActionType);
      return searchAction;
    }

    $this.executeEvent = function (events, options, callback) {
      options = options || { saveEvent: true };

      var event = events[0];

      if (isDeleteEvent(event) && !confirm("Вы действительно хотите удалить документ?")) {
        return;
      }

      loadFromServer("ticket", "ExecuteEvent", "POST", event, function (results) {
        var actionResults = results[0].ActionResults;
        _.each(actionResults, function (action) {
          if (isGotoActionType(action)) {
            // Переходим на страницу
            if (action.Data) {
              goTo(action.Data);
            }
          } else if (isSaveActionType(action)) {
            if (isCreateActionType(action) && action.Data) {
              goTo(action.Data);
            } else {
              //console.log(results);
            }
          } else if (action.ActionType == "GoBack") {
            goBack();
          } else if (isCallbackAction(action.ActionType)) {
            callback(action);
          } else {
            throw ("Нет обработчика для Action=" + action.ActionType);
          }
        });

        // Сохранить событие
        if (options.saveEvent && isGotoEvent(event) && !isSaveEvent(event) && !isDeleteEvent(event)) {
          saveEvent(event);
        }
      });
    };

    $this.executeControl = function (element) {
      var events = JSON.parse(JSON.stringify(element.control.Events));

      _.each(events, function (e) {
        var actionSubmit = _.find(e.Actions, function (a) {
          return a.ActionType == "UpdateDocument";
        });

        var actionSearch = _.find(e.Actions, function (a) {
          return a.ActionType == "Search";
        });

        if (actionSearch || actionSubmit) {

          var emitName = actionSearch ? "search" : "submit";
          element.$emit(emitName, events, actionSearch ? actionSearch : actionSubmit);

        } else {

          $this.executeEvent(events);

        }
      });
    };

    $this.findView = function (data) {
      // Заходим в каждый элемент и если это блок, то повторяем
      if (data.Props && data.Props.Id)
        _.each(data, function (view) {
          return $this.findView(view);
        });
    }

    $this.bindControl = function (vm, data) {
      var control = (vm.block && vm.block.Controls) ? _.find(vm.block.Controls, function (c) {
        return c.Props.Id == data.controlId;
      }) : null;
      if (control) {
        vm.$watch(function () {
          return control.Value;
        }, function (value) {
          data.callback(value);
        });
      } else {
        vm.$emit('bindControl', data);
      }
    }

    $this.selectComponent = function (control) {

      switch (control.FilterType) {
        case "Text": return "filter-input";
        case "Select": return "control-select";
        case "Switch": return "control-switch";
        case "Switch3": return "filter-switch3";
        case "MultiSelect": return "control-multiselect";
        case "DatePicker": return "control-date";
        case "DateRange": return "control-daterange";
      }

      switch (control.ControlType) {
        case "ButtonSearch": return "control-button";
        case "Button": return "control-button";
        case "TextInput": return "control-input";
        case "Select": return "control-select";
        case "MultiSelect": return "control-multiselect";
        case "AutoComplete": return "control-autocomplete";
        case "TextArea": return "control-textarea";
        case "DatePicker": return "control-date";
        case "Label": return "control-label";
        case "DatePicker": return "control-date";
        case "Switch": return "control-switch";
        case "File": return "control-file";
      }
    };

    $this.editorPost = function (method, data, callback) {
      loadFromServer("editor", method, "POST", data, callback);
    }

    $this.editorGet = function (method, data, callback) {
      loadFromServer("editor", method, "GET", data, callback);
    }

    $this.postForm = function (method, data, callback, progress) {
      $.ajax({
        type: "POST",
        url: $this.baseUrl + 'ticket' + "/" + method,
        contentType: false,
        processData: false,
        data: data,
        cache: false,
        crossDomain: true,
        xhr: function () {
          var xhr = new window.XMLHttpRequest();
          xhr.upload.addEventListener("progress", function (evt) {
            if (evt.lengthComputable) {
              if (progress) {
                progress(Math.round(evt.loaded / evt.total * 100));
              }
            }
          }, false);

          return xhr;
        },
        xhrFields: {
          withCredentials: true
        },
        success: function (result) {
          callback(result.Objects);
        },
        error: function (xhr, status, p3) {
          alert(status);
        }
      });
    }

    $this.openURL = function (method) {
      window.open($this.baseUrl + method, '_blank', '');
    }

    this.getWidth = function (block) {
      var width = block.Props.Width || block.Props.GridWidth;
      if (width == 0) {
        return 12;
      }
      return width;
    }

    this.getStyle = function (control) {
      var style = {};
      if (control.Props.Style) {
        if (control.Props.Style.BackgroundColor)
          style['background-color'] = control.Props.Style.BackgroundColor;
        if (control.Props.Style.TextColor)
          style['color'] = control.Props.Style.TextColor;
        if (control.Props.Style.TextWeight)
          style['font-weight'] = control.Props.Style.TextWeight;
      }
      return style;
    }

    loadSidebar(function (sidebarJson) {
      initApp(sidebarJson);

      goToLast();
    });

    if (window.history && window.history.pushState) {

      history.pushState("nohb", null, "");
      $(window).on("popstate", function (event) {
        if (!event.originalEvent.state) {
          history.pushState("nohb", null, "");
          return;
        } else {
          goBack();
        }
      });
    }
  };

  window.app = new App();

});
