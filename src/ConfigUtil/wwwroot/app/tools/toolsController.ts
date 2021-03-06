﻿/// OSVR-Config
///
/// <copyright>
/// Copyright 2016 Sensics, Inc.
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///     http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// </copyright>
/// 
module app.tools {
    class ToolsController {
        trackerViewerPath = "";

        startTrackerViewer() {
            var paths = (typeof this.trackerViewerPath !== "undefined" && this.trackerViewerPath !== null) ?
                this.trackerViewerPath.split(" ") : [];
            this.$http.post("/api/starttrackerviewer", {}, {
                params: {
                    paths: paths
                }
            }).then(
                success => {
                    console.log("starttrackerviewer call succeeded.");
                },
                failure => {
                    console.log("starttrackerviewer call failed.");
                });
                
        }

        startServer() {
            this.$http.post("/api/startserver", {}).then(
                success => {
                    console.log("startserver call succeeded.");
                },
                failure => {
                    console.log("startserver call failed.");
                });
        }

        static $inject = ["$http"];
        constructor(private $http: ng.IHttpService) { }
    }

    angular.module("app.tools", ["ui.router"])
        .config(["$stateProvider", ($stateProvider: angular.ui.IStateProvider) => {
            $stateProvider.state("tools", {
                url: "/tools",
                templateUrl: "app/tools/tools.html",
                controller: "app.tools.ToolsController as vm"
            });
        }])
        .controller("app.tools.ToolsController", ToolsController);
}