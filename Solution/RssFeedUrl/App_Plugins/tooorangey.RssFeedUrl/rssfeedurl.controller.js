angular.module("umbraco").controller("tooorangey.RssFeedUrlController", function ($scope, $http) {
    var vm = this;
    vm.status = {
        linkTested:false,
        linkValid: false,
        isChecking: false,
        buttonText: 'Check Feed Url'
    }

    vm.overlay = {
        show:false,
        feed: {},
        view: '/app_plugins/tooorangey.RssFeedUrl/preview.html',
        close: function (oldModel) {
            vm.overlay.show = false;
        },
        title: 'Feed Preview'
    }

    vm.checkFeedUrl = checkFeedUrl;
    vm.updateValue = updateValue;
    vm.previewFeed = previewFeed;

    function previewFeed() {
        console.log(vm.overlay);
        vm.overlay.show = true;
    }
    function updateValue() {
        resetCheck();
    }
    function resetCheck() {
        vm.status.linkTested = false;
        vm.status.linkValid = false;
        vm.status.isChecking = false;
        vm.status.statusMessage = '';
        vm.status.buttonText = 'Check Feed Url';
    }
    function checkFeedUrl() {
        if (!vm.status.linkTested) {
            resetCheck();
            vm.status.isChecking = true;
            $http({
                method: 'GET',
                url: '/umbraco/backoffice/api/feed/GetRssFeed?feedUrl=' + $scope.model.value
            }).then(function successCallback(response) {
                console.log(response.data);
                var feedDetails = response.data;
                vm.status.isChecking = false;
                vm.status.linkTested = true;
               
               
                if (response.data.HasFeedResults) {
                    vm.overlay.feed = feedDetails;
                    vm.overlay.title = feedDetails.SyndicationFeed.Title.Text;
                    vm.status.linkValid = response.data.HasFeedResults;
                    vm.status.linkValid = true;
                    vm.status.statusMessage = '';
                    vm.status.buttonText = 'Valid Feed Url';
                }
                else {
                    vm.status.linkValid = false;
                    vm.status.statusMessage = 'A valid Rss Feed was not found at this Url';
                    vm.status.buttonText = 'Not Valid';
                }

            }, function errorCallback(response) {
                // called asynchronously if an error occurs
                // or server returns response with an error status.
                vm.status.linkTested = true;
                vm.status.linkValid = false;
                vm.status.isChecking = false;
                vm.status.statusMessage = 'An error requesting and reading the feed url';
                console.log(response);
            });


        }


    }

 

});