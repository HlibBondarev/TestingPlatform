/// <reference path="../jquery.jqGrid.src.js" />
$(function () {
    $("#jqGrid").jqGrid({
        url: "/TestManagement/GetAnswers",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['ID', 'Question Id', 'Answer', 'Is proper'],
        colModel: [
            { key: true, hidden: true, name: 'ID', index: 'ID', editable: true },
            { key: false, hidden: true, name: 'QuestionId', index: 'QuestionId', editable: true },
            { key: false, name: 'Answer', index: 'Answer', editable: true, sortable: false, edittype: 'textarea', editoptions: { rows: "5", cols: "18" } },
            { key: false, name: 'IsProper', index: 'IsProper', editable: true, sortable: false, edittype: 'checkbox', editoptions: { value: "true:false" } }
        ],
        pager: jQuery('#jqControls'),
        rowNum: 10,
        rowList: [5, 10, 20],
        height: '100%',
        viewrecords: true,
        caption: 'Answers Records',
        emptyrecords: 'No Answers Records are Available to Display',
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
    }).navGrid('#jqControls', { edit: true, add: true, del: true, search: true, searchtext: "Search answer", refresh: true },
        {
            zIndex: 100,
            url: '/TestManagement/EditAnswerInJqGrid',
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
            url: "/TestManagement/CreateAnswerInJqGrid",
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
            url: "/TestManagement/DeleteAnswerInJqGrid",
            closeOnEscape: true,
            closeAfterDelete: true,
            recreateForm: true,
            msg: "Are you sure you want to delete Answer... ? ",
            afterComplete: function (response) {
                if (response.responseText) {
                    alert(response.responseText);
                }
            }
        },
        {
            zIndex: 100,
            caption: "Search answer",
            sopt: ['eq', 'cn']
        });
});