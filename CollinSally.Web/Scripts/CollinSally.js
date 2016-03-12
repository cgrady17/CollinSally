$(function () {
    //// NAVIGATION [BEGIN] ////
    $("#menu-icon").click(function () {
        var icon = $(this).children(".glyphicon");
        if (icon.hasClass("glyphicon-menu-hamburger")) {
            icon.removeClass("glyphicon-menu-hamburger").addClass("glyphicon-remove");
            $("#nav-wrap").show(400);
        } else {
            icon.addClass("glyphicon-menu-hamburger").removeClass("glyphicon-remove");
            $("#nav-wrap").hide(400);
        }
    });

    //// NAVIGATION [END] ////

    //// WEDDING [BEGIN] ////

    /**
     * On-Change event for the "Are you attending?" radio button question.
     * Reveals the arrival date field if affirmative.
     */
    $("input[name='Attending']").change(function () {
        $("#otherAttendeesGroup").show(400);
    });

    $("input[name='OtherAttendees']").change(function () {
        if ($(this).data("radio-bool") === "True") {
            $("#attendee-list").show(400);
        } else {
            $("#attendee-list").hide(400);
        }

        $("button[type=submit]").parent().show(400);
    });

    if ($("input[name='Name']").length) $("input[name='Name']").focus();
    if ($("input[name='CaptchaPass']").length) $("input[name='CaptchaPass']").val("COLLINSALLY_NOT_ROBOT");

    /**
     * Performs operations immediately before the submission of the RSVP form.
     * @param {} formData
     * @param {} jqForm
     * @param {} options
     * @returns {}
     */
    var rsvpBeforeSubmit = function (formData, jqForm, options) {
        jqForm.hide(400);
        $(".form-loader").show(400);
    };

    /**
     * Performs operations on a successful submission of the RSVP form.
     * @param {} responseText
     * @param {} statusText
     * @param {} xhr
     * @param {} $form
     * @returns {}
     */
    var rsvpSuccess = function (responseText, statusText, xhr, $form) {
        $(".form-loader").hide(400);
        var outputDiv = $("#rsvp-form-output");
        var name = $("input[name='Name']").val();
        var attending = $("input[name='Attending'][data-radio-bool='True']").is(":checked");
        if (responseText.status === 0) {
            // Error
            outputDiv.addClass("alert alert-danger");
            outputDiv.html("<h3><strong>Error!</strong> Sorry " + name + ", it looks like we ran into some trouble recording your RSVP. Here's the error message: " + responseText.error + "</h3>");
        } else {
            // Success
            outputDiv.addClass("alert alert-success");
            var innerMsg = attending ? "We'll keep you up to date on the latest Celebration information using the email address you provided. We can't wait to see you there!" : "We're sorry you won't be able to attend and we'll miss you!";
            outputDiv.html("<h3><strong>Thanks " + name + "!</strong> We've successfully recorded your RSVP for Sally & Collin's Marriage Celebration. " + innerMsg + "</h3>");

            if (attending) {
                setTimeout(function () {
                    $("#rsvp-next").show(400);
                }, 1500);
            }
        }
    };

    var rsvpError = function (xhr, textStatus, errorThrown) {
        $(".form-loader").hide(400);
        var outputDiv = $("#rsvp-form-output");
        var name = $("input[name='Name']").val();
        outputDiv.addClass("alert alert-danger");
        outputDiv.html("<h3><strong>Error!</strong> Sorry " + name + ", it looks like we ran into some trouble recording your RSVP. Please try again later. Here's the error message: " + textStatus + "</h3>");
    };

    /**
     * Prepares the RSVP form for AJAX-based submission.
     */
    $("#rsvp-form").ajaxForm({
        target: "#rsvp-form-output",
        beforeSubmit: rsvpBeforeSubmit,
        success: rsvpSuccess,
        error: rsvpError,
        dataType: "json"
    });

    $("#attendee-list").dynamiclist();

    //// WEDDING [END] ////
});

(function (collinSally) {
})(window.collinSally || (window.collinSally = {}));