var root = ProjectRootDirectoryAttribute.ThisAssemblyProjectRootDirectory;

return await Bootstrapper
  .Factory
  .CreateWeb(args)
  .AddSetting(WebKeys.CachePath, $"{root}/obj/statiq-cache")
  .AddSetting(WebKeys.TempPath, $"{root}/obj/statiq-temp")
  .SetThemePath($"{root}/src/theme")
  .SetOutputPath($"{root}/bin/Cicd/wwwbin")
  .RunAsync();
