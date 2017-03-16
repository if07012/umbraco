angular.module("umbraco")
    .directive('datetime', ['$http', 'assetsService', 'angularHelper', function ($http, assetsService, angularHelper) {

        return {
            scope: {
                value: '=?',
                format: '=?'
            },
            restrict: 'A', require: '?ngController',
            link: function (scope, element, ngController) {
                scope.model = scope.$parent.model;

                assetsService.loadCss('lib/datetimepicker/bootstrap-datetimepicker.min.css').then(function () {
                    var config = {
                        pickDate: true,
                        pickTime: true,
                        useSeconds: true,
                        dateFormat: scope.format === undefined ? 'DD-MM-YYYY' : scope.format,
                        icons: {
                            time: "icon-time",
                            date: "icon-calendar",
                            up: "icon-chevron-up",
                            down: "icon-chevron-down"
                        }

                    };
                    function applyDate(e) {
                        angularHelper.safeApply(scope, function () {
                            // when a date is changed, update the model
                            if (e.date && e.date.isValid()) {
                                scope.value = e.date.format(scope.model.config.format);
                            }
                            else {
                                scope.hasDatetimePickerValue = false;
                                scope.value = null;
                            }

                            // setModelValue();
                            element.datetimepicker("hide", 0);
                        });
                    }
                    //map the user config
                    scope.model.config = angular.extend(config, scope.model.config);
                    var filesToLoad = ["lib/moment/moment-with-locales.js",
                                       "lib/datetimepicker/bootstrap-datetimepicker.js"];
                    assetsService.load(filesToLoad, scope).then(
                        function () {
                            //The Datepicker js and css files are available and all components are ready to use.
                            // Get the id of the datepicker button that was clicked
                            element
                   .datetimepicker(angular.extend({ useCurrent: true }, scope.model.config))
                   .on("dp.change", applyDate)
                   .on("dp.error", function (a, b, c) {
                       scope.hasDatetimePickerValue = false;
                       scope.datePickerForm.datepicker.$setValidity("pickerError", false);
                   });
                            element.find("input").bind("blur", function () {
                                scope.$apply();
                            });

                            //Ensure to remove the event handler when this instance is destroyted
                            scope.$on('$destroy', function () {
                                element.find("input").unbind("blur");
                                element.datetimepicker("destroy");
                            });


                            var unsubscribe = scope.$on("formSubmitting", function (ev, args) {
                                setModelValue();
                            });
                            //unbind doc click event!
                            scope.$on('$destroy', function () {
                                unsubscribe();
                            });
                            scope.$watch('value', function (e) {
                                if (e === undefined) return;
                                if (e.indexOf === undefined) return;
                                if (e.indexOf('/Date') < 0) return;
                                var date = new Date(parseInt(e.substr(6)));
                                var result = moment(date).format(scope.model.config.dateFormat);
                                element.find('input').val(result)
                                var data = element.datetimepicker('setValue', date);
                            });

                        });
                });
            }
        }

    }])
    .directive('jtable', ['$http', 'assetsService', function ($http, assetsService) {

        return {
            restrict: 'A',
            templateUrl: 'lib/dynamiccrud/crud-template.html',
            link: function (scope, element) {
                var url = "/VoxTeneo/Generics/";
                element.parent().parent().parent().parent().parent().find('label').css({ display: 'none' });
                element.parent().parent().parent().parent().parent().find('.controls').css({ 'margin-left': '0px' });
                element.parent().parent().parent().parent().parent().find('.ui-resizable-e').css({ display: 'none' });

                var table = "Customers";
                scope.showForm = false;
                $http.get('/VoxTeneo/JTableHelpers/GetSchema/?name=' + table).then(function (e) {
                    var jtFirstLoad = true;
                    var ajaxUrlList = url + "Get";
                    var fields = {};
                    for (var i in e.data.Schema) {
                        var temp = {};
                        for (var j in e.data.Schema[i]) {
                            if (typeof (e.data.Schema[i][j]) === 'string') {
                                if (e.data.Schema[i][j].indexOf('function') >= 0)
                                    temp[j] = eval.call(null, e.data.Schema[i][j]);
                                else {

                                    temp[j] = e.data.Schema[i][j];
                                }
                            } else {
                                temp[j] = e.data.Schema[i][j]
                            }
                        }
                        if ('CustomAction' === i) {
                            var customAction = {};
                            customAction.sorting = false;
                            customAction.display = function (data) {
                                if (data.record) {
                                    var result = "";
                                    for (var k in temp) {
                                        result += '<span class="btn btn-default  ' + temp[k].Css.join() + '" data-toggle="tooltip" title="' + temp[k].Title + '"> ' + (temp[k].Html ? temp[k].Html : ' <i class="fa ' + (temp[k].Icon ? temp[k].Icon : 'fa-list-alt') + '" aria-hidden="true"></i>') + '</span>';
                                    }
                                    setTimeout(function () {
                                        $(document).ready(function () {
                                            $('[data-toggle="tooltip"]').tooltip();
                                            $('.btn-default').each(function () {
                                                $(this).on('click', function () {
                                                    debugger;
                                                });
                                            });
                                        });
                                    });
                                    return result + "";
                                }
                            };
                            fields[i] = customAction;
                        } else
                            fields[i] = temp;
                    }
                    var forms = [];
                    function Reset() {
                        forms = [];
                        for (var i in e.data.Forms) {
                            var temp = {};
                            for (var j in e.data.Forms[i]) {
                                if (e.data.Forms[i][j].indexOf('function') >= 0)
                                    temp[j] = eval.call(null, e.data.Schema[i][j]);
                                else
                                    temp[j] = e.data.Forms[i][j];
                            }
                            forms.push(temp);
                        }
                    }
                    Reset();
                    scope.forms = forms;
                    scope.back = function () {
                        scope.showForm = false;
                        Reset();
                    }
                    scope.saveData = function () {
                        var model = {};
                        for (var i in scope.forms) {
                            model[scope.forms[i].id] = scope.forms[i].value;
                        }
                        $http.post(url + "?model=" + table + "&state=" + scope.state, model).then(function (e) {
                        });
                    };
                    //Table Grid
                    $(element.find('#grid')).jtable({
                        paging: true, //Enable paging
                        title: 'Manage ' + table,
                        formCreated: function (a, obj, c, d, e) {
                            if (obj.formType === "edit") {
                                setTimeout(function () {
                                    $('.ui-resizable-e').css({ display: 'none' })
                                }, 500);
                                scope.state = "Update";
                                scope.showForm = true;
                                var temp = scope.forms;
                                for (var i in temp) {
                                    temp[i].value = obj.record[temp[i].id];
                                }
                                scope.forms = temp;
                                scope.$apply();
                            }
                        },
                        toolbar: {
                            hoverAnimation: true, //Enable/disable small animation on mouse hover to a toolbar item.
                            hoverAnimationDuration: 60, //Duration of the hover animation.
                            hoverAnimationEasing: undefined, //Easing of the hover animation. Uses jQuery's default animation ('swing') if set to undefined.
                            items: [{
                                text: 'Create New',
                                click: function () {
                                    scope.showForm = true;
                                    scope.state = "";
                                }
                            }] //Array of your custom toolbar items.
                        },
                        create: true,
                        pageSizes: [10, 20, 25, 50, 100, 150],
                        pageSize: 20,
                        deleteConfirmation: function (e) {
                            setTimeout(function () {
                                $('.ui-resizable-e').css({ display: 'none' })
                            }, 500);
                            scope.state = "Delete";
                            scope.showForm = true;
                            var temp = scope.forms;
                            for (var i in temp) {
                                temp[i].value = e.record[temp[i].id];
                            }
                            scope.forms = temp;
                            scope.$apply();
                        },
                        sorting: true, //Enable sorting
                        defaultSorting: '', //Set default sorting                        
                        columnSelectable: true,
                        actions: {
                            listAction: function (postData, jtParams) {
                                jtParams.Schema = table;

                                return $.Deferred(function ($dfd) {
                                    $.ajax({
                                        url: ajaxUrlList,
                                        type: 'POST',
                                        dataType: 'json',
                                        data: {
                                            PageCurrent: jtParams.jtStartIndex,
                                            PageSize: jtParams.jtPageSize,
                                            Schema: table,
                                            Sorting: jtParams.jtSorting != undefined ? jtParams.jtSorting.split(' ')[0] : jtParams.jtSorting,
                                            SortingType: jtParams.jtSorting != undefined ? jtParams.jtSorting.split(' ')[1] : jtParams.jtSorting
                                        },
                                        beforeSend: function (xhr, settings) {
                                            settings.data += "&jtFirstLoad=" + jtFirstLoad;
                                        },
                                        success: function (data) {
                                            var hasRecord = (data != null && data.hasOwnProperty('TotalRecordCount') && data.TotalRecordCount > 0);

                                            $('#jtStartIndex').val(jtParams.jtStartIndex);
                                            $('#jtPageSize').val(jtParams.jtPageSize);
                                            $('#jtSorting').val(jtParams.jtSorting);
                                            data.Result = "OK";
                                            $dfd.resolve(data);
                                        },
                                        error: function () {
                                            $dfd.reject();
                                        }
                                    });
                                });
                            },
                        },
                        fields: fields,
                        recordsLoaded: function (event, data) {

                        }
                    });
                    $(element.find('#grid')).jtable('load');

                    //Form Grid
                });


            }
        }
    }])
        .controller("Umbraco.DynamicCrud",
    function () {

    });