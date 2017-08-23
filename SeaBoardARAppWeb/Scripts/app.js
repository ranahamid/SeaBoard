// variable used for cross site CSOM calls
var context;
// peoplePicker variable needs to be globally scoped as the generated html contains JS that will call into functions of this class
var peoplePicker, AudienceValue, PMOwnerValue;
//var peoplePicker, Audience, PMOwner, ForwardRequesTo, BackupDesignerValue, Designer;
var csomPeoplePicker;
var spLanguage;
//Wait for the page to load
function PeoplePickerSB(spHostUrlfrm, appWebUrlfrm, spLanguagefrm) {
    
    $(document).ready(function () {
        //Get the URI decoded SharePoint site url from the SPHostUrl parameter.
        var spHostUrl = decodeURIComponent(spHostUrlfrm);
        
        var appWebUrl = decodeURIComponent($('#SPAppWebUrl').val());
        if (typeof (Storage) !== "undefined") {
            if (appWebUrl == null || appWebUrl == "" || appWebUrl == "null" || appWebUrl == undefined || appWebUrl == "undefined") {
                appWebUrl = localStorage["appWebUrl"];
            }
            else {
                localStorage["appWebUrl"] = appWebUrl;
            }
        } else {
            // Sorry! No Web Storage support..
        }
        spLanguage = decodeURIComponent(spLanguagefrm);
        //Build absolute path to the layouts root with the spHostUrl
        var layoutsRoot = spHostUrl + '/_layouts/15/';

        //load all appropriate scripts for the page to function
        $.getScript(layoutsRoot + 'SP.Runtime.js',
            function () {
                $.getScript(layoutsRoot + 'SP.js',
                    function () {
                        //Load the SP.UI.Controls.js file to render the App Chrome
                        $.getScript(layoutsRoot + 'SP.UI.Controls.js', renderSPChrome);

                        //load scripts for cross site calls (needed to use the people picker control in an IFrame)
                        $.getScript(layoutsRoot + 'SP.RequestExecutor.js', function () {
                            context = new SP.ClientContext(appWebUrl);
                            var factory = new SP.ProxyWebRequestExecutorFactory(appWebUrl);
                            context.set_webRequestExecutorFactory(factory);
                            //RenderUserControlDesigner();
                            try {
                                PMOwnerValue = PeoplePickerVarAssign($('#spanPMOwner'), $('#inputPMOwner'), $('#divPMOwnerSearch'), $('#PMOwnerValue'), "PMOwnerValue", "#PMOwner", "/Maintab/GetPeoplePickerData");
                            }
                            catch (e) {

                            }
                            //Make a people picker control
                            //1. context = SharePoint Client Context object
                            //2. $('#spanAdministrators') = SPAN that will 'host' the people picker control
                            //3. $('#inputAdministrators') = INPUT that will be used to capture user input
                            //4. $('#divAdministratorsSearch') = DIV that will show the 'dropdown' of the people picker
                            //5. $('#hdnAdministrators') = INPUT hidden control that will host a JSON string of the resolved users
                            peoplePicker = new CAMControl.PeoplePicker(context, $('#spanAdministrators'), $('#inputAdministrators'), $('#divAdministratorsSearch'), $('#hdnAdministrators'));
                            // required to pass the variable name here!
                            peoplePicker.InstanceName = "peoplePicker";
                            // Pass current language, if not set defaults to en-US. Use the SPLanguage query string param or provide a string like "nl-BE"
                            // Do not set the Language property if you do not have foreseen javascript resource file for your language
                            peoplePicker.Language = spLanguage;
                            // optionally show more/less entries in the people picker dropdown, 4 is the default
                            peoplePicker.MaxEntriesShown = 5;
                            // Can duplicate entries be selected (default = false)
                            peoplePicker.AllowDuplicates = false;
                            // Show the user loginname
                            peoplePicker.ShowLoginName = true;
                            // Show the user title
                            peoplePicker.ShowTitle = true;
                            // Set principal type to determine what is shown (default = 1, only users are resolved). 
                            // See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx for more details
                            // Set ShowLoginName and ShowTitle to false if you're resolving groups
                            peoplePicker.PrincipalType = 1;
                            // start user resolving as of 2 entered characters (= default)
                            peoplePicker.MinimalCharactersBeforeSearching = 2;
                            // Hookup everything
                            peoplePicker.Initialize();

                            peoplePicker.ServerDataMethod = "Maintab/GetPeoplePickerData";

                            //Make a Csom people picker control
                            //1. context = SharePoint Client Context object
                            //2. $('#spanCsomAdministrators') = SPAN that will 'host' the people picker control
                            //3. $('#inputCsomAdministrators') = INPUT that will be used to capture user input
                            //4. $('#divCsomAdministratorsSearch') = DIV that will show the 'dropdown' of the people picker
                            //5. $('#hdnCsomAdministrators') = INPUT hidden control that will host a JSON string of the resolved users
                            //6. data url on the server (webmethod in webforms, controller action in MVC)
                            csomPeoplePicker = new CAMControl.PeoplePicker(context, $('#spanCsomAdministrators'), $('#inputCsomAdministrators'), $('#divCsomAdministratorsSearch'), $('#hdnCsomAdministrators'), 'ServiceRequest/GetPeoplePickerData');
                            // required to pass the variable name here!
                            csomPeoplePicker.InstanceName = "csomPeoplePicker";
                            // Pass current language, if not set defaults to en-US. Use the SPLanguage query string param or provide a string like "nl-BE"
                            // Do not set the Language property if you do not have foreseen javascript resource file for your language
                            csomPeoplePicker.Language = spLanguage;
                            // optionally show more/less entries in the people picker dropdown, 4 is the default
                            csomPeoplePicker.MaxEntriesShown = 5;
                            // Can duplicate entries be selected (default = false)
                            csomPeoplePicker.AllowDuplicates = false;
                            // Show the user loginname
                            csomPeoplePicker.ShowLoginName = true;
                            // Show the user title
                            csomPeoplePicker.ShowTitle = true;
                            // Set principal type to determine what is shown (default = 1, only users are resolved). 
                            // See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx for more details
                            // Set ShowLoginName and ShowTitle to false if you're resolving groups
                            csomPeoplePicker.PrincipalType = 1;
                            // start user resolving as of 2 entered characters (= default)
                            csomPeoplePicker.MinimalCharactersBeforeSearching = 2;
                            // Hookup everything
                            csomPeoplePicker.Initialize();
                        });

                    });
            });

        $("#GetValuesByJavascript").click(function (event) {
            event.preventDefault();
            //get json string from hidden field and parse it to PeoplePickerUser object
            var pickedPeople = $.parseJSON($('#hdnCsomAdministrators').val());

            var pickedPeopleString = "";

            //loop picked persons and create string to show
            $.each(pickedPeople, function (key, value) {
                pickedPeopleString += value.Name + " ";
            });

            //alert(pickedPeopleString);
        });
    });
}

