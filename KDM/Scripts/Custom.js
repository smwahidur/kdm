$("[confirm]").click(function (e) {
    //e.preventDefault();
    return confirm("Are you sure?");
});

//$("[print]").click(function (e) {
//    return window.print();
//});

$("[refresh]").click(function (e) {
    e.preventDefault();
    window.location.reload();
});



var loadingHtml = "<div class='loading-panel' style='font-size:2em; font-weight:bold;'>LOADING...</div>";
function showLoading(containerName) {
    var container = $(containerName).get(0);
    $(".loading-panel", container).remove();
    $(container).prepend(loadingHtml);
}

function hideLoading(containerName) {
    var container = $(containerName).get(0);
    var loadingContainer = $(".loading-panel", container);
    
    $(loadingContainer).remove();
}


function PostFormData(form, data) {

    var url = $(form).attr('action');
    var method = $(form).attr('method');
    var data = ConvertToFormData(form);
    var response = "";
    var options = {
        url: url,
        type: method,
        data: data,
        contentType: false,
        processData: false,
        dataType:"json",
        async: false,
        success: function (result) {
            console.log("Success");
            if (result.success == false) {
                SetErrors(result.errors);
            }
            
            //console.log(result);
            //return result;
            response = result;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log("Error");
            response= textStatus;
        }
    };

    $.ajax(options);

    return response;
}

function getWithQueryString(form, url = "") {
    
    var formElement = $("form[name=" + form + "]").get(0);
    console.log(formElement);

    if (url == "")
        url = $(formElement).attr('action');

    var method = $(formElement).attr('method');
    //var data = new FormData(formElement);
    var data = $(formElement).serialize();
    var response = "";

    console.log(url, method, data);

    var options = {
        url: url,
        type: method,
        data: data,
        contentType: false,
        processData: false,
        enctype: 'multipart/form-data',
        async: false,
        success: function (result) {
            if (result.success == false) {
                SetErrors(result.errors);
            }
            response = result;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            response = textStatus;
        }
    };

    $.ajax(options);

    return response;
}

function getUrlWithParameter(form, url = "") {

    var formElement = $("form[name=" + form + "]").get(0);
    console.log(formElement);

    if (url == "")
        url = $(formElement).attr('action');


    var data = $(formElement).serialize();

    console.log(url + "?" + data);
    var newUrl = url + "?" + data;
    return newUrl;
}

function ResponseProcessor(responseId, response) {

    switch (responseId) {
        case "CreateEmployeeSuccess":

            break;
        case "CreateEmployeeError":
            break;
    }
}

function SetErrors(errors) {

    $.each(errors, function (i,v) {
        console.log(v.Key, v.Message);
        $("[data-valmsg-for=" + v.Key + "]").text(v.Message);
    });
}


function PostJSONData(form) {

    var url = $(form).attr('action');
    var method = $(form).attr('method');
    var contentType = 'application/json; charset=utf-8';
    var data = ConvertToJSONData(form);

    console.log(url, method, data);

    var options = {
        url: url,
        type: method,
        data: data,
        contentType: contentType,
        processData: false,
        dataType: "json",
        async: false,
        success: function (result) {
            console.log("Success");
            return JSON.stringify(result);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log("Error");
            return textStatus;
        }
    };

    $.ajax(options);

}

function ConvertToFormData(form) {
    var array = jQuery(form).serializeArray();
    var formData = new FormData();
    var json = {};

    jQuery.each(array, function () {
        formData.append(this.name, this.value);
    });

    //console.log(formData);

    return formData;
}

function ConvertToJSONData(form) {
    var array = jQuery(form).serializeArray();
    var json = {};

    jQuery.each(array, function () {
        json[this.name] = this.value;
    });

    return JSON.stringify(json);
}


function formToQueryString(form) {
    return (new URLSearchParams( new FormData($("[name=" + form + "]").get(0)) )).toString();
}

//usage getAllUrlParams
//getAllUrlParams().product; // 'shirt'
//getAllUrlParams().color; // 'blue'
//getAllUrlParams().newuser; // true
//getAllUrlParams().nonexistent; // undefined
//getAllUrlParams('http://test.com/?a=abc').a; // 'abc'

