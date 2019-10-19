/// <reference path="../jquery.jqGrid.src.js" />
$(function () {
    $("#jqGrid").jqGrid({
        url: "/TestManagement/GetQuestions",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['ID', 'Topic Id', 'Question', 'Answer type Id', 'Answer type', 'Resource reference', 'Weight'],
        colModel: [
            { key: true, hidden: true, name: 'ID', index: 'ID', editable: true },
            { key: false, hidden: true, name: 'TopicId', index: 'TopicId', editable: true },
            { key: false, name: 'Question', index: 'Question', editable: true, sortable: false, edittype: 'textarea', editoptions: { rows: "5", cols: "18" } },
            { key: false, hidden: true, editrules: { edithidden: true, required: true }, name: 'AnswerTypeId', index: 'AnswerTypeId', editable: true, sortable: false, edittype: 'select', editoptions: { value: { '1': 'CheckBox', '2': 'RadioButton', '3': 'Text' } } },
            { key: false, name: 'AnswerType', index: 'AnswerType', editable: false, sortable: false },
            { key: false, hidden: true, name: 'ResourceRef', index: 'ResourceRef', editable: true, sortable: false, search: false },
            { key: false, name: 'QuestionWeight', index: 'QuestionWeight', editable: true, align: "center", sortable: false }
        ],
        pager: jQuery('#jqControls'),
        rowNum: 10,
        rowList: [5, 10, 20, 30, 40],
        height: '100%',
        viewrecords: true,
        caption: 'Questions Records',
        emptyrecords: 'No Questions Records are Available to Display',
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
    }).navGrid('#jqControls', { edit: true, add: true, del: true, search: true, searchtext: "Search question", refresh: true },
        {
            zIndex: 100,
            url: '/TestManagement/EditQuestionInJqGrid',
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
            url: "/TestManagement/CreateQuestionInJqGrid",
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
            url: "/TestManagement/DeleteQuestionInJqGrid",
            closeOnEscape: true,
            closeAfterDelete: true,
            recreateForm: true,
            msg: "Are you sure you want to delete Question... ? ",
            afterComplete: function (response) {
                if (response.responseText) {
                    alert(response.responseText);
                }
            }
        },
        {
            zIndex: 100,
            caption: "Search question",
            sopt: ['eq', 'cn']
        });
});