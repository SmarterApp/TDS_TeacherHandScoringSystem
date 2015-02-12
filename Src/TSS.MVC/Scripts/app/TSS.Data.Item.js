



//end main function
var TSS = (typeof (TSS)) == 'undefined' ? {} : TSS;
TSS.Common = (typeof (TSS.Common)) == 'undefined' ? {} : TSS.Common;

TSS.Common.TSSQUERY = "TSS_QUERY";
TSS.Common.ASSIGNMENTIDS = "TSS.Detail.AssignmentIds";
TSS.Common.CURRENTID = "TSS_CURRENTID";

TSS.Common.storage = $.localStorage;
TSS.Common.getQuery = function () {
    var storage = TSS.Common.storage;
    if (!storage.get(TSS.Common.TSSQUERY)) {
        storage.set(TSS.Common.TSSQUERY, new TSS.Common.tssQuery());
    }
    return storage.get(TSS.Common.TSSQUERY);
};

TSS.Common.saveAssignmentIds = function(ids) {
    TSS.Common.storage.set(TSS.Common.ASSIGNMENTIDS, ids);
};

TSS.Common.getAssignmentIds = function () {
    return TSS.Common.storage.get(TSS.Common.ASSIGNMENTIDS);
};

TSS.Common.clearStorage = function () {
    TSS.Common.storage.removeAll();
};

TSS.Common.saveQueryValue = function (filterName, value, callback) {
    var storage = TSS.Common.storage;
    var q = getQuery();
    for (var i = 0; i < q.columnFilterValues.length; i++) {
        if (q.columnFilterValues[i].filterName === filterName) {
            q.columnFilterValues[i].value = (value == "-1" ? "" : value);
            break;
        }
    }
    storage.set(TSS.Common.TSSQUERY, q);
    callback();
};

TSS.Common.SaveQuery = function (query) {
    TSS.Common.storage.set(TSS.Common.TSSQUERY, query);
};

TSS.Common.getCurrentAssignmentId = function() {
    return TSS.Common.storage.get(TSS.Common.CURRENTID);
};

TSS.Common.setCurrentAssignmentId = function (id) {
    return TSS.Common.storage.set(TSS.Common.CURRENTID,id);
};

TSS.Common.getAppliedFilters = function () {

    var filters = TSS.Common.getQuery().columnFilterValues;
    var returnObj = new Object();
    for (var i = 0; i < filters.length; i++) {
        returnObj[filters[i].filterName] = filters[i].value;
    }

    return returnObj;
};

TSS.Common.tssQuery = function () {
    var that = this;
    this.columnFilterValues = [];
    var filterElements = $(".chosen-select");
    filterElements.each(function () {
        var filterName = $(this).data('filtername');
        var id = this.id;
        that.columnFilterValues.push({ id: id, filterName: filterName, value: "" });
    });
    this.sortColumn = "";
    this.sortDirection = "";
};

// grab data
function retrieveData(callback) {
    var dataSource = '/api/item/list';  //json extension is not handled yet.
    //Filter out 'Scored' statuses from the Sample

    $.ajaxSetup({ cache: false });
    var postData = { "pageNumber": 1, "pageLength": 100, "filters": { "test-Name": "", "a-SessionId": "" }, "sortColumn": "Status", "sortDirection": "DESC" };
    var data = jQuery.post(dataSource,postData, function (data) {
        $.ajaxSetup({ cache: true });
        renderDataVisualsTemplate(data, callback);
    },'json');
};

// render compiled handlebars template
function renderDataVisualsTemplate(data, callback) {
    //handlebarsDebugHelper();
    //renderHandlebarsTemplate('dataDetailsTemplate.handlebars.html', '#data-details', data);
    renderHandlebarsTemplate('/Sample/StudentItems.dataTableTemplate.handlebars.html', '#data-details', data, callback);

    wireDataTableEvents();

    //var table = $('#studentList').DataTable();
    //table.order([[0, 'asc']])
    //    .draw( false );


};

// render handlebars templates via ajax
function getTemplateAjax(path, callback) {
    var source, template;
    jQuery.ajax({
        url: path,
        success: function (data) {
            source = data;
            template = Handlebars.compile(source);
            if (callback) callback(template);
        }
    });
};

// function to compile handlebars template
function renderHandlebarsTemplate(withTemplate, inElement, withData, render) {
    getTemplateAjax(withTemplate, function (template) {
        jQuery(inElement).html(template(withData));
        if (render)
            render();
    })
};

// add handlebars debugger
function handlebarsDebugHelper() {
    Handlebars.registerHelper("debug", function (optionalValue) {
        console.log("Current Context");
        console.log("====================");
        console.log(this);
    });
};

function wireDataTableEvents() {

    //Wire up events for the DataTable.
    $(document).on("click", ".mainCheckBox", function () {
        $("input[type=checkbox]").prop('checked', this.checked);
    });

    //TODO: figure out how to trap the Ordering Event.
    //Maybe trap the AfterRefresh Event?
    $(document).on('order.dt', '#studentList', function () {

        //Since the Checkbox at the top also causes and order, then this fails every time and always clears.
        //$("input[type=checkbox]").prop('checked', false);

        //alert("Order Event: Ids are prepared");
        //prepareIds();

    });

}