function getAllUrlParams(url) {

    // get query string from url (optional) or window
    var queryString = url ? url.split('?')[1] : window.location.search.slice(1);

    // we'll store the parameters here
    var obj = {};

    // if query string exists
    if (queryString) {

        // stuff after # is not part of query string, so get rid of it
        queryString = queryString.split('#')[0];

        // split our query string into its component parts
        var arr = queryString.split('&');

        for (var i = 0; i < arr.length; i++) {
            // separate the keys and the values
            var a = arr[i].split('=');

            // set parameter name and value (use 'true' if empty)
            var paramName = a[0];
            var paramValue = typeof (a[1]) === 'undefined' ? true : a[1];

            // (optional) keep case consistent
            paramName = paramName.toLowerCase();
            if (typeof paramValue === 'string') paramValue = paramValue.toLowerCase();

            // if the paramName ends with square brackets, e.g. colors[] or colors[2]
            if (paramName.match(/\[(\d+)?\]$/)) {

                // create key if it doesn't exist
                var key = paramName.replace(/\[(\d+)?\]/, '');
                if (!obj[key]) obj[key] = [];

                // if it's an indexed array e.g. colors[2]
                if (paramName.match(/\[\d+\]$/)) {
                    // get the index value and add the entry at the appropriate position
                    var index = /\[(\d+)\]/.exec(paramName)[1];
                    obj[key][index] = paramValue;
                } else {
                    // otherwise add the value to the end of the array
                    obj[key].push(paramValue);
                }
            } else {
                // we're dealing with a string
                if (!obj[paramName]) {
                    // if it doesn't exist, create property
                    obj[paramName] = paramValue;
                } else if (obj[paramName] && typeof obj[paramName] === 'string') {
                    // if property does exist and it's a string, convert it to an array
                    obj[paramName] = [obj[paramName]];
                    obj[paramName].push(paramValue);
                } else {
                    // otherwise add the property
                    obj[paramName].push(paramValue);
                }
            }
        }
    }

    return obj;
}

$(function () {
    //$("[datepicker]").attr("readonly", "false");
    $("[datepicker]").daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,
        minYear: 1901,
        maxYear: parseInt(moment().format('YYYY'), 10),
        locale: {
            format: 'DD-MM-YYYY'
        }
    }, function (start, end, label) {

    });

    $("[datepicker]").on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD-MM-YYYY'));
        $(this).val(picker.endDate.format('DD-MM-YYYY'));
    });

});

$(function () {
    $("[datetimepicker]").attr("readonly", "true");
    $("[datetimepicker]").daterangepicker({
        timePicker: true,
        singleDatePicker: true,
        showDropdowns: true,
        minYear: 1901,
        maxYear: parseInt(moment().format('YYYY'), 10),
        locale: {
            format: 'DD-MM-YYYY hh:mm A'
        }
    }, function (start, end, label) {

    });

    $("[datetimepicker]").on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD-MM-YYYY  hh:mm A'));
        $(this).val(picker.endDate.format('DD-MM-YYYY hh:mm A'));
    });

});
//----------------- EMPLOYEE ------------------

function EmployeeCreate(form) {
    
    var result = PostFormData(form)

    console.log(result);

    return false;
}

function AddPhone(form) {

    var result = PostFormData(form)

    console.log(result);

    return false;
}

function DeletePhoneEmployee(form) {

    var result = PostFormData(form)

    console.log(result);

    return false;
}
//----------------- EMPLOYEE END ------------------

//----------------- DATA LOADING ------------------

$(function () {
    $("[erp-load-data-action]").click(function () {
        var targetTable = $(this).attr("erp-attr-target-table");
        var targetUrl = $(this).attr("erp-attr-target-url");
        var referenceId = $(this).attr("erp-attr-reference-id");
        $("[name=ReferenceId]").val(referenceId);
        var deleteAction = true;
        var deleteUrl = "/Employee/DeletePhone";
        ErpLoadTableData(targetTable, targetUrl,deleteAction,deleteUrl,referenceId, "");
    });
})