//function to get a parameter value by a specific key
function getQueryStringParameter(urlParameterKey) {
    var params = document.URL.split('?')[1].split('&');
    var strParams = '';
    for (var i = 0; i < params.length; i = i + 1) {
        var singleParam = params[i].split('=');
        if (singleParam[0] === urlParameterKey)
            return singleParam[1];
    }
}

function chromeLoaded() {
    // $('body').show();
}
//function callback to render chrome after SP.UI.Controls.js loads
function renderSPChrome() {
    //Set the chrome options for launching Help, Account, and Contact pages
    var options = {
        'appTitle': document.title,
        'onCssLoaded': 'chromeLoaded()'
    };
    //Load the Chrome Control in the divSPChrome element of the page
    var chromeNavigation = new SP.UI.Controls.Navigation('divSPChrome', options);
    chromeNavigation.setVisible(true);
}

function RenderUserControlPMOwner() {
    //Make a people picker control
    //1. context = SharePoint Client Context object
    //2. $('#spanAdministrators') = SPAN that will 'host' the people picker control
    //3. $('#inputAdministrators') = INPUT that will be used to capture user input
    //4. $('#divAdministratorsSearch') = DIV that will show the 'dropdown' of the people picker
    //5. $('#hdnAdministrators') = INPUT hidden control that will host a JSON string of the resolved users
    PMOwner = new CAMControl.PeoplePicker(context, $('#spanPMOwner'), $('#inputPMOwner'), $('#divPMOwnerSearch'), $('#PMOwner'));
    // required to pass the variable name here!
    PMOwner.InstanceName = "PMOwner";
    // Pass current language, if not set defaults to en-US. Use the SPLanguage query string param or provide a string like "nl-BE"
    // Do not set the Language property if you do not have foreseen javascript resource file for your language
    PMOwner.Language = spLanguage;
    // optionally show more/less entries in the people picker dropdown, 4 is the default
    PMOwner.MaxEntriesShown = 5;
    // Can duplicate entries be selected (default = false)
    PMOwner.AllowDuplicates = false;
    // Show the user loginname
    PMOwner.ShowLoginName = true;
    // Show the user title
    PMOwner.ShowTitle = true;
    // Set principal type to determine what is shown (default = 1, only users are resolved). 
    // See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx for more details
    // Set ShowLoginName and ShowTitle to false if you're resolving groups
    PMOwner.PrincipalType = 1;
    // start user resolving as of 2 entered characters (= default)
    PMOwner.MinimalCharactersBeforeSearching = 2;
    // Hookup everything
    PMOwner.Initialize();

    PMOwner.ServerDataMethod = "ServiceRequest/GetPeoplePickerData";
}

