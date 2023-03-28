$(document).ready(LoadEvent);

function LoadEvent() {
    $(".member_icon").click(function () {
        $("#member_user").toggle();
    });

    $("#menu-button").click(function () {
        $("#main_menu").toggle();
    });

    $(window).resize(function (e) {
        var windowsize = $(window).width();
        if (windowsize >= 800) {
            $("#main_menu").show();
        }
    });


}




function PasswordValidation(Password) {
    if (Password.length < 8) {
        return "Password length minimum 8 characters.";
    }
    if (!Password.match(/(?=.*\d)/) || !Password.match(/(?=.*[a-z])/) || !Password.match(/(?=.*[A-Z])/) || !Password.match(/[0-9a-zA-Z]/)) {
        //if (!Password.match(/([a-zA-Z])/) || !Password.match(/([0-9])/)) {
        return "The password must have one upper case, one lower case and one digit.";
    }
    if (!checkspecialChars(Password)) {
        return "The password must have one symbol.";
    }
    return "";
}
var specialChars = "<>@!#$%^&*()_+[]{}?:;|'\"\\,./~`-=";
var checkspecialChars = function (string) {
    for (i = 0; i < specialChars.length; i++) {
        if (string.indexOf(specialChars[i]) > -1) {
            return true
        }
    }
    return false;
}

function isValidEmailAddress(EmailAddress) {
    var pattern = /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
    return pattern.test(EmailAddress);
}

(function ($, undefined) {
    var specialChars = "<>@!#$%^&*()_+[]{}?:;|'\"\\,./~`-=";
    window.IGScript = {

        // Password Validation
        PasswordValidation: function (Password) {
            if (Password.length < 6) {
                return "Password length minimum 6 characters.";
            }
            if (!Password.match(/([a-zA-Z])/) || !Password.match(/([0-9])/)) {
                return "password must one upper case, one lower case and one digit.";
            }
            if (!checkspecialChars(Password)) {
                return "Password must have one symbol.";
            }
            return "";
        },
        checkspecialChars: function (string) {
            for (i = 0; i < specialChars.length; i++) {
                if (string.indexOf(specialChars[i]) > -1) {
                    return true
                }
            }
            return false;
        },

        // Alerts
        WarningAlert: function (Message) {
            $.alert(Message, {
                title: 'Project Tracking System Warning',
                type: 'danger',
                closeTime: 5000

            });
        },
        ErrorAlert: function (Message) {
            $.alert(Message, {
                title: 'Project Tracking System Error',
                type: 'danger',
                closeTime: 5000
            });
        },

        SuccessAlert: function (Message) {
            $.alert(Message, {
                title: 'Project Tracking System Success',
                type: 'success',
                closeTime: 5000
            });
        },

        ConfirmationAlert: function (Message, SuccessFuntion, FailedFunction) {
            $('<div></div>').appendTo('body')
                            .html('<div><h6>' + Message + '?</h6></div>')
                            .dialog({
                                modal: true, title: 'Project Tracking System Confirmation', zIndex: 10000, autoOpen: true,
                                width: 'auto', resizable: false,
                                dialogClass: 'no-close',
                                buttons: {
                                    Yes: function () {
                                        $(this).dialog("close");
                                        SuccessFuntion();

                                    },
                                    No: function () {
                                        $(this).dialog("close");
                                        FailedFunction();

                                    }
                                },
                                close: function (event, ui) {
                                    $(this).dialog("close");
                                    $(this).remove();
                                }
                            });
            $(".ui-dialog-titlebar-close").text('X');
        },

        // Validation
        isValidEmailAddress: function (EmailAddress) {
            var pattern = /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
            return pattern.test(EmailAddress);
        },
        ToFloat: function (stirngvalue) {

            if (stirngvalue == "NaN") {
                stirngvalue = '0';
            }

            if (stirngvalue == "NaN" || stirngvalue.length === 0) {
                return 0;
            } else {

                if (stirngvalue.indexOf(",") >= 0) {
                    stirngvalue.replace(",", "");
                }

                return parseFloat(stirngvalue);

            }
        },
        ToInt: function (stirngvalue) {
            if (stirngvalue == "NaN") {
                return 0;
            } else {
                return parseInt(stirngvalue);
            }
        }

    }
})(jQuery);

