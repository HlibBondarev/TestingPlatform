/// <reference path="../jquery.jqGrid.src.js" />
$(function () {
    $("#jqGrid").jqGrid({
        url: "/Admin/GetModeratorSubcriptions",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['ID', 'User Profile Id', 'User email', 'Start date', 'Subscription period', 'Course count', 'Is trial', 'Is approved'],
        colModel: [
            { key: true, hidden: true, name: 'ID', index: 'ID', editable: true },
            { key: false, hidden: true, name: 'UserProfileId', index: 'UserProfileId', editable: true },
            { key: false, name: 'Email', index: 'Email', editable: false, sortable: false },
            { key: false, name: 'StartDate', index: 'StartDate', editable: false, align: "center", sortable: false },
            { key: false, name: 'SubscriptionPeriod', index: 'SubscriptionPeriod', editable: true, align: "center", sortable: false, search: false },
            { key: false, name: 'CourseCount', index: 'CourseCount', editable: true, align: "center", sortable: false, search: false },
            { key: false, name: 'IsTrial', index: 'IsTrial', editable: false, align: "center", sortable: false, edittype: 'checkbox', editoptions: { value: "true:false" } },
            { key: false, name: 'IsApproved', index: 'IsApproved', editable: true, align: "center", sortable: false, edittype: 'checkbox', editoptions: { value: "true:false" } }
        ],
        pager: jQuery('#jqControls'),
        rowNum: 10,
        rowList: [5, 10, 20, 30, 40],
        height: '100%',
        viewrecords: true,
        caption: 'Moderator subscription records',
        emptyrecords: 'No Moderator subscription records are Available to Display',
        jsonReader: {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            repeatitems: false,
            Id: "0"
        },
        autowidth: true,
        multiselect: false
    }).navGrid('#jqControls', { edit: true, add: false, del: true, search: true, searchtext: "Search subscription", refresh: true },
        {
            zIndex: 100,
            url: '/Admin/EditModeratorSubscription',
            closeOnEscape: true,
            closeAfterEdit: true,
            recreateForm: true,
            afterComplete: function (response) {
                if (response.responseText) {
                    alert(response.responseText);
                }
            }
        },
        null,// Creation method
        {
            zIndex: 100,
            url: "/Admin/DeleteModeratorSubscription",
            closeOnEscape: true,
            closeAfterDelete: true,
            recreateForm: true,
            msg: "Are you sure you want to delete this Subscription? ",
            afterComplete: function (response) {
                if (response.responseText) {
                    alert(response.responseText);
                }
            }
        },
        {
            zIndex: 100,
            caption: "Search subscription",
            sopt: ['eq', 'cn']
        });
});