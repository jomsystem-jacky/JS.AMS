﻿"use strict";
// Class definition
const tableName = "payment_term_datatable";
const searchField = "SearchName";

var KTDatatableRemoteAjaxDemo = function () {
    var demo = function () {
        var datatable = $('#' + tableName).KTDatatable({
            // datasource definition
            data: {
                type: 'remote',
                source: {
                    read: {
                        method: "POST",
                        url: "/Client/Configuration/PaymentTerm/GetAllPaymentTerms",
                        dataType: 'json',
                        params: {
                            "name": "",
                        },

                        map: function (raw) {
                            $('#loading').hide();
                            $('#' + tableName).show();
                            // sample data mapping
                            var dataSet = raw;
                            if (typeof raw.data !== 'undefined') {
                                dataSet = raw.data;
                            }
                            return dataSet;
                        },
                    },
                },
                pageSize: 10,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                //    saveState: false
            },

            // layout definition
            layout: {
                scroll: false,
                footer: false,
                spinner: {
                    message: true,
                    message: "Loading..."
                }
            },

            // column sorting
            sortable: true,
            pagination: true,

            // columns definition
            columns: [
                {
                    field: 'name',
                    title: 'Name',
                    sortable: true,
                },
                {
                    field: 'termType',
                    title: 'Term Type',
                    sortable: false,
                    template: function (row) {
                        var type = {
                            1: {
                                'title': 'Cash On Delivery',
                                //'class': ' label-light-danger'
                            },
                            2: {
                                'title': 'Payment By Days',
                                //'class': ' label-light-success'
                            }
                        };
                        return '<span>' + type[row.termType].title + '</span>';
                    },
                },
                {
                    field: 'duration',
                    title: 'Duration',
                    sortable: false,
                    template: function (row) {
                        if (row.duration) {
                            return '<span>' + [row.duration] + '</span>';
                        }
                        else {
                            return '<span class="font-weight-bold">-</span>';
                        }
                    },

                },
                {
                    field: 'durationType',
                    title: 'Type',
                    sortable: false,
                    template: function (row) {
                        var type = {
                            1: {
                                'title': 'Day',
                                //'class': ' label-light-danger'
                            },
                            2: {
                                'title': 'Month',
                                //'class': ' label-light-success'
                            },
                            3: {
                                'title': 'Year',
                                //'class': ' label-light-success'
                            }
                        };
                        if (type[row.durationType]) {
                            return '<span>' + type[row.durationType].title + '</span>';
                        } else {
                            return '<span class="font-weight-bold">-</span>';
                        }
                    },

                },
                {
                    field: '',
                    title: '',
                    sortable: false,
                    width: 75,
                    overflow: 'visible',
                    template: function (row) {
                        return "<div class='dropdown dropdown-inline'/>"
                            + "<a href='/Client/Configuration/PaymentTerm/Edit?id=" + row.id + "' class='btn btn-sm btn-clean btn-icon mr-2' title='View details'>"
                            + "<i class='fas fa-search-plus'></i>"
                            + "</a>"
                            ;
                    },
                }
            ]
        });

        document.getElementById(searchField).value = datatable.getDataSourceParam("name");
    };


    return {
        init: function () {
            demo();
        },
    };
}();


function ResetTable() {
    document.getElementById(searchField).value = "";
    localStorage.removeItem(tableName + '-1-meta');
    window.location = window.location
}
function Search() {

    $('#loading').show();
    $('#' + tableName).hide();
    var datatable = $('#' + tableName).KTDatatable();
    datatable.setDataSourceParam("name", document.getElementById(searchField).value.toLowerCase());
    datatable.reload();
}

jQuery(document).ready(function () {
    KTDatatableRemoteAjaxDemo.init();
});