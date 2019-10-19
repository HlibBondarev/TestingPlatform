/// <reference path="../jquery.jqGrid.src.js" />
$(function () {
    $("#jqGrid").jqGrid({
        url: "/TestManagement/GetTopics",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['ID', 'Course Id', 'Name', 'Description', 'Number', 'Is free'],
        colModel: [
            { key: true, hidden: true, name: 'ID', index: 'ID', editable: true },
            { key: false, hidden: true, name: 'CourseId', index: 'CourseId', editable: true },
            { key: false, name: 'Name', index: 'Name', editable: true, sortable: false },
            { key: false, name: 'Description', index: 'Description', editable: true, sortable: false, search: false},
            { key: false, name: 'TopicNumber', index: 'TopicNumber', editable: true, align: "center", sortable: false },
            { key: false, name: 'IsFree', index: 'IsFree', editable: true, align: "center", sortable: false, edittype: 'checkbox', editoptions: { value: "true:false" } }
        ],
        pager: jQuery('#jqControls'),
        rowNum: 10,
        rowList: [5, 10, 20, 30, 40],
        height: '100%',
        viewrecords: true,
        caption: 'Topics Records',
        emptyrecords: 'No Topics Records are Available to Display',
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
    }).navGrid('#jqControls', { edit: true, add: true, del: true, search: true, searchtext: "Search topic", refresh: true },
        {
            zIndex: 100,
            url: '/TestManagement/EditTopicInJqGrid',
            closeOnEscape: true,
            closeAfterEdit: true,
            recreateForm: true,
            afterComplete: function (response) {
                if (response.responseText) {
                    alert(response.responseText);
                }
            }
        },
        {
            zIndex: 100,
            url: "/TestManagement/CreateTopicInJqGrid",
            closeOnEscape: true,
            closeAfterAdd: true,
            afterComplete: function (response) {
                if (response.responseText) {
                    alert(response.responseText);
                }
            }
        },
        {
            zIndex: 100,
            url: "/TestManagement/DeleteTopicInJqGrid",
            closeOnEscape: true,
            closeAfterDelete: true,
            recreateForm: true,
            msg: "Are you sure you want to delete Topic... ? ",
            afterComplete: function (response) {
                if (response.responseText) {
                    alert(response.responseText);
                }
            }
        },
        {
            zIndex: 100,
            caption: "Search topic",
            sopt: ['eq', 'cn']
        });  
});  