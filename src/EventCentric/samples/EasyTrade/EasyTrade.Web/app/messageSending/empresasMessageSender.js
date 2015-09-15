﻿(function () {
    'use strict';

    angular
        .module('app')
        .factory('empresasMessageSender', empresasMessageSender);

    empresasMessageSender.$inject = ['$http'];

    function empresasMessageSender($http) {

        var empresasQueueUrl = "http://localhost:50588";

        var service = {
            nuevaEmpresa: nuevaEmpresa,
            desactivarEmpresa: desactivarEmpresa,
            reactivarEmpresa: reactivarEmpresa
        };

        return service;

        function nuevaEmpresa(empresa) {
            return $http.post(empresasQueueUrl + '/empresas/nueva-empresa', empresa);
        }

        function desactivarEmpresa(idEmpresa) {
            return $http.post(empresasQueueUrl + '/empresas/desactivar-empresa/' + idEmpresa);
        }

        function reactivarEmpresa(idEmpresa) {
            return $http.post(empresasQueueUrl + '/empresas/reactivar-empresa/' + idEmpresa);
        }
    }
})();