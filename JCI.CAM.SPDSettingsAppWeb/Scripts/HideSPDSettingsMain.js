if (typeof JCI_CAM_SPDSettingsApp_AppWeb != undefined) {
    var JCI_CAM_SPDSettingsApp_AppWeb = 'https://jciftccamapps9.azurewebsites.net/SPDSettings';
}
var headID = document.getElementsByTagName('head')[0];
var newScript = document.createElement('script');
newScript.type = 'text/javascript';
newScript.src = JCI_CAM_SPDSettingsApp_AppWeb + '/scripts/settings.js';
headID.appendChild(newScript);


if (typeof JCI_CAM_SPDSettingsApp_AppWeb != undefined) { var JCI_CAM_SPDSettingsApp_AppWeb = 'https://jciftccamapps9.azurewebsites.net/SPDSettings'; } var headID = document.getElementsByTagName('head')[0]; var newScript = document.createElement('script'); newScript.type = 'text/javascript'; newScript.src = JCI_CAM_SPDSettingsApp_AppWeb + '/scripts/settings.js'; headID.appendChild(newScript);