Array.prototype.Remove = function (x) {
    return this.filter(function (v) {
        return v !== x;
    });
};
Array.prototype.Add = function (x) {
    this.push(x);
};
Array.prototype.Count = function (x) {
    return this.length;
};

Array.prototype.Find = function (x) {
    var result = false;
    for (var i = 0; i < this.length; i++) {
        if (this[i] === x) {
            result = true;
        }
    }
    return result;
};



Array.prototype.RemoveByValue = function (val) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] === val) {
            this.splice(i, 1);
            i--;
        }
    }
    return this;
}
//String.prototype.replaceAll = function (search, replacement) {
//    var target = this;
//    return target.replace(new RegExp(search, 'g'), replacement);
//};

function onlyNumbers(chr) {
    return !(chr > 31 && (chr < 48 || chr > 57))
}
function onlyNumbersdecimal(chr) {
    return !(chr > 31 && (chr < 46 || chr > 57))

}
function filterDecimals(inp) {
    return inp.replace(/^(\d*\.\d)\d*$/g, '$1');
}
function isTextSelected(input) {

    if (typeof input.selectionStart == "number") {
        return input.selectionStart == 0 && input.selectionEnd == input.value.length;
    } else if (typeof document.selection != "undefined") {
        input.focus();
        return document.selection.createRange().text == input.value;
    }
}
function textLength(value) {
    var maxLength = 144;

    if (value.length > maxLength) return false;
    return true;
}

function ismoney(event, element) {
    var chr = window.event ? event.keyCode : event.which;
    if ((chr > 31 && (chr < 46 || chr > 57)))
        return false;

    var valid = /^\d{0,20}(\.\d{0,2})?$/.test(this.value),q
      val = this.value;

    //if (!valid) 
    //this.value = val.substring(0, val.length - 1);
}
function minmax(value, min, max) {

    if (parseInt(value) < min || isNaN(parseInt(value)))
        return 0;
    else if (parseInt(value) > max)
        return 100;
    else return value;
}
//function ismoney(event, element) {

//    var charCode = window.event ? event.keyCode : event.which;
//    if (charCode == 8 || charCode == 0)
//        return true;

//    if ((charCode != 45 || $(element).val().indexOf('-') != -1) &&      // “-” CHECK MINUS, AND ONLY ONE.
//        (charCode != 46 || $(element).val().indexOf('.') != -1) &&      // “.” CHECK DOT, AND ONLY ONE.
//        (charCode < 48 || charCode > 57))

//        return false;

//    if (element.value.indexOf(".") > 0)
//    {
//        if (element.value.length >= event.target.selectionEnd && event.target.selectionEnd > element.value.indexOf(".") + 3) return false;
//    }

//    return true;


//}

$(document).ready(function () {
    $(".currency").attr('maxlength', '20');
});
// Remove commas
function RemoveComma(number) {
    var result = number.replace(/,/g, "");
    return result;
}
// for us comma sepraters 
function numberWithCommas(value) {
    if (value == null) {
        return 0;
    }
    if (value == null) {
        return 0;
    }

    if (value.toString().indexOf(".") < 0) {

        var a = Number.parseFloat(value);
        value = a.toFixed(3);
    }

    var parts = value.toString().split(".");
    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return parts.join(".");
}

$(".currency").focusout(function () {
    var num = $(this).val().replace(/,/g, "");
    if (num.length > 0) {
        var commaNum = numberWithCommas(num);
        $(this).val(commaNum);
    }
    else
        $(this).val("0.000");
});

