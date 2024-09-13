module.exports = function () {
    var instanceRoot = "E:\\www\\test";
  var config = {
    websiteRoot: instanceRoot + "\\",
    sitecoreLibraries: instanceRoot + "\\bin",
    licensePath: instanceRoot + "\\App_Data\\license.xml",
      packageXmlBasePath: ".\\src\\Project\\DEWAXP\\code\\App_Data\\packages\\DEWAXP.xml",
    packagePath: instanceRoot + "\\App_Data\\packages",
    solutionName: "DEWAXP",
    buildConfiguration: "Debug",
    buildToolsVersion: '17.0',
    buildMaxCpuCount: 0,
    buildVerbosity: "minimal",
    buildPlatform: "Any CPU",
    publishPlatform: "AnyCpu",
    runCleanBuilds: false
  };
  return config;
}
