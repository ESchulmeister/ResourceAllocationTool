'use strict';

const HighlightColor = '#f3b7b5';

let passwordEditor = null, userNameEditor = null;
let cboProjects = null;
let returnUrl = null;
let userName = null, password = null;
let projectID = null;

let general_msg = 'An Unexpected error has occurred. Please contact the system administrator.';


$(document).ready(function () {

    hideSpinner();
    $('#divError').hide();
    $('#divPopup').hide();
    $('#divGrid').hide();
    $('#divManager').hide();
    $('#divEHA').hide();

    $('#divMenu').show();
    $('#divLoginPartial').show();

    returnUrl = getQSVars()["returnUrl"];

    $('#txtPassword, #txtUserName').keypress(function (e) {
        if (e.which == 13) { //Enter Key
            $('#btnLogin').trigger('click');
        }
    });

    $('#btnLogout').click(function () {
        $.ajax({
            type: 'PATCH',
            url: '../api/ApiAccount',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            cache: false,

            success:
                function (data) {
                    window.location.href = '/account/login';
                },
            error: function (xhr, ajaxOptions, thrownError) {

                let _msg =
                    ((xhr.status && xhr.status === 500) || xhr.responseJSON == undefined || xhr.responseJSON == null) ?
                        general_msg : xhr.responseJSON.replace("'", "");

                alert(_msg);
            }
        });

    });
});


function showSpinner() {
    let left = $('#container').width() /2;
    let top = $('#anchor').position.top;
    $('#divSpinner').css({ top: top , left: left, position: 'absolute' });
    $('#divSpinner').css('z-index', 9999);
    $('#divSpinner').show();
}



function get_credentials() {
    passwordEditor = $('#txtPassword').dxTextBox('instance');
    userNameEditor = $('#txtUserName').dxTextBox('instance');
    userName= userNameEditor.option('value');
    password = passwordEditor.option('value');
}

function userName_focus(e) {
    setTimeout(function () {
        e.component.focus();
    }, 0);
}

function hideSpinner()
{
    $('#divSpinner').hide();
}

function changePasswordMode() {

    get_credentials();
    passwordEditor.option('mode', passwordEditor.option('mode') === 'text' ? 'password' : 'text');
}




function btnLogin_Click() {
    $('#divError').hide();

    get_credentials();

    let postvalues = JSON.stringify({ UserName: userName, Password: password });   

    $.ajax({
        type: 'POST',
        url: '../api/ApiAccount',
        data: postvalues,
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        cache: false,
        beforeSend: function () {
            showSpinner();   


        },
        success:
            function (data) {
                window.location.href ='/';    //Home/Index view
            },
        error: function (xhr, ajaxOptions, thrownError) {

            hideSpinner();

            let _msg =
                ((xhr.status && xhr.status === 500) || xhr.responseJSON == undefined || xhr.responseJSON == null) ?
                    general_msg :   xhr.responseJSON.replace("'", "");

            $('#divError').html(_msg);
            $('#divError').show();
        }
    });
}


function userName_change() {
    setButtonEnabled();
}
function password_change() {
    setButtonEnabled();
}

function setButtonEnabled() {
    get_credentials();

    let isDisabled = (userName == null || userName.trim() === '' || password  == null || password.trim() === '');

    let button = $('#btnLogin').dxButton('instance');

    button.option('disabled', isDisabled);
}

function getQSVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}


function showPopup(e) {
    var login = $('#hdnLogin').val();

    var url = "../api/ApiUsers/" + login;

    $.get(url, function (currUser) {
        console.log(currUser);

        $('#lblName').val(currUser.FullName);
        $('#lblLogin').val(currUser.Login);
        $('#lblClock').val(currUser.Clock);
        $('#lblFTE').val(parseFloat(currUser.FTE).toFixed(2));

        if ( currUser.RoleID != null) {
            $('#lblRole').val(currUser.Role.Name);
    }

    })
    .fail(function (xhr) {
        let _msg =
            ((xhr.status && xhr.status === 500) || xhr.responseJSON == undefined || xhr.responseJSON == null) ?
                general_msg :  xhr.responseJSON.replace("'", "");
        alert(_msg);
    });

    $('#userDetails').modal({
        backdrop: false,
        show: true,
    });

    $('.modal-dialog').draggable({
        handle: '.modal-header'
    });

    $('.modal-dialog').attr('draggable', 'True');

}

function getProjectID() {
    return getComboValue('#selProjects');
}
function getPeriodID() {
    return getComboValue('#selPeriods');
}
function getEmployeeID() {
    return getComboValue('#selEmployees');
}

function getManagerID() {
    let mgrID = getComboValue('#selManagers');

    if (isNaN(parseInt(mgrID))) {
        mgrID = getQSVars()["id"];
    }

    return (mgrID == undefined || mgrID == null) ? 0 : mgrID;
}

function getComboValue(path) {
    const combo = $(path).dxSelectBox('instance');

    const id = combo.option('value');

    return (id == null) ? 0 : id;
}


function refreshTeamsGrid() {
    let projectID = getProjectID();

    if (projectID === 0) {
        $('#divGrid').hide();
        $('#divManager').hide();
        return;
    }

    setUpManager(projectID);

    $('#gridProjectTeams').dxDataGrid('refresh');
}

function refreshProjectAllocationsGrid() {
    let projectID = getProjectID();
    let periodID = getPeriodID();

    if (projectID === 0 || periodID == 0) {
        $('#divGrid').hide();
        return;
    }

    $('#gridProjectHourAllocation').dxDataGrid('refresh');
    $('#divGrid').show();
}

function refreshEmpByManagerGrid() {
    let mgrID = getManagerID();
    if (mgrID === 0 ) {
        $('#divGrid').hide();
        return;
    }

    $('#grdUsersByManager').dxDataGrid('refresh');
    $('#divGrid').show();
}