// convert to 3 decimal points
$('.currency').on('change', function () {
    var num = this.value.replace(/,/g, "");
    if (num.length > 0) {
        this.value = parseFloat(num).toFixed(3);
        $(".currency").digits();
    }
    else
        $(this).val("0.000");
});
// resticted to 0 to 100
$(".percentage").keydown(function () {
    // Save old value.
    $(this).data("old", $(this).val());

});
$(".percentage").keyup(function () {
    // Check correct, else revert back to old value.
    if (parseInt($(this).val()) <= 100 && parseInt($(this).val()) >= 0)
        ;
    else
        $(this).val($(this).data("old"));
});
// date format
function formatdate(date) {

    if (date == null) return "";
    if (date == "") return "";

    var milli = date.replace(/\/Date\((-?\d+)\)\//, '$1');
    var d = new Date(parseInt(milli));
    // var d = new Date();           

    day = d.getDate();
    day = day < 10 ? "0" + day : day;



    mon = d.getMonth() + 1;
    mon = mon < 10 ? "0" + mon : mon;

    year = d.getFullYear()

    hours = d.getHours();
    hours = hours < 10 ? "0" + hours : hours;

    minutes = d.getMinutes();
    minutes = minutes < 10 ? "0" + minutes : minutes;

    seconds = d.getSeconds();
    seconds = seconds < 10 ? "0" + seconds : seconds;

    var hours = (hours + 24 - 2) % 24;
    var mid = 'am';
    if (hours == 0) { //At 00 hours we need to show 12 am
        hours = 12;
    }
    else if (hours > 12) {
        hours = hours % 12;
        mid = 'pm';
    }
    //return (mon + "/" + day + "/" + year + " " + hours + ":" + minutes + ":" + seconds + " " + mid);
    return (mon + "/" + day + "/" + year);
}
function formatdatetime(date) {

    if (date != null) {
        var milli = date.replace(/\/Date\((-?\d+)\)\//, '$1');
        var d = new Date(parseInt(milli));
        // var d = new Date();           

        day = d.getDate();
        day = day < 10 ? "0" + day : day;



        mon = d.getMonth() + 1;
        mon = mon < 10 ? "0" + mon : mon;        
        year = d.getFullYear()

        hours = d.getHours();
        hours = hours < 10 ? "0" + hours : hours;

        minutes = d.getMinutes();
        minutes = minutes < 10 ? "0" + minutes : minutes;

        seconds = d.getSeconds();
        seconds = seconds < 10 ? "0" + seconds : seconds;

        //var hours = hours;//(hours + 24 - 2) % 24;
        //var mid = 'am';
        //if (hours == 0) { //At 00 hours we need to show 12 am
        //    hours = 12;
        //}
        //else if (hours => 12) {
        //    hours = hours % 12;
        //    mid = 'pm';
        //}
        //+ " " + mid
        return (day + "/" + mon + "/" + year + " " + hours + ":" + minutes + ":" + seconds);
        //return (mon + "/" + day + "/" + year);
    } else {
        return "";
    }
}

// date format ddMMyyyy
function formatdateddMMyyyy(date) {

    var milli = date.replace(/\/Date\((-?\d+)\)\//, '$1');
    var d = new Date(parseInt(milli));
    // var d = new Date();           

    day = d.getDate();
    day = day < 10 ? "0" + day : day;



    mon = d.getMonth() + 1;
    mon = mon < 10 ? "0" + mon : mon;

    year = d.getFullYear()

    hours = d.getHours();
    hours = hours < 10 ? "0" + hours : hours;

    minutes = d.getMinutes();
    minutes = minutes < 10 ? "0" + minutes : minutes;

    seconds = d.getSeconds();
    seconds = seconds < 10 ? "0" + seconds : seconds;

    var hours = (hours + 24 - 2) % 24;
    var mid = 'am';
    if (hours == 0) { //At 00 hours we need to show 12 am
        hours = 12;
    }
    else if (hours > 12) {
        hours = hours % 12;
        mid = 'pm';
    }
    //return (mon + "/" + day + "/" + year + " " + hours + ":" + minutes + ":" + seconds + " " + mid);
    return (day + "/" + mon + "/" + year);
}

function TodateTime(date) {
    var str, year, month, day, hour, minute, d, finalDate;

    str = date.replace(/\D/g, "");
    d = new Date(parseInt(str));

    year = d.getFullYear();
    month = pad(d.getMonth() + 1);
    day = pad(d.getDate());
    hour = pad(d.getHours());
    minutes = pad(d.getMinutes());

    finalDate = year + "-" + month + "-" + day + " " + hour + ":" + minutes;

    function pad(num) {
        num = "0" + num;
        return num.slice(-2);
    }
}

$.fn.digits = function () {
    return this.each(function () {
        $(this).text($(this).text().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,"));
    })
}