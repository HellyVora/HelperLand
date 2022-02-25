var globalVar = "Global to Booking page";
function Tab1Click() {
    $("#tab1").addClass("active-tab");
    $("#tab2").removeClass("active-tab").addClass("tab");
    $("#tab3").removeClass("active-tab").addClass("tab");
    $("#tab4").removeClass("active-tab").addClass("tab");

    $("#tabContent2").hide();
    $("#tabContent3").hide();
    $("#tabContent4").hide();
    $("#tabContent1").show();
}

function Tab2Click() {
    $("#tab2").addClass("active-tab");
    $("#tab1").removeClass("active-tab").addClass("tab");
    $("#tab3").removeClass("active-tab").addClass("tab");
    $("#tab4").removeClass("active-tab").addClass("tab");

    $("#tabContent1").hide();
    $("#tabContent3").hide();
    $("#tabContent4").hide();
    $("#tabContent2").show();
}

function Tab3Click() {
    $("#tab3").addClass("active-tab");
    $("#tab1").removeClass("active-tab").addClass("tab");
    $("#tab2").removeClass("active-tab").addClass("tab");
    $("#tab4").removeClass("active-tab").addClass("tab");

    $("#tabContent1").hide();
    $("#tabContent2").hide();
    $("#tabContent4").hide();
    $("#tabContent3").show();
    $("#divAddress").html("Loading Address view...")
        .load('@Url.Action("GetAddress", "BookServiceRequest")');
}
function Tab4Click() {
    $("#tab4").addClass("active-tab");
    $("#tab1").removeClass("active-tab").addClass("tab");
    $("#tab2").removeClass("active-tab").addClass("tab");
    $("#tab3").removeClass("active-tab").addClass("tab");

    $("#tabContent1").hide();
    $("#tabContent2").hide();
    $("#tabContent3").hide();
    $("#tabContent4").show();
}


function SearchZipCode() {
    $("#tab2").addClass("active-tab");
    $("#tab1").removeClass("active-tab").addClass("tab");
    $("#tabContent1").hide();
    $("#tabContent2").show();
}

function SaveServiceDetail() {
    $("#tab3").addClass("active-tab");
    $("#tab2").removeClass("active-tab").addClass("tab");
    $("#tabContent2").hide();
    $("#tabContent4").hide();
    $("#tabContent3").show();

    $("#divAddress").html("Loading Address view...")
        .load('@Url.Action("GetAddress", "BookServiceRequest")');
}
function SaveAddress() {
    $("#tab4").addClass("active-tab");
    $("#tab3").removeClass("active-tab").addClass("tab");
    $("#tabContent3").hide();
    $("#tabContent4").show();
    $("#confirmZipCode").html($("#ZipCode").val());
}

function CompleteBooking() {
    debugger;
    var booking = {};
    booking.zipCode = $("#ZipCode").val();
    booking.bookingStartTime = $("#StartDate").val() + " " + $("#StartTime").val();
    booking.hours = $("#Hours").val();
    booking.address1 = $("#Address1").val();

    $.ajax({
        url: '@Url.Action("SaveBooking", "BookServiceRequest")',
        type: 'post',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(booking),
        success: function (resp) {
            debugger;
            $("#successMessage").show();
            var html = "<br/><br/>Congratulations!<br/> Your Booking hass been confirmed for <br/><b>Service Date</b>: " + resp.Result.bookingStartTime + "<br/>" +
                "<b>Service Hours</b>: " + resp.Result.hours + "<br/>" +
                "<b>Zip Code</b>: " + resp.Result.zipCode + "<br/>";
            $("#successMessage").html(html).fadeOut(7000);
        },
        error: function (err) {
            debugger;
            alert(err.responseText);
        }
    });
}