function PeoplePickerVarAssign(spanControl, inputControl, divControlSearch, hiddenControl, variableName, valueFieldId, serverDataMethod) {
    //Make a people picker control
    //1. context = SharePoint Client Context object
    //2. $('#spanAdministrators') = SPAN that will 'host' the people picker control
    //3. $('#inputAdministrators') = INPUT that will be used to capture user input
    //4. $('#divAdministratorsSearch') = DIV that will show the 'dropdown' of the people picker
    //5. $('#hdnAdministrators') = INPUT hidden control that will host a JSON string of the resolved users
    var element = new CAMControl.PeoplePicker(context, spanControl, inputControl, divControlSearch, hiddenControl);
    // required to pass the variable name here!
    element.InstanceName = variableName;
    // Pass current language, if not set defaults to en-US. Use the SPLanguage query string param or provide a string like "nl-BE"
    // Do not set the Language property if you do not have foreseen javascript resource file for your language
    element.Language = spLanguage;
    // optionally show more/less entries in the people picker dropdown, 4 is the default
    element.MaxEntriesShown = 5;
    // Can duplicate entries be selected (default = false)
    element.AllowDuplicates = false;
    // Show the user loginname
    element.ShowLoginName = true;
    // Show the user title
    element.ShowTitle = true;
    // Set principal type to determine what is shown (default = 1, only users are resolved). 
    // See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx for more details
    // Set ShowLoginName and ShowTitle to false if you're resolving groups
    element.PrincipalType = 1;
    // start user resolving as of 2 entered characters (= default)
    element.MinimalCharactersBeforeSearching = 2;
    // Hookup everything
    element.Initialize();
    element.ValueFieldId = valueFieldId;
    element.ServerDataMethod = serverDataMethod;
    return element;
}
function RenderUserControlAudience() {
    //Make a people picker control
    //1. context = SharePoint Client Context object
    //2. $('#spanAdministrators') = SPAN that will 'host' the people picker control
    //3. $('#inputAdministrators') = INPUT that will be used to capture user input
    //4. $('#divAdministratorsSearch') = DIV that will show the 'dropdown' of the people picker
    //5. $('#hdnAdministrators') = INPUT hidden control that will host a JSON string of the resolved users
    Audience = new CAMControl.PeoplePicker(context, $('#spanAudience'), $('#inputAudience'), $('#divAudienceSearch'), $('#Audience'));
    // required to pass the variable name here!
    Audience.InstanceName = "Audience";
    // Pass current language, if not set defaults to en-US. Use the SPLanguage query string param or provide a string like "nl-BE"
    // Do not set the Language property if you do not have foreseen javascript resource file for your language
    Audience.Language = spLanguage;
    // optionally show more/less entries in the people picker dropdown, 4 is the default
    Audience.MaxEntriesShown = 5;
    // Can duplicate entries be selected (default = false)
    Audience.AllowDuplicates = false;
    // Show the user loginname
    Audience.ShowLoginName = true;
    // Show the user title
    Audience.ShowTitle = true;
    // Set principal type to determine what is shown (default = 1, only users are resolved). 
    // See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx for more details
    // Set ShowLoginName and ShowTitle to false if you're resolving groups
    Audience.PrincipalType = 1;
    // start user resolving as of 2 entered characters (= default)
    Audience.MinimalCharactersBeforeSearching = 2;
    // Hookup everything
    Audience.Initialize();

    Audience.ServerDataMethod = "ServiceRequest/GetPeoplePickerData";
}