function refreshEmployeeAllocations() {
    let employeeID = getEmployeeID();
    let periodID = getPeriodID();

    if (employeeID === 0 || periodID == 0) {
        $('#divGrid').hide();
        $('#divEHA').hide();
        
        return;
    }

    loadEHA(employeeID, periodID);

    $('#gridEmployeeProjectHourAllocation').dxDataGrid('refresh');
    $('#divGrid').show();
}

function loadEHA(employeeID, periodID) {
    var url = "../api/ApiEmployeeAllocations/" + employeeID + '?periodID=' + periodID;

    $.get(url, function (totals) {
        console.log(totals);

        $('#divEHA').show();
        $('#lblFTEHours').html(parseFloat(totals.FTE));     // "lblFTE" conflicts with the Profile div
        $('#lblTotalHours').html(totals.TotalHours);
        $('#lblHoursRemaining').html(totals.RemainingHours);
        $('#lblAllocatedHours').html(totals.AllocatedHours);
        $('#lblHoursUsed').html(totals.UsedHours);

    })
    .fail(function (xhr) {
        let _msg =
            ((xhr.status && xhr.status === 500) || xhr.responseJSON == undefined || xhr.responseJSON == null) ?
                general_msg : xhr.responseJSON.replace("'", "");
        alert(_msg);
    });
}

function updateEmployeeAllocations(e) {
    refreshEmployeeAllocations();
}


function grdEmployees_OnEditorPreparing(e) {
    if (e.dataField === 'FullNameReversed' && e.parentType === 'dataRow') {
        e.editorOptions.disabled = true;
    }

    onSearch(e);
}


function grdProjectTeam_OnEditorPreparing(e) {
    if (!(e.dataField === 'UserID' && e.parentType === 'dataRow')) {
        return;
    }
    if (!e.row.isNewRow) {
        e.editorOptions.disabled = true;
        return;
    }

    e.editorOptions.disabled = false;

    let projectID = getProjectID();

    //ListAvailable  users
    let url = '../api/ApiUsers/' + projectID;

    var dataSource = {
        store: new DevExpress.data.AspNet.createStore({
            key: 'ID',
            loadUrl: url,
        }),
    }
    e.editorOptions.dataSource = dataSource;
}

function grdProjectAllocation_OnEditorPreparing(e) {
    e.editorOptions.disabled =
        (!(e.dataField === 'ActualHours' || e.dataField === 'EstimatedHours') && e.parentType === 'dataRow');
}
function grdEmployeeAllocations_OnEditorPreparing(e) {
    e.editorOptions.disabled =
        (!(e.dataField === 'ActualHours' || e.dataField === 'EstimatedHours') && e.parentType === 'dataRow');
}

function grdUsersByManager_OnEditorPreparing(e) {
    e.editorOptions.disabled =
        ((e.dataField === 'FullNameReversed' || e.dataField === 'FTE') && e.parentType === 'dataRow');

 }

//not used
function reverseName(name) {
    let parts = name.split(',');
    if (parts.length != 2) {
        return name;
    }

    return parts[1].trim() + ' ' + parts[0].trim();
}

function setUpManager(projectID) {
    var url = "../api/ApiManagers/" + projectID;

    $.get(url, function (projManager) {

        $('#divManager').show();

        if (projManager == undefined || projManager == null) {
            $('#divGrid').hide();
            $('#lblManager').hide();
            return;
        }

        $('#divGrid').show();
        $('#lblManager').show();

        var url = 'EmployeesByManager?id=' + projManager.ID + '&nm=' + projManager.FullNameReversed;
        $('#linkMgr').attr('href', url)

        $('#lblManager').html(projManager.FullName);

    })
    .fail(function (xhr) {
        let _msg =
            ((xhr.status && xhr.status === 500) || xhr.responseJSON == undefined || xhr.responseJSON == null) ?
                general_msg : xhr.responseJSON.replace("'", "");
        alert(_msg);
    });
}

function validate_fte(e) {
    let fte = e.data.FTE;
    return (fte != undefined && fte != null && fte > 0 && fte <= 1);
}

function validate_days(e) {
    let days = e.data.WorkDays;
    return (days != undefined && days != null && days > 0 && days <= 20);
}

function validate_hours(e) {
    let hours = e.data.WorkHours;
    return (hours != undefined && hours != null && hours > 0 && hours <= 200);
}

function grdPeriods_OnInitNewRow(e) {
    e.data.WorkDays = 20;
    e.data.WorkHours = 160;
}

function grdProjectTeam_OnInitNewRow(e) {
    e.data.ProjectID = getProjectID();
}

function grdPeriods_OnEditorPreparing(e) {
    if (e.parentType !== 'dataRow') {
        return;
    }

    const isNewRow = e.row.isNewRow;

    if (e.dataField === 'Name') {
        e.editorOptions.disabled = !isNewRow;
    }
}

function updateProjectAllocations(e) {
    refreshProjectAllocationsGrid();
}

function onSearch(e) {
    if (e.parentType === 'searchPanel') {
        e.editorOptions.onValueChanged = function (arg) {
            if (arg.value.length == 0 || arg.value.length >= 3) {
                e.component.searchByText(arg.value);
            }
        }
    }
}

function validate_actual(e) {
    let actual = e.data.ActualHours;
    return (actual != undefined && actual != null && actual > 0);
}

function setSelectedMgrValue() {
    let mgrName = getQSVars()["nm"];
    if (mgrName == undefined || mgrName == null) {
        return;
    }
    let _selBox = $('#selManagers').dxSelectBox('instance');

    _selBox.option('value', decodeURIComponent(mgrName));

    refreshEmpByManagerGrid();
}
