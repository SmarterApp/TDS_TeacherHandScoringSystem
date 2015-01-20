var jqueryNoConflict = jQuery;

//begin main function
jQuery(document).ready(function () {
    retrieveData();
});
//end main function

// grab data
function retrieveData() {
    var dataSource = 'RepositoryItems.json.js';  //json extension is not handled yet.
    //Filter out 'Scored' statuses from the Sample

    jQuery.getJSON(dataSource, renderDataVisualsTemplate);
};

// render compiled handlebars template
function renderDataVisualsTemplate(data) {
    //handlebarsDebugHelper();
    //renderHandlebarsTemplate('dataDetailsTemplate.handlebars.html', '#data-details', data);
    renderHandlebarsTemplate('~/Views/HandleBars Templates/ItemRepositoryDataTemplate.cshtml', '#item-repo-data', data);

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
function renderHandlebarsTemplate(withTemplate, inElement, withData) {
    getTemplateAjax(withTemplate, function (template) {
        jQuery(inElement).html(template(withData));
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
    $(document).on('order.dt', '#repositoryitems', function () {

        //Since the Checkbox at the top also causes and order, then this fails every time and always clears.
        //$("input[type=checkbox]").prop('checked', false);

        //alert("Order Event: Ids are prepared");
        prepareIds();

    });

}