function ErpLoadTableData(targetTable, targetUrl, deleteAction, deleteUrl, referenceId, actions) {
    var method = 'get';
    var contentType = 'application/json; charset=utf-8';

    var fieldNames = new Array();
    var c = 0;
    $("th", targetTable).each(function () {
        fieldNames[c] = $(this).attr("erp-table-field");
        c++;
    });

    console.log(fieldNames);

    var options = {
        url: targetUrl,
        type: method,
        contentType: contentType,
        processData: false,
        dataType: "json",
        async: false,
        success: function (result) {
            var data = JSON.parse(result.Message);
            var html = "";
            $.each(data, function (index, row) {
                html += "<tr>";
                $.each(row, function (i, cell) {
                    if ($.inArray(i, fieldNames) > -1) {
                        html += "<td>" + cell + "</td>";
                    }
                });

                html += "<td width='15px'>";
                if (deleteAction == true) {
                    html += "<form action='" + deleteUrl + "' method='post' onsubmit = 'return DeletePhoneEmployee(this)'>"
                    html += "<input type='hidden' name='PhoneId' value='" + row.Id + "'>";
                    html += "<button type='submit' confirm class='btn btn-sm btn-danger pull-right'>Delete</button>";
                    html += "</form>";
                }
                
                html += '</td>';

                html += "</tr>";
            });

            console.log(html);

            $("tbody", targetTable).html(html);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus);
        }
    };

    $.ajax(options);
}


function LoadNextDropDown(ele) {


    var target = "[name=" + $(ele).attr("target") + "]";

    var controller = $(target).attr("controller");
   
    var action = $(target).attr("action");
    var area = $(target).attr("area");
   
    var value = $(ele).children("option:selected").val();
    var url = "";
    if (area == '')
    {        
       var url = "/" + controller + "/" + action + "?id=" + value;
    }        
    else if (area != '') {
        var url = "/" + area + "/" + controller + "/" + action + "?id=" + value;
    }

    $.get(url, function (data) {
        console.log(data);
        var html = "<option value='-1'>--Select--</option>";
        $.each(data, function (i, d) {
            html += "<option value="+d.Value+">"+d.Text+"</option>";
        });

        $(target).html(html);
    });
}


//----------------- DATA LOADING ------------------


// ---------------- START MENU ----------------------------

$(function () {

    var url = window.location.pathname;

    $(".dynamic-menu a").each(function (i, d) {

        var href = $(d).attr("href");

        console.log(url, href);


        if (url.trim() == href.trim()) {
            
            $(d).addClass("mm-active");
            var parentId = $(d).attr("parent-id");
            $("[childs-for=" + parentId + "]").addClass("mm-show");
        }
        else {
            $(d).removeClass("mm-active")
        }

    });
})

//----------------- END MENU ------------------------------

//----------------- START UTILITY -------------------------

$(function () {
    $('[timepicker]').timepicker({
        timeFormat: 'h:mm p',
        interval: 10,
        minTime: '10',
        maxTime: '6:00pm',
        defaultTime: '11',
        startTime: '10:00',
        dynamic: false,
        dropdown: false,
        scrollbar: true
    });
})

function ShowPopUpWindow(windowName, url) {
    var width = screen.width - 100;
    var height = screen.height - 200;
    var top = (screen.height - height) / 2;
    var left = (screen.width - width) / 2;

    var wParam = "width=" + width;
    wParam += ", height=" + height;
    wParam += ", top=" + top + ", left=" + left;
    wParam += ", fullscreen=yes";
    wParam += ", location=no";
    var newWin = window.open(url, windowName, wParam);
    newWin.document.title = windowName;
    if (window.focus) { newWin.focus(); }
    return false;
}

$("[print]").click(function (e) {
    var html = "<html><head><link rel='stylesheet' type='text/css' href='~/Content/print.css'></head><body><div class='print-content' ></div></body></html>";
    
    var divToPrint = $('.section-to-print');
    var htmlToPrint = $(divToPrint).html();
    var printContainer = $(".print-container", html).html(htmlToPrint);
    console.log(htmlToPrint);
    var newWin = window.open("PRINT");
    newWin.document.write(htmlToPrint);

    var head = newWin.document.getElementsByTagName('HEAD')[0];
    var link = newWin.document.createElement('link');
    link.rel = 'stylesheet';
    link.type = 'text/css';
    link.href = '~/Content/print.css';
    head.appendChild(link);  

    newWin.print();
    newWin.close();
});

function showPDFReport(url, container = "") {

    var width = document.documentElement.clientWidth;
    var height = document.documentElement.clientHeight;

    if (container != "") {
        showLoading(container);
        var embedPDFContainer = "<embed class='embed-pdf-container' width=100% height=" + height + " src='" + url + "' type='application/pdf'>";
        $(container).append($(embedPDFContainer));
        hideLoading(container);
    }
    else {
        window.open(url, '_blank', "location=0,menubar=0,status=0,scrollbars=0,width=" + width + ",height=" + height);
    }
}
//----------------- END UTILITY ---------------------------