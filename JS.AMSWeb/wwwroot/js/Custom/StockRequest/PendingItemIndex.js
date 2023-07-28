﻿"use strict";
// Class definition
const tableName = "stock_pending_request_item_datatable";
const searchCodeField = "SearchCode";
const searchNameField = "SearchName";

var KTDatatableRemoteAjaxDemo = function () {
    var demo = function () {
        var datatable = $('#' + tableName).KTDatatable({
            // datasource definition
            data: {
                type: 'remote',
                source: {
                    read: {
                        method: "POST",
                        url: "/Inventory/PendingStockRequest/GetAllRequestItems",
                        dataType: 'json',
                        params: {
                            "requestId": stockRequestId,
                            "productCode": "",
                            "productName": ""
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
                    field: 'productCode',
                    title: 'Product Code',
                    sortable: true,
                },
                {
                    field: 'productName',
                    title: 'Product Name',
                    sortable: true,
                },
                {
                    field: 'requestQuantity',
                    title: 'Request',
                    sortable: false,
                    width: 100
                },
                {
                    field: 'approvedQuantity',
                    title: 'Approved',
                    sortable: false,
                    template: function (row) {
                        const value = row.approvedQuantity || '-';
                        return '<div class="col-6"'
                            + 'id="qParent' + row.stockRequestItemId + '" >'
                            + '<span class="value-label"'
                            + 'id="q' + row.stockRequestItemId + '" >'
                            + value
                            + ' </span>'
                            + '</div>'
                            ;
                    },
                    width: 100
                },
                {
                    field: 'receivedQuantity',
                    title: 'Received',
                    sortable: false,
                    template: function (row) {
                        const value = row.receivedQuantity || '-';
                        return value
                            ;
                    },
                    width: 100
                }
                //    {
                //        field: '',
                //        title: '',
                //        sortable: false,
                //        width: 75,
                //        overflow: 'visible',
                //        template: function (row) {
                //            if (!!isEnableEditJs) {
                //                return "<div class='dropdown dropdown-inline'/>"
                //                    + "<button type='button' id='btnEdit" + row.stockRequestItemId + "' class='btn btn-sm btn-clean btn-icon edit-row' onclick='EditItem(\"" + row.stockRequestItemId + "\",\"" + row.requestQuantity + "\")' title='Edit'>"
                //                    + "<i  id='btnEditIcon" + row.stockRequestItemId + "' class='far fa-edit'></i>"
                //                    + "</button>"
                //                    ;
                //            }
                //            else {
                //                return "";
                //            }
                //        },
                //    }
            ]
        });

        document.getElementById(searchCodeField).value = datatable.getDataSourceParam("productCode");
        document.getElementById(searchNameField).value = datatable.getDataSourceParam("productName");
    };


    return {
        init: function () {
            demo();
        },
    };
}();


function ResetTable() {
    //show loading
    $('#loading').show();
    $('#' + tableName).hide();

    //set text field value to null
    document.getElementById(searchCodeField).value = "";
    document.getElementById(searchNameField).value = "";
    localStorage.removeItem(tableName + '-1-meta');

    //redraw table
    var datatable = $('#' + tableName).KTDatatable();
    datatable.setDataSourceParam("productCode", "");
    datatable.setDataSourceParam("productName", "");
    datatable.reload();
}
function Search() {

    $('#loading').show();
    $('#' + tableName).hide();
    var datatable = $('#' + tableName).KTDatatable();
    datatable.setDataSourceParam("productCode", document.getElementById(searchCodeField).value.toLowerCase());
    datatable.setDataSourceParam("productName", document.getElementById(searchNameField).value.toLowerCase());
    datatable.reload();
}

jQuery(document).ready(function () {
    KTDatatableRemoteAjaxDemo.init();
});