function RenderUserControlBackupDesigner() {
    //Make a people picker control
    //1. context = SharePoint Client Context object
    //2. $('#spanAdministrators') = SPAN that will 'host' the people picker control
    //3. $('#inputAdministrators') = INPUT that will be used to capture user input
    //4. $('#divAdministratorsSearch') = DIV that will show the 'dropdown' of the people picker
    //5. $('#hdnAdministrators') = INPUT hidden control that will host a JSON string of the resolved users
    BackupDesignerValue = new CAMControl.PeoplePicker(context, $('#spanBackupDesigner'), $('#inputBackupDesigner'), $('#divBackupDesignerSearch'), $('#BackupDesignerValue'));
    // required to pass the variable name here!
    BackupDesignerValue.InstanceName = "BackupDesignerValue";
    // Pass current language, if not set defaults to en-US. Use the SPLanguage query string param or provide a string like "nl-BE"
    // Do not set the Language property if you do not have foreseen javascript resource file for your language
    BackupDesignerValue.Language = spLanguage;
    // optionally show more/less entries in the people picker dropdown, 4 is the default
    BackupDesignerValue.MaxEntriesShown = 5;
    // Can duplicate entries be selected (default = false)
    BackupDesignerValue.AllowDuplicates = false;
    // Show the user loginname
    BackupDesignerValue.ShowLoginName = true;
    // Show the user title
    BackupDesignerValue.ShowTitle = true;
    // Set principal type to determine what is shown (default = 1, only users are resolved). 
    // See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx for more details
    // Set ShowLoginName and ShowTitle to false if you're resolving groups
    BackupDesignerValue.PrincipalType = 1;
    // start user resolving as of 2 entered characters (= default)
    BackupDesignerValue.MinimalCharactersBeforeSearching = 2;
    // Hookup everything
    BackupDesignerValue.Initialize();

    BackupDesignerValue.ServerDataMethod = "GetPeoplePickerData";
    BackupDesignerValue.ValueFieldId = "#BackupDesigner";
}

function RenderUserControlDesigner() {
    //Make a people picker control
    //1. context = SharePoint Client Context object
    //2. $('#spanAdministrators') = SPAN that will 'host' the people picker control
    //3. $('#inputAdministrators') = INPUT that will be used to capture user input
    //4. $('#divAdministratorsSearch') = DIV that will show the 'dropdown' of the people picker
    //5. $('#hdnAdministrators') = INPUT hidden control that will host a JSON string of the resolved users
    Designer = new CAMControl.PeoplePicker(context, $('#spanDesigner'), $('#inputDesigner'), $('#divDesignerSearch'), $('#Designer'));
    // required to pass the variable name here!
    Designer.InstanceName = "Designer";
    // Pass current language, if not set defaults to en-US. Use the SPLanguage query string param or provide a string like "nl-BE"
    // Do not set the Language property if you do not have foreseen javascript resource file for your language
    Designer.Language = spLanguage;
    // optionally show more/less entries in the people picker dropdown, 4 is the default
    Designer.MaxEntriesShown = 5;
    // Can duplicate entries be selected (default = false)
    Designer.AllowDuplicates = false;
    // Show the user loginname
    Designer.ShowLoginName = true;
    // Show the user title
    Designer.ShowTitle = true;
    // Set principal type to determine what is shown (default = 1, only users are resolved). 
    // See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx for more details
    // Set ShowLoginName and ShowTitle to false if you're resolving groups
    Designer.PrincipalType = 1;
    // start user resolving as of 2 entered characters (= default)
    Designer.MinimalCharactersBeforeSearching = 2;
    // Hookup everything
    Designer.Initialize();

    Designer.ServerDataMethod = "GetPeoplePickerData";
}

function RenderUserControl(role) {
    //Make a people picker control
    //1. context = SharePoint Client Context object
    //2. $('#spanAdministrators') = SPAN that will 'host' the people picker control
    //3. $('#inputAdministrators') = INPUT that will be used to capture user input
    //4. $('#divAdministratorsSearch') = DIV that will show the 'dropdown' of the people picker
    //5. $('#hdnAdministrators') = INPUT hidden control that will host a JSON string of the resolved users
    peoplePicker = new CAMControl.PeoplePicker(context, $('#span' + role), $('#input' + role), $('#div' + role + 'Search'), $('#' + role));
    // required to pass the variable name here!
    peoplePicker.InstanceName = role;
    // Pass current language, if not set defaults to en-US. Use the SPLanguage query string param or provide a string like "nl-BE"
    // Do not set the Language property if you do not have foreseen javascript resource file for your language
    peoplePicker.Language = spLanguage;
    // optionally show more/less entries in the people picker dropdown, 4 is the default
    peoplePicker.MaxEntriesShown = 5;
    // Can duplicate entries be selected (default = false)
    peoplePicker.AllowDuplicates = false;
    // Show the user loginname
    peoplePicker.ShowLoginName = true;
    // Show the user title
    peoplePicker.ShowTitle = true;
    // Set principal type to determine what is shown (default = 1, only users are resolved). 
    // See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx for more details
    // Set ShowLoginName and ShowTitle to false if you're resolving groups
    peoplePicker.PrincipalType = 1;
    // start user resolving as of 2 entered characters (= default)
    peoplePicker.MinimalCharactersBeforeSearching = 2;
    // Hookup everything
    peoplePicker.Initialize();

    peoplePicker.ServerDataMethod = "ServiceRequest/GetPeoplePickerData";
}