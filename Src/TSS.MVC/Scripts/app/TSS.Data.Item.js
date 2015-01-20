


//begin main function
/* jQuery(document).ready(function () {
	alert("Loading StudentItems.data-script.js...");
	retrieveData();
}); */
//end main function

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
