angular.module("umbraco")
    .directive('select2', ['$http', function ($http) {
        return {
            restrict: 'A',
            scope: {
                item: '=?'
            },
            link: function (scope, element) {
                element.empty();
                $http.get('/VoxTeneo/Profiles/').then(function (data) {
                    for (var i in data.data) {
                        $(element)
       .append($("<option></option>")
                  .attr("value", data.data[i].Id)
                  .text(data.data[i].FullName));
                    }

                    element.find('option').each(function () {
                        var option = this;
                        var length = _.filter(scope.item,
                            function (n) {
                                return n.id === $(option).val();
                            });
                        if (length.length > 0)
                            $(option).attr('selected', 'selected');
                    });
                    scope.item = JSON.stringify(scope.item);
                    $(element).select2();
                    $(element).change(function () {
                        var temp = [];
                        for (var i in $(element).select2('data')) {
                            temp.push({
                                id: $(element).select2('data')[i].id,
                                text: $(element).select2('data')[i].text
                            });
                        }
                        scope.item = JSON.stringify(temp);
                        if (!scope.$$phase) {
                            scope.$apply();
                        }
                    });
                });


            }
        }
    }])
    .directive('contentmessage', ['$http', function ($http) {

        return {
            restrict: 'A',
            scope: {
                profileId: '=?', items: '=?'
            },
            transclude: true,
            template: ' <ul class="ca bqe bqf agk">' +

                ' <li class="tu b ahx"> <' +
                    'div class="input-group"> <input type="text" ng-model="value" class="form-control" placeholder="Message"> ' +
                        '<div class="om"> ' +
                            '<button type="button" ng-click="sendMessage()" class="cg pl"> ' +
                                '<span class="h bbv"></span> ' +
                            '</button> ' +
                        '</div>' +
                    '</div>' +
                '</li>' +
                 ' <li class="tu b ahx"  ng-repeat="data in items">' +
                    ' ' +
                        '<img class="bqa wp yy agc" src="{{data.Person.AvatarUrl}}">' +
                    '' +
                        '<div class="tv">' +
                            '<div class="bqj">' +
                                '<small class="aec axr">{{data.InforDate}}</small>' +
                                '<h6>{{data.Person.FullName}}</h6>' +
                                '<p>{{data.ContentMessage}}</p>' +
                            '</div>' +
                        '</div>' +
                    '' +
                ' </li>' +
                '<li class="tu b ahx">' +
                '<button ng-click="LoadMore()" ng-show="!IsLast" ng-disabled="IsLoading" type="button" style="width:100%" class="btn btn-primary btn-lg" id="load" data-loading-text=" Processing Order"><i ng-show="IsLoading" style="width: initial;" class="fa fa-spinner fa-spin "></i> Load More</button> </li>' +
                '</ul>',
            link: function (scope, element) {
                function calculateTime(item) {
                    item.CreateDate = new Date(parseInt(item.CreateDate.substr(6)));
                    var date = new Date() - item.CreateDate;
                    var year = parseInt(date / (365 * 31 * 24 * 60 * 60 * 1000));
                    date -= year * (365 * 31 * 24 * 60 * 60 * 1000);
                    var month = parseInt(date / (31 * 24 * 60 * 60 * 1000));
                    date -= month * (31 * 24 * 60 * 60 * 1000);
                    var day = parseInt(date / (24 * 60 * 60 * 1000));
                    date -= day * (24 * 60 * 60 * 1000);
                    var hour = parseInt(date / (60 * 60 * 1000));
                    date -= hour * (60 * 60 * 1000);
                    var minutes = parseInt(date / (60 * 1000));
                    date -= minutes * (60 * 1000);
                    var second = parseInt(date / (1000));
                    if (day > 0)
                        item.InforDate = day + " days ago ";
                    else
                        if (hour > 0)
                            item.InforDate = hour + " hours ago ";
                        else
                            if (minutes > 0)
                                item.InforDate = minutes + " minutes ago ";
                            else
                                item.InforDate = second + " seconds ago ";
                    return item;
                }
                element.css({ width: '100%' });
                scope.sendMessage = function () {
                    var message = {
                        id: 0,
                        personId: scope.profileId,
                        ContentMessage: scope.value
                    }
                    $http.post('/VoxTeneo/Profiles/SaveMessage/?profileId=' + scope.profileId, message).then(function (data) {
                        scope.items.unshift(calculateTime(data.data));

                    });

                }
                scope.IsLoading = true;
                $http.post('/VoxTeneo/Profiles/GetMessage/?profileId=' + scope.profileId,
                {
                    PageSize: 5,
                    PageCurrent: 1
                }).then(function (data) {
                    debugger;
                    scope.items = _.map(data.data.Records, function (item) {
                        
                        item = calculateTime(item);
                        if (item.LastUpdated != null)
                            item.LastUpdated = new Date(parseInt(item.LastUpdated.substr(6)));
                        return item;
                    });
                    scope.IsLast = data.data.IsLast;
                    scope.PageCurrent = data.data.PageCurrent;
                    scope.IsLoading = false;
                });
                scope.LoadMore = function () {
                    scope.IsLoading = true;
                    $http.post('/VoxTeneo/Profiles/GetMessage/?profileId=' + scope.profileId,
                {
                    PageSize: 5,
                    PageCurrent: scope.PageCurrent + 1
                }).then(function (data) {
                    var items = _.map(data.data.Records, function (item) {
                        item = calculateTime(item);
                        if (item.LastUpdated != null)
                            item.LastUpdated = new Date(parseInt(item.LastUpdated.substr(6)));
                        return item;
                    });
                    for (var i in items) {
                        scope.items.push(items[i]);
                    }
                    
                    scope.IsLast = data.data.IsLast;
                    scope.PageCurrent = data.data.PageCurrent;
                    scope.IsLoading = false;
                });
                }
            }
        }
    }])
    .controller("Umbraco.ChooseFriend",
    function () {

    });